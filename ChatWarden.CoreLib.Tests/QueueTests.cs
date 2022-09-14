using ChatWarden.CoreLib.Bot.Queue;
using ChatWarden.CoreLib.Tests.Models;
using ChatWarden.CoreLib.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace ChatWarden.CoreLib.Tests
{
    [TestClass]
    public class QueueTests
    {
        private static Box? box;
        private static Publisher? publisher;
        private static ConsumerBase? consumer;

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
            publisher = new Publisher(box);
            consumer = new ConsumerBase(box);
        }

        [TestMethod]
        public void AddOrder_GetOrder_OneOrder()
        {
            Assert.IsNotNull(publisher);
            Assert.IsNotNull(consumer);
            var length = RandomNumberGenerator.GetInt32(1, 10);
            var bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)RandomNumberGenerator.GetInt32(0, 255);
            }

            var order = new TestOrder(bytes);
            publisher.PublishOrder(order).Wait();
            var (taskId, data) = consumer.GetOrder().Result;
            Assert.IsTrue(data.SequenceEqual(bytes));
            consumer.AckOrder(taskId).Wait();
        }

        [TestMethod]
        public void AddOrder_GetOrder_TwoOrders()
        {
            Assert.IsNotNull(publisher);
            Assert.IsNotNull(consumer);
            var length1 = RandomNumberGenerator.GetInt32(1, 10);
            var bytes1 = new byte[length1];
            for (int i = 0; i < length1; i++)
            {
                bytes1[i] = (byte)RandomNumberGenerator.GetInt32(0, 255);
            }

            var length2 = RandomNumberGenerator.GetInt32(1, 10);
            var bytes2 = new byte[length2];
            for (int i = 0; i < length2; i++)
            {
                bytes2[i] = (byte)RandomNumberGenerator.GetInt32(0, 255);
            }

            var order1 = new TestOrder(bytes1);
            var order2 = new TestOrder(bytes2);
            publisher.PublishOrder(order1).Wait();

            publisher.PublishOrder(order2).Wait();

            var res = consumer.GetOrder().Result;

            Assert.IsTrue(res.data.SequenceEqual(bytes1));
            var id1 = res.taskId;
            res = consumer.GetOrder().Result;

            var id2 = res.taskId;
            Assert.IsTrue(res.data.SequenceEqual(bytes2));
            consumer.AckOrder(id1).Wait();
            consumer.AckOrder(id2).Wait();
        }

        [TestMethod]
        public void AddOrder_GetOrder_Resume_TwoOrders()
        {
            Assert.IsNotNull(publisher);
            Assert.IsNotNull(consumer);
            var length1 = RandomNumberGenerator.GetInt32(1, 10);
            var bytes1 = new byte[length1];
            for (int i = 0; i < length1; i++)
            {
                bytes1[i] = (byte)RandomNumberGenerator.GetInt32(0, 255);
            }

            var length2 = RandomNumberGenerator.GetInt32(1, 10);
            var bytes2 = new byte[length2];
            for (int i = 0; i < length2; i++)
            {
                bytes2[i] = (byte)RandomNumberGenerator.GetInt32(0, 255);
            }

            var order1 = new TestOrder(bytes1);
            var order2 = new TestOrder(bytes2);
            publisher.PublishOrder(order1).Wait();

            publisher.PublishOrder(order2).Wait();

            var res = consumer.GetOrder().Result;

            Assert.IsTrue(res.data.SequenceEqual(bytes1));
            var id1 = res.taskId;
            res = consumer.GetOrder().Result;

            var id2 = res.taskId;
            consumer.ReturnOrder(id2).Wait();
            res = consumer.GetOrder().Result;
            Assert.IsTrue(res.data.SequenceEqual(bytes2));
            consumer.AckOrder(id1).Wait();
            consumer.AckOrder(id2).Wait();
        }
    }
}