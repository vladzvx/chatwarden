using ChatWarden.CoreLib.Bot.Queue.Orders;
using ChatWarden.CoreLib.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ChatWarden.CoreLib.Bot.Queue.Orders.OrderBase;

namespace ChatWarden.CoreLib.Tests
{
    [TestClass]
    public class OrdersTests
    {
        [TestMethod]
        public void DeleteMessageOrder_CreationTest()
        {
            long chatId = PseudoUnicIdsGenerator.Get();
            long messageNumber = PseudoUnicIdsGenerator.Get();
            var bytes = DeleteMessageOrder.CreateByteArray(chatId, messageNumber);
            var order = new DeleteMessageOrder(bytes);
            Assert.IsTrue(order.Type == OrderBase.OrderType.DeleteMessage);
            Assert.IsTrue(order.ChatId == chatId);
            Assert.IsTrue(order.MessageNumber == messageNumber);
        }

        [TestMethod]
        public void SanctionOrder_CreationTest()
        {
            long chatId = PseudoUnicIdsGenerator.Get();
            long userId = PseudoUnicIdsGenerator.Get();
            var type = OrderType.BanUserForever;
            var bytes = SanctionOrder.CreateByteArray(chatId, userId, type);
            var order = new SanctionOrder(bytes);
            Assert.IsTrue(order.Type == type);
            Assert.IsTrue(order.ChatId == chatId);
            Assert.IsTrue(order.UserId == userId);
        }

        [TestMethod]
        public void SendTextMessageOrder_CreationTest()
        {
            long chatId = PseudoUnicIdsGenerator.Get();
            var text = "qaaыыфывфЁ12ё11ё`1``~~~~";
            var bytes = SendTextMessageOrder.CreateByteArray(chatId, text);
            var order = new SendTextMessageOrder(bytes);
            Assert.IsTrue(order.Type == OrderType.SendTextMessage);
            Assert.IsTrue(order.ChatId == chatId);
            Assert.IsTrue(order.Text == text);
        }
    }
}