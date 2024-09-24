using System;
using Date = Util.Date;

public class Sale
{
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


    public Date get_Date_Sold() => this.Date_Sold;
    public double get_Amount_sale() => this.Amount_sale;
    public double get_Fees_sale() => this.Fees_sale;
    public string get_Buyer() => this.Buyer;
    public int get_ItemID_sale() => this.ItemID_sale;

    public void set_Date_Sold(Date d) => this.Date_Sold = d;
    public void set_Amount_sale(double a) => this.Amount_sale = a;
    public void set_Fees_sale(double f) => this.Fees_sale = f;
    public void set_Buyer(string b) => this.Buyer = b;
    public void set_ItemID_sale(int i) => this.ItemID_sale = i;
}
