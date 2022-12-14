namespace ChatWarden.CoreLib.Bot.Queue.Orders
{
    public class SanctionOrder : OrderBase
    {
        public long ChatId => BitConverter.ToInt64(Data, 1);
        public long UserId => BitConverter.ToInt64(Data, 9);

        public SanctionOrder(byte[] data)
        {
            if (data.Length < 17)
            {
                throw new ArgumentException("For SanctionOrder byte[] data length must be > 16");
            }

            Data = data;
        }

        public static byte[] CreateByteArray(long chatId, long userId, OrderType orderType)
        {
            var tmp = new List<byte>
            {
                (byte)orderType
            };
            tmp.AddRange(BitConverter.GetBytes(chatId));
            tmp.AddRange(BitConverter.GetBytes(userId));
            return tmp.ToArray();
        }
    }
}
