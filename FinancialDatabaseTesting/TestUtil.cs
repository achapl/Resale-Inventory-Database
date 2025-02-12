namespace FinancialDatabaseTesting;

using FinancialDatabase;
using NUnit.Framework.Internal.Execution;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ObjectiveC;
using System.Windows.Forms;
using FormsLabel = System.Windows.Forms.Label;

public class TestUtil
{

    [Test]
    public void Test_DefaultVal_Int()
    {
        Assert.AreEqual(-1, Util.DEFAULT_INT);
    }

    [Test]
    public void Test_DefaultVal_Double()
    {
        Assert.AreEqual(-1.0, Util.DEFAULT_DOUBLE);
    }

    [Test]
    public void Test_DefaultVal_String()
    {
        Assert.AreEqual(null, Util.DEFAULT_STRING);
    }

    [Test]
    public void Test_DefaultVal_Date()
    {
        Assert.IsTrue(Util.DEFAULT_DATE.Equals(new Util.Date(-1, -1, -1)));
    }

    [Test]
    public void Test_DefaultVal_Image()
    {
        Assert.IsTrue(TestingUtil.compareImages(TestingUtil.defImage, Util.DEFAULT_IMAGE.image, false));
        Assert.AreEqual(-1, Util.DEFAULT_IMAGE.imageID);
    }

    [Test]
    public void Test_DefaultVal_Images()
    {
        Assert.IsTrue(TestingUtil.compareImages(TestingUtil.defImage, Util.DEFAULT_IMAGES[0].image, false));
        Assert.AreEqual(-1, Util.DEFAULT_IMAGES[0].imageID);
    }



    [TestCase("", false)]
    [TestCase(" ", false)]
    [TestCase("!", false)]
    [TestCase("@", false)]
    [TestCase("#", false)]
    [TestCase("$", false)]
    [TestCase("%", false)]
    [TestCase("^", false)]
    [TestCase("&", false)]
    [TestCase("*", false)]
    [TestCase("(", false)]
    [TestCase("-", false)]
    [TestCase("_", false)]
    [TestCase("=", false)]
    [TestCase("+", false)]
    [TestCase("`", false)]
    [TestCase("~", false)]
    [TestCase("1", true)]
    [TestCase("A", true)]
    [TestCase("a", true)]
    [TestCase("\t", false)]
    [TestCase("\n", false)]
    [TestCase("\b", false)]
    [TestCase("\t\t\t", false)]
    [TestCase("\n\n\n", false)]
    [TestCase("\b\b\b", false)]
    [TestCase("      ", false)]
    [TestCase("!@#$%^&*", false)]
    [TestCase("Hello World", true)]
    [TestCase("Hello World!", true)]
    [TestCase("!Hello World", true)]
    [TestCase("Hello !World", true)]
    [TestCase("$%^&*($%^&a$%^&", true)] // contains 'a' in middle
    public void Test_containsAnAlphaNumeric(string str, bool containsAlpNum)
    {
        Assert.AreEqual(containsAlpNum, Util.containsAnAlphaNumeric(str));
    }



    public static object[] mySplitCases =
    {
        new object[] {"",",", new List<string> { "" } },
        new object[] {" ",",", new List<string> { " " } },
        new object[] {"Hello",",", new List<string> { "Hello" } },
        new object[] {"World!",",", new List<string> { "World!" } },
        new object[] {"Hello,World!",",", new List<string> { "Hello", "World!" } },
        new object[] {"Hello,World,This,Is,A,Test", ",", new List<string> { "Hello", "World", "This", "Is", "A", "Test" } },
        new object[] {"Hello,World,This,Is,A,Test,", ",", new List<string> { "Hello", "World", "This", "Is", "A", "Test", "" } },
        //new object[] {"",",", new List<string> { "" } }
    };

    [TestCaseSource(nameof(mySplitCases))]
    public void Test_mySplit(string str, string delim, List<string> output)
    {
        Assert.AreEqual(output, Util.mySplit(str, delim));
    }



