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
            return new Regex("[^a-zA-Z0-9_\u4e00-\u9fa5a]").Replace(string.Join("_", input.Split(" ")), "");
        }
    }
}
