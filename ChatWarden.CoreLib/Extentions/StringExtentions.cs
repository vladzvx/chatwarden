using System.Text.RegularExpressions;

namespace ChatWarden.CoreLib.Extentions
{
    public static class StringExtentions
    {
        private static readonly Regex tokenRegex = new(@"^(\d+):.+$");
        public static long? GetBotId(this string str)
        {
            var match = tokenRegex.Match(str);
            if (match.Success)
            {
                return long.Parse(match.Groups[1].Value);
            }
            else
            {
                return null;
            }
        }
    }
}
