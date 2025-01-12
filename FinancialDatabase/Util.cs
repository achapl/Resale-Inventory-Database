using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public class Util
{
    /// <summary>
    /// Default value used for unassigned init values for a database object
    /// </summary>
    public static int DEFAULT_INT = -1;

    /// <summary>
    /// Default value used for unassigned init values for a database object
    /// </summary>
    public static double DEFAULT_DOUBLE = -1.0;

    /// <summary>
    /// Default value used for unassigned init values for a database object
    /// </summary>
    public static string DEFAULT_STRING = null;

    /// <summary>
    /// Default Date used for unassigned init values for a database object
    /// </summary>
    public static Date DEFAULT_DATE = new Date(-1, -1, -1);

    /// <summary>
    /// Default image used for unassigned init values for a database object
    /// </summary>
    public static MyImage DEFAULT_IMAGE = new MyImage(Image.FromFile(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png"), -1);

    /// <summary>
    /// Default list of images used for unassigned init values for a database object
    /// </summary>
    public static List<MyImage> DEFAULT_IMAGES = new List<MyImage>() {DEFAULT_IMAGE};


    /// <summary>
    /// Token used for finding definitively the start of a raw image returned from python
    /// </summary>
    public static string imgStartToken = "'IMAGESTART***";

    /// <summary>
    /// Token used for finding definitively the start of a raw image returned from python
    /// </summary>
    public static string imgEndToken   = "***IMAGEEND'";
    public Util()
	{
	}

    /// <summary>
    /// Checks if a given string contains at least 1 alpha numeric char
    /// Making a name for an item should require at least 1
    /// </summary>
    /// <param name="str">String to test</param>
    /// <returns>True if str contains at least 1 alpha numeric char</returns>
    public static bool containsAnAlphaNumeric(string str)
    {
        if (str.Length == 0) return false;

        foreach (char c in str)
        {
            int charVal = c;
            if (((charVal >= 48 && charVal <= 57) || // Number
                  (charVal >= 65 && charVal <= 90) || // Uppercase letter
                  (charVal >= 97 && charVal <= 122))) // Lowercase letter
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Wraps String.Split(delim) so it can be returned as a list
    /// </summary>
    /// <param name="str">String to be split</param>
    /// <param name="delim">Delimeter to split on</param>
    /// <returns>List of strings (not including delims) seperated by the delim from the original str</returns>
    public static List<string> mySplit(string str, string delim)
    {
        if (delim is null || delim.CompareTo("") == 0)
        {
            throw new Exception("ERROR: Delimeter is null or empty!");
        }
        string[] arrOfStrings = str.Split(delim);
        List<string> listOfStrings = new List<string>(arrOfStrings);
        return listOfStrings;
    }


    /// <summary>
    /// Turns a Date_Purchased into a string
    /// Formatted for usage in SQL (with escape characters)
    /// ex: List {2024,12,5} -> '\"2024-12-5\"'
    /// </summary>
    /// <param name="dateList">List of ints that represent the Date_Purchased</param>
    /// <returns></returns>
    public static string datetoString(List<int> dateList)
    {
        return "\\\"" + dateList[0] + "-" + dateList[1] + "-" + dateList[2] + "\\\"";
    }

    public static string dateToString(Date d)
    {
        return d.toDateString();
    }


    /// <summary>
    /// Trims only the first matches from a list of given substrings off start/end of a string
    /// Regardless of if the trim at the start and end of the given string are the same
    /// Ex: strToTrim: "Hello Cruel World"
    ///     trimStrs:  {"World", "Lumber", "Cruel", "Hello", "Dog"}
    ///     Returns -> " Cruel "
    /// </summary>
    /// <param name="strToTrim">Given string to trim</param>
    /// <param name="trimStrs">Array of strings to trim off</param>
    /// <returns>Returns original string, but without any strings from the given list that may be at the start/end of the original string</returns>
    public static string myTrim(string strToTrim, string[] trimStrs)
    {
        if (strToTrim == null) { throw new Exception("Error: Null strToTrim"); }
        if (trimStrs == null) { throw new Exception("ERROR: Expected at least empty array for trimStrings, not null"); }

        bool trimmedStart = false;
        bool trimmedEnd = false;

        foreach (string s in trimStrs)
        {
            if (!trimmedStart && strToTrim.StartsWith(s)) 
            {
                trimmedStart = true;
                strToTrim = strToTrim.Remove(strToTrim.IndexOf(s), s.Length);
            }

            if (!trimmedEnd   && strToTrim.EndsWith(s))   
            {
                trimmedEnd = true;  
                strToTrim = strToTrim.Remove(strToTrim.LastIndexOf(s));
            }

            // Algorithm is finished when the first and last parts of the string are trimmed off
            if (trimmedStart && trimmedEnd) { break; }
        }

        return strToTrim;
    }


    /// <summary>
    /// Clears all the text of a given list of labels
    /// </summary>
    /// <param name="l"> List of labels</param>
    public static void clearLabelText(List<Label> l)
    {
        foreach (Label label in l)
        {
            label.Text = "";
        }
    }


    /// <summary>
    /// Clears all the text of a given list of text boxes
    /// And resets their background color to white
    /// </summary>
    /// <param name="t">List of textboxes</param>
    public static void clearTBox(List<TextBox> t)
    {
        foreach (TextBox tb in t)
        {
            tb.Text = "";
            tb.BackColor = Color.White;
        }
    }


    /// <summary>
    /// Clears all the text of a given list of text boxes
    /// And resets their background color to white
    /// </summary>
    /// <param name="c">TextBox given as a control</param>
    public static void clearTBox(Control c)
    {
        if (c is not TextBox) { return; }

        TextBox tb = c as TextBox;
        tb.Text = "";
        tb.BackColor = Color.White;
    }


    /// <summary>
    /// Converts an Amount_purchase of ounces to ounces and pounds
    /// </summary>
    /// <param name="ozs">Number of ounces</param>
    /// <returns>List of 2 ints. First is lbs, and second element is ounces</returns>
    public static List<int> ozToOzLbs(int ozs)
    {
        // Default
        if (ozs < 0)
        {
            throw new Exception("ERROR: Given Ounces is < 0 or Null!");
        }
        int lbs = ozs / 16;
        int oz = ozs - (16 * lbs);
        return new List<int>(new int[] { lbs, oz });
    }


    /// <summary>
    /// Check that a given attribute value (Given as a string) is of the given type
    /// Note: Types are from MySQL, not C#
    /// Ex: "1234.5" for the type "double unsigned" returns true, but false for "int unsigned"
    /// </summary>
    /// <param name="attrib">Value to test</param>
    /// <param name="type">Type that value should match</param>
    /// <returns></returns>
    public static bool checkTypeOkay(string attrib, string type)
    {
        // It is not this function's job to catch null values for data types. That would depend on the way the database table is set up for the corresponding column
        if (attrib == null) { return true; }
        switch (type)
        {
            case "date":
                try
                {
                    new Date(attrib);
                    return true;
                }
                catch
                {
                    return false;
                }

            case "double unsigned":
                return (Double.TryParse(attrib, out double doub) &&
                        doub >= 0);

            case "int unsigned":
                return Int32.TryParse(attrib, out int i) &&
                       i >= 0;

            case "int":
                return Int32.TryParse(attrib, out int _);

            case "varchar(255)":
                return attrib.Length <= 255;

            case "varchar(45)":
                return attrib.Length <= 45;

            case "mediumtext":
                return true;

            case "longblob":
                return true;

            default:
                throw new Exception("ERROR: UNKNOWN TYPE: " + type + "\n For attribute: " + attrib);

        }
    }


    /// <summary>
    /// Helper function for splitOnTopLevelCommas
    /// Will clean up the string by
    /// - removing comma at s[0],
    /// - trim whitespace at start and end,
    /// - trim quotes at s[0] and s[end]
    /// Treats a  substring of a single quote as a valid substring
    /// Since it could theoretically be a "valid" name for an item
    /// 
    /// ex: ', "Item-2"'
    /// Gives: 'Item-2'
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string splitOnTopLevelCommas_StringCleanup(string s)
    {
        if (s.Length == 0) { return ""; }


        // Clean up the split item to be added
        // Remove the comma
        if (s[0] == ',')
        {
            s = s.Substring(1);
        }

        if (s.Length == 0) { return ""; } // Check for edge case, 0 length string after trimming anything off

        // Remove whitespaces
        s = s.Trim();

        if (s.Length == 0) { return ""; }
        // Return string of len 1 since nothing could be trimmed off it further.
        // Ex: A single unmatched quote can be assumed to be a literal title of something since the quote wasn't paired
        if (s.Length == 1) { return s; }
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


    /// <summary>
    /// Split string inStr on commas, except the ones contained in strings inside of the string, encapsulated in quotes
    //   ie: ('2521', 'name "of i\'tem', '43', 435, .etc) will be split into the list (strings represented w/o quotes for simplicity)
    //       { 2521,   name "of i\'tem,   43,  435, .etc }
    /// </summary>
    /// <param name="s">String to be split</param>
    /// <returns>List of the string s split on the top level commas</returns>
    public static List<string> splitOnTopLevelCommas(string s)
    {
        if (s.Length == 0) return new List<string> {};

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
                    throw new Exception("ERROR splitOnTopLevelCommas - current curr <= 0, when it should be past zero for curr: " + i);
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
        if (lastTopLevelComma <= s.Length - 1)
        {
            splitToBeAdded = s.Substring(lastTopLevelComma);
            splitToBeAdded = splitOnTopLevelCommas_StringCleanup(splitToBeAdded);
            result.Add(splitToBeAdded);
        }

        return result;

    }


    /// <summary>
    /// Count number of a certain substrings found within a string
    /// </summary>
    /// <param name="str">String containing substrings</param>
    /// <param name="substr">Substring to count in the main string</param>
    /// <returns>Count of substring finds in the string</returns>
    public static int substringCount(string str, string substr)
    {
        if (substr == null || substr.Length == 0) { throw new Exception("Error: Substring to find is empty"); }
        if (str == null) { throw new Exception("Error: string to search for is null"); }
        if (str.Length == 0) {  return 0; }

        int count = 0;
        int index = 0;
        int curr = 0;
        while (curr < str.Length)
        {
            index = str.IndexOf(substr, curr);
            if (index == -1) { break; }
            curr = index + 1;
            count++;
        }
        return count;
    }

    /// <summary>
    /// Given a string, it will split it on the first character
    // Will split on top level pairings, not any commas
    // Won't work on quotes, both "\"", and "'"
    /// </summary>
    /// <param name="inStr">String to split</param>
    /// <param name="left">'left' of the pair of chars to split on (ex: '(', or '{')</param>
    /// <returns>Returns a List of strings of the split of inStr</returns>
    // 
    public static List<string> pairedCharTopLevelSplit(string inStr, char left)
    {

        if (inStr.Length == 0) return new List<string> { };
        Dictionary<char, char> LRDict = new Dictionary<char, char>() {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
        };

        if (!LRDict.ContainsKey(left))
        {
            throw new Exception("ERROR splitOnFirstChar (SplitOnTopLevelPairs): Unknown Paired-Char to Split on encountered: " + left);
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

            if (pairCount < 0) throw new Exception("\"ERROR splitOnFirstChar (SplitOnTopLevelPairs): Unbalanced start/end pairings. More ends than starts");

            // Even top-level pairing found
            if (pairCount == 0 && !needNewL)
            {
                needNewL = true;
                result.Add(inStr.Substring(lastL, i - lastL + 1).Trim(trimChars));
            }
        }

        if (pairCount != 0)
        {
            throw new Exception("ERROR splitOnFirstChar (SplitOnTopLevelPairs): Unbalanced start/end pairings. More starts than ends");
        }

        return result;
    }


    /// <summary>
    /// For the raw output of a database query that includes images, give a list of tuples that mark the start and end token positions of the images
    /// </summary>
    /// <param name="inStr">raw query output from python containing image start and end tokens</param>
    /// <param name="imgStartsAndEnds">List containing tuples that mark the start and end token positions of the images</param>
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
        if (rawImage.CompareTo("") == 0) { throw new Exception("Error: Empty Image given to convert"); }

        rawImage = rawImage.Trim(new char[] { '[', ']' });
        bool isHex = isStringHex(rawImage);

        string splitToken;
        if (rawImage.Contains(','))
            splitToken = ", ";
        else if (rawImage.Contains('-'))
            splitToken = "-";
        else
            throw new Exception("Error: Unknown split token in raw image: " + rawImage[..10]);

        List<string> s = new List<string>(rawImage.Split(splitToken));
        byte[] ret2 = new byte[s.Count];
        for (int j = 0; j < s.Count; j++)
        {
            string elem = s[j];
            if (isHex)
                ret2[j] = (byte)Int32.Parse(s[j], System.Globalization.NumberStyles.HexNumber);
            else
                ret2[j] = (byte)Int32.Parse(elem);
        }
        MemoryStream m = new MemoryStream(ret2);
        int max = ret2.Max();
        return Image.FromStream(m);
    }

    private static bool isStringHex(string str)
    {
        foreach (char c in str)
        {
            if ((c >= 65 && c <= 70) || // 'A'->'F'
                (c >= 97 && c <= 102)) // 'a'->'f'
            {
                return true; 
            }
        }
        return false;
    }

    public static Dictionary<Control, string> combineDictionaries(params Dictionary<Control, string>[] controlAttribs)
    {
        for (int i = 0; i < controlAttribs.Length-1; i++)
        {
            if (controlAttribs[i] == null || controlAttribs[i+1] == null) throw new Exception("ERROR: Null Dictionary when combining dictionaries!");
            controlAttribs[0] = _combineDictionaries(controlAttribs[i], controlAttribs[i+1]);
        }
        return controlAttribs[0];
    }

    private static Dictionary<Control, string> _combineDictionaries(Dictionary<Control, string> controlAttrib1,
                                                                   Dictionary<Control, string> controlAttrib2)
    {
        foreach (KeyValuePair<Control, string> kvp in controlAttrib2)
        {
            if (controlAttrib1.ContainsKey(kvp.Key))
            {
                throw new Exception("Error: Combining dictionaries with multiple equivalant keys");
            }

            controlAttrib1.Add(kvp.Key, kvp.Value);
        }
        return controlAttrib1;

    }

    public struct Date
    {

        public int year, month, day;

        public Date(int y, int m, int d)
        {
            
            checkValidDate(y,m,d);
            this.year = y;
            this.month = m;
            this.day = d;
        }

        public Date(string s)
        {
            if (s.CompareTo("") == 0) { throw new Exception("ERROR: Empty Date Given"); }

            // Assumed to be format "datetime.Date_Purchased(yyyy, (m)m, (d)d)"
            if (s.Length > 14 && s.Substring(0, 14).CompareTo("datetime.date(") == 0)
            {
                List<string> components = new List<string>(s.Split(", "));
                year = Int32.Parse(components[0].Substring(14, (components[0].Length) - 14));
                month = Int32.Parse(components[1]);
                day = Int32.Parse(components[2].Trim(')'));
            }
            // Assumed to be format "yyyy-mm-dd"
            else
            {
                List<string> components = new List<string>(s.Split("-"));
                year = Int32.Parse(components[0]);
                month = Int32.Parse(components[1]);
                day = Int32.Parse(components[2]);
            }

            this = new Date(year, month, day);
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


        /// <summary>
        /// Formats Y M D into a string yyyy-mm-dd
        /// Note: Should not restrict input to only Date_Purchased-valid integers since this method should work for "invalid" dates that the user may want to display intentionally
        // This is espically true since this func is also used to test constructors for invalid dates.
        /// </summary>
        public static string toDateString(int y, int m, int d)
        {
            return y + "-" + m + "-" + d;
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


        private void checkValidDate(int y, int m, int d)
        {
            if (y == -1 && m == -1 && d == -1) { return; } // -1/ -1/ -1 is fine for default Date_Purchased
            try
            {
                new DateTime(y, m, d);
            }
            catch
            {
                throw new Exception("ERROR: Invalid Date: Y-" + year + " M-" + month + " D-" + day);
            }
        }
    };

}
