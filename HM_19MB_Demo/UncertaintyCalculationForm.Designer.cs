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
            _tabMain.Size = new Size(1379, 820);
            _tabMain.TabIndex = 0;
            // 
            // _tabInput
            // 
            _tabInput.Controls.Add(inputLayout);
            _tabInput.Location = new Point(4, 29);
            _tabInput.Name = "_tabInput";
            _tabInput.Padding = new Padding(3);
            _tabInput.Size = new Size(1371, 787);
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
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            inputLayout.Size = new Size(1365, 781);
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
            configStrip.Size = new Size(1365, 44);
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
            txtGiaTriDat.Text = "0.0";
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
            lblMethodB.Size = new Size(492, 20);
            lblMethodB.TabIndex = 9;
            lblMethodB.Text = "Phương pháp tính ĐKĐBĐ của thiết bị đo nhiệt độ đa kênh chuẩn (loại B)";
            // 
            // rbUseU
            // 
            rbUseU.AutoSize = true;
            rbUseU.Checked = true;
            rbUseU.Location = new Point(1195, 9);
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
            rbUseDelta.Location = new Point(1285, 9);
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
            correctionStrip.Size = new Size(1365, 36);
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
            typeBStrip.Size = new Size(1365, 36);
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
            txtUMax.Text = "0.00";
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
            txtDeltaMax.Text = "0.00";
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
            gridPanel.Size = new Size(1365, 515);
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
            gridData.Size = new Size(1349, 499);
            gridData.TabIndex = 0;
            // 
            // resultPanel
            // 
            resultPanel.Controls.Add(resultCards);
            resultPanel.Dock = DockStyle.Fill;
            resultPanel.Location = new Point(0, 631);
            resultPanel.Margin = new Padding(0);
            resultPanel.Name = "resultPanel";
            resultPanel.Padding = new Padding(8, 4, 8, 4);
            resultPanel.Size = new Size(1365, 110);
            resultPanel.TabIndex = 4;
            // 
            // resultCards
            // 
            resultCards.ColumnCount = 4;
            resultCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            resultCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            resultCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            resultCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            resultCards.Dock = DockStyle.Fill;
            resultCards.Location = new Point(8, 4);
            resultCards.Name = "resultCards";
            resultCards.RowCount = 2;
            resultCards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            resultCards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            resultCards.Size = new Size(1349, 102);
            resultCards.TabIndex = 0;
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
            bottomBar.Size = new Size(1365, 40);
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
            _tabResults.Size = new Size(1422, 787);
            _tabResults.TabIndex = 1;
            _tabResults.Text = "Kết quả tổng hợp";
            _tabResults.UseVisualStyleBackColor = true;
            // 
            // _tabBudget
            // 
            _tabBudget.Location = new Point(4, 29);
            _tabBudget.Name = "_tabBudget";
            _tabBudget.Padding = new Padding(3);
            _tabBudget.Size = new Size(1422, 787);
            _tabBudget.TabIndex = 2;
            _tabBudget.Text = "Độ không đảm bảo đo";
            _tabBudget.UseVisualStyleBackColor = true;
            // 
            // UncertaintyCalculationForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1379, 820);
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
