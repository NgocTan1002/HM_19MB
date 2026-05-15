using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HM_19MB_Demo
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            ChartArea chartArea1 = new ChartArea();
            Legend legend1 = new Legend();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            mainLayout = new TableLayoutPanel();
            metadataPanel = new Panel();
            metadataLayout = new TableLayoutPanel();
            metadataFieldsPanel = new Panel();
            lblDeviceName = new Label();
            txtDeviceName = new TextBox();
            lblDeviceCode = new Label();
            txtDeviceCode = new TextBox();
            lblDeviceNumber = new Label();
            txtDeviceNumber = new TextBox();
            lblSealNumber = new Label();
            txtSealNumber = new TextBox();
            lblManufacturer = new Label();
            txtManufacturer = new TextBox();
            lblManufactureYear = new Label();
            txtManufactureYear = new TextBox();
            lblUsingUnit = new Label();
            txtUsingUnit = new TextBox();
            lblMethod = new Label();
            txtMethod = new TextBox();
            lblCalibCondition = new Label();
            lblEnvTemp = new Label();
            txtEnvTemp = new TextBox();
            lblEnvHumidity = new Label();
            txtEnvHumidity = new TextBox();
            lblTechnicalSpecs = new Label();
            txtTechnicalSpecs = new TextBox();
            lblMeasuringDevices = new Label();
            txtMeasuringDevices = new TextBox();
            lblCalibDate = new Label();
            calibDatePanel = new FlowLayoutPanel();
            txtCalibDay = new TextBox();
            lblMonth = new Label();
            txtCalibMonth = new TextBox();
            lblYear = new Label();
            txtCalibYear = new TextBox();
            _calibPanel = new Panel();
            _gridCalibration = new DataGridView();
            calibrationToolbar = new FlowLayoutPanel();
            lblCalibrationTitle = new Label();
            lblKenhCount = new Label();
            numKenhCount = new NumericUpDown();
            _btnAddCalibPoint = new Button();
            _btnDeleteCalibPoint = new Button();
            _lblCalibStatus = new Label();
            _split = new SplitContainer();
            _chart = new Chart();
            _chartToolbar = new Panel();
            BtnShowGuide = new Button();
            _chkHumidity = new CheckBox();
            _chkTemperature = new CheckBox();
            gridLayout = new TableLayoutPanel();
            _lblLastReceived = new Label();
            _grid = new DataGridView();
            bottomBar = new Panel();
            bottomFlow = new FlowLayoutPanel();
            _cmbPort = new ComboBox();
            lblBaudRate = new Label();
            _btnConnect = new Button();
            _btnExport = new Button();
            _btnUncertainty = new Button();
            _lblStatus = new Label();
            mainLayout.SuspendLayout();
            metadataPanel.SuspendLayout();
            metadataLayout.SuspendLayout();
            metadataFieldsPanel.SuspendLayout();
            calibDatePanel.SuspendLayout();
            _calibPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_gridCalibration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numKenhCount).BeginInit();
            calibrationToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_split).BeginInit();
            _split.Panel1.SuspendLayout();
            _split.Panel2.SuspendLayout();
            _split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_chart).BeginInit();
            _chartToolbar.SuspendLayout();
            gridLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_grid).BeginInit();
            bottomBar.SuspendLayout();
            bottomFlow.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(metadataPanel, 0, 0);
            mainLayout.Controls.Add(_split, 0, 1);
            mainLayout.Controls.Add(bottomBar, 0, 2);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Margin = new Padding(3, 4, 3, 4);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(7, 8, 7, 8);
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle());
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 59F));
            mainLayout.Size = new Size(1600, 1175);
            mainLayout.TabIndex = 0;
            // 
            // metadataPanel
            // 
            metadataPanel.AutoScroll = true;
            metadataPanel.BackColor = Color.White;
            metadataPanel.Controls.Add(metadataLayout);
            metadataPanel.Dock = DockStyle.Top;
            metadataPanel.Location = new Point(10, 12);
            metadataPanel.Margin = new Padding(3, 4, 3, 4);
            metadataPanel.Name = "metadataPanel";
            metadataPanel.Padding = new Padding(10);
            metadataPanel.Size = new Size(1580, 320);
            metadataPanel.TabIndex = 0;
            // 
            // metadataLayout
            // 
            metadataLayout.ColumnCount = 2;
            metadataLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 840F));
            metadataLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            metadataLayout.Controls.Add(metadataFieldsPanel, 0, 0);
            metadataLayout.Controls.Add(_calibPanel, 1, 0);
            metadataLayout.Dock = DockStyle.Fill;
            metadataLayout.Location = new Point(10, 10);
            metadataLayout.Name = "metadataLayout";
            metadataLayout.RowCount = 1;
            metadataLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            metadataLayout.Size = new Size(1560, 300);
            metadataLayout.TabIndex = 27;
            // 
            // metadataFieldsPanel
            // 
            metadataFieldsPanel.BackColor = Color.White;
            metadataFieldsPanel.Controls.Add(lblDeviceName);
            metadataFieldsPanel.Controls.Add(txtDeviceName);
            metadataFieldsPanel.Controls.Add(lblDeviceCode);
            metadataFieldsPanel.Controls.Add(txtDeviceCode);
            metadataFieldsPanel.Controls.Add(lblDeviceNumber);
            metadataFieldsPanel.Controls.Add(txtDeviceNumber);
            metadataFieldsPanel.Controls.Add(lblSealNumber);
            metadataFieldsPanel.Controls.Add(txtSealNumber);
            metadataFieldsPanel.Controls.Add(lblManufacturer);
            metadataFieldsPanel.Controls.Add(txtManufacturer);
            metadataFieldsPanel.Controls.Add(lblManufactureYear);
            metadataFieldsPanel.Controls.Add(txtManufactureYear);
            metadataFieldsPanel.Controls.Add(lblUsingUnit);
            metadataFieldsPanel.Controls.Add(txtUsingUnit);
            metadataFieldsPanel.Controls.Add(lblMethod);
            metadataFieldsPanel.Controls.Add(txtMethod);
            metadataFieldsPanel.Controls.Add(lblCalibCondition);
            metadataFieldsPanel.Controls.Add(lblEnvTemp);
            metadataFieldsPanel.Controls.Add(txtEnvTemp);
            metadataFieldsPanel.Controls.Add(lblEnvHumidity);
            metadataFieldsPanel.Controls.Add(txtEnvHumidity);
            metadataFieldsPanel.Controls.Add(lblTechnicalSpecs);
            metadataFieldsPanel.Controls.Add(txtTechnicalSpecs);
            metadataFieldsPanel.Controls.Add(lblMeasuringDevices);
            metadataFieldsPanel.Controls.Add(txtMeasuringDevices);
            metadataFieldsPanel.Controls.Add(lblCalibDate);
            metadataFieldsPanel.Controls.Add(calibDatePanel);
            metadataFieldsPanel.Dock = DockStyle.Fill;
            metadataFieldsPanel.Location = new Point(3, 3);
            metadataFieldsPanel.Name = "metadataFieldsPanel";
            metadataFieldsPanel.Size = new Size(834, 294);
            metadataFieldsPanel.TabIndex = 0;
            // 
            // lblDeviceName
            // 
            lblDeviceName.AutoSize = true;
            lblDeviceName.Location = new Point(10, 15);
            lblDeviceName.Name = "lblDeviceName";
            lblDeviceName.Size = new Size(142, 20);
            lblDeviceName.TabIndex = 0;
            lblDeviceName.Text = "Tên phương tiện đo:";
            lblDeviceName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDeviceName
            // 
            txtDeviceName.Location = new Point(160, 12);
            txtDeviceName.Name = "txtDeviceName";
            txtDeviceName.Size = new Size(660, 27);
            txtDeviceName.TabIndex = 1;
            txtDeviceName.Text = "Tủ nhiệt";
            // 
            // lblDeviceCode
            // 
            lblDeviceCode.AutoSize = true;
            lblDeviceCode.Location = new Point(10, 50);
            lblDeviceCode.Name = "lblDeviceCode";
            lblDeviceCode.Size = new Size(59, 20);
            lblDeviceCode.TabIndex = 2;
            lblDeviceCode.Text = "Ký hiệu:";
            lblDeviceCode.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDeviceCode
            // 
            txtDeviceCode.Location = new Point(160, 47);
            txtDeviceCode.Name = "txtDeviceCode";
            txtDeviceCode.Size = new Size(150, 27);
            txtDeviceCode.TabIndex = 3;
            // 
            // lblDeviceNumber
            // 
            lblDeviceNumber.AutoSize = true;
            lblDeviceNumber.Location = new Point(330, 50);
            lblDeviceNumber.Name = "lblDeviceNumber";
            lblDeviceNumber.Size = new Size(61, 20);
            lblDeviceNumber.TabIndex = 4;
            lblDeviceNumber.Text = "Số hiệu:";
            lblDeviceNumber.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDeviceNumber
            // 
            txtDeviceNumber.Location = new Point(400, 47);
            txtDeviceNumber.Name = "txtDeviceNumber";
            txtDeviceNumber.Size = new Size(150, 27);
            txtDeviceNumber.TabIndex = 5;
            // 
            // lblSealNumber
            // 
            lblSealNumber.AutoSize = true;
            lblSealNumber.Location = new Point(570, 50);
            lblSealNumber.Name = "lblSealNumber";
            lblSealNumber.Size = new Size(134, 20);
            lblSealNumber.TabIndex = 6;
            lblSealNumber.Text = "Số tem hiệu chuẩn:";
            lblSealNumber.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSealNumber
            // 
            txtSealNumber.Location = new Point(710, 47);
            txtSealNumber.Name = "txtSealNumber";
            txtSealNumber.Size = new Size(110, 27);
            txtSealNumber.TabIndex = 7;
            // 
            // lblManufacturer
            // 
            lblManufacturer.AutoSize = true;
            lblManufacturer.Location = new Point(10, 85);
            lblManufacturer.Name = "lblManufacturer";
            lblManufacturer.Size = new Size(94, 20);
            lblManufacturer.TabIndex = 8;
            lblManufacturer.Text = "Nơi sản xuất:";
            lblManufacturer.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtManufacturer
            // 
            txtManufacturer.Location = new Point(160, 82);
            txtManufacturer.Name = "txtManufacturer";
            txtManufacturer.Size = new Size(300, 27);
            txtManufacturer.TabIndex = 9;
            // 
            // lblManufactureYear
            // 
            lblManufactureYear.AutoSize = true;
            lblManufactureYear.Location = new Point(480, 85);
            lblManufactureYear.Name = "lblManufactureYear";
            lblManufactureYear.Size = new Size(102, 20);
            lblManufactureYear.TabIndex = 10;
            lblManufactureYear.Text = "Năm sản xuất:";
            lblManufactureYear.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtManufactureYear
            // 
            txtManufactureYear.Location = new Point(590, 82);
            txtManufactureYear.Name = "txtManufactureYear";
            txtManufactureYear.Size = new Size(100, 27);
            txtManufactureYear.TabIndex = 11;
            // 
            // lblUsingUnit
            // 
            lblUsingUnit.AutoSize = true;
            lblUsingUnit.Location = new Point(10, 120);
            lblUsingUnit.Name = "lblUsingUnit";
            lblUsingUnit.Size = new Size(112, 20);
            lblUsingUnit.TabIndex = 12;
            lblUsingUnit.Text = "Đơn vị sử dụng:";
            lblUsingUnit.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtUsingUnit
            // 
            txtUsingUnit.Location = new Point(160, 117);
            txtUsingUnit.Name = "txtUsingUnit";
            txtUsingUnit.Size = new Size(324, 27);
            txtUsingUnit.TabIndex = 13;
            // 
            // lblMethod
            // 
            lblMethod.AutoSize = true;
            lblMethod.Location = new Point(492, 120);
            lblMethod.Name = "lblMethod";
            lblMethod.Size = new Size(166, 20);
            lblMethod.TabIndex = 14;
            lblMethod.Text = "Phương pháp thực hiện:";
            lblMethod.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtMethod
            // 
            txtMethod.Location = new Point(662, 117);
            txtMethod.Name = "txtMethod";
            txtMethod.Size = new Size(158, 27);
            txtMethod.TabIndex = 15;
            txtMethod.Text = "QTHC 1.013 : 2019";
            // 
            // lblCalibCondition
            // 
            lblCalibCondition.AutoSize = true;
            lblCalibCondition.Location = new Point(10, 158);
            lblCalibCondition.Name = "lblCalibCondition";
            lblCalibCondition.Size = new Size(149, 20);
            lblCalibCondition.TabIndex = 16;
            lblCalibCondition.Text = "Điều kiện hiệu chuẩn:";
            lblCalibCondition.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEnvTemp
            // 
            lblEnvTemp.AutoSize = true;
            lblEnvTemp.Location = new Point(180, 158);
            lblEnvTemp.Name = "lblEnvTemp";
            lblEnvTemp.Size = new Size(149, 20);
            lblEnvTemp.TabIndex = 17;
            lblEnvTemp.Text = "Nhiệt độ môi trường:";
            lblEnvTemp.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtEnvTemp
            // 
            txtEnvTemp.Location = new Point(335, 155);
            txtEnvTemp.Name = "txtEnvTemp";
            txtEnvTemp.Size = new Size(149, 27);
            txtEnvTemp.TabIndex = 18;
            // 
            // lblEnvHumidity
            // 
            lblEnvHumidity.AutoSize = true;
            lblEnvHumidity.Location = new Point(500, 158);
            lblEnvHumidity.Name = "lblEnvHumidity";
            lblEnvHumidity.Size = new Size(127, 20);
            lblEnvHumidity.TabIndex = 19;
            lblEnvHumidity.Text = "Độ ẩm tương đối:";
            lblEnvHumidity.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtEnvHumidity
            // 
            txtEnvHumidity.Location = new Point(633, 155);
            txtEnvHumidity.Name = "txtEnvHumidity";
            txtEnvHumidity.Size = new Size(187, 27);
            txtEnvHumidity.TabIndex = 20;
            // 
            // lblTechnicalSpecs
            // 
            lblTechnicalSpecs.AutoSize = true;
            lblTechnicalSpecs.Location = new Point(10, 193);
            lblTechnicalSpecs.Name = "lblTechnicalSpecs";
            lblTechnicalSpecs.Size = new Size(123, 20);
            lblTechnicalSpecs.TabIndex = 21;
            lblTechnicalSpecs.Text = "Đặc tính kỹ thuật:";
            lblTechnicalSpecs.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtTechnicalSpecs
            // 
            txtTechnicalSpecs.Location = new Point(160, 190);
            txtTechnicalSpecs.Name = "txtTechnicalSpecs";
            txtTechnicalSpecs.Size = new Size(660, 27);
            txtTechnicalSpecs.TabIndex = 22;
            // 
            // lblMeasuringDevices
            // 
            lblMeasuringDevices.AutoSize = true;
            lblMeasuringDevices.Location = new Point(10, 228);
            lblMeasuringDevices.Name = "lblMeasuringDevices";
            lblMeasuringDevices.Size = new Size(200, 20);
            lblMeasuringDevices.TabIndex = 23;
            lblMeasuringDevices.Text = "Các phương tiện đo sử dụng:";
            lblMeasuringDevices.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtMeasuringDevices
            // 
            txtMeasuringDevices.Location = new Point(220, 225);
            txtMeasuringDevices.Name = "txtMeasuringDevices";
            txtMeasuringDevices.Size = new Size(600, 27);
            txtMeasuringDevices.TabIndex = 24;
            // 
            // lblCalibDate
            // 
            lblCalibDate.AutoSize = true;
            lblCalibDate.Location = new Point(10, 265);
            lblCalibDate.Name = "lblCalibDate";
            lblCalibDate.Size = new Size(204, 20);
            lblCalibDate.TabIndex = 25;
            lblCalibDate.Text = "Đã tiến hành hiệu chuẩn ngày";
            lblCalibDate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // calibDatePanel
            // 
            calibDatePanel.AutoSize = true;
            calibDatePanel.Controls.Add(txtCalibDay);
            calibDatePanel.Controls.Add(lblMonth);
            calibDatePanel.Controls.Add(txtCalibMonth);
            calibDatePanel.Controls.Add(lblYear);
            calibDatePanel.Controls.Add(txtCalibYear);
            calibDatePanel.Location = new Point(220, 261);
            calibDatePanel.Name = "calibDatePanel";
            calibDatePanel.Size = new Size(240, 33);
            calibDatePanel.TabIndex = 26;
            // 
            // txtCalibDay
            // 
            txtCalibDay.Location = new Point(3, 3);
            txtCalibDay.Name = "txtCalibDay";
            txtCalibDay.Size = new Size(35, 27);
            txtCalibDay.TabIndex = 0;
            // 
            // lblMonth
            // 
            lblMonth.AutoSize = true;
            lblMonth.Location = new Point(45, 3);
            lblMonth.Margin = new Padding(4, 3, 2, 0);
            lblMonth.Name = "lblMonth";
            lblMonth.Size = new Size(47, 20);
            lblMonth.TabIndex = 1;
            lblMonth.Text = "tháng";
            lblMonth.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtCalibMonth
            // 
            txtCalibMonth.Location = new Point(97, 3);
            txtCalibMonth.Name = "txtCalibMonth";
            txtCalibMonth.Size = new Size(35, 27);
            txtCalibMonth.TabIndex = 2;
            // 
            // lblYear
            // 
            lblYear.AutoSize = true;
            lblYear.Location = new Point(139, 3);
            lblYear.Margin = new Padding(4, 3, 2, 0);
            lblYear.Name = "lblYear";
            lblYear.Size = new Size(38, 20);
            lblYear.TabIndex = 3;
            lblYear.Text = "năm";
            lblYear.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtCalibYear
            // 
            txtCalibYear.Location = new Point(182, 3);
            txtCalibYear.Name = "txtCalibYear";
            txtCalibYear.Size = new Size(55, 27);
            txtCalibYear.TabIndex = 4;
            // 
            // _calibPanel
            // 
            _calibPanel.BackColor = Color.White;
            _calibPanel.Controls.Add(_gridCalibration);
            _calibPanel.Controls.Add(calibrationToolbar);
            _calibPanel.Dock = DockStyle.Fill;
            _calibPanel.Location = new Point(843, 3);
            _calibPanel.Name = "_calibPanel";
            _calibPanel.Padding = new Padding(6, 4, 6, 4);
            _calibPanel.Size = new Size(714, 294);
            _calibPanel.TabIndex = 1;
            // 
            // _gridCalibration
            // 
            _gridCalibration.AllowUserToAddRows = false;
            _gridCalibration.AllowUserToDeleteRows = false;

            _gridCalibration.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _gridCalibration.BackgroundColor = Color.White;
            _gridCalibration.BorderStyle = BorderStyle.FixedSingle;

            _gridCalibration.Dock = DockStyle.Fill;
            _gridCalibration.EnableHeadersVisualStyles = true;

            _gridCalibration.Font = new Font("Segoe UI", 9F);
            _gridCalibration.Location = new Point(6, 40);
            _gridCalibration.MultiSelect = false;
            _gridCalibration.Name = "_gridCalibration";
            _gridCalibration.ReadOnly = true;

            _gridCalibration.RowHeadersVisible = false;
            _gridCalibration.RowHeadersWidth = 51;

            _gridCalibration.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _gridCalibration.Size = new Size(702, 235);
            _gridCalibration.TabIndex = 1;

            // Header cơ bản
            _gridCalibration.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = SystemColors.Control,
                ForeColor = SystemColors.ControlText,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                SelectionBackColor = SystemColors.Control,
                SelectionForeColor = SystemColors.ControlText,
                WrapMode = DataGridViewTriState.True
            };

            _gridCalibration.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Dòng thường
            _gridCalibration.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = SystemColors.Highlight,
                SelectionForeColor = SystemColors.HighlightText
            };

            // Dòng xen kẽ
            _gridCalibration.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White
            };

            // Màu lưới cơ bản
            _gridCalibration.GridColor = SystemColors.ControlDark;
            // 
            // calibrationToolbar
            // 
            calibrationToolbar.BackColor = Color.White;
            calibrationToolbar.Controls.Add(lblCalibrationTitle);
            calibrationToolbar.Controls.Add(lblKenhCount);
            calibrationToolbar.Controls.Add(numKenhCount);
            calibrationToolbar.Controls.Add(_btnAddCalibPoint);
            calibrationToolbar.Controls.Add(_btnDeleteCalibPoint);
            calibrationToolbar.Controls.Add(_lblCalibStatus);
            calibrationToolbar.Dock = DockStyle.Top;
            calibrationToolbar.Location = new Point(6, 4);
            calibrationToolbar.Name = "calibrationToolbar";
            calibrationToolbar.Padding = new Padding(0, 2, 0, 2);
            calibrationToolbar.Size = new Size(702, 36);
            calibrationToolbar.TabIndex = 0;
            calibrationToolbar.WrapContents = false;
            // 
            // lblCalibrationTitle
            // 
            lblCalibrationTitle.AutoSize = true;
            lblCalibrationTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCalibrationTitle.ForeColor = SystemColors.ActiveCaptionText;
            lblCalibrationTitle.Location = new Point(4, 8);
            lblCalibrationTitle.Margin = new Padding(4, 6, 12, 0);
            lblCalibrationTitle.Name = "lblCalibrationTitle";
            lblCalibrationTitle.Size = new Size(157, 21);
            lblCalibrationTitle.TabIndex = 0;
            lblCalibrationTitle.Text = "Kết quả hiệu chuẩn";
            // 
            // lblKenhCount
            // 
            lblKenhCount.AutoSize = true;
            lblKenhCount.Location = new Point(173, 10);
            lblKenhCount.Margin = new Padding(0, 8, 4, 0);
            lblKenhCount.Name = "lblKenhCount";
            lblKenhCount.Size = new Size(85, 20);
            lblKenhCount.TabIndex = 1;
            lblKenhCount.Text = "Số kênh (k):";
            // 
            // numKenhCount
            // 
            numKenhCount.Location = new Point(262, 6);
            numKenhCount.Margin = new Padding(0, 4, 12, 0);
            numKenhCount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numKenhCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numKenhCount.Name = "numKenhCount";
            numKenhCount.Size = new Size(60, 27);
            numKenhCount.TabIndex = 2;
            numKenhCount.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // _btnAddCalibPoint
            // 
            _btnAddCalibPoint.BackColor = Color.White;
            _btnAddCalibPoint.Cursor = Cursors.Hand;
            _btnAddCalibPoint.FlatAppearance.BorderColor = Color.Silver;
            _btnAddCalibPoint.FlatStyle = FlatStyle.Flat;
            _btnAddCalibPoint.ForeColor = Color.Black;
            _btnAddCalibPoint.Location = new Point(334, 5);
            _btnAddCalibPoint.Margin = new Padding(0, 3, 6, 0);
            _btnAddCalibPoint.Name = "_btnAddCalibPoint";
            _btnAddCalibPoint.Size = new Size(160, 28);
            _btnAddCalibPoint.TabIndex = 3;
            _btnAddCalibPoint.Text = "Thêm điểm kiểm tra";
            _btnAddCalibPoint.UseVisualStyleBackColor = false;
            // 
            // _btnDeleteCalibPoint
            // 
            _btnDeleteCalibPoint.BackColor = Color.White;
            _btnDeleteCalibPoint.Cursor = Cursors.Hand;
            _btnDeleteCalibPoint.Enabled = false;
            _btnDeleteCalibPoint.FlatAppearance.BorderColor = Color.Silver;
            _btnDeleteCalibPoint.FlatStyle = FlatStyle.Flat;
            _btnDeleteCalibPoint.ForeColor = Color.Black;
            _btnDeleteCalibPoint.Location = new Point(500, 5);
            _btnDeleteCalibPoint.Margin = new Padding(0, 3, 6, 0);
            _btnDeleteCalibPoint.Name = "_btnDeleteCalibPoint";
            _btnDeleteCalibPoint.Size = new Size(100, 28);
            _btnDeleteCalibPoint.TabIndex = 4;
            _btnDeleteCalibPoint.Text = "Xóa dòng";
            _btnDeleteCalibPoint.UseVisualStyleBackColor = false;
            // 
            // _lblCalibStatus
            // 
            _lblCalibStatus.AutoSize = true;
            _lblCalibStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            _lblCalibStatus.ForeColor = Color.DimGray;
            _lblCalibStatus.Location = new Point(614, 10);
            _lblCalibStatus.Margin = new Padding(8, 8, 0, 0);
            _lblCalibStatus.Name = "_lblCalibStatus";
            _lblCalibStatus.Size = new Size(111, 20);
            _lblCalibStatus.TabIndex = 5;
            _lblCalibStatus.Text = "Chưa có dữ liệu";
            // 
            // _split
            // 
            _split.Dock = DockStyle.Fill;
            _split.Location = new Point(10, 340);
            _split.Margin = new Padding(3, 4, 3, 4);
            _split.Name = "_split";
            // 
            // _split.Panel1
            // 
            _split.Panel1.Controls.Add(_chart);
            _split.Panel1.Controls.Add(_chartToolbar);
            // 
            // _split.Panel2
            // 
            _split.Panel2.Controls.Add(gridLayout);
            _split.Size = new Size(1580, 764);
            _split.SplitterDistance = 790;
            _split.SplitterWidth = 5;
            _split.TabIndex = 1;
            // 
            // _chart
            // 
            chartArea1.AxisX.LabelStyle.Angle = -30;
            chartArea1.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea1.AxisX.Title = "Thời gian";
            chartArea1.AxisY.Interval = 1D;
            chartArea1.AxisY.IsStartedFromZero = false;
            chartArea1.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea1.AxisY.Maximum = 30D;
            chartArea1.AxisY.Minimum = 20D;
            chartArea1.AxisY.Title = "Nhiệt độ (°C)";
            chartArea1.AxisY2.Interval = 1D;
            chartArea1.AxisY2.LabelStyle.ForeColor = Color.SteelBlue;
            chartArea1.AxisY2.MajorGrid.Enabled = false;
            chartArea1.AxisY2.Maximum = 65D;
            chartArea1.AxisY2.Minimum = 55D;
            chartArea1.AxisY2.Title = "Độ ẩm (%)";
            chartArea1.AxisY2.TitleForeColor = Color.SteelBlue;
            chartArea1.Name = "MainArea";
            _chart.ChartAreas.Add(chartArea1);
            _chart.Dock = DockStyle.Fill;
            legend1.Docking = Docking.Bottom;
            legend1.Name = "MainLegend";
            _chart.Legends.Add(legend1);
            _chart.Location = new Point(0, 38);
            _chart.Margin = new Padding(3, 4, 3, 4);
            _chart.Name = "_chart";
            _chart.Size = new Size(790, 726);
            _chart.TabIndex = 0;
            _chart.Text = "chart1";
            // 
            // _chartToolbar
            // 
            _chartToolbar.BackColor = Color.FromArgb(245, 245, 250);
            _chartToolbar.Controls.Add(BtnShowGuide);
            _chartToolbar.Controls.Add(_chkHumidity);
            _chartToolbar.Controls.Add(_chkTemperature);
            _chartToolbar.Dock = DockStyle.Top;
            _chartToolbar.Location = new Point(0, 0);
            _chartToolbar.Name = "_chartToolbar";
            _chartToolbar.Padding = new Padding(5, 4, 5, 4);
            _chartToolbar.Size = new Size(790, 38);
            _chartToolbar.TabIndex = 1;
            // 
            // BtnShowGuide
            // 
            BtnShowGuide.Location = new Point(603, 5);
            BtnShowGuide.Name = "BtnShowGuide";
            BtnShowGuide.Size = new Size(179, 29);
            BtnShowGuide.TabIndex = 1;
            BtnShowGuide.Text = "Hướng dẫn đặt đầu đo";
            BtnShowGuide.UseVisualStyleBackColor = true;
            BtnShowGuide.Click += BtnShowGuide_Click;
            // 
            // _chkHumidity
            // 
            _chkHumidity.AutoSize = true;
            _chkHumidity.Checked = true;
            _chkHumidity.CheckState = CheckState.Checked;
            _chkHumidity.Cursor = Cursors.Hand;
            _chkHumidity.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _chkHumidity.ForeColor = Color.Black;
            _chkHumidity.Location = new Point(182, 8);
            _chkHumidity.Name = "_chkHumidity";
            _chkHumidity.Size = new Size(172, 24);
            _chkHumidity.TabIndex = 2;
            _chkHumidity.Text = "Thiết bị đo nhiệt ẩm";
            _chkHumidity.CheckedChanged += _chkDisplayFilter_CheckedChanged;
            // 
            // _chkTemperature
            // 
            _chkTemperature.AutoSize = true;
            _chkTemperature.Checked = true;
            _chkTemperature.CheckState = CheckState.Checked;
            _chkTemperature.Cursor = Cursors.Hand;
            _chkTemperature.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _chkTemperature.ForeColor = Color.Black;
            _chkTemperature.Location = new Point(8, 8);
            _chkTemperature.Name = "_chkTemperature";
            _chkTemperature.Size = new Size(168, 24);
            _chkTemperature.TabIndex = 0;
            _chkTemperature.Text = "Thiết bị đo nhiệt độ";
            _chkTemperature.CheckedChanged += _chkDisplayFilter_CheckedChanged;
            // 
            // gridLayout
            // 
            gridLayout.ColumnCount = 1;
            gridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            gridLayout.Controls.Add(_lblLastReceived, 0, 1);
            gridLayout.Controls.Add(_grid, 0, 0);
            gridLayout.Dock = DockStyle.Fill;
            gridLayout.Location = new Point(0, 0);
            gridLayout.Margin = new Padding(3, 4, 3, 4);
            gridLayout.Name = "gridLayout";
            gridLayout.RowCount = 2;
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            gridLayout.Size = new Size(785, 764);
            gridLayout.TabIndex = 0;
            // 
            // _lblLastReceived
            // 
            _lblLastReceived.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            _lblLastReceived.ForeColor = Color.Gray;
            _lblLastReceived.Location = new Point(3, 735);
            _lblLastReceived.Name = "_lblLastReceived";
            _lblLastReceived.Size = new Size(778, 29);
            _lblLastReceived.TabIndex = 1;
            _lblLastReceived.Text = "Chưa nhận dữ liệu";
            _lblLastReceived.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _grid
            // 
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            _grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _grid.Dock = DockStyle.Fill;
            _grid.Location = new Point(3, 4);
            _grid.Margin = new Padding(3, 4, 3, 4);
            _grid.Name = "_grid";
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.RowHeadersWidth = 51;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.Size = new Size(779, 727);
            _grid.TabIndex = 0;
            // 
            // bottomBar
            // 
            bottomBar.BackColor = Color.White;
            bottomBar.Controls.Add(bottomFlow);
            bottomBar.Dock = DockStyle.Fill;
            bottomBar.Location = new Point(10, 1112);
            bottomBar.Margin = new Padding(3, 4, 3, 4);
            bottomBar.Name = "bottomBar";
            bottomBar.Padding = new Padding(5);
            bottomBar.Size = new Size(1580, 51);
            bottomBar.TabIndex = 2;
            // 
            // bottomFlow
            // 
            bottomFlow.BackColor = Color.White;
            bottomFlow.Controls.Add(_cmbPort);
            bottomFlow.Controls.Add(lblBaudRate);
            bottomFlow.Controls.Add(_btnConnect);
            bottomFlow.Controls.Add(_btnExport);
            bottomFlow.Controls.Add(_btnUncertainty);
            bottomFlow.Controls.Add(_lblStatus);
            bottomFlow.Dock = DockStyle.Fill;
            bottomFlow.Location = new Point(5, 5);
            bottomFlow.Margin = new Padding(3, 4, 3, 4);
            bottomFlow.Name = "bottomFlow";
            bottomFlow.Size = new Size(1570, 41);
            bottomFlow.TabIndex = 0;
            bottomFlow.WrapContents = false;
            // 
            // _cmbPort
            // 
            _cmbPort.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbPort.FormattingEnabled = true;
            _cmbPort.Location = new Point(3, 4);
            _cmbPort.Margin = new Padding(3, 4, 3, 4);
            _cmbPort.Name = "_cmbPort";
            _cmbPort.Size = new Size(125, 28);
            _cmbPort.TabIndex = 0;
            // 
            // lblBaudRate
            // 
            lblBaudRate.AutoSize = true;
            lblBaudRate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblBaudRate.Location = new Point(142, 11);
            lblBaudRate.Margin = new Padding(11, 11, 11, 0);
            lblBaudRate.Name = "lblBaudRate";
            lblBaudRate.Size = new Size(137, 20);
            lblBaudRate.TabIndex = 1;
            lblBaudRate.Text = "BAUD RATE: 9600";
            lblBaudRate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _btnConnect
            // 
            _btnConnect.Location = new Point(293, 4);
            _btnConnect.Margin = new Padding(3, 4, 3, 4);
            _btnConnect.Name = "_btnConnect";
            _btnConnect.Size = new Size(130, 37);
            _btnConnect.TabIndex = 2;
            _btnConnect.Text = "Kết nối";
            _btnConnect.UseVisualStyleBackColor = true;
            // 
            // _btnExport
            // 
            _btnExport.Location = new Point(429, 4);
            _btnExport.Margin = new Padding(3, 4, 3, 4);
            _btnExport.Name = "_btnExport";
            _btnExport.Size = new Size(193, 37);
            _btnExport.TabIndex = 4;
            _btnExport.Text = "Xuất biên bản hiệu chuẩn";
            _btnExport.UseVisualStyleBackColor = true;
            // 
            // _btnUncertainty
            // 
            _btnUncertainty.BackColor = Color.White;
            _btnUncertainty.Location = new Point(628, 4);
            _btnUncertainty.Margin = new Padding(3, 4, 3, 4);
            _btnUncertainty.Name = "_btnUncertainty";
            _btnUncertainty.Size = new Size(315, 37);
            _btnUncertainty.TabIndex = 6;
            _btnUncertainty.Text = "Tính số hiệu chính và độ không đảm bảo đo";
            _btnUncertainty.UseVisualStyleBackColor = false;
            // 
            // _lblStatus
            // 
            _lblStatus.AutoSize = true;
            _lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblStatus.ForeColor = Color.DarkRed;
            _lblStatus.Location = new Point(964, 11);
            _lblStatus.Margin = new Padding(18, 11, 0, 0);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(97, 20);
            _lblStatus.TabIndex = 5;
            _lblStatus.Text = "Chưa kết nối";
            _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1600, 1175);
            Controls.Add(mainLayout);
            Font = new Font("Segoe UI", 9F);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1255, 984);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hệ thống hiệu chuẩn nhiệt độ - độ ẩm";
            WindowState = FormWindowState.Maximized;
            mainLayout.ResumeLayout(false);
            metadataPanel.ResumeLayout(false);
            metadataLayout.ResumeLayout(false);
            metadataFieldsPanel.ResumeLayout(false);
            metadataFieldsPanel.PerformLayout();
            calibDatePanel.ResumeLayout(false);
            calibDatePanel.PerformLayout();
            _calibPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_gridCalibration).EndInit();
            ((System.ComponentModel.ISupportInitialize)numKenhCount).EndInit();
            calibrationToolbar.ResumeLayout(false);
            calibrationToolbar.PerformLayout();
            _split.Panel1.ResumeLayout(false);
            _split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_split).EndInit();
            _split.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_chart).EndInit();
            _chartToolbar.ResumeLayout(false);
            _chartToolbar.PerformLayout();
            gridLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
            bottomBar.ResumeLayout(false);
            bottomFlow.ResumeLayout(false);
            bottomFlow.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        // Post-initialization methods (called after InitializeComponent)
        // ─────────────────────────────────────────────────────────────────



        private void InitializeChartSeries()
        {
            // ── Series NHIỆT ĐỘ (trục Y trái) ───────────────────────────────────
            for (int i = 0; i < 10; i++)
            {
                _chart.Series.Add(new Series($"Đầu đo {i + 1}")
                {
                    ChartType = SeriesChartType.Line,
                    Color = ProbeColors[i],
                    BorderWidth = 2,
                    ChartArea = "MainArea",
                    Legend = "MainLegend",
                    IsXValueIndexed = true,
                    YAxisType = AxisType.Primary
                });
            }
            _chart.Series.Add(new Series("Trung bình")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Black,
                BorderWidth = 3,
                BorderDashStyle = ChartDashStyle.Dash,
                ChartArea = "MainArea",
                Legend = "MainLegend",
                IsXValueIndexed = true,
                YAxisType = AxisType.Primary
            });

            // ── Series ĐỘ ẨM (trục Y2 phải) ─────────────────────────────────────
            for (int i = 0; i < 10; i++)
            {
                _chart.Series.Add(new Series($"ĐA Đầu đo {i + 1}")
                {
                    ChartType = SeriesChartType.Line,
                    Color = ChangeColorAlpha(ProbeColors[i], 140),
                    BorderWidth = 1,
                    BorderDashStyle = ChartDashStyle.Dot,
                    ChartArea = "MainArea",
                    Legend = "MainLegend",
                    IsXValueIndexed = true,
                    YAxisType = AxisType.Secondary,
                    IsVisibleInLegend = false
                });
            }
            _chart.Series.Add(new Series("TB Độ ẩm")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.SteelBlue,
                BorderWidth = 3,
                BorderDashStyle = ChartDashStyle.DashDot,
                ChartArea = "MainArea",
                Legend = "MainLegend",
                IsXValueIndexed = true,
                YAxisType = AxisType.Secondary
            });

            // ── Wire event — _chartToolbar và _chkShowHumidity đã có trong Designer ──
            //_chkShowHumidity.CheckedChanged += ChkShowHumidity_CheckedChanged;
        }
        // Helper: làm mờ màu để series độ ẩm không lấn át nhiệt độ
        private static Color ChangeColorAlpha(Color c, int alpha)
            => Color.FromArgb(alpha, c.R, c.G, c.B);

        private void InitializeGridContent()
        {
            _grid.Columns.Add("STT", "STT");
            _grid.Columns.Add("DauDo", "Đầu đo");
            _grid.Columns.Add("NhietDo", "Nhiệt độ (°C)");
            _grid.Columns.Add("DoAm", "Độ ẩm (%)");
            _grid.Columns.Add("ThoiGian", "Thời gian");

            // Vô hiệu hóa sắp xếp để giữ nguyên thứ tự các dòng
            foreach (DataGridViewColumn col in _grid.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            _grid.Columns["STT"].FillWeight = 30;
            _grid.Columns["DauDo"].FillWeight = 70;
            _grid.Columns["NhietDo"].FillWeight = 80;
            _grid.Columns["DoAm"].FillWeight = 70;
            _grid.Columns["ThoiGian"].FillWeight = 100;

            for (int i = 1; i <= 10; i++)
                _grid.Rows.Add(i.ToString(), $"Đầu đo {i}", "---", "---", "---");
            _grid.Rows.Add("11", "Trung bình", "---", "---", "---");
            _grid.Rows.Add("12", "Độ đồng đều", "---", "---", "---");
            _grid.Rows.Add("13", "Độ ổn định", "---", "---", "---");

            // Đăng ký sự kiện để điều chỉnh chiều cao các dòng khi form được hiển thị
            _grid.DataBindingComplete += (s, e) => AdjustRowHeights();
        }

        private void AdjustRowHeights()
        {
            if (_grid.Rows.Count == 0) return;

            // Tính toán chiều cao có sẵn (trừ đi header)
            int availableHeight = _grid.ClientSize.Height - _grid.ColumnHeadersHeight;
            int rowCount = _grid.Rows.Count;

            if (rowCount > 0 && availableHeight > 0)
            {
                int rowHeight = availableHeight / rowCount;
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    row.Height = rowHeight;
                }
            }
        }

        // ── Control field declarations ────────────────────────────────────
        private TableLayoutPanel mainLayout;
        private Panel metadataPanel;
        private TableLayoutPanel metadataLayout;
        private Panel metadataFieldsPanel;
        private Panel bottomBar;
        private FlowLayoutPanel bottomFlow;
        private Label lblBaudRate;
        private TableLayoutPanel gridLayout;

        // Metadata labels
        private Label lblDeviceName;
        private Label lblDeviceCode;
        private Label lblDeviceNumber;
        private Label lblSealNumber;
        private Label lblManufacturer;
        private Label lblManufactureYear;
        private Label lblUsingUnit;
        private Label lblMethod;
        private Label lblCalibCondition;
        private Label lblEnvTemp;
        private Label lblEnvHumidity;
        private Label lblTechnicalSpecs;
        private Label lblMeasuringDevices;
        private Label lblCalibDate;
        private FlowLayoutPanel calibDatePanel;
        private Label lblMonth;
        private Label lblYear;


        private TextBox txtDeviceName, txtDeviceCode, txtDeviceNumber, txtSealNumber;
        private TextBox txtManufacturer, txtManufactureYear, txtUsingUnit, txtMethod;
        private TextBox txtEnvTemp, txtEnvHumidity, txtTechnicalSpecs, txtMeasuringDevices;
        private TextBox txtCalibDay, txtCalibMonth, txtCalibYear;

        private Panel _calibPanel;
        private FlowLayoutPanel calibrationToolbar;
        private Label lblCalibrationTitle;
        private Label lblKenhCount;
        private NumericUpDown numKenhCount;
        private Button _btnAddCalibPoint;
        private Button _btnDeleteCalibPoint;
        private Label _lblCalibStatus;
        private DataGridView _gridCalibration;

        private Chart _chart;
        private DataGridView _grid;
        private Label _lblLastReceived;

        private SplitContainer _split;
        private ComboBox _cmbPort;
        private Button _btnConnect;
        private Button _btnExport;
        private Button _btnUncertainty;
        private Label _lblStatus;

        private Panel _chartToolbar;
        private CheckBox _chkTemperature;
        private CheckBox _chkHumidity;
        private Button BtnShowGuide;
        private Label label1;
        private Label label2;
    }
}
