using ChatWarden.CoreLib.Bot.Queue.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatWarden.CoreLib.Tests.Models
{
    public class TestOrder : IOrder
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
