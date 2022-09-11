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

        internal async Task<UserStatus> GetUserStatus(long id)
        {
            var tmp = await _box.Call< TarantoolTuple<long, long, long>,byte[]>("get_status", TarantoolTuple.Create(id, BotId, ChatId));
            return (UserStatus)tmp.Data[0][0];
        }

        internal async Task SetUserStatus(long id, UserStatus userStatus)
        {
            await _box.Call("set_status", TarantoolTuple.Create(id, BotId, ChatId, new byte[1] { (byte)userStatus }));
        }

        internal async Task AddMessage(long userId, long chatId, long messageNumber, long time)
        {
            await _box.Call("add_message", TarantoolTuple.Create(userId, chatId, messageNumber, time));
        }

        internal async Task<byte[]> GetState(long botId, long chatId)
        {
            var tmp = await _box.Call< TarantoolTuple<long, long>,byte[]>("get_state", TarantoolTuple.Create(botId, chatId));
            return tmp.Data[0];
        }

        internal async Task SetState(long botId, long chatId, byte[] state)
        {
            await _box.Call<TarantoolTuple<long, long, byte[]>> ("set_state", TarantoolTuple.Create(botId, chatId, state));
        }

        internal async Task<string> GetHelp(long botId, long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, string>("get_help", TarantoolTuple.Create(botId, chatId));
            return tmp.Data[0];
        }

        internal async Task SetHelp(long botId, long chatId, string text)
        {
            await _box.Call<TarantoolTuple<long, long, string>>("set_help", TarantoolTuple.Create(botId, chatId, text));
        }

        internal async Task AddChat(long botId, long chatId, string helpText)
        {
            await _box.Call("add_chat", TarantoolTuple.Create(botId, chatId, new byte[] { (byte)BotStatus.Common }, helpText));
        }

        internal async Task<long[]> GetMessages(long userId, long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, long[]>("get_messages", TarantoolTuple.Create(userId, chatId));
            return tmp.Data[0];
        }
    }
}
