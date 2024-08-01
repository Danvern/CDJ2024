using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ImportableType
{
	NONE,
	STRING,
	STRING_MULTI,
	INTEGER,
	INTEGER_MULTI,
	PERCENT,
	FLOAT,
	FLOAT_MULTI,
	BOOL,
}

public class DataPair<T>
{
	T var;

	public DataPair(T var)
	{
		this.var = var;
	}
}

public class DataEntry
{
	public Dictionary<string, (ImportableType, string)> data = new();
	public DataEntry(string[] names, string[] values, ImportableType[] valueMap)
	{
		for (int i = 0; i < values.Length && i < valueMap.Length; i++)
		{
			data.Add(names[i], new(valueMap[i], values[i]));
		}
	}

	public string GetString(string parameter)
	{
		if (!data.ContainsKey(parameter) || data[parameter].Item1 != ImportableType.STRING)
			return "";
		return data[parameter].Item2;
	}

	public string[] GetStringArray(string parameter)
	{
		if (!data.ContainsKey(parameter) || data[parameter].Item1 != ImportableType.STRING_MULTI)
			return new string[0];
		string result = data[parameter].Item2;
		result = OwlString.RemoveWhitespaceAroundComma(result);
		return result.Split(',', options: StringSplitOptions.RemoveEmptyEntries);
	}

	public int GetInteger(string parameter)
	{
		int val = 0;
		if (!data.ContainsKey(parameter) || data[parameter].Item1 != ImportableType.INTEGER)
			return 0;
		int.TryParse(data[parameter].Item2, out val);
		return val;
	}

	public int[] GetIntegerArray(string parameter)
	{
		if (!data.ContainsKey(parameter) || data[parameter].Item1 != ImportableType.INTEGER_MULTI)
			return new int[0];
		string result = data[parameter].Item2;
		result = OwlString.RemoveWhitespaceAroundComma(result);
		string[] results = result.Split(',', options: StringSplitOptions.RemoveEmptyEntries);
		int[] nums = new int[results.Length];
		for (int i = 0; i < results.Length; i++)
		{
			int val = 0;
			int.TryParse(results[i], out val);
			nums[i] = val;
		}
		return nums;
	}

	public float GetPercent(string parameter)
	{
		float val = 1.00f;
		if (!data.ContainsKey(parameter) || data[parameter].Item1 != ImportableType.PERCENT)
			return 0;
		if (float.TryParse(data[parameter].Item2.Replace("%", ""), out val))
			val = val / 100f;
		return val;
	}

	public float GetFloat(string parameter)
	{
		float val = 0f;
		if (!data.ContainsKey(parameter) || data[parameter].Item1 != ImportableType.FLOAT)
			return 0;
		float.TryParse(data[parameter].Item2, out val);
		return val;
	}

	public float[] GetFloatArray(string parameter)
	{
		if (!data.ContainsKey(parameter) || data[parameter].Item1 != ImportableType.FLOAT_MULTI)
			return new float[0];
		string result = data[parameter].Item2;
		result = OwlString.RemoveWhitespaceAroundComma(result);
		string[] results = result.Split(',', options: StringSplitOptions.RemoveEmptyEntries);
		float[] nums = new float[results.Length];
		for (int i = 0; i < results.Length; i++)
		{
			float val = 0;
			float.TryParse(results[i], out val);
			nums[i] = val;
		}
		return nums;
	}

	public bool GetBool(string parameter)
	{
		if (!data.ContainsKey(parameter) || data[parameter].Item1 != ImportableType.BOOL)
			return false;
		return bool.Parse(data[parameter].Item2);
	}
}

public class Database
{
	private Dictionary<string, List<DataEntry>> entries = new();

	public bool ContainsCategory(string key)
	{
		return entries.ContainsKey(key);
	}

	public int CategoryCount(string key)
	{
		if (!ContainsCategory(key))
			return 0;
		return entries[key].Count;
	}

	public List<DataEntry> GetEntriesByKey(string key)
	{
		if (!ContainsCategory(key))
			return null;
		return entries[key];
	}

