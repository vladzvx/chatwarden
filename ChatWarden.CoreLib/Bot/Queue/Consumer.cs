using ChatWarden.CoreLib.Bot.Queue.Orders;
using Microsoft.Extensions.Hosting;
using ProGaudi.Tarantool.Client;
using Telegram.Bot;
using Telegram.Bot.Types;
using static ChatWarden.CoreLib.Bot.Queue.Orders.OrderBase;

namespace ChatWarden.CoreLib.Bot.Queue
{
    public class Consumer : ConsumerBase, IHostedService
    {
        private readonly ITelegramBotClient _telegramBotClient;
#pragma warning disable IDE0052 // Удалить непрочитанные закрытые члены
        private Task? _workingTask;
#pragma warning restore IDE0052 // Удалить непрочитанные закрытые члены
        private readonly CancellationTokenSource _cts;
        public Consumer(Box box, ITelegramBotClient telegramBotClient) : base(box)
        {
            _cts = new CancellationTokenSource();
            _telegramBotClient = telegramBotClient;
        }

        private async Task Worker(object cancellationToken)
        {
            if (cancellationToken is CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var (taskId, data) = await GetOrder();
                        if (data.Length > 0)
                        {
                            if (Enum.IsDefined(typeof(OrderType), data[0]))
                            {
                                try
                                {
                                    var type = (OrderType)data[0];
                                    switch (type)
                                    {
                                        case OrderType.SendTextMessage:
                                            {
                                                var ord = new SendTextMessageOrder(data);
                                                await _telegramBotClient.SendTextMessageAsync(ord.ChatId, ord.Text);
                                                break;
                                            }
                                        case OrderType.DeleteMessage:
                                            {
                                                var ord = new DeleteMessageOrder(data);
                                                await _telegramBotClient.DeleteMessageAsync(ord.ChatId, (int)ord.MessageNumber);
                                                break;
                                            }
                                        case OrderType.BanUserForTwoHours:
                                            {
                                                var ord = new SanctionOrder(data);
                                                await _telegramBotClient.BanChatMemberAsync(ord.ChatId, ord.UserId, DateTime.UtcNow.AddHours(2));
                                                break;
                                            }
                                        case OrderType.BanUserForever:
                                            {
                                                var ord = new SanctionOrder(data);
                                                await _telegramBotClient.BanChatMemberAsync(ord.ChatId, ord.UserId);
                                                break;
                                            }
                                        case OrderType.RestrictMedia:
                                            {
                                                var ord = new SanctionOrder(data);
                                                await _telegramBotClient.RestrictChatMemberAsync(ord.ChatId, (int)ord.UserId, new ChatPermissions() { CanSendMediaMessages = false }, DateTime.UtcNow.AddDays(7));
                                                break;
                                            }
                                        case OrderType.RestrictSendingDay:
                                            {
                                                var ord = new SanctionOrder(data);
                                                await _telegramBotClient.RestrictChatMemberAsync(ord.ChatId, (int)ord.UserId, new ChatPermissions() { CanSendMessages = false }, DateTime.UtcNow.AddDays(1));
                                                break;
                                            }
                                        case OrderType.RestrictSendingHour:
                                            {
                                                var ord = new SanctionOrder(data);
                                                await _telegramBotClient.RestrictChatMemberAsync(ord.ChatId, (int)ord.UserId, new ChatPermissions() { CanSendMessages = false }, DateTime.UtcNow.AddHours(1));
                                                break;
                                            }
                                        case OrderType.RestrictSendingWeek:
                                            {
                                                var ord = new SanctionOrder(data);
                                                await _telegramBotClient.RestrictChatMemberAsync(ord.ChatId, (int)ord.UserId, new ChatPermissions() { CanSendMessages = false }, DateTime.UtcNow.AddDays(7));
                                                break;
                                            }
                                    }
                                }
                                catch (Telegram.Bot.Exceptions.ApiRequestException apiEx)
                                {
                                    if (!apiEx.Message.StartsWith("Bad Request: can't restrict self") && !apiEx.Message.StartsWith("Bad Request: message to delete not found"))
                                    {
                                        await ReturnOrder(taskId);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        await AckOrder(taskId);

                    }
                    catch (Exception ex)
                    {

                    }
                    await Task.Delay(100);
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _workingTask = Worker(_cts.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}
