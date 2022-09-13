using static ChatWarden.CoreLib.Bot.Queue.Orders.OrderBase;

namespace ChatWarden.CoreLib.Extentions
{
    internal static class OrderTypeExtentions
    {
        public static string GetDurationText(this OrderType orderType)
        {
            return orderType switch
            {
                OrderType.RestrictMedia or OrderType.RestrictSendingWeek => "неделю",
                OrderType.RestrictSendingDay => "сутки",
                OrderType.RestrictSendingHour => "час",
                _ => string.Empty,
            };
        }
    }
}
