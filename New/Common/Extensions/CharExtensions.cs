using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class CharExtensions
    {
        public static bool IsValidIdentifierChar(this byte c)
        {
            return IsValidIdentifierStartChar(c) || c == '_';
        }
        public static bool IsValidIdentifierStartChar(this byte c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
        public static bool IsUpper(this char c)
        {
            return char.IsUpper(c);
        }
        public static bool IsLower(this char c)
        {
            return char.IsLower(c);
        }

        public static bool IsHexChar(this char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }
    }
}
