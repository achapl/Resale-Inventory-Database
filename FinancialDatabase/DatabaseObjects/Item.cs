using FinancialDatabase;
using System;
using System.CodeDom;
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
    public int ITEM_ID { get; set; }
    [JsonInclude]
    private string _Name;
    public string Name { 
        get => this._Name; 

        set
        {
            if (value is not null &&
                value.Length > 0)
            {
                // Trim off any quotations around the value should there be any. Just the first and last ones. Any others would probably actually be a part of the value
                if (value[0] == value[value.Length - 1] && (value[0] == '\'' || value[0] == '"'))
                {
                    value = value.Substring(1, value.Length - 2);
                }
            }
            this._Name = value;
        }
    }
    public int PurchaseID { get; set; }
    public int ShippingID { get; set; }
    public int thumbnailID { get; set; }
    [JsonInclude]
    public int InitialQuantity { get; set; }
    [JsonInclude]
    public int CurrentQuantity { get; set; }
    [JsonInclude]
    public string Notes_item { get; set; }

    // From purchase table
    public Date Date_Purchased { get; set; }
    public double Amount_purchase { get; set; }
    public double Tax { get; set; }
    public double Fees_purchase { get; set; }
    public string Seller { get; set; }
    public string Notes_purchase { get; set; }

    // From shipping table
    [JsonInclude]
    public int Length { get; set; }
    [JsonInclude]
    public int Width { get; set; }
    [JsonInclude]
    public int Height { get; set; }
    [JsonInclude]
    public int Weight { get; set; }
    public string Notes_shipping { get; set; }

    // From image table
    public List<MyImage> _images;
    public List<MyImage> images {
        get
        {
            if (_images == Util.DEFAULT_IMAGES || _images == null)
            {
                return Util.DEFAULT_IMAGES;
            }
            else
            {
                return _images;
            }
        }

        set
        {
            if (this._images is null)
            {
                this._images = new List<MyImage>();
            }
            this._images.Clear();
            foreach (MyImage image in value)
            {
                this._images.Add(image);
            }
        }
    }

    // Extra
    public MyImage _thumbnail;
    public MyImage thumbnail {
        get
        {
            if (this._thumbnail == null) 
            {
                this._thumbnail = Util.DEFAULT_IMAGE;
            }
            return this._thumbnail;
        }

        set
        {
            if (value == null)
            {
                this._thumbnail = Util.DEFAULT_IMAGE;
                return;
            }
            this._thumbnail = value;
        }
    }

    // For Testing
    [JsonInclude]
    public List<string> imagePaths { get; set; }
    [JsonInclude]
    public List<Sale> sales { get; set; }


