using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Panel hiển thị chấm tròn màu cho connection health indicator
    /// </summary>
    internal class HealthDotPanel : Panel
    {
        private Color _dotColor = Color.Red;

        public HealthDotPanel()
        {
            Width = 16;
            Height = 16;
            BackColor = Color.Transparent;
        }

        /// <summary>
        /// Màu của chấm tròn
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DotColor
        {
            get => _dotColor;
            set
            {
                if (_dotColor != value)
                {
                    _dotColor = value;
                    Invalidate(); // Trigger repaint
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Vẽ hình tròn
            using (var brush = new SolidBrush(_dotColor))
            {
                e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            }

            // Vẽ viền để nổi bật hơn
            using (var pen = new Pen(Color.FromArgb(100, Color.Black), 1))
            {
                e.Graphics.DrawEllipse(pen, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}
