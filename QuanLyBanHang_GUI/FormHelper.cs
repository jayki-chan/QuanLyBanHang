using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Static helper — cung cấp style và factory methods dùng chung.
    /// Không kế thừa Form để tránh conflict với Visual Studio Designer.
    /// </summary>
    public static class FormHelper
    {
        // ── Palette ────────────────────────────────────────────
        public static readonly Color NavBlue = Color.FromArgb(30, 55, 100);
        public static readonly Color NavBlueLight = Color.FromArgb(242, 246, 255);
        public static readonly Color BgGray = Color.FromArgb(245, 246, 250);
        public static readonly Color FooterBg = Color.FromArgb(232, 236, 244);
        public static readonly Color BorderColor = Color.FromArgb(208, 214, 228);
        public static readonly Color GridAlt = Color.FromArgb(246, 249, 255);
        public static readonly Color SelectBg = Color.FromArgb(205, 222, 252);
        public static readonly Color SelectFg = Color.FromArgb(15, 38, 80);

        // ── Button colors ──────────────────────────────────────
        public static readonly Color BtnReload = Color.FromArgb(85, 110, 155);
        public static readonly Color BtnThem = Color.FromArgb(34, 139, 86);
        public static readonly Color BtnSua = Color.FromArgb(175, 118, 18);
        public static readonly Color BtnLuu = Color.FromArgb(30, 55, 100);
        public static readonly Color BtnHuy = Color.FromArgb(130, 48, 48);
        public static readonly Color BtnXoa = Color.FromArgb(188, 40, 40);

        // ── Style DataGridView ─────────────────────────────────
        public static void StyleGrid(DataGridView g)
        {
            g.Dock = DockStyle.Fill;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.ReadOnly = true;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = Color.White;
            g.GridColor = Color.FromArgb(218, 224, 236);
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            g.ColumnHeadersDefaultCellStyle.BackColor = NavBlue;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 36;
            g.EnableHeadersVisualStyles = false;

            g.DefaultCellStyle.BackColor = Color.White;
            g.DefaultCellStyle.ForeColor = Color.FromArgb(35, 45, 65);
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            g.DefaultCellStyle.SelectionBackColor = SelectBg;
            g.DefaultCellStyle.SelectionForeColor = SelectFg;
            g.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            g.RowTemplate.Height = 32;
            g.AlternatingRowsDefaultCellStyle.BackColor = GridAlt;
        }

        // ── Tạo nút hành động ─────────────────────────────────
        public static Button MakeBtn(string text, Color bg)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(96, 34),
                Font = new Font("Segoe UI", 9F),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 5, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        // ── Tạo nút Trở Về ────────────────────────────────────
        public static Button MakeBtnTroVe(EventHandler onClick)
        {
            var b = new Button
            {
                Text = "Trở Về",
                Size = new Size(90, 34),
                Location = new Point(8, 12),
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(48, 62, 90),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(175, 188, 212);
            b.FlatAppearance.BorderSize = 1;
            b.Click += onClick;
            return b;
        }

        /// <summary>
        /// Xây dựng toàn bộ footer panel chứa 6 nút action + nút Trở Về.
        /// Trả về các nút qua out parameters để form gán event.
        /// </summary>
        public static Panel BuildFooter(
            out Button btnReload_, out Button btnThem_, out Button btnSua_,
            out Button btnLuu_, out Button btnHuybo_, out Button btnXoa_,
            EventHandler onTroVe)
        {
            var pnlFooter = new Panel
            {
                BackColor = FooterBg,
                Dock = DockStyle.Bottom,
                Height = 58
            };
            pnlFooter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderColor), 0, 0, pnlFooter.Width, 0);

            // FlowPanel cho 6 nút bên trái
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(10, 12, 0, 0),
                BackColor = Color.Transparent
            };

            btnReload_ = MakeBtn("↺  Reload", BtnReload);
            btnThem_ = MakeBtn("+  Thêm", BtnThem);
            btnSua_ = MakeBtn("✎  Sửa", BtnSua);
            btnLuu_ = MakeBtn("💾  Lưu", BtnLuu);
            btnHuybo_ = MakeBtn("✕  Hủy Bỏ", BtnHuy);
            btnXoa_ = MakeBtn("🗑  Xóa", BtnXoa);
            flow.Controls.AddRange(new Control[]
                { btnReload_, btnThem_, btnSua_, btnLuu_, btnHuybo_, btnXoa_ });

            // Panel cố định bên phải cho nút Trở Về
            var rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 112,
                BackColor = Color.Transparent
            };
            rightPanel.Controls.Add(MakeBtnTroVe(onTroVe));

            pnlFooter.Controls.Add(flow);
            pnlFooter.Controls.Add(rightPanel);
            return pnlFooter;
        }

        // ── Tạo Label + TextBox ────────────────────────────────
        public static (Label lbl, TextBox txt) MakeField(
            Panel parent, string caption, int x, int width, int y = 28)
        {
            var lbl = new Label
            {
                AutoSize = true,
                Location = new Point(x, y - 18),
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(68, 82, 110),
                Text = caption
            };
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 24),
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            return (lbl, txt);
        }

        // ── Tạo Label + ComboBox ───────────────────────────────
        public static (Label lbl, ComboBox cbo) MakeCombo(
            Panel parent, string caption, int x, int width, int y = 28)
        {
            var lbl = new Label
            {
                AutoSize = true,
                Location = new Point(x, y - 18),
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(68, 82, 110),
                Text = caption
            };
            var cbo = new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 24),
                Font = new Font("Segoe UI", 9.5F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(cbo);
            return (lbl, cbo);
        }

        // ── Tạo Label + DateTimePicker ─────────────────────────
        public static (Label lbl, DateTimePicker dtp) MakeDatePicker(
            Panel parent, string caption, int x, int width, int y = 28)
        {
            var lbl = new Label
            {
                AutoSize = true,
                Location = new Point(x, y - 18),
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(68, 82, 110),
                Text = caption
            };
            var dtp = new DateTimePicker
            {
                Location = new Point(x, y),
                Size = new Size(width, 24),
                Font = new Font("Segoe UI", 9.5F),
                Format = DateTimePickerFormat.Short
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(dtp);
            return (lbl, dtp);
        }

        // ── Tạo Header Panel ───────────────────────────────────
        public static (Panel pnl, Label lbl) BuildHeader(string title)
        {
            var pnl = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            var lbl = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 13.5F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = title
            };
            pnl.Controls.Add(lbl);
            return (pnl, lbl);
        }

        // ── Tạo Input Panel ────────────────────────────────────
        public static Panel BuildInputPanel(int height = 68)
        {
            var p = new Panel
            {
                BackColor = NavBlueLight,
                Dock = DockStyle.Top,
                Height = height,
                Padding = new Padding(14, 10, 14, 6)
            };
            p.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderColor), 0, p.Height - 1, p.Width, p.Height - 1);
            return p;
        }

        // ── Tạo Grid Panel ─────────────────────────────────────
        public static (Panel pnlGrid, DataGridView dgv) BuildGridPanel()
        {
            var dgv = new DataGridView();
            StyleGrid(dgv);
            var p = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 10, 14, 0),
                BackColor = BgGray
            };
            p.Controls.Add(dgv);
            return (p, dgv);
        }

        // ── Toggle edit state ─────────────────────────────────
        public static void SetEditMode(bool editing,
            Panel pnlInput,
            Button btnLuu, Button btnHuybo,
            Button btnThem, Button btnSua, Button btnXoa, Button btnReload)
        {
            pnlInput.Enabled = editing;
            btnLuu.Enabled = editing;
            btnHuybo.Enabled = editing;
            btnThem.Enabled = !editing;
            btnSua.Enabled = !editing;
            btnXoa.Enabled = !editing;
            btnReload.Enabled = !editing;
        }

        // ── MessageBox shortcuts ───────────────────────────────
        public static void ShowOK(string msg)
            => MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        public static void ShowWarn(string msg)
            => MessageBox.Show(msg, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        public static void ShowError(string msg)
            => MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        public static bool Confirm(string msg)
            => MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
               == DialogResult.Yes;
    }
}