    public static object[] mySplicCasesEXCEPTIONS =
    {
        new object[] {"Goodbye World!", "" },
        new object[] {null!, ""},
        new object[] {"", ""} // delim of len 0
    };

    [TestCaseSource(nameof(mySplicCasesEXCEPTIONS))]
    public void Test_mySplitEXCEPTIONS(string str, string delim)
    {
        Assert.Catch<Exception>(() => Util.mySplit(str, delim));
    }



    public static object[] myTrimCases =
    {
        new object[] {"", new string[] {}, ""},
        new object[] {"", new string[] { "" }, "" },
        new object[] {"", new string[] { "Hello" }, "" },
        new object[] {"Hello", new string[] {}, "Hello" },
        new object[] {"Hel", new string[] { "Hello" }, "Hel" },
        new object[] {"Hello Cruel World", new string[] { "Hello" }, " Cruel World" },
        new object[] {"Hello Cruel World", new string[] { "Hello", "World" }, " Cruel " },
        new object[] {"Hello Cruel World", new string[] { "World", "Hello" }, " Cruel " },
        new object[] {"Hello Cruel World", new string[] { "World", "Lumber", "Cruel", "Hello", "Dog" }, " Cruel " },
        new object[] {"Hello Hello Cruel World", new string[] { "World", "Hello" }, " Cruel " },
        new object[] {"Hello Hello Cruel World", new string[] { "World", "Hello", "Hello" }, " Cruel " },
    };
    [TestCaseSource(nameof(myTrimCases))]
    public void Test_myTrim(string strToTrim, string[] trimStrs, string resultStr)
    {
        StringAssert.Contains(resultStr, Util.myTrim(strToTrim, trimStrs));
    }

    public static object[] myTrimCasesEXCEPTIONS =
    {
        new object[] {"", null },

        new object[] {null, new string[] { "" }},
    };
    [TestCaseSource(nameof(myTrimCasesEXCEPTIONS))]
    public void Test_MyTrimEXCEPTIONS(string strToTrim, string[] trimStrs)
    {
        Assert.Catch<Exception>(() => Util.myTrim(strToTrim, trimStrs));
    }



    public static object[] clearLabelTextCases =
    {
        new object[] {new List<FormsLabel>{}, "" },
        new object[] {new List<FormsLabel>{new FormsLabel()}, "abcd" },
        new object[] {new List<FormsLabel>{new FormsLabel()}, "Test123" },
        new object[] {new List<FormsLabel>{new FormsLabel(), new FormsLabel() }, "Test1234" },
        new object[] {new List<FormsLabel>{new FormsLabel(), new FormsLabel(), new FormsLabel()}, "Test12345" },
    };

    [TestCaseSource(nameof(clearLabelTextCases))]
    public static void Test_clearLabelText(List<FormsLabel> labels, string generatedText)
    {
        foreach (FormsLabel label in labels)
        {
            label.Text = generatedText;
        }

        Util.clearLabelText(labels);
        foreach (FormsLabel label in labels)
        {
            Assert.AreEqual(0, label.Text.CompareTo(""));
        }
    }



    public static object[] clearTBoxTextCases =
    {
        new object[] {new List<TextBox>{}, "" },
        new object[] {new List<TextBox>{new TextBox()}, "abcd" },
        new object[] {new List<TextBox>{new TextBox()}, "Test123" },
        new object[] {new List<TextBox>{new TextBox(), new TextBox() }, "Test1234" },
        new object[] {new List<TextBox>{new TextBox(), new TextBox(), new TextBox()}, "Test12345" },
    };

