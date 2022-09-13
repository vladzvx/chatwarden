namespace ChatWarden.CoreLib.Bot.Queue.Orders
{
    public abstract class OrderBase
    {
        internal static readonly byte[] EmptyData = new byte[1] { 0 };
        public enum OrderType : byte
        {
            Empty = 0,

            DeleteMessage = 1,
            SendTextMessage = 20,

            BanUserForever = 40,
            BanUserForTwoHours = 41,
            RestrictMedia = 42,
            RestrictSendingHour = 43,
            RestrictSendingDay = 44,
            RestrictSendingWeek = 45,
        }
        public byte[] Data { get; protected set; } = EmptyData;
        public OrderType Type => (OrderType)Data[0];
    }
}
