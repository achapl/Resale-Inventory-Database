using FinancialDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;
using Python.Runtime;
using System.Xml.Linq;
using System.IO;



public class CtrlerOfPythonToDTBConnector
{
    

    const string PYTHON_EXEC = @"/K python "/* + "-m pdb "*/ + @"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Python\Connection\DCQControl.py ";
    const string START_COL_MARKER = "Column Names:"; // Marker to the Start of Column Names
    const string END_COL_MARKER = "END OF COLUMN NAMES"; // Marker to the End of Column Names
    const string EOS = "EOS";     // end-of-stream
    
    QueryBuilder QB;

    public CtrlerOfPythonToDTBConnector()
    {
        QB = new QueryBuilder();
        //runPython();
    }

    private void runPython()
    {
        Console.WriteLine(Runtime.PythonDLL);
        Runtime.PythonDLL = @"C:\Users\Owner\AppData\Local\Programs\Python\Python311\python311.dll";
        Console.WriteLine(Runtime.PythonDLL);
        PythonEngine.Initialize();

        var mod = PyModule.Import("DCQControl.py");
    }


    public List<string> getTableNames()
    {
        List<string> tableNames;

        string query = "SHOW TABLES";
        string output = runStatement(query);
        
        removeColumnNames(ref output);
        string[] startAndEnd = { "('", "',)" };
        output = Util.myTrim(output, startAndEnd );
        tableNames = new List<string>(output.Split(new string[] { "',)('" }, StringSplitOptions.None));


        return tableNames;
    }

    public Dictionary<string, string> getColDataTypes()
    {

        List<string> tableNames = getTableNames();

        Dictionary<string, string> colDataTypes = new Dictionary<string, string>();
        string query;
        string output;
        foreach (string tableName in tableNames)
        {
            query = "SHOW COLUMNS FROM " + tableName + ";";
            output = runStatement(query);
            removeColumnNames(ref output);
            string[] startAndEnd = { "('", "',)" };
            output = Util.myTrim(output, startAndEnd);
            List<string> colAndColTypes = new List<string>(output.Split(new string[] { "')('" }, StringSplitOptions.None));
            foreach(string colAndType in colAndColTypes)
            {
                List<string> typesForCol = new List<string>(colAndType.Split(new string[] { "', '" }, StringSplitOptions.None));
                string colName = tableName + "." + typesForCol[0];
                string type    = typesForCol[1];
                colDataTypes[colName] = type;
            }
        }

        return colDataTypes;
    }

    // General queries, done by manual string input
    public List<ResultItem> RunItemSearchQuery(string query)
    {
        string queryOutput = runStatement(query);

        List<ResultItem> parsedItems = parseRawQuery(queryOutput);

        return parsedItems;
    }

    public string runStatement(string statement)
    {
        string queryOutput = "";
        string line;

        System.Diagnostics.Process p = startPython(statement);

        line = p.StandardOutput.ReadLine();
        while (line.CompareTo(EOS) != 0)
        {
            queryOutput += line;
            line = p.StandardOutput.ReadLine();
        }

        kill(p);
        return queryOutput;
    }

    // Given a search query, turn it into a string query and run it
    public List<ResultItem> RunSearchQuery(SearchQuery Q)
    {
        string query = QB.buildSearchByNameQuery(Q);
        return RunItemSearchQuery(query);
    }
    
    // Start the python process and return the process so its output can be read
    private Process startPython(string query)
    {
        string args = PYTHON_EXEC + query;
        System.Diagnostics.Process p = new System.Diagnostics.Process();

        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = "CMD.exe";
        p.StartInfo.Arguments = args;
        p.Start();
        return p;
    }

