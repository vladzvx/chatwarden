using ChatWarden.CoreLib.Bot.Queue;
using ChatWarden.CoreLib.Bot.Queue.Orders;
using ChatWarden.CoreLib.Bot.UpdatesProcessing.Interfaces;
using ChatWarden.CoreLib.Extentions;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatWarden.CoreLib.Bot.UpdatesProcessing
{
    public class CustomUpdateHandler : ICustomUpdateHandler
    {
        private readonly Publisher _publisher;
        private readonly BotState _botState;
        private readonly UsersRepository _usersRepository;
        private readonly MessagesRepository _messagesRepository;
        public CustomUpdateHandler(Publisher publisher, BotState botState, UsersRepository usersRepository, MessagesRepository messagesRepository)
        {
            _publisher = publisher;
            _botState = botState;
            _usersRepository = usersRepository;
            _messagesRepository = messagesRepository;
        }

        public ReceiverOptions ReceiverOptions { get; private set; } = new ReceiverOptions()
        {
            AllowedUpdates = new UpdateType[] { UpdateType.Message }
        };

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if ((update != null && update.Type == UpdateType.Message && update.Message != null && (update.Message.Chat.Type == ChatType.Supergroup || update.Message.Chat.Type == ChatType.Group)) && botClient.BotId.HasValue && update.Message != null && update.Message.From != null)
                {
                    #region Инициализация общих переменных
                    var chatId = update.Message.Chat.Id;
                    var userId = update.Message.From.Id;
                    var botId = botClient.BotId.Value;
                    var messageNumber = update.Message.MessageId;
                    #endregion

                    #region Получение состояния, логирование сообщения
                    var status = await _botState.GetChatState(chatId);
                    await _messagesRepository.AddMessage(userId, messageNumber, DateTime.UtcNow.Ticks, chatId);
                    var senderStatus = await _usersRepository.GetUserStatus(userId, botId, chatId);
                    var mode = Enum.IsDefined(typeof(Mode), status[0]) ? (Mode)status[0] : Mode.Unknown;
                    if (mode == Mode.Unknown && update.Message!=null && update.Message.Text!=null && !update.Message.Text.Contains("/activate")) return;
                    #endregion

                    #region Зачистка сообщений от каналов
                    if (update.Message != null && update.Message.From != null && update.Message.From.Username == "Channel_Bot")
                    {
                        await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                    }
                    #endregion

                    #region Логирование пользователя вне режима набега
                    if (senderStatus == UserStatus.Unknown && mode != Mode.Overrun && update.Message != null && update.Message.From != null)
                    {
                        await _usersRepository.SetUserStatus(userId,botId,chatId, UserStatus.Common);
                    }
                    #endregion

                    #region Обработка режима набега
                    if (update.Message != null && update.Message.From!=null && update.Message.From.Id!= 777000 && mode == Mode.Overrun && senderStatus <= UserStatus.Common)
                    {
                        await SetRestrictOrder(update.Message, OrderBase.OrderType.BanUserForTwoHours,overrun:true);
                        return;
                    }
                    #endregion

                    #region Обработка команд на ограничения пользователей
                    
                    if (update.Message != null)
                    {
                        UserStatus targetStatus = UserStatus.Unknown;
                        if (update.Message.ReplyToMessage != null && update.Message.ReplyToMessage.From != null)
                        {
                            targetStatus = await _usersRepository.GetUserStatus(update.Message.ReplyToMessage.From.Id, botId, chatId);
                        }
                        if (senderStatus >= UserStatus.Admin && targetStatus < UserStatus.Priveleged)
                        {
                            switch (update.Message.Text)
                            {
                                case "-a":
                                case "-pr":
                                    if (update.Message.ReplyToMessage != null && update.Message.ReplyToMessage.From != null)
                                    {
                                        await _usersRepository.SetUserStatus(update.Message.ReplyToMessage.From.Id, botId, chatId, update.Message.Text == "-a" ? UserStatus.Admin : UserStatus.Priveleged);
                                    }
                                    await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                                    break;
                                case "-w":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.RestrictSendingWeek);
                                    break;
                                case "-d":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.RestrictSendingDay);
                                    break;
                                case "-h":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.RestrictSendingHour);
                                    break;
                                case "-m":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.RestrictMedia);
                                    break;
                                case "-b":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.BanUserForever);
                                    break;
                                case "-w-p":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.RestrictSendingWeek, true);
                                    break;
                                case "-h-p":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.RestrictSendingHour, true);
                                    break;
                                case "-d-p":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.RestrictSendingDay, true);
                                    break;
                                case "-m-p":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.RestrictMedia, true);
                                    break;
                                case "-b-p":
                                    await SetRestrictOrder(update.Message, OrderBase.OrderType.BanUserForever, true);
                                    break;
                            }
                            if (update.Message.Text != null && update.Message.Text.StartsWith("/ban"))
                            {
                                await SetRestrictOrder(update.Message, OrderBase.OrderType.BanUserForever, true);
                            }
                        }
                        //await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                        //return;
                    }
                    #endregion

                    #region Переключение режима набега и другие команды
                    if (update.Message != null && update.Message.Text != null)
                    {
                        if (update.Message.Text.Contains("/protection_mode_on"))
                        {
                            if (senderStatus >= UserStatus.Admin)
                            {
                                status[0] = (byte)Mode.Overrun;
                                await _botState.SetChatState(status,chatId);
                                await _publisher.Add(SendTextMessageOrder.CreateByteArray(update.Message.Chat.Id, "Активирован режим набега!"));
                            }
                            await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                            return;
                        }
                        else if (update.Message.Text.Contains("/protection_mode_off"))
                        {
                            if (senderStatus >= UserStatus.Admin)
                            {
                                status[0] = (byte)Mode.Common;
                                await _botState.SetChatState(status,chatId);
                                await _publisher.Add(SendTextMessageOrder.CreateByteArray(update.Message.Chat.Id, "Деактивирован режим набега!"));
                            }
                            await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                            return;
                        }
                        else if (update.Message.Text.Contains("/help"))
                        {
                            if (senderStatus >= UserStatus.Admin)
                            {
                                await _publisher.Add(SendTextMessageOrder.CreateByteArray(update.Message.Chat.Id, await _botState.GetHelp()));
                            }
                            await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                            return;
                        }
                        else if (update.Message.Text.Contains("/activate"))
                        {
                            var reg = new Regex(@"^/activate (.+)$");
                            var match = reg.Match(update.Message.Text);
                            if (match.Success)
                            {
                                var token = Environment.GetEnvironmentVariable("TOKEN");
                                if (token == match.Groups[1].Value)
                                {
                                    await _botState.AddChat(chatId);
                                    await _botState.SetChatState(new byte[] { (byte)Mode.Common}, chatId);
                                    await _usersRepository.SetUserStatus(userId, botId, chatId, UserStatus.SuperAdmin);
                                    await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
#pragma warning disable IDE0059 // Ненужное присваивание значения
            catch (Exception)
#pragma warning restore IDE0059 // Ненужное присваивание значения
            {

            }
        }
        private async Task SetRestrictOrder(Message message, OrderBase.OrderType orderType, bool notify = false,bool overrun = false)
        {
            if (overrun && message.From != null)
            {
                await _publisher.Add(SanctionOrder.CreateByteArray(message.Chat.Id, message.From.Id, orderType));
                var mesages = await _messagesRepository.GetMessages(message.From.Id, message.Chat.Id);
                foreach (long id in mesages)
                {
                    await _publisher.Add(DeleteMessageOrder.CreateByteArray(message.Chat.Id, id));
                }
            }
            else if (message.ReplyToMessage != null && message.ReplyToMessage.From != null)
            {
                #region Формирование команды
                await _publisher.Add(SanctionOrder.CreateByteArray(message.Chat.Id, message.ReplyToMessage.From.Id, orderType));
                if (orderType == OrderBase.OrderType.BanUserForever || orderType == OrderBase.OrderType.BanUserForTwoHours)
                {
                    var mesages = await _messagesRepository.GetMessages(message.ReplyToMessage.From.Id, message.Chat.Id);
                    foreach (long id in mesages)
                    {
                        await _publisher.Add(DeleteMessageOrder.CreateByteArray(message.Chat.Id, id));
                    }
                }
                else
                {
                    await _publisher.Add(DeleteMessageOrder.CreateByteArray(message.Chat.Id, message.ReplyToMessage.MessageId));
                }
                #endregion

                #region Отчёт в чат об исполнении
                if (notify)
                {
                    var duration = orderType.GetDurationText();
                    if (orderType == OrderBase.OrderType.BanUserForever)
                    {
                        var text = await _botState.GetRandomBanReplic();
                        await _publisher.Add(SendTextMessageOrder.CreateByteArray(message.Chat.Id, message.ReplyToMessage.From.FirstName + " " +
                            text));
                    }
                    else if (!string.IsNullOrEmpty(duration))
                    {
                        string? replicText = string.Empty;
                        if (orderType == OrderBase.OrderType.RestrictMedia)
                        {
                            replicText = await _botState.GetRandomMediaReplic();
                        }
                        else if (orderType == OrderBase.OrderType.RestrictSendingDay || orderType == OrderBase.OrderType.RestrictSendingHour || orderType == OrderBase.OrderType.RestrictSendingWeek)
                        {
                            replicText = await _botState.GetRandomRestrictReplic();
                        }

                        if (!string.IsNullOrEmpty(replicText))
                        {
                            await _publisher.Add(SendTextMessageOrder.CreateByteArray(message.Chat.Id, message.ReplyToMessage.From.FirstName + " " + replicText +" "+ duration + "."));
                        }
                    }
                }
                #endregion
            }
            await _publisher.Add(DeleteMessageOrder.CreateByteArray(message.Chat.Id, message.MessageId));
        }
    }
}
