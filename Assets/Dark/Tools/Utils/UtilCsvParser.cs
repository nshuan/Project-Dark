using System.Collections.Generic;

namespace Dark.Tools.Utils
{
    public class UtilCsvParser
    {
        public static List<string[]> Parse(string csvContent)
        {
            var rows = new List<string[]>();
            bool inQuotes = false;
            string currentValue = "";
            List<string> currentRow = new List<string>();

            for (int i = 0; i < csvContent.Length; i++)
            {
                char c = csvContent[i];

                if (c == '\"')
                {
                    // Toggle inQuotes when encountering an unescaped quote
                    if (inQuotes && i + 1 < csvContent.Length && csvContent[i + 1] == '\"')
                    {
                        // Escaped quote
                        currentValue += '\"';
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    currentRow.Add(currentValue);
                    currentValue = "";
                }
                else if ((c == '\n' || c == '\r') && !inQuotes)
                {
                    if (!string.IsNullOrEmpty(currentValue) || currentRow.Count > 0)
                    {
                        currentRow.Add(currentValue);
                        rows.Add(currentRow.ToArray());
                        currentRow = new List<string>();
                        currentValue = "";
                    }
                }
                else
                {
                    currentValue += c;
                }
            }
                
            // Add last row if any
            if (!string.IsNullOrEmpty(currentValue) || currentRow.Count > 0)
            {
                currentRow.Add(currentValue);
                rows.Add(currentRow.ToArray());
            }

            return rows;
        }
    }
}
