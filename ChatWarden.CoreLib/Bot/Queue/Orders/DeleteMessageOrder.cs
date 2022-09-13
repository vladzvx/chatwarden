namespace ChatWarden.CoreLib.Bot.Queue.Orders
{
    public class DeleteMessageOrder : OrderBase
    {
        public long ChatId => BitConverter.ToInt64(Data, 1);
        public long MessageNumber => BitConverter.ToInt64(Data, 9);

        public DeleteMessageOrder(byte[] data)
        {
            if (data.Length < 17)
            {
                throw new ArgumentException("For DeleteMessageOrder byte[] data length must be > 16");
            }

            Data = data;
        }

        public static byte[] CreateByteArray(long chatId, long MessageNumber)
        {
            var tmp = new List<byte>
            {
                (byte)OrderType.DeleteMessage
            };
            tmp.AddRange(BitConverter.GetBytes(chatId));
            tmp.AddRange(BitConverter.GetBytes(MessageNumber));
            return tmp.ToArray();
        }
    }
}
