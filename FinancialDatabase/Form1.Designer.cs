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
            tabCollection = new TabControl();
            SearchTab = new TabPage();
            itemSearchView = new ItemSearchViewer();
            showPurcPriceCheckBox = new CheckBox();
            showDateCheckBox = new CheckBox();
            soldOutCheckBox = new CheckBox();
            inStockCheckBox = new CheckBox();
            boughtBeforeLbl = new Label();
            boughtAfterLbl = new Label();
            boughtBeforeDatePicker = new DateTimePicker();
            boughtAfterDatePicker = new DateTimePicker();
            searchButton = new Button();
            searchBox = new TextBox();
            comboBox1 = new ComboBox();
            comboBoxMenuStrip = new MenuStrip();
            ItemTab = new TabPage();
            setThumbnailButton = new Button();
            deleteImageButton = new Button();
            AddImageButton = new Button();
            mainPictureViewer = new MainPictureViewer();
            allPictureViewer = new AllPictureViewer();
            deleteItemButton = new Button();
            itemDatePurcLbl = new Label();
            deleteShipInfoButton = new Button();
            itemNameLbl = new Label();
            itemEditButton = new Button();
            itemUpdateButton = new Button();
            itemHeightLbl = new Label();
            itemWidthLbl = new Label();
            itemLengthLbl = new Label();
            itemWeightOzLbl = new Label();
            itemWeightLbsLbl = new Label();
            itemItemNoLbl = new Label();
            itemCurrQtyLbl = new Label();
            itemInitQtyLbl = new Label();
            itemSoldPriceLbl = new Label();
            itemPurcPriceLbl = new Label();
            itemNameTxtbox = new TextBox();
            itemTagHeightLbl = new Label();
            itemTagWidthLbl = new Label();
            itemTagLengthLbl = new Label();
            itemTagWeightOzLbl = new Label();
            itemTagWeightLbsLbl = new Label();
            itemTagItemNoLbl = new Label();
            itemTagInitQtyLbl = new Label();
            itemTagCurrQtyLbl = new Label();
            itemTagSoldPriceLbl = new Label();
            itemTagPurcPriceLbl = new Label();
            itemTagDatePurcLbl = new Label();
            itemTagNameLbl = new Label();
            itemHeightTxtbox = new TextBox();
            itemWidthTxtbox = new TextBox();
            itemLengthTxtbox = new TextBox();
            itemWeightOzTxtbox = new TextBox();
            itemWeightLbsTxtbox = new TextBox();
            itemCurrQtyTxtbox = new TextBox();
            itemInitQtyTxtbox = new TextBox();
            PurchaseTab = new TabPage();
            PurcPurcDateLbl = new Label();
            PurcCurrQtyTextbox = new TextBox();
            PurcTagCurrQtyLbl = new Label();
            UpdatePurcButton = new Button();
            PurcPurcNotesLbl = new Label();
            PurcPurcPriceLbl = new Label();
            EditPurcButton = new Button();
            NewPurcButton = new Button();
            PurcPurcNotesTextbox = new TextBox();
            PurcTagPurcNotesLbl = new Label();
            PurcTagPurcDateLbl = new Label();
            PurcDatePicker = new DateTimePicker();
            PurcPurcPriceTextbox = new TextBox();
            PurcWeightOzTextbox = new TextBox();
            PurcWeightLbsTextbox = new TextBox();
            PurcHeightTextbox = new TextBox();
            PurcWidthTextbox = new TextBox();
            PurcLengthTextbox = new TextBox();
            PurcInitQtyTextbox = new TextBox();
            PurcNameTextbox = new TextBox();
            PurcTagWeightOzLbl = new Label();
            ShippingInfoLbl = new Label();
            PurcTagWeightLbs = new Label();
            PurcTagHeightLbl = new Label();
            PurcTagWidthLbl = new Label();
            PurcTagLengthLbl = new Label();
            PurcTagInitQtyLbl = new Label();
            PurcTagNameLbl = new Label();
            AddNewItemLbl = new Label();
            addItemButton = new Button();
            PurcTagPurcPriceLbl = new Label();
            PurchaseListBox = new ListBox();
            Sale = new TabPage();
            SaleDeleteSaleButton = new Button();
            SaleUpdateButton = new Button();
            SaleDateLbl = new Label();
            SaleEditSaleButton = new Button();
            SaleTagNameLbl = new Label();
            SaleTagSaleDateLbl = new Label();
            SaleDatePicker = new DateTimePicker();
            SaleNameLbl = new Label();
            SaleTagNewSaleDateLbl = new Label();
            SaleNewSaleDatePicker = new DateTimePicker();
            SaleAmountLbl = new Label();
            SaleNewSaleAmountTextbox = new TextBox();
            SaleTagNewSaleAmountLbl = new Label();
            AddSaleButton = new Button();
            SaleAmountTextbox = new TextBox();
            SaleTagSaleAmountLbl = new Label();
            saleListBox = new ListBox();
            resultItemBindingSource = new BindingSource(components);
            openFileDialog1 = new OpenFileDialog();
            tabCollection.SuspendLayout();
            SearchTab.SuspendLayout();
            ItemTab.SuspendLayout();
            PurchaseTab.SuspendLayout();
            Sale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resultItemBindingSource).BeginInit();
            SuspendLayout();
            // 
            // tabCollection
            // 
            tabCollection.Controls.Add(SearchTab);
            tabCollection.Controls.Add(ItemTab);
            tabCollection.Controls.Add(PurchaseTab);
            tabCollection.Controls.Add(Sale);
            tabCollection.Location = new Point(14, 14);
            tabCollection.Margin = new Padding(4, 3, 4, 3);
            tabCollection.Multiline = true;
            tabCollection.Name = "tabCollection";
            tabCollection.SelectedIndex = 0;
            tabCollection.Size = new Size(1119, 492);
            tabCollection.TabIndex = 5;
            // 
            // SearchTab
            // 
            SearchTab.Controls.Add(itemSearchView);
            SearchTab.Controls.Add(showPurcPriceCheckBox);
            SearchTab.Controls.Add(showDateCheckBox);
            SearchTab.Controls.Add(soldOutCheckBox);
            SearchTab.Controls.Add(inStockCheckBox);
            SearchTab.Controls.Add(boughtBeforeLbl);
            SearchTab.Controls.Add(boughtAfterLbl);
            SearchTab.Controls.Add(boughtBeforeDatePicker);
            SearchTab.Controls.Add(boughtAfterDatePicker);
            SearchTab.Controls.Add(searchButton);
            SearchTab.Controls.Add(searchBox);
            SearchTab.Controls.Add(comboBox1);
            SearchTab.Controls.Add(comboBoxMenuStrip);
            SearchTab.Location = new Point(4, 24);
            SearchTab.Margin = new Padding(4, 3, 4, 3);
            SearchTab.Name = "SearchTab";
            SearchTab.Padding = new Padding(4, 3, 4, 3);
            SearchTab.Size = new Size(1111, 464);
            SearchTab.TabIndex = 0;
            SearchTab.Text = "Search";
            SearchTab.UseVisualStyleBackColor = true;
            // 
            // itemSearchView
            // 
            itemSearchView.AutoScroll = true;
            itemSearchView.AutoScrollMinSize = new Size(191, 180);
            itemSearchView.Location = new Point(538, 49);
            itemSearchView.Name = "itemSearchView";
            itemSearchView.Size = new Size(566, 406);
            itemSearchView.TabIndex = 18;
            itemSearchView.Text = "customControl11";
            itemSearchView.Click += itemSearchView_Click;
            // 
            // showPurcPriceCheckBox
            // 
            showPurcPriceCheckBox.AutoSize = true;
            showPurcPriceCheckBox.Location = new Point(602, 13);
            showPurcPriceCheckBox.Margin = new Padding(4, 3, 4, 3);
            showPurcPriceCheckBox.Name = "showPurcPriceCheckBox";
            showPurcPriceCheckBox.Size = new Size(103, 19);
            showPurcPriceCheckBox.TabIndex = 17;
            showPurcPriceCheckBox.Text = "Purchase Price";
            showPurcPriceCheckBox.UseVisualStyleBackColor = true;
            // 
            // showDateCheckBox
            // 
            showDateCheckBox.AutoSize = true;
            showDateCheckBox.Location = new Point(538, 13);
            showDateCheckBox.Margin = new Padding(4, 3, 4, 3);
            showDateCheckBox.Name = "showDateCheckBox";
            showDateCheckBox.Size = new Size(50, 19);
            showDateCheckBox.TabIndex = 15;
            showDateCheckBox.Text = "Date";
            showDateCheckBox.UseVisualStyleBackColor = true;
            // 
            // soldOutCheckBox
            // 
            soldOutCheckBox.AutoSize = true;
            soldOutCheckBox.Checked = true;
            soldOutCheckBox.CheckState = CheckState.Checked;
            soldOutCheckBox.Location = new Point(257, 145);
            soldOutCheckBox.Margin = new Padding(4, 3, 4, 3);
            soldOutCheckBox.Name = "soldOutCheckBox";
            soldOutCheckBox.Size = new Size(72, 19);
            soldOutCheckBox.TabIndex = 14;
            soldOutCheckBox.Text = "Sold Out";
            soldOutCheckBox.UseVisualStyleBackColor = true;
            // 
            // inStockCheckBox
            // 
            inStockCheckBox.AutoSize = true;
            inStockCheckBox.Checked = true;
            inStockCheckBox.CheckState = CheckState.Checked;
            inStockCheckBox.Location = new Point(257, 118);
            inStockCheckBox.Margin = new Padding(4, 3, 4, 3);
            inStockCheckBox.Name = "inStockCheckBox";
            inStockCheckBox.Size = new Size(68, 19);
            inStockCheckBox.TabIndex = 13;
            inStockCheckBox.Text = "In Stock";
            inStockCheckBox.UseVisualStyleBackColor = true;
            // 
            // boughtBeforeLbl
            // 
            boughtBeforeLbl.AutoSize = true;
            boughtBeforeLbl.Location = new Point(155, 87);
            boughtBeforeLbl.Margin = new Padding(4, 0, 4, 0);
            boughtBeforeLbl.Name = "boughtBeforeLbl";
            boughtBeforeLbl.Size = new Size(83, 15);
            boughtBeforeLbl.TabIndex = 12;
            boughtBeforeLbl.Text = "Bought Before";
            // 
            // boughtAfterLbl
            // 
            boughtAfterLbl.AutoSize = true;
            boughtAfterLbl.Location = new Point(166, 57);
            boughtAfterLbl.Margin = new Padding(4, 0, 4, 0);
            boughtAfterLbl.Name = "boughtAfterLbl";
            boughtAfterLbl.Size = new Size(75, 15);
            boughtAfterLbl.TabIndex = 11;
            boughtAfterLbl.Text = "Bought After";
            // 
            // boughtBeforeDatePicker
            // 
            boughtBeforeDatePicker.Location = new Point(257, 87);
            boughtBeforeDatePicker.Margin = new Padding(4, 3, 4, 3);
            boughtBeforeDatePicker.Name = "boughtBeforeDatePicker";
            boughtBeforeDatePicker.Size = new Size(236, 23);
            boughtBeforeDatePicker.TabIndex = 10;
            // 
            // boughtAfterDatePicker
            // 
            boughtAfterDatePicker.Location = new Point(257, 57);
            boughtAfterDatePicker.Margin = new Padding(4, 3, 4, 3);
            boughtAfterDatePicker.Name = "boughtAfterDatePicker";
            boughtAfterDatePicker.Size = new Size(236, 23);
            boughtAfterDatePicker.TabIndex = 9;
            boughtAfterDatePicker.Value = new DateTime(2020, 1, 1, 0, 1, 0, 0);
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
            searchButton.Click += searchButton_Click;
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
            // comboBoxMenuStrip
            // 
            comboBoxMenuStrip.Location = new Point(4, 3);
            comboBoxMenuStrip.Name = "comboBoxMenuStrip";
            comboBoxMenuStrip.Padding = new Padding(7, 2, 0, 2);
            comboBoxMenuStrip.Size = new Size(1103, 24);
            comboBoxMenuStrip.TabIndex = 5;
            comboBoxMenuStrip.Text = "comboBoxMenuStrip";
            // 
            // ItemTab
            // 
            ItemTab.Controls.Add(setThumbnailButton);
            ItemTab.Controls.Add(deleteImageButton);
            ItemTab.Controls.Add(AddImageButton);
            ItemTab.Controls.Add(mainPictureViewer);
            ItemTab.Controls.Add(allPictureViewer);
            ItemTab.Controls.Add(deleteItemButton);
            ItemTab.Controls.Add(itemDatePurcLbl);
            ItemTab.Controls.Add(deleteShipInfoButton);
            ItemTab.Controls.Add(itemNameLbl);
            ItemTab.Controls.Add(itemEditButton);
            ItemTab.Controls.Add(itemUpdateButton);
            ItemTab.Controls.Add(itemHeightLbl);
            ItemTab.Controls.Add(itemWidthLbl);
            ItemTab.Controls.Add(itemLengthLbl);
            ItemTab.Controls.Add(itemWeightOzLbl);
            ItemTab.Controls.Add(itemWeightLbsLbl);
            ItemTab.Controls.Add(itemItemNoLbl);
            ItemTab.Controls.Add(itemCurrQtyLbl);
            ItemTab.Controls.Add(itemInitQtyLbl);
            ItemTab.Controls.Add(itemSoldPriceLbl);
            ItemTab.Controls.Add(itemPurcPriceLbl);
            ItemTab.Controls.Add(itemNameTxtbox);
            ItemTab.Controls.Add(itemTagHeightLbl);
            ItemTab.Controls.Add(itemTagWidthLbl);
            ItemTab.Controls.Add(itemTagLengthLbl);
            ItemTab.Controls.Add(itemTagWeightOzLbl);
            ItemTab.Controls.Add(itemTagWeightLbsLbl);
            ItemTab.Controls.Add(itemTagItemNoLbl);
            ItemTab.Controls.Add(itemTagInitQtyLbl);
            ItemTab.Controls.Add(itemTagCurrQtyLbl);
            ItemTab.Controls.Add(itemTagSoldPriceLbl);
            ItemTab.Controls.Add(itemTagPurcPriceLbl);
            ItemTab.Controls.Add(itemTagDatePurcLbl);
            ItemTab.Controls.Add(itemTagNameLbl);
            ItemTab.Controls.Add(itemHeightTxtbox);
            ItemTab.Controls.Add(itemWidthTxtbox);
            ItemTab.Controls.Add(itemLengthTxtbox);
            ItemTab.Controls.Add(itemWeightOzTxtbox);
            ItemTab.Controls.Add(itemWeightLbsTxtbox);
            ItemTab.Controls.Add(itemCurrQtyTxtbox);
            ItemTab.Controls.Add(itemInitQtyTxtbox);
            ItemTab.Location = new Point(4, 24);
            ItemTab.Margin = new Padding(4, 3, 4, 3);
            ItemTab.Name = "ItemTab";
            ItemTab.Padding = new Padding(4, 3, 4, 3);
            ItemTab.Size = new Size(1111, 464);
            ItemTab.TabIndex = 1;
            ItemTab.Text = "Item";
            ItemTab.UseVisualStyleBackColor = true;
            // 
            // setThumbnailButton
            // 
            setThumbnailButton.Location = new Point(350, 405);
            setThumbnailButton.Name = "setThumbnailButton";
            setThumbnailButton.Size = new Size(96, 23);
            setThumbnailButton.TabIndex = 62;
            setThumbnailButton.Text = "Set Thumbnail";
            setThumbnailButton.UseVisualStyleBackColor = true;
            setThumbnailButton.Click += setThumbnailButton_Click;
            // 
            // deleteImageButton
            // 
            deleteImageButton.Location = new Point(156, 405);
            deleteImageButton.Name = "deleteImageButton";
            deleteImageButton.Size = new Size(89, 23);
            deleteImageButton.TabIndex = 61;
            deleteImageButton.Text = "Delete Image";
            deleteImageButton.UseVisualStyleBackColor = true;
            // 
            // AddImageButton
            // 
            AddImageButton.Location = new Point(251, 405);
            AddImageButton.Name = "AddImageButton";
            AddImageButton.Size = new Size(93, 23);
            AddImageButton.TabIndex = 60;
            AddImageButton.Text = "Add Image(s)";
            AddImageButton.UseVisualStyleBackColor = true;
            AddImageButton.Click += addImageButton_Click;
            // 
            // mainPictureViewer
            // 
            mainPictureViewer.Location = new Point(505, 44);
            mainPictureViewer.Name = "mainPictureViewer";
            mainPictureViewer.Size = new Size(451, 393);
            mainPictureViewer.TabIndex = 59;
            mainPictureViewer.Text = "customControl31";
            // 
            // allPictureViewer
            // 
            allPictureViewer.Location = new Point(962, 44);
            allPictureViewer.Name = "allPictureViewer";
            allPictureViewer.Size = new Size(126, 393);
            allPictureViewer.TabIndex = 58;
            allPictureViewer.Text = "customControl21";
            allPictureViewer.Click += allPictureViewer_Click;
            // 
            // deleteItemButton
            // 
            deleteItemButton.Location = new Point(382, 18);
            deleteItemButton.Name = "deleteItemButton";
            deleteItemButton.Size = new Size(117, 27);
            deleteItemButton.TabIndex = 57;
            deleteItemButton.Text = "Delete Item";
            deleteItemButton.UseVisualStyleBackColor = true;
            deleteItemButton.Click += deleteItemButton_Click;
            // 
            // itemDatePurcLbl
            // 
            itemDatePurcLbl.AutoSize = true;
            itemDatePurcLbl.Location = new Point(120, 99);
            itemDatePurcLbl.Name = "itemDatePurcLbl";
            itemDatePurcLbl.Size = new Size(44, 15);
            itemDatePurcLbl.TabIndex = 56;
            itemDatePurcLbl.Text = "label43";
            // 
            // deleteShipInfoButton
            // 
            deleteShipInfoButton.Location = new Point(373, 317);
            deleteShipInfoButton.Name = "deleteShipInfoButton";
            deleteShipInfoButton.Size = new Size(126, 23);
            deleteShipInfoButton.TabIndex = 55;
            deleteShipInfoButton.Text = "Delete Shipping Info";
            deleteShipInfoButton.UseVisualStyleBackColor = true;
            deleteShipInfoButton.Click += deleteShipInfoButton_Click;
            // 
            // itemNameLbl
            // 
            itemNameLbl.AutoSize = true;
            itemNameLbl.Location = new Point(120, 52);
            itemNameLbl.Name = "itemNameLbl";
            itemNameLbl.Size = new Size(44, 15);
            itemNameLbl.TabIndex = 54;
            itemNameLbl.Text = "label40";
            // 
            // itemEditButton
            // 
            itemEditButton.Location = new Point(120, 20);
            itemEditButton.Name = "itemEditButton";
            itemEditButton.Size = new Size(75, 23);
            itemEditButton.TabIndex = 53;
            itemEditButton.Text = "Edit";
            itemEditButton.UseVisualStyleBackColor = true;
            itemEditButton.Click += itemEditButton_Click;
            // 
            // itemUpdateButton
            // 
            itemUpdateButton.Location = new Point(120, 315);
            itemUpdateButton.Margin = new Padding(4, 3, 4, 3);
            itemUpdateButton.Name = "itemUpdateButton";
            itemUpdateButton.Size = new Size(117, 27);
            itemUpdateButton.TabIndex = 37;
            itemUpdateButton.Text = "Update";
            itemUpdateButton.UseVisualStyleBackColor = true;
            itemUpdateButton.Click += itemUpdateButton_Click;
            // 
            // itemHeightLbl
            // 
            itemHeightLbl.AutoSize = true;
            itemHeightLbl.Location = new Point(399, 282);
            itemHeightLbl.Margin = new Padding(4, 0, 4, 0);
            itemHeightLbl.Name = "itemHeightLbl";
            itemHeightLbl.Size = new Size(44, 15);
            itemHeightLbl.TabIndex = 36;
            itemHeightLbl.Text = "label26";
            // 
            // itemWidthLbl
            // 
            itemWidthLbl.AutoSize = true;
            itemWidthLbl.Location = new Point(399, 251);
            itemWidthLbl.Margin = new Padding(4, 0, 4, 0);
            itemWidthLbl.Name = "itemWidthLbl";
            itemWidthLbl.Size = new Size(44, 15);
            itemWidthLbl.TabIndex = 35;
            itemWidthLbl.Text = "label25";
            // 
            // itemLengthLbl
            // 
            itemLengthLbl.AutoSize = true;
            itemLengthLbl.Location = new Point(399, 220);
            itemLengthLbl.Margin = new Padding(4, 0, 4, 0);
            itemLengthLbl.Name = "itemLengthLbl";
            itemLengthLbl.Size = new Size(44, 15);
            itemLengthLbl.TabIndex = 34;
            itemLengthLbl.Text = "label24";
            // 
            // itemWeightOzLbl
            // 
            itemWeightOzLbl.AutoSize = true;
            itemWeightOzLbl.Location = new Point(399, 188);
            itemWeightOzLbl.Margin = new Padding(4, 0, 4, 0);
            itemWeightOzLbl.Name = "itemWeightOzLbl";
            itemWeightOzLbl.Size = new Size(44, 15);
            itemWeightOzLbl.TabIndex = 33;
            itemWeightOzLbl.Text = "label23";
            // 
            // itemWeightLbsLbl
            // 
            itemWeightLbsLbl.AutoSize = true;
            itemWeightLbsLbl.Location = new Point(399, 157);
            itemWeightLbsLbl.Margin = new Padding(4, 0, 4, 0);
            itemWeightLbsLbl.Name = "itemWeightLbsLbl";
            itemWeightLbsLbl.Size = new Size(44, 15);
            itemWeightLbsLbl.TabIndex = 32;
            itemWeightLbsLbl.Text = "label22";
            // 
            // itemItemNoLbl
            // 
            itemItemNoLbl.AutoSize = true;
            itemItemNoLbl.Location = new Point(120, 281);
            itemItemNoLbl.Margin = new Padding(4, 0, 4, 0);
            itemItemNoLbl.Name = "itemItemNoLbl";
            itemItemNoLbl.Size = new Size(44, 15);
            itemItemNoLbl.TabIndex = 31;
            itemItemNoLbl.Text = "label21";
            // 
            // itemCurrQtyLbl
            // 
            itemCurrQtyLbl.AutoSize = true;
            itemCurrQtyLbl.Location = new Point(120, 250);
            itemCurrQtyLbl.Margin = new Padding(4, 0, 4, 0);
            itemCurrQtyLbl.Name = "itemCurrQtyLbl";
            itemCurrQtyLbl.Size = new Size(44, 15);
            itemCurrQtyLbl.TabIndex = 30;
            itemCurrQtyLbl.Text = "label20";
            // 
            // itemInitQtyLbl
            // 
            itemInitQtyLbl.AutoSize = true;
            itemInitQtyLbl.Location = new Point(120, 219);
            itemInitQtyLbl.Margin = new Padding(4, 0, 4, 0);
            itemInitQtyLbl.Name = "itemInitQtyLbl";
            itemInitQtyLbl.Size = new Size(44, 15);
            itemInitQtyLbl.TabIndex = 29;
            itemInitQtyLbl.Text = "label19";
            // 
            // itemSoldPriceLbl
            // 
            itemSoldPriceLbl.AutoSize = true;
            itemSoldPriceLbl.Location = new Point(120, 188);
            itemSoldPriceLbl.Margin = new Padding(4, 0, 4, 0);
            itemSoldPriceLbl.Name = "itemSoldPriceLbl";
            itemSoldPriceLbl.Size = new Size(44, 15);
            itemSoldPriceLbl.TabIndex = 28;
            itemSoldPriceLbl.Text = "label18";
            // 
            // itemPurcPriceLbl
            // 
            itemPurcPriceLbl.AutoSize = true;
            itemPurcPriceLbl.Location = new Point(120, 157);
            itemPurcPriceLbl.Margin = new Padding(4, 0, 4, 0);
            itemPurcPriceLbl.Name = "itemPurcPriceLbl";
            itemPurcPriceLbl.Size = new Size(44, 15);
            itemPurcPriceLbl.TabIndex = 27;
            itemPurcPriceLbl.Text = "label17";
            // 
            // itemNameTxtbox
            // 
            itemNameTxtbox.Location = new Point(120, 49);
            itemNameTxtbox.Margin = new Padding(4, 3, 4, 3);
            itemNameTxtbox.Name = "itemNameTxtbox";
            itemNameTxtbox.Size = new Size(309, 23);
            itemNameTxtbox.TabIndex = 14;
            itemNameTxtbox.Leave += TextBoxAttribute_Leave;
            // 
            // itemTagHeightLbl
            // 
            itemTagHeightLbl.AutoSize = true;
            itemTagHeightLbl.Location = new Point(343, 282);
            itemTagHeightLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagHeightLbl.Name = "itemTagHeightLbl";
            itemTagHeightLbl.Size = new Size(43, 15);
            itemTagHeightLbl.TabIndex = 12;
            itemTagHeightLbl.Text = "Height";
            // 
            // itemTagWidthLbl
            // 
            itemTagWidthLbl.AutoSize = true;
            itemTagWidthLbl.Location = new Point(347, 251);
            itemTagWidthLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagWidthLbl.Name = "itemTagWidthLbl";
            itemTagWidthLbl.Size = new Size(39, 15);
            itemTagWidthLbl.TabIndex = 11;
            itemTagWidthLbl.Text = "Width";
            // 
            // itemTagLengthLbl
            // 
            itemTagLengthLbl.AutoSize = true;
            itemTagLengthLbl.Location = new Point(321, 220);
            itemTagLengthLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagLengthLbl.Name = "itemTagLengthLbl";
            itemTagLengthLbl.Size = new Size(65, 15);
            itemTagLengthLbl.TabIndex = 10;
            itemTagLengthLbl.Text = "Length (in)";
            // 
            // itemTagWeightOzLbl
            // 
            itemTagWeightOzLbl.AutoSize = true;
            itemTagWeightOzLbl.Location = new Point(318, 188);
            itemTagWeightOzLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagWeightOzLbl.Name = "itemTagWeightOzLbl";
            itemTagWeightOzLbl.Size = new Size(68, 15);
            itemTagWeightOzLbl.TabIndex = 9;
            itemTagWeightOzLbl.Text = "Weight (oz)";
            // 
            // itemTagWeightLbsLbl
            // 
            itemTagWeightLbsLbl.AutoSize = true;
            itemTagWeightLbsLbl.Location = new Point(315, 157);
            itemTagWeightLbsLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagWeightLbsLbl.Name = "itemTagWeightLbsLbl";
            itemTagWeightLbsLbl.Size = new Size(71, 15);
            itemTagWeightLbsLbl.TabIndex = 8;
            itemTagWeightLbsLbl.Text = "Weight (lbs)";
            // 
            // itemTagItemNoLbl
            // 
            itemTagItemNoLbl.AutoSize = true;
            itemTagItemNoLbl.Location = new Point(57, 281);
            itemTagItemNoLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagItemNoLbl.Name = "itemTagItemNoLbl";
            itemTagItemNoLbl.Size = new Size(50, 15);
            itemTagItemNoLbl.TabIndex = 7;
            itemTagItemNoLbl.Text = "Item No";
            // 
            // itemTagInitQtyLbl
            // 
            itemTagInitQtyLbl.AutoSize = true;
            itemTagInitQtyLbl.Location = new Point(22, 219);
            itemTagInitQtyLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagInitQtyLbl.Name = "itemTagInitQtyLbl";
            itemTagInitQtyLbl.Size = new Size(85, 15);
            itemTagInitQtyLbl.TabIndex = 6;
            itemTagInitQtyLbl.Text = "Initial Quantity";
            // 
            // itemTagCurrQtyLbl
            // 
            itemTagCurrQtyLbl.AutoSize = true;
            itemTagCurrQtyLbl.Location = new Point(11, 250);
            itemTagCurrQtyLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagCurrQtyLbl.Name = "itemTagCurrQtyLbl";
            itemTagCurrQtyLbl.Size = new Size(96, 15);
            itemTagCurrQtyLbl.TabIndex = 5;
            itemTagCurrQtyLbl.Text = "Current Quantity";
            // 
            // itemTagSoldPriceLbl
            // 
            itemTagSoldPriceLbl.AutoSize = true;
            itemTagSoldPriceLbl.Location = new Point(48, 188);
            itemTagSoldPriceLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagSoldPriceLbl.Name = "itemTagSoldPriceLbl";
            itemTagSoldPriceLbl.Size = new Size(59, 15);
            itemTagSoldPriceLbl.TabIndex = 4;
            itemTagSoldPriceLbl.Text = "Sold Price";
            // 
            // itemTagPurcPriceLbl
            // 
            itemTagPurcPriceLbl.AutoSize = true;
            itemTagPurcPriceLbl.Location = new Point(23, 157);
            itemTagPurcPriceLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagPurcPriceLbl.Name = "itemTagPurcPriceLbl";
            itemTagPurcPriceLbl.Size = new Size(84, 15);
            itemTagPurcPriceLbl.TabIndex = 3;
            itemTagPurcPriceLbl.Text = "Purchase Price";
            // 
            // itemTagDatePurcLbl
            // 
            itemTagDatePurcLbl.AutoSize = true;
            itemTagDatePurcLbl.Location = new Point(14, 99);
            itemTagDatePurcLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagDatePurcLbl.Name = "itemTagDatePurcLbl";
            itemTagDatePurcLbl.Size = new Size(89, 15);
            itemTagDatePurcLbl.TabIndex = 2;
            itemTagDatePurcLbl.Text = "Date Purchased";
            // 
            // itemTagNameLbl
            // 
            itemTagNameLbl.AutoSize = true;
            itemTagNameLbl.Location = new Point(64, 52);
            itemTagNameLbl.Margin = new Padding(4, 0, 4, 0);
            itemTagNameLbl.Name = "itemTagNameLbl";
            itemTagNameLbl.Size = new Size(39, 15);
            itemTagNameLbl.TabIndex = 0;
            itemTagNameLbl.Text = "Name";
            // 
            // itemHeightTxtbox
            // 
            itemHeightTxtbox.Location = new Point(399, 279);
            itemHeightTxtbox.Name = "itemHeightTxtbox";
            itemHeightTxtbox.Size = new Size(100, 23);
            itemHeightTxtbox.TabIndex = 52;
            itemHeightTxtbox.Leave += TextBoxAttribute_Leave;
            // 
            // itemWidthTxtbox
            // 
            itemWidthTxtbox.Location = new Point(399, 248);
            itemWidthTxtbox.Name = "itemWidthTxtbox";
            itemWidthTxtbox.Size = new Size(100, 23);
            itemWidthTxtbox.TabIndex = 51;
            itemWidthTxtbox.Leave += TextBoxAttribute_Leave;
            // 
            // itemLengthTxtbox
            // 
            itemLengthTxtbox.Location = new Point(399, 217);
            itemLengthTxtbox.Name = "itemLengthTxtbox";
            itemLengthTxtbox.Size = new Size(100, 23);
            itemLengthTxtbox.TabIndex = 50;
            itemLengthTxtbox.Leave += TextBoxAttribute_Leave;
            // 
            // itemWeightOzTxtbox
            // 
            itemWeightOzTxtbox.Location = new Point(399, 185);
            itemWeightOzTxtbox.Name = "itemWeightOzTxtbox";
            itemWeightOzTxtbox.Size = new Size(100, 23);
            itemWeightOzTxtbox.TabIndex = 49;
            itemWeightOzTxtbox.Leave += TextBoxAttribute_Leave;
            // 
            // itemWeightLbsTxtbox
            // 
            itemWeightLbsTxtbox.Location = new Point(399, 154);
            itemWeightLbsTxtbox.Name = "itemWeightLbsTxtbox";
            itemWeightLbsTxtbox.Size = new Size(100, 23);
            itemWeightLbsTxtbox.TabIndex = 48;
            itemWeightLbsTxtbox.Leave += TextBoxAttribute_Leave;
            // 
            // itemCurrQtyTxtbox
            // 
            itemCurrQtyTxtbox.Location = new Point(120, 247);
            itemCurrQtyTxtbox.Name = "itemCurrQtyTxtbox";
            itemCurrQtyTxtbox.Size = new Size(100, 23);
            itemCurrQtyTxtbox.TabIndex = 47;
            itemCurrQtyTxtbox.Leave += TextBoxAttribute_Leave;
            // 
            // itemInitQtyTxtbox
            // 
            itemInitQtyTxtbox.Location = new Point(120, 216);
            itemInitQtyTxtbox.Name = "itemInitQtyTxtbox";
            itemInitQtyTxtbox.Size = new Size(100, 23);
            itemInitQtyTxtbox.TabIndex = 46;
            itemInitQtyTxtbox.Leave += TextBoxAttribute_Leave;
            // 
            // PurchaseTab
            // 
            PurchaseTab.Controls.Add(PurcPurcDateLbl);
            PurchaseTab.Controls.Add(PurcCurrQtyTextbox);
            PurchaseTab.Controls.Add(PurcTagCurrQtyLbl);
            PurchaseTab.Controls.Add(UpdatePurcButton);
            PurchaseTab.Controls.Add(PurcPurcNotesLbl);
            PurchaseTab.Controls.Add(PurcPurcPriceLbl);
            PurchaseTab.Controls.Add(EditPurcButton);
            PurchaseTab.Controls.Add(NewPurcButton);
            PurchaseTab.Controls.Add(PurcPurcNotesTextbox);
            PurchaseTab.Controls.Add(PurcTagPurcNotesLbl);
            PurchaseTab.Controls.Add(PurcTagPurcDateLbl);
            PurchaseTab.Controls.Add(PurcDatePicker);
            PurchaseTab.Controls.Add(PurcPurcPriceTextbox);
            PurchaseTab.Controls.Add(PurcWeightOzTextbox);
            PurchaseTab.Controls.Add(PurcWeightLbsTextbox);
            PurchaseTab.Controls.Add(PurcHeightTextbox);
            PurchaseTab.Controls.Add(PurcWidthTextbox);
            PurchaseTab.Controls.Add(PurcLengthTextbox);
            PurchaseTab.Controls.Add(PurcInitQtyTextbox);
            PurchaseTab.Controls.Add(PurcNameTextbox);
            PurchaseTab.Controls.Add(PurcTagWeightOzLbl);
            PurchaseTab.Controls.Add(ShippingInfoLbl);
            PurchaseTab.Controls.Add(PurcTagWeightLbs);
            PurchaseTab.Controls.Add(PurcTagHeightLbl);
            PurchaseTab.Controls.Add(PurcTagWidthLbl);
            PurchaseTab.Controls.Add(PurcTagLengthLbl);
            PurchaseTab.Controls.Add(PurcTagInitQtyLbl);
            PurchaseTab.Controls.Add(PurcTagNameLbl);
            PurchaseTab.Controls.Add(AddNewItemLbl);
            PurchaseTab.Controls.Add(addItemButton);
            PurchaseTab.Controls.Add(PurcTagPurcPriceLbl);
            PurchaseTab.Controls.Add(PurchaseListBox);
            PurchaseTab.Location = new Point(4, 24);
            PurchaseTab.Margin = new Padding(4, 3, 4, 3);
            PurchaseTab.Name = "PurchaseTab";
            PurchaseTab.Padding = new Padding(4, 3, 4, 3);
            PurchaseTab.Size = new Size(1111, 464);
            PurchaseTab.TabIndex = 2;
            PurchaseTab.Text = "Purchase Group";
            PurchaseTab.UseVisualStyleBackColor = true;
            // 
            // PurcPurcDateLbl
            // 
            PurcPurcDateLbl.AutoSize = true;
            PurcPurcDateLbl.Location = new Point(603, 105);
            PurcPurcDateLbl.Name = "PurcPurcDateLbl";
            PurcPurcDateLbl.Size = new Size(44, 15);
            PurcPurcDateLbl.TabIndex = 31;
            PurcPurcDateLbl.Text = "label44";
            // 
            // PurcCurrQtyTextbox
            // 
            PurcCurrQtyTextbox.Location = new Point(349, 412);
            PurcCurrQtyTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcCurrQtyTextbox.Name = "PurcCurrQtyTextbox";
            PurcCurrQtyTextbox.Size = new Size(96, 23);
            PurcCurrQtyTextbox.TabIndex = 30;
            // 
            // PurcTagCurrQtyLbl
            // 
            PurcTagCurrQtyLbl.AutoSize = true;
            PurcTagCurrQtyLbl.Location = new Point(349, 394);
            PurcTagCurrQtyLbl.Name = "PurcTagCurrQtyLbl";
            PurcTagCurrQtyLbl.Size = new Size(96, 15);
            PurcTagCurrQtyLbl.TabIndex = 29;
            PurcTagCurrQtyLbl.Text = "Current Quantity";
            // 
            // UpdatePurcButton
            // 
            UpdatePurcButton.Location = new Point(603, 198);
            UpdatePurcButton.Name = "UpdatePurcButton";
            UpdatePurcButton.Size = new Size(75, 23);
            UpdatePurcButton.TabIndex = 28;
            UpdatePurcButton.Text = "Update";
            UpdatePurcButton.UseVisualStyleBackColor = true;
            UpdatePurcButton.Click += updatePurcButton_Click;
            // 
            // PurcPurcNotesLbl
            // 
            PurcPurcNotesLbl.AutoSize = true;
            PurcPurcNotesLbl.Location = new Point(603, 158);
            PurcPurcNotesLbl.Name = "PurcPurcNotesLbl";
            PurcPurcNotesLbl.Size = new Size(44, 15);
            PurcPurcNotesLbl.TabIndex = 27;
            PurcPurcNotesLbl.Text = "label41";
            // 
            // PurcPurcPriceLbl
            // 
            PurcPurcPriceLbl.AutoSize = true;
            PurcPurcPriceLbl.Location = new Point(603, 51);
            PurcPurcPriceLbl.Name = "PurcPurcPriceLbl";
            PurcPurcPriceLbl.Size = new Size(44, 15);
            PurcPurcPriceLbl.TabIndex = 26;
            PurcPurcPriceLbl.Text = "label15";
            // 
            // EditPurcButton
            // 
            EditPurcButton.Location = new Point(633, 7);
            EditPurcButton.Name = "EditPurcButton";
            EditPurcButton.Size = new Size(75, 27);
            EditPurcButton.TabIndex = 25;
            EditPurcButton.Text = "Edit";
            EditPurcButton.UseVisualStyleBackColor = true;
            EditPurcButton.Click += editPurcButton_Click;
            // 
            // NewPurcButton
            // 
            NewPurcButton.Location = new Point(501, 7);
            NewPurcButton.Margin = new Padding(4, 3, 4, 3);
            NewPurcButton.Name = "NewPurcButton";
            NewPurcButton.Size = new Size(122, 27);
            NewPurcButton.TabIndex = 24;
            NewPurcButton.Text = "New Purchase";
            NewPurcButton.UseVisualStyleBackColor = true;
            NewPurcButton.Click += newPurcButton_Click;
            // 
            // PurcPurcNotesTextbox
            // 
            PurcPurcNotesTextbox.Location = new Point(603, 155);
            PurcPurcNotesTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcPurcNotesTextbox.Name = "PurcPurcNotesTextbox";
            PurcPurcNotesTextbox.Size = new Size(207, 23);
            PurcPurcNotesTextbox.TabIndex = 23;
            // 
            // PurcTagPurcNotesLbl
            // 
            PurcTagPurcNotesLbl.AutoSize = true;
            PurcTagPurcNotesLbl.Location = new Point(501, 158);
            PurcTagPurcNotesLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagPurcNotesLbl.Name = "PurcTagPurcNotesLbl";
            PurcTagPurcNotesLbl.Size = new Size(89, 15);
            PurcTagPurcNotesLbl.TabIndex = 22;
            PurcTagPurcNotesLbl.Text = "Purchase Notes";
            // 
            // PurcTagPurcDateLbl
            // 
            PurcTagPurcDateLbl.AutoSize = true;
            PurcTagPurcDateLbl.Location = new Point(501, 105);
            PurcTagPurcDateLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagPurcDateLbl.Name = "PurcTagPurcDateLbl";
            PurcTagPurcDateLbl.Size = new Size(82, 15);
            PurcTagPurcDateLbl.TabIndex = 21;
            PurcTagPurcDateLbl.Text = "Purchase Date";
            // 
            // PurcDatePicker
            // 
            PurcDatePicker.Location = new Point(603, 99);
            PurcDatePicker.Margin = new Padding(4, 3, 4, 3);
            PurcDatePicker.Name = "PurcDatePicker";
            PurcDatePicker.Size = new Size(233, 23);
            PurcDatePicker.TabIndex = 20;
            // 
            // PurcPurcPriceTextbox
            // 
            PurcPurcPriceTextbox.Location = new Point(603, 48);
            PurcPurcPriceTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcPurcPriceTextbox.Name = "PurcPurcPriceTextbox";
            PurcPurcPriceTextbox.Size = new Size(116, 23);
            PurcPurcPriceTextbox.TabIndex = 19;
            // 
            // PurcWeightOzTextbox
            // 
            PurcWeightOzTextbox.Location = new Point(801, 413);
            PurcWeightOzTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcWeightOzTextbox.Name = "PurcWeightOzTextbox";
            PurcWeightOzTextbox.Size = new Size(40, 23);
            PurcWeightOzTextbox.TabIndex = 18;
            // 
            // PurcWeightLbsTextbox
            // 
            PurcWeightLbsTextbox.Location = new Point(712, 413);
            PurcWeightLbsTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcWeightLbsTextbox.Name = "PurcWeightLbsTextbox";
            PurcWeightLbsTextbox.Size = new Size(40, 23);
            PurcWeightLbsTextbox.TabIndex = 17;
            // 
            // PurcHeightTextbox
            // 
            PurcHeightTextbox.Location = new Point(633, 413);
            PurcHeightTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcHeightTextbox.Name = "PurcHeightTextbox";
            PurcHeightTextbox.Size = new Size(40, 23);
            PurcHeightTextbox.TabIndex = 16;
            // 
            // PurcWidthTextbox
            // 
            PurcWidthTextbox.Location = new Point(557, 413);
            PurcWidthTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcWidthTextbox.Name = "PurcWidthTextbox";
            PurcWidthTextbox.Size = new Size(40, 23);
            PurcWidthTextbox.TabIndex = 15;
            // 
            // PurcLengthTextbox
            // 
            PurcLengthTextbox.Location = new Point(475, 413);
            PurcLengthTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcLengthTextbox.Name = "PurcLengthTextbox";
            PurcLengthTextbox.Size = new Size(40, 23);
            PurcLengthTextbox.TabIndex = 14;
            // 
            // PurcInitQtyTextbox
            // 
            PurcInitQtyTextbox.Location = new Point(260, 413);
            PurcInitQtyTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcInitQtyTextbox.Name = "PurcInitQtyTextbox";
            PurcInitQtyTextbox.Size = new Size(81, 23);
            PurcInitQtyTextbox.TabIndex = 13;
            // 
            // PurcNameTextbox
            // 
            PurcNameTextbox.Location = new Point(111, 413);
            PurcNameTextbox.Margin = new Padding(4, 3, 4, 3);
            PurcNameTextbox.Name = "PurcNameTextbox";
            PurcNameTextbox.Size = new Size(142, 23);
            PurcNameTextbox.TabIndex = 12;
            // 
            // PurcTagWeightOzLbl
            // 
            PurcTagWeightOzLbl.AutoSize = true;
            PurcTagWeightOzLbl.Location = new Point(797, 395);
            PurcTagWeightOzLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagWeightOzLbl.Name = "PurcTagWeightOzLbl";
            PurcTagWeightOzLbl.Size = new Size(68, 15);
            PurcTagWeightOzLbl.TabIndex = 11;
            PurcTagWeightOzLbl.Text = "Weight (oz)";
            // 
            // ShippingInfoLbl
            // 
            ShippingInfoLbl.AutoSize = true;
            ShippingInfoLbl.Location = new Point(472, 375);
            ShippingInfoLbl.Margin = new Padding(4, 0, 4, 0);
            ShippingInfoLbl.Name = "ShippingInfoLbl";
            ShippingInfoLbl.Size = new Size(138, 15);
            ShippingInfoLbl.TabIndex = 10;
            ShippingInfoLbl.Text = "Shipping Info: (Optional)";
            // 
            // PurcTagWeightLbs
            // 
            PurcTagWeightLbs.AutoSize = true;
            PurcTagWeightLbs.Location = new Point(709, 395);
            PurcTagWeightLbs.Margin = new Padding(4, 0, 4, 0);
            PurcTagWeightLbs.Name = "PurcTagWeightLbs";
            PurcTagWeightLbs.Size = new Size(71, 15);
            PurcTagWeightLbs.TabIndex = 9;
            PurcTagWeightLbs.Text = "Weight (lbs)";
            // 
            // PurcTagHeightLbl
            // 
            PurcTagHeightLbl.AutoSize = true;
            PurcTagHeightLbl.Location = new Point(629, 395);
            PurcTagHeightLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagHeightLbl.Name = "PurcTagHeightLbl";
            PurcTagHeightLbl.Size = new Size(64, 15);
            PurcTagHeightLbl.TabIndex = 8;
            PurcTagHeightLbl.Text = "Height (in)";
            // 
            // PurcTagWidthLbl
            // 
            PurcTagWidthLbl.AutoSize = true;
            PurcTagWidthLbl.Location = new Point(553, 395);
            PurcTagWidthLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagWidthLbl.Name = "PurcTagWidthLbl";
            PurcTagWidthLbl.Size = new Size(60, 15);
            PurcTagWidthLbl.TabIndex = 7;
            PurcTagWidthLbl.Text = "Width (in)";
            // 
            // PurcTagLengthLbl
            // 
            PurcTagLengthLbl.AutoSize = true;
            PurcTagLengthLbl.Location = new Point(472, 395);
            PurcTagLengthLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagLengthLbl.Name = "PurcTagLengthLbl";
            PurcTagLengthLbl.Size = new Size(65, 15);
            PurcTagLengthLbl.TabIndex = 6;
            PurcTagLengthLbl.Text = "Length (in)";
            // 
            // PurcTagInitQtyLbl
            // 
            PurcTagInitQtyLbl.AutoSize = true;
            PurcTagInitQtyLbl.Location = new Point(257, 395);
            PurcTagInitQtyLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagInitQtyLbl.Name = "PurcTagInitQtyLbl";
            PurcTagInitQtyLbl.Size = new Size(85, 15);
            PurcTagInitQtyLbl.TabIndex = 5;
            PurcTagInitQtyLbl.Text = "Initial Quantity";
            // 
            // PurcTagNameLbl
            // 
            PurcTagNameLbl.AutoSize = true;
            PurcTagNameLbl.Location = new Point(107, 395);
            PurcTagNameLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagNameLbl.Name = "PurcTagNameLbl";
            PurcTagNameLbl.Size = new Size(66, 15);
            PurcTagNameLbl.TabIndex = 4;
            PurcTagNameLbl.Text = "Item Name";
            // 
            // AddNewItemLbl
            // 
            AddNewItemLbl.AutoSize = true;
            AddNewItemLbl.Location = new Point(7, 375);
            AddNewItemLbl.Margin = new Padding(4, 0, 4, 0);
            AddNewItemLbl.Name = "AddNewItemLbl";
            AddNewItemLbl.Size = new Size(86, 15);
            AddNewItemLbl.TabIndex = 3;
            AddNewItemLbl.Text = "Add New Item:";
            // 
            // addItemButton
            // 
            addItemButton.Location = new Point(7, 409);
            addItemButton.Margin = new Padding(4, 3, 4, 3);
            addItemButton.Name = "addItemButton";
            addItemButton.Size = new Size(88, 27);
            addItemButton.TabIndex = 2;
            addItemButton.Text = "Add Item";
            addItemButton.UseVisualStyleBackColor = true;
            addItemButton.Click += addItemButton_Click;
            // 
            // PurcTagPurcPriceLbl
            // 
            PurcTagPurcPriceLbl.AutoSize = true;
            PurcTagPurcPriceLbl.Location = new Point(501, 51);
            PurcTagPurcPriceLbl.Margin = new Padding(4, 0, 4, 0);
            PurcTagPurcPriceLbl.Name = "PurcTagPurcPriceLbl";
            PurcTagPurcPriceLbl.Size = new Size(84, 15);
            PurcTagPurcPriceLbl.TabIndex = 1;
            PurcTagPurcPriceLbl.Text = "Purchase Price";
            // 
            // PurchaseListBox
            // 
            PurchaseListBox.FormattingEnabled = true;
            PurchaseListBox.ItemHeight = 15;
            PurchaseListBox.Location = new Point(7, 7);
            PurchaseListBox.Margin = new Padding(4, 3, 4, 3);
            PurchaseListBox.Name = "PurchaseListBox";
            PurchaseListBox.Size = new Size(468, 319);
            PurchaseListBox.TabIndex = 0;
            PurchaseListBox.MouseDoubleClick += purcListBox_DoubleClick;
            // 
            // Sale
            // 
            Sale.Controls.Add(SaleDeleteSaleButton);
            Sale.Controls.Add(SaleUpdateButton);
            Sale.Controls.Add(SaleDateLbl);
            Sale.Controls.Add(SaleEditSaleButton);
            Sale.Controls.Add(SaleTagNameLbl);
            Sale.Controls.Add(SaleTagSaleDateLbl);
            Sale.Controls.Add(SaleDatePicker);
            Sale.Controls.Add(SaleNameLbl);
            Sale.Controls.Add(SaleTagNewSaleDateLbl);
            Sale.Controls.Add(SaleNewSaleDatePicker);
            Sale.Controls.Add(SaleAmountLbl);
            Sale.Controls.Add(SaleNewSaleAmountTextbox);
            Sale.Controls.Add(SaleTagNewSaleAmountLbl);
            Sale.Controls.Add(AddSaleButton);
            Sale.Controls.Add(SaleAmountTextbox);
            Sale.Controls.Add(SaleTagSaleAmountLbl);
            Sale.Controls.Add(saleListBox);
            Sale.Location = new Point(4, 24);
            Sale.Margin = new Padding(4, 3, 4, 3);
            Sale.Name = "Sale";
            Sale.Size = new Size(1111, 464);
            Sale.TabIndex = 4;
            Sale.Text = "Sale";
            Sale.UseVisualStyleBackColor = true;
            // 
            // SaleDeleteSaleButton
            // 
            SaleDeleteSaleButton.Location = new Point(706, 136);
            SaleDeleteSaleButton.Name = "SaleDeleteSaleButton";
            SaleDeleteSaleButton.Size = new Size(75, 23);
            SaleDeleteSaleButton.TabIndex = 31;
            SaleDeleteSaleButton.Text = "Delete";
            SaleDeleteSaleButton.UseVisualStyleBackColor = true;
            SaleDeleteSaleButton.Click += saleDeleteButton_Click;
            // 
            // SaleUpdateButton
            // 
            SaleUpdateButton.Location = new Point(625, 136);
            SaleUpdateButton.Name = "SaleUpdateButton";
            SaleUpdateButton.Size = new Size(75, 23);
            SaleUpdateButton.TabIndex = 30;
            SaleUpdateButton.Text = "Update";
            SaleUpdateButton.UseVisualStyleBackColor = true;
            SaleUpdateButton.Click += saleUpdateButton_Click;
            // 
            // SaleDateLbl
            // 
            SaleDateLbl.AutoSize = true;
            SaleDateLbl.Location = new Point(625, 113);
            SaleDateLbl.Name = "SaleDateLbl";
            SaleDateLbl.Size = new Size(44, 15);
            SaleDateLbl.TabIndex = 29;
            SaleDateLbl.Text = "label54";
            // 
            // SaleEditSaleButton
            // 
            SaleEditSaleButton.Location = new Point(625, 3);
            SaleEditSaleButton.Name = "SaleEditSaleButton";
            SaleEditSaleButton.Size = new Size(75, 23);
            SaleEditSaleButton.TabIndex = 28;
            SaleEditSaleButton.Text = "Edit";
            SaleEditSaleButton.UseVisualStyleBackColor = true;
            SaleEditSaleButton.Click += saleEditSaleButton_Click;
            // 
            // SaleTagNameLbl
            // 
            SaleTagNameLbl.AutoSize = true;
            SaleTagNameLbl.Location = new Point(547, 44);
            SaleTagNameLbl.Name = "SaleTagNameLbl";
            SaleTagNameLbl.Size = new Size(66, 15);
            SaleTagNameLbl.TabIndex = 27;
            SaleTagNameLbl.Text = "Item Name";
            // 
            // SaleTagSaleDateLbl
            // 
            SaleTagSaleDateLbl.AutoSize = true;
            SaleTagSaleDateLbl.Location = new Point(558, 113);
            SaleTagSaleDateLbl.Name = "SaleTagSaleDateLbl";
            SaleTagSaleDateLbl.Size = new Size(55, 15);
            SaleTagSaleDateLbl.TabIndex = 26;
            SaleTagSaleDateLbl.Text = "Sale Date";
            // 
            // SaleDatePicker
            // 
            SaleDatePicker.Location = new Point(625, 107);
            SaleDatePicker.Name = "SaleDatePicker";
            SaleDatePicker.Size = new Size(200, 23);
            SaleDatePicker.TabIndex = 25;
            // 
            // SaleNameLbl
            // 
            SaleNameLbl.AutoSize = true;
            SaleNameLbl.Location = new Point(625, 44);
            SaleNameLbl.Name = "SaleNameLbl";
            SaleNameLbl.Size = new Size(44, 15);
            SaleNameLbl.TabIndex = 24;
            SaleNameLbl.Text = "label51";
            // 
            // SaleTagNewSaleDateLbl
            // 
            SaleTagNewSaleDateLbl.AutoSize = true;
            SaleTagNewSaleDateLbl.Location = new Point(201, 356);
            SaleTagNewSaleDateLbl.Name = "SaleTagNewSaleDateLbl";
            SaleTagNewSaleDateLbl.Size = new Size(55, 15);
            SaleTagNewSaleDateLbl.TabIndex = 23;
            SaleTagNewSaleDateLbl.Text = "Sale Date";
            // 
            // SaleNewSaleDatePicker
            // 
            SaleNewSaleDatePicker.Location = new Point(201, 374);
            SaleNewSaleDatePicker.Name = "SaleNewSaleDatePicker";
            SaleNewSaleDatePicker.Size = new Size(200, 23);
            SaleNewSaleDatePicker.TabIndex = 22;
            // 
            // SaleAmountLbl
            // 
            SaleAmountLbl.AutoSize = true;
            SaleAmountLbl.Location = new Point(625, 79);
            SaleAmountLbl.Name = "SaleAmountLbl";
            SaleAmountLbl.Size = new Size(44, 15);
            SaleAmountLbl.TabIndex = 20;
            SaleAmountLbl.Text = "label48";
            // 
            // SaleNewSaleAmountTextbox
            // 
            SaleNewSaleAmountTextbox.Location = new Point(108, 374);
            SaleNewSaleAmountTextbox.Margin = new Padding(4, 3, 4, 3);
            SaleNewSaleAmountTextbox.Name = "SaleNewSaleAmountTextbox";
            SaleNewSaleAmountTextbox.Size = new Size(80, 23);
            SaleNewSaleAmountTextbox.TabIndex = 18;
            // 
            // SaleTagNewSaleAmountLbl
            // 
            SaleTagNewSaleAmountLbl.AutoSize = true;
            SaleTagNewSaleAmountLbl.Location = new Point(104, 356);
            SaleTagNewSaleAmountLbl.Margin = new Padding(4, 0, 4, 0);
            SaleTagNewSaleAmountLbl.Name = "SaleTagNewSaleAmountLbl";
            SaleTagNewSaleAmountLbl.Size = new Size(75, 15);
            SaleTagNewSaleAmountLbl.TabIndex = 16;
            SaleTagNewSaleAmountLbl.Text = "Sale Amount_purchase";
            // 
            // AddSaleButton
            // 
            AddSaleButton.Location = new Point(4, 370);
            AddSaleButton.Margin = new Padding(4, 3, 4, 3);
            AddSaleButton.Name = "AddSaleButton";
            AddSaleButton.Size = new Size(88, 27);
            AddSaleButton.TabIndex = 14;
            AddSaleButton.Text = "Add Sale";
            AddSaleButton.UseVisualStyleBackColor = true;
            AddSaleButton.Click += addSaleButton_Click;
            // 
            // SaleAmountTextbox
            // 
            SaleAmountTextbox.Location = new Point(625, 76);
            SaleAmountTextbox.Margin = new Padding(4, 3, 4, 3);
            SaleAmountTextbox.Name = "SaleAmountTextbox";
            SaleAmountTextbox.Size = new Size(116, 23);
            SaleAmountTextbox.TabIndex = 4;
            // 
            // SaleTagSaleAmountLbl
            // 
            SaleTagSaleAmountLbl.AutoSize = true;
            SaleTagSaleAmountLbl.Location = new Point(538, 79);
            SaleTagSaleAmountLbl.Margin = new Padding(4, 0, 4, 0);
            SaleTagSaleAmountLbl.Name = "SaleTagSaleAmountLbl";
            SaleTagSaleAmountLbl.Size = new Size(75, 15);
            SaleTagSaleAmountLbl.TabIndex = 1;
            SaleTagSaleAmountLbl.Text = "Sale Amount_purchase";
            // 
            // saleListBox
            // 
            saleListBox.FormattingEnabled = true;
            saleListBox.ItemHeight = 15;
            saleListBox.Location = new Point(4, 3);
            saleListBox.Margin = new Padding(4, 3, 4, 3);
            saleListBox.Name = "saleListBox";
            saleListBox.Size = new Size(527, 304);
            saleListBox.TabIndex = 0;
            saleListBox.MouseDoubleClick += saleListBox_DoubleClick;
            // 
            // resultItemBindingSource
            // 
            resultItemBindingSource.DataSource = typeof(Item);
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Multiselect = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1146, 519);
            Controls.Add(tabCollection);
            MainMenuStrip = comboBoxMenuStrip;
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabCollection.ResumeLayout(false);
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
        public System.Windows.Forms.TabControl tabCollection;
        public System.Windows.Forms.TabPage SearchTab;
        public System.Windows.Forms.TabPage ItemTab;
        public System.Windows.Forms.MenuStrip comboBoxMenuStrip;
        public System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.TextBox searchBox;
        public System.Windows.Forms.Button searchButton;
        public System.Windows.Forms.DateTimePicker boughtBeforeDatePicker;
        public System.Windows.Forms.DateTimePicker boughtAfterDatePicker;
        public System.Windows.Forms.Label boughtBeforeLbl;
        public System.Windows.Forms.Label boughtAfterLbl;
        public System.Windows.Forms.CheckBox inStockCheckBox;
        public System.Windows.Forms.CheckBox soldOutCheckBox;
        public System.Windows.Forms.CheckBox showPurcPriceCheckBox;
        public System.Windows.Forms.CheckBox showDateCheckBox;
        public System.Windows.Forms.Label itemTagNameLbl;
        public System.Windows.Forms.Label itemTagWeightOzLbl;
        public System.Windows.Forms.Label itemTagWeightLbsLbl;
        public System.Windows.Forms.Label itemTagItemNoLbl;
        public System.Windows.Forms.Label itemTagInitQtyLbl;
        public System.Windows.Forms.Label itemTagCurrQtyLbl;
        public System.Windows.Forms.Label itemTagSoldPriceLbl;
        public System.Windows.Forms.Label itemTagPurcPriceLbl;
        public System.Windows.Forms.Label itemTagDatePurcLbl;
        public System.Windows.Forms.Label itemTagHeightLbl;
        public System.Windows.Forms.Label itemTagWidthLbl;
        public System.Windows.Forms.Label itemTagLengthLbl;
        public System.Windows.Forms.TextBox itemNameTxtbox;
        public System.Windows.Forms.Label itemWidthLbl;
        public System.Windows.Forms.Label itemLengthLbl;
        public System.Windows.Forms.Label itemWeightOzLbl;
        public System.Windows.Forms.Label itemWeightLbsLbl;
        public System.Windows.Forms.Label itemItemNoLbl;
        public System.Windows.Forms.Label itemCurrQtyLbl;
        public System.Windows.Forms.Label itemInitQtyLbl;
        public System.Windows.Forms.Label itemSoldPriceLbl;
        public System.Windows.Forms.Label itemPurcPriceLbl;
        public System.Windows.Forms.Label itemHeightLbl;
        public System.Windows.Forms.Button itemUpdateButton;
        public System.Windows.Forms.TabPage PurchaseTab;
        public System.Windows.Forms.ListBox PurchaseListBox;
        public System.Windows.Forms.Label ShippingInfoLbl;
        public System.Windows.Forms.Label PurcTagWeightLbs;
        public System.Windows.Forms.Label PurcTagHeightLbl;
        public System.Windows.Forms.Label PurcTagWidthLbl;
        public System.Windows.Forms.Label PurcTagLengthLbl;
        public System.Windows.Forms.Label PurcTagInitQtyLbl;
        public System.Windows.Forms.Label PurcTagNameLbl;
        public System.Windows.Forms.Label AddNewItemLbl;
        public System.Windows.Forms.Button addItemButton;
        public System.Windows.Forms.Label PurcTagPurcPriceLbl;
        public System.Windows.Forms.TextBox PurcWeightOzTextbox;
        public System.Windows.Forms.TextBox PurcWeightLbsTextbox;
        public System.Windows.Forms.TextBox PurcHeightTextbox;
        public System.Windows.Forms.TextBox PurcWidthTextbox;
        public System.Windows.Forms.TextBox PurcLengthTextbox;
        public System.Windows.Forms.TextBox PurcInitQtyTextbox;
        public System.Windows.Forms.TextBox PurcNameTextbox;
        public System.Windows.Forms.Label PurcTagWeightOzLbl;
        public System.Windows.Forms.TextBox PurcPurcNotesTextbox;
        public System.Windows.Forms.Label PurcTagPurcNotesLbl;
        public System.Windows.Forms.Label PurcTagPurcDateLbl;
        public System.Windows.Forms.DateTimePicker PurcDatePicker;
        public System.Windows.Forms.TextBox PurcPurcPriceTextbox;
        public System.Windows.Forms.TabPage Sale;
        public System.Windows.Forms.Button NewPurcButton;
        public System.Windows.Forms.ListBox saleListBox;
        public System.Windows.Forms.BindingSource resultItemBindingSource;
        public System.Windows.Forms.Label SaleTagSaleAmountLbl;
        public System.Windows.Forms.TextBox SaleAmountTextbox;
        public TextBox itemHeightTxtbox;
        public TextBox itemWidthTxtbox;
        public TextBox itemWeightOzTxtbox;
        public TextBox itemWeightLbsTxtbox;
        public TextBox itemCurrQtyTxtbox;
        public TextBox itemInitQtyTxtbox;
        public TextBox itemLengthTxtbox;
        public Button itemEditButton;
        public Label itemNameLbl;
        public Button deleteShipInfoButton;
        public Button EditPurcButton;
        public Label PurcPurcNotesLbl;
        public Label PurcPurcPriceLbl;
        public Button UpdatePurcButton;
        public TextBox PurcCurrQtyTextbox;
        public Label PurcTagCurrQtyLbl;
        public Label itemDatePurcLbl;
        public Label PurcPurcDateLbl;
        public Label SaleNameLbl;
        public Label SaleTagNewSaleDateLbl;
        public DateTimePicker SaleNewSaleDatePicker;
        public Label SaleAmountLbl;
        public TextBox SaleNewSaleAmountTextbox;
        public Label SaleTagNewSaleAmountLbl;
        public Label label47;
        public Button AddSaleButton;
        public DateTimePicker SaleDatePicker;
        public Label SaleTagNameLbl;
        public Label SaleTagSaleDateLbl;
        public Button SaleEditSaleButton;
        public Label SaleDateLbl;
        public Label label49;
        public Button SaleUpdateButton;
        public Button SaleDeleteSaleButton;
        public Button deleteItemButton;
        public ItemSearchViewer itemSearchView;
        public AllPictureViewer allPictureViewer;
        public MainPictureViewer mainPictureViewer;
        public Button deleteImageButton;
        public Button AddImageButton;
        public OpenFileDialog openFileDialog1;
        private Button setThumbnailButton;
    }
}

