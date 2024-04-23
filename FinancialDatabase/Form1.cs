using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinancialDatabase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Remove escape characters
        private string formatQuery(string query)
        {
            string escapeChar = "^";
            // Add '^' before special characters ('*', '<', '>', .etc), 


            StringBuilder sb = new StringBuilder(query, 1024);
            int count = 0;
            // Note: for edge case, special char at start of string, copy swithc inside for loop, and modify it outside for case i=0
            for (int i = 1; i < query.Length; i++)
            {
                switch (query[i])
                {
                    case '*':
                    case '<':
                    case '>':
                    case '&':
                    case '"':
                        sb.Insert(i+count, escapeChar);
                        count++;
                        break;
                }
            }

            return sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const string DEFAULTQUERY = "SELECT * FROM city where ID < 5";

            // Activate Python Script 'Pyscript.py' as process 'p'
            string query = this.textBox1.Text;
            if (query == "")
            {
                query = DEFAULTQUERY;
            }

            query = formatQuery(query);

            Console.WriteLine(query);
            string cmdText = @"/K python C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Pyscript.py " + query;
            Console.WriteLine(cmdText);
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "CMD.exe";
            p.StartInfo.Arguments = cmdText;
            p.Start();
            

            // Show redirected output of process 'p's standard output
            String s   = p.StandardOutput.ReadLine();
            String eos = "EOS";     // end-of-stream
            
            // Until end of stream is shown, keep writing the next line to the console, and adding it to the listBox1
            while(s.CompareTo(eos) != 0)
            {                 

                Console.WriteLine(s);
                this.listBox1.Items.Add(s);
                s = p.StandardOutput.ReadLine();

            }

            // End process 'p'
            try
            {
                p.Kill();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
