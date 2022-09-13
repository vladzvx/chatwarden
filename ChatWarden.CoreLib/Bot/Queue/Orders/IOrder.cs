using ProGaudi.Tarantool.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatWarden.CoreLib.Bot.Queue.Orders
{
    public interface IOrder
    {
        internal static readonly byte[] EmptyData = new byte[1] { 0 };
        public enum OrderType : byte
        {
            Empty = 0,

            DeleteMessage = 1,
            SendMessage = 2,

            BanUserForever = 20,
            BanUserForTwoHours = 21,
            RestrictMedia = 22,
            RestrictSendingHour = 23,
            RestrictSendingDay = 24,
            RestrictSendingWeek = 25,
        }
        public byte[] Data { get; }
        public OrderType Type => (OrderType)Data[0];
    }
}
