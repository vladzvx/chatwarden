using System.Security.Cryptography;

namespace ChatWarden.CoreLib.Extentions
{
    internal static class ArrayStringExtention
    {
        public static string GetRandom(this string[] strings)
        {
            if (strings.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                return strings[RandomNumberGenerator.GetInt32(0, strings.Length)];
            }
        }
    }
}
