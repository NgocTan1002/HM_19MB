using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Custom Label render công thức toán học bằng GDI+.
    ///
    /// Cú pháp MathText:
    ///   FRAC{tử}{mẫu}     → phân số
    ///   SQRT{biểu thức}   → căn bậc hai
    ///   SUP{x}            → chỉ số trên
    ///   SUB{x}            → chỉ số dưới
    ///   BAR{x}            → gạch trên (trung bình)
    ///   _delta_           → δ
    ///   _Delta_           → Δ
    ///   _sigma_           → σ
    ///   _pm_              → ±
    ///   _cdot_            → ·
    ///   _sum_             → Σ
    ///
    /// Ví dụ:
    ///   "BAR{t}SUB{ch} = FRAC{1}{k} _cdot_ _sum_ BAR{t}SUB{j}"
    ///   "u SUB{ch1} = SQRT{u SUB{ch1}SUP{2} + u SUB{ch2}SUP{2}}"
    /// </summary>
    public class MathLabel : Control
    {
        private string _mathText = "";

        [Category("Math")]
        [Description("Biểu thức toán học hiển thị.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string MathText
        {
            get => _mathText;
            set { _mathText = value ?? ""; Invalidate(); RecalcSize(); }
        }

        private float _baseFontSize = 9f;

        [Category("Math")]
        [Description("Cỡ chữ cơ bản.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float BaseFontSize
        {
            get => _baseFontSize;
            set { _baseFontSize = value; Invalidate(); RecalcSize(); }
        }

        public MathLabel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);  // bật transparent
            // KHÔNG set BackColor = Transparent trong constructor —
            // để parent control vẽ background trước, tránh ArgumentException.
            ForeColor = Color.Black;
        }

        // Cho phép WinForms chấp nhận Color.Transparent
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        // Vẽ background của parent trước khi vẽ công thức (giả lập transparent)
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Không gọi base — để background trong suốt hoàn toàn
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Vẽ nền từ parent để tránh artifact
            if (Parent != null)
            {
                var pt = Parent.PointToClient(this.PointToScreen(Point.Empty));
                e.Graphics.TranslateTransform(-pt.X, -pt.Y);
                using var pea = new PaintEventArgs(e.Graphics, new System.Drawing.Rectangle(pt, Parent.Size));
                InvokePaintBackground(Parent, pea);
                InvokePaint(Parent, pea);
                e.Graphics.TranslateTransform(pt.X, pt.Y);
            }

            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var tokens = Tokenize(_mathText);
            float x = 2;
            float baseline = Height / 2f + _baseFontSize * 0.35f;
            RenderTokens(g, tokens, ref x, baseline, _baseFontSize, false, false);
        }

        // ── Tokenizer ────────────────────────────────────────────────────

        private static Token[] Tokenize(string text)
        {
            var list = new System.Collections.Generic.List<Token>();
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == ' ') { list.Add(new Token(TokenType.Space, "")); i++; continue; }

                if (StartsAt(text, i, "FRAC{"))
                {
                    i += 4;
                    string num = ReadBrace(text, ref i);
                    string den = ReadBrace(text, ref i);
                    list.Add(new Token(TokenType.Frac, num + "|" + den));
                    continue;
                }
                if (StartsAt(text, i, "SQRT{"))
                {
                    i += 4;
                    list.Add(new Token(TokenType.Sqrt, ReadBrace(text, ref i)));
                    continue;
                }
                if (StartsAt(text, i, "SUP{"))
                {
                    i += 3;
                    list.Add(new Token(TokenType.Sup, ReadBrace(text, ref i)));
                    continue;
                }
                if (StartsAt(text, i, "SUB{"))
                {
                    i += 3;
                    list.Add(new Token(TokenType.Sub, ReadBrace(text, ref i)));
                    continue;
                }
                if (StartsAt(text, i, "BAR{"))
                {
                    i += 3;
                    list.Add(new Token(TokenType.Bar, ReadBrace(text, ref i)));
                    continue;
                }

                // Ký tự đặc biệt _name_
                if (text[i] == '_')
                {
                    int end = text.IndexOf('_', i + 1);
                    if (end > i)
                    {
                        string sym = GreekOrSymbol(text.Substring(i + 1, end - i - 1));
                        list.Add(new Token(TokenType.Text, sym));
                        i = end + 1;
                        continue;
                    }
                }

                // Văn bản thông thường
                int start = i;
                while (i < text.Length &&
                       text[i] != ' ' &&
                       text[i] != '_' &&
                       !StartsAt(text, i, "FRAC{") &&
                       !StartsAt(text, i, "SQRT{") &&
                       !StartsAt(text, i, "SUP{") &&
                       !StartsAt(text, i, "SUB{") &&
                       !StartsAt(text, i, "BAR{"))
                {
                    i++;
                }
                if (i > start)
                    list.Add(new Token(TokenType.Text, text.Substring(start, i - start)));
            }
            return list.ToArray();
        }

        private static string ReadBrace(string text, ref int i)
        {
            if (i >= text.Length || text[i] != '{') return "";
            int depth = 1; i++;
            int start = i;
            while (i < text.Length && depth > 0)
            {
                if (text[i] == '{') depth++;
                else if (text[i] == '}') depth--;
                if (depth > 0) i++;
                else i++;
            }
            return text.Substring(start, i - start - 1);
        }

        private static bool StartsAt(string s, int i, string prefix)
        {
            if (i + prefix.Length > s.Length) return false;
            return string.Compare(s, i, prefix, 0, prefix.Length, StringComparison.Ordinal) == 0;
        }

        // FIX: so sánh trước khi .ToLower() để phân biệt Delta / delta
        private static string GreekOrSymbol(string name) => name switch
        {
            "delta" => "δ",
            "Delta" => "Δ",
            "sigma" => "σ",
            "Sigma" => "Σ",
            "sum" => "Σ",
            "alpha" => "α",
            "beta" => "β",
            "gamma" => "γ",
            "mu" => "μ",
            "pi" => "π",
            "sqrt" => "√",
            "pm" => "±",
            "times" => "×",
            "cdot" => "·",
            "approx" => "≈",
            "neq" => "≠",
            "leq" => "≤",
            "geq" => "≥",
            "inf" => "∞",
            "partial" => "∂",
            _ => name
        };

        // ── Renderer ─────────────────────────────────────────────────────

        private void RenderTokens(Graphics g, Token[] tokens, ref float x, float baseline,
                                   float fontSize, bool isSup, bool isSub)
        {
            float yOff = isSup ? -fontSize * 0.55f : (isSub ? fontSize * 0.28f : 0f);
            float fs = (isSup || isSub) ? fontSize * 0.68f : fontSize;

            using var fontReg = new Font("Segoe UI", fs, FontStyle.Regular, GraphicsUnit.Point);
            using var fontItal = new Font("Cambria Math", fs, FontStyle.Italic, GraphicsUnit.Point);
            using var brush = new SolidBrush(ForeColor);

            foreach (var tok in tokens)
            {
                switch (tok.Type)
                {
                    case TokenType.Space:
                        x += fs * 0.28f;
                        break;

                    case TokenType.Text:
                        // Biến đơn ký tự dùng italic; số và ký hiệu dùng regular
                        bool isVar = tok.Value.Length == 1 && char.IsLetter(tok.Value[0]);
                        var tf = isVar ? fontItal : fontReg;
                        var tsz = g.MeasureString(tok.Value, tf);
                        float ty = baseline + yOff - tsz.Height * 0.72f;
                        g.DrawString(tok.Value, tf, brush, x, ty);
                        x += tsz.Width - 3f;
                        break;

                    case TokenType.Frac:
                        x += RenderFrac(g, tok.Value, x, baseline + yOff, fontSize, brush);
                        break;

                    case TokenType.Sqrt:
                        x += RenderSqrt(g, tok.Value, x, baseline + yOff, fontSize);
                        break;

                    case TokenType.Sup:
                        {
                            var sub = Tokenize(tok.Value);
                            RenderTokens(g, sub, ref x, baseline, fontSize, true, false);
                            break;
                        }
                    case TokenType.Sub:
                        {
                            var sub = Tokenize(tok.Value);
                            RenderTokens(g, sub, ref x, baseline, fontSize, false, true);
                            break;
                        }
                    case TokenType.Bar:
                        x += RenderBar(g, tok.Value, x, baseline + yOff, fs, brush);
                        break;
                }
            }
        }

        private float RenderFrac(Graphics g, string fracStr, float x, float baseline,
                                  float fontSize, Brush brush)
        {
            int sep = fracStr.IndexOf('|');
            string ns = sep >= 0 ? fracStr.Substring(0, sep) : fracStr;
            string ds = sep >= 0 ? fracStr.Substring(sep + 1) : "1";

            float small = fontSize * 0.72f;
            float nw = MeasureTokens(g, Tokenize(ns), small);
            float dw = MeasureTokens(g, Tokenize(ds), small);
            float fw = Math.Max(nw, dw) + 8f;
            float lineY = baseline - fontSize * 0.08f;

            using var fn = new Font("Cambria Math", small, FontStyle.Regular, GraphicsUnit.Point);
            float lineH = g.MeasureString("X", fn).Height;

            // Tử số
            float nx = x + (fw - nw) / 2f;
            float numBase = lineY - 2f;
            var nt = Tokenize(ns);
            RenderTokens(g, nt, ref nx, numBase, small, false, false);

            // Mẫu số
            float dx = x + (fw - dw) / 2f;
            float denBase = lineY + lineH * 0.52f;
            var dt = Tokenize(ds);
            RenderTokens(g, dt, ref dx, denBase, small, false, false);

            // Gạch ngang
            using var pen = new Pen(ForeColor, 1f);
            g.DrawLine(pen, x, lineY, x + fw, lineY);

            return fw + 4f;
        }

        private float RenderSqrt(Graphics g, string inner, float x, float baseline, float fontSize)
        {
            var tokens = Tokenize(inner);
            float iw = MeasureTokens(g, tokens, fontSize);
            float lineY = baseline - fontSize * 0.75f;

            using var pen = new Pen(ForeColor, 1.2f);
            // Ký hiệu check √
            float cx = x;
            g.DrawLine(pen, cx, baseline - fontSize * 0.32f,
                            cx + fontSize * 0.22f, baseline);
            g.DrawLine(pen, cx + fontSize * 0.22f, baseline,
                            cx + fontSize * 0.42f, lineY);
            // Nét ngang trên
            g.DrawLine(pen, cx + fontSize * 0.42f, lineY,
                            cx + fontSize * 0.42f + iw + 4f, lineY);

            float ix = x + fontSize * 0.48f;
            RenderTokens(g, tokens, ref ix, baseline, fontSize, false, false);

            return fontSize * 0.42f + iw + 8f;
        }

        // Gạch trên cho BAR{x} — ký hiệu trung bình
        private float RenderBar(Graphics g, string inner, float x, float baseline,
                                 float fontSize, Brush brush)
        {
            var tokens = Tokenize(inner);
            float iw = MeasureTokens(g, tokens, fontSize);
            float startX = x;

            RenderTokens(g, tokens, ref x, baseline, fontSize, false, false);

            using var pen = new Pen(ForeColor, 1f);
            float barY = baseline - fontSize * 0.88f;
            g.DrawLine(pen, startX, barY, startX + iw, barY);

            return iw + 2f;
        }

        // ── Measure ──────────────────────────────────────────────────────

        private float MeasureTokens(Graphics g, Token[] tokens, float fontSize)
        {
            float w = 0f;
            using var font = new Font("Cambria Math", fontSize, FontStyle.Regular, GraphicsUnit.Point);
            foreach (var tok in tokens)
            {
                switch (tok.Type)
                {
                    case TokenType.Space:
                        w += fontSize * 0.28f; break;
                    case TokenType.Text:
                        w += g.MeasureString(tok.Value, font).Width - 3f; break;
                    case TokenType.Bar:
                        w += MeasureTokens(g, Tokenize(tok.Value), fontSize) + 2f; break;
                    case TokenType.Frac:
                        {
                            int sep = tok.Value.IndexOf('|');
                            string n = sep >= 0 ? tok.Value.Substring(0, sep) : tok.Value;
                            string d = sep >= 0 ? tok.Value.Substring(sep + 1) : "1";
                            float s = fontSize * 0.72f;
                            float nw = MeasureTokens(g, Tokenize(n), s);
                            float dw = MeasureTokens(g, Tokenize(d), s);
                            w += Math.Max(nw, dw) + 12f; break;
                        }
                    case TokenType.Sqrt:
                        w += fontSize * 0.42f + MeasureTokens(g, Tokenize(tok.Value), fontSize) + 8f; break;
                    case TokenType.Sup:
                    case TokenType.Sub:
                        w += MeasureTokens(g, Tokenize(tok.Value), fontSize * 0.68f); break;
                }
            }
            return w;
        }

        private void RecalcSize()
        {
            if (!IsHandleCreated) return;
            using var g = CreateGraphics();
            float w = MeasureTokens(g, Tokenize(_mathText), _baseFontSize);
            Width = (int)w + 24;
        }

        protected override void OnResize(EventArgs e) { base.OnResize(e); Invalidate(); }

        // ── Token model ──────────────────────────────────────────────────

        private enum TokenType { Text, Space, Frac, Sqrt, Sup, Sub, Bar }
        private struct Token
        {
            public readonly TokenType Type;
            public readonly string Value;
            public Token(TokenType t, string v) { Type = t; Value = v; }
        }
    }
}
