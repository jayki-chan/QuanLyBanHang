using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Cửa sổ chat với AI trợ lý – hiển thị dưới dạng floating tool window.
    /// Hỗ trợ truy vấn CSDL SQL Server và trả lời ngữ cảnh theo cấp bậc tài khoản.
    /// </summary>
    public class ChatWidget : Form
    {
        private AIService   _ai;
        private RichTextBox _rtbChat;
        private TextBox     _txtInput;
        private Button      _btnSend;
        private Label       _lblTyping;
        private bool        _isBusy;

        static readonly Color NavBlue  = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray   = Color.FromArgb(245, 246, 250);
        static readonly Color BotColor = Color.FromArgb(0, 105, 200);

        public ChatWidget(NhanVienDTO user)
        {
            _ai = new AIService(user);
            BuildUI();
            AppendBot(
                "Xin chào! Tôi là trợ lý AI của phần mềm Quản Lý Bán Hàng.\n\n" +
                "Tôi có thể giúp bạn:\n" +
                "  • Tra cứu dữ liệu thực tế (khách hàng, sản phẩm, hóa đơn...)\n" +
                "  • Giải thích cách sử dụng các tính năng của phần mềm\n" +
                "  • Thống kê / báo cáo nhanh theo yêu cầu\n" +
                "  • Tư vấn nghiệp vụ bán hàng\n\n" +
                "Hãy đặt câu hỏi bằng tiếng Việt!"
            );
        }

        private void BuildUI()
        {
            this.Text            = "Trợ lý AI – Quản Lý Bán Hàng";
            this.Size            = new Size(460, 620);
            this.MinimumSize     = new Size(360, 440);
            this.StartPosition   = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.BackColor       = BgGray;
            this.ShowInTaskbar   = false;

            // Đặt vị trí góc phải - dưới màn hình
            var wa = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(wa.Right - this.Width - 24, wa.Bottom - this.Height - 24);

            // ── Header ────────────────────────────────────────
            var titleBar = new Panel
            {
                Dock = DockStyle.Top, Height = 48,
                BackColor = NavBlue
            };
            var lblTitle = new Label
            {
                Text = "🤖  Trợ lý AI – Quản Lý Bán Hàng",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };

            var btnClear = new Button
            {
                Text      = "Xóa LS",
                Width     = 68,
                Height    = 32,
                Dock      = DockStyle.Right,
                BackColor = Color.FromArgb(55, 82, 140),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 8.5F),
                Cursor    = Cursors.Hand,
                Margin    = new Padding(0, 8, 10, 8)
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += (s, e) =>
            {
                _ai.ClearHistory();
                _rtbChat.Clear();
                AppendBot("Đã xóa lịch sử hội thoại. Tôi sẵn sàng hỗ trợ bạn!");
            };

            titleBar.Controls.Add(lblTitle);
            titleBar.Controls.Add(btnClear);

            // ── Chat display (RichTextBox) ─────────────────────
            _rtbChat = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                ReadOnly    = true,
                BorderStyle = BorderStyle.None,
                BackColor   = BgGray,
                Font        = new Font("Segoe UI", 9.5F),
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                Padding     = new Padding(10, 8, 10, 8),
                DetectUrls  = false,
                WordWrap    = true
            };

            // ── Typing indicator ───────────────────────────────
            var typingPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 22,
                BackColor = BgGray
            };
            _lblTyping = new Label
            {
                Text      = "  🔄 AI đang xử lý...",
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                ForeColor = Color.FromArgb(100, 130, 185),
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                Visible   = false
            };
            typingPanel.Controls.Add(_lblTyping);

            // ── Separator ──────────────────────────────────────
            var sep = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 1,
                BackColor = Color.FromArgb(210, 218, 232)
            };

            // ── Input panel ────────────────────────────────────
            var inputPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 52,
                BackColor = Color.White,
                Padding   = new Padding(8, 6, 8, 6)
            };

            _btnSend = new Button
            {
                Text      = "Gửi ➤",
                Width     = 72,
                Height    = 40,
                Dock      = DockStyle.Right,
                BackColor = NavBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            _btnSend.FlatAppearance.BorderSize = 0;
            _btnSend.Click += BtnSend_Click;

            _txtInput = new TextBox
            {
                Dock        = DockStyle.Fill,
                Font        = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor   = Color.FromArgb(248, 249, 252)
            };
            _txtInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    e.SuppressKeyPress = true;
                    BtnSend_Click(s, e);
                }
            };

            inputPanel.Controls.Add(_btnSend);
            inputPanel.Controls.Add(_txtInput);

            // ── Assemble (thứ tự ngược với chiều dock) ────────
            this.Controls.Add(_rtbChat);
            this.Controls.Add(typingPanel);
            this.Controls.Add(sep);
            this.Controls.Add(inputPanel);
            this.Controls.Add(titleBar);
        }

        // ── Hiển thị tin nhắn ─────────────────────────────────

        private void AppendUser(string text)
        {
            _rtbChat.AppendText("\n");
            AppendFormatted("  Bạn:  ",
                new Font("Segoe UI", 8.5F, FontStyle.Bold), NavBlue);
            AppendFormatted(text + "\n",
                new Font("Segoe UI", 9.5F), Color.FromArgb(22, 34, 60));
            ScrollToEnd();
        }

        private void AppendBot(string text)
        {
            _rtbChat.AppendText("\n");
            AppendFormatted("🤖 Trợ lý:  ",
                new Font("Segoe UI", 8.5F, FontStyle.Bold), BotColor);
            AppendFormatted(text + "\n",
                new Font("Segoe UI", 9.5F), Color.FromArgb(22, 34, 60));
            AppendFormatted("─────────────────────────────────────\n",
                new Font("Segoe UI", 7F), Color.FromArgb(200, 210, 228));
            ScrollToEnd();
        }

        private void AppendFormatted(string text, Font font, Color color)
        {
            int start = _rtbChat.TextLength;
            _rtbChat.AppendText(text);
            _rtbChat.Select(start, text.Length);
            _rtbChat.SelectionFont  = font;
            _rtbChat.SelectionColor = color;
            _rtbChat.SelectionLength = 0;
        }

        private void ScrollToEnd()
        {
            _rtbChat.SelectionStart = _rtbChat.Text.Length;
            _rtbChat.ScrollToCaret();
        }

        // ── Xử lý gửi tin nhắn ───────────────────────────────

        private async void BtnSend_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;

            string msg = _txtInput.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            _txtInput.Text     = "";
            AppendUser(msg);

            _isBusy            = true;
            _btnSend.Enabled   = false;
            _lblTyping.Visible = true;

            try
            {
                string reply = await _ai.SendMessageAsync(msg);
                AppendBot(reply);
            }
            catch (Exception ex)
            {
                AppendBot($"⚠️ Lỗi: {ex.Message}");
            }
            finally
            {
                _isBusy            = false;
                _btnSend.Enabled   = true;
                _lblTyping.Visible = false;
                _txtInput.Focus();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _txtInput.Focus();
        }
    }
}
