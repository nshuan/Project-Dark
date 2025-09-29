using System;
using System.Collections.Generic;
using System.Text;

namespace Dark.Tools.Utils
{
    public class UtilCsvParser
    {
        /// <summary>
        /// Full data get from sheet,
        /// this function will find the first range containing full of "comment"
        /// and get all line below that as true data
        /// </summary>
        /// <param name="csvContent"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
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

            return ValidateParse(rows);
        }

        private static List<string[]> ValidateParse(List<string[]> rows)
        {
            int startCol = 0, endCol = 0, startRow = -1;
            var commentFound = false;

            for (var i = 0; i < rows.Count; i++)
            {
                for (var j = 0; j < rows[i].Length; j++)
                {
                    if (commentFound == false)
                    {
                        if (rows[i][j] == "comment")
                        {
                            commentFound = true;
                            startCol = j;
                            startRow = i + 2;
                        }
                    }
                    
                    if (commentFound == true)
                    {
                        if (rows[i][j] == "comment")
                        {
                            endCol = j;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                
                if (commentFound)
                    break;
            }

            if (commentFound)
            {
                var result = new List<string[]>();
                for (var i = startRow; i < rows.Count; i++)
                {
                    result.Add(new string[endCol - startCol + 1]);
                    for (var j = startCol; j <= endCol; j++)
                    {
                        result[i - startRow][j - startCol] = rows[i][j];
                    }
                }
                
                return result;
            }
            
            return rows;
        }
    }
}
