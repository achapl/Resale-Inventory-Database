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
        if (strToTrim == null) { return ""; }
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

    public static void clearLabelText(List<Label> l)
    {
        foreach (Label label in l)
        {
            label.Text = "";
        }
    }

    public static void clearTBoxText(List<TextBox> t)
    {
        foreach (TextBox tb in t)
        {
            tb.Text = "";
        } 
    }

    public static List<int> ozToOzLbs(int ozs)
    {
        // Default
        if (ozs == -1)
        {
            return new List<int>() { -1, -1 };
        }
        int lbs = ozs / 16;
        int oz = ozs - (16 * lbs);
        return new List<int>(new int[] { lbs, oz });
    }

    public static bool checkTypeOkay(string attrib, string type)
    {
        // It is not this function's job to catch null values for data types. That would depend on the way the database table is set up for the corresponding column
        if (attrib == null) { return true; }
        switch (type)
        {
            case "date":
                return true;

            case "double unsigned":
                try { Double.Parse(attrib); }
                catch { return false; }
                return true;

            case "int unsigned":
                try { Int32.Parse(attrib); }
                catch { return false; }
                if (Int32.Parse(attrib) < 0) return false;
                return true;

            case "int":
                try { Int32.Parse(attrib); }
                catch { return false; }
                return true;

            case "varchar(255)":
                return true;

            case "varchar(45)":
                return true;

            case "mediumtext":
                return true;

            case "longblob":
                return true;

            default:
                return false;

        }
    }

    public static string splitOnTopLevelCommas_StringCleanup(string s)
    {
        // Clean up the split item to be added
        // Remove the comma
        if (s[0] == ',')
        {
            s = s.Substring(1);
        }

        // Remove whitespaces
        s = s.Trim();

        // Remove any quotes
        if (s[0] == '\'')
        {
            s = s.Trim('\'');
        }
        else if (s[0] == '"')
        {
            s = s.Trim('"');
        }
        return s;
    }

    // Split string inStr on commas, except the ones contained in strings inside of the string, encapsulated in quotes
    // ie: ('2521', 'name "of i\'tem', '43', 435, .etc) will be split into the list (strings represented w/o quotes for simplicity)
    //     { 2521,   name "of i\'tem,   43,  435, .etc }
    public static List<string> splitOnTopLevelCommas(string s)
    {
        if (s.Length == 0) return new List<string>();

        char escape = '/';
        char endQuote = (char)0;
        List<char> topLevelStartChars = new List<char>(new char[] { '"', '\'', '(', '{', '[' });
        List<char> topLevelEndChars = new List<char>(new char[] { '"', '\'', ')', '}', ']' });
        Dictionary<char, char> openAndCloseChars = new Dictionary<char, char> { { '(', ')' },
                                                                                { '{', '}' },
                                                                                { '[', ']' },
                                                                                { '"', '"' },
                                                                                { '\'', '\'' }};
        List<string> result = new List<string>();
        bool onTopLevel = true;
        int lastTopLevelComma = 0;
        string splitToBeAdded;

        // Initial cleanup of the string
        // Remove any parenthesies encapsulating the whole string
        if (s[0] == '(' && s.Last() == ')')
        {
            s = s.Substring(1, s.Length - 2);
        }
        for (int i = 0; i < s.Length; i++)
        {
            // If no quote char found yet, and current char is a quote char
            if (onTopLevel && topLevelStartChars.Contains(s[i]))
            {
                endQuote = openAndCloseChars[s[i]];
                onTopLevel = false;
                continue;
            }

            // If another quote has been found underneath top level quote
            if (!onTopLevel && s[i] == endQuote)
            {
                // Edge case out-of-bounds check
                if (i > 0 && s[i - 1] != escape)
                {
                    onTopLevel = true;
                }
                else if (i <= 0)
                {
                    Console.WriteLine("ERROR splitOnTopLevelCommas - current index <= 0, when it should be past zero for index: " + i);
                    return new List<string>();
                }
                // Else, inStr[i] - 1 must be escape char, skip continue to next char
                else
                {
                    continue;
                }
            }


            // Top level comma to split on is found
            if (onTopLevel && s[i] == ',')
            {
                splitToBeAdded = s.Substring(lastTopLevelComma, i - lastTopLevelComma);
                splitToBeAdded = splitOnTopLevelCommas_StringCleanup(splitToBeAdded);
                result.Add(splitToBeAdded);
                lastTopLevelComma = i;
            }


        }
        // Add last element, (which doesn't have a comma after it)

        splitToBeAdded = s.Substring(lastTopLevelComma);
        splitToBeAdded = splitOnTopLevelCommas_StringCleanup(splitToBeAdded);
        result.Add(splitToBeAdded);

        return result;

    }

    // Given a string, it will split it on the first character
    // Will split on top level commas
    public static List<string> PairedCharTopLevelSplit(string inStr, char left)
    {

        if (inStr.Length == 0) return new List<string>();
        Dictionary<char, char> LRDict = new Dictionary<char, char>() {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
            { '\'', '\'' },
            { '"' , '"'  }};

        if (!LRDict.ContainsKey(left))
        {
            Console.WriteLine("ERROR splitOnFirstChar (SplitOnTopLevelPairs): Unknown Paired-Char to Split on encountered: " + left);
            return new List<string>();
        }
        char right = LRDict[left];

        List<string> result = new List<string>();

        // Splits on top level pairings.
        // ie: () (()) () ((()())) splits into  [(), (()), (), ((()()))]
        char[] trimChars = { left, right };
        int pairCount = 0;
        int lastL = 0;
        bool needNewL = false;
        char c;
        for (int i = 0; i < inStr.Length; i++)
        {
            c = inStr[i];
            if (c == left)
            {
                pairCount++;

                if (needNewL)
                {
                    lastL = i;
                    needNewL = false;
                }
            }
            if (c == right) pairCount--;

            if (pairCount < 0) break;

            // Even top-level pairing found
            if (pairCount == 0 && !needNewL)
            {
                needNewL = true;
                result.Add(inStr.Substring(lastL, i - lastL + 1).Trim(trimChars));
            }
        }

        if (pairCount != 0)
        {
            Console.WriteLine("ERROR splitOnFirstChar (SplitOnTopLevelPairs): Unbalanced Pairing");
        }

        return result;
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

        public Date(string s)
        {
            // TODO: COMPLETE THIS
            // Assumed to be format
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
