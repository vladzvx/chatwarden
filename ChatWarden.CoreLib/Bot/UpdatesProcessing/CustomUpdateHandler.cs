using ChatWarden.CoreLib.Bot.Queue;
using ChatWarden.CoreLib.Bot.Queue.Orders;
using ChatWarden.CoreLib.Bot.UpdatesProcessing.Interfaces;
using ChatWarden.CoreLib.Extentions;
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
        public CustomUpdateHandler(Publisher publisher, BotState botState)
        {
            _publisher = publisher;
            _botState = botState;
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
                if (CheckUpdate(update) && botClient.BotId.HasValue && update.Message != null && update.Message.From != null)
                {
                    #region Получение состояния, логирование сообщения
                    var chatState = await _botState.GetChatState(botClient.BotId.Value, update.Message.Chat.Id);
                    await chatState.AddMessage(update.Message.From.Id, update.Message.MessageId, DateTime.UtcNow.Ticks);
                    var senderStatus = await chatState.GetUserStatus(update.Message.From.Id);
                    var status = await chatState.GetState();
                    var mode = Enum.IsDefined(typeof(Mode), status[0]) ? (Mode)status[0] : Mode.Common;
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
                        await chatState.SetUserStatus(update.Message.From.Id, UserStatus.Common);
                    }
                    #endregion

                    #region Обработка режима набега
                    if (update.Message != null && mode == Mode.Overrun && senderStatus < UserStatus.Common)
                    {
                        await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                        await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.BanUserForTwoHours);
                        return;
                    }
                    #endregion

                    #region Обработка команд на ограничения пользователей

                    if (update.Message != null && update.Message.ReplyToMessage != null && update.Message.ReplyToMessage.From != null)
                    {
                        var targetStatus = await chatState.GetUserStatus(update.Message.ReplyToMessage.From.Id);
                        if (senderStatus >= UserStatus.Admin && targetStatus < UserStatus.Priveleged)
                        {
                            switch (update.Message.Text ?? update.Message.Caption)
                            {
                                case "-a":
                                    await chatState.SetUserStatus(update.Message.ReplyToMessage.From.Id, UserStatus.Admin);
                                    break;
                                case "-pr":
                                    await chatState.SetUserStatus(update.Message.ReplyToMessage.From.Id, UserStatus.Priveleged);
                                    break;
                                case "-w":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.RestrictSendingWeek);
                                    break;
                                case "-d":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.RestrictSendingDay);
                                    break;
                                case "-h":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.RestrictSendingHour);
                                    break;
                                case "-m":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.RestrictMedia);
                                    break;
                                case "-b":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.BanUserForever);
                                    break;
                                case "-w-p":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.RestrictSendingWeek, true);
                                    break;
                                case "-h-p":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.RestrictSendingHour, true);
                                    break;
                                case "-d-p":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.RestrictSendingDay, true);
                                    break;
                                case "-m-p":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.RestrictMedia, true);
                                    break;
                                case "-b-p":
                                    await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.BanUserForever, true);
                                    break;
                            }
                            if (update.Message.Text != null && update.Message.Text.StartsWith("/ban"))
                            {
                                await SetRestrictOrder(chatState, update.Message, OrderBase.OrderType.BanUserForever, true);
                            }
                        }
                        await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                        return;
                    }
                    #endregion

                    #region Переключение режима набега
                    if (update.Message != null && update.Message.Text != null)
                    {
                        if (update.Message.Text.Contains("/protection_mode_on"))
                        {
                            if (senderStatus >= UserStatus.Admin)
                            {
                                status[0] = (byte)Mode.Overrun;
                                await chatState.SetState(status);
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
                                await chatState.SetState(status);
                                await _publisher.Add(SendTextMessageOrder.CreateByteArray(update.Message.Chat.Id, "Деактивирован режим набега!"));
                            }
                            await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                            return;
                        }
                        else if (update.Message.Text.Contains("/help"))
                        {
                            if (senderStatus >= UserStatus.Admin)
                            {
                                await _publisher.Add(SendTextMessageOrder.CreateByteArray(update.Message.Chat.Id, await chatState.GetHelp()));
                            }
                            await _publisher.Add(DeleteMessageOrder.CreateByteArray(update.Message.Chat.Id, update.Message.MessageId));
                            return;
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

        private async Task SetRestrictOrder(ChatState state, Message message, OrderBase.OrderType orderType, bool notify = false)
        {
            if (message.ReplyToMessage != null && message.ReplyToMessage.From != null)
            {
                #region Формирование команды
                await _publisher.Add(SanctionOrder.CreateByteArray(message.Chat.Id, message.ReplyToMessage.From.Id, orderType));
                if (orderType == OrderBase.OrderType.BanUserForever || orderType == OrderBase.OrderType.BanUserForTwoHours)
                {
                    var mesages = await state.GetMessages(message.ReplyToMessage.From.Id);
                    foreach (long id in mesages)
                    {
                        await _publisher.Add(DeleteMessageOrder.CreateByteArray(message.Chat.Id, id));
                    }
                }
                else
                {
                    await _publisher.Add(DeleteMessageOrder.CreateByteArray(message.Chat.Id, message.MessageId));
                }
                #endregion

                #region Отчёт в чат об исполнении
                if (notify)
                {
                    var duration = orderType.GetDurationText();
                    if (orderType == OrderBase.OrderType.BanUserForever)
                    {
                        var text = await state.GetRandomBanReplic();
                        await _publisher.Add(SendTextMessageOrder.CreateByteArray(message.Chat.Id, message.ReplyToMessage.From.FirstName + " " +
                            text));
                    }
                    else if (!string.IsNullOrEmpty(duration))
                    {
                        string? replicText = string.Empty;
                        if (orderType == OrderBase.OrderType.RestrictMedia)
                        {
                            replicText = await state.GetRandomMediaReplic();
                        }
                        else if (orderType == OrderBase.OrderType.RestrictSendingDay || orderType == OrderBase.OrderType.RestrictSendingHour || orderType == OrderBase.OrderType.RestrictSendingWeek)
                        {
                            replicText = await state.GetRandomRestrictReplic();
                        }

                        if (!string.IsNullOrEmpty(replicText))
                        {
                            await _publisher.Add(SendTextMessageOrder.CreateByteArray(message.Chat.Id, message.ReplyToMessage.From.FirstName + " " + duration + " " + replicText));
                        }
                    }
                }
                #endregion
            }
        }

        private static bool CheckUpdate(Update update)
        {
            return update != null && update.Type == UpdateType.Message && update.Message != null && (update.Message.Chat.Type == ChatType.Supergroup || update.Message.Chat.Type == ChatType.Group) && !(string.IsNullOrEmpty(update.Message.Text ?? update.Message.Caption)) || (update != null && update.Message != null && update.Message.NewChatMembers != null && update.Message.NewChatMembers.Length > 0);
        }
    }
}
