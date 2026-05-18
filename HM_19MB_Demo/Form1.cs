using HM_19MB_Demo.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HM_19MB_Demo
{
    public partial class Form1 : Form
    {
        // ── Serial ───────────────────────────────────────────────────────────
        private readonly SerialReader _serialReader = new SerialReader();
        private MeasurementBlock? _lastBlock;
        private DateTime _lastDataReceivedTime = DateTime.MinValue;

        // ── Connection Health Indicator ──────────────────────────────────────
        private HealthDotPanel? _pnlHealthDot;
        private Label? _lblLastDataAge;
        private System.Windows.Forms.Timer? _healthTimer;

        // ── Chart history ────────────────────────────────────────────────────
        private const int MaxChartPoints = 720;
        private readonly List<double>[] _probeHistory = new List<double>[10];
        private readonly List<double>[] _humidityHistory = new List<double>[10];
        private readonly List<double> _avgHistory = new List<double>();
        private readonly List<double> _avgHumidityHistory = new List<double>();
        private readonly List<string> _timeLabels = new List<string>();

        // ── Session state ────────────────────────────────────────────────────
        private int? _currentSessionId = null;
        private bool IsHumidityMeasurementEnabled => _chkHumidity.Checked;

        // ── Probe colors (referenced by Designer) ────────────────────────────
        private static readonly Color[] ProbeColors =
        {
            Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple,
            Color.Teal, Color.Brown, Color.Magenta, Color.DarkCyan, Color.DarkGreen
        };

        // ────────────────────────────────────────────────────────────────────
        public Form1()
        {
            for (int i = 0; i < 10; i++)
            {
                _probeHistory[i] = new List<double>();
                _humidityHistory[i] = new List<double>();
            }
            InitializeComponent();
            InitializeChartSeries();
            InitializeGridContent();
            WireEvents();
            RefreshPorts();
            InitializeDefaultValues();

            // Hiển thị trạng thái auto-save
            _lblStatus.Text = "Chưa kết nối";
            _lblStatus.ForeColor = Color.DarkBlue;
        }

        // ── Initialize default values ────────────────────────────────────────
        private void InitializeDefaultValues()
        {
            // Tự động điền ngày tháng năm hiện tại
            DateTime now = DateTime.Now;
            txtCalibDay.Text = now.Day.ToString();
            txtCalibMonth.Text = now.Month.ToString();
            txtCalibYear.Text = now.Year.ToString();
        }

        // ── Form Load — safe place to set SplitterDistance ──────────────────
        private void Form1_Load(object? sender, EventArgs e)
        {
            int min = _split.Panel1MinSize;
            int max = _split.Width - _split.Panel2MinSize;
            int desired = _split.Width / 2;
            if (max > min)
                _split.SplitterDistance = Math.Max(min, Math.Min(desired, max));

            // Điều chỉnh chiều cao các dòng trong grid để fill toàn bộ
            AdjustRowHeights();
        }

        // ── Event wiring ─────────────────────────────────────────────────────
        private void WireEvents()
        {
            _serialReader.BlockReceived += SerialReader_BlockReceived;
            _serialReader.ErrorOccurred += SerialReader_ErrorOccurred;

            _btnConnect.Click += BtnConnect_Click;
            _btnExport.Click += BtnExport_Click;
            _btnUncertainty.Click += BtnUncertainty_Click;

            // Điều chỉnh lại chiều cao các dòng khi resize
            _grid.SizeChanged += (s, e) => AdjustRowHeights();

            // ── Initialize Connection Health Indicator ───────────────────────
            InitializeHealthIndicator();

            this.Load += Form1_Load;
            this.FormClosing += async (_, e) =>
            {
                // Lưu dữ liệu chưa lưu trước khi đóng
                if (_pendingRecords.Count > 0)
                {
                    e.Cancel = true;
                    await SaveAllPendingBeforeCloseAsync();
                    _autoSaveTimer?.Dispose();
                    _healthTimer?.Dispose();
                    _serialReader.Dispose();
                    Application.Exit();
                }
                else
                {
                    _autoSaveTimer?.Dispose();
                    _healthTimer?.Dispose();
                    _serialReader.Dispose();
                }
            };
            // Khởi tạo bảng kết quả hiệu chuẩn
            InitializeCalibrationResultsPanel();
        }

        // ── Connection Health Indicator ──────────────────────────────────────
        private void InitializeHealthIndicator()
        {
            // Tạo health dot panel
            _pnlHealthDot = new HealthDotPanel
            {
                Width = 16,
                Height = 16,
                DotColor = Color.Red,
                Margin = new Padding(18, 12, 8, 0)
            };

            // Tạo label hiển thị thời gian nhận dữ liệu cuối
            _lblLastDataAge = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.Gray,
                Text = "Không có dữ liệu",
                Margin = new Padding(0, 13, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Thêm vào bottomFlow (sau _lblStatus)
            int statusIndex = bottomFlow.Controls.IndexOf(_lblStatus);
            if (statusIndex >= 0)
            {
                bottomFlow.Controls.Add(_pnlHealthDot);
                bottomFlow.Controls.Add(_lblLastDataAge);
                bottomFlow.Controls.SetChildIndex(_pnlHealthDot, statusIndex + 1);
                bottomFlow.Controls.SetChildIndex(_lblLastDataAge, statusIndex + 2);
            }

            // Tạo timer để cập nhật health status
            _healthTimer = new System.Windows.Forms.Timer
            {
                Interval = 5000 // 5 giây
            };
            _healthTimer.Tick += HealthTimer_Tick;
        }

        private void HealthTimer_Tick(object? sender, EventArgs e)
        {
            if (_lastBlock == null || _lastDataReceivedTime == DateTime.MinValue)
            {
                // Chưa có dữ liệu
                if (_pnlHealthDot != null) _pnlHealthDot.DotColor = Color.Red;
                if (_lblLastDataAge != null) _lblLastDataAge.Text = "Không có dữ liệu";
                return;
            }

            // Tính khoảng cách thời gian từ lần nhận cuối (dùng thời gian máy tính, không phải thiết bị)
            var timeSinceLastData = DateTime.Now - _lastDataReceivedTime;
            int secondsAgo = (int)timeSinceLastData.TotalSeconds;

            if (secondsAgo < 10)
            {
                // < 10s → xanh lá, "Nhận: vừa xong"
                if (_pnlHealthDot != null) _pnlHealthDot.DotColor = Color.LimeGreen;
                if (_lblLastDataAge != null) _lblLastDataAge.Text = "Nhận: vừa xong";
            }
            else if (secondsAgo <= 60)
            {
                // 10s-60s → vàng, "Nhận: Ns trước"
                if (_pnlHealthDot != null) _pnlHealthDot.DotColor = Color.Gold;
                if (_lblLastDataAge != null) _lblLastDataAge.Text = $"Nhận: {secondsAgo}s trước";
            }
            else
            {
                // > 60s → đỏ, "Không có dữ liệu"
                if (_pnlHealthDot != null) _pnlHealthDot.DotColor = Color.Red;
                if (_lblLastDataAge != null) _lblLastDataAge.Text = "Không có dữ liệu";
            }
        }

        // ── Serial events ─────────────────────────────────────────────────────
        private void SerialReader_BlockReceived(object? sender, MeasurementBlock block)
        {
            if (InvokeRequired) { Invoke(new Action(() => SerialReader_BlockReceived(sender, block))); return; }

            _lastBlock = block;
            _lastDataReceivedTime = DateTime.Now; // Lưu thời gian nhận dữ liệu thực tế

            // Thêm vào hàng đợi để lưu tự động (nếu bật)
            if (_autoSaveEnabled)
            {
                QueueRecordForSaving(block);
            }

            UpdateGrid(block);
            UpdateChart(block);
            _lblLastReceived.Text = $"Lần nhận cuối: {block.Timestamp:HH:mm dd/MM/yyyy}  |  Thiết bị: {block.DeviceId}";
            _lblStatus.Text = $"Đang kết nối — nhận lúc {DateTime.Now:HH:mm}";
        }

        private void SerialReader_ErrorOccurred(object? sender, string msg)
        {
            if (InvokeRequired) { Invoke(new Action(() => SerialReader_ErrorOccurred(sender, msg))); return; }
            _lblStatus.Text = $"Lỗi: {msg}";
            _lblStatus.ForeColor = Color.DarkRed;
        }

        // ── Button handlers ───────────────────────────────────────────────────
        private void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (_serialReader.IsConnected)
            {
                try { _serialReader.Disconnect(); }
                catch (Exception ex)
                {
                    AppLogger.Warning("Form1", "Error disconnecting serial port", ex);
                }
                _btnConnect.Text = "Kết nối";
                _lblStatus.Text = "Đã ngắt kết nối";
                _lblStatus.ForeColor = Color.DarkRed;

                // Dừng health timer khi ngắt kết nối
                _healthTimer?.Stop();
                _lastDataReceivedTime = DateTime.MinValue;
                if (_pnlHealthDot != null) _pnlHealthDot.DotColor = Color.Red;
                if (_lblLastDataAge != null) _lblLastDataAge.Text = "Không có dữ liệu";
            }
            else
            {
                if (_cmbPort.SelectedItem is not string portName || string.IsNullOrEmpty(portName))
                {
                    MessageBox.Show("Vui lòng chọn cổng COM.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    _serialReader.Connect(portName);
                    _btnConnect.Text = "Ngắt kết nối";
                    _lblStatus.Text = $"Đã kết nối {portName} @ 9600";
                    _lblStatus.ForeColor = Color.DarkGreen;

                    // Bắt đầu health timer khi kết nối
                    _healthTimer?.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở cổng {portName}:\n{ex.Message}",
                        "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private async void BtnExport_Click(object? sender, EventArgs e)
        {
            // Lưu dữ liệu đo thô chưa lưu
            try
            {
                await DatabaseService.EnsureSchemaAsync();

                if (_currentSessionId == null)
                {
                    _currentSessionId = await DatabaseService.TaoPhienMoiAsync(CollectMetadata());
                    OnSessionIdAssigned();
                }

                if (_pendingRecords.Count > 0)
                    await SavePendingRecordsAsync();

                await EnsureCalibrationGridSavedAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private async void BtnUncertainty_Click(object? sender, EventArgs e)
        {
            // Mở form tính toán độ không đảm bảo đo
            if (_currentSessionId == null)
            {
                try
                {
                    await DatabaseService.EnsureSchemaAsync();
                    _currentSessionId = await DatabaseService.TaoPhienMoiAsync(CollectMetadata());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể tạo phiên hiệu chuẩn:\n{ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            int phienId = _currentSessionId.Value;
            if (_uncertaintyForm != null && !_uncertaintyForm.IsDisposed)
            {
                _uncertaintyForm.BringToFront();
                _uncertaintyForm.Focus();
                return;
            }

            _uncertaintyForm = new UncertaintyCalculationForm(
                kenhCount: _currentKenhCount,
                measurementCount: _currentMeasurementCount,
                phienId: phienId,
                onResultAdded: OnCalibrationResultAdded,
                onConfigChanged: (k, n) => SetCalibrationConfig(k, n, clearRowsOnChannelChange: true));
            _uncertaintyForm.FormClosed += (s, e) => _uncertaintyForm = null;
            _uncertaintyForm.Show(this);
        }

        private async Task HandleCalibrationResultAdded(int phienId, CalibrationResultRow row)
        {
            if (phienId <= 0)
            {
                MessageBox.Show("Chưa có phiên hiệu chuẩn để lưu kết quả.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                row.STT = await DatabaseService.LaySTTTiepTheoAsync(phienId);
                row.Id = await DatabaseService.LuuKetQuaHieuChuanAsync(phienId, row);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu kết quả hiệu chuẩn:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Grid update ───────────────────────────────────────────────────────
        private void UpdateGrid(MeasurementBlock block)
        {
            string t = block.Timestamp.ToString("HH:mm", CultureInfo.InvariantCulture);

            const bool showTemp = true;
            bool showHum = IsHumidityMeasurementEnabled;

            for (int i = 0; i < 10; i++)
            {
                bool hasTemp = i < block.ProbeCount && !float.IsNaN(block.ProbeTemperatures[i]);
                bool hasHum = showHum && i < block.ProbeCount && !float.IsNaN(block.ProbeHumidities[i]);
                _grid.Rows[i].Cells["NhietDo"].Value = showTemp
                    ? hasTemp ? block.ProbeTemperatures[i].ToString("F1", CultureInfo.InvariantCulture) : "---"
                    : "---";
                _grid.Rows[i].Cells["DoAm"].Value = showHum
                    ? hasHum ? block.ProbeHumidities[i].ToString("F1", CultureInfo.InvariantCulture) : "---"
                    : "---";
                _grid.Rows[i].Cells["ThoiGian"].Value = hasTemp || hasHum ? t : "---";
            }

            _grid.Rows[10].Cells["NhietDo"].Value = showTemp
                ? block.AvgTemperature.ToString("F1", CultureInfo.InvariantCulture) : "---";
            _grid.Rows[10].Cells["DoAm"].Value = showHum
                ? !float.IsNaN(block.AvgHumidity) ? block.AvgHumidity.ToString("F1", CultureInfo.InvariantCulture) : "---" : "---";
            _grid.Rows[10].Cells["ThoiGian"].Value = t;

            _grid.Rows[11].Cells["NhietDo"].Value = showTemp
                ? block.UniformityTemp.ToString("F1", CultureInfo.InvariantCulture) : "---";
            _grid.Rows[11].Cells["DoAm"].Value = showHum
                ? !float.IsNaN(block.UniformityHumidity) ? block.UniformityHumidity.ToString("F1", CultureInfo.InvariantCulture) : "---" : "---";
            _grid.Rows[11].Cells["ThoiGian"].Value = t;

            _grid.Rows[12].Cells["NhietDo"].Value = showTemp ? block.StabilityTemperature : "---";
            _grid.Rows[12].Cells["DoAm"].Value = showHum ? block.StabilityHumidity : "---";
            _grid.Rows[12].Cells["ThoiGian"].Value = t;
        }

        // ── Chart update ──────────────────────────────────────────────────────
        private void UpdateChart(MeasurementBlock block)
        {
            string label = block.Timestamp.ToString("HH:mm", CultureInfo.InvariantCulture);
            _timeLabels.Add(label);
            if (_timeLabels.Count > MaxChartPoints) _timeLabels.RemoveAt(0);

            const bool showTemp = true;
            bool showHum = IsHumidityMeasurementEnabled;

            // Lưu lịch sử
            for (int i = 0; i < 10; i++)
            {
                if (i < block.ProbeCount && !float.IsNaN(block.ProbeTemperatures[i]))
                {
                    _probeHistory[i].Add(block.ProbeTemperatures[i]);
                    if (_probeHistory[i].Count > MaxChartPoints) _probeHistory[i].RemoveAt(0);
                }

                if (showHum && i < block.ProbeCount && !float.IsNaN(block.ProbeHumidities[i]))
                {
                    _humidityHistory[i].Add(block.ProbeHumidities[i]);
                    if (_humidityHistory[i].Count > MaxChartPoints) _humidityHistory[i].RemoveAt(0);
                }
            }
            _avgHistory.Add(block.AvgTemperature);
            if (_avgHistory.Count > MaxChartPoints) _avgHistory.RemoveAt(0);

            if (showHum && !float.IsNaN(block.AvgHumidity))
            {
                _avgHumidityHistory.Add(block.AvgHumidity);
                if (_avgHumidityHistory.Count > MaxChartPoints) _avgHumidityHistory.RemoveAt(0);
            }

            // ── Vẽ series nhiệt độ (index 0–10) ─────────────────────────────────
            for (int i = 0; i < 10; i++)
            {
                _chart.Series[i].Points.Clear();
                if (showTemp)
                {
                    int offset = Math.Max(0, _timeLabels.Count - _probeHistory[i].Count);
                    for (int j = 0; j < _probeHistory[i].Count; j++)
                        _chart.Series[i].Points.AddXY(_timeLabels[offset + j], _probeHistory[i][j]);
                }
            }
            var avgTemp = _chart.Series["Trung bình"];
            avgTemp.Points.Clear();
            if (showTemp)
            {
                int avgTOffset = Math.Max(0, _timeLabels.Count - _avgHistory.Count);
                for (int j = 0; j < _avgHistory.Count; j++)
                    avgTemp.Points.AddXY(_timeLabels[avgTOffset + j], _avgHistory[j]);
            }

            // ── Vẽ series độ ẩm (index 11–21) ───────────────────────────────────
            for (int i = 0; i < 10; i++)
            {
                var s = _chart.Series[$"ĐA Đầu đo {i + 1}"];
                s.Points.Clear();
                if (showHum)
                {
                    int offset = Math.Max(0, _timeLabels.Count - _humidityHistory[i].Count);
                    for (int j = 0; j < _humidityHistory[i].Count; j++)
                        s.Points.AddXY(_timeLabels[offset + j], _humidityHistory[i][j]);
                }
            }
            var avgHum = _chart.Series["TB Độ ẩm"];
            avgHum.Points.Clear();
            if (showHum)
            {
                int avgHOffset = Math.Max(0, _timeLabels.Count - _avgHumidityHistory.Count);
                for (int j = 0; j < _avgHumidityHistory.Count; j++)
                    avgHum.Points.AddXY(_timeLabels[avgHOffset + j], _avgHumidityHistory[j]);
            }

            AutoScaleChartAxes();
        }

        private void RedrawChart()
        {
            const bool showTemp = true;
            bool showHum = IsHumidityMeasurementEnabled;

            // Vẽ lại series nhiệt độ
            for (int i = 0; i < 10; i++)
            {
                _chart.Series[i].Points.Clear();
                _chart.Series[i].Enabled = showTemp;
                if (showTemp)
                {
                    int offset = Math.Max(0, _timeLabels.Count - _probeHistory[i].Count);
                    for (int j = 0; j < _probeHistory[i].Count; j++)
                        _chart.Series[i].Points.AddXY(_timeLabels[offset + j], _probeHistory[i][j]);
                }
            }

            var avgTempSeries = _chart.Series["Trung bình"];
            avgTempSeries.Points.Clear();
            avgTempSeries.Enabled = showTemp;
            if (showTemp)
            {
                int avgOffset = Math.Max(0, _timeLabels.Count - _avgHistory.Count);
                for (int j = 0; j < _avgHistory.Count; j++)
                    avgTempSeries.Points.AddXY(_timeLabels[avgOffset + j], _avgHistory[j]);
            }

            // Vẽ lại series độ ẩm
            for (int i = 0; i < 10; i++)
            {
                var s = _chart.Series[$"ĐA Đầu đo {i + 1}"];
                s.Points.Clear();
                s.Enabled = showHum;
                if (showHum)
                {
                    int offset = Math.Max(0, _timeLabels.Count - _humidityHistory[i].Count);
                    for (int j = 0; j < _humidityHistory[i].Count; j++)
                        s.Points.AddXY(_timeLabels[offset + j], _humidityHistory[i][j]);
                }
            }

            var avgHumSeries = _chart.Series["TB Độ ẩm"];
            avgHumSeries.Points.Clear();
            avgHumSeries.Enabled = showHum;
            if (showHum)
            {
                int avgOffset = Math.Max(0, _timeLabels.Count - _avgHumidityHistory.Count);
                for (int j = 0; j < _avgHumidityHistory.Count; j++)
                    avgHumSeries.Points.AddXY(_timeLabels[avgOffset + j], _avgHumidityHistory[j]);
            }

            // Cập nhật trục Y
            _chart.ChartAreas["MainArea"].AxisY.Enabled = showTemp ? AxisEnabled.True : AxisEnabled.False;
            _chart.ChartAreas["MainArea"].AxisY2.Enabled = showHum ? AxisEnabled.True : AxisEnabled.False;
            AutoScaleChartAxes();
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private void AutoScaleChartAxes()
        {
            var area = _chart.ChartAreas["MainArea"];
            area.AxisY.Minimum = double.NaN;
            area.AxisY.Maximum = double.NaN;
            area.AxisY2.Minimum = double.NaN;
            area.AxisY2.Maximum = double.NaN;
            area.RecalculateAxesScale();
        }

        private SessionMetadata CollectMetadata()
        {
            return new SessionMetadata
            {
                TenThietBi = txtDeviceName.Text,
                KyHieu = txtDeviceCode.Text,
                SoHieu = txtDeviceNumber.Text,
                SoTem = txtSealNumber.Text,
                NoiSanXuat = txtManufacturer.Text,
                NamSanXuat = txtManufactureYear.Text,
                DonViSuDung = txtUsingUnit.Text,
                PhuongPhap = txtMethod.Text,
                NhietDoMoiTruong = txtEnvTemp.Text,
                DoAmTuongDoi = txtEnvHumidity.Text,
                DacTinhKyThuat = txtTechnicalSpecs.Text,
                ThietBiChuan = txtMeasuringDevices.Text,
                NgayHieuChuan = int.TryParse(txtCalibDay.Text, out int day) &&
                           int.TryParse(txtCalibMonth.Text, out int month) &&
                           int.TryParse(txtCalibYear.Text, out int year)
                    ? new DateTime(year, month, day)
                    : DateTime.Today
            };
        }

        private void RefreshPorts()
        {
            _cmbPort.Items.Clear();
            foreach (var p in SerialReader.GetAvailablePorts())
                _cmbPort.Items.Add(p);
            if (_cmbPort.Items.Count > 0)
                _cmbPort.SelectedIndex = 0;
        }

        private void _chkDisplayFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (!_chkTemperature.Checked)
                _chkTemperature.Checked = true;

            const bool showTemp = true;
            bool showHum = IsHumidityMeasurementEnabled;

            // ── Cập nhật cột trong bảng ─────────────────────────────────
            _grid.Columns["NhietDo"].Visible = showTemp;
            _grid.Columns["DoAm"].Visible = showHum;

            // ── Cập nhật series nhiệt độ trên biểu đồ ──────────────────
            for (int i = 0; i < 10; i++)
                _chart.Series[i].Enabled = showTemp;
            _chart.Series["Trung bình"].Enabled = showTemp;

            _chart.ChartAreas["MainArea"].AxisY.Enabled = showTemp
                ? AxisEnabled.True
                : AxisEnabled.False;

            // ── Cập nhật series độ ẩm trên biểu đồ ─────────────────────
            for (int i = 0; i < 10; i++)
                _chart.Series[$"ĐA Đầu đo {i + 1}"].Enabled = showHum;
            _chart.Series["TB Độ ẩm"].Enabled = showHum;

            _chart.ChartAreas["MainArea"].AxisY2.Enabled = showHum
                ? AxisEnabled.True
                : AxisEnabled.False;

            // ── Cập nhật lại dữ liệu grid nếu đang có block ────────────
            if (_lastBlock != null)
                UpdateGrid(_lastBlock);
        }

        private void ShowProbeGuideDialog(int probeCount)
        {
            string message = probeCount switch
            {
                9 => "Thể tích > 0,5 m³ — Dùng sơ đồ a)\n\n" +
                     "• Đầu đo 1-8: đặt tại 8 góc của hộp\n" +
                     "• Đầu đo 9: đặt tại tâm hình học\n" +
                     "• Cách thành tủ: 50–60 mm (hoặc 1/10 cạnh tủ)",

                5 => "Thể tích ≤ 0,5 m³ — Dùng sơ đồ b)\n\n" +
                     "• Đầu đo 1-4: đặt tại 4 góc chéo nhau\n" +
                     "• Đầu đo 5: đặt tại tâm hình học\n" +
                     "• Cách thành tủ: 50–60 mm (hoặc 1/10 cạnh tủ)",

                3 => "Thể tích ≤ 0,5 m³ — Dùng sơ đồ c)\n\n" +
                     "• Đầu đo 1, 3: đặt tại 2 góc chéo xa nhất\n" +
                     "• Đầu đo 2: đặt tại tâm hình học\n" +
                     "• Cách thành tủ: 50–60 mm (hoặc 1/10 cạnh tủ)",

                _ => "Số đầu đo không hợp lệ. Chọn 3, 5 hoặc 9."
            };

            MessageBox.Show(message, $"Hướng dẫn đặt {probeCount} đầu đo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnShowGuide_Click(object sender, EventArgs e)
        {
            // Cho user chọn số đầu đo
            int probeCount = GetProbeCount();

            if (probeCount > 0)
            {
                using var guide = new ProbeGuideForm(probeCount);
                guide.ShowDialog(this);
            }
        }

        private int GetProbeCount()
        {
            // Tạo form chọn số đầu đo
            using var selectForm = new Form
            {
                Text = "Chọn sơ đồ đặt đầu đo",
                Size = new Size(500, 280),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 5,
                ColumnCount = 1
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Tiêu đề
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 15F)); // Khoảng cách
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Nút 1
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Nút 2
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Nút 3

            // Tiêu đề
            var lblTitle = new Label
            {
                Text = "Chọn số lượng đầu đo theo thể tích buồng nhiệt:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 0, 0, 10)
            };
            mainPanel.Controls.Add(lblTitle, 0, 0);

            int selectedCount = 0;

            // Nút 9 đầu đo
            var btn9 = new Button
            {
                Text = "9 đầu đo - Thể tích > 0,5 m³",
                Height = 45,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn9.Click += (s, e) => { selectedCount = 9; selectForm.DialogResult = DialogResult.OK; selectForm.Close(); };
            mainPanel.Controls.Add(btn9, 0, 2);

            // Nút 5 đầu đo
            var btn5 = new Button
            {
                Text = "5 đầu đo - Thể tích ≤ 0,5 m³",
                Height = 45,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Margin = new Padding(0, 8, 0, 0),
                Cursor = Cursors.Hand
            };
            btn5.Click += (s, e) => { selectedCount = 5; selectForm.DialogResult = DialogResult.OK; selectForm.Close(); };
            mainPanel.Controls.Add(btn5, 0, 3);

            // Nút 3 đầu đo
            var btn3 = new Button
            {
                Text = "3 đầu đo - Thể tích ≤ 0,5 m³",
                Height = 45,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Margin = new Padding(0, 8, 0, 0),
                Cursor = Cursors.Hand
            };
            btn3.Click += (s, e) => { selectedCount = 3; selectForm.DialogResult = DialogResult.OK; selectForm.Close(); };
            mainPanel.Controls.Add(btn3, 0, 4);

            selectForm.Controls.Add(mainPanel);

            return selectForm.ShowDialog() == DialogResult.OK ? selectedCount : 0;
        }

        private void mainLayout_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
