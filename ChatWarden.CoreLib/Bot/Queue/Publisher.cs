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

        public async Task PublishOrder(OrderBase order)
        {
            await _box.Call("add_order", TarantoolTuple.Create(order.Data));
        }

        internal async Task Add(byte[] bytes)
        {
            await _box.Call("add_order", TarantoolTuple.Create(bytes));
        }
    }
}
