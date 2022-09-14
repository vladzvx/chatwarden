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
    public class UsersRepositoryTests
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
            var state = new UsersRepository(box);

            var user = PseudoUnicIdsGenerator.Get();
            var readedStatus1 = state?.GetUserStatus(user, _botid,_chatid).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == UserStatus.Unknown);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotOneUserOneIteration()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new UsersRepository(box);

            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            state?.SetUserStatus(user, _botid, _chatid, status).Wait();
            var readedStatus1 = state?.GetUserStatus(user, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotOneUserThreeIterations()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new UsersRepository(box);

            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            state?.SetUserStatus(user, _botid, _chatid, status).Wait();
            var readedStatus1 = state?.GetUserStatus(user, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            status = UserStatus.SuperAdmin;
            state?.SetUserStatus(user, _botid, _chatid, status).Wait();
            var readedStatus2 = state?.GetUserStatus(user, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status);

            status = UserStatus.Common;
            state?.SetUserStatus(user, _botid, _chatid, status).Wait();
            var readedStatus3 = state?.GetUserStatus(user, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus3);
            Assert.IsTrue(readedStatus3.Value == status);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotTwoUsersOneIteration()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new UsersRepository(box);

            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var status1 = UserStatus.Admin;
            var status2 = UserStatus.Common;

            state?.SetUserStatus(user1, _botid, _chatid, status1).Wait();
            var readedStatus1 = state?.GetUserStatus(user1, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status1);

            state?.SetUserStatus(user2, _botid, _chatid, status2).Wait();
            var readedStatus2 = state?.GetUserStatus(user2, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status2);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotTwoUsersTwoIterations()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new UsersRepository(box);

            var user1 = PseudoUnicIdsGenerator.Get();
            var user2 = PseudoUnicIdsGenerator.Get();
            var status1 = UserStatus.Admin;
            var status2 = UserStatus.Common;

            state?.SetUserStatus(user1, _botid, _chatid, status1).Wait();
            var readedStatus1 = state?.GetUserStatus(user1, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status1);

            state?.SetUserStatus(user2, _botid, _chatid, status2).Wait();
            var readedStatus2 = state?.GetUserStatus(user2, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus2);
            Assert.IsTrue(readedStatus2.Value == status2);

            status2 = UserStatus.SuperAdmin;
            state?.SetUserStatus(user2, _botid, _chatid, status2).Wait();
            var readedStatus3 = state?.GetUserStatus(user2, _botid, _chatid).Result;
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
            var state = new UsersRepository(box);

            //var localChat = PseudoUnicIdsGenerator.Get();
            var localBot = PseudoUnicIdsGenerator.Get();

            Assert.IsNotNull(box);
            var localState = new UsersRepository(box);
            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            var status2 = UserStatus.Common;
            state?.SetUserStatus(user, _botid, _chatid, status).Wait();
            var readedStatus1 = state?.GetUserStatus(user, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            localState.SetUserStatus(user, localBot, _chatid2, status2).Wait();
            var readedStatus2 = localState.GetUserStatus(user, localBot, _chatid2).Result;
            Assert.IsTrue(readedStatus2 == status2);
        }

        [TestMethod]
        public void SetUserStatus_GetUserStatus_OneBotTwoChatsOneUserOneIteration()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new UsersRepository(box);

            var localChat = PseudoUnicIdsGenerator.Get();
            var localBot = PseudoUnicIdsGenerator.Get();

            Assert.IsNotNull(box);
            var localState = new UsersRepository(box);
            var user = PseudoUnicIdsGenerator.Get();
            var status = UserStatus.Admin;
            var status2 = UserStatus.Common;
            state?.SetUserStatus(user, _botid, _chatid, status).Wait();
            var readedStatus1 = state?.GetUserStatus(user, _botid, _chatid).Result;
            Assert.IsNotNull(readedStatus1);
            Assert.IsTrue(readedStatus1.Value == status);

            localState.SetUserStatus(user, localBot, localChat, status2).Wait();
            var readedStatus2 = localState.GetUserStatus(user, localBot, localChat).Result;
            Assert.IsTrue(readedStatus2 == status2);
        }
    }
}