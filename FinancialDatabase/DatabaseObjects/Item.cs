using FinancialDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Date = Util.Date;



public class Item : IEquatable<Item>
{
    string rawResult;
    List<string> rawList;

    // From item table
    int ITEM_ID;
    [JsonInclude]
    string Name;
    int PurchaseID;
    int SaleID;
    int ShippingID;
    [JsonInclude]
    int InitialQuantity;
    [JsonInclude]
    int CurrentQuantity;
    [JsonInclude]
    string Notes_item;

    // From purchase table
    Date Date_Purchased;
    double Amount_purchase;
    double Tax;
    double Fees_purchase;
    string Seller;
    string Notes_purchase;

    // From shipping table
    [JsonInclude]
    int Length;
    [JsonInclude]
    int Width;
    [JsonInclude]
    int Height;
    [JsonInclude]
    int Weight;
    string Notes_shipping;

    // From image table
    List<MyImage> images;

    // Extra
    double totalSales;
    private MyImage thumbnail;

    // For Testing
    [JsonInclude]
    List<string> imagePaths;
    [JsonInclude]
    public List<Sale> sales { get; set; }


#pragma warning disable CS8618 // Util.DEFAULTs are set to non-null vals
    public Item()
    {
        setDefaults();
    }
    
    private void setDefaults()
    {
        //string rawResult;
        //List<string> rawList;

        // From item table
        ITEM_ID         = Util.DEFAULT_INT;
        Name            = Util.DEFAULT_STRING;
        PurchaseID      = Util.DEFAULT_INT;
        SaleID          = Util.DEFAULT_INT;
        ShippingID      = Util.DEFAULT_INT;
        InitialQuantity = Util.DEFAULT_INT;
        CurrentQuantity = Util.DEFAULT_INT;
        Notes_item      = Util.DEFAULT_STRING;

        // From purchase table
        Date_Purchased  = Util.DEFAULT_DATE;
        Amount_purchase = Util.DEFAULT_DOUBLE;
        Tax             = Util.DEFAULT_DOUBLE;
        Fees_purchase   = Util.DEFAULT_DOUBLE;
        Seller          = Util.DEFAULT_STRING;
        Notes_purchase  = Util.DEFAULT_STRING;

        // From shipping table
        Length          = Util.DEFAULT_INT;
        Width           = Util.DEFAULT_INT;
        Height          = Util.DEFAULT_INT;
        Weight          = Util.DEFAULT_INT;
        Notes_shipping  = Util.DEFAULT_STRING;

        //From image table
        set_images(Util.DEFAULT_IMAGES);
        
        
        // Extra
        totalSales      = SaleTab.getTotalSales(this);

        sales = new List<Sale>();
    }

    public Item(List<string> item, List<string> colNames)
    {
        setDefaults();
        this.rawList = item;

        for(int i = 0; i < colNames.Count; i++) 
        {
            string itemAttribute = item[i];
            // Missing info, skip
            if (itemAttribute.CompareTo("None") == 0)
            {
                itemAttribute = null!;
            }
            switch(colNames[i])
            {
                // From item table
                case "ITEM_ID":
                case "item.ITEM_ID":
                    set_ITEM_ID(itemAttribute);
                    break;
                case "Name":
                case "item.Name":
                    set_Name(itemAttribute);
                    break;
                case "PurchaseID":
                case "item.PurchaseID":
                    set_PurchaseID(itemAttribute);
                    break;
                case "SaleID":
                case "item.SaleID":
                    set_SaleID(itemAttribute);
                    break;
                case "ShippingID":
                case "item.ShippingID":
                    set_ShippingID(itemAttribute);
                    break;
                case "InitialQuantity":
                case "item.InitialQuantity":
                    set_InitialQuantity(itemAttribute);
                    break;
                case "CurrentQuantity":
                case "item.CurrentQuantity":
                    set_CurrentQuantity(itemAttribute);
                    break;
                case "Notes_item":
                case "item.Notes_item":
                    set_Notes_item(itemAttribute);
                    break;
                case "thumbnail":
                case "thumbnail.thumbnail":
                    set_Thumbnail(itemAttribute);
                    break;
                // From purchase table
                case "Date_Purchased":
                case "purchase.Date_Purchased":
                    set_Date_Purchased(itemAttribute);
                    break;
                case "Amount_purchase":
                case "purchase.Amount_purchase":
                    set_Amount_purchase(itemAttribute);
                    break;
                case "Tax":
                case "purchase.Tax":
                    set_Tax(itemAttribute);
                    break;
                case "Fees_purchase":
                case "purchase.Fees_purchase":
                    set_Fees_purchase(itemAttribute);
                    break;
                case "Seller":
                case "purchase.Seller":
                    set_Seller(itemAttribute);
                    break;
                case "Notes_purchase":
                case "purchase.Notes_purchase":
                    set_Notes_purchase(itemAttribute);
                    break;

                // From shipping table
                case "Length":
                case "shipping.Length":
                    set_Length(itemAttribute);
                    break;
                case "Width":
                case "shipping.Width":
                    set_Width(itemAttribute);
                    break;
                case "Height":
                case "shipping.Height":
                    set_Height(itemAttribute);
                    break;
                case "Weight":
                case "shipping.Weight":
                    set_Weight(itemAttribute);
                    break;
                case "Notes_shipping":
                case "shipping.Notes_shipping":
                    set_Notes_shipping(itemAttribute);
                    break;
            }
        }
    }
    public void set_Thumbnail(MyImage image)
    {
        if (image == null)
        {
            this.thumbnail = Util.DEFAULT_IMAGE;
            return;
        } 
        this.thumbnail = image;
    }
    public void set_Thumbnail(string itemAttribute)
    {
        if (itemAttribute is null || itemAttribute.CompareTo("") == 0)
        {
            this.thumbnail = Util.DEFAULT_IMAGE;
            return;
        }

        Image i = Util.rawImageStrToImage(itemAttribute);
        this.thumbnail = new MyImage(i,-1);
    }

