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
            lblTchResult = new Label();
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
            lblUch1Final = new Label();
            lblUch2Final = new Label();
            lblUcFinal = new Label();
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
            btnSaveToDb = new Button();
            lblStep4Title = new Label();
            lblStep5Title = new Label();
            mainLayout.SuspendLayout();
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
            mainLayout.Controls.Add(step1Panel, 0, 0);
            mainLayout.Controls.Add(step2Panel, 0, 1);
            mainLayout.Controls.Add(splitStep3, 0, 2);
            mainLayout.Controls.Add(resultPanel, 0, 3);
            mainLayout.Controls.Add(bottomPanel, 0, 4);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(10);
            mainLayout.RowCount = 5;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            mainLayout.Size = new Size(1400, 1100);
            mainLayout.TabIndex = 0;
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
            step1Panel.Location = new Point(13, 13);
            step1Panel.Name = "step1Panel";
            step1Panel.Size = new Size(1374, 94);
            step1Panel.TabIndex = 0;
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
            step2Panel.Location = new Point(13, 113);
            step2Panel.Name = "step2Panel";
            step2Panel.Size = new Size(1374, 428);
            step2Panel.TabIndex = 1;
            // 
            // lblStep2Title
            // 
            lblStep2Title.AutoSize = true;
            lblStep2Title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStep2Title.Location = new Point(10, 10);
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
            gridMeasurements.Location = new Point(10, 40);
            gridMeasurements.Name = "gridMeasurements";
            gridMeasurements.RowHeadersWidth = 51;
            gridMeasurements.Size = new Size(1352, 344);
            gridMeasurements.TabIndex = 1;
            // 
            // lblUch1Result
            // 
            lblUch1Result.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblUch1Result.AutoSize = true;
            lblUch1Result.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUch1Result.ForeColor = Color.DarkBlue;
            lblUch1Result.Location = new Point(10, 394);
            lblUch1Result.Name = "lblUch1Result";
            lblUch1Result.Size = new Size(157, 23);
            lblUch1Result.TabIndex = 2;
            lblUch1Result.Text = "uch1 = (chưa tính)";
            // 
            // lblTchResult
            // 
            lblTchResult.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTchResult.AutoSize = true;
            lblTchResult.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTchResult.ForeColor = Color.DarkMagenta;
            lblTchResult.Location = new Point(29, 75);
            lblTchResult.Name = "lblTchResult";
            lblTchResult.Size = new Size(85, 23);
            lblTchResult.TabIndex = 3;
            lblTchResult.Text = "t̄_ch = ---";
            // 
            // splitStep3
            // 
            splitStep3.ColumnCount = 2;
            splitStep3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            splitStep3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            splitStep3.Controls.Add(pnlStandards, 0, 0);
            splitStep3.Controls.Add(pnlIndicator, 1, 0);
            splitStep3.Dock = DockStyle.Fill;
            splitStep3.Location = new Point(13, 547);
            splitStep3.Name = "splitStep3";
            splitStep3.RowCount = 1;
            splitStep3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            splitStep3.Size = new Size(1374, 319);
            splitStep3.TabIndex = 2;
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
            pnlStandards.Size = new Size(749, 313);
            pnlStandards.TabIndex = 0;
            // 
            // lblStep3Title
            // 
            lblStep3Title.AutoSize = true;
            lblStep3Title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStep3Title.Location = new Point(10, 10);
            lblStep3Title.Name = "lblStep3Title";
            lblStep3Title.Size = new Size(375, 23);
            lblStep3Title.TabIndex = 0;
            lblStep3Title.Text = "Nhập thông số thiết bị chuẩn (U1...Uj, ∂1...∂j)";
            // 
            // rbUseU
            // 
            rbUseU.AutoSize = true;
            rbUseU.Checked = true;
            rbUseU.Location = new Point(10, 40);
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
            rbUseDelta.Location = new Point(200, 40);
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
            gridStandards.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridStandards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridStandards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridStandards.Location = new Point(10, 63);
            gridStandards.Name = "gridStandards";
            gridStandards.RowHeadersWidth = 51;
            gridStandards.Size = new Size(727, 201);
            gridStandards.TabIndex = 3;
            // 
            // lblUch2Result
            // 
            lblUch2Result.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblUch2Result.AutoSize = true;
            lblUch2Result.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUch2Result.ForeColor = Color.DarkGreen;
            lblUch2Result.Location = new Point(10, 279);
            lblUch2Result.Name = "lblUch2Result";
            lblUch2Result.Size = new Size(157, 23);
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
            pnlIndicator.Size = new Size(613, 313);
            pnlIndicator.TabIndex = 1;
            // 
            // lblStep3bTitle
            // 
            lblStep3bTitle.AutoSize = true;
            lblStep3bTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStep3bTitle.Location = new Point(10, 10);
            lblStep3bTitle.Name = "lblStep3bTitle";
            lblStep3bTitle.Size = new Size(398, 23);
            lblStep3bTitle.TabIndex = 0;
            lblStep3bTitle.Text = "Nhập chỉ thị tủ nhiệt (t_tn1, t_tn2) & Độ phân giải";
            // 
            // gridIndicator
            // 
            gridIndicator.AllowUserToAddRows = false;
            gridIndicator.AllowUserToDeleteRows = false;
            gridIndicator.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridIndicator.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridIndicator.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridIndicator.Location = new Point(10, 40);
            gridIndicator.Name = "gridIndicator";
            gridIndicator.RowHeadersWidth = 51;
            gridIndicator.Size = new Size(591, 196);
            gridIndicator.TabIndex = 1;
            // 
            // lblResA
            // 
            lblResA.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblResA.AutoSize = true;
            lblResA.Location = new Point(10, 244);
            lblResA.Name = "lblResA";
            lblResA.Size = new Size(149, 20);
            lblResA.TabIndex = 2;
            lblResA.Text = "Độ chia nhỏ nhất (A):";
            // 
            // numResolutionA
            // 
            numResolutionA.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            numResolutionA.DecimalPlaces = 2;
            numResolutionA.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numResolutionA.Location = new Point(165, 242);
            numResolutionA.Name = "numResolutionA";
            numResolutionA.Size = new Size(80, 27);
            numResolutionA.TabIndex = 3;
            numResolutionA.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblResD
            // 
            lblResD.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblResD.AutoSize = true;
            lblResD.Location = new Point(270, 244);
            lblResD.Name = "lblResD";
            lblResD.Size = new Size(109, 20);
            lblResD.TabIndex = 4;
            lblResD.Text = "Hệ số nhân (d):";
            // 
            // numResolutionD
            // 
            numResolutionD.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            numResolutionD.DecimalPlaces = 2;
            numResolutionD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numResolutionD.Location = new Point(385, 242);
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
            resultPanel.Location = new Point(13, 872);
            resultPanel.Name = "resultPanel";
            resultPanel.Size = new Size(1374, 154);
            resultPanel.TabIndex = 4;
            // 
            // lblUch1Final
            // 
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
            lblUcFinal.AutoSize = true;
            lblUcFinal.Font = new Font("Segoe UI", 9F);
            lblUcFinal.ForeColor = Color.DarkBlue;
            lblUcFinal.Location = new Point(504, 114);
            lblUcFinal.Name = "lblUcFinal";
            lblUcFinal.Size = new Size(60, 20);
            lblUcFinal.TabIndex = 3;
            lblUcFinal.Text = "uc = ---";
            // 
            // lblUbk1
            // 
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
            lblUbkResult.AutoSize = true;
            lblUbkResult.Font = new Font("Segoe UI", 9F);
            lblUbkResult.ForeColor = Color.DarkOrange;
            lblUbkResult.Location = new Point(1089, 75);
            lblUbkResult.Name = "lblUbkResult";
            lblUbkResult.Size = new Size(69, 20);
            lblUbkResult.TabIndex = 9;
            lblUbkResult.Text = "ubk = ---";
            // 
            // lblTtnResult
            // 
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
            lblUFinal.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblUFinal.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblUFinal.ForeColor = Color.DarkRed;
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
            bottomPanel.Controls.Add(btnSaveToDb);
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.Location = new Point(13, 1032);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(1374, 55);
            bottomPanel.TabIndex = 4;
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
            // btnSaveToDb
            // 
            btnSaveToDb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveToDb.BackColor = Color.LightGreen;
            btnSaveToDb.Location = new Point(1134, 7);
            btnSaveToDb.Name = "btnSaveToDb";
            btnSaveToDb.Size = new Size(120, 40);
            btnSaveToDb.TabIndex = 3;
            btnSaveToDb.Text = "Lưu vào DB";
            btnSaveToDb.UseVisualStyleBackColor = false;
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
            // lblStep5Title
            // 
            lblStep5Title.AutoSize = true;
            lblStep5Title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStep5Title.Location = new Point(10, 5);
            lblStep5Title.Name = "lblStep5Title";
            lblStep5Title.Size = new Size(280, 23);
            lblStep5Title.TabIndex = 0;
            lblStep5Title.Text = "Kết quả tính ubk (chỉ thị tủ nhiệt)";
            // 
            // UncertaintyCalculationForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1400, 1100);
            Controls.Add(mainLayout);
            MinimumSize = new Size(1200, 700);
            Name = "UncertaintyCalculationForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Tính toán độ không đảm bảo đo";
            mainLayout.ResumeLayout(false);
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
        private Button btnSaveToDb;
    }
}
