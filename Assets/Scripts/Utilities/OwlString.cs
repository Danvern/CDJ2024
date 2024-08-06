using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class OwlString
{
	public static string RemoveWhitespaceAroundComma(string str)
	{
		return Regex.Replace(str, " *, *", ",");
	}

	public static string CleanSpecialCharacters(string str)
	{
		string clean = str.Trim(' ', ',', '\"', '\'', '\r', '\t');
		clean = clean.TrimEnd(Environment.NewLine.ToCharArray());

		return clean;
	}
	
	// Splits lines with multiple function calls FOO(1,2), BAR(0,0)
	public static string[] SplitFunctionLines(string line)
    {
        List<string> values = new();
        bool inParens = false;
        int startIndex = 0;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '(')
            {
                inParens = true;
            }
            else if (line[i] == ')')
            {
                inParens = false;
            }
            else if (line[i] == ',' && !inParens)
            {
                values.Add(line.Substring(startIndex, i - startIndex).Trim(' ', '"'));
                startIndex = i + 1;
            }
        }

        values.Add(line.Substring(startIndex).Trim(' ', '"'));
        return values.ToArray();
    }

	/// <summary>
    /// Computes the FNV-1a hash for the input string. 
    /// The FNV-1a hash is a non-cryptographic hash function known for its speed and good distribution properties.
    /// Useful for creating Dictionary keys instead of using strings.
    /// https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
    /// </summary>
    /// <param name="str">The input string to hash.</param>
    /// <returns>An integer representing the FNV-1a hash of the input string.</returns>
    public static int ComputeFNV1aHash(this string str) {
        uint hash = 2166136499;
        foreach (char c in str) {
            hash = (hash ^ c) * 17661737;
        }
        return unchecked((int)hash);
    }
}