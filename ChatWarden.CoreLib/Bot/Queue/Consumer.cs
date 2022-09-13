using ChatWarden.CoreLib.Bot.Queue.Orders;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

namespace ChatWarden.CoreLib.Bot.Queue
{
    public class Consumer : QueueWorkerBase
    {
        public Consumer(Box box) : base(box)
        {
        }

        internal async Task<(long taskId,byte[] data)> GetOrder()
        {
            var tmp =await _box.Call<TarantoolTuple<long, string, byte[]>>("get_order");
            return (tmp.Data[0].Item1,tmp.Data[0].Item3);
        }

        internal async Task ReturnOrder(long id)
        {
            await _box.Call("return_order", TarantoolTuple.Create(id));
        }

        internal async Task AckOrder(long id)
        {
            await _box.Call("ack_order", TarantoolTuple.Create(id));
        }
    }
}
