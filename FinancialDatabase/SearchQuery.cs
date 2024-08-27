using System;
using System.Collections.Generic;

public class SearchQuery
{

    List<string> searchTerms;
    string singleTerm;
    string startDate;
    string endDate;
    bool inStock;
    bool soldOut;
    bool dateCol;
    bool priceCol;

    public SearchQuery(
        List<string> searchTerms,
        string singleTerm,
        string startDate,
        string endDate,
        bool inStock,
        bool soldOut,
        bool dateCol,
        bool priceCol)
	{
        this.searchTerms = searchTerms;
        this.singleTerm = singleTerm;
        this.startDate = startDate;
        this.endDate = endDate;
        this.inStock = inStock;
        this.soldOut = soldOut;
        this.dateCol = dateCol;
        this.priceCol = priceCol;
    }
    
    public List<string> getSearchTerms() => this.searchTerms;
    public string getSingleTerm() => this.singleTerm;
    public string getStartDate() => this.startDate;
    public string getEndDate() => this.endDate;
    public bool getInStock() => this.inStock;
    public bool getSoldOut() => this.soldOut;
    public bool getDateCol() => this.dateCol;
    public bool getPriceCol() => this.priceCol;

    public void setSingleSearchTerm(string term) => this.singleTerm = term;
}
