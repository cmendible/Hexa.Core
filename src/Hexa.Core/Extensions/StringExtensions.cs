//----------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class HashHelper
    {
        public static string CalculateMD5Hash(this string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                return CalculateHash(input, md5);
            }
        }

        public static string CalculateSHA1Hash(this string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                return CalculateHash(input, sha1);
            }
        }

        private static string CalculateHash(string input, HashAlgorithm hashAlgorithm)
        {
            byte[] inputBytes;
            byte[] hash;

            using (var hashProvider = hashAlgorithm)
            {
                inputBytes = Encoding.ASCII.GetBytes(input);
                hash = hashProvider.ComputeHash(inputBytes);
            }

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }
    }

    public static class HtmlHelper
    {
        public static string StripHtml(this string text)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return reg.Replace(text, "");
        }
    }

    public static class StringSlugExtension
    {
        private static readonly Dictionary<string, string> rules1;
        private static readonly Dictionary<string, string> rules2;

        static StringSlugExtension()
        {
            List<char> invalidChars = "àáâãäåçèéêëìíîïñòóôõöøùúûüýÿ".ToCharArray().ToList();
            List<char> validChars = "aaaaaaceeeeiiiinoooooouuuuyy".ToCharArray().ToList();
            rules1 = invalidChars.ToDictionary(i => i.ToString(), i => validChars[invalidChars.IndexOf(i)].ToString());

            invalidChars = new[] { 'Þ', 'þ', 'Ð', 'ð', 'ß', 'Œ', 'œ', 'Æ', 'æ', 'µ', '&', '(', ')' }.ToList();

            List<string> validStrings =
                new[] { "TH", "th", "DH", "dh", "ss", "OE", "oe", "AE", "ae", "u", "and", "", "" }.ToList();

            rules2 = invalidChars.ToDictionary(i => i.ToString(), i => validStrings[invalidChars.IndexOf(i)]);
        }

        /// <summary>
        /// Will transform "some $ugly ###url wit[]h spaces" into "some-ugly-url-with-spaces"
        /// </summary>
        public static string Slugify(this string phrase)
        {
            string str = phrase.ToLower();

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

        private static string _StrTr(this string source, Dictionary<string, string> replacements)
        {
            var finds = new string[replacements.Keys.Count];

            replacements.Keys.CopyTo(finds, 0);

            string findpattern = string.Join("|", finds);

            var regex = new Regex(findpattern);

            MatchEvaluator evaluator =
                delegate(Match m)
            {
                string match = m.Captures[0].Value; // either "hello" or "hi" from the original string

                if (replacements.ContainsKey(match))
                {
                    return replacements[match];
                }
                else
                {
                    return match;
                }
            };

            return regex.Replace(source, evaluator);
        }
    }
}