using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrPorker.Extensions
{
    public static class StringExtensions
    {
        public static string GetFirstWord(this string input)
        {
            for (int i = 0; i < input.Length; i++)
                if (input[i] == ' ')
                    return input[..i];

            return input; // Return the entire string if no space is found
        }
    }
}
