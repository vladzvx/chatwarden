using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatWarden.CoreLib.Bot.Queue.Orders.IOrder;

namespace ChatWarden.CoreLib.Bot.Queue.Orders
{
    public class DeleteMessageOrder : IOrder
    {
        public byte[] Data { get; private set; } = IOrder.EmptyData;
        //public OrderType Type => (OrderType)Data[0];
        public long ChatId => BitConverter.ToInt64(Data, 1);
        public long MessageNumber => BitConverter.ToInt64(Data, 9);

        public DeleteMessageOrder(byte[] data)
        {
            if (data.Length < 17) throw new ArgumentException("For DeleteMessageOrder byte[] data length must be > 16");
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
