#region License

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion

namespace Hexa.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// HexEncodingHelper
    /// </summary>
    public static class HexEncodingHelper
    {
        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored.
        /// </summary>
        /// <param name="hexString">string to convert to byte array</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        public static byte[] GetBytes(string value)
        {
            if (!InHexFormat(value))
                throw new ArgumentException("value is not in Hexadecimal format");

            string newString = "";
            char c;

            // remove all none A-F, 0-9, characters
            for (int i = 0; i < value.Length; i++)
            {
                c = value[i];
                if (IsHexDigit(c))
                    newString += c;
            }
            // if odd number of characters, discard last character
            if (newString.Length%2 != 0)
                newString = newString.Substring(0, newString.Length - 1);

            int byteLength = newString.Length/2;
            var bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new[] {newString[j], newString[j + 1]});
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }

        /// <summary>
        /// Converts a byte array to string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.Byte.ToString(System.String)")]
        public static string ToString(byte[] value)
        {
            string hexString = "";
            for (int i = 0; i < value.Length; i++)
            {
                hexString += value[i].ToString("X2");
            }
            return hexString;
        }

        /// <summary>
        /// Determines if given string is in proper hexadecimal string format
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals",
            MessageId = "div")]
        public static bool InHexFormat(string value)
        {
            int result = 0;

            int div = Math.DivRem(value.Length, 2, out result);
            if (result > 0)
                return false;

            foreach (char digit in value)
            {
                if (!IsHexDigit(digit))
                {
                    return false;
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>true if hex digit, false if not</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo",
            MessageId = "System.Char.ToUpper(System.Char)")]
        private static bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }

        /// <summary>
        /// Converts 1 or 2 character string into equivalant byte value
        /// </summary>
        /// <param name="hex">1 or 2 character string</param>
        /// <returns>byte</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.Byte.Parse(System.String,System.Globalization.NumberStyles)")]
        private static byte HexToByte(string value)
        {
            if (value.Length > 2 || value.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(value, NumberStyles.HexNumber);
            return newByte;
        }
    }
}