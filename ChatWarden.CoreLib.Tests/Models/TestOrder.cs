using ChatWarden.CoreLib.Bot.Queue.Orders;

namespace ChatWarden.CoreLib.Tests.Models
{
    public class TestOrder : OrderBase
    {
        public TestOrder(byte[] data)
        {
            Data = data;
        }
    }
}
