using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    public partial class ProbeGuideForm : Form
    {
        private PictureBox _picBox;
        private Label _lblDesc;
        private Button _btnClose;
            public ProbeGuideForm(int probeCount)
            {
                Text = $"Hướng dẫn đặt {probeCount} đầu đo";
                Size = new Size(700, 580);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 3,
                    Padding = new Padding(12)
                };
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));   // tiêu đề
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // ảnh
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));   // mô tả + nút

                // Tiêu đề
                var lblTitle = new Label
                {
                    Text = probeCount switch
                    {
                        9 => "Sơ đồ a) — Thể tích > 0,5 m³ — 9 đầu đo",
                        5 => "Sơ đồ b) — Thể tích ≤ 0,5 m³ — 5 đầu đo",
                        3 => "Sơ đồ c) — Thể tích ≤ 0,5 m³ — 3 đầu đo",
                        _ => "Hướng dẫn đặt đầu đo"
                    },
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                // Ảnh
                _picBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = probeCount switch
                    {
                        9 => Properties.Resources.probe_9,
                        5 => Properties.Resources.probe_5,
                        3 => Properties.Resources.probe_3,
                        _ => null
                    }
                };

                // Mô tả
                _lblDesc = new Label
                {
                    Text = probeCount switch
                    {
                        9 => "• Đầu đo 1–8: đặt tại 8 góc hộp\n" +
                             "• Đầu đo 9: đặt tại tâm hình học\n" +
                             "• Cách thành tủ: 50–60 mm (hoặc 1/10 cạnh tủ)",
                        5 => "• Đầu đo 1–4: đặt tại 4 góc chéo nhau qua không gian\n" +
                             "• Đầu đo 5: đặt tại tâm hình học\n" +
                             "• Cách thành tủ: 50–60 mm (hoặc 1/10 cạnh tủ)",
                        3 => "• Đầu đo 1, 3: đặt tại 2 góc chéo xa nhất\n" +
                             "• Đầu đo 2: đặt tại tâm hình học\n" +
                             "• Cách thành tủ: 50–60 mm (hoặc 1/10 cạnh tủ)",
                        _ => ""
                    },
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 9.5F),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(8, 0, 0, 0)
                };

                _btnClose = new Button
                {
                    Text = "Đóng",
                    Size = new Size(90, 32),
                    Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                    DialogResult = DialogResult.OK
                };

                var bottomPanel = new Panel { Dock = DockStyle.Fill };
                bottomPanel.Controls.Add(_lblDesc);
                bottomPanel.Controls.Add(_btnClose);
                _btnClose.Location = new Point(bottomPanel.Width - 102, 24);
                _btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;

                layout.Controls.Add(lblTitle, 0, 0);
                layout.Controls.Add(_picBox, 0, 1);
                layout.Controls.Add(bottomPanel, 0, 2);
                Controls.Add(layout);

                AcceptButton = _btnClose;
            }
        }
}
