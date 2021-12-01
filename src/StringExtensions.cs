using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace NY.Dataverse.LINQPadDriver
{
    public static class StringExtensions
    {
        private static readonly CSharpCodeProvider _csharpCodeProvider = new CSharpCodeProvider();

        public static string Sanitise(this string input)
        {
            var joinedInput = string.Join("_", input.Split(" "));
            if (_csharpCodeProvider.IsValidIdentifier(joinedInput))
            {
                return joinedInput;
            }
            return new Regex("[^a-zA-Z0-9_]").Replace(joinedInput, "");
        }
    }
}
