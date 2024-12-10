using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialDatabase.DatabaseObjects
{
    public class Purchase
    {

        private int id         { get; set; }
        private Util.Date date { get; set; }
        private int amount     { get; set; }
        public List<ResultItem> items { get; set; }

        public Purchase()
        {

        }

        public Purchase(int id, Util.Date date, int amount)
        {
            this.id = id;
            this.date = date;
            this.amount = amount;
        }

        public void add(ResultItem item)
        {
            if (items is null)
            {
                items = new List<ResultItem>();
            }
            items.Add(item);
        }


    }
}
