using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheDotFactory
{
    public static class StringExtension
    {
        /// <summary>
        /// Repeat input string given amount of times
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="count">Times to repeat</param>
        /// <returns>Repeated string</returns>
        public static string Repeat(this string input, int count)
        {
            string result = "";

            for (int i = 0; i < count; i++)
            {
                result += input;
            }

            return result;
        }
    }
}
