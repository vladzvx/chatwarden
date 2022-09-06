using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

namespace ChatWarden.Bot.State
{
    public class State
    {
        public readonly long BotId;
        public readonly long ChatId;
        private readonly Box _box;

        public State(Box box, long botId, long chatId)
        {
            _box = box;
            BotId = botId;
            ChatId = chatId;
        }

        public BotStatus Status;
        public string HelpText;
        public long[] Administrators;
        public long[] Privileged;
        public string BanReplic;
        public string MediaReplic;
        public string RestrictReplic;

        internal async Task<UserStatus> GetStatus(long id)
        {
            var tmp = await _box.Call< TarantoolTuple<long, long, long>,byte[]>("get_status", TarantoolTuple.Create(id, BotId, ChatId));
            return (UserStatus)tmp.Data[0][0];
        }

        internal async Task SetStatus(long id, UserStatus userStatus)
        {
            await _box.Call("set_status", TarantoolTuple.Create(id, BotId, ChatId, new byte[1] { (byte)userStatus }));
        }

        internal async Task AddMessage(long userId, long chatId, long messageNumber, long time)
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
