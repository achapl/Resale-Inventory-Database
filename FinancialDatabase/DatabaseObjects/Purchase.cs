using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialDatabase.DatabaseObjects
{
    public class Purchase
    {

        public string notes_purchase;
        public List<ResultItem> items;
        public double fees_purchase;
        public Util.Date date;
        public string seller;
        public double amount;
        public double tax;
        public int id;

        public Purchase()
        {

        }

        public Purchase(int id, Util.Date date, double amount)
        {
            this.id = id;
            this.date = date;
            this.amount = amount;
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

        public void add(ResultItem item)
        {
            if (items is null)
            {
                items = new List<ResultItem>();
            }
            items.Add(item);
        }

        public void set_fees_purchase(string fees_purchase)
        {
            if (fees_purchase == null)
            {
                this.fees_purchase = Util.DEFAULT_DOUBLE;
            }
            else
            {
                this.fees_purchase = Double.Parse(fees_purchase);
            }
        }
        public void set_amount(string amount)
        {
            if (amount == null)
            {
                this.amount = Util.DEFAULT_DOUBLE;
            }
            else
            {
                this.amount = Double.Parse(amount);
            }
        }
        public void set_date(string date)
        {
            if (date == null)
            {
                this.date = Util.DEFAULT_DATE;
            }
            else
            {
                this.date = new Util.Date(date);
            }
        }
        public void set_seller(string seller)
        {
            if (seller == null)
            {
                this.seller = Util.DEFAULT_STRING;
            }
            else
            {
                this.seller = seller;
            }
        }
        public void set_tax(string tax)
        {
            if (tax == null)
            {
                this.tax = Util.DEFAULT_DOUBLE;
            }
            else
            {
                this.tax = Double.Parse(tax);
            }
        }
        public void set_id(string id)
        {
            if (id == null)
            {
                this.id = Util.DEFAULT_INT;
            }
            else
            {
                this.id = Int32.Parse(id);
            }
        }
        public void set_notes_purchase(string notes_purchase)
        {
            if (notes_purchase == null)
            {
                this.notes_purchase = Util.DEFAULT_STRING;
            }
            else
            {
                this.notes_purchase = notes_purchase;
            }
        }

    }
}
