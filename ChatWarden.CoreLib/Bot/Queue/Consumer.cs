using ChatWarden.CoreLib.Bot.Queue.Orders;
using Microsoft.Extensions.Hosting;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using Telegram.Bot;
using Telegram.Bot.Types;
using static ChatWarden.CoreLib.Bot.Queue.Orders.OrderBase;

namespace ChatWarden.CoreLib.Bot.Queue
{
    public class Consumer : QueueWorkerBase, IHostedService
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private Task? _workingTask;
        private readonly CancellationTokenSource _cts;
        public Consumer(Box box, ITelegramBotClient telegramBotClient) : base(box)
        {
            _cts = new CancellationTokenSource();
            _telegramBotClient = telegramBotClient;
        }

        private async Task worker(object cancellationToken)
        {
            if (cancellationToken is CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var order = await GetOrder();
                        if (order.data.Length > 0)
                        {
                            if (Enum.IsDefined(typeof(OrderType), order.data[0]))
                            {
                                var type = (OrderType)order.data[0];
                                switch (type)
                                {
                                    case OrderType.SendTextMessage:
                                        {
                                            var ord = new SendTextMessageOrder(order.data);
                                            await _telegramBotClient.SendTextMessageAsync(ord.ChatId, ord.Text);
                                            break;
                                        }
                                    case OrderType.DeleteMessage:
                                        {
                                            var ord = new DeleteMessageOrder(order.data);
                                            await _telegramBotClient.DeleteMessageAsync(ord.ChatId, (int)ord.MessageNumber);
                                            break;
                                        }
                                    case OrderType.BanUserForTwoHours:
                                        {
                                            var ord = new SanctionOrder(order.data);
                                            await _telegramBotClient.BanChatMemberAsync(ord.ChatId, (int)ord.UserId, DateTime.UtcNow.AddHours(2));
                                            break;
                                        }
                                    case OrderType.BanUserForever:
                                        {
                                            var ord = new SanctionOrder(order.data);
                                            await _telegramBotClient.BanChatMemberAsync(ord.ChatId, (int)ord.UserId);
                                            break;
                                        }
                                    case OrderType.RestrictMedia:
                                        {
                                            var ord = new SanctionOrder(order.data);
                                            await _telegramBotClient.RestrictChatMemberAsync(ord.ChatId, (int)ord.UserId, new ChatPermissions() { CanSendMediaMessages = true }, DateTime.UtcNow.AddDays(7));
                                            break;
                                        }
                                    case OrderType.RestrictSendingDay:
                                        {
                                            var ord = new SanctionOrder(order.data);
                                            await _telegramBotClient.RestrictChatMemberAsync(ord.ChatId, (int)ord.UserId, new ChatPermissions() { CanSendMessages = true }, DateTime.UtcNow.AddDays(1));
                                            break;
                                        }
                                    case OrderType.RestrictSendingHour:
                                        {
                                            var ord = new SanctionOrder(order.data);
                                            await _telegramBotClient.RestrictChatMemberAsync(ord.ChatId, (int)ord.UserId, new ChatPermissions() { CanSendMessages = true }, DateTime.UtcNow.AddHours(1));
                                            break;
                                        }
                                    case OrderType.RestrictSendingWeek:
                                        {
                                            var ord = new SanctionOrder(order.data);
                                            await _telegramBotClient.RestrictChatMemberAsync(ord.ChatId, (int)ord.UserId, new ChatPermissions() { CanSendMessages = true }, DateTime.UtcNow.AddDays(7));
                                            break;
                                        }

                                }
                            }
                        }
                        await AckOrder(order.taskId);

                    }
                    catch (Exception)
                    {

                    }
                    await Task.Delay(100);
                }
            }
        }

        internal async Task<(long taskId, byte[] data)> GetOrder()
        {
            var tmp = await _box.Call<TarantoolTuple<long, string, byte[]>>("get_order");
            return (tmp.Data[0].Item1, tmp.Data[0].Item3);
        }

        internal async Task ReturnOrder(long id)
        {
            await _box.Call("return_order", TarantoolTuple.Create(id));
        }

        internal async Task AckOrder(long id)
        {
            await _box.Call("ack_order", TarantoolTuple.Create(id));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _workingTask = worker(_cts.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}
