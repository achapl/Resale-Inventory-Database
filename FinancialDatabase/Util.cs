using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public class Util
{

    public static int DEFAULT_INT = -1;
    public static double DEFAULT_DOUBLE = -1.0;
    public static string DEFAULT_STRING = null;
    public static Date DEFAULT_DATE = new Date(-1, -1, -1);
    public static MyImage DEFAULT_IMAGE = new MyImage(Image.FromFile(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png"), -1);
    public static List<MyImage> DEFAULT_IMAGES = new List<MyImage>() {DEFAULT_IMAGE};
    public static string imgStartToken = "'IMAGESTART***";
    public static string imgEndToken   = "***IMAGEEND'";
    public Util()
	{
	}

    public static List<string> mySplit(string str, string delim)
    {
        string[] arrOfStrings = str.Split(delim);
        List<string> listOfStrings = new List<string>(arrOfStrings);
        return listOfStrings;
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

    public static void clearTBox(List<TextBox> t)
    {
        foreach (TextBox tb in t)
        {
            tb.Text = "";
            tb.BackColor = Color.White;
        } 
    }

    public static void clearTBox(TextBox t)
    {
        t.Text = "";
        t.BackColor = Color.White;
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


        // Check for any raw images contained within the string
        List<Tuple<int, int>> imgStartsAndEnds = new List<Tuple<int, int>>();
        getImageStartEnds(s, out imgStartsAndEnds);


        // Loop through the string to find top-level commas
        for (int i = 0; i < s.Length; i++)
        {
            char currChar = s[i];
            char prevChar = '\0';
            if (i > 0) prevChar = s[i - 1];
            // Check for image attribute and handle seperately
            bool contLargerLoop = false;
            foreach (Tuple<int, int> pair in imgStartsAndEnds)
            {
                if (i == pair.Item1)
                {
                    result.Add(s.Substring(i + imgStartToken.Length, pair.Item2 - i - imgStartToken.Length));
                    i = pair.Item2 + imgEndToken.Length;
                    lastTopLevelComma = i;
                    contLargerLoop = true;
                }
            }
            if (contLargerLoop) continue;


            // If no quote char found yet, and current char is a quote char
            if (onTopLevel && topLevelStartChars.Contains(currChar))
            {
                endQuote = openAndCloseChars[currChar];
                onTopLevel = false;
                continue;
            }

            // If another quote has been found underneath top level quote
            if (!onTopLevel && currChar == endQuote)
            {
                // Edge case out-of-bounds check
                if (i > 0 && prevChar != escape)
                {
                    onTopLevel = true;
                }
                else if (i <= 0)
                {
                    throw new Exception("ERROR splitOnTopLevelCommas - current index <= 0, when it should be past zero for index: " + i);
                }
                // Else, inStr[i] - 1 must be escape char, skip continue to next char
                else
                {
                    continue;
                }
            }


            // Top level comma to split on is found
            if (onTopLevel && currChar == ',')
            {
                splitToBeAdded = s.Substring(lastTopLevelComma, i - lastTopLevelComma);
                splitToBeAdded = splitOnTopLevelCommas_StringCleanup(splitToBeAdded);
                result.Add(splitToBeAdded);
                lastTopLevelComma = i;
            }


        }
        // Add last element, (which doesn't have a comma after it)
        if (lastTopLevelComma < s.Length - 1)
        {
            splitToBeAdded = s.Substring(lastTopLevelComma);
            splitToBeAdded = splitOnTopLevelCommas_StringCleanup(splitToBeAdded);
            result.Add(splitToBeAdded);
        }

        return result;

    }

    public static int substringCount(string str, string substr)
    {
        int count = -1;
        int index = 0;
        while (index != -1)
        {
            index = str.IndexOf(substr, index++);
            count++;
        }
        return count;
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


        // Check for any raw images contained within the string
        List<Tuple<int, int>> imgStartsAndEnds = new List<Tuple<int, int>>();
        getImageStartEnds(inStr, out imgStartsAndEnds);


        // Splits on top level pairings.
        // ie: () (()) () ((()())) splits into  [(), (()), (), ((()()))]
        char[] trimChars = { left, right };
        int pairCount = 0;
        int lastL = 0;
        bool needNewL = false;
        char c;
        for (int i = 0; i < inStr.Length; i++)
        {

            // Check for image attribute and handle seperately
            bool contLargerLoop = false;
            foreach (Tuple<int, int> pair in imgStartsAndEnds)
            {
                if (i == pair.Item1)
                {
                    i = pair.Item2 + imgEndToken.Length - 1;
                    contLargerLoop = true;
                }
            }
            if (contLargerLoop) continue;

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

    private static void getImageStartEnds(string inStr, out List<Tuple<int, int>> imgStartsAndEnds)
    {
        imgStartsAndEnds = new List<Tuple<int, int>>();
        if (inStr == null || inStr.Length == 0) return;
        int start = 0;
        int end = 0;
        int count = 0;
        while (start < inStr.Length)
        {
            // 200 is arbitrarily large number.
            // Images would only be searched for on a single item basis
            // Should be <200 images per item
            count++;
            if (count > 200) { throw new Exception("Error: getImageStartEnds: Possible Infinite Loop"); }


            // Find next start and end of an image
            start = inStr.Substring(end).IndexOf(imgStartToken);
            if (start == -1) break;

            end = inStr.Substring(start).IndexOf(imgEndToken) + start;
            if (end == -1) throw new Exception("ERROR: Util.getImageStartEnds: Could not find matching end token for image!");


            // Add start and end, and return where the nearest commas will be
            // assuming the tokene encompass the entire image string
            imgStartsAndEnds.Add(new Tuple<int,int>(start,end));

        }
        

    }

    // Note: Keep this method returning Image. It's only meant to
    // interact with raw images themselves w/o regard to the ImageID
    public static Image rawImageStrToImage(string rawImage)
    {
        if (rawImage == null) { return Util.DEFAULT_IMAGE.image; }
        byte[] ret2 = new byte[rawImage.Length];

        rawImage = rawImage.Trim(new char[] { '[', ']' });

        List<string> s = new List<string>(rawImage.Split(", "));
        for (int j = 0; j < s.Count; j++)
        {
            string elem = s[j];
            ret2[j] = (byte)Int32.Parse(elem);
        }
        return Image.FromStream(new MemoryStream(ret2));
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
            if (s.CompareTo("") == 0) { this = new Date(); return; }

            // Assumed to be format "datetime.date(yyyy, mm, dd)"
            if (s.Length > 14 && s.Substring(0, 14).CompareTo("datetime.date(") == 0)
            {
                List<string> components = new List<string>(s.Split(", "));
                year = Int32.Parse(components[0].Substring(14, (components[0].Length) - 14));
                month = Int32.Parse(components[1]);
                day = Int32.Parse(components[2].Trim(')'));
            }
            // Assumed to be format "yyyy-mm-d"
            else
            {
                List<string> components = new List<string>(s.Split("-"));
                year = Int32.Parse(components[0]);
                month = Int32.Parse(components[1]);
                day = Int32.Parse(components[2]);
            }
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
        
        public DateTime toDateTime()
        {
            // Cannot have DateTime(0,0,0), set to min DateTime 
            if (year == 0 && month == 0 && day == 0)
            {
                return new DateTime(2020, 1, 1);
            }

            return new DateTime(year, month, day);
        }
        
    };

}
