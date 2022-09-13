using System.Security.Cryptography;

namespace ChatWarden.CoreLib.Extentions
{
    internal static class ArrayExtention
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T GetRandom<T>(this T[] values)
        {
            if (values.Length == 0)
            {
                throw new ArgumentException("Values length must be >= 1!");
            }
            else
            {
                return values[RandomNumberGenerator.GetInt32(0, values.Length)];
            }
        }
    }
}