    public void getAttrib(string attrib, out string ret)
    {
        switch (attrib)
        {
            // From item table
            case "item.Name":
                ret = get_Name();
                break;
            case "item.Notes_item":
                ret = get_Notes_item();
                break;

            // From purchase table
            case "purchase.Seller":
                ret = get_Seller();
                break;
            case "purchase.Notes_purchase":
                ret = get_Notes_purchase();
                break;

            // From shipping table
            case "shipping.Notes_shipping":
                ret = get_Notes_shipping();
                break;
            default:
                throw new Exception("ERROR: Unknown string Attrib: " + attrib);
        }
    }
    
    public void getAttrib(string attrib, out int ret)
    {
        switch (attrib)
        {
            // From item table
            case "item.ITEM_ID":
                ret = get_ITEM_ID();
                break;
            case "item.PurchaseID":
                ret = get_PurchaseID();
                break;
            case "item.SaleID":
                ret = get_SaleID();
                break;
            case "item.ShippingID":
                ret = get_ShippingID();
                break;
            case "item.InitialQuantity":
                ret = get_InitialQuantity();
                break;
            case "item.CurrentQuantity":
                ret = get_CurrentQuantity();
                break;

            // From shipping table
            case "shipping.Length":
                ret = get_Length();
                break;
            case "shipping.Width":
                ret = get_Width();
                break;
            case "shipping.Height":
                ret = get_Height();
                break;
            case "shipping.Weight":
                ret = get_Weight();
                break;
            case "shipping.WeightLbs":
                ret = get_WeightLbs();
                break;
            case "shipping.WeightOz":
                ret = get_WeightOz();
                break;

            default:
                throw new Exception("ERROR: Unknown int Attrib: " + attrib);
        }
    }
    
    public void getAttrib(string attrib, out double ret)
    {
        switch (attrib)
        {
            // From purchase table
            case "purchase.Amount_purchase":
                ret = get_Amount_purchase();
                break;
            case "purchase.Tax":
                ret = get_Tax();
                break;
            case "purchase.Fees_purchase":
                ret = get_Fees_purchase();
                break;

            // From shipping table
            case "shipping.Length":
                ret = get_Length();
                break;
            case "shipping.Width":
                ret = get_Width();
                break;
            case "shipping.Height":
                ret = get_Height();
                break;
            case "shipping.Weight":
                ret = get_Weight();
                break;

            default:
                throw new Exception("ERROR: Unknown double Attrib: " + attrib);
        }
    }
    
    public void getAttrib(string attrib, out Date ret)
    {
        switch (attrib)
        {
            // From purchase table
            case "purchase.Date_Purchased":
                ret = get_Date_Purchased();
                break;

            default:
                throw new Exception("ERROR: Unknown Date_Purchased Attrib: " + attrib);
        }
    }

