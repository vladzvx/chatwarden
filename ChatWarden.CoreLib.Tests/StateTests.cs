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
    public class StateTests
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
        public void GetUserStatus_NoUser()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var user = PseudoUnicIdsGenerator.Get();
            var readedStatus1 = state?.GetUserStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == UserStatus.Common);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotOneUserOneIteration()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state =  new State(box, _botid, _chatid);

            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            state?.SetUserStatus(user, status).Wait();
            var readedStatus1 = state?.GetUserStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotOneUserThreeIterations()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            state?.SetUserStatus(user, status).Wait();
            var readedStatus1 = state?.GetUserStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            status = UserStatus.SuperAdmin;
            state?.SetUserStatus(user, status).Wait();
            var readedStatus2 = state?.GetUserStatus(user).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status);

            status = UserStatus.Common;
            state?.SetUserStatus(user, status).Wait();
            var readedStatus3 = state?.GetUserStatus(user).Result;
            Assert.IsNotNull(readedStatus3);
            Assert.IsTrue(readedStatus3.Value == status);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotTwoUsersOneIteration()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var status1 = UserStatus.Admin;
            var status2 = UserStatus.Common;

            state?.SetUserStatus(user1, status1).Wait();
            var readedStatus1 = state?.GetUserStatus(user1).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status1);

            state?.SetUserStatus(user2, status2).Wait();
            var readedStatus2 = state?.GetUserStatus(user2).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status2);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotTwoUsersTwoIterations()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var status1 = UserStatus.Admin;
            var status2 = UserStatus.Common;

            state?.SetUserStatus(user1, status1).Wait();
            var readedStatus1 = state?.GetUserStatus(user1).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status1);

            state?.SetUserStatus(user2, status2).Wait();
            var readedStatus2 = state?.GetUserStatus(user2).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status2);

            status2 = UserStatus.SuperAdmin;
            state?.SetUserStatus(user2, status2).Wait();
            var readedStatus3 = state?.GetUserStatus(user2).Result;
            Assert.IsNotNull(readedStatus3);
            Assert.IsTrue(readedStatus3.Value == status2);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_TwoBotsOneChatOneUserOneIteration()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var _chatid2 = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            //var localChat = PseudoUnicIdsGenerator.Get();
            var localBot = PseudoUnicIdsGenerator.Get();

            Assert.IsNotNull(box);
            var localState = new State(box, localBot, _chatid2);
            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            var status2 = UserStatus.Common;
            state?.SetUserStatus(user, status).Wait();
            var readedStatus1 = state?.GetUserStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            localState.SetUserStatus(user, status2).Wait();
            var readedStatus2 = localState.GetUserStatus(user).Result;
            Assert.IsTrue(readedStatus2 == status2);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotTwoChatsOneUserOneIteration()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var localChat = PseudoUnicIdsGenerator.Get();
            var localBot = PseudoUnicIdsGenerator.Get();

            Assert.IsNotNull(box);
            var localState = new State(box, localBot, localChat);
            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            var status2 = UserStatus.Common;
            state?.SetUserStatus(user, status).Wait();
            var readedStatus1 = state?.GetUserStatus(user).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            localState.SetUserStatus(user, status2).Wait();
            var readedStatus2 = localState.GetUserStatus(user).Result;
            Assert.IsTrue(readedStatus2 == status2);
        }

        [TestMethod]
        public void AddChat_OneBotTwoChats()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();

            var chatid1 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();

            var state = new State(box, _botid, chatid1);
            var state2 = new State(box, _botid, chatid2);

            state?.AddChat("111").Wait();
            state2?.AddChat("111").Wait();
        }


        [TestMethod]
        public void GetBanReplic_AddBanReplic_OneBotOneChatTwoReplics()
        {
            Assert.IsNotNull(box);
            var botid = PseudoUnicIdsGenerator.Get();
            var chatid1 = PseudoUnicIdsGenerator.Get();
            var state = new State(box, botid, chatid1);

            var replic1 = "replic1";
            var replic2 = "replic2";
            state?.AddChat("111").Wait();
            var tmp = state?.GetBanReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 0);
            state?.AddBanReplic(replic1).Wait();
            tmp = state?.GetBanReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 1);
            Assert.IsTrue(tmp.Contains(replic1));
            state?.AddBanReplic(replic2).Wait();
            tmp = state?.GetBanReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 2);
            Assert.IsTrue(tmp.Contains(replic1));
            Assert.IsTrue(tmp.Contains(replic2));
        }

        [TestMethod]
        public void GetMediaReplic_AddMediaReplic_OneBotOneChatTwoReplics()
        {
            Assert.IsNotNull(box);
            var botid = PseudoUnicIdsGenerator.Get();
            var chatid1 = PseudoUnicIdsGenerator.Get();
            var state = new State(box, botid, chatid1);


            var replic1 = "replic1";
            var replic2 = "replic2";
            state?.AddChat("111").Wait();
            var tmp = state?.GetMediaReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 0);
            state?.AddMediaReplic(replic1).Wait();
            tmp = state?.GetMediaReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 1);
            Assert.IsTrue(tmp.Contains(replic1));
            state?.AddMediaReplic(replic2).Wait();
            tmp = state?.GetMediaReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 2);
            Assert.IsTrue(tmp.Contains(replic1));
            Assert.IsTrue(tmp.Contains(replic2));
            tmp = state?.GetBanReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 0);
        }

        [TestMethod]
        public void GetRestrictReplic_AddRestrictReplic_OneBotOneChatTwoReplics()
        {
            Assert.IsNotNull(box);
            var botid = PseudoUnicIdsGenerator.Get();
            var chatid1 = PseudoUnicIdsGenerator.Get();
            var state = new State(box, botid, chatid1);

            var replic1 = "replic1";
            var replic2 = "replic2";
            state?.AddChat("111").Wait();
            var tmp = state?.GetRestrictReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 0);
            state?.AddRestrictReplic(replic1).Wait();
            tmp = state?.GetRestrictReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 1);
            Assert.IsTrue(tmp.Contains(replic1));
            state?.AddRestrictReplic(replic2).Wait();
            tmp = state?.GetRestrictReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 2);
            Assert.IsTrue(tmp.Contains(replic1));
            Assert.IsTrue(tmp.Contains(replic2));
            tmp = state?.GetBanReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 0);
            tmp = state?.GetMediaReplics().Result;
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.Length == 0);
        }

        [TestMethod]
        public void SetHelp_GetHelp_OneBotOneChat()
        {
            Assert.IsNotNull(box);

            var text1 = "111";
            var text2 = "1asdaыфыффёёёЁЁёёёёё```11";
            var botid = PseudoUnicIdsGenerator.Get();
            var chatid1 = PseudoUnicIdsGenerator.Get();
            var state = new State(box, botid, chatid1);


            state?.AddChat(text1).Wait();
            var text = state?.GetHelp().Result;
            Assert.IsNotNull(text);
            Assert.IsTrue(text == text1);

            state?.SetHelp(text2).Wait();
            text = state?.GetHelp().Result;
            Assert.IsNotNull(text);
            Assert.IsTrue(text == text2);
        }

        [TestMethod]
        public void AddChat_OneBotTwoChats_ThrowsException()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var botid = PseudoUnicIdsGenerator.Get();

            var chatid2 = PseudoUnicIdsGenerator.Get();
            var state2 = new State(box, botid, chatid2);
            state?.AddChat("111").Wait();
            try
            {
                state?.AddChat("111").Wait();//todo добиться нормального поведения 
                Assert.Fail();
            }
            catch { }

            state2?.AddChat("111").Wait();
        }

        [TestMethod]
        public void AddChat_GetState_OneBotTwoChats_ThrowsException()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();


            var botid = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);
            var state2 = new State(box, botid, chatid2);

            state?.AddChat("111").Wait();
            try
            {
                state?.AddChat("111").Wait();
                Assert.Fail();
            }
            catch { }

            state2?.AddChat("111").Wait();
            var botState = state2?.GetState().Result;
            Assert.IsNotNull(botState);
            Assert.IsTrue(((Mode)botState[0]) == Mode.Common);
        }

        [TestMethod]
        public void AddChat_GetState_SetState_OneBotTwoChats_ThrowsException()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var botid = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var state2 = new State(box, botid, chatid2);
            state?.AddChat("111").Wait();
            try
            {
                state?.AddChat("111").Wait();
                Assert.Fail();
            }
            catch { }

            state2?.AddChat("111").Wait();
            var botState = state2?.GetState().Result;
            Assert.IsNotNull(botState);
            Assert.IsTrue(((Mode)botState[0]) == Mode.Common);
            botState[0] = (byte)Mode.Overrun;
            state2?.SetState(botState);

            botState = state?.GetState().Result;
            Assert.IsNotNull(botState);
            Assert.IsTrue(((Mode)botState[0]) == Mode.Common);

            botState = state2?.GetState().Result;
            Assert.IsNotNull(botState);
            Assert.IsTrue(((Mode)botState[0]) == Mode.Overrun);
        }

        [TestMethod]
        public void AddChat_GetHelp_OneBotTwoChats_ThrowsException()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var text = "asdsaas122``ыыы";
            var botid = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var state2 = new State(box, botid, chatid2);

            state?.AddChat(text).Wait();
            try
            {
                state?.AddChat(text).Wait();//todo поправить поведение
                Assert.Fail();
            }
            catch { }

            state2?.AddChat(text).Wait();
            var help = state?.GetHelp().Result;
            Assert.IsNotNull(help);
            Assert.IsTrue(help == text);
        }

        [TestMethod]
        public void AddMessage_GetMessagesTest_OneChatOneUser()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var user1 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11).Wait();
            state?.AddMessage(user1, number2, 11).Wait();
            state?.AddMessage(user1, number3, 11).Wait();
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
            var state = new State(box, _botid, _chatid);

            var user1 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11).Wait();
            state?.AddMessage(user1, number1, 11).Wait();
            state?.AddMessage(user1, number1, 11).Wait();
            state?.AddMessage(user1, number2, 11).Wait();
            state?.AddMessage(user1, number3, 11).Wait();
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
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var user1 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11).Wait();
            state?.AddMessage(user1, number2, 11).Wait();
            state?.AddMessage(user1, number3, 11).Wait();
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
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new State(box, _botid, _chatid);

            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();
            var number1 = 1L;
            var number2 = 2L;
            var number3 = 3L;
            state?.AddMessage(user1, number1, 11).Wait();
            state?.AddMessage(user2, number1, 11).Wait();
            state?.AddMessage(user1, number2, 11).Wait();
            state?.AddMessage(user1, number3, 11).Wait();
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
    }
}