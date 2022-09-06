using ChatWarden.Bot.State;
using ChatWarden.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using System;
using System.Linq;

namespace ChatWarden.Tests
{
    [TestClass]
    public class StateTests
    {
        static State? state;
        static Box? box;
        static long chatId;
        static long botId;
        [ClassInitialize]
#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
        public static void ClassInitialize(TestContext context)
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
        {
            chatId = PseudoUnicIdsGenerator.Get();
            botId = PseudoUnicIdsGenerator.Get();
            TestEnvConfigurer.ReadEnvFile(".env");
            TestEnvConfigurer.SetTarantoolConnectionStringToEnvironment();
            var tnt = Environment.GetEnvironmentVariable("TARANTOOL_CNNSTR");
            box = new Box(new ClientOptions(tnt));
            box.Connect().Wait();
            state = new State(box, botId, chatId);
        }

        [TestMethod]
        public void GetStatus_NoUser()
        {
            var user = PseudoUnicIdsGenerator.Get();
            var readedStatus1 = state?.GetStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == UserStatus.Common);
        }

        [TestMethod]
        public void SetStatus_GetStatus_OneBotOneUserOneIteration()
        {
            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            state?.SetStatus(user, status).Wait();
            var readedStatus1 = state?.GetStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);
        }

        [TestMethod]
        public void SetStatus_GetStatus_OneBotOneUserThreeIterations()
        {
            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            state?.SetStatus(user, status).Wait();
            var readedStatus1 = state?.GetStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            status = UserStatus.SuperAdmin;
            state?.SetStatus(user, status).Wait();
            var readedStatus2 = state?.GetStatus(user).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status);

            status = UserStatus.Common;
            state?.SetStatus(user, status).Wait();
            var readedStatus3 = state?.GetStatus(user).Result;
            Assert.IsNotNull(readedStatus3);
            Assert.IsTrue(readedStatus3.Value == status);
        }

        [TestMethod]
        public void SetStatus_GetStatus_OneBotTwoUsersOneIteration()
        {
            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var status1 = UserStatus.Admin;
            var status2 = UserStatus.Common;

            state?.SetStatus(user1, status1).Wait();
            var readedStatus1 = state?.GetStatus(user1).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status1);

            state?.SetStatus(user2, status2).Wait();
            var readedStatus2 = state?.GetStatus(user2).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status2);
        }

        [TestMethod]
        public void SetStatus_GetStatus_OneBotTwoUsersTwoIterations()
        {
            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var status1 = UserStatus.Admin;
            var status2 = UserStatus.Common;

            state?.SetStatus(user1, status1).Wait();
            var readedStatus1 = state?.GetStatus(user1).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status1);

            state?.SetStatus(user2, status2).Wait();
            var readedStatus2 = state?.GetStatus(user2).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status2);

            status2 = UserStatus.SuperAdmin;
            state?.SetStatus(user2, status2).Wait();
            var readedStatus3 = state?.GetStatus(user2).Result;
            Assert.IsNotNull(readedStatus3);
            Assert.IsTrue(readedStatus3.Value == status2);
        }

        [TestMethod]
        public void SetStatus_GetStatus_TwoBotsOneChatOneUserOneIteration()
        {
            //var localChat = PseudoUnicIdsGenerator.Get();
            var localBot = PseudoUnicIdsGenerator.Get();

            Assert.IsNotNull(box);
            var localState = new State(box, localBot, chatId);
            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            var status2 = UserStatus.Common;
            state?.SetStatus(user, status).Wait();
            var readedStatus1 = state?.GetStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            localState.SetStatus(user, status2).Wait();
            var readedStatus2 = localState.GetStatus(user).Result;
            Assert.IsTrue(readedStatus2 == status2);
        }

        [TestMethod]
        public void SetStatus_GetStatus_OneBotTwoChatsOneUserOneIteration()
        {
            var localChat = PseudoUnicIdsGenerator.Get();
            //var localBot = PseudoUnicIdsGenerator.Get();

            Assert.IsNotNull(box);
            var localState = new State(box, botId, localChat);
            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            var status2 = UserStatus.Common;
            state?.SetStatus(user, status).Wait();
            var readedStatus1 = state?.GetStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            localState.SetStatus(user, status2).Wait();
            var readedStatus2 = localState.GetStatus(user).Result;
            Assert.IsTrue(readedStatus2 == status2);
        }

        [TestMethod]
        public void AddMessage_GetMessagesTest_OneChatOneUser()
        {
            var user1 = PseudoUnicIdsGenerator.Get();
            var chatid = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, chatid, number1, 11).Wait();
            state?.AddMessage(user1, chatid, number2, 11).Wait();
            state?.AddMessage(user1, chatid, number3, 11).Wait();
            var q = state?.GetMessages(user1, chatid).Result;
            Assert.IsNotNull(q);
            Assert.IsTrue(q.Length == 3);
            Assert.IsTrue(q.Contains(number1));
            Assert.IsTrue(q.Contains(number2));
            Assert.IsTrue(q.Contains(number3));
        }

        [TestMethod]
        public void AddMessage_GetMessagesTest_OneChatOneUserDuplicatedValues()
        {
            var user1 = PseudoUnicIdsGenerator.Get();
            var chatid = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, chatid, number1, 11).Wait();
            state?.AddMessage(user1, chatid, number1, 11).Wait();
            state?.AddMessage(user1, chatid, number1, 11).Wait();
            state?.AddMessage(user1, chatid, number2, 11).Wait();
            state?.AddMessage(user1, chatid, number3, 11).Wait();
            var q = state?.GetMessages(user1, chatid).Result;
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
            var user1 = PseudoUnicIdsGenerator.Get();
            var chatid1 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, chatid1, number1, 11).Wait();
            state?.AddMessage(user1, chatid1, number2, 11).Wait();
            state?.AddMessage(user1, chatid1, number3, 11).Wait();
            state?.AddMessage(user1, chatid2, number3, 11).Wait();
            var q1 = state?.GetMessages(user1, chatid1).Result;
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
            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var chatid1 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, chatid1, number1, 11).Wait();
            state?.AddMessage(user2, chatid1, number1, 11).Wait();
            state?.AddMessage(user1, chatid1, number2, 11).Wait();
            state?.AddMessage(user1, chatid1, number3, 11).Wait();
            state?.AddMessage(user1, chatid2, number3, 11).Wait();
            var q1 = state?.GetMessages(user1, chatid1).Result;
            var q2 = state?.GetMessages(user1, chatid2).Result;
            var q3 = state?.GetMessages(user2, chatid1).Result;
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
    }
}