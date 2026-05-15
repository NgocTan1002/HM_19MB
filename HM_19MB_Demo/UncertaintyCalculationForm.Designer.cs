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
            mainLayout = new TableLayoutPanel();
            step0Panel = new Panel();
            lblStep0Title = new Label();
            lblGiaTriDat = new Label();
            txtGiaTriDat = new TextBox();
            lblGiaTriDatUnit = new Label();
            step1Panel = new Panel();
            lblStep1Title = new Label();
            lblChannels = new Label();
            numChannels = new NumericUpDown();
            lblMeasurements = new Label();
            numMeasurements = new NumericUpDown();
            btnApplyConfig = new Button();
            step2Panel = new Panel();
            lblStep2Title = new Label();
            gridMeasurements = new DataGridView();
            lblUch1Result = new Label();
            splitStep3 = new TableLayoutPanel();
            pnlStandards = new Panel();
            lblStep3Title = new Label();
            rbUseU = new RadioButton();
            rbUseDelta = new RadioButton();
            gridStandards = new DataGridView();
            lblUch2Result = new Label();
            pnlIndicator = new Panel();
            lblStep3bTitle = new Label();
            gridIndicator = new DataGridView();
            lblResA = new Label();
            numResolutionA = new NumericUpDown();
            lblResD = new Label();
            numResolutionD = new NumericUpDown();
            resultPanel = new Panel();
            lblStep4Title = new Label();
            lblUch1Final = new Label();
            lblUch2Final = new Label();
            lblUcFinal = new Label();
            lblTchResult = new Label();
            lblUbk1 = new Label();
            lblUbk2 = new Label();
            lblUbk3 = new Label();
            lblUbk4 = new Label();
            lblUbkResult = new Label();
            lblTtnResult = new Label();
            lblDeltaT = new Label();
            lblDeltaOd = new Label();
            lblDeltaDd = new Label();
            lblUFinal = new Label();
            bottomPanel = new Panel();
            btnCalculate = new Button();
            btnAddToTable = new Button();
            btnSaveToDb = new Button();
            lblStep5Title = new Label();
            mainLayout.SuspendLayout();
            step0Panel.SuspendLayout();
            step1Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numChannels).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMeasurements).BeginInit();
            step2Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridMeasurements).BeginInit();
            splitStep3.SuspendLayout();
            pnlStandards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridStandards).BeginInit();
            pnlIndicator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridIndicator).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numResolutionA).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numResolutionD).BeginInit();
            resultPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(step0Panel, 0, 0);
            mainLayout.Controls.Add(step1Panel, 0, 1);
            mainLayout.Controls.Add(step2Panel, 0, 2);
            mainLayout.Controls.Add(splitStep3, 0, 3);
            mainLayout.Controls.Add(resultPanel, 0, 4);
            mainLayout.Controls.Add(bottomPanel, 0, 5);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(10);
            mainLayout.RowCount = 6;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            mainLayout.Size = new Size(1400, 1170);
            mainLayout.TabIndex = 0;
            // 
            // step0Panel
            // 
            step0Panel.BackColor = Color.FromArgb(255, 248, 230);
            step0Panel.BorderStyle = BorderStyle.FixedSingle;
            step0Panel.Controls.Add(lblStep0Title);
            step0Panel.Controls.Add(lblGiaTriDat);
            step0Panel.Controls.Add(txtGiaTriDat);
            step0Panel.Controls.Add(lblGiaTriDatUnit);
            step0Panel.Dock = DockStyle.Fill;
            step0Panel.Location = new Point(13, 13);
            step0Panel.Name = "step0Panel";
            step0Panel.Size = new Size(1374, 64);
            step0Panel.TabIndex = 0;
            // 
            // lblStep0Title
            // 
            lblStep0Title.AutoSize = true;
            lblStep0Title.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStep0Title.Location = new Point(10, 8);
            lblStep0Title.Name = "lblStep0Title";
            lblStep0Title.Size = new Size(136, 25);
            lblStep0Title.TabIndex = 0;
            lblStep0Title.Text = "Điểm kiểm tra";
            // 
            // lblGiaTriDat
            // 
            lblGiaTriDat.AutoSize = true;
            lblGiaTriDat.Location = new Point(10, 38);
            lblGiaTriDat.Name = "lblGiaTriDat";
            lblGiaTriDat.Size = new Size(107, 20);
            lblGiaTriDat.TabIndex = 1;
            lblGiaTriDat.Text = "Giá trị đặt (°C):";
            // 
            // txtGiaTriDat
            // 
            txtGiaTriDat.Location = new Point(120, 35);
            txtGiaTriDat.Name = "txtGiaTriDat";
            txtGiaTriDat.Size = new Size(100, 27);
            txtGiaTriDat.TabIndex = 2;
            txtGiaTriDat.Text = "0.0";
            // 
            // lblGiaTriDatUnit
            // 
            lblGiaTriDatUnit.AutoSize = true;
            lblGiaTriDatUnit.Location = new Point(228, 38);
            lblGiaTriDatUnit.Name = "lblGiaTriDatUnit";
            lblGiaTriDatUnit.Size = new Size(24, 20);
            lblGiaTriDatUnit.TabIndex = 3;
            lblGiaTriDatUnit.Text = "°C";
            // 
            // step1Panel
            // 
            step1Panel.BackColor = Color.FromArgb(240, 248, 255);
            step1Panel.BorderStyle = BorderStyle.FixedSingle;
            step1Panel.Controls.Add(lblStep1Title);
            step1Panel.Controls.Add(lblChannels);
            step1Panel.Controls.Add(numChannels);
            step1Panel.Controls.Add(lblMeasurements);
            step1Panel.Controls.Add(numMeasurements);
            step1Panel.Controls.Add(btnApplyConfig);
            step1Panel.Dock = DockStyle.Fill;
            step1Panel.Location = new Point(13, 83);
            step1Panel.Name = "step1Panel";
            step1Panel.Size = new Size(1374, 94);
            step1Panel.TabIndex = 1;
            // 
            // lblStep1Title
            // 
            lblStep1Title.AutoSize = true;
            lblStep1Title.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStep1Title.Location = new Point(10, 10);
            lblStep1Title.Name = "lblStep1Title";
            lblStep1Title.Size = new Size(275, 25);
            lblStep1Title.TabIndex = 0;
            lblStep1Title.Text = "Cấu hình số kênh và số lần đo";
            // 
            // lblChannels
            // 
            lblChannels.AutoSize = true;
            lblChannels.Location = new Point(10, 55);
            lblChannels.Name = "lblChannels";
            lblChannels.Size = new Size(82, 20);
            lblChannels.TabIndex = 1;
            lblChannels.Text = "Số kênh (j):";
            // 
            // numChannels
            // 
            numChannels.Location = new Point(100, 53);
            numChannels.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numChannels.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numChannels.Name = "numChannels";
            numChannels.Size = new Size(100, 27);
            numChannels.TabIndex = 2;
            numChannels.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // lblMeasurements
            // 
            lblMeasurements.AutoSize = true;
            lblMeasurements.Location = new Point(220, 55);
            lblMeasurements.Name = "lblMeasurements";
            lblMeasurements.Size = new Size(97, 20);
            lblMeasurements.TabIndex = 3;
            lblMeasurements.Text = "Số lần đo (n):";
            // 
            // numMeasurements
            // 
            numMeasurements.Location = new Point(330, 53);
            numMeasurements.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numMeasurements.Name = "numMeasurements";
            numMeasurements.Size = new Size(100, 27);
            numMeasurements.TabIndex = 4;
            numMeasurements.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // btnApplyConfig
            // 
            btnApplyConfig.Location = new Point(450, 50);
            btnApplyConfig.Name = "btnApplyConfig";
            btnApplyConfig.Size = new Size(100, 35);
            btnApplyConfig.TabIndex = 5;
            btnApplyConfig.Text = "Áp dụng";
            btnApplyConfig.UseVisualStyleBackColor = true;
            // 
            // step2Panel
            // 
            step2Panel.BorderStyle = BorderStyle.FixedSingle;
            step2Panel.Controls.Add(lblStep2Title);
            step2Panel.Controls.Add(gridMeasurements);
            step2Panel.Controls.Add(lblUch1Result);
            step2Panel.Dock = DockStyle.Fill;
            step2Panel.Location = new Point(13, 183);
            step2Panel.Name = "step2Panel";
            step2Panel.Size = new Size(1374, 474);
            step2Panel.TabIndex = 2;
            // 
            // lblStep2Title
            // 
            lblStep2Title.AutoSize = true;
            lblStep2Title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStep2Title.Location = new Point(10, 8);
            lblStep2Title.Name = "lblStep2Title";
            lblStep2Title.Size = new Size(401, 23);
            lblStep2Title.TabIndex = 0;
            lblStep2Title.Text = "Nhập dữ liệu đo (ti,j) - Tự động tính t̄j, Sj, uch1,j";
            // 
            // gridMeasurements
            // 
            gridMeasurements.AllowUserToAddRows = false;
            gridMeasurements.AllowUserToDeleteRows = false;
            gridMeasurements.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridMeasurements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridMeasurements.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridMeasurements.Location = new Point(10, 35);
            gridMeasurements.Name = "gridMeasurements";
            gridMeasurements.RowHeadersWidth = 51;
            gridMeasurements.Size = new Size(1352, 402);
            gridMeasurements.TabIndex = 1;
            // 
            // lblUch1Result
            // 
            lblUch1Result.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblUch1Result.AutoSize = true;
            lblUch1Result.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblUch1Result.ForeColor = Color.DarkBlue;
            lblUch1Result.Location = new Point(10, 440);
            lblUch1Result.Name = "lblUch1Result";
            lblUch1Result.Size = new Size(140, 20);
            lblUch1Result.TabIndex = 2;
            lblUch1Result.Text = "uch1 = (chưa tính)";
            // 
            // splitStep3
            // 
            splitStep3.ColumnCount = 2;
            splitStep3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            splitStep3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            splitStep3.Controls.Add(pnlStandards, 0, 0);
            splitStep3.Controls.Add(pnlIndicator, 1, 0);
            splitStep3.Dock = DockStyle.Fill;
            splitStep3.Location = new Point(13, 663);
            splitStep3.Name = "splitStep3";
            splitStep3.RowCount = 1;
            splitStep3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            splitStep3.Size = new Size(1374, 214);
            splitStep3.TabIndex = 3;
            // 
            // pnlStandards
            // 
            pnlStandards.BackColor = Color.FromArgb(255, 250, 240);
            pnlStandards.BorderStyle = BorderStyle.FixedSingle;
            pnlStandards.Controls.Add(lblStep3Title);
            pnlStandards.Controls.Add(rbUseU);
            pnlStandards.Controls.Add(rbUseDelta);
            pnlStandards.Controls.Add(gridStandards);
            pnlStandards.Controls.Add(lblUch2Result);
            pnlStandards.Dock = DockStyle.Fill;
            pnlStandards.Location = new Point(3, 3);
            pnlStandards.Name = "pnlStandards";
            pnlStandards.Size = new Size(749, 208);
            pnlStandards.TabIndex = 0;
            // 
            // lblStep3Title
            // 
            lblStep3Title.AutoSize = true;
            lblStep3Title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStep3Title.Location = new Point(10, 8);
            lblStep3Title.Name = "lblStep3Title";
            lblStep3Title.Size = new Size(375, 23);
            lblStep3Title.TabIndex = 0;
            lblStep3Title.Text = "Nhập thông số thiết bị chuẩn (U1...Uj, ∂1...∂j)";
            // 
            // rbUseU
            // 
            rbUseU.AutoSize = true;
            rbUseU.Checked = true;
            rbUseU.Location = new Point(10, 36);
            rbUseU.Name = "rbUseU";
            rbUseU.Size = new Size(177, 24);
            rbUseU.TabIndex = 1;
            rbUseU.TabStop = true;
            rbUseU.Text = "Tính từ U (uch2 = U/2)";
            rbUseU.UseVisualStyleBackColor = true;
            // 
            // rbUseDelta
            // 
            rbUseDelta.AutoSize = true;
            rbUseDelta.Location = new Point(200, 36);
            rbUseDelta.Name = "rbUseDelta";
            rbUseDelta.Size = new Size(183, 24);
            rbUseDelta.TabIndex = 2;
            rbUseDelta.Text = "Tính từ ∂ (uch2 = ∂/√3)";
            rbUseDelta.UseVisualStyleBackColor = true;
            // 
            // gridStandards
            // 
            gridStandards.AllowUserToAddRows = false;
            gridStandards.AllowUserToDeleteRows = false;
            gridStandards.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gridStandards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridStandards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridStandards.Location = new Point(10, 62);
            gridStandards.Name = "gridStandards";
            gridStandards.RowHeadersWidth = 51;
            gridStandards.Size = new Size(727, 114);
            gridStandards.TabIndex = 3;
            // 
            // lblUch2Result
            // 
            lblUch2Result.AutoSize = true;
            lblUch2Result.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblUch2Result.ForeColor = Color.DarkGreen;
            lblUch2Result.Location = new Point(10, 180);
            lblUch2Result.Name = "lblUch2Result";
            lblUch2Result.Size = new Size(140, 20);
            lblUch2Result.TabIndex = 4;
            lblUch2Result.Text = "uch2 = (chưa tính)";
            // 
            // pnlIndicator
            // 
            pnlIndicator.BackColor = Color.FromArgb(245, 245, 255);
            pnlIndicator.BorderStyle = BorderStyle.FixedSingle;
            pnlIndicator.Controls.Add(lblStep3bTitle);
            pnlIndicator.Controls.Add(gridIndicator);
            pnlIndicator.Controls.Add(lblResA);
            pnlIndicator.Controls.Add(numResolutionA);
            pnlIndicator.Controls.Add(lblResD);
            pnlIndicator.Controls.Add(numResolutionD);
            pnlIndicator.Dock = DockStyle.Fill;
            pnlIndicator.Location = new Point(758, 3);
            pnlIndicator.Name = "pnlIndicator";
            pnlIndicator.Size = new Size(613, 208);
            pnlIndicator.TabIndex = 1;
            // 
            // lblStep3bTitle
            // 
            lblStep3bTitle.AutoSize = true;
            lblStep3bTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStep3bTitle.Location = new Point(10, 6);
            lblStep3bTitle.Name = "lblStep3bTitle";
            lblStep3bTitle.Size = new Size(398, 23);
            lblStep3bTitle.TabIndex = 0;
            lblStep3bTitle.Text = "Nhập chỉ thị tủ nhiệt (t_tn1, t_tn2) & Độ phân giải";
            // 
            // gridIndicator
            // 
            gridIndicator.AllowUserToAddRows = false;
            gridIndicator.AllowUserToDeleteRows = false;
            gridIndicator.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gridIndicator.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridIndicator.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridIndicator.Location = new Point(10, 33);
            gridIndicator.Name = "gridIndicator";
            gridIndicator.RowHeadersWidth = 51;
            gridIndicator.Size = new Size(591, 137);
            gridIndicator.TabIndex = 1;
            // 
            // lblResA
            // 
            lblResA.AutoSize = true;
            lblResA.Location = new Point(12, 178);
            lblResA.Name = "lblResA";
            lblResA.Size = new Size(149, 20);
            lblResA.TabIndex = 2;
            lblResA.Text = "Độ chia nhỏ nhất (A):";
            // 
            // numResolutionA
            // 
            numResolutionA.DecimalPlaces = 2;
            numResolutionA.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numResolutionA.Location = new Point(167, 176);
            numResolutionA.Name = "numResolutionA";
            numResolutionA.Size = new Size(80, 27);
            numResolutionA.TabIndex = 3;
            numResolutionA.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblResD
            // 
            lblResD.AutoSize = true;
            lblResD.Location = new Point(272, 178);
            lblResD.Name = "lblResD";
            lblResD.Size = new Size(109, 20);
            lblResD.TabIndex = 4;
            lblResD.Text = "Hệ số nhân (d):";
            // 
            // numResolutionD
            // 
            numResolutionD.DecimalPlaces = 2;
            numResolutionD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numResolutionD.Location = new Point(387, 176);
            numResolutionD.Maximum = new decimal(new int[] { 5, 0, 0, 65536 });
            numResolutionD.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numResolutionD.Name = "numResolutionD";
            numResolutionD.Size = new Size(80, 27);
            numResolutionD.TabIndex = 5;
            numResolutionD.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // resultPanel
            // 
            resultPanel.BackColor = Color.FromArgb(240, 255, 240);
            resultPanel.BorderStyle = BorderStyle.FixedSingle;
            resultPanel.Controls.Add(lblStep4Title);
            resultPanel.Controls.Add(lblUch1Final);
            resultPanel.Controls.Add(lblUch2Final);
            resultPanel.Controls.Add(lblUcFinal);
            resultPanel.Controls.Add(lblTchResult);
            resultPanel.Controls.Add(lblUbk1);
            resultPanel.Controls.Add(lblUbk2);
            resultPanel.Controls.Add(lblUbk3);
            resultPanel.Controls.Add(lblUbk4);
            resultPanel.Controls.Add(lblUbkResult);
            resultPanel.Controls.Add(lblTtnResult);
            resultPanel.Controls.Add(lblDeltaT);
            resultPanel.Controls.Add(lblDeltaOd);
            resultPanel.Controls.Add(lblDeltaDd);
            resultPanel.Controls.Add(lblUFinal);
            resultPanel.Dock = DockStyle.Fill;
            resultPanel.Location = new Point(13, 883);
            resultPanel.Name = "resultPanel";
            resultPanel.Size = new Size(1374, 214);
            resultPanel.TabIndex = 4;
            // 
            // lblStep4Title
            // 
            lblStep4Title.AutoSize = true;
            lblStep4Title.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStep4Title.Location = new Point(10, 10);
            lblStep4Title.Name = "lblStep4Title";
            lblStep4Title.Size = new Size(166, 25);
            lblStep4Title.TabIndex = 0;
            lblStep4Title.Text = "Kết quả tính toán";
            // 
            // lblUch1Final
            // 
            lblUch1Final.Anchor = AnchorStyles.None;
            lblUch1Final.AutoSize = true;
            lblUch1Final.Font = new Font("Segoe UI", 9F);
            lblUch1Final.Location = new Point(504, 32);
            lblUch1Final.Name = "lblUch1Final";
            lblUch1Final.Size = new Size(76, 20);
            lblUch1Final.TabIndex = 1;
            lblUch1Final.Text = "uch1 = ---";
            // 
            // lblUch2Final
            // 
            lblUch2Final.Anchor = AnchorStyles.None;
            lblUch2Final.AutoSize = true;
            lblUch2Final.Font = new Font("Segoe UI", 9F);
            lblUch2Final.Location = new Point(504, 75);
            lblUch2Final.Name = "lblUch2Final";
            lblUch2Final.Size = new Size(76, 20);
            lblUch2Final.TabIndex = 2;
            lblUch2Final.Text = "uch2 = ---";
            // 
            // lblUcFinal
            // 
            lblUcFinal.Anchor = AnchorStyles.None;
            lblUcFinal.AutoSize = true;
            lblUcFinal.Font = new Font("Segoe UI", 9F);
            lblUcFinal.ForeColor = Color.Black;
            lblUcFinal.Location = new Point(504, 114);
            lblUcFinal.Name = "lblUcFinal";
            lblUcFinal.Size = new Size(60, 20);
            lblUcFinal.TabIndex = 3;
            lblUcFinal.Text = "uc = ---";
            // 
            // lblTchResult
            // 
            lblTchResult.Anchor = AnchorStyles.None;
            lblTchResult.AutoSize = true;
            lblTchResult.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTchResult.ForeColor = Color.Black;
            lblTchResult.Location = new Point(29, 75);
            lblTchResult.Name = "lblTchResult";
            lblTchResult.Size = new Size(71, 20);
            lblTchResult.TabIndex = 3;
            lblTchResult.Text = "t̄_ch = ---";
            // 
            // lblUbk1
            // 
            lblUbk1.Anchor = AnchorStyles.None;
            lblUbk1.AutoSize = true;
            lblUbk1.Font = new Font("Segoe UI", 9F);
            lblUbk1.Location = new Point(777, 32);
            lblUbk1.Name = "lblUbk1";
            lblUbk1.Size = new Size(77, 20);
            lblUbk1.TabIndex = 5;
            lblUbk1.Text = "ubk1 = ---";
            // 
            // lblUbk2
            // 
            lblUbk2.Anchor = AnchorStyles.None;
            lblUbk2.AutoSize = true;
            lblUbk2.Font = new Font("Segoe UI", 9F);
            lblUbk2.Location = new Point(777, 75);
            lblUbk2.Name = "lblUbk2";
            lblUbk2.Size = new Size(77, 20);
            lblUbk2.TabIndex = 6;
            lblUbk2.Text = "ubk2 = ---";
            // 
            // lblUbk3
            // 
            lblUbk3.Anchor = AnchorStyles.None;
            lblUbk3.AutoSize = true;
            lblUbk3.Font = new Font("Segoe UI", 9F);
            lblUbk3.Location = new Point(777, 114);
            lblUbk3.Name = "lblUbk3";
            lblUbk3.Size = new Size(77, 20);
            lblUbk3.TabIndex = 7;
            lblUbk3.Text = "ubk3 = ---";
            // 
            // lblUbk4
            // 
            lblUbk4.Anchor = AnchorStyles.None;
            lblUbk4.AutoSize = true;
            lblUbk4.Font = new Font("Segoe UI", 9F);
            lblUbk4.Location = new Point(1089, 32);
            lblUbk4.Name = "lblUbk4";
            lblUbk4.Size = new Size(77, 20);
            lblUbk4.TabIndex = 8;
            lblUbk4.Text = "ubk4 = ---";
            // 
            // lblUbkResult
            // 
            lblUbkResult.Anchor = AnchorStyles.None;
            lblUbkResult.AutoSize = true;
            lblUbkResult.Font = new Font("Segoe UI", 9F);
            lblUbkResult.ForeColor = Color.Black;
            lblUbkResult.Location = new Point(1089, 75);
            lblUbkResult.Name = "lblUbkResult";
            lblUbkResult.Size = new Size(69, 20);
            lblUbkResult.TabIndex = 9;
            lblUbkResult.Text = "ubk = ---";
            // 
            // lblTtnResult
            // 
            lblTtnResult.Anchor = AnchorStyles.None;
            lblTtnResult.AutoSize = true;
            lblTtnResult.Font = new Font("Segoe UI", 9F);
            lblTtnResult.Location = new Point(29, 32);
            lblTtnResult.Name = "lblTtnResult";
            lblTtnResult.Size = new Size(69, 20);
            lblTtnResult.TabIndex = 1;
            lblTtnResult.Text = "t̄_tn = ---";
            // 
            // lblDeltaT
            // 
            lblDeltaT.Anchor = AnchorStyles.None;
            lblDeltaT.AutoSize = true;
            lblDeltaT.Font = new Font("Segoe UI", 9F);
            lblDeltaT.Location = new Point(32, 114);
            lblDeltaT.Name = "lblDeltaT";
            lblDeltaT.Size = new Size(60, 20);
            lblDeltaT.TabIndex = 2;
            lblDeltaT.Text = "Δt = ---";
            // 
            // lblDeltaOd
            // 
            lblDeltaOd.Anchor = AnchorStyles.None;
            lblDeltaOd.AutoSize = true;
            lblDeltaOd.Font = new Font("Segoe UI", 9F);
            lblDeltaOd.Location = new Point(220, 32);
            lblDeltaOd.Name = "lblDeltaOd";
            lblDeltaOd.Size = new Size(83, 20);
            lblDeltaOd.TabIndex = 3;
            lblDeltaOd.Text = "δt_od = ---";
            // 
            // lblDeltaDd
            // 
            lblDeltaDd.Anchor = AnchorStyles.None;
            lblDeltaDd.AutoSize = true;
            lblDeltaDd.Font = new Font("Segoe UI", 9F);
            lblDeltaDd.Location = new Point(220, 77);
            lblDeltaDd.Name = "lblDeltaDd";
            lblDeltaDd.Size = new Size(83, 20);
            lblDeltaDd.TabIndex = 4;
            lblDeltaDd.Text = "δt_dd = ---";
            // 
            // lblUFinal
            // 
            lblUFinal.Anchor = AnchorStyles.None;
            lblUFinal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblUFinal.ForeColor = Color.Black;
            lblUFinal.Location = new Point(1089, 103);
            lblUFinal.Name = "lblUFinal";
            lblUFinal.Size = new Size(263, 36);
            lblUFinal.TabIndex = 4;
            lblUFinal.Text = "U = ---";
            lblUFinal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // bottomPanel
            // 
            bottomPanel.Controls.Add(btnCalculate);
            bottomPanel.Controls.Add(btnAddToTable);
            bottomPanel.Controls.Add(btnSaveToDb);
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.Location = new Point(13, 1103);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(1374, 54);
            bottomPanel.TabIndex = 5;
            // 
            // btnCalculate
            // 
            btnCalculate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCalculate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCalculate.Location = new Point(1254, 7);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(120, 40);
            btnCalculate.TabIndex = 0;
            btnCalculate.Text = "Tính toán";
            btnCalculate.UseVisualStyleBackColor = true;
            // 
            // btnAddToTable
            // 
            btnAddToTable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddToTable.BackColor = Color.FromArgb(0, 120, 215);
            btnAddToTable.Enabled = false;
            btnAddToTable.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddToTable.ForeColor = Color.White;
            btnAddToTable.Location = new Point(1094, 7);
            btnAddToTable.Name = "btnAddToTable";
            btnAddToTable.Size = new Size(150, 40);
            btnAddToTable.TabIndex = 1;
            btnAddToTable.Text = "Thêm vào bảng";
            btnAddToTable.UseVisualStyleBackColor = false;
            // 
            // btnSaveToDb
            // 
            btnSaveToDb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveToDb.BackColor = Color.LightGreen;
            btnSaveToDb.Location = new Point(974, 7);
            btnSaveToDb.Name = "btnSaveToDb";
            btnSaveToDb.Size = new Size(110, 40);
            btnSaveToDb.TabIndex = 2;
            btnSaveToDb.Text = "Lưu vào DB";
            btnSaveToDb.UseVisualStyleBackColor = false;
            // 
            // lblStep5Title
            // 
            lblStep5Title.AutoSize = true;
            lblStep5Title.Location = new Point(0, 0);
            lblStep5Title.Name = "lblStep5Title";
            lblStep5Title.Size = new Size(100, 23);
            lblStep5Title.TabIndex = 0;
            lblStep5Title.Visible = false;
            // 
            // UncertaintyCalculationForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1400, 1170);
            Controls.Add(mainLayout);
            MinimumSize = new Size(1200, 700);
            Name = "UncertaintyCalculationForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Tính toán độ không đảm bảo đo";
            WindowState = FormWindowState.Maximized;
            mainLayout.ResumeLayout(false);
            step0Panel.ResumeLayout(false);
            step0Panel.PerformLayout();
            step1Panel.ResumeLayout(false);
            step1Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numChannels).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMeasurements).EndInit();
            step2Panel.ResumeLayout(false);
            step2Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridMeasurements).EndInit();
            splitStep3.ResumeLayout(false);
            pnlStandards.ResumeLayout(false);
            pnlStandards.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridStandards).EndInit();
            pnlIndicator.ResumeLayout(false);
            pnlIndicator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridIndicator).EndInit();
            ((System.ComponentModel.ISupportInitialize)numResolutionA).EndInit();
            ((System.ComponentModel.ISupportInitialize)numResolutionD).EndInit();
            resultPanel.ResumeLayout(false);
            resultPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        // ── Step 0 controls ──────────────────────────────────────────────
        private Panel step0Panel;
        private Label lblStep0Title;
        private Label lblGiaTriDat;
        private TextBox txtGiaTriDat;
        private Label lblGiaTriDatUnit;

        // ── Step 1 controls ──────────────────────────────────────────────
        private TableLayoutPanel mainLayout;
        private Panel step1Panel;
        private Label lblStep1Title;
        private Label lblChannels;
        private NumericUpDown numChannels;
        private Label lblMeasurements;
        private NumericUpDown numMeasurements;
        private Button btnApplyConfig;
        private Panel step2Panel;
        private Label lblStep2Title;
        private DataGridView gridMeasurements;
        private Label lblUch1Result;
        private TableLayoutPanel splitStep3;
        private Panel pnlStandards;
        private Label lblStep3Title;
        private RadioButton rbUseU;
        private RadioButton rbUseDelta;
        private DataGridView gridStandards;
        private Label lblUch2Result;
        private Panel pnlIndicator;
        private Label lblStep3bTitle;
        private DataGridView gridIndicator;
        private Label lblResA;
        private NumericUpDown numResolutionA;
        private Label lblResD;
        private NumericUpDown numResolutionD;
        private Panel resultPanel;
        private Label lblStep4Title;
        private Label lblUch1Final;
        private Label lblUch2Final;
        private Label lblUcFinal;
        private Label lblUFinal;
        private Label lblTchResult;
        private Label lblStep5Title;
        private Label lblTtnResult;
        private Label lblDeltaT;
        private Label lblDeltaOd;
        private Label lblDeltaDd;
        private Label lblUbk1;
        private Label lblUbk2;
        private Label lblUbk3;
        private Label lblUbk4;
        private Label lblUbkResult;
        private Panel bottomPanel;
        private Button btnCalculate;
        private Button btnAddToTable;   // MỚI
        private Button btnSaveToDb;
    }
}