	public DataEntry GetEntryByIndex(string key, int index)
	{
		if (!ContainsCategory(key) || index >= entries[key].Count)
			return null;
		return entries[key][index];
	}

	public void AddEntry(DataEntry entry, string key = "_")
	{
		if (!ContainsCategory(key))
			entries.Add(key, new List<DataEntry>() { entry });
		else
			entries[key].Add(entry);
	}
}

public class OwlDatabase : MonoBehaviour
{
	public static DataEntry GetRandomDatabaseEntry(Database database, string category)
	{
		if (database.ContainsCategory(category) && database.CategoryCount(category) > 0)
		{
			int randomIndex = Random.Range(0, database.CategoryCount(category));
			return database.GetEntryByIndex(category, randomIndex);
		}
		return null;
	}

	public static DataEntry GetRandomDatabaseEntryConditional(Database database, string category, Predicate<DataEntry> condition)
	{
		if (database.ContainsCategory(category) && database.CategoryCount(category) > 0)
		{
			// Filter entries based on provided conditional
			List<DataEntry> filtered = database.GetEntriesByKey(category).FindAll(condition);
			if (filtered.Count > 0)
			{
				// Select a random entry from the filtered list
				int randomIndex = Random.Range(0, database.CategoryCount(category));
				return filtered[randomIndex];
			}
		}
		return null;
	}

	void Start()
	{
		// to help with loading from different regions
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
		System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
	}

	public static Database LoadCSV(string path, Dictionary<string, ImportableType> types)
	{
		var textFile = Resources.Load<TextAsset>(path);
		try
		{
			Database data = new();
			string[] lines = textFile.text.Split("\n");


			string[] headerLine = SplitCsvLine(lines[0]);
			int[] validLines = new int[types.Keys.Count];
			int matchCount = 0;
			// Only read the columns which match with the provided types.
			for (int i = 0; i < headerLine.Length && matchCount < validLines.Length; i++)
			{
				if (types.ContainsKey(headerLine[i]))
				{
					validLines[matchCount] = i;
					matchCount++;
				}
			}
			if (matchCount < validLines.Length)
			{
				Debug.LogWarning("CSV " + path + " missing some required fields.");
				return null;
			}

			// Establish types
			string[] nameMap = new string[matchCount]; // the name of the column matched to its position 
			ImportableType[] typeMap = new ImportableType[matchCount]; // the type of the column matched to its position 
			for (int i = 0; i < matchCount; i++)
			{
				int rI = validLines[i];

				nameMap[i] = headerLine[rI];
				typeMap[i] = types[headerLine[rI]];

			}

			// Load data lines
			for (int i = 1; i < lines.Length; i++)
			{
				string[] values = SplitCsvLine(lines[i]);
				string[] valueMap = new string[matchCount];
				if (values.Length == validLines.Length)
				{
					for (int vI = 0; vI < matchCount; vI++)
					{
						int rI = validLines[vI];
						valueMap[vI] = values[rI];
					}

					DataEntry entry = new(nameMap, valueMap, typeMap);
					data.AddEntry(entry);
				}
			}

			Debug.LogFormat("CSV {0} loaded successfully.", path);
			return data;
		}
		catch (Exception e)
		{
			Debug.LogErrorFormat("Error loading CSV {0}: {1}", path, e.Message);
		}

		return null;
	}

	public static string RemoveSpecialCharacters(string str)
	{
		StringBuilder sb = new StringBuilder();
		foreach (char c in str)
		{
			if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
			{
				sb.Append(c);
			}
		}
		return sb.ToString();
	}

	private static string[] SplitCsvLine(string line)
	{
		List<string> values = new();
		bool inQuotes = false;
		int startIndex = 0;

		for (int i = 0; i < line.Length; i++)
		{
			if (line[i] == '"')
			{
				inQuotes = !inQuotes;
			}
			else if (line[i] == ',' && !inQuotes)
			{
				values.Add(OwlString.CleanSpecialCharacters(line.Substring(startIndex, i - startIndex)));
				startIndex = i + 1;
			}
		}

		values.Add(OwlString.CleanSpecialCharacters(line.Substring(startIndex)));

		return values.ToArray();
	}
}