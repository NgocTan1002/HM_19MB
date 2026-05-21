using HM_19MB_Demo.Data;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    public partial class Form1
    {
        // ── Constants ─────────────────────────────────────────────────────
        private const int MaxKenhCount = 10;

        // Tên cột cố định
        private const string ColStt = "CalStt";
        private const string ColGiaTriDat = "CalGiaTriDat";
        private const string ColGiaTriChiThi = "CalGiaTriChiThi";
        // Cột kênh: "CalKenh1" .. "CalKenh10" — tạo động
        private const string ColTrungBinh = "CalTrungBinh";
        private const string ColSoHieuChinh = "CalSoHieuChinh";
        private const string ColDoOnDinh = "CalDoOnDinh";
        private const string ColDoDongDeu = "CalDoDongDeu";
        private const string ColDKDB = "CalDKDB";
        private const string ColXoa = "CalXoa";

        // ── State ─────────────────────────────────────────────────────────
        private UncertaintyCalculationForm? _uncertaintyForm;
        private bool _forceCloseUncertaintyForm;
        private int _currentKenhCount = 3; // k hiện tại (3/5/9/10)
        private int _currentMeasurementCount = 10; // n hiện tại
        private bool _updatingCalibrationConfig;
        private bool _updatingCalibrationGrid;

        // ── Khởi tạo UI bảng kết quả ─────────────────────────────────────

        /// <summary>
        /// Khởi tạo panel kết quả hiệu chuẩn và nhúng vào metadataPanel.
        /// Gọi từ Form1.WireEvents() sau InitializeComponent().
        /// </summary>
        internal void InitializeCalibrationResultsPanel()
        {
            numKenhCount.ValueChanged += (s, e) =>
            {
                SetCalibrationConfig((int)numKenhCount.Value, _currentMeasurementCount, clearRowsOnChannelChange: true);
            };

            numMeasurementCount.ValueChanged += (s, e) =>
            {
                SetCalibrationConfig(_currentKenhCount, (int)numMeasurementCount.Value, clearRowsOnChannelChange: false);
            };

            _btnAddCalibPoint.Click += BtnAddCalibPoint_Click;

            _btnDeleteCalibPoint.Click += async (s, e) =>
            {
                await DeleteSelectedCalibRowAsync();
            };

            _gridCalibration.ReadOnly = false;
            _gridCalibration.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            _gridCalibration.CellDoubleClick += GridCalibration_CellDoubleClick;
            _gridCalibration.CellEndEdit += GridCalibration_CellEndEdit;
            _gridCalibration.DataError += (s, e) => e.ThrowException = false;

            _gridCalibration.SelectionChanged += (s, e) =>
            {
                _btnDeleteCalibPoint.Enabled = _gridCalibration.SelectedRows.Count > 0;
            };

            // Xây cột ban đầu theo số kênh hiện tại
            _currentKenhCount = (int)numKenhCount.Value;
            _currentMeasurementCount = (int)numMeasurementCount.Value;
            RebuildCalibrationColumns(_currentKenhCount);

            // Load dữ liệu nếu đã có session
            _ = TryLoadCalibrationResultsAsync();
        }

        private void SetCalibrationConfig(int kenhCount, int measurementCount, bool clearRowsOnChannelChange)
        {
            kenhCount = Math.Max((int)numKenhCount.Minimum, Math.Min((int)numKenhCount.Maximum, kenhCount));
            measurementCount = Math.Max((int)numMeasurementCount.Minimum, Math.Min((int)numMeasurementCount.Maximum, measurementCount));

            bool channelChanged = kenhCount != _currentKenhCount;
            bool measurementChanged = measurementCount != _currentMeasurementCount;
            if (!channelChanged && !measurementChanged) return;

            _currentKenhCount = kenhCount;
            _currentMeasurementCount = measurementCount;

            _updatingCalibrationConfig = true;
            try
            {
                numKenhCount.Value = kenhCount;
                numMeasurementCount.Value = measurementCount;
            }
            finally
            {
                _updatingCalibrationConfig = false;
            }

            if (channelChanged)
            {
                RebuildCalibrationColumns(kenhCount);
                if (clearRowsOnChannelChange)
                {
                    _gridCalibration.Rows.Clear();
                }
            }

            if (!_updatingCalibrationConfig && _uncertaintyForm != null && !_uncertaintyForm.IsDisposed)
            {
                _uncertaintyForm.SetConfiguration(kenhCount, measurementCount);
            }
        }

        // ── Xây dựng cột động ─────────────────────────────────────────────

        private void RebuildCalibrationColumns(int kenhCount)
        {
            _gridCalibration.Columns.Clear();

            void AddCol(string name, string header, int fillWeight = 60,
                        DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleCenter,
                        Color? backColor = null)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = name,
                    HeaderText = header,
                    FillWeight = fillWeight,
                    ReadOnly = name == ColStt,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = align,
                        BackColor = backColor ?? Color.White,
                    },
                };
                _gridCalibration.Columns.Add(col);
            }

            // Cột cố định — phần A.1
            AddCol(ColStt, "STT", 35);
            AddCol(ColGiaTriDat, "Giá trị đặt\n(°C)", 70);
            AddCol(ColGiaTriChiThi, "Giá trị chỉ thị\n(°C)", 70);

            // Cột kênh — động
            for (int i = 1; i <= kenhCount; i++)
                AddCol($"CalKenh{i}", $"Kênh {i}\n(°C)", 60);

            AddCol(ColTrungBinh, "Trung bình\n(°C)", 70, DataGridViewContentAlignment.MiddleCenter,
                   Color.FromArgb(240, 248, 255));

            // Cột cố định — phần B
            AddCol(ColSoHieuChinh, "Số hiệu chính\n(°C)", 72, DataGridViewContentAlignment.MiddleCenter,
                   Color.FromArgb(255, 255, 240));
            AddCol(ColDoOnDinh, "Độ ổn định\n(°C)", 68, DataGridViewContentAlignment.MiddleCenter,
                   Color.FromArgb(255, 255, 240));
            AddCol(ColDoDongDeu, "Độ đồng đều\n(°C)", 68, DataGridViewContentAlignment.MiddleCenter,
                   Color.FromArgb(255, 255, 240));
            AddCol(ColDKDB, "ĐKĐB mở rộng\n(°C)\nk=2, P=95%", 90, DataGridViewContentAlignment.MiddleCenter,
                   Color.FromArgb(255, 245, 235));

            _gridCalibration.Rows.Clear();
        }

        // ── Thêm điểm kiểm tra ────────────────────────────────────────────

        private void BtnAddCalibPoint_Click(object? sender, EventArgs e)
        {
            // Mở UncertaintyCalculationForm dạng non-modal
            // Nếu form đã mở thì bring to front
            if (_uncertaintyForm != null && !_uncertaintyForm.IsDisposed)
            {
                if (!_uncertaintyForm.Visible)
                    _uncertaintyForm.Show(this);
                _uncertaintyForm.BringToFront();
                _uncertaintyForm.Focus();
                return;
            }

            _uncertaintyForm = new UncertaintyCalculationForm(
                kenhCount: _currentKenhCount,
                measurementCount: _currentMeasurementCount,
                phienId: _currentSessionId,
                onResultAdded: OnCalibrationResultAdded,
                onConfigChanged: (k, n) => SetCalibrationConfig(k, n, clearRowsOnChannelChange: true)
            );

            ConfigureUncertaintyFormLifetime(_uncertaintyForm);
            _uncertaintyForm.Show(this);
        }

        private void ConfigureUncertaintyFormLifetime(UncertaintyCalculationForm form)
        {
            form.FormClosing += (s, e) =>
            {
                if (!_forceCloseUncertaintyForm && e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    form.Hide();
                }
            };

            form.FormClosed += (s, e) =>
            {
                if (ReferenceEquals(_uncertaintyForm, form))
                    _uncertaintyForm = null;
            };
        }

        // ── Callback khi UncertaintyCalculationForm trả về kết quả ────────

        private void OnCalibrationResultAdded(CalibrationResultRow row)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnCalibrationResultAdded(row)));
                return;
            }

            row.STT = _gridCalibration.Rows.Count + 1;
            AddRowToCalibrationGrid(row);

            if (_currentSessionId.HasValue)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await DatabaseService.EnsureSchemaAsync();

                        // 1. Lưu kết quả tổng hợp
                        row.Id = await DatabaseService.LuuKetQuaHieuChuanAsync(
                            _currentSessionId.Value, row);

                        // 2. Lưu chi tiết từng lần đo
                        if (row.Id > 0 && row.ChiTietLanDos?.Count > 0)
                            await DatabaseService.LuuChiTietLanDoAsync(
                                row.Id, row.ChiTietLanDos);

                    }
                    catch (Exception ex)
                    {
                        AppLogger.Error("CalibrationResults", "Lỗi lưu", ex);
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show($"Lỗi lưu DB:\n{ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                });
            }
        }

        // ── Thêm 1 dòng vào grid ──────────────────────────────────────────

        private void AddRowToCalibrationGrid(CalibrationResultRow row)
        {
            // Đảm bảo số cột kênh đúng với row.SoKenh
            int k = row.SoKenh > 0 ? row.SoKenh : _currentKenhCount;
            if (k != _currentKenhCount)
            {
                _currentKenhCount = k;
                RebuildCalibrationColumns(k);
            }

            var values = new System.Collections.Generic.List<object>
            {
                row.STT,
                row.GiaTriDat.ToString("F1"),
                row.GiaTriChiThi.ToString("F1"),
            };

            // Giá trị từng kênh
            for (int i = 0; i < k; i++)
            {
                values.Add(double.IsNaN(row.Kenh[i])
                    ? "---"
                    : row.Kenh[i].ToString("F2"));
            }

            values.Add(row.GiaTriTrungBinh.ToString("F2"));
            values.Add(row.SoHieuChinh.ToString("F2"));
            values.Add(row.DoOnDinh.ToString("F2"));
            values.Add(row.DoDongDeu.ToString("F2"));
            values.Add($"±{row.DoKhongDamBao:F2}");

            int rowIdx = _gridCalibration.Rows.Add(values.ToArray());

            // Tag dòng với Id DB để xoá sau này
            _gridCalibration.Rows[rowIdx].Tag = row;

            // Scroll to bottom
            _gridCalibration.FirstDisplayedScrollingRowIndex = rowIdx;
        }

        private async void GridCalibration_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (_updatingCalibrationGrid || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var gridRow = _gridCalibration.Rows[e.RowIndex];
            if (gridRow.Tag is not CalibrationResultRow row)
                return;

            try
            {
                UpdateCalibrationRowFromGrid(gridRow, row);
                FormatCalibrationGridRow(gridRow, row);
                await SaveCalibrationRowEditAsync(row);
            }
            catch (Exception ex)
            {
                FormatCalibrationGridRow(gridRow, row);
                MessageBox.Show($"Không thể cập nhật dòng:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GridCalibration_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var column = _gridCalibration.Columns[e.ColumnIndex];
            if (column.ReadOnly)
                return;

            _gridCalibration.CurrentCell = _gridCalibration.Rows[e.RowIndex].Cells[e.ColumnIndex];
            _gridCalibration.BeginEdit(true);
        }

        private void UpdateCalibrationRowFromGrid(DataGridViewRow gridRow, CalibrationResultRow row)
        {
            int k = row.SoKenh > 0 ? row.SoKenh : _currentKenhCount;
            var kenh = row.Kenh.ToArray();
            for (int i = 0; i < k && i < row.Kenh.Length; i++)
                kenh[i] = ReadCalibrationCell(gridRow, $"CalKenh{i + 1}", allowMissing: true);

            double giaTriDat = ReadCalibrationCell(gridRow, ColGiaTriDat);
            double giaTriChiThi = ReadCalibrationCell(gridRow, ColGiaTriChiThi);
            double giaTriTrungBinh = ReadCalibrationCell(gridRow, ColTrungBinh);
            double soHieuChinh = ReadCalibrationCell(gridRow, ColSoHieuChinh);
            double doOnDinh = ReadCalibrationCell(gridRow, ColDoOnDinh);
            double doDongDeu = ReadCalibrationCell(gridRow, ColDoDongDeu);
            double doKhongDamBao = ReadCalibrationCell(gridRow, ColDKDB);

            row.STT = gridRow.Index + 1;
            row.GiaTriDat = giaTriDat;
            row.GiaTriChiThi = giaTriChiThi;
            row.Kenh = kenh;
            row.GiaTriTrungBinh = giaTriTrungBinh;
            row.SoHieuChinh = soHieuChinh;
            row.DoOnDinh = doOnDinh;
            row.DoDongDeu = doDongDeu;
            row.DoKhongDamBao = doKhongDamBao;
        }

        private double ReadCalibrationCell(DataGridViewRow gridRow, string columnName, bool allowMissing = false)
        {
            if (!_gridCalibration.Columns.Contains(columnName))
            {
                if (allowMissing) return double.NaN;
                throw new InvalidOperationException($"Thiếu cột {columnName}.");
            }

            string text = gridRow.Cells[columnName].Value?.ToString() ?? string.Empty;
            text = text.Replace("±", string.Empty).Trim();
            if (text == "---" || text.Length == 0)
                return double.NaN;

            if (double.TryParse(text, out double value))
                return value;

            throw new FormatException($"Giá trị '{text}' không phải số hợp lệ.");
        }

        private void FormatCalibrationGridRow(DataGridViewRow gridRow, CalibrationResultRow row)
        {
            _updatingCalibrationGrid = true;
            try
            {
                gridRow.Cells[ColStt].Value = row.STT;
                gridRow.Cells[ColGiaTriDat].Value = row.GiaTriDat.ToString("F1");
                gridRow.Cells[ColGiaTriChiThi].Value = row.GiaTriChiThi.ToString("F1");

                int k = row.SoKenh > 0 ? row.SoKenh : _currentKenhCount;
                for (int i = 0; i < k && i < row.Kenh.Length; i++)
                {
                    string columnName = $"CalKenh{i + 1}";
                    if (!_gridCalibration.Columns.Contains(columnName)) continue;
                    gridRow.Cells[columnName].Value = double.IsNaN(row.Kenh[i])
                        ? "---"
                        : row.Kenh[i].ToString("F2");
                }

                gridRow.Cells[ColTrungBinh].Value = row.GiaTriTrungBinh.ToString("F2");
                gridRow.Cells[ColSoHieuChinh].Value = row.SoHieuChinh.ToString("F2");
                gridRow.Cells[ColDoOnDinh].Value = row.DoOnDinh.ToString("F2");
                gridRow.Cells[ColDoDongDeu].Value = row.DoDongDeu.ToString("F2");
                gridRow.Cells[ColDKDB].Value = $"±{row.DoKhongDamBao:F2}";
            }
            finally
            {
                _updatingCalibrationGrid = false;
            }
        }

        private async Task SaveCalibrationRowEditAsync(CalibrationResultRow row)
        {
            if (!_currentSessionId.HasValue)
                return;

            await DatabaseService.EnsureSchemaAsync();
            row.Id = await DatabaseService.LuuKetQuaHieuChuanAsync(_currentSessionId.Value, row);
        }

        // ── Xoá dòng được chọn ───────────────────────────────────────────

        private async Task DeleteSelectedCalibRowAsync()
        {
            if (_gridCalibration.SelectedRows.Count == 0) return;

            var selectedRow = _gridCalibration.SelectedRows[0];
            var rowData = selectedRow.Tag as CalibrationResultRow;

            var confirm = MessageBox.Show(
                $"Xoá điểm kiểm tra STT = {selectedRow.Cells[ColStt].Value}?",
                "Xác nhận xoá",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                // Xoá khỏi DB nếu có session và có Id
                if (_currentSessionId.HasValue && rowData != null && rowData.Id > 0)
                {
                    await DatabaseService.XoaKetQuaHieuChuanAsync(
                        _currentSessionId.Value, rowData.STT);
                }

                // Xoá khỏi grid và cập nhật lại STT
                int deletedIndex = selectedRow.Index;
                _gridCalibration.Rows.RemoveAt(deletedIndex);

                // Cập nhật lại STT hiển thị
                for (int i = deletedIndex; i < _gridCalibration.Rows.Count; i++)
                {
                    _gridCalibration.Rows[i].Cells[ColStt].Value = i + 1;
                    if (_gridCalibration.Rows[i].Tag is CalibrationResultRow r)
                        r.STT = i + 1;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xoá dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Load từ DB ───────────────────────────────────────────────────

        private async Task TryLoadCalibrationResultsAsync()
        {
            if (!_currentSessionId.HasValue) return;

            try
            {
                var rows = await DatabaseService.LayKetQuaHieuChuanAsync(_currentSessionId.Value);
                if (rows.Count == 0) return;

                // Xác định k từ dữ liệu đã lưu
                int k = rows.Max(r => r.SoKenh);
                if (k < 1) k = _currentKenhCount;

                if (InvokeRequired)
                    Invoke(new Action(() => LoadCalibrationRowsToGrid(rows, k)));
                else
                    LoadCalibrationRowsToGrid(rows, k);
            }
            catch (Exception ex)
            {
                AppLogger.Warning("CalibrationResults", "Không thể load kết quả hiệu chuẩn", ex);
            }
        }

        private void LoadCalibrationRowsToGrid(
            System.Collections.Generic.List<CalibrationResultRow> rows, int k)
        {
            _gridCalibration.Rows.Clear();
            _currentKenhCount = k;
            RebuildCalibrationColumns(k);
            if (numKenhCount.Value != k)
                numKenhCount.Value = Math.Min(numKenhCount.Maximum, Math.Max(numKenhCount.Minimum, k));

            foreach (var row in rows)
                AddRowToCalibrationGrid(row);

            // Xoá màu highlight sau khi load
            foreach (DataGridViewRow r in _gridCalibration.Rows)
                r.DefaultCellStyle.BackColor = Color.Empty;

        }

        internal async Task EnsureCalibrationGridSavedAsync()
        {
            if (!_currentSessionId.HasValue)
                return;

            await DatabaseService.EnsureSchemaAsync();

            int savedCount = 0;
            foreach (DataGridViewRow gridRow in _gridCalibration.Rows)
            {
                if (gridRow.Tag is not CalibrationResultRow row)
                    continue;

                row.STT = gridRow.Index + 1;
                row.Id = await DatabaseService.LuuKetQuaHieuChuanAsync(_currentSessionId.Value, row);

                if (row.Id > 0 && row.ChiTietLanDos?.Count > 0)
                    await DatabaseService.LuuChiTietLanDoAsync(row.Id, row.ChiTietLanDos);

                savedCount++;
            }

        }

        // ── Public helper cho Form1 gọi khi session thay đổi ─────────────

        /// <summary>
        /// Gọi khi tạo session mới — xoá grid và reset trạng thái.
        /// </summary>
        internal void ResetCalibrationResults()
        {
            _gridCalibration.Rows.Clear();
            if (_uncertaintyForm != null && !_uncertaintyForm.IsDisposed)
            {
                _forceCloseUncertaintyForm = true;
                try
                {
                    _uncertaintyForm.Close();
                }
                finally
                {
                    _forceCloseUncertaintyForm = false;
                }
            }
        }

        /// <summary>
        /// Gọi sau khi session ID được gán — load dữ liệu từ DB.
        /// </summary>
        internal void OnSessionIdAssigned()
        {
            _ = TryLoadCalibrationResultsAsync();
        }
    }
}
