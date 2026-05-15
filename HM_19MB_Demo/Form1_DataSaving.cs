using HM_19MB_Demo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Wrapper class cho MeasurementBlock với thông tin retry
    /// </summary>
    internal class PendingRecord
    {
        public MeasurementBlock Block { get; set; }
        public int RetryCount { get; set; }
        public DateTime FirstQueued { get; set; }

        public PendingRecord(MeasurementBlock block)
        {
            Block = block;
            RetryCount = 0;
            FirstQueued = DateTime.Now;
        }
    }

    // Partial class chứa logic lưu dữ liệu cải tiến
    public partial class Form1
    {
        // ── Auto-save state ──────────────────────────────────────────────────
        private bool _autoSaveEnabled = true; // Mặc định BẬT tự động lưu
        private int _autoSaveIntervalSeconds = 5; // Lưu mỗi 5 giây
        private DateTime _lastAutoSave = DateTime.MinValue;
        private readonly List<PendingRecord> _pendingRecords = new List<PendingRecord>();
        private readonly List<PendingRecord> _deadLetterQueue = new List<PendingRecord>();
        private System.Threading.Timer? _autoSaveTimer;
        private readonly object _pendingLock = new object();

        /// <summary>
        /// Số lượng bản ghi đã thất bại vĩnh viễn (vượt quá 3 lần retry)
        /// </summary>
        public int DeadLetterCount => _deadLetterQueue.Count;

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
                AppLogger.Error("DataSaving", $"Auto-save error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lưu tất cả các bản ghi đang chờ vào database
        /// </summary>
        private async Task SavePendingRecordsAsync()
        {
            List<PendingRecord> recordsToSave;
            
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
                    _currentSessionId = await DatabaseService.TaoPhienMoiAsync(meta);

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
                    await DatabaseService.LuuKetQuaDoAsync(_currentSessionId.Value, record.Block);
                    savedCount++;
                }

                _lastAutoSave = DateTime.Now;

            }
            catch (Exception ex)
            {
                // Xử lý retry logic: chỉ đưa lại những record có RetryCount < 3
                List<PendingRecord> toRetry = new List<PendingRecord>();
                List<PendingRecord> toDeadLetter = new List<PendingRecord>();

                foreach (var record in recordsToSave)
                {
                    record.RetryCount++;

                    if (record.RetryCount < 3)
                    {
                        toRetry.Add(record);
                    }
                    else
                    {
                        // Vượt quá 3 lần retry → chuyển vào dead letter queue
                        toDeadLetter.Add(record);
                        AppLogger.Error("DataSaving", 
                            $"Record failed after {record.RetryCount} retries. " +
                            $"Device: {record.Block.DeviceId}, " +
                            $"Timestamp: {record.Block.Timestamp:yyyy-MM-dd HH:mm:ss}, " +
                            $"FirstQueued: {record.FirstQueued:yyyy-MM-dd HH:mm:ss}", 
                            ex);
                    }
                }

                lock (_pendingLock)
                {
                    // Đưa các record còn retry được trở lại queue
                    _pendingRecords.InsertRange(0, toRetry);

                    // Thêm các record thất bại vĩnh viễn vào dead letter queue
                    _deadLetterQueue.AddRange(toDeadLetter);
                }

                // Cập nhật UI nếu có record bị dead letter
                if (_deadLetterQueue.Count > 0)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            _lblStatus.Text = $"CẢNH BÁO: {_deadLetterQueue.Count} bản ghi lưu thất bại vĩnh viễn!";
                            _lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                        }));
                    }
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
                _pendingRecords.Add(new PendingRecord(block));
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
                        AppLogger.Error("DataSaving", $"Auto-save error: {ex.Message}", ex);
                        
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
        private async Task<bool> SaveCurrentRecordAsync()
        {
            if (_lastBlock == null)
            {
                MessageBox.Show("Chưa có dữ liệu để lưu.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                await DatabaseService.EnsureSchemaAsync();

                var meta = CollectMetadata();
                if (_currentSessionId == null)
                    _currentSessionId = await DatabaseService.TaoPhienMoiAsync(meta);

                await DatabaseService.LuuKetQuaDoAsync(_currentSessionId.Value, _lastBlock);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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
                        ToastNotification.ShowSuccess("Đã lưu tất cả dữ liệu thành công!");
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
