using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatWarden.Tests.Support
{
    public static class PseudoUnicIdsGenerator
    {
        public static long Get()
        {
            var bytes = RandomNumberGenerator.GetBytes(8);
            var value1 = BitConverter.ToInt64(bytes, 0);
            return value1;
        }
    }
}
