namespace HM_19MB_Demo
{
    partial class SessionManagerForm
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            toolbar = new Panel();
            lblTitle = new Label();
            lblSearch = new Label();
            _txtSearch = new TextBox();
            gridContainer = new Panel();
            _grid = new DataGridView();
            ColId = new DataGridViewTextBoxColumn();
            ColTen = new DataGridViewTextBoxColumn();
            ColKyHieu = new DataGridViewTextBoxColumn();
            ColSoHieu = new DataGridViewTextBoxColumn();
            ColDonVi = new DataGridViewTextBoxColumn();
            ColNgayHC = new DataGridViewTextBoxColumn();
            ColDiem = new DataGridViewTextBoxColumn();
            ColLanDo = new DataGridViewTextBoxColumn();
            ColNgayTao = new DataGridViewTextBoxColumn();
            _lblLoading = new Label();
            bottomBar = new Panel();
            _btnDong = new Button();
            _btnXoa = new Button();
            _btnTaoMoi = new Button();
            _btnMoPhien = new Button();
            _lblHint = new Label();
            toolbar.SuspendLayout();
            gridContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_grid).BeginInit();
            bottomBar.SuspendLayout();
            SuspendLayout();
            // 
            // toolbar
            // 
            toolbar.BackColor = Color.FromArgb(240, 244, 248);
            toolbar.Controls.Add(lblTitle);
            toolbar.Controls.Add(lblSearch);
            toolbar.Controls.Add(_txtSearch);
            toolbar.Dock = DockStyle.Top;
            toolbar.Location = new Point(0, 0);
            toolbar.Name = "toolbar";
            toolbar.Padding = new Padding(10, 10, 10, 0);
            toolbar.Size = new Size(1116, 52);
            toolbar.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 60, 100);
            lblTitle.Location = new Point(10, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(200, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Danh sách phiên đo";
            // 
            // lblSearch
            // 
            lblSearch.AutoSize = true;
            lblSearch.ForeColor = Color.DimGray;
            lblSearch.Location = new Point(340, 17);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(77, 21);
            lblSearch.TabIndex = 1;
            lblSearch.Text = "Tìm kiếm:";
            // 
            // _txtSearch
            // 
            _txtSearch.Location = new Point(433, 12);
            _txtSearch.Name = "_txtSearch";
            _txtSearch.PlaceholderText = "Tên thiết bị, số hiệu, đơn vị...";
            _txtSearch.Size = new Size(275, 29);
            _txtSearch.TabIndex = 2;
            _txtSearch.TextChanged += TxtSearch_TextChanged;
            // 
            // gridContainer
            // 
            gridContainer.Controls.Add(_grid);
            gridContainer.Controls.Add(_lblLoading);
            gridContainer.Dock = DockStyle.Fill;
            gridContainer.Location = new Point(0, 52);
            gridContainer.Name = "gridContainer";
            gridContainer.Size = new Size(1116, 492);
            gridContainer.TabIndex = 1;
            // 
            // _grid
            // 
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(248, 250, 253);
            _grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.BackgroundColor = Color.White;
            _grid.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(230, 236, 245);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(40, 60, 90);
            dataGridViewCellStyle2.Padding = new Padding(4);
            _grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            _grid.ColumnHeadersHeight = 42;
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _grid.Columns.AddRange(new DataGridViewColumn[] { ColId, ColTen, ColKyHieu, ColSoHieu, ColDonVi, ColNgayHC, ColDiem, ColLanDo, ColNgayTao });
            _grid.Dock = DockStyle.Fill;
            _grid.Font = new Font("Segoe UI", 9F);
            _grid.GridColor = Color.FromArgb(220, 228, 238);
            _grid.Location = new Point(0, 0);
            _grid.MultiSelect = false;
            _grid.Name = "_grid";
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.RowHeadersWidth = 51;
            _grid.RowTemplate.Height = 32;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.Size = new Size(1116, 492);
            _grid.TabIndex = 0;
            _grid.CellDoubleClick += Grid_CellDoubleClick;
            _grid.SelectionChanged += Grid_SelectionChanged;
            // 
            // ColId
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColId.DefaultCellStyle = dataGridViewCellStyle3;
            ColId.FillWeight = 35F;
            ColId.HeaderText = "ID";
            ColId.MinimumWidth = 6;
            ColId.Name = "ColId";
            ColId.ReadOnly = true;
            // 
            // ColTen
            // 
            ColTen.FillWeight = 180F;
            ColTen.HeaderText = "Tên thiết bị";
            ColTen.MinimumWidth = 6;
            ColTen.Name = "ColTen";
            ColTen.ReadOnly = true;
            // 
            // ColKyHieu
            // 
            ColKyHieu.FillWeight = 80F;
            ColKyHieu.HeaderText = "Ký hiệu";
            ColKyHieu.MinimumWidth = 6;
            ColKyHieu.Name = "ColKyHieu";
            ColKyHieu.ReadOnly = true;
            // 
            // ColSoHieu
            // 
            ColSoHieu.FillWeight = 90F;
            ColSoHieu.HeaderText = "Số hiệu";
            ColSoHieu.MinimumWidth = 6;
            ColSoHieu.Name = "ColSoHieu";
            ColSoHieu.ReadOnly = true;
            // 
            // ColDonVi
            // 
            ColDonVi.FillWeight = 160F;
            ColDonVi.HeaderText = "Đơn vị sử dụng";
            ColDonVi.MinimumWidth = 6;
            ColDonVi.Name = "ColDonVi";
            ColDonVi.ReadOnly = true;
            // 
            // ColNgayHC
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColNgayHC.DefaultCellStyle = dataGridViewCellStyle4;
            ColNgayHC.HeaderText = "Ngày hiệu chuẩn";
            ColNgayHC.MinimumWidth = 6;
            ColNgayHC.Name = "ColNgayHC";
            ColNgayHC.ReadOnly = true;
            // 
            // ColDiem
            // 
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColDiem.DefaultCellStyle = dataGridViewCellStyle5;
            ColDiem.FillWeight = 65F;
            ColDiem.HeaderText = "Điểm đo";
            ColDiem.MinimumWidth = 6;
            ColDiem.Name = "ColDiem";
            ColDiem.ReadOnly = true;
            // 
            // ColLanDo
            // 
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColLanDo.DefaultCellStyle = dataGridViewCellStyle6;
            ColLanDo.FillWeight = 75F;
            ColLanDo.HeaderText = "Lần đo thô";
            ColLanDo.MinimumWidth = 6;
            ColLanDo.Name = "ColLanDo";
            ColLanDo.ReadOnly = true;
            // 
            // ColNgayTao
            // 
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColNgayTao.DefaultCellStyle = dataGridViewCellStyle7;
            ColNgayTao.FillWeight = 130F;
            ColNgayTao.HeaderText = "Ngày tạo";
            ColNgayTao.MinimumWidth = 6;
            ColNgayTao.Name = "ColNgayTao";
            ColNgayTao.ReadOnly = true;
            // 
            // _lblLoading
            // 
            _lblLoading.Dock = DockStyle.Fill;
            _lblLoading.Font = new Font("Segoe UI", 11F, FontStyle.Italic);
            _lblLoading.ForeColor = Color.Gray;
            _lblLoading.Location = new Point(0, 0);
            _lblLoading.Name = "_lblLoading";
            _lblLoading.Size = new Size(1116, 492);
            _lblLoading.TabIndex = 1;
            _lblLoading.Text = "Đang tải...";
            _lblLoading.TextAlign = ContentAlignment.MiddleCenter;
            _lblLoading.Visible = false;
            // 
            // bottomBar
            // 
            bottomBar.BackColor = Color.FromArgb(245, 247, 250);
            bottomBar.Controls.Add(_btnDong);
            bottomBar.Controls.Add(_btnXoa);
            bottomBar.Controls.Add(_btnTaoMoi);
            bottomBar.Controls.Add(_btnMoPhien);
            bottomBar.Controls.Add(_lblHint);
            bottomBar.Dock = DockStyle.Bottom;
            bottomBar.Location = new Point(0, 544);
            bottomBar.Name = "bottomBar";
            bottomBar.Padding = new Padding(12, 10, 12, 0);
            bottomBar.Size = new Size(1116, 56);
            bottomBar.TabIndex = 2;
            // 
            // _btnDong
            // 
            _btnDong.Location = new Point(1001, 10);
            _btnDong.Name = "_btnDong";
            _btnDong.Size = new Size(100, 33);
            _btnDong.TabIndex = 8;
            _btnDong.Text = "Đóng";
            _btnDong.UseVisualStyleBackColor = true;
            // 
            // _btnXoa
            // 
            _btnXoa.Location = new Point(873, 10);
            _btnXoa.Name = "_btnXoa";
            _btnXoa.Size = new Size(122, 33);
            _btnXoa.TabIndex = 7;
            _btnXoa.Text = "Xóa phiên";
            _btnXoa.UseVisualStyleBackColor = true;
            // 
            // _btnTaoMoi
            // 
            _btnTaoMoi.Location = new Point(714, 10);
            _btnTaoMoi.Name = "_btnTaoMoi";
            _btnTaoMoi.Size = new Size(153, 33);
            _btnTaoMoi.TabIndex = 6;
            _btnTaoMoi.Text = "Tạo phiên đo mới";
            _btnTaoMoi.UseVisualStyleBackColor = true;
            // 
            // _btnMoPhien
            // 
            _btnMoPhien.Location = new Point(546, 10);
            _btnMoPhien.Name = "_btnMoPhien";
            _btnMoPhien.Size = new Size(162, 33);
            _btnMoPhien.TabIndex = 5;
            _btnMoPhien.Text = "Mở phiên đã chọn";
            _btnMoPhien.UseVisualStyleBackColor = true;
            // 
            // _lblHint
            // 
            _lblHint.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            _lblHint.ForeColor = Color.DimGray;
            _lblHint.Location = new Point(12, 11);
            _lblHint.Name = "_lblHint";
            _lblHint.Size = new Size(500, 34);
            _lblHint.TabIndex = 0;
            _lblHint.Text = "Chọn một phiên đo rồi nhấn 'Mở phiên' để tiếp tục làm việc, hoặc 'Tạo phiên mới' để bắt đầu đo thiết bị mới.";
            // 
            // SessionManagerForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1116, 600);
            Controls.Add(gridContainer);
            Controls.Add(bottomBar);
            Controls.Add(toolbar);
            Font = new Font("Segoe UI", 9.5F);
            KeyPreview = true;
            MinimumSize = new Size(800, 500);
            Name = "SessionManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý phiên đo";
            Load += SessionManagerForm_Load;
            KeyDown += SessionManagerForm_KeyDown;
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            gridContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
            bottomBar.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel toolbar;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox _txtSearch;
        private System.Windows.Forms.Panel gridContainer;
        private System.Windows.Forms.Label _lblLoading;
        private System.Windows.Forms.DataGridView _grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTen;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColKyHieu;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSoHieu;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDonVi;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColNgayHC;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDiem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLanDo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColNgayTao;
        private System.Windows.Forms.Panel bottomBar;
        private System.Windows.Forms.Label _lblHint;
        private Button _btnDong;
        private Button _btnXoa;
        private Button _btnTaoMoi;
        private Button _btnMoPhien;
    }
}
