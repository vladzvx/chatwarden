using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatWarden.CoreLib.Bot.Queue.Orders.IOrder;

namespace ChatWarden.CoreLib.Bot.Queue.Orders
{
    public class SendTextMessageOrder : IOrder
    {
        public byte[] Data { get; private set; } = IOrder.EmptyData;
        //public OrderType Type => (OrderType)Data[0];
        public long ChatId => BitConverter.ToInt64(Data, 1);
        public string Text => Encoding.Unicode.GetString(Data, 9, Data.Length - 9);

        public SendTextMessageOrder(byte[] data)
        {
            if (data.Length < 11) throw new ArgumentException("For SendTextMessageOrder byte[] data length must be > 10");
            Data = data;
        }

        public static byte[] CreateByteArray(long chatId,string text)
        {
            var tmp = new List<byte>
            {
                (byte)OrderType.SendMessage
            };
            tmp.AddRange(BitConverter.GetBytes(chatId));
            tmp.AddRange(Encoding.Unicode.GetBytes(text));
            return tmp.ToArray();
        }
    }
}
