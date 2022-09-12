using ChatWarden.CoreLib.Extentions;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

namespace ChatWarden.CoreLib.Bot
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

        public Mode Status => (Mode)GetState(BotId, ChatId).Result[0];
        public string HelpText => GetHelp(BotId, ChatId).Result;
        public string BanReplic => GetBanReplics(BotId, ChatId).Result.GetRandom();
        public string MediaReplic => GetMediaReplics(BotId, ChatId).Result.GetRandom();
        public string RestrictReplic => GetRestrictReplics(BotId, ChatId).Result.GetRandom();

        #region users
        internal async Task<UserStatus> GetUserStatus(long id)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long, long>, byte[]>("get_status", TarantoolTuple.Create(id, BotId, ChatId));
            return (UserStatus)tmp.Data[0][0];
        }

        internal async Task SetUserStatus(long id, UserStatus userStatus)
        {
            await _box.Call("set_status", TarantoolTuple.Create(id, BotId, ChatId, new byte[1] { (byte)userStatus }));
        }
        #endregion

        #region messages
        internal async Task AddMessage(long userId, long messageNumber, long time, long? chatId = null)
        {
            await _box.Call("add_message", TarantoolTuple.Create(userId, chatId??ChatId, messageNumber, time));
        }

        internal async Task<long[]> GetMessages(long userId, long? chatId = null)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, long[]>("get_messages", TarantoolTuple.Create(userId, chatId??ChatId));
            return tmp.Data[0];
        }
        #endregion

        #region chat profile settings
        internal async Task AddChat(string helpText)
        {
            await _box.Call("add_chat", TarantoolTuple.Create(BotId, ChatId, new byte[] { (byte)Mode.Common }, helpText));
        }

        internal async Task AddBanReplic(long botId, long chatId, string text)
        {
            await _box.Call("add_ban_replic", TarantoolTuple.Create(botId, chatId, text));
        }

        internal async Task<string[]> GetBanReplics(long botId, long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, string[]>("get_ban_replics", TarantoolTuple.Create(botId, chatId));
            return tmp.Data[0];
        }

        internal async Task<string> GetRandomBanReplic()
        {
            var tmp = await GetBanReplics(BotId, ChatId);
            return tmp.GetRandom();
        }

        internal async Task AddMediaReplic(long botId, long chatId, string text)
        {
            await _box.Call("add_media_replic", TarantoolTuple.Create(botId, chatId, text));
        }

        internal async Task<string[]> GetMediaReplics(long botId, long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, string[]>("get_media_replics", TarantoolTuple.Create(botId, chatId));
            return tmp.Data[0];
        }

        internal async Task AddRestrictReplic(long botId, long chatId, string text)
        {
            await _box.Call("add_restrict_replic", TarantoolTuple.Create(botId, chatId, text));
        }

        internal async Task<string[]> GetRestrictReplics(long botId, long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, string[]>("get_restrict_replics", TarantoolTuple.Create(botId, chatId));
            return tmp.Data[0];
        }

        internal async Task<byte[]> GetState(long botId, long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, byte[]>("get_state", TarantoolTuple.Create(botId, chatId));
            return tmp.Data[0];
        }

        internal async Task SetState(long botId, long chatId, byte[] state)
        {
            await _box.Call<TarantoolTuple<long, long, byte[]>>("set_state", TarantoolTuple.Create(botId, chatId, state));
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
        #endregion
    }
}
