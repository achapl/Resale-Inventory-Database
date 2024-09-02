using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Date = Util.Date;



public class ResultItem : IEquatable<ResultItem>
{
    string rawResult;
    List<string> rawList;

    // From item table
    int ITEM_ID;
    string Name;
    int PurchaseID;
    int SaleID;
    int ShippingID;
    int InitialQuantity;
    int CurrentQuantity;
    string Notes_item;

    // From purchase table
    Date Date_Purchased;
    double Amount_purchase;
    double Tax;
    double Fees_purchase;
    string Seller;
    string Notes_purchase;

    // From sale table
    Date Date_Sold;
    double Amount_sale;
    double Fees_sale;
    string Buyer;

    // From shipping table
    int Length;
    int Width;
    int Height;
    int Weight;
    string Notes_shipping;

    #pragma warning disable CS8618 // Defaults are set to non-null vals
    public ResultItem()
    {
        setDefaults();
    }
    #pragma warning disable CS8618 // Defaults are set to non-null vals
    public ResultItem(string item, List<string> colNames)
	{
        this.setDefaults();
		this.rawResult = item;
	}

    void setDefaults()
    {
        //string rawResult;
        //List<string> rawList;

        // From item table
        ITEM_ID = -1;
        Name = "";
        PurchaseID = -1;
        SaleID = -1;
        ShippingID = -1;
        InitialQuantity = -1;
        CurrentQuantity = -1;
        Notes_item = "";

        // From purchase table
        Date_Purchased = new Util.Date(-1,-1,-1);
        Amount_purchase = -1;
        Tax = -1;
        Fees_purchase = -1;
        Seller = "";
        Notes_purchase = "";

        // From sale table
        Date_Sold = new Date(-1, -1, -1);
        Amount_sale = -1;
        Fees_sale = -1;
        Buyer = "";

        // From shipping table
        Length = -1;
        Width = -1;
        Height = -1;
        Weight = -1;
        Notes_shipping = "";
    }

    public ResultItem(List<string> item, List<string> colNames)
    {
        this.rawList = item;

        for(int i = 0; i < colNames.Count; i++) 
        {
            string itemAttribute = item[i];
            // Missing info, skip
            if (item[i].CompareTo("None") == 0)
            {
                itemAttribute = null!;
            }
            switch(colNames[i])
            {
                // From item table
                case "ITEM_ID":
                    set_ITEM_ID(itemAttribute);
                    break;
                case "Name":
                    set_Name(itemAttribute);
                    break;
                case "PurchaseID":
                    set_PurchaseID(itemAttribute);
                    break;
                case "SaleID":
                    set_SaleID(itemAttribute);
                    break;
                case "ShippingID":
                    set_ShippingID(itemAttribute);
                    break;
                case "InitialQuantity":
                    set_InitialQuantity(itemAttribute);
                    break;
                case "CurrentQuantity":
                    set_CurrentQuantity(itemAttribute);
                    break;
                case "Notes_item":
                    set_Notes_item(itemAttribute);
                    break;

                // From purchase table
                case "Date_Purchased":
                    set_Date_Purchased(itemAttribute);
                    break;
                case "Amount_purchase":
                    set_Amount_purchase(itemAttribute);
                    break;
                case "Tax":
                    set_Tax(itemAttribute);
                    break;
                case "Fees_purchase":
                    set_Fees_purchase(itemAttribute);
                    break;
                case "Seller":
                    set_Seller(itemAttribute);
                    break;
                case "Notes_purchase":
                    set_Notes_purchase(itemAttribute);
                    break;

                // From sale table
                case "Date_Sold":
                    set_Date_Sold(itemAttribute);
                    break;
                case "Amount_sale":
                    set_Amount_sale(itemAttribute);
                    break;
                case "Fees_sale":
                    set_Fees_sale(itemAttribute);
                    break;
                case "Buyer":
                    set_Buyer(itemAttribute);
                    break;

                // From shipping table
                case "Length":
                    set_Length(itemAttribute);
                    break;
                case "Width":
                    set_Width(itemAttribute);
                    break;
                case "Height":
                    set_Height(itemAttribute);
                    break;
                case "Weight":
                    set_Weight(itemAttribute);
                    break;
                case "Notes_shipping":
                    set_Notes_shipping(itemAttribute);
                    break;
            }
        }
    }

    