    public string getAttribAsStr(string attrib)
    {
        string retVal = "";
        switch (attrib)
        {
            // From item table
            case "item.Name":
                retVal = get_Name();
                break;
            case "item.Notes_item":
                retVal = get_Notes_item();
                break;

            // From purchase table
            case "purchase.Seller":
                retVal = get_Seller();
                break;
            case "purchase.Notes_purchase":
                retVal = get_Notes_purchase();
                break;
            case "purchase.Date_Purchased":
                retVal = get_Date_Purchased_str();
                break;

            // From shipping table
            case "shipping.Notes_shipping":
                retVal = get_Notes_shipping();
                break;

            // From item table
            case "item.ITEM_ID":
                retVal = get_ITEM_ID().ToString();
                break;
            case "item.PurchaseID":
                retVal = get_PurchaseID().ToString();
                break;
            case "item.SaleID":
                retVal = get_SaleID().ToString();
                break;
            case "item.ShippingID":
                retVal = get_ShippingID().ToString();
                break;
            case "item.InitialQuantity":
                retVal = get_InitialQuantity().ToString();
                break;
            case "item.CurrentQuantity":
                retVal = get_CurrentQuantity().ToString();
                break;

            // From shipping table
            case "shipping.Length":
                retVal = get_Length().ToString();
                break;
            case "shipping.Width":
                retVal = get_Width().ToString();
                break;
            case "shipping.Height":
                retVal = get_Height().ToString();
                break;
            case "shipping.Weight":
                retVal = get_Weight().ToString();
                break;
            case "shipping.WeightLbs":
                retVal = get_WeightLbs().ToString();
                break;
            case "shipping.WeightOz":
                retVal = get_WeightOz().ToString();
                break;

            // From purchase table
            case "purchase.Amount_purchase":
                retVal = get_Amount_purchase().ToString();
                break;
            case "purchase.Tax":
                retVal = get_Tax().ToString();
                break;
            case "purchase.Fees_purchase":
                retVal = get_Fees_purchase().ToString();
                break;
        }

        if (retVal.CompareTo(Util.DEFAULT_DATE.toDateString()) == 0 || 
            retVal.CompareTo(Util.DEFAULT_DOUBLE.ToString()) == 0 ||
            retVal.CompareTo(Util.DEFAULT_INT.ToString())  == 0 ||
            retVal.CompareTo(Util.DEFAULT_STRING) == 0)
        {
            throw new Exception("ERROR: Unknown attribute: " + attrib);
        }
        return retVal;
    }

    public bool hasItemEntry() => ITEM_ID != Util.DEFAULT_INT;

    public bool hasPurchaseEntry() => PurchaseID != Util.DEFAULT_INT;

    public bool hasSaleEntry() => totalSales != Util.DEFAULT_INT;


    public bool hasShippingEntry() => ShippingID != Util.DEFAULT_INT;


    public bool hasImageEntry() => images != Util.DEFAULT_IMAGES;


    // From item table
    public int get_ITEM_ID() => ITEM_ID;
    public string get_Name() => Name;
    public int get_PurchaseID() => PurchaseID;
    public int get_SaleID() => SaleID;
    public int get_ShippingID() => ShippingID;
    public int get_InitialQuantity() => InitialQuantity;
    public int get_CurrentQuantity() => CurrentQuantity;
    public string get_Notes_item() => Notes_item;
    public MyImage get_Thumbnail()
    {
        if (this.thumbnail == null) { set_Thumbnail(Util.DEFAULT_IMAGE); } 
        return this.thumbnail;
    }


    // From purchase table
    public Date get_Date_Purchased() => Date_Purchased;
    public string get_Date_Purchased_str() => Date_Purchased.toDateString();
    public double get_Amount_purchase() => Amount_purchase;
    public double get_Tax() => Tax;
    public double get_Fees_purchase() => Fees_purchase;
    public string get_Seller() => Seller;
    public string get_Notes_purchase() => Notes_purchase;
    // From shipping table
    public int get_Length() => Length;
    public int get_Width() => Width;
    public int get_Height() => Height;
    public int get_Weight() => Weight;
    public int get_WeightOz() => Weight % 16;
    public int get_WeightLbs() => Weight / 16;
    public string get_Notes_shipping() => Notes_shipping;

    // From image table
    public List<MyImage> get_Images() {
        if (images == Util.DEFAULT_IMAGES || images == null)
        {
            return Util.DEFAULT_IMAGES;
        }
        else
        {
            return images;
        }
    }

