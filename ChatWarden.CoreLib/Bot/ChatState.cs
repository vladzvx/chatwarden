using ChatWarden.CoreLib.Extentions;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

namespace ChatWarden.CoreLib.Bot
{
    public class ChatState
    {
        public readonly struct StateId
        {
            public readonly long BotId;
            public readonly long ChatId;
            public StateId(long botId, long chatId)
            {
                BotId = botId;
                ChatId = chatId;
            }
        }

        public readonly StateId Id;
        public ValueTask<Mode> Status => GetMode();
        public Task<string> HelpText => GetHelp();
        public Task<string> BanReplic => GetRandomBanReplic();
        public Task<string> MediaReplic => GetRandomMediaReplic();
        public Task<string> RestrictReplic => GetRandomRestrictReplic();

        private readonly Box _box;

        public ChatState(Box box, long botId, long chatId)
        {
            _box = box;
            Id = new StateId(botId, chatId);
        }

        #region users
        internal async Task<UserStatus> GetUserStatus(long id)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long, long>, byte[]>("get_status", TarantoolTuple.Create(id, Id.BotId, Id.ChatId));
            return (UserStatus)tmp.Data[0][0];
        }

        internal async Task SetUserStatus(long id, UserStatus userStatus)
        {
            await _box.Call("set_status", TarantoolTuple.Create(id, Id.BotId, Id.ChatId, new byte[1] { (byte)userStatus }));
        }
        #endregion

        #region messages
        internal async Task AddMessage(long userId, long messageNumber, long time, long? chatId = null)
        {
            await _box.Call("add_message", TarantoolTuple.Create(userId, chatId ?? Id.ChatId, messageNumber, time));
        }

        internal async Task<long[]> GetMessages(long userId, long? chatId = null)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, long[]>("get_messages", TarantoolTuple.Create(userId, chatId ?? Id.ChatId));
            return tmp.Data[0];
        }
        #endregion

        #region chat profile settings
        internal async ValueTask<Mode> GetMode()
        {
            var tmp = await GetState();
            if (tmp.Length > 0 && Enum.IsDefined(typeof(Mode), tmp[0]))
            {
                return (Mode)tmp[0];
            }
            else
            {
                return Mode.Common;
            }
        }

        internal async Task AddChat(string helpText)
        {
            await _box.Call("add_chat", TarantoolTuple.Create(Id.BotId, Id.ChatId, new byte[] { (byte)Mode.Common }, helpText));
        }

        internal async Task AddBanReplic(string text)
        {
            await _box.Call("add_ban_replic", TarantoolTuple.Create(Id.BotId, Id.ChatId, text));
        }

        internal async Task<string[]> GetBanReplics()
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, string[]>("get_ban_replics", TarantoolTuple.Create(Id.BotId, Id.ChatId));
            return tmp.Data[0];
        }

        internal async Task<string> GetRandomBanReplic()
        {
            var tmp = await GetBanReplics();
            return tmp.GetRandom();
        }

        internal async Task AddMediaReplic(string text)
        {
            await _box.Call("add_media_replic", TarantoolTuple.Create(Id.BotId, Id.ChatId, text));
        }

        internal async Task<string[]> GetMediaReplics()
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, string[]>("get_media_replics", TarantoolTuple.Create(Id.BotId, Id.ChatId));
            return tmp.Data[0];
        }

        internal async Task<string> GetRandomMediaReplic()
        {
            var tmp = await GetMediaReplics();
            return tmp.GetRandom();
        }

        internal async Task AddRestrictReplic(string text)
        {
            await _box.Call("add_restrict_replic", TarantoolTuple.Create(Id.BotId, Id.ChatId, text));
        }

        internal async Task<string[]> GetRestrictReplics()
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, string[]>("get_restrict_replics", TarantoolTuple.Create(Id.BotId, Id.ChatId));
            return tmp.Data[0];
        }

        internal async Task<string> GetRandomRestrictReplic()
        {
            var tmp = await GetRestrictReplics();
            return tmp.GetRandom();
        }

        internal async Task<byte[]> GetState()
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, byte[]>("get_state", TarantoolTuple.Create(Id.BotId, Id.ChatId));
            return tmp.Data[0];
        }

        internal async Task SetState(byte[] state)
        {
            await _box.Call<TarantoolTuple<long, long, byte[]>>("set_state", TarantoolTuple.Create(Id.BotId, Id.ChatId, state));
        }

        internal async Task<string> GetHelp()
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, string>("get_help", TarantoolTuple.Create(Id.BotId, Id.ChatId));
            return tmp.Data[0];
        }

        internal async Task SetHelp(string text)
        {
            await _box.Call<TarantoolTuple<long, long, string>>("set_help", TarantoolTuple.Create(Id.BotId, Id.ChatId, text));
        }
        #endregion
    }
}
