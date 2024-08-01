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
}