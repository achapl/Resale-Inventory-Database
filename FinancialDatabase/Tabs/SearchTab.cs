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
using ItemHitPair = System.Tuple<Item, int>;
using Date = Util.Date;

public class SearchTab
{
    Form1 Form1;
    TabController tabController;

    public List<Item> searchItems;

    public SearchTab(TabController tabController, Form1 Form1)
    {
        searchItems = new List<Item>();
        this.tabController = tabController;
        this.Form1 = Form1;
    }


    private List<Item> getItemList(List<ItemHitPair> list)
    {
        return list.Select(_ => _.Item1).ToList();
    }

    private void incrHits(ref List<ItemHitPair> itemsAndHits,int index)
    {
        itemsAndHits[index] = new ItemHitPair(itemsAndHits[index].Item1, itemsAndHits[index].Item2 + 1);
    }

    // Given a list of search terms, run a separate search on each individual search term, and aggregate the results
    private List<Item> runSearch(SearchQuery Q)
    {
        // List of ResultItems and how many times it has been found given all of the search terms individually
        // (max one hit per search term)
        List<ItemHitPair> itemsAndHits = new List<ItemHitPair>();

        // Note: Defualt search, "", is accounted for in this loop, still searches for all items
        foreach (string searchTerm in Q.getSearchTerms())
        {
            Q.setSingleSearchTerm(searchTerm);
            List<Item> result = Database.getItems(Q);

            // Put the results from the search term into itemAndHitsPair
            // If already in there, increase the hit count for that term
            foreach (Item item in result)
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

        Form1.itemSearchView.clearItems();
        tabController.clearSearchItems();
        List<string> searchTerms = new List<string>(Form1.searchBox.Text.Split(' '));
        DateTime startDateRaw = Form1.boughtAfterDatePicker.Value;
        DateTime endDateRaw = Form1.boughtBeforeDatePicker.Value;
        bool inStock = Form1.inStockCheckBox.Checked;
        bool soldOut = Form1.soldOutCheckBox.Checked;
        bool dateCol = Form1.showDateCheckBox.Checked;
        bool priceCol = Form1.showPurcPriceCheckBox.Checked;

        Date startDate = new Date(Form1.boughtAfterDatePicker);
        Date endDate  = new Date(Form1.boughtBeforeDatePicker);
        //string startDate = startDateRaw.Year.ToString() + "-" + startDateRaw.Month.ToString() + "-" + startDateRaw.Day.ToString();
        //string endDate = endDateRaw.Year.ToString() + "-" + endDateRaw.Month.ToString() + "-" + endDateRaw.Day.ToString();

        SearchQuery Q = new SearchQuery(searchTerms,
                                        "",
                                        startDate,
                                        endDate,
                                        inStock,
                                        soldOut,
                                        dateCol,
                                        priceCol);

        List<Item> result = runSearch(Q);
        string itemStr = "";
        Form1.itemSearchView.clearItems();
        for (int i = 0; i < result.Count; i++)
        {
            itemStr = result[i].get_Name();
            if (priceCol) { itemStr = result[i].get_Amount_purchase() + ", " + itemStr; }
            if (dateCol)  { itemStr += ", " + result[i].get_Date_Purchased().toDateString(); }
            Form1.itemSearchView.addRow(result[i].get_Thumbnail().image, itemStr);
            addSearchItems(result[i]);
            
        }
    }

    public void clearSearchItems() {
        Form1.itemSearchView.clearItems();
    }

    public List<Item> getSearchItems()
    {
        if (searchItems is null)
        {
            searchItems = new List<Item>();
        }
        return searchItems;
    }

    public void clearCurrItemsVar()
    {
        searchItems.Clear();
    }

    public void addSearchItems(Item newItem)
    {
        searchItems.Add(newItem);
    }

    public void addSearchItems(List<Item> newItems)
    {
        searchItems.AddRange(newItems);
    }
}