    // From item table
    public int get_ITEM_ID()
    {
        // Default value if uninitialized
        if (ITEM_ID == -1)
        {
            return -1;
        }
        else
        {
            return ITEM_ID;
        }
    }
    public string get_Name()
    {
        // Default value if uninitialized
        if (Name == null)
        {
            return "";
        }
        else
        {
            return Name;
        }
    }
    public int get_PurchaseID()
    {
        // Default value if uninitialized
        if (PurchaseID == -1)
        {
            return -1;
        }
        else
        {
            return PurchaseID;
        }
    }
    public int get_SaleID()
    {
        // Default value if uninitialized
        if (SaleID == -1)
        {
            return -1;
        }
        else
        {
            return SaleID;
        }
    }
    public int get_ShippingID()
    {
        // Default value if uninitialized
        if (ShippingID == -1)
        {
            return -1;
        }
        else
        {
            return ShippingID;
        }
    }
    public int get_InitialQuantity()
    {
        // Default value if uninitialized
        if (InitialQuantity == -1)
        {
            return -1;
        }
        else
        {
            return InitialQuantity;
        }
    }
    public int get_CurrentQuantity()
    {
        // Default value if uninitialized
        if (CurrentQuantity == -1)
        {
            return -1;
        }
        else
        {
            return CurrentQuantity;
        }
    }
    public string get_Notes_item()
    {
        // Default value if uninitialized
        if (Notes_item == null)
        {
            return "";
        }
        else
        {
            return Notes_item;
        }
    }

    // From purchase table
    public Date get_Date_Purchased()
    {
        return this.Date_Purchased;
    }
    public double get_Amount_purchase()
    {
        // Default value if uninitialized
        if (Amount_purchase == -1)
        {
            return -1;
        }
        else
        {
            return Amount_purchase;
        }
    }
    public double get_Tax()
    {
        // Default value if uninitialized
        if (Tax == -1)
        {
            return -1;
        }
        else
        {
            return Tax;
        }
    }
    public double get_Fees_purchase()
    {
        // Default value if uninitialized
        if (Fees_purchase == -1)
        {
            return -1;
        }
        else
        {
            return Fees_purchase;
        }
    }
    public string get_Seller()
    {
        // Default value if uninitialized
        if (Seller == null)
        {
            return "";
        }
        else
        {
            return Seller;
        }
    }
    public string get_Notes_purchase()
    {
        // Default value if uninitialized
        if (Notes_purchase == null)
        {
            return "";
        }
        else
        {
            return Notes_purchase;
        }
    }

    // From sale table
    public List<int> get_Date_Sold()
    {
        List<int> l = new List<int>();
        l.Add(Date_Sold.year);
        l.Add(Date_Sold.month);
        l.Add(Date_Sold.day);
        return l;
    }
    public double get_Amount_sale()
    {
        // Default value if uninitialized
        if (Amount_sale == -1)
        {
            return -1;
        }
        else
        {
            return Amount_sale;
        }
    }
    public double get_Fees_sale()
    {
        // Default value if uninitialized
        if (Fees_sale == -1)
        {
            return -1;
        }
        else
        {
            return Fees_sale;
        }
    }
    public string get_Buyer()
    {
        // Default value if uninitialized
        if (Buyer == null)
        {
            return "";
        }
        else
        {
            return Buyer;
        }
    }

    // From shipping table
    public int get_Length()
    {
        // Default value if uninitialized
        if (Length == -1)
        {
            return -1;
        }
        else
        {
            return Length;
        }
    }
    public int get_Width()
    {
        // Default value if uninitialized
        if (Width == -1)
        {
            return -1;
        }
        else
        {
            return Width;
        }
    }
    public int get_Height()
    {
        // Default value if uninitialized
        if (Height == -1)
        {
            return -1;
        }
        else
        {
            return Height;
        }
    }
    public int get_Weight()
    {
        // Default value if uninitialized
        if (Weight == -1)
        {
            return -1;
        }
        else
        {
            return Weight;
        }
    }
    public string get_Notes_shipping()
    {
        // Default value if uninitialized
        if (Notes_shipping == null)
        {
            return "";
        }
        else
        {
            return Notes_shipping;
        }
    }

    // From item table
    public void set_ITEM_ID(int ITEM_ID) => this.ITEM_ID = ITEM_ID;
    public void set_ITEM_ID(string ITEM_ID) => this.ITEM_ID = Int32.Parse(ITEM_ID);
    public void set_Name(string Name)
    {
        // Trim off any quotations around the name should there be any
        if (Name[0] == Name[Name.Length - 1] && Name[0] == '\'')
        {
            Name = Name.Trim('\'');
        }
        else if (Name[0] == Name[Name.Length - 1] && Name[0] == '"')
        {
            Name = Name.Trim('"');
        }
        this.Name = Name;

    }

    public void set_PurchaseID(int PurchaseID) => this.PurchaseID = PurchaseID;
    public void set_PurchaseID(string PurchaseID)
    {
        if (PurchaseID == null)
        {
            this.PurchaseID = -1;
        }
        else
        {
            this.PurchaseID = Int32.Parse(PurchaseID);
        }
    }
    public void set_SaleID(int SaleID) => this.SaleID = SaleID;
    public void set_SaleID(string SaleID)
    {
        if (SaleID == null)
        {
            this.SaleID = -1;
        }
        else
        {
            this.SaleID = Int32.Parse(SaleID);
        }
    }
    public void set_ShippingID(int ShippingID) => this.ShippingID = ShippingID;
    public void set_ShippingID(string ShippingID)
    {
        if (ShippingID == null)
        {
            this.ShippingID = -1;
        }
        else
        {
            this.ShippingID = Int32.Parse(ShippingID);
        }
    }
    public void set_InitialQuantity(int InitialQuantity) => this.InitialQuantity = InitialQuantity;
    public void set_InitialQuantity(string InitialQuantity)
    {
        if (InitialQuantity == null)
        {
            this.InitialQuantity = -1;
        }
        else
        {
            this.InitialQuantity = Int32.Parse(InitialQuantity);
        }
    }
    public void set_CurrentQuantity(int CurrentQuantity) => this.CurrentQuantity = CurrentQuantity;
    public void set_CurrentQuantity(string CurrentQuantity)
    {
        if (CurrentQuantity == null)
        {
            this.CurrentQuantity = -1;
        }
        else
        {
            this.CurrentQuantity = Int32.Parse(CurrentQuantity);
        }
    }
    public void set_Notes_item(string Notes_item) => this.Notes_item = Notes_item;


