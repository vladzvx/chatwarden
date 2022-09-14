using ChatWarden.CoreLib.Extentions;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

namespace ChatWarden.CoreLib.Bot
{
    public class MessagesRepository
    {
        private readonly Box _box;

        public MessagesRepository(Box box)
        {
            _box = box;
        }

        internal async Task DeleteMessage(long userId, long chatId, long messageNumber)
        {
            await _box.Call("del_message", TarantoolTuple.Create(userId, chatId, messageNumber));
        }

        internal async Task AddMessage(long userId, long messageNumber, long time, long chatId)
        {
            await _box.Call("add_message", TarantoolTuple.Create(userId, chatId, messageNumber, time));
        }

        internal async Task<long[]> GetMessages(long userId, long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, long[]>("get_messages", TarantoolTuple.Create(userId, chatId));
            return tmp.Data[0];
        }
    }
}