    [TestCaseSource(nameof(clearTBoxTextCases))]
    public static void Test_clearTBox(List<TextBox> tBoxes, string generatedText)
    {
        foreach (TextBox tBox in tBoxes)
        {
            tBox.Text = generatedText;
            tBox.BackColor = Color.Gray;
        }

        Util.clearTBox(tBoxes);
        foreach (TextBox tBox in tBoxes)
        {
            Assert.AreEqual(0, tBox.Text.CompareTo(""));
            Assert.IsTrue(tBox.BackColor.Equals(Color.White));
        }
    }



    public static object[] clearTBoxTextCasesControl =
    {
        new object[] { new TextBox(), "" },
        new object[] { new TextBox(), "abcd" },
        new object[] { new TextBox(), "Test123" },
        new object[] { new TextBox(), "Test1234" },
        new object[] { new TextBox(), "Test12345" },
    };

    [TestCaseSource(nameof(clearTBoxTextCasesControl))]
    public static void Test_clearTBoxControl(Control tBox, string generatedText)
    {
        tBox.Text = generatedText;
        tBox.BackColor = Color.Gray;

        Util.clearControl(tBox);

        Assert.AreEqual(0, tBox.Text.CompareTo(""));
        Assert.IsTrue(tBox.BackColor.Equals(Color.White));
    }



    [TestCase(0, 0, 0)]
    [TestCase(1, 0, 1)]
    [TestCase(2, 0, 2)]
    [TestCase(3, 0, 3)]
    [TestCase(4, 0, 4)]
    [TestCase(5, 0, 5)]
    [TestCase(6, 0, 6)]
    [TestCase(7, 0, 7)]
    [TestCase(8, 0, 8)]
    [TestCase(9, 0, 9)]
    [TestCase(10, 0, 10)]
    [TestCase(11, 0, 11)]
    [TestCase(12, 0, 12)]
    [TestCase(13, 0, 13)]
    [TestCase(14, 0, 14)]
    [TestCase(15, 0, 15)]
    [TestCase(16, 1, 0)]
    [TestCase(17, 1, 1)]
    [TestCase(18, 1, 2)]
    public static void Test_ozToOzLbs(int oz, int ttlLbs, int ttlOz)
    {
        List<int> result = Util.ozToOzLbs(oz);
        int result_Lbs = result[0];
        int result_oz = result[1];

        Assert.AreEqual(ttlLbs, result_Lbs, "Wrong Amount of Lbs!");
        Assert.AreEqual(ttlOz, result_oz, "Wrong Amount of Oz!");
    }



    public static object[] ozToLbsEXCEPTIONS = {
        new object[] { -1 },
        new object[] { -2 },
        new object[] { -4 },
        new object[] { -9 },
        new object[] { -16 },
        new object[] { -17 },
    };

    [TestCaseSource(nameof(ozToLbsEXCEPTIONS))]
    public static void Test_ozToOzLbsEXCEPTIONS(int oz)
    {
        Assert.Catch<Exception>(() => Util.ozToOzLbs(oz));
    }



    // double unsigned
    [TestCase("3.14", "double unsigned", true)]
    [TestCase("0.0", "double unsigned", true)]
    [TestCase("0", "double unsigned", true)]
    [TestCase("3", "double unsigned", true)]

    [TestCase("-.01", "double unsigned", false)]
    [TestCase("-1", "double unsigned", false)]
    [TestCase("", "double unsigned", false)]

    // int unsigned
    [TestCase("15", "int unsigned", true)]
    [TestCase("0", "int unsigned", true)]
    [TestCase("1", "int unsigned", true)]
    [TestCase("3", "int unsigned", true)]

    [TestCase("-1", "int unsigned", false)]
    [TestCase("-2", "int unsigned", false)]
    [TestCase("", "int unsigned", false)]