#pragma warning disable CS8618 // Util.DEFAULTs are set to non-null vals
    public Item()
    {
        setDefaults();
    }

    public Item(int itemID)
    {
        if (itemID == Util.DEFAULT_INT)
        {
            this.ITEM_ID = -1;
            return;
        }

        Item actualItem = Database.getItem(itemID);
        this.ITEM_ID = actualItem.ITEM_ID;
        this.Name = actualItem.Name;
        this.CurrentQuantity = actualItem.CurrentQuantity;
        this.InitialQuantity = actualItem.InitialQuantity;
        this.Notes_item = actualItem.Notes_item;
        this.thumbnailID = actualItem.thumbnailID;

        this.ShippingID = actualItem.ShippingID;
        this.Length = actualItem.Length;
        this.Width = actualItem.Width;
        this.Height = actualItem.Height;
        this.Weight = actualItem.Weight;
        this.Notes_shipping = actualItem.Notes_shipping;

        this.PurchaseID = actualItem.PurchaseID;
        this.Amount_purchase = actualItem.Amount_purchase;
        this.Date_Purchased = actualItem.Date_Purchased;
        this.Fees_purchase = actualItem.Fees_purchase;
        this.Notes_purchase = actualItem.Notes_purchase;
        this.Seller = actualItem.Seller;
        this.Tax = actualItem.Tax;

        this.sales = actualItem.sales;

    }

    private void setDefaults()
    {
        //string rawResult;
        //List<string> rawList;

        // From item table
        ITEM_ID = Util.DEFAULT_INT;
        Name = Util.DEFAULT_STRING;
        PurchaseID = Util.DEFAULT_INT;
        ShippingID = Util.DEFAULT_INT;
        InitialQuantity = Util.DEFAULT_INT;
        CurrentQuantity = Util.DEFAULT_INT;
        Notes_item = Util.DEFAULT_STRING;
        thumbnailID = Util.DEFAULT_INT;

        // From purchase table
        Date_Purchased = Util.DEFAULT_DATE;
        Amount_purchase = Util.DEFAULT_DOUBLE;
        Tax = Util.DEFAULT_DOUBLE;
        Fees_purchase = Util.DEFAULT_DOUBLE;
        Seller = Util.DEFAULT_STRING;
        Notes_purchase = Util.DEFAULT_STRING;

        // From shipping table
        Length = Util.DEFAULT_INT;
        Width = Util.DEFAULT_INT;
        Height = Util.DEFAULT_INT;
        Weight = Util.DEFAULT_INT;
        Notes_shipping = Util.DEFAULT_STRING;

        //From image table
        images = Util.DEFAULT_IMAGES;


        sales = new List<Sale>();
    }

    public Item(List<string> item, List<string> colNames)
    {
        setDefaults();
        this.rawList = item;

        for (int i = 0; i < colNames.Count; i++)
        {
            string itemAttribute = item[i];
            // Missing info, skip
            if (itemAttribute.CompareTo("None") == 0)
            {
                itemAttribute = null!;
            }
            switch (colNames[i])
            {
                // From item table
                case "ITEM_ID":
                case "item.ITEM_ID":
                    set_ITEM_ID_str(itemAttribute);
                    break;
                case "Name":
                case "item.Name":
                    Name = itemAttribute;
                    break;
                case "PurchaseID":
                case "item.PurchaseID":
                    set_PurchaseID_str(itemAttribute);
                    break;
                case "ShippingID":
                case "item.ShippingID":
                    set_ShippingID_str(itemAttribute);
                    break;
                case "InitialQuantity":
                case "item.InitialQuantity":
                    set_InitialQuantity_str(itemAttribute);
                    break;
                case "CurrentQuantity":
                case "item.CurrentQuantity":
                    set_CurrentQuantity_str(itemAttribute);
                    break;
                case "Notes_item":
                case "item.Notes_item":
                    Notes_item = itemAttribute;
                    break;
                case "thumbnail":
                case "thumbnail.thumbnail":
                    set_Thumbnail(itemAttribute);
                    break;
                case "thumbnailID":
                case "item.thumbnailID":
                    set_thumbnailID_str(itemAttribute);
                    break;

                // From purchase table
                case "Date_Purchased":
                case "purchase.Date_Purchased":
                    set_Date_Purchased_str(itemAttribute);
                    break;
                case "Amount_purchase":
                case "purchase.Amount_purchase":
                    set_Amount_purchase_str(itemAttribute);
                    break;
                case "Tax":
                case "purchase.Tax":
                    set_Tax_str(itemAttribute);
                    break;
                case "Fees_purchase":
                case "purchase.Fees_purchase":
                    set_Fees_purchase_str(itemAttribute);
                    break;
                case "Seller":
                case "purchase.Seller":
                    Seller = itemAttribute;
                    break;
                case "Notes_purchase":
                case "purchase.Notes_purchase":
                    Notes_purchase = itemAttribute;
                    break;

                // From shipping table
                case "Length":
                case "shipping.Length":
                    set_Length_str(itemAttribute);
                    break;
                case "Width":
                case "shipping.Width":
                    set_Width_str(itemAttribute);
                    break;
                case "Height":
                case "shipping.Height":
                    set_Height_str(itemAttribute);
                    break;
                case "Weight":
                case "shipping.Weight":
                    set_Weight_str(itemAttribute);
                    break;
                case "Notes_shipping":
                case "shipping.Notes_shipping":
                    Notes_shipping = itemAttribute;
                    break;
            }
        }
    }
    public void set_Thumbnail(string itemAttribute)
    {
        if (itemAttribute is null || itemAttribute.CompareTo("") == 0)
        {
            this.thumbnail = Util.DEFAULT_IMAGE;
            return;
        }

        Image i = Util.rawImageStrToImage(itemAttribute);
        this.thumbnail = new MyImage(i, -1);
    }

    public void getAttrib(string attrib, out string ret)
    {
        switch (attrib)
        {
            // From item table
            case "item.Name":
                ret = Name;
                break;
            case "item.Notes_item":
                ret = Notes_item;
                break;

            // From purchase table
            case "purchase.Seller":
                ret = Seller;
                break;
            case "purchase.Notes_purchase":
                ret = Notes_purchase;
                break;

            // From shipping table
            case "shipping.Notes_shipping":
                ret = Notes_shipping;
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
                ret = ITEM_ID;
                break;
            case "item.PurchaseID":
                ret = PurchaseID;
                break;
            case "item.ShippingID":
                ret = ShippingID;
                break;
            case "item.InitialQuantity":
                ret = InitialQuantity;
                break;
            case "item.CurrentQuantity":
                ret = CurrentQuantity;
                break;

            // From shipping table
            case "shipping.Length":
                ret = Length;
                break;
            case "shipping.Width":
                ret = Width;
                break;
            case "shipping.Height":
                ret = Height;
                break;
            case "shipping.Weight":
                ret = Weight;
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
                ret = Amount_purchase;
                break;
            case "purchase.Tax":
                ret = Tax;
                break;
            case "purchase.Fees_purchase":
                ret = Fees_purchase;
                break;

            // From shipping table
            case "shipping.Length":
                ret = Length;
                break;
            case "shipping.Width":
                ret = Width;
                break;
            case "shipping.Height":
                ret = Height;
                break;
            case "shipping.Weight":
                ret = Weight;
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
                ret = Date_Purchased;
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
                retVal = Name;
                break;
            case "item.Notes_item":
                retVal = Notes_item;
                break;

                // TODO: DELETE all non-item/shipping cases from this? Remove all this info from Item?
            // From purchase table
            case "purchase.Seller":
                retVal = Seller;
                break;
            case "purchase.Notes_purchase":
                retVal = Notes_purchase;
                break;
            case "purchase.Date_Purchased":
                retVal = get_Date_Purchased_str();
                break;

            // From shipping table
            case "shipping.Notes_shipping":
                retVal = Notes_shipping;
                break;

            // From item table
            case "item.ITEM_ID":
                retVal = ITEM_ID.ToString();
                break;
            case "item.PurchaseID":
                retVal = PurchaseID.ToString();
                break;
            case "item.ShippingID":
                retVal = ShippingID.ToString();
                break;
            case "item.InitialQuantity":
                retVal = InitialQuantity.ToString();
                break;
            case "item.CurrentQuantity":
                retVal = CurrentQuantity.ToString();
                break;

            // From shipping table
            case "shipping.Length":
                retVal = Length.ToString();
                break;
            case "shipping.Width":
                retVal = Width.ToString();
                break;
            case "shipping.Height":
                retVal = Height.ToString();
                break;
            case "shipping.Weight":
                retVal = Weight.ToString();
                break;
            case "shipping.WeightLbs":
                retVal = get_WeightLbs().ToString();
                break;
            case "shipping.WeightOz":
                retVal = get_WeightOz().ToString();
                break;

            // From purchase table
            case "purchase.Amount_purchase":
                retVal = Amount_purchase.ToString();
                break;
            case "purchase.Tax":
                retVal = Tax.ToString();
                break;
            case "purchase.Fees_purchase":
                retVal = Fees_purchase.ToString();
                break;
        }
        return retVal;
    }

    public bool hasItemEntry() => ITEM_ID != Util.DEFAULT_INT;

    public bool hasPurchaseEntry() => PurchaseID != Util.DEFAULT_INT;


    public bool hasShippingEntry() => ShippingID != Util.DEFAULT_INT;


    public bool hasImageEntry() => images != Util.DEFAULT_IMAGES;





    // From purchase table
    public string get_Date_Purchased_str() => Date_Purchased.toDateString();
    public int get_WeightOz() => Weight % 16;
    public int get_WeightLbs()
    {
        if (Weight == Util.DEFAULT_INT)
        {
            return Weight;
        }
        else
        {
            return Weight / 16;
        }
    }



    // Extra
    public double getTotalSales()
    {
        if (sales is null)
        {
            throw new Exception("Error: '" + Name + "' item.sales is null");
        }

        double totalSales = 0;

        foreach (Sale s in sales)
        {
            totalSales += s.get_Amount_sale();
        }
        return totalSales;
    }
    

    // From item table
    public void set_ITEM_ID_str(string ITEM_ID) => this.ITEM_ID = Int32.Parse(ITEM_ID);

    public void set_PurchaseID_str(string PurchaseID)
    {
        if (PurchaseID is null || PurchaseID.CompareTo("") == 0)
        {
            this.PurchaseID = Util.DEFAULT_INT;
            return;
        }
        this.PurchaseID = Int32.Parse(PurchaseID);
    }
    public void set_ShippingID_str(string ShippingID)
    {
        if (ShippingID is null || ShippingID.CompareTo("") == 0)
        {
            this.ShippingID = Util.DEFAULT_INT;
            return;
        }
        this.ShippingID = Int32.Parse(ShippingID);
    }
    public void set_InitialQuantity_str(string InitialQuantity)
    {
        if (InitialQuantity is null || InitialQuantity.CompareTo("") == 0)
        {
            this.InitialQuantity = Util.DEFAULT_INT;
            return;
        }
        this.InitialQuantity = Int32.Parse(InitialQuantity);
    }
    public void set_CurrentQuantity_str(string CurrentQuantity)
    {
        if (CurrentQuantity is null || CurrentQuantity.CompareTo("") == 0)
        {
            this.CurrentQuantity = Util.DEFAULT_INT;
            return;
        }
        this.CurrentQuantity = Int32.Parse(CurrentQuantity);
    }
    public void set_thumbnailID_str(string thumbnailID)
    {
        if (thumbnailID is null || thumbnailID.CompareTo("") == 0)
        {
            this.thumbnailID = Util.DEFAULT_INT;
            return;
        }
        this.thumbnailID = Int32.Parse(thumbnailID);
    }


    // From purchase table
    public void set_Date_Purchased(int year, int month, int day)
    {
        this.Date_Purchased = new Date(year, month, day);
    }
    public void set_Date_Purchased_str(string date)
    {
        if (date is null || date.CompareTo("") == 0)
        {
            this.Date_Purchased = Util.DEFAULT_DATE;
            return;
        }
        this.Date_Purchased = parseDate(date);
    }
    public void set_Amount_purchase_str(string Amount_purchase)
    {
        if (Amount_purchase is null || Amount_purchase.CompareTo("") == 0)
        {
            this.Amount_purchase = Util.DEFAULT_DOUBLE;
            return;
        }
        this.Amount_purchase = Double.Parse(Amount_purchase);
    }
    public void set_Tax_str(string Tax)
    {
        if (Tax is null || Tax.CompareTo("") == 0)
        {
            this.Tax = Util.DEFAULT_DOUBLE;
            return;
        }
        this.Tax = Double.Parse(Tax);
    }
    public void set_Fees_purchase_str(string Fees_purchase)
    {
        if (Fees_purchase is null || Fees_purchase.CompareTo("") == 0)
        {
            this.Fees_purchase = Util.DEFAULT_DOUBLE;
            return;
        }
        this.Fees_purchase = Double.Parse(Fees_purchase);
    }
    

    // From shipping table
    public void set_Length_str(string Length)
    {
        if (Length is null || Length.CompareTo("") == 0)
        {
            this.Length = Util.DEFAULT_INT;
            return;
        }
        this.Length = Int32.Parse(Length);
    }
    public void set_Width_str(string Width)
    {
        if (Width is null || Width.CompareTo("") == 0)
        {
            this.Width = Util.DEFAULT_INT;
            return;
        }
        this.Width = Int32.Parse(Width);
    }
    public void set_Height_str(string Height)
    {
        if (Height is null || Height.CompareTo("") == 0)
        {
            this.Height = Util.DEFAULT_INT;
            return;
        }
        this.Height = Int32.Parse(Height);
    }
    public void set_Weight_str(string Weight)
    {
        if (Weight is null || Weight.CompareTo("") == 0)
        {
            this.Weight = Util.DEFAULT_INT;
            return;
        }
        this.Weight = Int32.Parse(Weight);
    }

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

    public void set_imagesFromDatabase()
    {
        this.images = Database.getAllImages(this);
    }


    // Extra

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

    public bool isDefaultValue(string attrib)
    {
        string val = getAttribAsStr(attrib);
        if (val == Util.DEFAULT_DOUBLE.ToString() ||
            val == Util.DEFAULT_DATE.ToString() ||
            val == Util.DEFAULT_INT.ToString() ||
            val == Util.DEFAULT_STRING)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal bool hasThumbnail()
    {
        return thumbnailID != Util.DEFAULT_INT;
    }
}
