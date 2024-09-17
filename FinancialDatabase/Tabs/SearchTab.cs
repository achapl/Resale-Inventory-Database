using FinancialDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ItemHitPair = System.Tuple<ResultItem, int>;

public class SearchTab
{

    CtrlerOfPythonToDTBConnector PyController;
    QueryBuilder QB;
    Form1 Form1;
    public SearchTab(Form1 Form1)
    {
        PyController = new CtrlerOfPythonToDTBConnector();
        QB = new QueryBuilder();
        this.Form1 = Form1;
    }


    private List<ResultItem> getItemList(List<ItemHitPair> list)
    {
        return list.Select(_ => _.Item1).ToList();
    }

    private void incrHits(ref List<ItemHitPair> itemsAndHits,int index)
    {
        itemsAndHits[index] = new ItemHitPair(itemsAndHits[index].Item1, itemsAndHits[index].Item2 + 1);
    }

    // Given a list of search terms, run a search on each individual search term, and aggregate the results
    public List<ResultItem> runSearch(SearchQuery Q)
    {
        // List of ResultItems and how many times it has been found given all of the search terms individually
        // (max one hit per search term)
        List<ItemHitPair> itemsAndHits = new List<ItemHitPair>();


        foreach (string searchTerm in Q.getSearchTerms())
        {
            Q.setSingleSearchTerm(searchTerm);
            List<ResultItem> result = PyController.RunSearchQuery(Q);

            // Put the results from the search term into itemAndHitsPair
            // If already in there, increase the hit cound for that term
            foreach (ResultItem item in result)
            {
                if (!getItemList(itemsAndHits).Contains(item))
                {
                    itemsAndHits.Add(new ItemHitPair(item, 1)); // Initially 1 hit
                }
                else
                {
                    int index = getItemList(itemsAndHits).IndexOf(item);
                    incrHits(ref itemsAndHits, index);
                    
                }
            }
        }

        // Sort by number of search term hits (ie: when searching "a b c", "a" and "b" both bring up "a e o b", 2 hits)
        itemsAndHits.Sort(delegate (ItemHitPair a, ItemHitPair b)
        {
            return b.Item2 - a.Item2;
        });


        // Only return items where all terms matched
        for (int i = 0; i < itemsAndHits.Count; i++)
        {
            if (itemsAndHits[i].Item2 != Q.getSearchTerms().Count)
            {
                itemsAndHits.RemoveAt(i);
                i--;
            }
        }
        return getItemList(itemsAndHits);
    }

    



    public void search()
    {
        Form1.listBox1.Items.Clear();
        Form1.currentItems.Clear();
        List<string> searchTerms = new List<string>(Form1.searchBox.Text.Split(' '));
        DateTime startDateRaw = Form1.dateTimePicker1.Value;
        DateTime endDateRaw = Form1.dateTimePicker2.Value;
        bool inStock = Form1.checkBox1.Checked;
        bool soldOut = Form1.checkBox2.Checked;
        bool dateCol = Form1.checkBox3.Checked;
        bool priceCol = Form1.checkBox5.Checked;

        string startDate = startDateRaw.Year.ToString() + "-" + startDateRaw.Month.ToString() + "-" + startDateRaw.Day.ToString();
        string endDate = endDateRaw.Year.ToString() + "-" + endDateRaw.Month.ToString() + "-" + endDateRaw.Day.ToString();

        SearchQuery Q = new SearchQuery(searchTerms,
                                        "",
                                        startDate,
                                        endDate,
                                        inStock,
                                        soldOut,
                                        dateCol,
                                        priceCol);

        List<ResultItem> result = runSearch(Q);
        string itemStr = "";
        for (int i = 0; i < result.Count; i++)
        {
            itemStr = result[i].get_Name();
            if (priceCol) { itemStr = result[i].get_Amount_purchase() + ", " + itemStr; }
            if (dateCol)  { itemStr += ", " + result[i].get_Date_Purchased().toDateString(); }
            Form1.listBox1.Items.Add(itemStr);
            Form1.currentItems.Add(result[i]);
        }
    }

}
