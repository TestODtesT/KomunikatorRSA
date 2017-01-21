using System;
using System.Numerics;
using System.Text;

namespace Communicator.Common
{
    public static class DisplayHelper
    {
        private static Encoding _encoding = Encoding.UTF8;

        public static void DisplayPrivateMessage(BigInteger[] message, string userName, DateTime date)
        {
            Console.WriteLine(GetPrivateMessage(GetEncryptedString(message), userName, date));
        }

        public static void DisplayPrivateMessage(BigInteger[] message, string userName, DateTime date, string to)
        {
            Console.WriteLine($"{date} #[{userName}] to #[{to}] - {GetEncryptedString(message)}");
        }

        public static void DisplayPrivateMessage(string messageText, string userName, DateTime date)
        {
            Console.WriteLine(GetPrivateMessage(messageText, userName, date));
        }

        public static void DisplayGlobalMessage(string message, DateTime date)
        {
            Console.WriteLine(GetGlobalMessage(message, date));
        }

        public static string GetPrivateMessage(string messageText, string userName, DateTime date)
        {
            return $"{date} #[{userName}] - {messageText}";
        }

        public static string GetGlobalMessage(string messageText, DateTime date)
        {
            return $"{date} >> {messageText}";
        }

        private static string GetEncryptedString(BigInteger[] value)
        {
            var sb = new StringBuilder();

            foreach (var number in value)
            {
                var bytes = number.ToByteArray();
                sb.Append(_encoding.GetString(bytes));
            }

            return sb.ToString();
        }
    }
}
