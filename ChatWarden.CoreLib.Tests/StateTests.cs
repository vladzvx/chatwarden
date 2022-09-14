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
        public void AddChat_OneBotTwoChats()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();

            var chatid1 = PseudoUnicIdsGenerator.Get();
            var chatid2 = PseudoUnicIdsGenerator.Get();

            var state = new BotState(box, _botid);

            state?.AddChat(chatid1).Wait();
            state?.AddChat(chatid2).Wait();
        }


        [TestMethod]
        public void GetBanReplic_AddBanReplic_OneBotOneChatTwoReplics()
        {
            Assert.IsNotNull(box);
            var botid = PseudoUnicIdsGenerator.Get();
            var state = new BotState(box, botid);

            var replic1 = "replic1";
            var replic2 = "replic2";

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
            var state = new BotState(box, botid);


            var replic1 = "replic1";
            var replic2 = "replic2";

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
            var state = new BotState(box, botid);

            var replic1 = "replic1";
            var replic2 = "replic2";

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
            var state = new BotState(box, botid);


            state?.SetHelp(text1).Wait();
            var text = state?.GetHelp().Result;
            Assert.IsNotNull(text);
            Assert.IsTrue(text == text1);

            state?.SetHelp(text2).Wait();
            text = state?.GetHelp().Result;
            Assert.IsNotNull(text);
            Assert.IsTrue(text == text2);
        }

        [TestMethod]
        public void AddChat_GetState_OneBotTwoChats()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();

            var chatid2 = PseudoUnicIdsGenerator.Get();
            var state = new BotState(box, _botid);

            state?.AddChat(_chatid).Wait();

            state?.AddChat(chatid2).Wait();
            var botState = state?.GetChatState(chatid2).Result;
            Assert.IsNotNull(botState);
            Assert.IsTrue(((Mode)botState[0]) == Mode.Common);
        }

        [TestMethod]
        public void AddChat_GetState_SetState_OneBotTwoChats()
        {
            Assert.IsNotNull(box);
            var _botid = PseudoUnicIdsGenerator.Get();
            var _chatid = PseudoUnicIdsGenerator.Get();
            var state = new BotState(box, _botid);

            var chatid2 = PseudoUnicIdsGenerator.Get();
            state?.AddChat(_chatid).Wait();
            state?.AddChat(chatid2).Wait();
            var botState = state?.GetChatState(chatid2).Result;
            Assert.IsNotNull(botState);
            Assert.IsTrue(((Mode)botState[0]) == Mode.Common);
            botState[0] = (byte)Mode.Overrun;
            state?.SetChatState(botState, chatid2);

            botState = state?.GetChatState(_chatid).Result;
            Assert.IsNotNull(botState);
            Assert.IsTrue(((Mode)botState[0]) == Mode.Common);

            botState = state?.GetChatState(chatid2).Result;
            Assert.IsNotNull(botState);
            Assert.IsTrue(((Mode)botState[0]) == Mode.Overrun);
        }
    }
}