using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

namespace ChatWarden.CoreLib.Bot
{
    public class UsersRepository
    {

        private readonly Box _box;

        public UsersRepository(Box box)
        {
            _box = box;
        }

        internal async Task<UserStatus> GetUserStatus(long id, long botId, long chatId)
        {
            var tmp = await _box.Call<TarantoolTuple<long, long, long>, byte[]>("get_status", TarantoolTuple.Create(id, botId, chatId));
            return (UserStatus)tmp.Data[0][0];
        }

        internal async Task SetUserStatus(long id, long botId, long chatId, UserStatus userStatus)
        {
            await _box.Call("set_status", TarantoolTuple.Create(id, botId, chatId, new byte[1] { (byte)userStatus }));
        }
    }
}
