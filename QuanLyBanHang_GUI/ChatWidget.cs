using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// GUI: Cửa sổ chat với AI trợ lý (floating tool window).
    ///  – Sidebar trái: danh sách lịch sử tất cả cuộc hội thoại (lưu trong DB)
    ///  – Panel phải: hội thoại hiện tại + ô nhập liệu
    ///  – Dùng ChatBUS (lớp BUS) để gọi AI và lưu/tải lịch sử
    /// </summary>
    public class ChatWidget : Form
    {
        private ChatBUS _bus;

        // ── Controls ─────────────────────────────────────────
        private ListBox      _lstSessions;
        private RichTextBox  _rtbChat;
        private TextBox      _txtInput;
        private Button       _btnSend;
        private Button       _btnNewChat;
        private Button       _btnDelSession;
        private Label        _lblTyping;
        private PictureBox   _picTyping;
        private Label        _lblSessionTitle;
        private bool         _isBusy;
        private bool         _suppressSelChange;

        // ── Màu sắc ──────────────────────────────────────────
        static readonly Color NavBlue    = Color.FromArgb(30, 55, 100);
        static readonly Color SidebarBg  = Color.FromArgb(40, 65, 116);
        static readonly Color SidebarSel = Color.FromArgb(60, 95, 160);
        static readonly Color BgGray     = Color.FromArgb(245, 246, 250);
        static readonly Color BotColor   = Color.FromArgb(0, 105, 200);
        static readonly Color UserColor  = Color.FromArgb(30, 55, 100);
        static readonly Color TypingColor = Color.FromArgb(100, 130, 185);

        public ChatWidget(NhanVienDTO user)
        {
            _bus = new ChatBUS(user);
            BuildUI();
            LoadSessionList();
        }

        // ─────────────────────────────────────────────────────
        //  Xây dựng UI
        // ─────────────────────────────────────────────────────
        private void BuildUI()
        {
            this.Text            = "Trợ lý AI – Quản Lý Bán Hàng";
            this.Size            = new Size(700, 660);
            this.MinimumSize     = new Size(560, 480);
            this.StartPosition   = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.BackColor       = BgGray;
            this.ShowInTaskbar   = false;

            var wa = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(wa.Right - this.Width - 24, wa.Bottom - this.Height - 24);

            // ═══════════════════════════════════════════════════
            //  Sidebar trái (lịch sử hội thoại)
            // ═══════════════════════════════════════════════════
            var sidebar = new Panel
            {
                Width     = 188,
                Dock      = DockStyle.Left,
                BackColor = SidebarBg
            };

            // Sidebar header: icon (Chat) + text
            var pnlSidebarHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 38,
                BackColor = NavBlue
            };
            var picSidebarIcon = new PictureBox
            {
                Image     = AppIcons.Get(IconType.Chat, 14, Color.White),
                SizeMode  = PictureBoxSizeMode.CenterImage,
                Width     = 28,
                Dock      = DockStyle.Left,
                BackColor = NavBlue
            };
            var lblSidebar = new Label
            {
                Text      = "LỊCH SỬ HỘI THOẠI",
                Dock      = DockStyle.Fill,
                BackColor = NavBlue,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlSidebarHeader.Controls.Add(lblSidebar);
            pnlSidebarHeader.Controls.Add(picSidebarIcon);

            // Nút "Cuộc hội thoại mới" (icon Add)
            _btnNewChat = new Button
            {
                Text                = " Cuộc hội thoại mới",
                Image               = AppIcons.Get(IconType.Add, 14, Color.White),
                TextImageRelation   = TextImageRelation.ImageBeforeText,
                ImageAlign          = ContentAlignment.MiddleLeft,
                Dock                = DockStyle.Top,
                Height              = 36,
                BackColor           = Color.FromArgb(34, 139, 86),
                ForeColor           = Color.White,
                FlatStyle           = FlatStyle.Flat,
                Font                = new Font("Segoe UI", 8.5F),
                Cursor              = Cursors.Hand,
                TextAlign           = ContentAlignment.MiddleLeft,
                Padding             = new Padding(6, 0, 0, 0)
            };
            _btnNewChat.FlatAppearance.BorderSize = 0;
            _btnNewChat.Click += BtnNewChat_Click;

            _lstSessions = new ListBox
            {
                Dock        = DockStyle.Fill,
                BackColor   = SidebarBg,
                ForeColor   = Color.FromArgb(210, 225, 255),
                Font        = new Font("Segoe UI", 8.5F),
                BorderStyle = BorderStyle.None,
                ItemHeight  = 44,
                DrawMode    = DrawMode.OwnerDrawFixed
            };
            _lstSessions.DrawItem             += LstSessions_DrawItem;
            _lstSessions.SelectedIndexChanged += LstSessions_SelectedIndexChanged;

            // Nút "Xóa hội thoại" (icon Delete)
            _btnDelSession = new Button
            {
                Text                = " Xóa hội thoại này",
                Image               = AppIcons.Get(IconType.Delete, 14, Color.White),
                TextImageRelation   = TextImageRelation.ImageBeforeText,
                ImageAlign          = ContentAlignment.MiddleLeft,
                Dock                = DockStyle.Bottom,
                Height              = 34,
                BackColor           = Color.FromArgb(150, 38, 38),
                ForeColor           = Color.White,
                FlatStyle           = FlatStyle.Flat,
                Font                = new Font("Segoe UI", 8.5F),
                Cursor              = Cursors.Hand,
                TextAlign           = ContentAlignment.MiddleLeft,
                Padding             = new Padding(6, 0, 0, 0),
                Enabled             = false
            };
            _btnDelSession.FlatAppearance.BorderSize = 0;
            _btnDelSession.Click += BtnDelSession_Click;

            sidebar.Controls.Add(_lstSessions);
            sidebar.Controls.Add(_btnNewChat);
            sidebar.Controls.Add(pnlSidebarHeader);
            sidebar.Controls.Add(_btnDelSession);

            // ═══════════════════════════════════════════════════
            //  Panel phải (hội thoại)
            // ═══════════════════════════════════════════════════

            // ── Header (icon Chat + tiêu đề session) ─────────
            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 44,
                BackColor = NavBlue
            };
            var picHeaderIcon = new PictureBox
            {
                Image     = AppIcons.Get(IconType.Chat, 18, Color.White),
                SizeMode  = PictureBoxSizeMode.CenterImage,
                Width     = 40,
                Dock      = DockStyle.Left,
                BackColor = NavBlue
            };
            _lblSessionTitle = new Label
            {
                Text      = "Trợ lý AI – Quản Lý Bán Hàng",
                Dock      = DockStyle.Fill,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = NavBlue,
                Padding   = new Padding(4, 0, 0, 0)
            };
            pnlHeader.Controls.Add(_lblSessionTitle);
            pnlHeader.Controls.Add(picHeaderIcon);

            // ── Chat display ──────────────────────────────────
            _rtbChat = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                ReadOnly    = true,
                BorderStyle = BorderStyle.None,
                BackColor   = BgGray,
                Font        = new Font("Segoe UI", 9.5F),
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                Padding     = new Padding(8, 6, 8, 6),
                DetectUrls  = false,
                WordWrap    = true
            };

            // ── Typing indicator (icon Reload + label) ────────
            var pnlTyping = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 20,
                BackColor = BgGray
            };
            _picTyping = new PictureBox
            {
                Image     = AppIcons.Get(IconType.Reload, 13, TypingColor),
                SizeMode  = PictureBoxSizeMode.CenterImage,
                Width     = 20,
                Dock      = DockStyle.Left,
                BackColor = BgGray,
                Visible   = false
            };
            _lblTyping = new Label
            {
                Text      = " AI đang xử lý...",
                Dock      = DockStyle.Fill,
                ForeColor = TypingColor,
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible   = false
            };
            pnlTyping.Controls.Add(_lblTyping);
            pnlTyping.Controls.Add(_picTyping);

            // ── Separator ────────────────────────────────────
            var sep = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 1,
                BackColor = Color.FromArgb(210, 218, 232)
            };

            // ── Input panel ──────────────────────────────────
            var pnlInput = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 50,
                BackColor = Color.White,
                Padding   = new Padding(6, 5, 6, 5)
            };

            // Nút Gửi (icon Login = mũi tên vào)
            _btnSend = new Button
            {
                Text                = " Gửi",
                Image               = AppIcons.Get(IconType.Login, 14, Color.White),
                TextImageRelation   = TextImageRelation.ImageBeforeText,
                ImageAlign          = ContentAlignment.MiddleLeft,
                Width               = 74,
                Height              = 40,
                Dock                = DockStyle.Right,
                BackColor           = NavBlue,
                ForeColor           = Color.White,
                FlatStyle           = FlatStyle.Flat,
                Font                = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor              = Cursors.Hand
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

            pnlInput.Controls.Add(_btnSend);
            pnlInput.Controls.Add(_txtInput);

            // ── Assemble right panel ─────────────────────────
            var pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = BgGray };
            pnlRight.Controls.Add(_rtbChat);
            pnlRight.Controls.Add(pnlTyping);
            pnlRight.Controls.Add(sep);
            pnlRight.Controls.Add(pnlInput);
            pnlRight.Controls.Add(pnlHeader);

            // ── Splitter ─────────────────────────────────────
            var splitter = new Splitter
            {
                Dock      = DockStyle.Left,
                Width     = 4,
                BackColor = NavBlue
            };

            this.Controls.Add(pnlRight);
            this.Controls.Add(splitter);
            this.Controls.Add(sidebar);
        }

        // ─────────────────────────────────────────────────────
        //  Tải danh sách sessions
        // ─────────────────────────────────────────────────────
        private void LoadSessionList()
        {
            _suppressSelChange = true;
            _lstSessions.Items.Clear();

            var sessions = _bus.GetSessions();
            foreach (var s in sessions)
                _lstSessions.Items.Add(s);

            _suppressSelChange = false;

            if (_lstSessions.Items.Count > 0)
            {
                // Chọn session mới nhất (index 0)
                _lstSessions.SelectedIndex = 0;
                // SelectedIndexChanged sẽ tự gọi DisplaySession
            }
            else
            {
                ShowWelcome();
                _btnDelSession.Enabled = false;
            }
        }

        private void ShowWelcome()
        {
            _rtbChat.Clear();
            AppendBot(
                "Xin chào! Tôi là trợ lý AI của phần mềm Quản Lý Bán Hàng.\n\n" +
                "Tôi có thể giúp bạn:\n" +
                "  • Tra cứu dữ liệu thực tế (khách hàng, sản phẩm, hóa đơn...)\n" +
                "  • Giải thích cách dùng các tính năng phần mềm\n" +
                "  • Thống kê / báo cáo nhanh theo yêu cầu\n" +
                "  • Tư vấn nghiệp vụ bán hàng\n\n" +
                "Nhập câu hỏi tiếng Việt rồi nhấn Enter hoặc nút Gửi");
            _lblSessionTitle.Text = "Trợ lý AI – Cuộc hội thoại mới";
        }

        // ─────────────────────────────────────────────────────
        //  Hiển thị messages của session được chọn
        // ─────────────────────────────────────────────────────
        private void DisplaySession(ChatSessionDTO session)
        {
            _bus.LoadSession(session.Id);
            _rtbChat.Clear();
            _lblSessionTitle.Text  = session.Title;
            _btnDelSession.Enabled = true;

            var msgs = _bus.GetMessages(session.Id);
            foreach (var m in msgs)
            {
                if (m.Role == "user")           AppendUser(m.Content, m.CreatedAt);
                else if (m.Role == "assistant") AppendBot(m.Content);
            }
            ScrollToEnd();
        }

        // ─────────────────────────────────────────────────────
        //  Vẽ ListBox item (OwnerDraw)
        // ─────────────────────────────────────────────────────
        private void LstSessions_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _lstSessions.Items.Count) return;
            if (!(_lstSessions.Items[e.Index] is ChatSessionDTO s)) return;

            bool selected = (e.State & DrawItemState.Selected) != 0;
            using (var bgBrush = new SolidBrush(selected ? SidebarSel : SidebarBg))
                e.Graphics.FillRectangle(bgBrush, e.Bounds);

            if (selected)
                using (var pen = new Pen(Color.FromArgb(100, 160, 255), 2))
                    e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Top,
                                             e.Bounds.Left, e.Bounds.Bottom);

            string dateStr  = s.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            string titleStr = s.Title ?? "Hội thoại mới";
            if (titleStr.Length > 30) titleStr = titleStr.Substring(0, 30) + "…";

            // Icon Chat nhỏ cho mỗi item
            var icon = AppIcons.Get(IconType.Chat, 12, Color.FromArgb(130, 160, 210));
            e.Graphics.DrawImage(icon, e.Bounds.Left + 6, e.Bounds.Top + 5);

            using (var fDate  = new Font("Segoe UI", 7.5F))
            using (var fTitle = new Font("Segoe UI", 8.5F))
            using (var bDate  = new SolidBrush(Color.FromArgb(160, 185, 230)))
            using (var bTitle = new SolidBrush(Color.FromArgb(220, 235, 255)))
            {
                e.Graphics.DrawString(dateStr,  fDate,  bDate,  new PointF(e.Bounds.Left + 22, e.Bounds.Top + 5));
                e.Graphics.DrawString(titleStr, fTitle, bTitle, new PointF(e.Bounds.Left + 8,  e.Bounds.Top + 24));
            }
        }

        // ─────────────────────────────────────────────────────
        //  Event handlers
        // ─────────────────────────────────────────────────────
        private void LstSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressSelChange) return;
            if (_lstSessions.SelectedItem is ChatSessionDTO s)
                DisplaySession(s);
            _btnDelSession.Enabled = _lstSessions.SelectedItem != null;
        }

        private void BtnNewChat_Click(object sender, EventArgs e)
        {
            _bus.LoadSession(0);   // Reset không có session
            _suppressSelChange = true;
            _lstSessions.SelectedIndex = -1;
            _suppressSelChange = false;
            _rtbChat.Clear();
            _btnDelSession.Enabled = false;
            ShowWelcome();
            _txtInput.Focus();
        }

        private void BtnDelSession_Click(object sender, EventArgs e)
        {
            if (!(_lstSessions.SelectedItem is ChatSessionDTO s)) return;

            if (MessageBox.Show(
                    $"Xóa hội thoại:\n\"{s.Title}\"?\n\nThao tác không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes) return;

            _bus.DeleteSession(s.Id);
            LoadSessionList();
        }

        // ─────────────────────────────────────────────────────
        //  Gửi tin nhắn
        // ─────────────────────────────────────────────────────
        private async void BtnSend_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;

            string msg = _txtInput.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            _txtInput.Text     = "";
            AppendUser(msg, DateTime.Now);

            _isBusy             = true;
            _btnSend.Enabled    = false;
            _lblTyping.Visible  = true;
            _picTyping.Visible  = true;

            try
            {
                bool isNew = _bus.CurrentSessionId == 0;
                string reply = await _bus.SendMessageAsync(msg);
                AppendBot(reply);

                // Nếu vừa tạo session mới, reload list và bôi chọn session đó
                if (isNew) ReloadAndSelectCurrent();

                // Cập nhật tiêu đề header
                foreach (ChatSessionDTO s2 in _bus.GetSessions())
                    if (s2.Id == _bus.CurrentSessionId)
                    { _lblSessionTitle.Text = s2.Title; break; }
            }
            catch (Exception ex)
            {
                AppendBot("[Lỗi]  " + ex.Message);
            }
            finally
            {
                _isBusy             = false;
                _btnSend.Enabled    = true;
                _lblTyping.Visible  = false;
                _picTyping.Visible  = false;
                _txtInput.Focus();
            }
        }

        /// <summary>Reload danh sách session và chọn session hiện tại.</summary>
        private void ReloadAndSelectCurrent()
        {
            int cid = _bus.CurrentSessionId;
            _suppressSelChange = true;
            _lstSessions.Items.Clear();

            var sessions = _bus.GetSessions();
            int selectIdx = 0;
            for (int i = 0; i < sessions.Count; i++)
            {
                _lstSessions.Items.Add(sessions[i]);
                if (sessions[i].Id == cid) selectIdx = i;
            }

            _suppressSelChange = false;

            if (_lstSessions.Items.Count > 0)
            {
                _suppressSelChange = true;
                _lstSessions.SelectedIndex = selectIdx;
                _suppressSelChange = false;
                _btnDelSession.Enabled = true;
            }
        }

        // ─────────────────────────────────────────────────────
        //  Hiển thị tin nhắn trong RichTextBox
        // ─────────────────────────────────────────────────────
        private void AppendUser(string text, DateTime time)
        {
            _rtbChat.AppendText("\n");
            AppendFormatted("  Bạn  [" + time.ToString("HH:mm") + "]\n",
                new Font("Segoe UI", 8F, FontStyle.Bold), UserColor);
            AppendFormatted("  " + text.Replace("\n", "\n  ") + "\n",
                new Font("Segoe UI", 9.5F), Color.FromArgb(22, 34, 60));
        }

        private void AppendBot(string text)
        {
            _rtbChat.AppendText("\n");
            AppendFormatted("  Trợ lý AI\n",
                new Font("Segoe UI", 8F, FontStyle.Bold), BotColor);
            AppendFormatted("  " + text.Replace("\n", "\n  ") + "\n",
                new Font("Segoe UI", 9.5F), Color.FromArgb(22, 34, 60));
            AppendFormatted("  " + new string('─', 52) + "\n",
                new Font("Segoe UI", 7F), Color.FromArgb(200, 210, 228));
            ScrollToEnd();
        }

        private void AppendFormatted(string text, Font font, Color color)
        {
            int start = _rtbChat.TextLength;
            _rtbChat.AppendText(text);
            _rtbChat.Select(start, text.Length);
            _rtbChat.SelectionFont   = font;
            _rtbChat.SelectionColor  = color;
            _rtbChat.SelectionLength = 0;
        }

        private void ScrollToEnd()
        {
            _rtbChat.SelectionStart = _rtbChat.Text.Length;
            _rtbChat.ScrollToCaret();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _txtInput.Focus();
        }
    }
}
