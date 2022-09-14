using ChatWarden.CoreLib.Bot;
using ChatWarden.CoreLib.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using System;
using System.Linq;

namespace ChatWarden.CoreLib.Tests
{
    [TestClass]
    public class MessagesRepositoryTests
    {
        private static Box? box;
        [ClassInitialize]
#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
        public static void ClassInitialize(TestContext context)
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
        {
            TestEnvConfigurer.ReadEnvFile(".env");
            TestEnvConfigurer.SetTarantoolConnectionStringToEnvironment();
            var tnt = Environment.GetEnvironmentVariable("TARANTOOL_CNNSTR");
            box = new Box(new ClientOptions(tnt));
            box.Connect().Wait();
        }

        [TestMethod]
        public void AddMessage_GetMessagesTest_OneChatOneUser()
        {
            Assert.IsNotNull(box);
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new MessagesRepository(box);

            var user1 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11, _chatid).Wait();
            state?.AddMessage(user1, number2, 11, _chatid).Wait();
            state?.AddMessage(user1, number3, 11, _chatid).Wait();
            var q = state?.GetMessages(user1, _chatid).Result;
            Assert.IsNotNull(q);
            Assert.IsTrue(q.Length == 3);
            Assert.IsTrue(q.Contains(number1));
            Assert.IsTrue(q.Contains(number2));
            Assert.IsTrue(q.Contains(number3));
        }

        [TestMethod]
        public void AddMessage_GetMessagesTest_OneChatOneUserDuplicatedValues()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new MessagesRepository(box);

            var user1 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11, _chatid).Wait();
            state?.AddMessage(user1, number1, 11, _chatid).Wait();
            state?.AddMessage(user1, number1, 11, _chatid).Wait();
            state?.AddMessage(user1, number2, 11, _chatid).Wait();
            state?.AddMessage(user1, number3, 11, _chatid).Wait();
            var q = state?.GetMessages(user1, _chatid).Result;
            Assert.IsNotNull(q);
            Assert.IsTrue(q.Length == 5);
            Assert.IsTrue(q.Contains(number1));
            Assert.IsTrue(q.Count(item => item == number1) == 3);
            Assert.IsTrue(q.Contains(number2));
            Assert.IsTrue(q.Contains(number3));
        }

        [TestMethod]
        public void AddMessage_GetMessagesTest_TwoChatsOneUser()
        {
            Assert.IsNotNull(box);
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new MessagesRepository(box);

            var user1 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11, _chatid).Wait();
            state?.AddMessage(user1, number2, 11, _chatid).Wait();
            state?.AddMessage(user1, number3, 11, _chatid).Wait();
            state?.AddMessage(user1, number3, 11, chatid2).Wait();
            var q1 = state?.GetMessages(user1, _chatid).Result;
            var q2 = state?.GetMessages(user1, chatid2).Result;
            Assert.IsNotNull(q1);
            Assert.IsNotNull(q2);
            Assert.IsTrue(q1.Length == 3);
            Assert.IsTrue(q2.Length == 1);
            Assert.IsTrue(q1.Contains(number1));
            Assert.IsTrue(q1.Contains(number2));
            Assert.IsTrue(q1.Contains(number3));
            Assert.IsTrue(q2.Contains(number3));
        }

        [TestMethod]
        public void AddMessage_GetMessagesTest_TwoChatsTwoUsers()
        {
            Assert.IsNotNull(box);
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new MessagesRepository(box);

            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11, _chatid).Wait();
            state?.AddMessage(user2, number1, 11, _chatid).Wait();
            state?.AddMessage(user1, number2, 11, _chatid).Wait();
            state?.AddMessage(user1, number3, 11, _chatid).Wait();
            state?.AddMessage(user1, number3, 11, chatid2).Wait();
            var q1 = state?.GetMessages(user1, _chatid).Result;
            var q2 = state?.GetMessages(user1, chatid2).Result;
            var q3 = state?.GetMessages(user2, _chatid).Result;
            Assert.IsNotNull(q1);
            Assert.IsNotNull(q2);
            Assert.IsNotNull(q3);
            Assert.IsTrue(q1.Length == 3);
            Assert.IsTrue(q2.Length == 1);
            Assert.IsTrue(q3.Length == 1);
            Assert.IsTrue(q1.Contains(number1));
            Assert.IsTrue(q1.Contains(number2));
            Assert.IsTrue(q1.Contains(number3));
            Assert.IsTrue(q2.Contains(number3));
            Assert.IsTrue(q3.Contains(number1));
        }

        [TestMethod]
        public void AddMessage_DeleteMessage_GetMessagesTest_TwoChatsTwoUsers()
        {
            Assert.IsNotNull(box);
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new MessagesRepository(box);

            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11, _chatid).Wait();
            state?.AddMessage(user2, number1, 11, _chatid).Wait();
            state?.AddMessage(user1, number2, 11, _chatid).Wait();
            state?.AddMessage(user1, number3, 11, _chatid).Wait();
            state?.AddMessage(user1, number3, 11, chatid2).Wait();
            var q1 = state?.GetMessages(user1, _chatid).Result;
            var q2 = state?.GetMessages(user1, chatid2).Result;
            var q3 = state?.GetMessages(user2, _chatid).Result;

            Assert.IsNotNull(q1);
            Assert.IsNotNull(q2);
            Assert.IsNotNull(q3);
            Assert.IsTrue(q1.Length == 3);
            Assert.IsTrue(q2.Length == 1);
            Assert.IsTrue(q3.Length == 1);
            Assert.IsTrue(q1.Contains(number1));
            Assert.IsTrue(q1.Contains(number2));
            Assert.IsTrue(q1.Contains(number3));
            Assert.IsTrue(q2.Contains(number3));
            Assert.IsTrue(q3.Contains(number1));
            state?.DeleteMessage(user1, _chatid, number3).Wait();
            q1 = state?.GetMessages(user1, _chatid).Result;
            Assert.IsNotNull(q1);
            Assert.IsTrue(q1.Contains(number1));
            Assert.IsTrue(q1.Contains(number2));
            Assert.IsFalse(q1.Contains(number3));
        }
    }
}