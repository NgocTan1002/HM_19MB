using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    // Partial class chứa logic lưu dữ liệu cải tiến
    public partial class Form1
    {
        // ── Auto-save state ──────────────────────────────────────────────────
        private bool _autoSaveEnabled = true; // Mặc định BẬT tự động lưu
        private int _autoSaveIntervalSeconds = 5; // Lưu mỗi 5 giây
        private DateTime _lastAutoSave = DateTime.MinValue;
        private readonly List<MeasurementBlock> _pendingRecords = new List<MeasurementBlock>();
        private System.Threading.Timer? _autoSaveTimer;
        private readonly object _pendingLock = new object();

        // ── Cấu hình lưu dữ liệu ─────────────────────────────────────────────
        
        /// <summary>
        /// Bật/tắt chế độ tự động lưu dữ liệu
        /// </summary>
        public void SetAutoSave(bool enabled, int intervalSeconds = 60)
        {
            _autoSaveEnabled = enabled;
            _autoSaveIntervalSeconds = intervalSeconds;

            if (enabled)
            {
                // Tạo timer để tự động lưu
                _autoSaveTimer?.Dispose();
                _autoSaveTimer = new System.Threading.Timer(
                    async _ => await AutoSaveTimerCallback(),
                    null,
                    TimeSpan.FromSeconds(intervalSeconds),
                    TimeSpan.FromSeconds(intervalSeconds)
                );
            }
            else
            {
                _autoSaveTimer?.Dispose();
                _autoSaveTimer = null;
            }
        }

        /// <summary>
        /// Callback được gọi định kỳ để tự động lưu dữ liệu
        /// </summary>
        private async Task AutoSaveTimerCallback()
        {
            if (!_autoSaveEnabled || _pendingRecords.Count == 0)
                return;

            try
            {
                await SavePendingRecordsAsync();
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không hiển thị MessageBox (vì đang chạy background)
                Console.WriteLine($"Auto-save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Lưu tất cả các bản ghi đang chờ vào database
        /// </summary>
        private async Task SavePendingRecordsAsync()
        {
            List<MeasurementBlock> recordsToSave;
            
            lock (_pendingLock)
            {
                if (_pendingRecords.Count == 0)
                    return;

                recordsToSave = _pendingRecords.ToList();
                _pendingRecords.Clear();
            }

            try
            {
                await DatabaseService.EnsureSchemaAsync();

                // Tạo session nếu chưa có
                if (_currentSessionId == null)
                {
                    var meta = CollectMetadata();
                    _currentSessionId = await DatabaseService.InsertSessionAsync(meta);
                    
                    // Cập nhật UI
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            _lblStatus.Text = $"Đã tạo session mới (ID: {_currentSessionId})";
                            _lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                        }));
                    }
                }

                // Lưu tất cả các bản ghi
                int savedCount = 0;
                foreach (var record in recordsToSave)
                {
                    await DatabaseService.InsertMeasurementRecordAsync(_currentSessionId.Value, record);
                    savedCount++;
                }

                _lastAutoSave = DateTime.Now;

                // Cập nhật UI
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        _lblStatus.Text = $"Đã lưu {savedCount} bản ghi vào DB (Session: {_currentSessionId})";
                        _lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                    }));
                }
            }
            catch (Exception ex)
            {
                // Đưa các bản ghi trở lại queue nếu lưu thất bại
                lock (_pendingLock)
                {
                    _pendingRecords.InsertRange(0, recordsToSave);
                }
                throw;
            }
        }

        /// <summary>
        /// Thêm bản ghi vào hàng đợi để lưu
        /// </summary>
        private void QueueRecordForSaving(MeasurementBlock block)
        {
            lock (_pendingLock)
            {
                _pendingRecords.Add(block);
            }

            // Lưu ngay lập tức nếu auto-save bật
            if (_autoSaveEnabled)
            {
                Task.Run(async () => 
                {
                    try
                    {
                        await SavePendingRecordsAsync();
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi
                        Console.WriteLine($"Auto-save error: {ex.Message}");
                        
                        // Hiển thị thông báo lỗi trên UI thread
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                _lblStatus.Text = $"Lỗi lưu dữ liệu: {ex.Message}";
                                _lblStatus.ForeColor = System.Drawing.Color.Red;
                            }));
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Lưu ngay lập tức bản ghi hiện tại (nút "Lưu dữ liệu")
        /// </summary>
        private async Task SaveCurrentRecordAsync()
        {
            if (_lastBlock == null)
            {
                MessageBox.Show("Chưa có dữ liệu để lưu.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await DatabaseService.EnsureSchemaAsync();

                var meta = CollectMetadata();
                if (_currentSessionId == null)
                    _currentSessionId = await DatabaseService.InsertSessionAsync(meta);

                await DatabaseService.InsertMeasurementRecordAsync(_currentSessionId.Value, _lastBlock);
                
                MessageBox.Show("Lưu dữ liệu thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Bắt đầu session mới (reset session ID)
        /// </summary>
        public void StartNewSession()
        {
            _currentSessionId = null;
            _pendingRecords.Clear();
            _lastAutoSave = DateTime.MinValue;
        }

        /// <summary>
        /// Lưu tất cả dữ liệu đang chờ trước khi đóng ứng dụng
        /// </summary>
        private async Task SaveAllPendingBeforeCloseAsync()
        {
            if (_pendingRecords.Count > 0)
            {
                var result = MessageBox.Show(
                    $"Có {_pendingRecords.Count} bản ghi chưa được lưu. Bạn có muốn lưu trước khi thoát?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await SavePendingRecordsAsync();
                        MessageBox.Show("Đã lưu tất cả dữ liệu thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi lưu dữ liệu:\n{ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