    // varchar (255)
    /* 255 chars, true */
    [TestCase("1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij123456789012345", "varchar(255)", true)]
    /* 256 chars, false */
    [TestCase("01234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij123456789012345", "varchar(255)", false)]
    [TestCase("\n\n\r\n\\\\\\\\\\\\\0", "varchar(255)", true)]
    [TestCase("\n\n\r\n\\\\\\\\\\\\0", "varchar(255)", true)]
    [TestCase("atbtsd4s", "varchar(255)", true)]
    [TestCase("Null", "varchar(255)", true)]
    [TestCase("null", "varchar(255)", true)]
    [TestCase(@"\\\0", "varchar(255)", true)]
    [TestCase(@"\\0", "varchar(255)", true)]
    [TestCase(@"\0", "varchar(255)", true)]
    [TestCase("", "varchar(255)", true)]
    // varchar(45)
    [TestCase("1234567890abcdefghij1234567890abcdefghij12345", "varchar(45)", true)]
    [TestCase("01234567890abcdefghij1234567890abcdefghij12345", "varchar(45)", false)]
    [TestCase("", "varchar(45)", true)]
    [TestCase("\n\n\r\n\\\\\\\\\\\\\0", "varchar(45)", true)]
    [TestCase("\n\n\r\n\\\\\\\\\\\\0", "varchar(45)", true)]
    [TestCase("atbtsd4s", "varchar(45)", true)]
    [TestCase("Null", "varchar(45)", true)]
    [TestCase("null", "varchar(45)", true)]
    [TestCase(@"\\\0", "varchar(45)", true)]
    [TestCase(@"\\0", "varchar(255)", true)]
    [TestCase(@"\0", "varchar(45)", true)]
    [TestCase("", "varchar(45)", true)]
    // mediumtext
    [TestCase("1234567890abcdefghij1234567890abcdefghij12345", "mediumtext", true)]
    [TestCase("", "mediumtext", true)]
    [TestCase("\n\n\r\n\\\\\\\\\\\\\0", "mediumtext", true)]
    [TestCase("\n\n\r\n\\\\\\\\\\\\0", "mediumtext", true)]
    [TestCase("atbtsd4s", "mediumtext", true)]
    [TestCase("Null", "mediumtext", true)]
    [TestCase("null", "mediumtext", true)]
    [TestCase(@"\\\0", "mediumtext", true)]
    [TestCase(@"\\0", "varchar(255)", true)]
    [TestCase(@"\0", "mediumtext", true)]
    [TestCase("", "mediumtext", true)]
    // longblob
    [TestCase("1234567890abcdefghij1234567890abcdefghij12345", "longblob", true)]
    [TestCase("", "longblob", true)]
    [TestCase("\n\n\r\n\\\\\\\\\\\\\0", "longblob", true)]
    [TestCase("\n\n\r\n\\\\\\\\\\\\0", "longblob", true)]
    [TestCase("atbtsd4s", "longblob", true)]
    [TestCase("Null", "longblob", true)]
    [TestCase("null", "longblob", true)]
    [TestCase(@"\\\0", "longblob", true)]
    [TestCase(@"\\0", "varchar(255)", true)]
    [TestCase(@"\0", "longblob", true)]
    [TestCase("", "longblob", true)]
    // date
    [TestCase("datetime.date(1978, 92, 16)", "date", false)]
    [TestCase("datetime.date(1978, 12, 96)", "date", false)]
    [TestCase("datetime.date(1978, 13, 16)", "date", false)]
    [TestCase("datetime.date(1978, 11, 46)", "date", false)]
    [TestCase("datetime.date(1978, 12, 16)", "date", true)]
    [TestCase("datetime.date(1978, 02, 16)", "date", true)]
    [TestCase("datetime.date(1978, 2, 16)", "date", true)]
    [TestCase("datetime.date(1978, 12, 6)", "date", true)]
    [TestCase("datetime.date(78, 12, 16)", "date", true)]
    [TestCase("datetime.date(78, 2, 16)", "date", true)]
    [TestCase("1978-92-16", "date", false)]
    [TestCase("1978-12-96", "date", false)]
    [TestCase("1978-13-16", "date", false)]
    [TestCase("1978-12-46", "date", false)]
    [TestCase("1978-12-16", "date", true)]
    [TestCase("1978-02-16", "date", true)]
    [TestCase("1978-2-16", "date", true)]
    [TestCase("1978-12-6", "date", true)]
    [TestCase("78-12-16", "date", true)]
    [TestCase("78-2-16", "date", true)]
    // int
    [TestCase("941240712", "int", true)]
    [TestCase("-1", "int", true)]
    [TestCase("0", "int", true)]
    [TestCase("1", "int", true)]
    public static void Test_checkTypeOkay(string attrib, string type, bool typeOkay)
    {
        Assert.AreEqual(typeOkay, Util.checkTypeOkay(attrib, type));
    }



