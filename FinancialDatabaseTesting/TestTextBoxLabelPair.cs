using FinancialDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialDatabaseTesting
{
    internal class TestTextBoxLabelPair
    {
        static TextBoxLabelPair tlp;
        static Form1 form1;

        [SetUp]
        public static void SetUp()
        {
            tlp = new TextBoxLabelPair();
            form1 = new Form1();

            form1.Controls.Add(tlp);
        }

        [TearDown]
        public static void TearDown()
        {
            if (tlp != null)
            {
                tlp.Dispose();
            }

            if (form1 != null)
            {
                form1.Dispose();
            }
        }

        [Test]
        public static void Test_Constructor()
        {
            Assert.IsTrue(form1.Controls.Contains(tlp));
            // Test that label is created by
            // seeing if text is accessible w/o error
            tlp.getLabelText();
            Assert.IsFalse(tlp.Visible);
        }


        [Test]
        public static void Test_editMode()
        {
            string expected = tlp.getLabelText();

            tlp.editMode();
            Assert.IsTrue(tlp.inEditMode);
            MyAssert.StringsEqual(expected, tlp.Text);
        }

        [Test]
        public static void Test_viewMode()
        {
            string expected = tlp.Text;

            tlp.viewMode();
            Assert.IsFalse(tlp.inEditMode);
            MyAssert.StringsEqual(expected, tlp.getLabelText());
        }


        [Test]
        public static void Test_flipVisibility()
        {
            bool priorEditMode = tlp.inEditMode;
            tlp.flipEditMode();
            Assert.IsTrue(priorEditMode != tlp.inEditMode);

            priorEditMode = tlp.inEditMode;
            tlp.flipEditMode();
            Assert.IsTrue(priorEditMode != tlp.inEditMode);
        }


        [Test]
        public static void Test_attribSET()
        {
            tlp.attrib = "Test";
            Assert.Catch<Exception>(() => tlp.attrib = "Test2");
        }
    }
}
