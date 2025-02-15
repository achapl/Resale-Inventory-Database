using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinancialDatabase.DatabaseObjects
{
    public class Purchase
    {

        [JsonInclude]
        public string Notes_purchase;
        [JsonInclude]
        public List<Item> items;
        [JsonInclude]
        public double Fees_purchase;
        public Util.Date Date_Purchased;
        [JsonInclude]
        public string Seller;
        [JsonInclude]
        public double Amount_purchase;
        [JsonInclude]
        public double Tax;
        [JsonInclude]
        public int PURCHASE_ID;

        [JsonInclude]
        // For testing
        public string Date_Purchased_str;

        public Purchase()
        {
            Notes_purchase = Util.DEFAULT_STRING;
            Amount_purchase = Util.DEFAULT_DOUBLE;
            Fees_purchase = Util.DEFAULT_DOUBLE;
            Date_Purchased = Util.DEFAULT_DATE;
            PURCHASE_ID = Util.DEFAULT_INT;
            Seller = Util.DEFAULT_STRING;
            Tax = Util.DEFAULT_DOUBLE;
            items = null;
    }

        public Purchase(List<string> itemAttributes, List<string> colNames)
        {
            for (int i = 0; i < colNames.Count; i++)
            {
                string? itemAttribute = itemAttributes[i];
                // Missing info, skip
                if (itemAttributes[i].CompareTo("None") == 0)
                {
                    itemAttribute = null;
                }
                switch (colNames[i])
                {
                    case "Fees_purchase":
                        set_fees_purchase(itemAttribute);
                        break;
                    case "Amount_purchase":
                        set_amount(itemAttribute);
                        break;
                    case "Date_Purchased":
                        set_date(itemAttribute);
                        break;
                    case "Tax":
                        set_tax(itemAttribute);
                        break;
                    case "PURCHASE_ID":
                        set_id(itemAttribute);
                        break;
                    case "Notes":
                        set_notes_purchase(itemAttribute);
                        break;
                    case "Seller":
                        set_seller(itemAttribute);
                        break;
                }
            }
        }

        public void add(Item item)
        {
            if (items is null)
            {
                items = new List<Item>();
            }
            items.Add(item);
        }

        public void set_fees_purchase(string fees_purchase)
        {
            if (fees_purchase == null)
            {
                this.Fees_purchase = Util.DEFAULT_DOUBLE;
            }
            else
            {
                this.Fees_purchase = Double.Parse(fees_purchase);
            }
        }
        public void set_amount(string amount)
        {
            if (amount == null)
            {
                this.Amount_purchase = Util.DEFAULT_DOUBLE;
            }
            else
            {
                this.Amount_purchase = Double.Parse(amount);
            }
        }
        public void set_date(string date)
        {
            if (date == null)
            {
                this.Date_Purchased = Util.DEFAULT_DATE;
            }
            else
            {
                this.Date_Purchased = new Util.Date(date);
            }
        }
        public void set_seller(string seller)
        {
            if (seller == null)
            {
                this.Seller = Util.DEFAULT_STRING;
            }
            else
            {
                this.Seller = seller;
            }
        }
        public void set_tax(string tax)
        {
            if (tax == null)
            {
                this.Tax = Util.DEFAULT_DOUBLE;
            }
            else
            {
                this.Tax = Double.Parse(tax);
            }
        }
        public void set_id(string id)
        {
            if (id == null)
            {
                this.PURCHASE_ID = Util.DEFAULT_INT;
            }
            else
            {
                this.PURCHASE_ID = Int32.Parse(id);
            }
        }
        public void set_notes_purchase(string notes_purchase)
        {
            if (notes_purchase == null)
            {
                this.Notes_purchase = Util.DEFAULT_STRING;
            }
            else
            {
                this.Notes_purchase = notes_purchase;
            }
        }

    }
}
