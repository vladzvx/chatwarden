using ChatWarden.CoreLib.Bot.Queue.Orders;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

namespace ChatWarden.CoreLib.Bot.Queue
{
    public class Publisher : QueueWorkerBase
    {
        public Publisher(Box box) : base(box)
        {
        }

        public async Task PublishOrder(IOrder order)
        {
            await _box.Call("add_order", TarantoolTuple.Create(order.Data));
        }
    }
}
