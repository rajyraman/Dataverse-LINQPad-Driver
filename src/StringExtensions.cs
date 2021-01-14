using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NY.Dataverse.LINQPadDriver
{
    public static class StringExtensions
    {
        public static string Sanitise(this string input)
        {
            return new Regex("[^a-zA-Z0-9_]").Replace(string.Join("_", input.Split(" ")), "");
        }
    }
}
