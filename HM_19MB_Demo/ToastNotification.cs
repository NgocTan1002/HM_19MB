using System;
using System.Drawing;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Toast notification - thông báo nhỏ tự động đóng, không steal focus
    /// </summary>
    public class ToastNotification : Form
    {
        private readonly System.Windows.Forms.Timer _closeTimer;
        private readonly System.Windows.Forms.Timer _fadeTimer;
        private readonly int _duration;
        private const int FadeDuration = 300; // milliseconds
        private const int FadeSteps = 15;

        private ToastNotification(string message, Color backgroundColor, string icon, int duration)
        {
            _duration = duration;

            // Form settings - không steal focus
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Width = 320;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = backgroundColor;
            Padding = new Padding(16);
            Opacity = 0.95;

            // Không steal focus khi show

            // Layout panel
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                Margin = new Padding(0),
                WrapContents = false
            };

            // Icon label
            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 16F, FontStyle.Regular),
                AutoSize = true,
                Margin = new Padding(0, 0, 12, 0),
                ForeColor = Color.White
            };

            // Message label
            var messageLabel = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                AutoSize = true,
                MaximumSize = new Size(260, 0),
                ForeColor = Color.White,
                Margin = new Padding(0)
            };

            panel.Controls.Add(iconLabel);
            panel.Controls.Add(messageLabel);
            Controls.Add(panel);

            // Position at bottom-right corner
            Load += (s, e) =>
            {
                var screen = Screen.FromPoint(Cursor.Position);
                var workingArea = screen.WorkingArea;
                Location = new Point(
                    workingArea.Right - Width - 16,
                    workingArea.Bottom - Height - 16
                );
            };

            // Close timer
            _closeTimer = new System.Windows.Forms.Timer { Interval = duration - FadeDuration };
            _closeTimer.Tick += (s, e) =>
            {
                _closeTimer.Stop();
                StartFadeOut();
            };

            // Fade timer
            _fadeTimer = new System.Windows.Forms.Timer { Interval = FadeDuration / FadeSteps };
            _fadeTimer.Tick += FadeTimer_Tick;

            // Click to close
            Click += (s, e) => Close();
            panel.Click += (s, e) => Close();
            iconLabel.Click += (s, e) => Close();
            messageLabel.Click += (s, e) => Close();
        }

        private void StartFadeOut()
        {
            _fadeTimer.Start();
        }

        private void FadeTimer_Tick(object? sender, EventArgs e)
        {
            Opacity -= (0.95 / FadeSteps);
            if (Opacity <= 0)
            {
                _fadeTimer.Stop();
                Close();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _closeTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _closeTimer?.Dispose();
                _fadeTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        // Prevent stealing focus
        protected override bool ShowWithoutActivation => true;

        // Static factory methods
        /// <summary>
        /// Hiển thị toast thông báo thành công (màu xanh)
        /// </summary>
        public static void ShowSuccess(string message, int duration = 3000)
        {
            Show(message, Color.FromArgb(40, 167, 69), "✅", duration);
        }

        /// <summary>
        /// Hiển thị toast cảnh báo (màu vàng)
        /// </summary>
        public static void ShowWarning(string message, int duration = 4000)
        {
            Show(message, Color.FromArgb(255, 193, 7), "⚠️", duration);
        }

        /// <summary>
        /// Hiển thị toast lỗi (màu đỏ)
        /// </summary>
        public static void ShowError(string message, int duration = 5000)
        {
            Show(message, Color.FromArgb(220, 53, 69), "❌", duration);
        }

        private static void Show(string message, Color backgroundColor, string icon, int duration)
        {
            // Phải chạy trên UI thread
            if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
            {
                Application.OpenForms[0].Invoke(new Action(() => Show(message, backgroundColor, icon, duration)));
                return;
            }

            var toast = new ToastNotification(message, backgroundColor, icon, duration);
            toast.Show();
        }
    }
}
