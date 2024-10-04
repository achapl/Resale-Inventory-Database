using System;
using Date = Util.Date;

public class Sale : IEquatable<Sale>
{
    int SALE_ID;
    Date Date_Sold;
    double Amount_sale;
    double Fees_sale;
    string Buyer;
    int ItemID_sale;


    public Sale()
	{
		this.Date_Sold = new Date();
		this.Amount_sale = -1;
	}

    public Sale(List<string> item, List<string> colNames)
    {

        for (int i = 0; i < colNames.Count; i++)
        {
            string itemAttribute = item[i];
            // Missing info, skip
            if (item[i].CompareTo("None") == 0)
            { 
                itemAttribute = null!;
            }
            switch (colNames[i])
            {
                case "SALE_ID":
                    set_SALE_ID(Int32.Parse(item[i]));
                    break;
                case "Date_Sold":
                    set_Date_Sold(new Date(item[i]));
                    break;
                case "Amount_sale":
                    set_Amount_sale(Double.Parse(item[i]));
                    break;
                case "Fees_sale":
                    set_Fees_sale(Double.Parse(item[i]));
                    break;
                case "Buyer":
                    set_Buyer(item[i]);
                    break;
                case "ItemID_sale":
                    set_ItemID_sale(Int32.Parse(item[i]));
                    break;
            }
        }
    }

    public void getAttribAsStr(string attrib, ref string ret)
    {
        switch (attrib)
        {
            // From sale table
            case "sale.SALE_ID":
                ret = get_SALE_ID().ToString();
                break;
            case "sale.Date_Sold":
                ret = get_Date_Sold().toDateString();
                break;
            case "sale.Amound_sale":
                ret = get_Amount_sale().ToString();
                break;
            case "sale.Fees_sale":
                ret = get_Fees_sale().ToString();
                break;
            case "sale.Buyer":
                ret = get_Buyer();
                break;
            case "sale.ItemID_sale":
                ret = get_ItemID_sale().ToString();
                break;
        }

        if (ret.CompareTo(Util.DEFAULT_DATE.toDateString()) == 0 ||
            ret.CompareTo(Util.DEFAULT_DOUBLE.ToString()) == 0 ||
            ret.CompareTo(Util.DEFAULT_INT.ToString()) == 0 ||
            ret.CompareTo(Util.DEFAULT_STRING) == 0)
        {
            ret = "";
        }
    }


    public int get_SALE_ID() => this.SALE_ID;
    public Date get_Date_Sold() => this.Date_Sold;
    public double get_Amount_sale() => this.Amount_sale;
    public double get_Fees_sale() => this.Fees_sale;
    public string get_Buyer() => this.Buyer;
    public int get_ItemID_sale() => this.ItemID_sale;

    public void set_SALE_ID(int s) => this.SALE_ID = s;
    public void set_Date_Sold(Date d) => this.Date_Sold = d;
    public void set_Amount_sale(double a) => this.Amount_sale = a;
    public void set_Fees_sale(double f) => this.Fees_sale = f;
    public void set_Buyer(string b) => this.Buyer = b;
    public void set_ItemID_sale(int i) => this.ItemID_sale = i;

#pragma warning disable CS8767 // 'other' and 'this' are checked for null
    public bool Equals(Sale other)
    {
        if (this == null && other == null) return true;
        if (this == null) return false;
        if (other == null) return false;
        return this.SALE_ID == other!.SALE_ID;
    }
}