    [TestCase("adfa44", "shorttext")]
    [TestCase("23425.234", "float")]
    public static void Test_checkTypeOkayEXCEPTION(string attrib, string type)
    {
        Assert.Catch<Exception>(() => Util.checkTypeOkay(attrib, type));
    }



    [TestCase(", \"Item-2\"", "Item-2")]
    [TestCase(" \"Item-2\"", "Item-2")]
    [TestCase(", 'Item-2'", "Item-2")]
    [TestCase("\"Item-2\"", "Item-2")]
    [TestCase("Item-2\"", "Item-2\"")]
    [TestCase(", \"It\"em-2\"", "It\"em-2")] // leave quotation in the middle of the string
    [TestCase(", \"\"", "")]
    [TestCase("\"\"", "")]
    [TestCase("\"", "\"")]
    [TestCase(", ", "")]
    [TestCase(",", "")]
    [TestCase("", "")]
    public static void Test_splitOnTopLevelCommas_StringCleanup(string s, string expected)
    {
        Assert.AreEqual(0, expected.CompareTo(Util.splitOnTopLevelCommas_StringCleanup(s)));
    }




    public static object[] splitOnTopLevelCommasCases = {
        new object[] {"This,string, is,a ,test", new List<string> {"This", "string", "is", "a", "test"} },
        new object[] {"This,string,is,a,test", new List<string> {"This", "string", "is", "a", "test"} },
        new object[] {", ", new List<string> {"",""} },
        new object[] {",", new List<string> {"",""} },
        new object[] {"", new List<string> { } },
    };
    [TestCaseSource(nameof(splitOnTopLevelCommasCases))]
    public static void Test_splitOnTopLevelCommas(string s, List<string> expected)
    {
        List<string> result = Util.splitOnTopLevelCommas(s);

        Assert.AreEqual(expected, result);
    }



    [TestCase("CatterpillarCaterpillarcatCat", "Cat", 3)]
    [TestCase("1231231231231", "1", 5)]
    [TestCase("abcDog", "Dog", 1)]
    [TestCase("Dogabc", "Dog", 1)]
    [TestCase("DogDo", "Dog", 1)]
    [TestCase("Dog", "Dog", 1)]
    [TestCase("dog", "Dog", 0)]
    [TestCase("Dg", "Dog", 0)]
    [TestCase("D", "Dog", 0)]
    [TestCase("g", "Dog", 0)]
    [TestCase("", "Dog", 0)]

    public static void Test_substringCount(string str, string substring, int count)
    {
        Assert.AreEqual(count, Util.substringCount(str, substring));
    }



    public static object[] substringCountEXCEPTIONS =
    {
        new object[] { null, "Dog"},
        new object[] { "Dog", null},
        new object[] { null, null},
        new object[] { "Dog", ""},
    };
    [TestCaseSource(nameof(substringCountEXCEPTIONS))]
    public static void Test_substringCountEXCEPTIONS(string str, string substring)
    {
        Assert.Catch<Exception>(() => Util.substringCount(str, substring));
    }



