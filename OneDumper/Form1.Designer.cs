namespace OneDumper
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Yerel", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Uzak", System.Windows.Forms.HorizontalAlignment.Left);
            this.partList = new System.Windows.Forms.DataGridView();
            this.materialRaisedButton1 = new MaterialSkin.Controls.MaterialRaisedButton();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.materialDivider1 = new MaterialSkin.Controls.MaterialDivider();
            this.materialDivider2 = new MaterialSkin.Controls.MaterialDivider();
            this.materialDivider3 = new MaterialSkin.Controls.MaterialDivider();
            this.materialDivider4 = new MaterialSkin.Controls.MaterialDivider();
            this.deviceList = new System.Windows.Forms.ComboBox();
            this.programmerList = new System.Windows.Forms.ListView();
            this.programmer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.materialRaisedButton3 = new MaterialSkin.Controls.MaterialRaisedButton();
            this.materialRaisedButton2 = new MaterialSkin.Controls.MaterialRaisedButton();
            this.materialRaisedButton4 = new MaterialSkin.Controls.MaterialRaisedButton();
            this.log = new System.Windows.Forms.DataGridView();
            this.unixtime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.backup_type = new System.Windows.Forms.ComboBox();
            this.data_type = new System.Windows.Forms.ComboBox();
            this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
            this.partition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.read = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.sector_size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sector_offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.partition_sectors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.partition_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.size_kb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sparse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.start_byte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.start_sector = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.partList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.log)).BeginInit();
            this.SuspendLayout();
            // 
            // partList
            // 
            this.partList.AllowUserToAddRows = false;
            this.partList.AllowUserToDeleteRows = false;
            this.partList.AllowUserToResizeColumns = false;
            this.partList.AllowUserToResizeRows = false;
            this.partList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.partList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.partList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.partition,
            this.size,
            this.read,
            this.sector_size,
            this.sector_offset,
            this.filename,
            this.partition_sectors,
            this.partition_number,
            this.size_kb,
            this.sparse,
            this.start_byte,
            this.start_sector});
            this.partList.Location = new System.Drawing.Point(349, 76);
            this.partList.MultiSelect = false;
            this.partList.Name = "partList";
            this.partList.RowHeadersVisible = false;
            this.partList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.partList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.partList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.partList.Size = new System.Drawing.Size(636, 371);
            this.partList.TabIndex = 0;
            // 
            // materialRaisedButton1
            // 
            this.materialRaisedButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialRaisedButton1.Depth = 0;
            this.materialRaisedButton1.Enabled = false;
            this.materialRaisedButton1.Location = new System.Drawing.Point(883, 456);
            this.materialRaisedButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialRaisedButton1.Name = "materialRaisedButton1";
            this.materialRaisedButton1.Primary = true;
            this.materialRaisedButton1.Size = new System.Drawing.Size(102, 36);
            this.materialRaisedButton1.TabIndex = 6;
            this.materialRaisedButton1.Text = "Yedekle";
            this.materialRaisedButton1.UseVisualStyleBackColor = true;
            this.materialRaisedButton1.Click += new System.EventHandler(this.materialRaisedButton1_ClickAsync);
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(12, 136);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(92, 19);
            this.materialLabel1.TabIndex = 8;
            this.materialLabel1.Text = "Programmer";
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(16, 76);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(46, 19);
            this.materialLabel2.TabIndex = 9;
            this.materialLabel2.Text = "Cihaz";
            // 
            // materialDivider1
            // 
            this.materialDivider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialDivider1.Depth = 0;
            this.materialDivider1.Location = new System.Drawing.Point(12, 456);
            this.materialDivider1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider1.Name = "materialDivider1";
            this.materialDivider1.Size = new System.Drawing.Size(865, 2);
            this.materialDivider1.TabIndex = 91;
            this.materialDivider1.Text = "materialDivider1";
            // 
            // materialDivider2
            // 
            this.materialDivider2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialDivider2.Depth = 0;
            this.materialDivider2.Location = new System.Drawing.Point(12, 575);
            this.materialDivider2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider2.Name = "materialDivider2";
            this.materialDivider2.Size = new System.Drawing.Size(865, 2);
            this.materialDivider2.TabIndex = 92;
            this.materialDivider2.Text = "materialDivider2";
            // 
            // materialDivider3
            // 
            this.materialDivider3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialDivider3.Depth = 0;
            this.materialDivider3.Location = new System.Drawing.Point(12, 456);
            this.materialDivider3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider3.Name = "materialDivider3";
            this.materialDivider3.Size = new System.Drawing.Size(2, 120);
            this.materialDivider3.TabIndex = 93;
            this.materialDivider3.Text = "materialDivider3";
            // 
            // materialDivider4
            // 
            this.materialDivider4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialDivider4.Depth = 0;
            this.materialDivider4.Location = new System.Drawing.Point(875, 456);
            this.materialDivider4.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider4.Name = "materialDivider4";
            this.materialDivider4.Size = new System.Drawing.Size(2, 120);
            this.materialDivider4.TabIndex = 94;
            this.materialDivider4.Text = "materialDivider4";
            // 
            // deviceList
            // 
            this.deviceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceList.FormattingEnabled = true;
            this.deviceList.Location = new System.Drawing.Point(16, 104);
            this.deviceList.Name = "deviceList";
            this.deviceList.Size = new System.Drawing.Size(242, 21);
            this.deviceList.TabIndex = 96;
            // 
            // programmerList
            // 
            this.programmerList.CheckBoxes = true;
            this.programmerList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.programmer});
            this.programmerList.FullRowSelect = true;
            this.programmerList.GridLines = true;
            listViewGroup3.Header = "Yerel";
            listViewGroup3.Name = "l";
            listViewGroup4.Header = "Uzak";
            listViewGroup4.Name = "r";
            this.programmerList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            this.programmerList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.programmerList.Location = new System.Drawing.Point(16, 163);
            this.programmerList.MultiSelect = false;
            this.programmerList.Name = "programmerList";
            this.programmerList.ShowItemToolTips = true;
            this.programmerList.Size = new System.Drawing.Size(327, 165);
            this.programmerList.TabIndex = 97;
            this.programmerList.UseCompatibleStateImageBehavior = false;
            this.programmerList.View = System.Windows.Forms.View.Details;
            // 
            // programmer
            // 
            this.programmer.Text = "";
            this.programmer.Width = 305;
            // 
            // materialRaisedButton3
            // 
            this.materialRaisedButton3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialRaisedButton3.Depth = 0;
            this.materialRaisedButton3.Location = new System.Drawing.Point(883, 541);
            this.materialRaisedButton3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialRaisedButton3.Name = "materialRaisedButton3";
            this.materialRaisedButton3.Primary = true;
            this.materialRaisedButton3.Size = new System.Drawing.Size(102, 36);
            this.materialRaisedButton3.TabIndex = 100;
            this.materialRaisedButton3.Text = "Log Kaydet";
            this.materialRaisedButton3.UseVisualStyleBackColor = true;
            this.materialRaisedButton3.Click += new System.EventHandler(this.materialRaisedButton3_ClickAsync);
            // 
            // materialRaisedButton2
            // 
            this.materialRaisedButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialRaisedButton2.Depth = 0;
            this.materialRaisedButton2.Location = new System.Drawing.Point(882, 498);
            this.materialRaisedButton2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialRaisedButton2.Name = "materialRaisedButton2";
            this.materialRaisedButton2.Primary = true;
            this.materialRaisedButton2.Size = new System.Drawing.Size(103, 36);
            this.materialRaisedButton2.TabIndex = 101;
            this.materialRaisedButton2.Text = "Sürücü Kur";
            this.materialRaisedButton2.UseVisualStyleBackColor = true;
            this.materialRaisedButton2.Click += new System.EventHandler(this.materialRaisedButton2_ClickAsync);
            // 
            // materialRaisedButton4
            // 
            this.materialRaisedButton4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialRaisedButton4.Depth = 0;
            this.materialRaisedButton4.Location = new System.Drawing.Point(264, 100);
            this.materialRaisedButton4.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialRaisedButton4.Name = "materialRaisedButton4";
            this.materialRaisedButton4.Primary = true;
            this.materialRaisedButton4.Size = new System.Drawing.Size(79, 30);
            this.materialRaisedButton4.TabIndex = 102;
            this.materialRaisedButton4.Text = "Bağlan";
            this.materialRaisedButton4.UseVisualStyleBackColor = true;
            this.materialRaisedButton4.Click += new System.EventHandler(this.materialRaisedButton4_ClickAsync);
            // 
            // log
            // 
            this.log.AllowUserToAddRows = false;
            this.log.AllowUserToDeleteRows = false;
            this.log.AllowUserToResizeColumns = false;
            this.log.AllowUserToResizeRows = false;
            this.log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.log.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.unixtime,
            this.type,
            this.date,
            this.message});
            this.log.Location = new System.Drawing.Point(20, 464);
            this.log.MultiSelect = false;
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.RowHeadersVisible = false;
            this.log.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.log.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.log.Size = new System.Drawing.Size(849, 105);
            this.log.TabIndex = 103;
            // 
            // unixtime
            // 
            this.unixtime.HeaderText = "unixtime";
            this.unixtime.Name = "unixtime";
            this.unixtime.ReadOnly = true;
            this.unixtime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.unixtime.Visible = false;
            // 
            // type
            // 
            this.type.HeaderText = "type";
            this.type.Name = "type";
            this.type.ReadOnly = true;
            this.type.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.type.Visible = false;
            // 
            // date
            // 
            this.date.HeaderText = "Tarih";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.date.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.date.Width = 125;
            // 
            // message
            // 
            this.message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.message.HeaderText = "Mesaj";
            this.message.Name = "message";
            this.message.ReadOnly = true;
            this.message.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel3.Location = new System.Drawing.Point(16, 339);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(84, 19);
            this.materialLabel3.TabIndex = 104;
            this.materialLabel3.Text = "Yedek Türü";
            // 
            // backup_type
            // 
            this.backup_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.backup_type.FormattingEnabled = true;
            this.backup_type.Items.AddRange(new object[] {
            "QFIL",
            "Fastboot"});
            this.backup_type.Location = new System.Drawing.Point(16, 366);
            this.backup_type.Name = "backup_type";
            this.backup_type.Size = new System.Drawing.Size(327, 21);
            this.backup_type.TabIndex = 106;
            // 
            // data_type
            // 
            this.data_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.data_type.FormattingEnabled = true;
            this.data_type.Items.AddRange(new object[] {
            "Orjinal Userdata Kullan",
            "Dummy Userdata Kullan"});
            this.data_type.Location = new System.Drawing.Point(16, 426);
            this.data_type.Name = "data_type";
            this.data_type.Size = new System.Drawing.Size(327, 21);
            this.data_type.TabIndex = 108;
            // 
            // materialLabel4
            // 
            this.materialLabel4.AutoSize = true;
            this.materialLabel4.Depth = 0;
            this.materialLabel4.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel4.Location = new System.Drawing.Point(16, 399);
            this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel4.Name = "materialLabel4";
            this.materialLabel4.Size = new System.Drawing.Size(114, 19);
            this.materialLabel4.TabIndex = 107;
            this.materialLabel4.Text = "Data Yedek Tipi";
            // 
            // partition
            // 
            this.partition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.partition.HeaderText = "Bölüm";
            this.partition.Name = "partition";
            this.partition.ReadOnly = true;
            this.partition.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // size
            // 
            this.size.FillWeight = 150F;
            this.size.HeaderText = "Bölüm Boyutu";
            this.size.Name = "size";
            this.size.ReadOnly = true;
            this.size.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.size.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // read
            // 
            this.read.FalseValue = "0";
            this.read.HeaderText = "Oku";
            this.read.Name = "read";
            this.read.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.read.TrueValue = "1";
            this.read.Width = 50;
            // 
            // sector_size
            // 
            this.sector_size.HeaderText = "sector_size";
            this.sector_size.Name = "sector_size";
            this.sector_size.Visible = false;
            // 
            // sector_offset
            // 
            this.sector_offset.HeaderText = "sector_offset";
            this.sector_offset.Name = "sector_offset";
            this.sector_offset.Visible = false;
            // 
            // filename
            // 
            this.filename.HeaderText = "filename";
            this.filename.Name = "filename";
            this.filename.Visible = false;
            // 
            // partition_sectors
            // 
            this.partition_sectors.HeaderText = "partition_sectors";
            this.partition_sectors.Name = "partition_sectors";
            this.partition_sectors.Visible = false;
            // 
            // partition_number
            // 
            this.partition_number.HeaderText = "partition_number";
            this.partition_number.Name = "partition_number";
            this.partition_number.Visible = false;
            // 
            // size_kb
            // 
            this.size_kb.HeaderText = "size_kb";
            this.size_kb.Name = "size_kb";
            this.size_kb.Visible = false;
            // 
            // sparse
            // 
            this.sparse.HeaderText = "sparse";
            this.sparse.Name = "sparse";
            this.sparse.Visible = false;
            // 
            // start_byte
            // 
            this.start_byte.HeaderText = "start_byte";
            this.start_byte.Name = "start_byte";
            this.start_byte.Visible = false;
            // 
            // start_sector
            // 
            this.start_sector.HeaderText = "start_sector";
            this.start_sector.Name = "start_sector";
            this.start_sector.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 587);
            this.Controls.Add(this.data_type);
            this.Controls.Add(this.materialLabel4);
            this.Controls.Add(this.backup_type);
            this.Controls.Add(this.materialLabel3);
            this.Controls.Add(this.log);
            this.Controls.Add(this.materialRaisedButton4);
            this.Controls.Add(this.materialRaisedButton2);
            this.Controls.Add(this.materialRaisedButton3);
            this.Controls.Add(this.programmerList);
            this.Controls.Add(this.deviceList);
            this.Controls.Add(this.materialDivider4);
            this.Controls.Add(this.materialDivider3);
            this.Controls.Add(this.materialDivider2);
            this.Controls.Add(this.materialDivider1);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.materialRaisedButton1);
            this.Controls.Add(this.partList);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "One Dumper @caneray-OneTeam";
            ((System.ComponentModel.ISupportInitialize)(this.partList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.log)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView partList;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton1;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialDivider materialDivider1;
        private MaterialSkin.Controls.MaterialDivider materialDivider2;
        private MaterialSkin.Controls.MaterialDivider materialDivider3;
        private MaterialSkin.Controls.MaterialDivider materialDivider4;
        private System.Windows.Forms.ComboBox deviceList;
        private System.Windows.Forms.ListView programmerList;
        private System.Windows.Forms.ColumnHeader programmer;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton3;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton2;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton4;
        private System.Windows.Forms.DataGridView log;
        private System.Windows.Forms.DataGridViewTextBoxColumn unixtime;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn message;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private System.Windows.Forms.ComboBox backup_type;
        private System.Windows.Forms.ComboBox data_type;
        private MaterialSkin.Controls.MaterialLabel materialLabel4;
        private System.Windows.Forms.DataGridViewTextBoxColumn partition;
        private System.Windows.Forms.DataGridViewTextBoxColumn size;
        private System.Windows.Forms.DataGridViewCheckBoxColumn read;
        private System.Windows.Forms.DataGridViewTextBoxColumn sector_size;
        private System.Windows.Forms.DataGridViewTextBoxColumn sector_offset;
        private System.Windows.Forms.DataGridViewTextBoxColumn filename;
        private System.Windows.Forms.DataGridViewTextBoxColumn partition_sectors;
        private System.Windows.Forms.DataGridViewTextBoxColumn partition_number;
        private System.Windows.Forms.DataGridViewTextBoxColumn size_kb;
        private System.Windows.Forms.DataGridViewTextBoxColumn sparse;
        private System.Windows.Forms.DataGridViewTextBoxColumn start_byte;
        private System.Windows.Forms.DataGridViewTextBoxColumn start_sector;
    }
}

