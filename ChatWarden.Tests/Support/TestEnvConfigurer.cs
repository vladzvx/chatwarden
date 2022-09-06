using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatWarden.Tests.Support
{
    public class TestEnvConfigurer
    {
        public static void ReadEnvFile(string pathToFile)
        {
            var GetEnvReg = new Regex(@"^([^=\n\t\r ]+) *= *([^\n\t\r ]+) *$");
            string[] lines = File.ReadAllLines(pathToFile);
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    Match match = GetEnvReg.Match(line);
                    if (match.Success)
                    {
                        Environment.SetEnvironmentVariable(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }
            }
        }

        public static bool TryReadEnvFile(string pathToFile)
        {
            if (File.Exists(pathToFile))
            {
                ReadEnvFile(pathToFile);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetTarantoolConnectionStringToEnvironment()
        {
            string? TARANTOOL_USER = Environment.GetEnvironmentVariable("TARANTOOL_USER");
            string? TARANTOOL_PWD = Environment.GetEnvironmentVariable("TARANTOOL_PWD");
            string? TARANTOOL_HOST = Environment.GetEnvironmentVariable("TARANTOOL_HOST");
            string? TARANTOOL_EXTERNAL_PORT = Environment.GetEnvironmentVariable("TARANTOOL_EXTERNAL_PORT");
            string TARANTOOL_CNNSTR = string.Format("{0}:{1}@{2}:{3}", TARANTOOL_USER, TARANTOOL_PWD, TARANTOOL_HOST, TARANTOOL_EXTERNAL_PORT);
            Environment.SetEnvironmentVariable("TARANTOOL_CNNSTR", TARANTOOL_CNNSTR);
        }

    }
}