    // Extra
    public double getTotalSales() => totalSales;
    

    // From item table
    public void set_ITEM_ID(int ITEM_ID) => this.ITEM_ID = ITEM_ID;
    public void set_ITEM_ID(string ITEM_ID) => this.ITEM_ID = Int32.Parse(ITEM_ID);

    public void set_Name(string Name)
    {
        if (Name is null)
        {
            throw new Exception("Error: Name is null");
        }
        if (Name.CompareTo("") == 0)
        {
            throw new Exception("Error: Name is blank");
        }
        // Trim off any quotations around the name should there be any. Just the first and last ones. Any others would probably actually be a part of the name
        if (Name[0] == Name[Name.Length - 1] && (Name[0] == '\'' || Name[0] == '"'))
        {
            Name = Name.Substring(1, Name.Length-2);
        }
        this.Name = Name;

    }

    public void set_PurchaseID(int PurchaseID) => this.PurchaseID = PurchaseID;
    public void set_PurchaseID(string PurchaseID)
    {
        if (PurchaseID is null || PurchaseID.CompareTo("") == 0)
        {
            this.PurchaseID = Util.DEFAULT_INT;
            return;
        }
        this.PurchaseID = Int32.Parse(PurchaseID);
    }
    public void set_SaleID(int SaleID) => this.SaleID = SaleID;
    public void set_SaleID(string SaleID)
    {
        if (SaleID is null || SaleID.CompareTo("") == 0)
        {
            this.SaleID = Util.DEFAULT_INT;
            return;
        }
        this.SaleID = Int32.Parse(SaleID);
    }
    public void set_ShippingID(int ShippingID) => this.ShippingID = ShippingID;
    public void set_ShippingID(string ShippingID)
    {
        if (ShippingID is null || ShippingID.CompareTo("") == 0)
        {
            this.ShippingID = Util.DEFAULT_INT;
            return;
        }
        this.ShippingID = Int32.Parse(ShippingID);
    }
    public void set_InitialQuantity(int InitialQuantity) => this.InitialQuantity = InitialQuantity;
    public void set_InitialQuantity(string InitialQuantity)
    {
        if (InitialQuantity is null || InitialQuantity.CompareTo("") == 0)
        {
            this.InitialQuantity = Util.DEFAULT_INT;
            return;
        }
        this.InitialQuantity = Int32.Parse(InitialQuantity);
    }
    public void set_CurrentQuantity(int CurrentQuantity) => this.CurrentQuantity = CurrentQuantity;
    public void set_CurrentQuantity(string CurrentQuantity)
    {
        if (CurrentQuantity is null || CurrentQuantity.CompareTo("") == 0)
        {
            this.CurrentQuantity = Util.DEFAULT_INT;
            return;
        }
        this.CurrentQuantity = Int32.Parse(CurrentQuantity);
    }
    public void set_Notes_item(string Notes_item) => this.Notes_item = Notes_item;


    // From purchase table
    public void set_Date_Purchased(int year, int month, int day)
    {
        this.Date_Purchased = new Date(year, month, day);
    }
    public void set_Date_Purchased(Date date) => this.Date_Purchased = date;
    public void set_Date_Purchased(string date)
    {
        if (date is null || date.CompareTo("") == 0)
        {
            this.Date_Purchased = Util.DEFAULT_DATE;
            return;
        }
        this.Date_Purchased = parseDate(date);
    }
    public void set_Amount_purchase(double Amount_purchase) => this.Amount_purchase = Amount_purchase;
    public void set_Amount_purchase(string Amount_purchase)
    {
        if (Amount_purchase is null || Amount_purchase.CompareTo("") == 0)
        {
            this.Amount_purchase = Util.DEFAULT_DOUBLE;
            return;
        }
        this.Amount_purchase = Double.Parse(Amount_purchase);
    }
    public void set_Tax(double Tax) => this.Tax = Tax;
    public void set_Tax(string Tax)
    {
        if (Tax is null || Tax.CompareTo("") == 0)
        {
            this.Tax = Util.DEFAULT_DOUBLE;
            return;
        }
        this.Tax = Double.Parse(Tax);
    }
    public void set_Fees_purchase(double Fees_purchase) => this.Fees_purchase = Fees_purchase;
    public void set_Fees_purchase(string Fees_purchase)
    {
        if (Fees_purchase is null || Fees_purchase.CompareTo("") == 0)
        {
            this.Fees_purchase = Util.DEFAULT_DOUBLE;
            return;
        }
        this.Fees_purchase = Double.Parse(Fees_purchase);
    }
    public void set_Seller(string Seller) => this.Seller = Seller;
    public void set_Notes_purchase(string Notes_purchase) => this.Notes_purchase = Notes_purchase;
    

