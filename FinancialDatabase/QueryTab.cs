using FinancialDatabase;
using System;
using System.Collections.Generic;

public class QueryTab
{

    QueryBuilder QB;
	public QueryTab()
	{
        QB = new QueryBuilder();
	}


    // Returns [name, string s w/o name]
    private List<string> extractName(string s)
    {

        // Get quotation type: (see if it uses '"' or ''' to quote the whole item name
        char quoteChar = s[s.IndexOf(',') + 2]; // for ex the quotes around 'name' : (itemID, ' name ', ...)



        int firstQuote = s.IndexOf(quoteChar);
        if (firstQuote == s.Length - 1)
        {
            Console.WriteLine("ERROR: Unrecognized name, only one quote!");
        }
        int lastQuote = -1;
        string name;

        bool foundLastQuote = false;
        int i = firstQuote++;


        //NOTE: Probably a better way to write thie while loop structure
        while (!foundLastQuote)
        {
            i = s.IndexOf(quoteChar, ++i);
            if (i == -1)
            {
                foundLastQuote = true;
            }
            else
            {
                // Special case, name that contains both " and ' is quoted alltogether with ', and any single quotes in string are escaped with \
                if (s[i - 1] != '\\')
                {
                    lastQuote = i;
                    foundLastQuote = true;
                    break;
                }
            }
        }

        name = s.Substring(firstQuote, lastQuote - firstQuote);
        // Remove 'quotes' from the name
        name = name.TrimEnd(quoteChar).TrimStart(quoteChar);
        s = s.Remove(firstQuote, lastQuote - firstQuote);
        List<string> retList = new List<string>();
        retList.Add(name);
        retList.Add(s);
        return retList;
    }

    //TODO: Unused Depreciated Code? (caller function is unused)
    private List<List<string>> runIndividualSearch(string term, string startDate, string endDate, bool isInStock, bool isSoldOut, bool dateCol, bool nameCol, bool priceCol)
    {

        //string query = QB.buildQuery(term, startDate, endDate, isInStock, isSoldOut, dateCol, nameCol, priceCol);

        string cmdText = @"/K python C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Python\Connection\DCQControl.py " + query;
        System.Diagnostics.Process p = new System.Diagnostics.Process();

        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = "CMD.exe";
        p.StartInfo.Arguments = cmdText;
        bool started = p.Start();

        // Show redirected output of process 'p's standard output
        string s = p.StandardOutput.ReadLine();
        string eos = "EOS";     // end-of-stream
        List<List<string>> retArr = new List<List<string>>();
        // Until end of stream is shown, keep writing the next line to the console, and adding it to the listBox1
        while (s.CompareTo(eos) != 0)
        {
            // Trim off "(" and ",)" from start/end of string
            string trimmedS = s.Substring(1, s.Length - 2);

            // Extract name of item
            // this must be done seperately due to the nature of multiple quote characters that may be use to quote the item name, and which may or may not be escaped in the string

            List<string> extractedName = extractName(trimmedS);
            string name = extractedName[0];
            trimmedS = extractedName[1];



            // Turn comma-seperated string into a list
            List<string> sList = new List<string>(trimmedS.Split(','));


            sList[1] = name;

            // Add list to return array
            retArr.Add(sList);

            s = p.StandardOutput.ReadLine();
        }

        // End process 'p'
        try
        {
            p.Kill();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return retArr;
    }



    public List<List<string>> runSearch(List<string> searchTerms, string startDate, string endDate, bool inStock, bool soldOut, bool dateCol, bool nameCol, bool priceCol)
    {

        List<List<string>> items = new List<List<string>>();
        List<int> hits = new List<int>();
        List<Tuple<int, List<string>>> combined = new List<Tuple<int, List<string>>>();

        for (int i = 0; i < searchTerms.Count; i++)
        {
            List<List<string>> result = runIndividualSearch(searchTerms[i], startDate, endDate, inStock, soldOut, dateCol, nameCol, priceCol);
            for (int j = 0; j < result.Count; j++)
            {
                if (!items.Contains(result[j]))
                {
                    items.Add(result[j]);
                    hits.Add(1);
                }
                else
                {
                    int index = items.IndexOf(result[j]);
                    hits[index]++;
                }
            }
        }


        for (int i = 0; i < items.Count; i++)
        {
            combined.Add(new Tuple<int, List<string>>(hits[i], items[i]));
        }

        // Sort by number of search term hits (ie: "a b c", "a" and "b" both bring up "a e o b", 2 hits)
        combined.Sort(delegate (Tuple<int, List<string>> a, Tuple<int, List<string>> b)
        {
            return b.Item1 - a.Item1;
        });
        int currIndex = 0;
        for (int i = 0; i < items.Count; i++)
        {
            // Only return items where all terms matched
            if (combined[i].Item1 == searchTerms.Count)
            {
                items[currIndex++] = combined[i].Item2;
            }
        }
        // Clear rest of items with <1 matches
        items.RemoveRange(currIndex, items.Count - currIndex);


        return items;
    }

}
