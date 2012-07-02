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

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
	public static class MD5Helper
	{
		public static string CalculateMD5Hash(this string input)
		{
            byte[] inputBytes;
            byte[] hash;

			// step 1, calculate MD5 hash from input
            using (MD5 md5 = MD5.Create())
            {
                inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                hash = md5.ComputeHash(inputBytes);
            }

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
			}
			return sb.ToString();
		}
	}

    public static class StringSlugExtension
    {
        private static Dictionary<string, string> rules1;
        private static Dictionary<string, string> rules2;

        static StringSlugExtension()
        {
            var invalidChars = "àáâãäåçèéêëìíîïñòóôõöøùúûüýÿ".ToCharArray().ToList();
            var validChars = "aaaaaaceeeeiiiinoooooouuuuyy".ToCharArray().ToList();
            rules1 = invalidChars.ToDictionary(i => i.ToString(), i => validChars[invalidChars.IndexOf(i)].ToString());

            invalidChars = new char[] { 'Þ', 'þ', 'Ð', 'ð', 'ß', 'Œ', 'œ', 'Æ', 'æ', 'µ', '&', '(', ')' }.ToList();
            var validStrings = new string[] { "TH", "th", "DH", "dh", "ss", "OE", "oe", "AE", "ae", "u", "and", "", "" }.ToList();
            rules2 = invalidChars.ToDictionary(i => i.ToString(), i => validStrings[invalidChars.IndexOf(i)]);
        }

        private static string _StrTr(this string source, Dictionary<string, string> replacements)
        {
            string[] finds = new string[replacements.Keys.Count];

            replacements.Keys.CopyTo(finds, 0);

            string findpattern = string.Join("|", finds);

            Regex regex = new Regex(findpattern);

            MatchEvaluator evaluator =
                delegate(Match m)
                {
                    string match = m.Captures[0].Value; // either "hello" or "hi" from the original string

                    if (replacements.ContainsKey(match))
                        return replacements[match];
                    else
                        return match;
                };

            return regex.Replace(source, evaluator);
        }

        /// <summary>
        /// Will transform "some $ugly ###url wit[]h spaces" into "some-ugly-url-with-spaces"
        /// </summary>
        public static string Slugify(this string phrase)
        {
            var str = phrase.ToLower();

            // Transform Invalid Chars.
            str = str._StrTr(rules1);

            // Transform Special Chars.
            str = str._StrTr(rules2);

            // Final clean up.
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // convert multiple spaces/hyphens into one space       
            str = Regex.Replace(str, @"[\s-]+", " ").Trim();

            // hyphens
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }

    }
}