    public static object[] pairedCharTopLevelSplitCases =
    {
        new object[] { "(,z,),(,a(,b,),c,),(,d,),(,e,(,f,(,g,),h,(,i,),j,),k,)", '(', new List<String> { ",z,", ",a(,b,),c,", ",d,", ",e,(,f,(,g,),h,(,i,),j,),k,"} },
        new object[] {"[This],[ is a test]", '[', new List<String> {"This", " is a test"} },
        new object[] {"[Cat]asfaslfijat[Dog]", '[', new List<string> { "Cat", "Dog"} },
        new object[] {"{}",'{', new List<string>{ "" } },
        new object[] {"", '(', new List<string>{ } },
    };
    [TestCaseSource(nameof(pairedCharTopLevelSplitCases))]
    public static void Test_pairedCharTopLevelSplit(string input, char left, List<string> expected)
    {
        Assert.AreEqual(expected, Util.pairedCharTopLevelSplit(input, left));
    }



    public static object[] pairedCharTopLevelSplitCasesEXCEPTIONS =
    {
        new object[] { "(,z,),(,a(,b,),c,),(,d,),(,e,(,f,(,g,),h,(,i,),j,),k,", '('}, // Unpaired outer-most paren
        new object[] { null, '(' },
        new object[] { "[][[]]]]", '['},
        new object[] { "())", '('},
        new object[] { "(()", '('},
        new object[] { "Test String", 'A'},
    };

    [TestCaseSource(nameof(pairedCharTopLevelSplitCasesEXCEPTIONS))]
    public static void Test_pairedCharTopLevelSplitEXCEPTIONS(string input, char left)
    {
        Assert.Catch<Exception>(() => Util.pairedCharTopLevelSplit(input, left));
    }



    public static object[] combineDictionariesCases =
    {
        new object[] { new Dictionary<Control, string> { { new FormsLabel(), "One"  }, { new FormsLabel(), "Two" }, { new FormsLabel(), "Three" }, { new FormsLabel(), "Four" } },
                       new Dictionary<Control, string> { { new FormsLabel(), "Five" }, { new FormsLabel(), "Six" }, { new FormsLabel(), "Seven" }, { new FormsLabel(), "Eight" }, },

                       new Dictionary<Control, string> { { new FormsLabel(), "One"  }, { new FormsLabel(), "Two" }, { new FormsLabel(), "Three" }, { new FormsLabel(), "Four" },
                                                         { new FormsLabel(), "Five" }, { new FormsLabel(), "Six" }, { new FormsLabel(), "Seven" }, { new FormsLabel(), "Eight" }, }
        },

        new object[] { new Dictionary<Control, string> { { new FormsLabel(), "One"  }, { new FormsLabel(), "Two" }, { new FormsLabel(), "Three" }, { new FormsLabel(), "Four" } },
                       new Dictionary<Control, string> {},

                       new Dictionary<Control, string> { { new FormsLabel(), "One"  }, { new FormsLabel(), "Two" }, { new FormsLabel(), "Three" }, { new FormsLabel(), "Four" },}
        },

        new object[] { new Dictionary<Control, string> {},
                       new Dictionary<Control, string> { { new FormsLabel(), "Five" }, { new FormsLabel(), "Six" }, { new FormsLabel(), "Seven" }, { new FormsLabel(), "Eight" }, },

                       new Dictionary<Control, string> { { new FormsLabel(), "Five" }, { new FormsLabel(), "Six" }, { new FormsLabel(), "Seven" }, { new FormsLabel(), "Eight" }, }
        },

        new object[] { new Dictionary<Control, string> {},
                       new Dictionary<Control, string> {},

                       new Dictionary<Control, string> {}
        },
    };

    [TestCaseSource(nameof(combineDictionariesCases))]
    public static void Test_combineDictionaries(Dictionary<Control, string> dict1, Dictionary<Control, string> dict2, Dictionary<Control, string> expected)
    {
        Dictionary<Control, string> result = Util.combineDictionaries(dict1, dict2);

        Assert.AreEqual(expected.Count, result.Count);

        foreach(KeyValuePair<Control, string> kv in result)
        {
            Assert.AreEqual(kv.Value, result[kv.Key]);
        }
    }



