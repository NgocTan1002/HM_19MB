using System.Drawing;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    partial class UncertaintyCalculationForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            _tabMain = new TabControl();
            _tabInput = new TabPage();
            inputLayout = new TableLayoutPanel();
            configStrip = new FlowLayoutPanel();
            lblGiaTriDat = new Label();
            txtGiaTriDat = new TextBox();
            lblGiaTriDatUnit = new Label();
            lblConfigSeparator = new Label();
            lblChannels = new Label();
            numChannels = new NumericUpDown();
            lblMeasurements = new Label();
            numMeasurements = new NumericUpDown();
            btnApplyConfig = new Button();
            lblMethodB = new Label();
            rbUseU = new RadioButton();
            rbUseDelta = new RadioButton();
            correctionStrip = new FlowLayoutPanel();
            lblCorrections = new Label();
            typeBStrip = new FlowLayoutPanel();
            lblUMax = new Label();
            txtUMax = new TextBox();
            lblDeltaMax = new Label();
            txtDeltaMax = new TextBox();
            lblResA = new Label();
            txtResA = new TextBox();
            lblResD = new Label();
            txtResD = new TextBox();
            gridPanel = new Panel();
            gridData = new DataGridView();
            resultPanel = new Panel();
            resultCards = new TableLayoutPanel();
            grpMeasurementCharacteristics = new GroupBox();
            measurementCharacteristicsLayout = new TableLayoutPanel();
            lblDtTitle = new Label();
            lblR_Dt = new Label();
            lblOdTitle = new Label();
            lblR_Od = new Label();
            lblDdTitle = new Label();
            lblR_Dd = new Label();
            grpUncertaintyComponents = new GroupBox();
            uncertaintyComponentsLayout = new TableLayoutPanel();
            lblUch1Title = new Label();
            lblR_Uch1 = new Label();
            lblUch2Title = new Label();
            lblR_Uch2 = new Label();
            lblUchTitle = new Label();
            lblR_Uch = new Label();
            lblUbkTitle = new Label();
            lblR_Ubk = new Label();
            grpFinalResult = new GroupBox();
            finalResultLayout = new TableLayoutPanel();
            lblUTitle = new Label();
            lblR_U = new Label();
            bottomBar = new FlowLayoutPanel();
            btnCalculateAndAdd = new Button();
            lblStatus = new Label();
            _tabResults = new TabPage();
            _tabBudget = new TabPage();
            _tabMain.SuspendLayout();
            _tabInput.SuspendLayout();
            inputLayout.SuspendLayout();
            configStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numChannels).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMeasurements).BeginInit();
            correctionStrip.SuspendLayout();
            typeBStrip.SuspendLayout();
            gridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridData).BeginInit();
            resultPanel.SuspendLayout();
            resultCards.SuspendLayout();
            grpMeasurementCharacteristics.SuspendLayout();
            measurementCharacteristicsLayout.SuspendLayout();
            grpUncertaintyComponents.SuspendLayout();
            uncertaintyComponentsLayout.SuspendLayout();
            grpFinalResult.SuspendLayout();
            finalResultLayout.SuspendLayout();
            bottomBar.SuspendLayout();
            SuspendLayout();
            // 
            // _tabMain
            // 
            _tabMain.Controls.Add(_tabInput);
            _tabMain.Controls.Add(_tabResults);
            _tabMain.Controls.Add(_tabBudget);
            _tabMain.Dock = DockStyle.Fill;
            _tabMain.Location = new Point(0, 0);
            _tabMain.Name = "_tabMain";
            _tabMain.SelectedIndex = 0;
            _tabMain.Size = new Size(1197, 820);
            _tabMain.TabIndex = 0;
            // 
            // _tabInput
            // 
            _tabInput.Controls.Add(inputLayout);
            _tabInput.Location = new Point(4, 29);
            _tabInput.Name = "_tabInput";
            _tabInput.Padding = new Padding(3);
            _tabInput.Size = new Size(1189, 787);
            _tabInput.TabIndex = 0;
            _tabInput.Text = "Nhập liệu";
            _tabInput.UseVisualStyleBackColor = true;
            // 
            // inputLayout
            // 
            inputLayout.ColumnCount = 1;
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            inputLayout.Controls.Add(configStrip, 0, 0);
            inputLayout.Controls.Add(correctionStrip, 0, 1);
            inputLayout.Controls.Add(typeBStrip, 0, 2);
            inputLayout.Controls.Add(gridPanel, 0, 3);
            inputLayout.Controls.Add(resultPanel, 0, 4);
            inputLayout.Controls.Add(bottomBar, 0, 5);
            inputLayout.Dock = DockStyle.Fill;
            inputLayout.Location = new Point(3, 3);
            inputLayout.Name = "inputLayout";
            inputLayout.RowCount = 6;
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            inputLayout.Size = new Size(1183, 781);
            inputLayout.TabIndex = 0;
            // 
            // configStrip
            // 
            configStrip.BackColor = Color.White;
            configStrip.Controls.Add(lblGiaTriDat);
            configStrip.Controls.Add(txtGiaTriDat);
            configStrip.Controls.Add(lblGiaTriDatUnit);
            configStrip.Controls.Add(lblConfigSeparator);
            configStrip.Controls.Add(lblChannels);
            configStrip.Controls.Add(numChannels);
            configStrip.Controls.Add(lblMeasurements);
            configStrip.Controls.Add(numMeasurements);
            configStrip.Controls.Add(btnApplyConfig);
            configStrip.Controls.Add(lblMethodB);
            configStrip.Controls.Add(rbUseU);
            configStrip.Controls.Add(rbUseDelta);
            configStrip.Dock = DockStyle.Fill;
            configStrip.Location = new Point(0, 0);
            configStrip.Margin = new Padding(0);
            configStrip.Name = "configStrip";
            configStrip.Padding = new Padding(8, 7, 8, 0);
            configStrip.Size = new Size(1183, 44);
            configStrip.TabIndex = 0;
            configStrip.WrapContents = false;
            // 
            // lblGiaTriDat
            // 
            lblGiaTriDat.AutoSize = true;
            lblGiaTriDat.Location = new Point(8, 12);
            lblGiaTriDat.Margin = new Padding(0, 5, 4, 0);
            lblGiaTriDat.Name = "lblGiaTriDat";
            lblGiaTriDat.Size = new Size(106, 20);
            lblGiaTriDat.TabIndex = 0;
            lblGiaTriDat.Text = "Điểm kiểm tra:";
            // 
            // txtGiaTriDat
            // 
            txtGiaTriDat.Location = new Point(122, 7);
            txtGiaTriDat.Margin = new Padding(4, 0, 4, 0);
            txtGiaTriDat.Name = "txtGiaTriDat";
            txtGiaTriDat.Size = new Size(80, 27);
            txtGiaTriDat.TabIndex = 1;
            // 
            // lblGiaTriDatUnit
            // 
            lblGiaTriDatUnit.AutoSize = true;
            lblGiaTriDatUnit.Location = new Point(206, 12);
            lblGiaTriDatUnit.Margin = new Padding(0, 5, 12, 0);
            lblGiaTriDatUnit.Name = "lblGiaTriDatUnit";
            lblGiaTriDatUnit.Size = new Size(24, 20);
            lblGiaTriDatUnit.TabIndex = 2;
            lblGiaTriDatUnit.Text = "°C";
            // 
            // lblConfigSeparator
            // 
            lblConfigSeparator.AutoSize = true;
            lblConfigSeparator.Location = new Point(242, 12);
            lblConfigSeparator.Margin = new Padding(0, 5, 12, 0);
            lblConfigSeparator.Name = "lblConfigSeparator";
            lblConfigSeparator.Size = new Size(13, 20);
            lblConfigSeparator.TabIndex = 3;
            lblConfigSeparator.Text = "|";
            // 
            // lblChannels
            // 
            lblChannels.AutoSize = true;
            lblChannels.Location = new Point(267, 12);
            lblChannels.Margin = new Padding(0, 5, 4, 0);
            lblChannels.Name = "lblChannels";
            lblChannels.Size = new Size(107, 20);
            lblChannels.TabIndex = 4;
            lblChannels.Text = "Số kênh chuẩn:";
            // 
            // numChannels
            // 
            numChannels.Location = new Point(382, 7);
            numChannels.Margin = new Padding(4, 0, 12, 0);
            numChannels.Maximum = new decimal(new int[] { 9, 0, 0, 0 });
            numChannels.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numChannels.Name = "numChannels";
            numChannels.Size = new Size(55, 27);
            numChannels.TabIndex = 5;
            numChannels.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // lblMeasurements
            // 
            lblMeasurements.AutoSize = true;
            lblMeasurements.Location = new Point(449, 12);
            lblMeasurements.Margin = new Padding(0, 5, 4, 0);
            lblMeasurements.Name = "lblMeasurements";
            lblMeasurements.Size = new Size(75, 20);
            lblMeasurements.TabIndex = 6;
            lblMeasurements.Text = "Số lần đo:";
            // 
            // numMeasurements
            // 
            numMeasurements.Location = new Point(532, 7);
            numMeasurements.Margin = new Padding(4, 0, 12, 0);
            numMeasurements.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numMeasurements.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numMeasurements.Name = "numMeasurements";
            numMeasurements.Size = new Size(55, 27);
            numMeasurements.TabIndex = 7;
            numMeasurements.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // btnApplyConfig
            // 
            btnApplyConfig.Location = new Point(603, 8);
            btnApplyConfig.Margin = new Padding(4, 1, 12, 0);
            btnApplyConfig.Name = "btnApplyConfig";
            btnApplyConfig.Size = new Size(80, 30);
            btnApplyConfig.TabIndex = 8;
            btnApplyConfig.Text = "Áp dụng";
            btnApplyConfig.UseVisualStyleBackColor = true;
            // 
            // lblMethodB
            // 
            lblMethodB.AutoSize = true;
            lblMethodB.Location = new Point(695, 12);
            lblMethodB.Margin = new Padding(0, 5, 4, 0);
            lblMethodB.Name = "lblMethodB";
            lblMethodB.Size = new Size(304, 20);
            lblMethodB.TabIndex = 9;
            lblMethodB.Text = "Phương pháp tính ĐKĐBĐ của chuẩn (loại B)";
            // 
            // rbUseU
            // 
            rbUseU.AutoSize = true;
            rbUseU.Checked = true;
            rbUseU.Location = new Point(1007, 9);
            rbUseU.Margin = new Padding(4, 2, 6, 0);
            rbUseU.Name = "rbUseU";
            rbUseU.Size = new Size(80, 24);
            rbUseU.TabIndex = 10;
            rbUseU.TabStop = true;
            rbUseU.Text = "Dùng U";
            rbUseU.UseVisualStyleBackColor = true;
            // 
            // rbUseDelta
            // 
            rbUseDelta.AutoSize = true;
            rbUseDelta.Location = new Point(1097, 9);
            rbUseDelta.Margin = new Padding(4, 2, 0, 0);
            rbUseDelta.Name = "rbUseDelta";
            rbUseDelta.Size = new Size(78, 24);
            rbUseDelta.TabIndex = 11;
            rbUseDelta.Text = "Dùng ∂";
            rbUseDelta.UseVisualStyleBackColor = true;
            // 
            // correctionStrip
            // 
            correctionStrip.BackColor = SystemColors.ControlLightLight;
            correctionStrip.Controls.Add(lblCorrections);
            correctionStrip.Dock = DockStyle.Fill;
            correctionStrip.Location = new Point(0, 44);
            correctionStrip.Margin = new Padding(0);
            correctionStrip.Name = "correctionStrip";
            correctionStrip.Padding = new Padding(8, 4, 8, 0);
            correctionStrip.Size = new Size(1183, 36);
            correctionStrip.TabIndex = 1;
            correctionStrip.WrapContents = false;
            // 
            // lblCorrections
            // 
            lblCorrections.AutoSize = true;
            lblCorrections.Location = new Point(8, 9);
            lblCorrections.Margin = new Padding(0, 5, 10, 0);
            lblCorrections.Name = "lblCorrections";
            lblCorrections.Size = new Size(308, 20);
            lblCorrections.TabIndex = 0;
            lblCorrections.Text = "Số hiệu chính của nhiệt kế chuẩn (từ GCNHC):";
            // 
            // typeBStrip
            // 
            typeBStrip.BackColor = SystemColors.ControlLightLight;
            typeBStrip.Controls.Add(lblUMax);
            typeBStrip.Controls.Add(txtUMax);
            typeBStrip.Controls.Add(lblDeltaMax);
            typeBStrip.Controls.Add(txtDeltaMax);
            typeBStrip.Controls.Add(lblResA);
            typeBStrip.Controls.Add(txtResA);
            typeBStrip.Controls.Add(lblResD);
            typeBStrip.Controls.Add(txtResD);
            typeBStrip.Dock = DockStyle.Fill;
            typeBStrip.Location = new Point(0, 80);
            typeBStrip.Margin = new Padding(0);
            typeBStrip.Name = "typeBStrip";
            typeBStrip.Padding = new Padding(8, 4, 8, 0);
            typeBStrip.Size = new Size(1183, 36);
            typeBStrip.TabIndex = 2;
            typeBStrip.WrapContents = false;
            // 
            // lblUMax
            // 
            lblUMax.AutoSize = true;
            lblUMax.Location = new Point(8, 9);
            lblUMax.Margin = new Padding(0, 5, 4, 0);
            lblUMax.Name = "lblUMax";
            lblUMax.Size = new Size(253, 20);
            lblUMax.TabIndex = 0;
            lblUMax.Text = "ĐKĐBĐ mở rộng của thiết bị đo (°C):";
            // 
            // txtUMax
            // 
            txtUMax.Location = new Point(269, 4);
            txtUMax.Margin = new Padding(4, 0, 14, 0);
            txtUMax.Name = "txtUMax";
            txtUMax.Size = new Size(60, 27);
            txtUMax.TabIndex = 1;
            // 
            // lblDeltaMax
            // 
            lblDeltaMax.AutoSize = true;
            lblDeltaMax.Location = new Point(343, 9);
            lblDeltaMax.Margin = new Padding(0, 5, 4, 0);
            lblDeltaMax.Name = "lblDeltaMax";
            lblDeltaMax.Size = new Size(246, 20);
            lblDeltaMax.TabIndex = 2;
            lblDeltaMax.Text = "Sai số cho phép của thiết bị đo (°C):";
            // 
            // txtDeltaMax
            // 
            txtDeltaMax.Location = new Point(597, 4);
            txtDeltaMax.Margin = new Padding(4, 0, 14, 0);
            txtDeltaMax.Name = "txtDeltaMax";
            txtDeltaMax.Size = new Size(60, 27);
            txtDeltaMax.TabIndex = 3;
            // 
            // lblResA
            // 
            lblResA.AutoSize = true;
            lblResA.Location = new Point(671, 9);
            lblResA.Margin = new Padding(0, 5, 4, 0);
            lblResA.Name = "lblResA";
            lblResA.Size = new Size(201, 20);
            lblResA.TabIndex = 4;
            lblResA.Text = "A-\tGiá trị độ chia nhỏ nhất  A:";
            // 
            // txtResA
            // 
            txtResA.Location = new Point(880, 4);
            txtResA.Margin = new Padding(4, 0, 14, 0);
            txtResA.Name = "txtResA";
            txtResA.Size = new Size(55, 27);
            txtResA.TabIndex = 5;
            txtResA.Text = "0.10";
            // 
            // lblResD
            // 
            lblResD.AutoSize = true;
            lblResD.Location = new Point(949, 9);
            lblResD.Margin = new Padding(0, 5, 4, 0);
            lblResD.Name = "lblResD";
            lblResD.Size = new Size(99, 20);
            lblResD.TabIndex = 6;
            lblResD.Text = "Hệ số nhân d:";
            // 
            // txtResD
            // 
            txtResD.Location = new Point(1056, 4);
            txtResD.Margin = new Padding(4, 0, 0, 0);
            txtResD.Name = "txtResD";
            txtResD.Size = new Size(55, 27);
            txtResD.TabIndex = 7;
            txtResD.Text = "0.50";
            // 
            // gridPanel
            // 
            gridPanel.Controls.Add(gridData);
            gridPanel.Dock = DockStyle.Fill;
            gridPanel.Location = new Point(0, 116);
            gridPanel.Margin = new Padding(0);
            gridPanel.Name = "gridPanel";
            gridPanel.Padding = new Padding(8);
            gridPanel.Size = new Size(1183, 445);
            gridPanel.TabIndex = 3;
            // 
            // gridData
            // 
            gridData.AllowUserToAddRows = false;
            gridData.AllowUserToDeleteRows = false;
            gridData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridData.BackgroundColor = Color.White;
            gridData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridData.Dock = DockStyle.Fill;
            gridData.Location = new Point(8, 8);
            gridData.Name = "gridData";
            gridData.RowHeadersVisible = false;
            gridData.RowHeadersWidth = 51;
            gridData.Size = new Size(1167, 429);
            gridData.TabIndex = 0;
            // 
            // resultPanel
            // 
            resultPanel.Controls.Add(resultCards);
            resultPanel.Dock = DockStyle.Fill;
            resultPanel.Location = new Point(0, 561);
            resultPanel.Margin = new Padding(0);
            resultPanel.Name = "resultPanel";
            resultPanel.Padding = new Padding(8, 4, 8, 4);
            resultPanel.Size = new Size(1183, 180);
            resultPanel.TabIndex = 4;
            // 
            // resultCards
            // 
            resultCards.ColumnCount = 3;
            resultCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            resultCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            resultCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            resultCards.Controls.Add(grpMeasurementCharacteristics, 0, 0);
            resultCards.Controls.Add(grpUncertaintyComponents, 1, 0);
            resultCards.Controls.Add(grpFinalResult, 2, 0);
            resultCards.Dock = DockStyle.Fill;
            resultCards.Location = new Point(8, 4);
            resultCards.Name = "resultCards";
            resultCards.RowCount = 1;
            resultCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            resultCards.Size = new Size(1167, 172);
            resultCards.TabIndex = 0;
            // 
            // grpMeasurementCharacteristics
            // 
            grpMeasurementCharacteristics.Controls.Add(measurementCharacteristicsLayout);
            grpMeasurementCharacteristics.Dock = DockStyle.Fill;
            grpMeasurementCharacteristics.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grpMeasurementCharacteristics.ForeColor = Color.FromArgb(60, 60, 60);
            grpMeasurementCharacteristics.Location = new Point(3, 3);
            grpMeasurementCharacteristics.Name = "grpMeasurementCharacteristics";
            grpMeasurementCharacteristics.Padding = new Padding(6, 14, 6, 4);
            grpMeasurementCharacteristics.Size = new Size(344, 166);
            grpMeasurementCharacteristics.TabIndex = 0;
            grpMeasurementCharacteristics.TabStop = false;
            grpMeasurementCharacteristics.Text = "Đặc trưng điểm đo";
            // 
            // measurementCharacteristicsLayout
            // 
            measurementCharacteristicsLayout.ColumnCount = 2;
            measurementCharacteristicsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            measurementCharacteristicsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            measurementCharacteristicsLayout.Controls.Add(lblDtTitle, 0, 0);
            measurementCharacteristicsLayout.Controls.Add(lblR_Dt, 1, 0);
            measurementCharacteristicsLayout.Controls.Add(lblOdTitle, 0, 1);
            measurementCharacteristicsLayout.Controls.Add(lblR_Od, 1, 1);
            measurementCharacteristicsLayout.Controls.Add(lblDdTitle, 0, 2);
            measurementCharacteristicsLayout.Controls.Add(lblR_Dd, 1, 2);
            measurementCharacteristicsLayout.Dock = DockStyle.Fill;
            measurementCharacteristicsLayout.Location = new Point(6, 33);
            measurementCharacteristicsLayout.Name = "measurementCharacteristicsLayout";
            measurementCharacteristicsLayout.RowCount = 3;
            measurementCharacteristicsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            measurementCharacteristicsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            measurementCharacteristicsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            measurementCharacteristicsLayout.Size = new Size(332, 129);
            measurementCharacteristicsLayout.TabIndex = 0;
            // 
            // lblDtTitle
            // 
            lblDtTitle.Dock = DockStyle.Fill;
            lblDtTitle.Font = new Font("Segoe UI", 9F);
            lblDtTitle.ForeColor = Color.Black;
            lblDtTitle.Location = new Point(3, 0);
            lblDtTitle.Name = "lblDtTitle";
            lblDtTitle.Size = new Size(199, 43);
            lblDtTitle.TabIndex = 0;
            lblDtTitle.Text = "Số hiệu chính Δt";
            lblDtTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblR_Dt
            // 
            lblR_Dt.Dock = DockStyle.Fill;
            lblR_Dt.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblR_Dt.ForeColor = Color.Black;
            lblR_Dt.Location = new Point(208, 0);
            lblR_Dt.Name = "lblR_Dt";
            lblR_Dt.Size = new Size(121, 43);
            lblR_Dt.TabIndex = 1;
            lblR_Dt.Text = "—";
            lblR_Dt.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblOdTitle
            // 
            lblOdTitle.Dock = DockStyle.Fill;
            lblOdTitle.Font = new Font("Segoe UI", 9F);
            lblOdTitle.ForeColor = Color.Black;
            lblOdTitle.Location = new Point(3, 43);
            lblOdTitle.Name = "lblOdTitle";
            lblOdTitle.Size = new Size(199, 43);
            lblOdTitle.TabIndex = 2;
            lblOdTitle.Text = "Độ ổn định";
            lblOdTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblR_Od
            // 
            lblR_Od.Dock = DockStyle.Fill;
            lblR_Od.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblR_Od.ForeColor = Color.Black;
            lblR_Od.Location = new Point(208, 43);
            lblR_Od.Name = "lblR_Od";
            lblR_Od.Size = new Size(121, 43);
            lblR_Od.TabIndex = 3;
            lblR_Od.Text = "—";
            lblR_Od.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblDdTitle
            // 
            lblDdTitle.Dock = DockStyle.Fill;
            lblDdTitle.Font = new Font("Segoe UI", 9F);
            lblDdTitle.ForeColor = Color.Black;
            lblDdTitle.Location = new Point(3, 86);
            lblDdTitle.Name = "lblDdTitle";
            lblDdTitle.Size = new Size(199, 43);
            lblDdTitle.TabIndex = 4;
            lblDdTitle.Text = "Độ đồng đều";
            lblDdTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblR_Dd
            // 
            lblR_Dd.Dock = DockStyle.Fill;
            lblR_Dd.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblR_Dd.ForeColor = Color.Black;
            lblR_Dd.Location = new Point(208, 86);
            lblR_Dd.Name = "lblR_Dd";
            lblR_Dd.Size = new Size(121, 43);
            lblR_Dd.TabIndex = 5;
            lblR_Dd.Text = "—";
            lblR_Dd.TextAlign = ContentAlignment.MiddleRight;
            // 
            // grpUncertaintyComponents
            // 
            grpUncertaintyComponents.Controls.Add(uncertaintyComponentsLayout);
            grpUncertaintyComponents.Dock = DockStyle.Fill;
            grpUncertaintyComponents.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grpUncertaintyComponents.ForeColor = Color.FromArgb(60, 60, 60);
            grpUncertaintyComponents.Location = new Point(353, 3);
            grpUncertaintyComponents.Name = "grpUncertaintyComponents";
            grpUncertaintyComponents.Padding = new Padding(6, 14, 6, 4);
            grpUncertaintyComponents.Size = new Size(344, 166);
            grpUncertaintyComponents.TabIndex = 1;
            grpUncertaintyComponents.TabStop = false;
            grpUncertaintyComponents.Text = "Thành phần độ không đảm bảo";
            // 
            // uncertaintyComponentsLayout
            // 
            uncertaintyComponentsLayout.ColumnCount = 2;
            uncertaintyComponentsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            uncertaintyComponentsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            uncertaintyComponentsLayout.Controls.Add(lblUch1Title, 0, 0);
            uncertaintyComponentsLayout.Controls.Add(lblR_Uch1, 1, 0);
            uncertaintyComponentsLayout.Controls.Add(lblUch2Title, 0, 1);
            uncertaintyComponentsLayout.Controls.Add(lblR_Uch2, 1, 1);
            uncertaintyComponentsLayout.Controls.Add(lblUchTitle, 0, 2);
            uncertaintyComponentsLayout.Controls.Add(lblR_Uch, 1, 2);
            uncertaintyComponentsLayout.Controls.Add(lblUbkTitle, 0, 3);
            uncertaintyComponentsLayout.Controls.Add(lblR_Ubk, 1, 3);
            uncertaintyComponentsLayout.Dock = DockStyle.Fill;
            uncertaintyComponentsLayout.Location = new Point(6, 33);
            uncertaintyComponentsLayout.Name = "uncertaintyComponentsLayout";
            uncertaintyComponentsLayout.RowCount = 4;
            uncertaintyComponentsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            uncertaintyComponentsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            uncertaintyComponentsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            uncertaintyComponentsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            uncertaintyComponentsLayout.Size = new Size(332, 129);
            uncertaintyComponentsLayout.TabIndex = 0;
            // 
            // lblUch1Title
            // 
            lblUch1Title.Dock = DockStyle.Fill;
            lblUch1Title.Font = new Font("Segoe UI", 9F);
            lblUch1Title.ForeColor = Color.Black;
            lblUch1Title.Location = new Point(3, 0);
            lblUch1Title.Name = "lblUch1Title";
            lblUch1Title.Size = new Size(199, 32);
            lblUch1Title.TabIndex = 0;
            lblUch1Title.Text = "Tản mát của chuẩn (uch_1)";
            lblUch1Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblR_Uch1
            // 
            lblR_Uch1.Dock = DockStyle.Fill;
            lblR_Uch1.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblR_Uch1.ForeColor = Color.Black;
            lblR_Uch1.Location = new Point(208, 0);
            lblR_Uch1.Name = "lblR_Uch1";
            lblR_Uch1.Size = new Size(121, 32);
            lblR_Uch1.TabIndex = 1;
            lblR_Uch1.Text = "—";
            lblR_Uch1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblUch2Title
            // 
            lblUch2Title.Dock = DockStyle.Fill;
            lblUch2Title.Font = new Font("Segoe UI", 9F);
            lblUch2Title.ForeColor = Color.Black;
            lblUch2Title.Location = new Point(3, 32);
            lblUch2Title.Name = "lblUch2Title";
            lblUch2Title.Size = new Size(199, 32);
            lblUch2Title.TabIndex = 2;
            lblUch2Title.Text = "ĐKĐBĐ chuẩn (uch_2)";
            lblUch2Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblR_Uch2
            // 
            lblR_Uch2.Dock = DockStyle.Fill;
            lblR_Uch2.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblR_Uch2.ForeColor = Color.Black;
            lblR_Uch2.Location = new Point(208, 32);
            lblR_Uch2.Name = "lblR_Uch2";
            lblR_Uch2.Size = new Size(121, 32);
            lblR_Uch2.TabIndex = 3;
            lblR_Uch2.Text = "—";
            lblR_Uch2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblUchTitle
            // 
            lblUchTitle.Dock = DockStyle.Fill;
            lblUchTitle.Font = new Font("Segoe UI", 9F);
            lblUchTitle.ForeColor = Color.Black;
            lblUchTitle.Location = new Point(3, 64);
            lblUchTitle.Name = "lblUchTitle";
            lblUchTitle.Size = new Size(199, 32);
            lblUchTitle.TabIndex = 4;
            lblUchTitle.Text = "Liên hợp chuẩn (uch)";
            lblUchTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblR_Uch
            // 
            lblR_Uch.Dock = DockStyle.Fill;
            lblR_Uch.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblR_Uch.ForeColor = Color.Black;
            lblR_Uch.Location = new Point(208, 64);
            lblR_Uch.Name = "lblR_Uch";
            lblR_Uch.Size = new Size(121, 32);
            lblR_Uch.TabIndex = 5;
            lblR_Uch.Text = "—";
            lblR_Uch.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblUbkTitle
            // 
            lblUbkTitle.Dock = DockStyle.Fill;
            lblUbkTitle.Font = new Font("Segoe UI", 9F);
            lblUbkTitle.ForeColor = Color.Black;
            lblUbkTitle.Location = new Point(3, 96);
            lblUbkTitle.Name = "lblUbkTitle";
            lblUbkTitle.Size = new Size(199, 33);
            lblUbkTitle.TabIndex = 6;
            lblUbkTitle.Text = "Liên hợp tủ (u_bk)";
            lblUbkTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblR_Ubk
            // 
            lblR_Ubk.Dock = DockStyle.Fill;
            lblR_Ubk.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblR_Ubk.ForeColor = Color.Black;
            lblR_Ubk.Location = new Point(208, 96);
            lblR_Ubk.Name = "lblR_Ubk";
            lblR_Ubk.Size = new Size(121, 33);
            lblR_Ubk.TabIndex = 7;
            lblR_Ubk.Text = "—";
            lblR_Ubk.TextAlign = ContentAlignment.MiddleRight;
            // 
            // grpFinalResult
            // 
            grpFinalResult.Controls.Add(finalResultLayout);
            grpFinalResult.Dock = DockStyle.Fill;
            grpFinalResult.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grpFinalResult.ForeColor = Color.FromArgb(60, 60, 60);
            grpFinalResult.Location = new Point(703, 3);
            grpFinalResult.Name = "grpFinalResult";
            grpFinalResult.Padding = new Padding(6, 14, 6, 4);
            grpFinalResult.Size = new Size(461, 166);
            grpFinalResult.TabIndex = 2;
            grpFinalResult.TabStop = false;
            grpFinalResult.Text = "Kết quả";
            // 
            // finalResultLayout
            // 
            finalResultLayout.ColumnCount = 2;
            finalResultLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            finalResultLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            finalResultLayout.Controls.Add(lblUTitle, 0, 0);
            finalResultLayout.Controls.Add(lblR_U, 1, 0);
            finalResultLayout.Dock = DockStyle.Fill;
            finalResultLayout.Location = new Point(6, 33);
            finalResultLayout.Name = "finalResultLayout";
            finalResultLayout.RowCount = 1;
            finalResultLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            finalResultLayout.Size = new Size(449, 129);
            finalResultLayout.TabIndex = 0;
            // 
            // lblUTitle
            // 
            lblUTitle.Dock = DockStyle.Fill;
            lblUTitle.Font = new Font("Segoe UI", 9F);
            lblUTitle.ForeColor = Color.Black;
            lblUTitle.Location = new Point(3, 0);
            lblUTitle.Name = "lblUTitle";
            lblUTitle.Size = new Size(272, 129);
            lblUTitle.TabIndex = 0;
            lblUTitle.Text = "U  (k=2, P=95%)";
            lblUTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblR_U
            // 
            lblR_U.Dock = DockStyle.Fill;
            lblR_U.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblR_U.ForeColor = Color.DarkGreen;
            lblR_U.Location = new Point(281, 0);
            lblR_U.Name = "lblR_U";
            lblR_U.Size = new Size(165, 129);
            lblR_U.TabIndex = 1;
            lblR_U.Text = "—";
            lblR_U.TextAlign = ContentAlignment.MiddleRight;
            // 
            // bottomBar
            // 
            bottomBar.Controls.Add(btnCalculateAndAdd);
            bottomBar.Controls.Add(lblStatus);
            bottomBar.Dock = DockStyle.Fill;
            bottomBar.Location = new Point(0, 741);
            bottomBar.Margin = new Padding(0);
            bottomBar.Name = "bottomBar";
            bottomBar.Padding = new Padding(8, 4, 8, 0);
            bottomBar.Size = new Size(1183, 40);
            bottomBar.TabIndex = 5;
            bottomBar.WrapContents = false;
            // 
            // btnCalculateAndAdd
            // 
            btnCalculateAndAdd.Enabled = false;
            btnCalculateAndAdd.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCalculateAndAdd.Location = new Point(11, 5);
            btnCalculateAndAdd.Margin = new Padding(3, 1, 12, 0);
            btnCalculateAndAdd.Name = "btnCalculateAndAdd";
            btnCalculateAndAdd.Size = new Size(360, 32);
            btnCalculateAndAdd.TabIndex = 0;
            btnCalculateAndAdd.Text = "Tính toán và Thêm vào bảng";
            btnCalculateAndAdd.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.DarkGreen;
            lblStatus.Location = new Point(383, 10);
            lblStatus.Margin = new Padding(0, 6, 20, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 20);
            lblStatus.TabIndex = 1;
            // 
            // _tabResults
            // 
            _tabResults.Location = new Point(4, 29);
            _tabResults.Name = "_tabResults";
            _tabResults.Padding = new Padding(3);
            _tabResults.Size = new Size(1189, 787);
            _tabResults.TabIndex = 1;
            _tabResults.Text = "Kết quả tổng hợp";
            _tabResults.UseVisualStyleBackColor = true;
            // 
            // _tabBudget
            // 
            _tabBudget.Location = new Point(4, 29);
            _tabBudget.Name = "_tabBudget";
            _tabBudget.Padding = new Padding(3);
            _tabBudget.Size = new Size(1189, 787);
            _tabBudget.TabIndex = 2;
            _tabBudget.Text = "Độ không đảm bảo đo";
            _tabBudget.UseVisualStyleBackColor = true;
            // 
            // UncertaintyCalculationForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1197, 820);
            Controls.Add(_tabMain);
            MinimumSize = new Size(900, 650);
            Name = "UncertaintyCalculationForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Tính toán độ không đảm bảo đo";
            _tabMain.ResumeLayout(false);
            _tabInput.ResumeLayout(false);
            inputLayout.ResumeLayout(false);
            configStrip.ResumeLayout(false);
            configStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numChannels).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMeasurements).EndInit();
            correctionStrip.ResumeLayout(false);
            correctionStrip.PerformLayout();
            typeBStrip.ResumeLayout(false);
            typeBStrip.PerformLayout();
            gridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridData).EndInit();
            resultPanel.ResumeLayout(false);
            resultCards.ResumeLayout(false);
            grpMeasurementCharacteristics.ResumeLayout(false);
            measurementCharacteristicsLayout.ResumeLayout(false);
            grpUncertaintyComponents.ResumeLayout(false);
            uncertaintyComponentsLayout.ResumeLayout(false);
            grpFinalResult.ResumeLayout(false);
            finalResultLayout.ResumeLayout(false);
            bottomBar.ResumeLayout(false);
            bottomBar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl _tabMain;
        private TabPage _tabInput;
        private TabPage _tabResults;
        private TabPage _tabBudget;

        private TableLayoutPanel inputLayout;
        private FlowLayoutPanel configStrip;
        private Label lblGiaTriDat;
        private TextBox txtGiaTriDat;
        private Label lblGiaTriDatUnit;
        private Label lblConfigSeparator;
        private Label lblChannels;
        private NumericUpDown numChannels;
        private Label lblMeasurements;
        private NumericUpDown numMeasurements;
        private Button btnApplyConfig;
        private Label lblMethodB;
        private RadioButton rbUseU;
        private RadioButton rbUseDelta;

        private FlowLayoutPanel correctionStrip;
        private Label lblCorrections;
        private FlowLayoutPanel typeBStrip;
        private Label lblUMax;
        private TextBox txtUMax;
        private Label lblDeltaMax;
        private TextBox txtDeltaMax;
        private Label lblResA;
        private TextBox txtResA;
        private Label lblResD;
        private TextBox txtResD;

        private Panel gridPanel;
        private DataGridView gridData;
        private Panel resultPanel;
        private TableLayoutPanel resultCards;
        private GroupBox grpMeasurementCharacteristics;
        private TableLayoutPanel measurementCharacteristicsLayout;
        private Label lblDtTitle;
        private Label lblOdTitle;
        private Label lblDdTitle;
        private GroupBox grpUncertaintyComponents;
        private TableLayoutPanel uncertaintyComponentsLayout;
        private Label lblUch1Title;
        private Label lblUch2Title;
        private Label lblUchTitle;
        private Label lblUbkTitle;
        private GroupBox grpFinalResult;
        private TableLayoutPanel finalResultLayout;
        private Label lblUTitle;
        private FlowLayoutPanel bottomBar;
        private Button btnCalculateAndAdd;
        private Button btnAddToTable;
        private Label lblStatus;

        private Label lblR_Dt;
        private Label lblR_Od;
        private Label lblR_Dd;
        private Label lblR_Uch1;
        private Label lblR_Uch2;
        private Label lblR_Uch;
        private Label lblR_Ubk;
        private Label lblR_U;
    }
}