    private void kill(Process p)
    {
        try
        {
            p.Kill();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    private List<ResultItem> parseRawQuery(string rawResult)
    {

        List<string> colNames = removeColumnNames(ref rawResult);

        // raw result is now the format "(itemName, itemID, .etc)(item2Name, item2ID, .etc)"
        // Seperate whole string into list of multiple item strings, "[ (itemName, itemID, .etc), (item2Name, item2ID, .etc) ]"
        List<string> rawItems = new List<string>(splitOnFirstChar(rawResult, SplitOnFirstCharOptions.SplitOnTopLevelPairs));

        List<ResultItem> results = new List<ResultItem>();
        foreach(string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> itemAttributes = splitOnTopLevelCommas(rawItem);
            results.Add(new ResultItem(itemAttributes, colNames));
        }

        return results;
    }


    // Returns the column names from rawResult, and returns rawResult w/o the column names
    private List<string> removeColumnNames(ref string rawResult)
    {
        string startMarker = "Column Names:"; // Marker to the Start of Column Names
        string endMarker = "END OF COLUMN NAMES"; // Marker to the End of Column Names


        // Get the positions of the start and end markers that surround the column names to determine start and end of the column names
        int startMarkerBegin = rawResult.IndexOf(startMarker);
        int colNamesBegin = startMarkerBegin + startMarker.Length;
        int colNamesEnd = rawResult.IndexOf(endMarker);
        string colNamesRaw = rawResult.Substring(colNamesBegin, colNamesEnd - colNamesBegin);

        // Remove column names from rawResult
        rawResult = rawResult.Substring(colNamesEnd + endMarker.Length, rawResult.Length - colNamesEnd - endMarker.Length);

        // Remove leading and trailing square brackets so it can be split into individual names by the commas
        colNamesRaw = colNamesRaw.Trim(new char[] { '[', ']' });
        colNamesRaw = colNamesRaw.Trim(new char[] { '\'', '\'' });

        return new List<string>(colNamesRaw.Split(new string[] { "', '" }, StringSplitOptions.None));
    }
    
    enum SplitOnFirstCharOptions
    {
        None = 0,
        SplitOnTopLevelPairs = 1
    }

    string splitOnTopLevelCommas_StringCleanup(string s)
    {
        // Clean up the split item to be added
        // Remove the comma
        if (s[0] == ',')
        {
            s = s.Substring(1);
        }

        // Remove whitespaces
        s = s.Trim();

        // Remove any quotes
        if (s[0] == '\'')
        {
            s = s.Trim('\'');
        }
        else if (s[0] == '"')
        {
            s = s.Trim('"');
        }
        return s;
    }

    // Split string s on commas, except the ones contained in strings inside of the string, encapsulated in quotes
    // ie: ('2521', 'name "of i\'tem', '43', 435, .etc) will be split into the list (strings represented w/o quotes for simplicity)
    //     { 2521,   name "of i\'tem,   43,  435, .etc }
    List<string> splitOnTopLevelCommas(string s)
    {
        if (s.Length == 0) return new List<string>();

        char escape = '\\';
        char endQuote = (char) 0;
        List<char> topLevelStartChars = new List<char>(new char[] { '"', '\'', '(', '{', '[' });
        List<char> topLevelEndChars   = new List<char>(new char[] { '"', '\'', ')', '}', ']' });
        Dictionary<char, char> openAndCloseChars = new Dictionary<char, char> { { '(', ')' },
                                                                                { '{', '}' }, 
                                                                                { '[', ']' }, 
                                                                                { '"', '"' }, 
                                                                                { '\'', '\'' }};
        List<string> result = new List<string>();
        bool onTopLevel = true;
        int lastTopLevelComma = 0;
        string splitToBeAdded;

        // Initial cleanup of the string
        // Remove any parenthesies encapsulating the whole string
        if (s[0] == '(' && s.Last() == ')')
        {
            s = s.Substring(1, s.Length - 2);
        }
        for (int i = 0; i < s.Length; i++)
        {
            // If no quote char found yet, and current char is a quote char
            if (onTopLevel && topLevelStartChars.Contains(s[i]))
            {
                endQuote = openAndCloseChars[s[i]];
                onTopLevel = false;
                continue;
            }

            // If another quote has been found underneath top level quote
            if (!onTopLevel && s[i] == endQuote)
            {
                // Edge case out-of-bounds check
                if (i > 0 && s[i - 1] != escape)
                {
                    onTopLevel = true;
                }
                else if (i <= 0)
                {
                    Console.WriteLine("ERROR splitOnTopLevelCommas - current index <= 0, when it should be past zero for index: " + i);
                    return null;
                }
                // Else, s[i] - 1 must be escape char, skip continue to next char
                else
                {
                    continue;
                }
            }


            // Top level comma to split on is found
            if (onTopLevel && s[i] == ',')
            {
                splitToBeAdded = s.Substring(lastTopLevelComma, i - lastTopLevelComma);
                splitToBeAdded = splitOnTopLevelCommas_StringCleanup(splitToBeAdded);
                result.Add(splitToBeAdded);
                lastTopLevelComma = i;
            }


        }
        // Add last element, (which doesn't have a comma after it)

        splitToBeAdded = s.Substring(lastTopLevelComma);
        splitToBeAdded = splitOnTopLevelCommas_StringCleanup(splitToBeAdded);
        result.Add(splitToBeAdded);

        return result;

    }
    
    // Given a string, it will split it on the first character
    List<string> splitOnFirstChar(string s, SplitOnFirstCharOptions option = 0)
    {
        if (s.Length == 0) return new List<string>();

        char firstChar = s[0];
        List<string> result = new List<string>();

        // Splits on top level pairings.
        // ie: () (()) () ((()())) splits into  [(), (()), (), ((()()))]
        if (option == SplitOnFirstCharOptions.SplitOnTopLevelPairs)
        {
            char left = firstChar;
            char right;
            switch (left)
            {
                case '(':
                    right = ')';
                    break;
                case '[':
                    right = ']';
                    break;
                case '{':
                    right = '}';
                    break;
                case '\'':
                    right = '\'';
                    break;
                case '"':
                    right = '"';
                    break;
                default:
                    Console.WriteLine("ERROR splitOnFirstChar (SplitOnTopLevelPairs): Unknown Paired-Char to Split on encountered: " + left);
                    return null;
            }

            char[] trimChars = { left, right };
            int pairCount = 0;
            int lastL = 0;
            bool needNewL = false;
            char c;
            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];
                if (c == left)
                {
                    pairCount++;

                    if (needNewL)
                    {
                        lastL = i;
                        needNewL = false;
                    }
                }
                if (c == right) pairCount--;

                if (pairCount < 0) break;

                // Even top-level pairing found
                if (pairCount == 0)
                {
                    needNewL = true;
                    result.Add(s.Substring(lastL, i - lastL + 1).Trim(trimChars));
                }
            }

            if (pairCount != 0)
            {
                Console.WriteLine("ERROR splitOnFirstChar (SplitOnTopLevelPairs): Unbalanced Pairing");
            }


        }

        return result;
    }


}
