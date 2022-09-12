using System;
using System.Security.Cryptography;

namespace ChatWarden.CoreLib.Tests.Support
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
