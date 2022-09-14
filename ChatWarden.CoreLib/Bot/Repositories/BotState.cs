using ChatWarden.CoreLib.Extentions;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using Telegram.Bot;

namespace ChatWarden.CoreLib.Bot
{
    public class BotState
    {
        private readonly long BotId;
        private readonly Box _box;
        internal BotState(Box box, long botId)
        {
            _box = box;
            BotId = botId;
            try
            {
                _box.Call("add_bot", TarantoolTuple.Create(BotId)).Wait();
            }
            catch { }
        }

        public BotState(Box box, ITelegramBotClient botClient)
        {
            _box = box;
            if (botClient.BotId.HasValue)
            {
                BotId = botClient.BotId.Value;
            }
            else
            {
                throw new ApplicationException("Failed init BotState: no id");
            }

            try
            {
                _box.Call("add_bot", TarantoolTuple.Create(BotId)).Wait();
            }
            catch { }
        }

        internal async Task<byte[]> GetChatState(long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long>, byte[]>("get_state", TarantoolTuple.Create(BotId, chatId));
            return tmp.Data[0];
        }

        internal async Task SetChatState(byte[] state, long chatId)
        {
            await _box.Call<TarantoolTuple<long, long, byte[]>>("set_state", TarantoolTuple.Create(BotId, chatId, state));
        }

        internal async Task AddChat(long chatId)
        {
            await _box.Call("add_chat", TarantoolTuple.Create(BotId, chatId, new byte[] { (byte)Mode.Common }));
        }

        public async Task AddBanReplic(string text)
        {
            await _box.Call("add_ban_replic", TarantoolTuple.Create(BotId, text));
        }

        internal async Task<string[]> GetBanReplics()
        {
            var tmp = await _box.Call<TarantoolTuple<long>, string[]>("get_ban_replics", TarantoolTuple.Create(BotId));
            return tmp.Data[0];
        }

        internal async Task<string> GetRandomBanReplic()
        {
            var tmp = await GetBanReplics();
            return tmp.GetRandom();
        }

        public async Task AddMediaReplic(string text)
        {
            await _box.Call("add_media_replic", TarantoolTuple.Create(BotId, text));
        }

        internal async Task<string[]> GetMediaReplics()
        {
            var tmp = await _box.Call<TarantoolTuple<long>, string[]>("get_media_replics", TarantoolTuple.Create(BotId));
            return tmp.Data[0];
        }

        internal async Task<string> GetRandomMediaReplic()
        {
            var tmp = await GetMediaReplics();
            return tmp.GetRandom();
        }

        public async Task AddRestrictReplic(string text)
        {
            await _box.Call("add_restrict_replic", TarantoolTuple.Create(BotId, text));
        }

        internal async Task<string[]> GetRestrictReplics()
        {
            var tmp = await _box.Call<TarantoolTuple<long>, string[]>("get_restrict_replics", TarantoolTuple.Create(BotId));
            return tmp.Data[0];
        }

        internal async Task<string> GetRandomRestrictReplic()
        {
            var tmp = await GetRestrictReplics();
            return tmp.GetRandom();
        }

        internal async Task<string> GetHelp()
        {
            var tmp = await _box.Call<TarantoolTuple<long>, string>("get_help", TarantoolTuple.Create(BotId));
            return tmp.Data[0];
        }

        internal async Task SetHelp(string text)
        {
            await _box.Call<TarantoolTuple<long, string>>("set_help", TarantoolTuple.Create(BotId, text));
        }
    }
}
