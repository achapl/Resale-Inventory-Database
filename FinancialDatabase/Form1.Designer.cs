namespace FinancialDatabase
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            manualQuery = new Button();
            listBox1 = new ListBox();
            textBox1 = new TextBox();
            tabControl1 = new TabControl();
            SearchTab = new TabPage();
            checkBox5 = new CheckBox();
            checkBox3 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            label2 = new Label();
            label1 = new Label();
            dateTimePicker2 = new DateTimePicker();
            dateTimePicker1 = new DateTimePicker();
            searchButton = new Button();
            searchBox = new TextBox();
            comboBox1 = new ComboBox();
            menuStrip1 = new MenuStrip();
            ItemTab = new TabPage();
            label43 = new Label();
            button5 = new Button();
            label40 = new Label();
            button4 = new Button();
            button1 = new Button();
            label26 = new Label();
            label25 = new Label();
            label24 = new Label();
            label23 = new Label();
            label22 = new Label();
            label21 = new Label();
            label20 = new Label();
            label19 = new Label();
            label18 = new Label();
            label17 = new Label();
            textBox3 = new TextBox();
            label14 = new Label();
            label13 = new Label();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            textBox10 = new TextBox();
            textBox9 = new TextBox();
            textBox8 = new TextBox();
            textBox7 = new TextBox();
            textBox6 = new TextBox();
            textBox5 = new TextBox();
            textBox4 = new TextBox();
            PurchaseTab = new TabPage();
            label44 = new Label();
            textBox11 = new TextBox();
            label42 = new Label();
            button7 = new Button();
            label41 = new Label();
            label15 = new Label();
            button6 = new Button();
            button3 = new Button();
            textBox21 = new TextBox();
            label37 = new Label();
            label36 = new Label();
            dateTimePicker4 = new DateTimePicker();
            textBox20 = new TextBox();
            textBox19 = new TextBox();
            textBox18 = new TextBox();
            textBox17 = new TextBox();
            textBox16 = new TextBox();
            textBox15 = new TextBox();
            textBox14 = new TextBox();
            textBox2 = new TextBox();
            label35 = new Label();
            label34 = new Label();
            label33 = new Label();
            label32 = new Label();
            label31 = new Label();
            label30 = new Label();
            label29 = new Label();
            label28 = new Label();
            label27 = new Label();
            button2 = new Button();
            label16 = new Label();
            listBox2 = new ListBox();
            Sale = new TabPage();
            button10 = new Button();
            label54 = new Label();
            button9 = new Button();
            label53 = new Label();
            label52 = new Label();
            dateTimePicker5 = new DateTimePicker();
            label51 = new Label();
            label50 = new Label();
            dateTimePicker3 = new DateTimePicker();
            label48 = new Label();
            textBox13 = new TextBox();
            label46 = new Label();
            button8 = new Button();
            textBox22 = new TextBox();
            label38 = new Label();
            listBox3 = new ListBox();
            resultItemBindingSource = new BindingSource(components);
            button11 = new Button();
            tabControl1.SuspendLayout();
            SearchTab.SuspendLayout();
            ItemTab.SuspendLayout();
            PurchaseTab.SuspendLayout();
            Sale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resultItemBindingSource).BeginInit();
            SuspendLayout();
            // 
            // manualQuery
            // 
            manualQuery.Location = new Point(405, 428);
            manualQuery.Margin = new Padding(4, 3, 4, 3);
            manualQuery.Name = "manualQuery";
            manualQuery.Size = new Size(110, 27);
            manualQuery.TabIndex = 1;
            manualQuery.Text = "Manual Query";
            manualQuery.UseVisualStyleBackColor = true;
            manualQuery.Click += button1_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(538, 42);
            listBox1.Margin = new Padding(4, 3, 4, 3);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(564, 409);
            listBox1.TabIndex = 2;
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(7, 432);
            textBox1.Margin = new Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(390, 23);
            textBox1.TabIndex = 4;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(SearchTab);
            tabControl1.Controls.Add(ItemTab);
            tabControl1.Controls.Add(PurchaseTab);
            tabControl1.Controls.Add(Sale);
            tabControl1.Location = new Point(14, 14);
            tabControl1.Margin = new Padding(4, 3, 4, 3);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1119, 492);
            tabControl1.TabIndex = 5;
            // 
            // SearchTab
            // 
            SearchTab.Controls.Add(checkBox5);
            SearchTab.Controls.Add(checkBox3);
            SearchTab.Controls.Add(listBox1);
            SearchTab.Controls.Add(checkBox2);
            SearchTab.Controls.Add(checkBox1);
            SearchTab.Controls.Add(label2);
            SearchTab.Controls.Add(label1);
            SearchTab.Controls.Add(dateTimePicker2);
            SearchTab.Controls.Add(dateTimePicker1);
            SearchTab.Controls.Add(searchButton);
            SearchTab.Controls.Add(searchBox);
            SearchTab.Controls.Add(comboBox1);
            SearchTab.Controls.Add(textBox1);
            SearchTab.Controls.Add(manualQuery);
            SearchTab.Controls.Add(menuStrip1);
            SearchTab.Location = new Point(4, 24);
            SearchTab.Margin = new Padding(4, 3, 4, 3);
            SearchTab.Name = "SearchTab";
            SearchTab.Padding = new Padding(4, 3, 4, 3);
            SearchTab.Size = new Size(1111, 464);
            SearchTab.TabIndex = 0;
            SearchTab.Text = "Search";
            SearchTab.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Location = new Point(602, 13);
            checkBox5.Margin = new Padding(4, 3, 4, 3);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(103, 19);
            checkBox5.TabIndex = 17;
            checkBox5.Text = "Purchase Price";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(538, 13);
            checkBox3.Margin = new Padding(4, 3, 4, 3);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(50, 19);
            checkBox3.TabIndex = 15;
            checkBox3.Text = "Date";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Location = new Point(257, 145);
            checkBox2.Margin = new Padding(4, 3, 4, 3);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(72, 19);
            checkBox2.TabIndex = 14;
            checkBox2.Text = "Sold Out";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(257, 118);
            checkBox1.Margin = new Padding(4, 3, 4, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(68, 19);
            checkBox1.TabIndex = 13;
            checkBox1.Text = "In Stock";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(155, 87);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(83, 15);
            label2.TabIndex = 12;
            label2.Text = "Bought Before";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(166, 57);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 11;
            label1.Text = "Bought After";
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(257, 87);
            dateTimePicker2.Margin = new Padding(4, 3, 4, 3);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(236, 23);
            dateTimePicker2.TabIndex = 10;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(257, 57);
            dateTimePicker1.Margin = new Padding(4, 3, 4, 3);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(236, 23);
            dateTimePicker1.TabIndex = 9;
            dateTimePicker1.Value = new DateTime(2020, 1, 1, 0, 1, 0, 0);
            // 
            // searchButton
            // 
            searchButton.Location = new Point(405, 8);
            searchButton.Margin = new Padding(4, 3, 4, 3);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(110, 27);
            searchButton.TabIndex = 8;
            searchButton.Text = "Search";
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += button2_Click;
            // 
            // searchBox
            // 
            searchBox.ForeColor = SystemColors.WindowText;
            searchBox.Location = new Point(7, 8);
            searchBox.Margin = new Padding(4, 3, 4, 3);
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(242, 23);
            searchBox.TabIndex = 7;
            searchBox.KeyDown += searchBox_KeyDown;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Item", "Fee" });
            comboBox1.Location = new Point(257, 8);
            comboBox1.Margin = new Padding(4, 3, 4, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(140, 23);
            comboBox1.TabIndex = 6;
            // 
            // menuStrip1
            // 
            menuStrip1.Location = new Point(4, 3);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(1103, 24);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // ItemTab
            // 
            ItemTab.Controls.Add(label43);
            ItemTab.Controls.Add(button5);
            ItemTab.Controls.Add(label40);
            ItemTab.Controls.Add(button4);
            ItemTab.Controls.Add(button1);
            ItemTab.Controls.Add(label26);
            ItemTab.Controls.Add(label25);
            ItemTab.Controls.Add(label24);
            ItemTab.Controls.Add(label23);
            ItemTab.Controls.Add(label22);
            ItemTab.Controls.Add(label21);
            ItemTab.Controls.Add(label20);
            ItemTab.Controls.Add(label19);
            ItemTab.Controls.Add(label18);
            ItemTab.Controls.Add(label17);
            ItemTab.Controls.Add(textBox3);
            ItemTab.Controls.Add(label14);
            ItemTab.Controls.Add(label13);
            ItemTab.Controls.Add(label12);
            ItemTab.Controls.Add(label11);
            ItemTab.Controls.Add(label10);
            ItemTab.Controls.Add(label9);
            ItemTab.Controls.Add(label8);
            ItemTab.Controls.Add(label7);
            ItemTab.Controls.Add(label6);
            ItemTab.Controls.Add(label5);
            ItemTab.Controls.Add(label4);
            ItemTab.Controls.Add(label3);
            ItemTab.Controls.Add(textBox10);
            ItemTab.Controls.Add(textBox9);
            ItemTab.Controls.Add(textBox8);
            ItemTab.Controls.Add(textBox7);
            ItemTab.Controls.Add(textBox6);
            ItemTab.Controls.Add(textBox5);
            ItemTab.Controls.Add(textBox4);
            ItemTab.Location = new Point(4, 24);
            ItemTab.Margin = new Padding(4, 3, 4, 3);
            ItemTab.Name = "ItemTab";
            ItemTab.Padding = new Padding(4, 3, 4, 3);
            ItemTab.Size = new Size(1111, 464);
            ItemTab.TabIndex = 1;
            ItemTab.Text = "Item";
            ItemTab.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            label43.AutoSize = true;
            label43.Location = new Point(120, 99);
            label43.Name = "label43";
            label43.Size = new Size(44, 15);
            label43.TabIndex = 56;
            label43.Text = "label43";
            // 
            // button5
            // 
            button5.Location = new Point(373, 317);
            button5.Name = "button5";
            button5.Size = new Size(126, 23);
            button5.TabIndex = 55;
            button5.Text = "Delete Shipping Info";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.Location = new Point(120, 52);
            label40.Name = "label40";
            label40.Size = new Size(44, 15);
            label40.TabIndex = 54;
            label40.Text = "label40";
            // 
            // button4
            // 
            button4.Location = new Point(120, 20);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 53;
            button4.Text = "Edit";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button1
            // 
            button1.Location = new Point(120, 315);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(117, 27);
            button1.TabIndex = 37;
            button1.Text = "Update";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(399, 282);
            label26.Margin = new Padding(4, 0, 4, 0);
            label26.Name = "label26";
            label26.Size = new Size(44, 15);
            label26.TabIndex = 36;
            label26.Text = "label26";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(399, 251);
            label25.Margin = new Padding(4, 0, 4, 0);
            label25.Name = "label25";
            label25.Size = new Size(44, 15);
            label25.TabIndex = 35;
            label25.Text = "label25";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new Point(399, 220);
            label24.Margin = new Padding(4, 0, 4, 0);
            label24.Name = "label24";
            label24.Size = new Size(44, 15);
            label24.TabIndex = 34;
            label24.Text = "label24";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(399, 188);
            label23.Margin = new Padding(4, 0, 4, 0);
            label23.Name = "label23";
            label23.Size = new Size(44, 15);
            label23.TabIndex = 33;
            label23.Text = "label23";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(399, 157);
            label22.Margin = new Padding(4, 0, 4, 0);
            label22.Name = "label22";
            label22.Size = new Size(44, 15);
            label22.TabIndex = 32;
            label22.Text = "label22";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(120, 281);
            label21.Margin = new Padding(4, 0, 4, 0);
            label21.Name = "label21";
            label21.Size = new Size(44, 15);
            label21.TabIndex = 31;
            label21.Text = "label21";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(120, 250);
            label20.Margin = new Padding(4, 0, 4, 0);
            label20.Name = "label20";
            label20.Size = new Size(44, 15);
            label20.TabIndex = 30;
            label20.Text = "label20";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(120, 219);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(44, 15);
            label19.TabIndex = 29;
            label19.Text = "label19";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(120, 188);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new Size(44, 15);
            label18.TabIndex = 28;
            label18.Text = "label18";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(120, 157);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(44, 15);
            label17.TabIndex = 27;
            label17.Text = "label17";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(120, 49);
            textBox3.Margin = new Padding(4, 3, 4, 3);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(309, 23);
            textBox3.TabIndex = 14;
            textBox3.Leave += TextBoxAttribute_Leave;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(343, 282);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(43, 15);
            label14.TabIndex = 12;
            label14.Text = "Height";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(347, 251);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(39, 15);
            label13.TabIndex = 11;
            label13.Text = "Width";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(321, 220);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(65, 15);
            label12.TabIndex = 10;
            label12.Text = "Length (in)";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(318, 188);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(68, 15);
            label11.TabIndex = 9;
            label11.Text = "Weight (oz)";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(315, 157);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(71, 15);
            label10.TabIndex = 8;
            label10.Text = "Weight (lbs)";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(57, 281);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(50, 15);
            label9.TabIndex = 7;
            label9.Text = "Item No";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(22, 219);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(85, 15);
            label8.TabIndex = 6;
            label8.Text = "Initial Quantity";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(11, 250);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(96, 15);
            label7.TabIndex = 5;
            label7.Text = "Current Quantity";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(48, 188);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(59, 15);
            label6.TabIndex = 4;
            label6.Text = "Sold Price";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(23, 157);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(84, 15);
            label5.TabIndex = 3;
            label5.Text = "Purchase Price";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(14, 99);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(89, 15);
            label4.TabIndex = 2;
            label4.Text = "Date Purchased";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(64, 52);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 0;
            label3.Text = "Name";
            // 
            // textBox10
            // 
            textBox10.Location = new Point(399, 279);
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(100, 23);
            textBox10.TabIndex = 52;
            textBox10.Leave += TextBoxAttribute_Leave;
            // 
            // textBox9
            // 
            textBox9.Location = new Point(399, 248);
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(100, 23);
            textBox9.TabIndex = 51;
            textBox9.Leave += TextBoxAttribute_Leave;
            // 
            // textBox8
            // 
            textBox8.Location = new Point(399, 217);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(100, 23);
            textBox8.TabIndex = 50;
            textBox8.Leave += TextBoxAttribute_Leave;
            // 
            // textBox7
            // 
            textBox7.Location = new Point(399, 185);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(100, 23);
            textBox7.TabIndex = 49;
            textBox7.Leave += TextBoxAttribute_Leave;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(399, 154);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(100, 23);
            textBox6.TabIndex = 48;
            textBox6.Leave += TextBoxAttribute_Leave;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(120, 247);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(100, 23);
            textBox5.TabIndex = 47;
            textBox5.Leave += TextBoxAttribute_Leave;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(120, 216);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(100, 23);
            textBox4.TabIndex = 46;
            textBox4.Leave += TextBoxAttribute_Leave;
            // 
            // PurchaseTab
            // 
            PurchaseTab.Controls.Add(label44);
            PurchaseTab.Controls.Add(textBox11);
            PurchaseTab.Controls.Add(label42);
            PurchaseTab.Controls.Add(button7);
            PurchaseTab.Controls.Add(label41);
            PurchaseTab.Controls.Add(label15);
            PurchaseTab.Controls.Add(button6);
            PurchaseTab.Controls.Add(button3);
            PurchaseTab.Controls.Add(textBox21);
            PurchaseTab.Controls.Add(label37);
            PurchaseTab.Controls.Add(label36);
            PurchaseTab.Controls.Add(dateTimePicker4);
            PurchaseTab.Controls.Add(textBox20);
            PurchaseTab.Controls.Add(textBox19);
            PurchaseTab.Controls.Add(textBox18);
            PurchaseTab.Controls.Add(textBox17);
            PurchaseTab.Controls.Add(textBox16);
            PurchaseTab.Controls.Add(textBox15);
            PurchaseTab.Controls.Add(textBox14);
            PurchaseTab.Controls.Add(textBox2);
            PurchaseTab.Controls.Add(label35);
            PurchaseTab.Controls.Add(label34);
            PurchaseTab.Controls.Add(label33);
            PurchaseTab.Controls.Add(label32);
            PurchaseTab.Controls.Add(label31);
            PurchaseTab.Controls.Add(label30);
            PurchaseTab.Controls.Add(label29);
            PurchaseTab.Controls.Add(label28);
            PurchaseTab.Controls.Add(label27);
            PurchaseTab.Controls.Add(button2);
            PurchaseTab.Controls.Add(label16);
            PurchaseTab.Controls.Add(listBox2);
            PurchaseTab.Location = new Point(4, 24);
            PurchaseTab.Margin = new Padding(4, 3, 4, 3);
            PurchaseTab.Name = "PurchaseTab";
            PurchaseTab.Padding = new Padding(4, 3, 4, 3);
            PurchaseTab.Size = new Size(1111, 464);
            PurchaseTab.TabIndex = 2;
            PurchaseTab.Text = "Purchase Group";
            PurchaseTab.UseVisualStyleBackColor = true;
            // 
            // label44
            // 
            label44.AutoSize = true;
            label44.Location = new Point(603, 105);
            label44.Name = "label44";
            label44.Size = new Size(44, 15);
            label44.TabIndex = 31;
            label44.Text = "label44";
            // 
            // textBox11
            // 
            textBox11.Location = new Point(349, 412);
            textBox11.Margin = new Padding(4, 3, 4, 3);
            textBox11.Name = "textBox11";
            textBox11.Size = new Size(96, 23);
            textBox11.TabIndex = 30;
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Location = new Point(349, 394);
            label42.Name = "label42";
            label42.Size = new Size(96, 15);
            label42.TabIndex = 29;
            label42.Text = "Current Quantity";
            // 
            // button7
            // 
            button7.Location = new Point(603, 198);
            button7.Name = "button7";
            button7.Size = new Size(75, 23);
            button7.TabIndex = 28;
            button7.Text = "Update";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.Location = new Point(603, 158);
            label41.Name = "label41";
            label41.Size = new Size(44, 15);
            label41.TabIndex = 27;
            label41.Text = "label41";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(603, 51);
            label15.Name = "label15";
            label15.Size = new Size(44, 15);
            label15.TabIndex = 26;
            label15.Text = "label15";
            // 
            // button6
            // 
            button6.Location = new Point(633, 7);
            button6.Name = "button6";
            button6.Size = new Size(75, 27);
            button6.TabIndex = 25;
            button6.Text = "Edit";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button3
            // 
            button3.Location = new Point(501, 7);
            button3.Margin = new Padding(4, 3, 4, 3);
            button3.Name = "button3";
            button3.Size = new Size(122, 27);
            button3.TabIndex = 24;
            button3.Text = "New Purchase";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // textBox21
            // 
            textBox21.Location = new Point(603, 155);
            textBox21.Margin = new Padding(4, 3, 4, 3);
            textBox21.Name = "textBox21";
            textBox21.Size = new Size(207, 23);
            textBox21.TabIndex = 23;
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new Point(501, 158);
            label37.Margin = new Padding(4, 0, 4, 0);
            label37.Name = "label37";
            label37.Size = new Size(89, 15);
            label37.TabIndex = 22;
            label37.Text = "Purchase Notes";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new Point(501, 105);
            label36.Margin = new Padding(4, 0, 4, 0);
            label36.Name = "label36";
            label36.Size = new Size(82, 15);
            label36.TabIndex = 21;
            label36.Text = "Purchase Date";
            // 
            // dateTimePicker4
            // 
            dateTimePicker4.Location = new Point(603, 99);
            dateTimePicker4.Margin = new Padding(4, 3, 4, 3);
            dateTimePicker4.Name = "dateTimePicker4";
            dateTimePicker4.Size = new Size(233, 23);
            dateTimePicker4.TabIndex = 20;
            // 
            // textBox20
            // 
            textBox20.Location = new Point(603, 48);
            textBox20.Margin = new Padding(4, 3, 4, 3);
            textBox20.Name = "textBox20";
            textBox20.Size = new Size(116, 23);
            textBox20.TabIndex = 19;
            // 
            // textBox19
            // 
            textBox19.Location = new Point(801, 413);
            textBox19.Margin = new Padding(4, 3, 4, 3);
            textBox19.Name = "textBox19";
            textBox19.Size = new Size(40, 23);
            textBox19.TabIndex = 18;
            // 
            // textBox18
            // 
            textBox18.Location = new Point(712, 413);
            textBox18.Margin = new Padding(4, 3, 4, 3);
            textBox18.Name = "textBox18";
            textBox18.Size = new Size(40, 23);
            textBox18.TabIndex = 17;
            // 
            // textBox17
            // 
            textBox17.Location = new Point(633, 413);
            textBox17.Margin = new Padding(4, 3, 4, 3);
            textBox17.Name = "textBox17";
            textBox17.Size = new Size(40, 23);
            textBox17.TabIndex = 16;
            // 
            // textBox16
            // 
            textBox16.Location = new Point(557, 413);
            textBox16.Margin = new Padding(4, 3, 4, 3);
            textBox16.Name = "textBox16";
            textBox16.Size = new Size(40, 23);
            textBox16.TabIndex = 15;
            // 
            // textBox15
            // 
            textBox15.Location = new Point(475, 413);
            textBox15.Margin = new Padding(4, 3, 4, 3);
            textBox15.Name = "textBox15";
            textBox15.Size = new Size(40, 23);
            textBox15.TabIndex = 14;
            // 
            // textBox14
            // 
            textBox14.Location = new Point(260, 413);
            textBox14.Margin = new Padding(4, 3, 4, 3);
            textBox14.Name = "textBox14";
            textBox14.Size = new Size(81, 23);
            textBox14.TabIndex = 13;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(111, 413);
            textBox2.Margin = new Padding(4, 3, 4, 3);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(142, 23);
            textBox2.TabIndex = 12;
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new Point(797, 395);
            label35.Margin = new Padding(4, 0, 4, 0);
            label35.Name = "label35";
            label35.Size = new Size(68, 15);
            label35.TabIndex = 11;
            label35.Text = "Weight (oz)";
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new Point(472, 375);
            label34.Margin = new Padding(4, 0, 4, 0);
            label34.Name = "label34";
            label34.Size = new Size(138, 15);
            label34.TabIndex = 10;
            label34.Text = "Shipping Info: (Optional)";
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Location = new Point(709, 395);
            label33.Margin = new Padding(4, 0, 4, 0);
            label33.Name = "label33";
            label33.Size = new Size(71, 15);
            label33.TabIndex = 9;
            label33.Text = "Weight (lbs)";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new Point(629, 395);
            label32.Margin = new Padding(4, 0, 4, 0);
            label32.Name = "label32";
            label32.Size = new Size(64, 15);
            label32.TabIndex = 8;
            label32.Text = "Height (in)";
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new Point(553, 395);
            label31.Margin = new Padding(4, 0, 4, 0);
            label31.Name = "label31";
            label31.Size = new Size(60, 15);
            label31.TabIndex = 7;
            label31.Text = "Width (in)";
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Location = new Point(472, 395);
            label30.Margin = new Padding(4, 0, 4, 0);
            label30.Name = "label30";
            label30.Size = new Size(65, 15);
            label30.TabIndex = 6;
            label30.Text = "Length (in)";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(257, 395);
            label29.Margin = new Padding(4, 0, 4, 0);
            label29.Name = "label29";
            label29.Size = new Size(85, 15);
            label29.TabIndex = 5;
            label29.Text = "Initial Quantity";
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new Point(107, 395);
            label28.Margin = new Padding(4, 0, 4, 0);
            label28.Name = "label28";
            label28.Size = new Size(66, 15);
            label28.TabIndex = 4;
            label28.Text = "Item Name";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new Point(7, 375);
            label27.Margin = new Padding(4, 0, 4, 0);
            label27.Name = "label27";
            label27.Size = new Size(86, 15);
            label27.TabIndex = 3;
            label27.Text = "Add New Item:";
            // 
            // button2
            // 
            button2.Location = new Point(7, 409);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 2;
            button2.Text = "Add Item";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(501, 51);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new Size(84, 15);
            label16.TabIndex = 1;
            label16.Text = "Purchase Price";
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(7, 7);
            listBox2.Margin = new Padding(4, 3, 4, 3);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(468, 319);
            listBox2.TabIndex = 0;
            listBox2.MouseDoubleClick += listBox2_MouseDoubleClick;
            // 
            // Sale
            // 
            Sale.Controls.Add(button11);
            Sale.Controls.Add(button10);
            Sale.Controls.Add(label54);
            Sale.Controls.Add(button9);
            Sale.Controls.Add(label53);
            Sale.Controls.Add(label52);
            Sale.Controls.Add(dateTimePicker5);
            Sale.Controls.Add(label51);
            Sale.Controls.Add(label50);
            Sale.Controls.Add(dateTimePicker3);
            Sale.Controls.Add(label48);
            Sale.Controls.Add(textBox13);
            Sale.Controls.Add(label46);
            Sale.Controls.Add(button8);
            Sale.Controls.Add(textBox22);
            Sale.Controls.Add(label38);
            Sale.Controls.Add(listBox3);
            Sale.Location = new Point(4, 24);
            Sale.Margin = new Padding(4, 3, 4, 3);
            Sale.Name = "Sale";
            Sale.Size = new Size(1111, 464);
            Sale.TabIndex = 4;
            Sale.Text = "Sale";
            Sale.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            button10.Location = new Point(625, 136);
            button10.Name = "button10";
            button10.Size = new Size(75, 23);
            button10.TabIndex = 30;
            button10.Text = "Update";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // label54
            // 
            label54.AutoSize = true;
            label54.Location = new Point(625, 113);
            label54.Name = "label54";
            label54.Size = new Size(44, 15);
            label54.TabIndex = 29;
            label54.Text = "label54";
            // 
            // button9
            // 
            button9.Location = new Point(625, 3);
            button9.Name = "button9";
            button9.Size = new Size(75, 23);
            button9.TabIndex = 28;
            button9.Text = "Edit";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // label53
            // 
            label53.AutoSize = true;
            label53.Location = new Point(547, 44);
            label53.Name = "label53";
            label53.Size = new Size(66, 15);
            label53.TabIndex = 27;
            label53.Text = "Item Name";
            // 
            // label52
            // 
            label52.AutoSize = true;
            label52.Location = new Point(558, 113);
            label52.Name = "label52";
            label52.Size = new Size(55, 15);
            label52.TabIndex = 26;
            label52.Text = "Sale Date";
            // 
            // dateTimePicker5
            // 
            dateTimePicker5.Location = new Point(625, 107);
            dateTimePicker5.Name = "dateTimePicker5";
            dateTimePicker5.Size = new Size(200, 23);
            dateTimePicker5.TabIndex = 25;
            // 
            // label51
            // 
            label51.AutoSize = true;
            label51.Location = new Point(625, 44);
            label51.Name = "label51";
            label51.Size = new Size(44, 15);
            label51.TabIndex = 24;
            label51.Text = "label51";
            // 
            // label50
            // 
            label50.AutoSize = true;
            label50.Location = new Point(201, 356);
            label50.Name = "label50";
            label50.Size = new Size(55, 15);
            label50.TabIndex = 23;
            label50.Text = "Sale Date";
            // 
            // dateTimePicker3
            // 
            dateTimePicker3.Location = new Point(201, 374);
            dateTimePicker3.Name = "dateTimePicker3";
            dateTimePicker3.Size = new Size(200, 23);
            dateTimePicker3.TabIndex = 22;
            // 
            // label48
            // 
            label48.AutoSize = true;
            label48.Location = new Point(625, 79);
            label48.Name = "label48";
            label48.Size = new Size(44, 15);
            label48.TabIndex = 20;
            label48.Text = "label48";
            // 
            // textBox13
            // 
            textBox13.Location = new Point(108, 374);
            textBox13.Margin = new Padding(4, 3, 4, 3);
            textBox13.Name = "textBox13";
            textBox13.Size = new Size(80, 23);
            textBox13.TabIndex = 18;
            // 
            // label46
            // 
            label46.AutoSize = true;
            label46.Location = new Point(104, 356);
            label46.Margin = new Padding(4, 0, 4, 0);
            label46.Name = "label46";
            label46.Size = new Size(75, 15);
            label46.TabIndex = 16;
            label46.Text = "Sale Amount";
            // 
            // button8
            // 
            button8.Location = new Point(4, 370);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 14;
            button8.Text = "Add Sale";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // textBox22
            // 
            textBox22.Location = new Point(625, 76);
            textBox22.Margin = new Padding(4, 3, 4, 3);
            textBox22.Name = "textBox22";
            textBox22.Size = new Size(116, 23);
            textBox22.TabIndex = 4;
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Location = new Point(538, 79);
            label38.Margin = new Padding(4, 0, 4, 0);
            label38.Name = "label38";
            label38.Size = new Size(75, 15);
            label38.TabIndex = 1;
            label38.Text = "Sale Amount";
            // 
            // listBox3
            // 
            listBox3.FormattingEnabled = true;
            listBox3.ItemHeight = 15;
            listBox3.Location = new Point(4, 3);
            listBox3.Margin = new Padding(4, 3, 4, 3);
            listBox3.Name = "listBox3";
            listBox3.Size = new Size(527, 304);
            listBox3.TabIndex = 0;
            listBox3.MouseDoubleClick += listBox3_MouseDoubleClick;
            // 
            // resultItemBindingSource
            // 
            resultItemBindingSource.DataSource = typeof(ResultItem);
            // 
            // button11
            // 
            button11.Location = new Point(706, 136);
            button11.Name = "button11";
            button11.Size = new Size(75, 23);
            button11.TabIndex = 31;
            button11.Text = "Delete";
            button11.UseVisualStyleBackColor = true;
            button11.Click += button11_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1146, 519);
            Controls.Add(tabControl1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            SearchTab.ResumeLayout(false);
            SearchTab.PerformLayout();
            ItemTab.ResumeLayout(false);
            ItemTab.PerformLayout();
            PurchaseTab.ResumeLayout(false);
            PurchaseTab.PerformLayout();
            Sale.ResumeLayout(false);
            Sale.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)resultItemBindingSource).EndInit();
            ResumeLayout(false);
        }




        #endregion
        public System.Windows.Forms.Button manualQuery;
        public System.Windows.Forms.ListBox listBox1;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.TabPage SearchTab;
        public System.Windows.Forms.TabPage ItemTab;
        public System.Windows.Forms.MenuStrip menuStrip1;
        public System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.TextBox searchBox;
        public System.Windows.Forms.Button searchButton;
        public System.Windows.Forms.DateTimePicker dateTimePicker2;
        public System.Windows.Forms.DateTimePicker dateTimePicker1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.CheckBox checkBox2;
        public System.Windows.Forms.CheckBox checkBox5;
        public System.Windows.Forms.CheckBox checkBox3;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label14;
        public System.Windows.Forms.Label label13;
        public System.Windows.Forms.Label label12;
        public System.Windows.Forms.TextBox textBox3;
        public System.Windows.Forms.Label label25;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.Label label23;
        public System.Windows.Forms.Label label22;
        public System.Windows.Forms.Label label21;
        public System.Windows.Forms.Label label20;
        public System.Windows.Forms.Label label19;
        public System.Windows.Forms.Label label18;
        public System.Windows.Forms.Label label17;
        public System.Windows.Forms.Label label26;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.TabPage PurchaseTab;
        public System.Windows.Forms.ListBox listBox2;
        public System.Windows.Forms.Label label34;
        public System.Windows.Forms.Label label33;
        public System.Windows.Forms.Label label32;
        public System.Windows.Forms.Label label31;
        public System.Windows.Forms.Label label30;
        public System.Windows.Forms.Label label29;
        public System.Windows.Forms.Label label28;
        public System.Windows.Forms.Label label27;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Label label16;
        public System.Windows.Forms.TextBox textBox19;
        public System.Windows.Forms.TextBox textBox18;
        public System.Windows.Forms.TextBox textBox17;
        public System.Windows.Forms.TextBox textBox16;
        public System.Windows.Forms.TextBox textBox15;
        public System.Windows.Forms.TextBox textBox14;
        public System.Windows.Forms.TextBox textBox2;
        public System.Windows.Forms.Label label35;
        public System.Windows.Forms.TextBox textBox21;
        public System.Windows.Forms.Label label37;
        public System.Windows.Forms.Label label36;
        public System.Windows.Forms.DateTimePicker dateTimePicker4;
        public System.Windows.Forms.TextBox textBox20;
        public System.Windows.Forms.TabPage Sale;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.ListBox listBox3;
        public System.Windows.Forms.BindingSource resultItemBindingSource;
        public System.Windows.Forms.Label label38;
        public System.Windows.Forms.TextBox textBox22;
        public TextBox textBox10;
        public TextBox textBox9;
        public TextBox textBox7;
        public TextBox textBox6;
        public TextBox textBox5;
        public TextBox textBox4;
        public TextBox textBox8;
        public Button button4;
        public Label label40;
        public Button button5;
        public Button button6;
        public Label label41;
        public Label label15;
        public Button button7;
        public TextBox textBox11;
        public Label label42;
        public Label label43;
        public Label label44;
        public Label label51;
        public Label label50;
        public DateTimePicker dateTimePicker3;
        public Label label48;
        public TextBox textBox13;
        public Label label46;
        public Label label47;
        public Button button8;
        public DateTimePicker dateTimePicker5;
        public Label label53;
        public Label label52;
        public Button button9;
        public Label label54;
        public Label label49;
        public Button button10;
        public Button button11;
    }
}