    // From purchase table
    public void set_Date_Purchased(int year, int month, int day)
    {
        this.Date_Purchased = new Date(year, month, day);
    }
    public void set_Date_Purchased(string date) => this.Date_Purchased = parseDate(date);
    public void set_Amount_purchase(double Amount_purchase) => this.Amount_purchase = Amount_purchase;
    public void set_Amount_purchase(string Amount_purchase)
    {
        if (Amount_purchase == null)
        {
            this.Amount_purchase = -1.0;
        }
        else
        {
            this.Amount_purchase = Double.Parse(Amount_purchase);
        }
    }
    public void set_Tax(double Tax) => this.Tax = Tax;
    public void set_Tax(string Tax)
    {
        if (Tax == null)
        {
            this.Tax = -1.0;
        }
        else
        {
            this.Tax = Double.Parse(Tax);
        }
    }
    public void set_Fees_purchase(double Fees_purchase) => this.Fees_purchase = Fees_purchase;
    public void set_Fees_purchase(string Fees_purchase) => this.Fees_purchase = double.Parse(Fees_purchase);
    public void set_Seller(string Seller) => this.Seller = Seller;
    public void set_Notes_purchase(string Notes_purchase) => this.Notes_purchase = Notes_purchase;
    

    // From sale table
    public void set_Date_Sold(int year, int month, int day) => this.Date_Sold = new Date(year, month, day);
    public void set_Date_Sold(string date) => this.Date_Sold = parseDate(date);
    public void set_Amount_sale(double Amount_sale) => this.Amount_sale = Amount_sale;
    public void set_Amount_sale(string Amount_sale)
    {
        if (Amount_sale == null)
        {
            this.Amount_sale = -1.0;
        }
        else
        {
            this.Amount_sale = Double.Parse(Amount_sale);
        }
    }
    public void set_Fees_sale(double Fees_sale) => this.Fees_sale = Fees_sale;
    public void set_Fees_sale(string Fees_sale)
    {
        if (Fees_sale == null)
        {
            this.Fees_sale = -1.0;
        }
        else
        {
            this.Fees_sale = Double.Parse(Fees_sale);
        }
    }
    public void set_Buyer(string Buyer) => this.Buyer = Buyer;
    


    // From shipping table
    public void set_Length(int Length) => this.Length = Length;
    public void set_Length(string Length)
    {
        if (Length == null)
        {
            this.Length = -1;
        }
        else
        {
            this.Length = Int32.Parse(Length);
        }
    }
    public void set_Width(int Width) => this.Width = Width;
    public void set_Width(string Width)
    {
        if (Width == null)
        {
            this.Width = -1;
        }
        else
        {
            this.Width = Int32.Parse(Width);
        }
    }
    public void set_Height(int Height) => this.Height = Height;
    public void set_Height(string Height)
    {
        if (Height == null)
        {
            this.Height = -1;
        }
        else
        {
            this.Height = Int32.Parse(Height);
        }
    }
    public void set_Weight(int Weight) => this.Weight = Weight;
    public void set_Weight(string Weight)
    {
        if (Weight == null)
        {
            this.Weight = -1;
        }
        else
        {
            this.Weight = Int32.Parse(Weight);
        }
    }
    public void set_Notes_shipping(string Notes_shipping) => this.Notes_shipping = Notes_shipping;


    #pragma warning disable CS8767 // 'other' and 'this' are checked for null
    public bool Equals(ResultItem other)
    {
        if (this  == null && other == null)  return true;
        if (this  == null)  return false;
        if (other == null)  return false;
        return this.ITEM_ID == other!.ITEM_ID;
    }

    private Date parseDate(string date)
    {
        if (date == null)
        {
            return new Date(0, 0, 0);
        }
        // date is format (y,m,d): "datetime.date(2020, 1, 1)" 
        date = date.Remove(0, "datetime.date".Length);
        // "(2020, 1, 1)"
        date = date.Trim(new char[] { '(', ')' });
        // "2020, 1, 1"
        List<string> ymd = new List<string>(date.Split(new string[] { ", " }, StringSplitOptions.None));
        // ["2020","1","1"] (string[])
        return new Date(Int32.Parse(ymd[0]), Int32.Parse(ymd[1]), Int32.Parse(ymd[2]));
    }
}