    // From shipping table
    public void set_Length(int Length) => this.Length = Length;
    public void set_Length(string Length)
    {
        if (Length is null || Length.CompareTo("") == 0)
        {
            this.Length = Util.DEFAULT_INT;
            return;
        }
        this.Length = Int32.Parse(Length);
    }
    public void set_Width(int Width) => this.Width = Width;
    public void set_Width(string Width)
    {
        if (Width is null || Width.CompareTo("") == 0)
        {
            this.Width = Util.DEFAULT_INT;
            return;
        }
        this.Width = Int32.Parse(Width);
    }
    public void set_Height(int Height) => this.Height = Height;
    public void set_Height(string Height)
    {
        if (Height is null || Height.CompareTo("") == 0)
        {
            this.Height = Util.DEFAULT_INT;
            return;
        }
        this.Height = Int32.Parse(Height);
    }
    public void set_Weight(int Weight) => this.Weight = Weight;
    public void set_Weight(string Weight)
    {
        if (Weight is null || Weight.CompareTo("") == 0)
        {
            this.Weight = Util.DEFAULT_INT;
            return;
        }
        this.Weight = Int32.Parse(Weight);
    }
    public void set_Notes_shipping(string Notes_shipping) => this.Notes_shipping = Notes_shipping;

    // From image table
    public void clear_images()
    {
        if (this.images is null)
        {
            this.images = new List<MyImage>();
        }
        this.images.Clear();
        // Note: this.images = Util.DEFAULT_IMAGES does not work. Must deep copy
        foreach (MyImage image in Util.DEFAULT_IMAGES)
        {
            this.images.Add(image);
        } 
    }
    public void add_image(MyImage image)
    {
        if (this.images is null)
        {
            this.images = new List<MyImage>();
        }

        if (this.images.Count != 0 && this.images[0] == Util.DEFAULT_IMAGE)
        {
            this.images.Clear();
        }
        this.images.Add(image);
    }
    public void add_images(List<MyImage> images)
    {
        if (this.images is null)
        {
            this.images = new List<MyImage>();
        }

        if (this.images.Count != 0 && this.images[0] == Util.DEFAULT_IMAGE)
        {
            this.images.Clear();
        }
        this.images.AddRange(images);
    }

    public void set_images()
    {
        set_images(Database.getAllImages(this));
    }

    public void set_images(List<MyImage> images)
    {
        if (this.images is null)
        {
            this.images = new List<MyImage>();
        }
        this.images.Clear();
        foreach(MyImage image in images)
        {
            this.images.Add(image);
        }
    }

    // Extra
    public void set_totalSales(double totalSales) => this.totalSales = totalSales;

    public bool Equals(Item? other)
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
            return Util.DEFAULT_DATE;
        }

        List<string> ymd;

        // date is format y-m-d
        if (date[4] == '-')
        {
            ymd = new List<string>(date.Split("-"));
            return new Date(Int32.Parse(ymd[0]), Int32.Parse(ymd[1]), Int32.Parse(ymd[2]));
        }


        // Date is format (y,m,d): "datetime.date(2020, 1, 1)" 
        date = date.Remove(0, "datetime.date".Length);
        // "(2020, 1, 1)"
        if (date[0] == '(') {
            date = date.Trim(new char[] { '(', ')' });
            // "2020, 1, 1"
            ymd = new List<string>(date.Split(new string[] { ", " }, StringSplitOptions.None));
            // ["2020","1","1"] (string[])
            return new Date(Int32.Parse(ymd[0]), Int32.Parse(ymd[1]), Int32.Parse(ymd[2]));
        }
        else
        {
            throw new Exception("Error: Unknown Date_Purchased format");
        }
    }

    // Name for an item in the database
    // Escape characters are taken care of,
    // So it should at least contain 1 alpha numeric char
    // And be >=1 length
    public static bool isValidName(string name)
    {
        if (name.Length == 0) return false;
        return Util.containsAnAlphaNumeric(name);
    }
}
