using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class Util
{
	public Util()
	{
	}

    public static string datetoString(List<int> d)
    {
        return "\\\"" + d[0] + "-" + d[1] + "-" + d[2] + "\\\"";
    }

    public static string dateToString(Date d)
    {
        return d.toDateString();
    }

    public static string myTrim(string strToTrim, string[] trimStrs)
    {
        if (strToTrim == null) { return null; }
        if (trimStrs == null) { return strToTrim; }

        bool foundStartTrim = false;
        bool foundEndTrim = false;

        foreach (string s in trimStrs)
        {
            if (!foundStartTrim && strToTrim.StartsWith(s)) 
            {
                foundStartTrim = true;
                strToTrim = strToTrim.Remove(strToTrim.IndexOf(s), s.Length);
            }

            if (!foundEndTrim   && strToTrim.EndsWith(s))   
            {
                foundEndTrim = true;  
                strToTrim = strToTrim.Remove(strToTrim.LastIndexOf(s));
            }

            if (foundStartTrim && foundEndTrim) { break; }
        }

        return strToTrim;



    }

    public struct Date
    {

        public int year, month, day;

        public Date(int y, int m, int d)
        {
            this.year = y;
            this.month = m;
            this.day = d;
        }

        public Date(Control d)
        {
            // If not the a DateTimePicker, set to error values and output error
            try { DateTimePicker _ = (DateTimePicker) d; }
            catch 
            {
                throw new ArgumentException("ERROR: Trying to make Util.Date from object of type control, but control object is not a DateTimePicker");
            }
            DateTimePicker dtp = (DateTimePicker) d;

                this.year = dtp.Value.Year;
                this.month = dtp.Value.Month;
                this.day = dtp.Value.Day;
        }

        public Date(DateTime d)
        {
            this.year  = d.Year;
            this.month = d.Month;
            this.day   = d.Day;
        }

        public Date(DateTimePicker d)
        {
            this.year = d.Value.Year;
            this.month = d.Value.Month;
            this.day = d.Value.Day;
        }

        public string toDateString()
        {
            return  year + "-" + month + "-" + day;
        }
    };

}