    public static object[] combineDictionariesCasesEXCEPTIONS =
    {
        new object[] { new Dictionary<Control, string> { { new FormsLabel(), "One"  }, { new FormsLabel(), "Two" }, { new FormsLabel(), "Three" }, { new FormsLabel(), "Four" } },
                       null,
        },

        new object[] { null,
                       new Dictionary<Control, string> { { new FormsLabel(), "Five" }, { new FormsLabel(), "Six" }, { new FormsLabel(), "Seven" }, { new FormsLabel(), "Eight" }, },
        },

        new object[] { null,
                       null
        },
    };

    [TestCaseSource(nameof(combineDictionariesCasesEXCEPTIONS))]
    public static void Test_combineDictionariesEXCEPTIONS(Dictionary<Control, string> dict1, Dictionary<Control, string> dict2)
    {
        Assert.Catch<Exception>(() => Util.combineDictionaries(dict1, dict2));
    }



    [TestCase(1753, 2,3)]
    public static void Test_DateConstructorYMD(int y, int m, int d)
    {
        Util.Date result = new Util.Date(y, m, d);
        Assert.AreEqual(m, result.month);
        Assert.AreEqual(y, result.year);
        Assert.AreEqual(d, result.day);
    }



    [TestCase(1, 2, 3)]
    public static void Test_DateConstructorSTRING(int y, int m, int d)
    {
        string dateStr = Util.Date.toDateString(y, m, d);
        Util.Date result = new Util.Date(dateStr);
        Assert.AreEqual(m, result.month);
        Assert.AreEqual(y, result.year);
        Assert.AreEqual(d, result.day);
    }



    [TestCase(1753, 2, 3)]
    // Constructor that takes a control that is a DateTimePicker
    // Here the control is a DateTimePicker
    public static void Test_DateConstructorCONTROL_DATETIMEPICKER(int y, int m, int d)
    {
        DateTimePicker date = new DateTimePicker();
        date.Value = new DateTime(y, m, d);
        Control controlWrapper = date;
        Util.Date result = new Util.Date(controlWrapper);
        Assert.AreEqual(m, result.month);
        Assert.AreEqual(y, result.year);
        Assert.AreEqual(d, result.day);
    }



    [Test]
    // Constructor that takes a control that is a DateTimePicker
    // Here the control is NOT a DateTimePicker
    public static void Test_DateConstructorCONTROLNOT_DATETIMEPICKER_EXCEPTIONS()
    {
        Assert.Catch<Exception>(() => new Util.Date(new FormsLabel()));
    }



    [TestCase(1753,2,3)]
    public static void Test_DateConstructorDATETIMEPICKER(int y, int m, int d)
    {
        DateTimePicker date = new DateTimePicker();
        date.Value = new DateTime(y, m, d);
        Util.Date result = new Util.Date(date);
        Assert.AreEqual(m, result.month);
        Assert.AreEqual(y, result.year);
        Assert.AreEqual(d, result.day);
    }


    [TestCase(1978,92,16, "1978-92-16")]
    [TestCase(1978,12,96, "1978-12-96")]
    [TestCase(1978,13,16, "1978-13-16")]
    [TestCase(1978,12,46, "1978-12-46")]
    [TestCase(1978,12,16, "1978-12-16")]
    [TestCase(1978,2,16,  "1978-2-16")]
    [TestCase(1978,12,6,  "1978-12-6")]
    public static void Test_toDateString(int y, int m, int d, string expected)
    {
        Assert.AreEqual(0, expected.CompareTo(Util.Date.toDateString(y,m,d)));
    }



    [TestCase(1978, 12, 16)]
    public static void Test_toDateTime(int y, int m, int d)
    {
        DateTime control = new DateTime(y, m, d);
        Util.Date test  = new Util.Date(y, m, d);
        Assert.AreEqual(control, test.toDateTime());
    }
}