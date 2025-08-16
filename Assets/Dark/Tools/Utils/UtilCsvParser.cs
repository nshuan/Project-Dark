using System;
using System.Collections.Generic;
using System.Text;

namespace Dark.Tools.Utils
{
    public class UtilCsvParser
    {
        public static List<string[]> Parse(string csvContent)
        {
            var rows = new List<string[]>();
            var currentRow = new List<string>();
            var currentValue = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < csvContent.Length; i++)
            {
                char c = csvContent[i];

                if (c == '\"')
                {
                    if (inQuotes && i + 1 < csvContent.Length && csvContent[i + 1] == '\"')
                    {
                        // Escaped quote
                        currentValue.Append('\"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    currentRow.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else if ((c == '\n' || c == '\r') && !inQuotes)
                {
                    // Handle \r\n (Windows newlines)
                    if (c == '\r' && i + 1 < csvContent.Length && csvContent[i + 1] == '\n')
                        i++;

                    currentRow.Add(currentValue.ToString());
                    rows.Add(currentRow.ToArray());

                    currentRow = new List<string>();
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            // Add final value and row if needed
            if (inQuotes)
            {
                throw new FormatException("Malformed CSV: unmatched quote.");
            }

            if (currentValue.Length > 0 || currentRow.Count > 0)
            {
                currentRow.Add(currentValue.ToString());
                rows.Add(currentRow.ToArray());
            }

            return rows;
        }
    }
}
