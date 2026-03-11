using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new MenuStrip();
            this.hệThốngToolStripMenuItem                   = new ToolStripMenuItem();
            this.câuHìnhHệThốngToolStripMenuItem            = new ToolStripMenuItem();
            this.quảnLýNgườiDùngToolStripMenuItem           = new ToolStripMenuItem();
            this.đăngNhậpToolStripMenuItem                  = new ToolStripMenuItem();
            this.đổiMậtKhẩuToolStripMenuItem                = new ToolStripMenuItem();
            this.đăngXuấtToolStripMenuItem                  = new ToolStripMenuItem();
            this.toolStripSeparator1                        = new ToolStripSeparator();
            this.thoátToolStripMenuItem                     = new ToolStripMenuItem();
            this.xemDanhMụcToolStripMenuItem                = new ToolStripMenuItem();
            this.danhMụcThànhPhốToolStripMenuItem           = new ToolStripMenuItem();
            this.danhMụcKháchhàngToolStripMenuItem          = new ToolStripMenuItem();
            this.danhMụcNhânViênToolStripMenuItem           = new ToolStripMenuItem();
            this.danhMụcSảnPhẩmToolStripMenuItem            = new ToolStripMenuItem();
            this.danhMụcHóaĐơnToolStripMenuItem             = new ToolStripMenuItem();
            this.danhMụcChiTiếtHóaĐơnToolStripMenuItem     = new ToolStripMenuItem();
            this.quảnLýDanhMụcĐơnToolStripMenuItem          = new ToolStripMenuItem();
            this.danhMụcThànhPhốToolStripMenuItem1          = new ToolStripMenuItem();
            this.danhMụcKháchhàngToolStripMenuItem1         = new ToolStripMenuItem();
            this.danhMụcNhânViênToolStripMenuItem1          = new ToolStripMenuItem();
            this.danhMụcSảnPhẩmToolStripMenuItem1           = new ToolStripMenuItem();
            this.danhMụcHóaĐơnToolStripMenuItem1            = new ToolStripMenuItem();
            this.danhMụcChiTiếtHóaĐơnToolStripMenuItem1    = new ToolStripMenuItem();
            this.quảnLýDanhMụcTheoNhómToolStripMenuItem     = new ToolStripMenuItem();
            this.kháchhàngTheoThànhPhốToolStripMenuItem     = new ToolStripMenuItem();
            this.hóaĐơnTheoKháchhàngToolStripMenuItem       = new ToolStripMenuItem();
            this.hóaĐơnTheoSảnPhẩmToolStripMenuItem        = new ToolStripMenuItem();
            this.hóaĐơnTheoNhânViênToolStripMenuItem        = new ToolStripMenuItem();
            this.chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem = new ToolStripMenuItem();
            this.giúpĐỡToolStripMenuItem                    = new ToolStripMenuItem();
            this.hướngDẫnSửDụngToolStripMenuItem            = new ToolStripMenuItem();
            this.tácGiảToolStripMenuItem                    = new ToolStripMenuItem();
            this.statusStrip1 = new StatusStrip();
            this.lblStatus    = new ToolStripStatusLabel();

            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();

            // ── MenuStrip ─────────────────────────────────────
            this.menuStrip1.BackColor = Color.FromArgb(30, 55, 100);
            this.menuStrip1.ForeColor = Color.White;
            this.menuStrip1.Font      = new Font("Segoe UI", 9.5F);
            this.menuStrip1.Dock      = DockStyle.Top;
            this.menuStrip1.Renderer  = new QuanLyBanHang_GUI.DarkMenuRenderer();
            this.menuStrip1.Items.AddRange(new ToolStripItem[] {
                hệThốngToolStripMenuItem,
                xemDanhMụcToolStripMenuItem,
                quảnLýDanhMụcĐơnToolStripMenuItem,
                quảnLýDanhMụcTheoNhómToolStripMenuItem,
                giúpĐỡToolStripMenuItem
            });

            // ── Hệ thống ──────────────────────────────────────
            this.hệThốngToolStripMenuItem.Text = "Hệ thống";
            this.hệThốngToolStripMenuItem.ForeColor = Color.White;
            this.hệThốngToolStripMenuItem.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            this.hệThốngToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                câuHìnhHệThốngToolStripMenuItem,
                quảnLýNgườiDùngToolStripMenuItem,
                new ToolStripSeparator(),
                đăngNhậpToolStripMenuItem,
                đổiMậtKhẩuToolStripMenuItem,
                đăngXuấtToolStripMenuItem,
                new ToolStripSeparator(),
                thoátToolStripMenuItem
            });

            this.câuHìnhHệThốngToolStripMenuItem.Text   = "Cấu hình hệ thống";
            this.câuHìnhHệThốngToolStripMenuItem.Font   = new Font("Segoe UI", 9.5F);
            this.câuHìnhHệThốngToolStripMenuItem.Click += new System.EventHandler(câuHìnhHệThốngToolStripMenuItem_Click);

            this.quảnLýNgườiDùngToolStripMenuItem.Text   = "Quản lý người dùng";
            this.quảnLýNgườiDùngToolStripMenuItem.Font   = new Font("Segoe UI", 9.5F);
            this.quảnLýNgườiDùngToolStripMenuItem.Click += new System.EventHandler(quảnLýNgườiDùngToolStripMenuItem_Click);

            this.đăngNhậpToolStripMenuItem.Text   = "Đăng nhập";
            this.đăngNhậpToolStripMenuItem.Font   = new Font("Segoe UI", 9.5F);
            this.đăngNhậpToolStripMenuItem.Click += new System.EventHandler(đăngNhậpToolStripMenuItem_Click);

            this.đổiMậtKhẩuToolStripMenuItem.Text   = "Đổi Mật khẩu";
            this.đổiMậtKhẩuToolStripMenuItem.Font   = new Font("Segoe UI", 9.5F);
            this.đổiMậtKhẩuToolStripMenuItem.Click += new System.EventHandler(đổiMậtKhẩuToolStripMenuItem_Click);

            this.đăngXuấtToolStripMenuItem.Text    = "Đăng xuất";
            this.đăngXuấtToolStripMenuItem.Font    = new Font("Segoe UI", 9.5F);
            this.đăngXuấtToolStripMenuItem.Visible = false;
            this.đăngXuấtToolStripMenuItem.Click  += new System.EventHandler(đăngXuấtToolStripMenuItem_Click);

            this.thoátToolStripMenuItem.Text   = "Thoát";
            this.thoátToolStripMenuItem.Font   = new Font("Segoe UI", 9.5F);
            this.thoátToolStripMenuItem.Click += new System.EventHandler(thoátToolStripMenuItem_Click);

            // ── Xem Danh mục ──────────────────────────────────
            this.xemDanhMụcToolStripMenuItem.Text = "Xem Danh mục";
            this.xemDanhMụcToolStripMenuItem.ForeColor = Color.White;
            this.xemDanhMụcToolStripMenuItem.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            this.xemDanhMụcToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                danhMụcThànhPhốToolStripMenuItem,
                danhMụcKháchhàngToolStripMenuItem,
                danhMụcNhânViênToolStripMenuItem,
                danhMụcSảnPhẩmToolStripMenuItem,
                danhMụcHóaĐơnToolStripMenuItem,
                danhMụcChiTiếtHóaĐơnToolStripMenuItem
            });

            this.danhMụcThànhPhốToolStripMenuItem.Text       = "Danh mục Thành Phố";
            this.danhMụcThànhPhốToolStripMenuItem.Font       = new Font("Segoe UI", 9.5F);
            this.danhMụcThànhPhốToolStripMenuItem.Click     += new System.EventHandler(danhMụcThànhPhốToolStripMenuItem_Click);
            this.danhMụcKháchhàngToolStripMenuItem.Text      = "Danh mục Khách Hàng";
            this.danhMụcKháchhàngToolStripMenuItem.Font      = new Font("Segoe UI", 9.5F);
            this.danhMụcKháchhàngToolStripMenuItem.Click    += new System.EventHandler(danhMụcKháchhàngToolStripMenuItem_Click);
            this.danhMụcNhânViênToolStripMenuItem.Text       = "Danh mục Nhân Viên";
            this.danhMụcNhânViênToolStripMenuItem.Font       = new Font("Segoe UI", 9.5F);
            this.danhMụcNhânViênToolStripMenuItem.Click     += new System.EventHandler(danhMụcNhânViênToolStripMenuItem_Click);
            this.danhMụcSảnPhẩmToolStripMenuItem.Text       = "Danh mục Sản Phẩm";
            this.danhMụcSảnPhẩmToolStripMenuItem.Font       = new Font("Segoe UI", 9.5F);
            this.danhMụcSảnPhẩmToolStripMenuItem.Click     += new System.EventHandler(danhMụcSảnPhẩmToolStripMenuItem_Click);
            this.danhMụcHóaĐơnToolStripMenuItem.Text        = "Danh mục Hóa Đơn";
            this.danhMụcHóaĐơnToolStripMenuItem.Font        = new Font("Segoe UI", 9.5F);
            this.danhMụcHóaĐơnToolStripMenuItem.Click      += new System.EventHandler(danhMụcHóaĐơnToolStripMenuItem_Click);
            this.danhMụcChiTiếtHóaĐơnToolStripMenuItem.Text  = "Danh mục Chi Tiết Hóa Đơn";
            this.danhMụcChiTiếtHóaĐơnToolStripMenuItem.Font  = new Font("Segoe UI", 9.5F);
            this.danhMụcChiTiếtHóaĐơnToolStripMenuItem.Click += new System.EventHandler(danhMụcChiTiếtHóaĐơnToolStripMenuItem_Click);

            // ── Quản lý Danh mục đơn ──────────────────────────
            this.quảnLýDanhMụcĐơnToolStripMenuItem.Text = "Quản lý Danh mục đơn";
            this.quảnLýDanhMụcĐơnToolStripMenuItem.ForeColor = Color.White;
            this.quảnLýDanhMụcĐơnToolStripMenuItem.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            this.quảnLýDanhMụcĐơnToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                danhMụcThànhPhốToolStripMenuItem1,
                danhMụcKháchhàngToolStripMenuItem1,
                danhMụcNhânViênToolStripMenuItem1,
                danhMụcSảnPhẩmToolStripMenuItem1,
                danhMụcHóaĐơnToolStripMenuItem1,
                danhMụcChiTiếtHóaĐơnToolStripMenuItem1
            });

            this.danhMụcThànhPhốToolStripMenuItem1.Text       = "Thành Phố";
            this.danhMụcThànhPhốToolStripMenuItem1.Font       = new Font("Segoe UI", 9.5F);
            this.danhMụcThànhPhốToolStripMenuItem1.Click     += new System.EventHandler(danhMụcThànhPhốToolStripMenuItem1_Click);
            this.danhMụcKháchhàngToolStripMenuItem1.Text      = "Khách Hàng";
            this.danhMụcKháchhàngToolStripMenuItem1.Font      = new Font("Segoe UI", 9.5F);
            this.danhMụcKháchhàngToolStripMenuItem1.Click    += new System.EventHandler(danhMụcKháchhàngToolStripMenuItem1_Click);
            this.danhMụcNhânViênToolStripMenuItem1.Text       = "Nhân Viên";
            this.danhMụcNhânViênToolStripMenuItem1.Font       = new Font("Segoe UI", 9.5F);
            this.danhMụcNhânViênToolStripMenuItem1.Click     += new System.EventHandler(danhMụcNhânViênToolStripMenuItem1_Click);
            this.danhMụcSảnPhẩmToolStripMenuItem1.Text       = "Sản Phẩm";
            this.danhMụcSảnPhẩmToolStripMenuItem1.Font       = new Font("Segoe UI", 9.5F);
            this.danhMụcSảnPhẩmToolStripMenuItem1.Click     += new System.EventHandler(danhMụcSảnPhẩmToolStripMenuItem1_Click);
            this.danhMụcHóaĐơnToolStripMenuItem1.Text        = "Hóa Đơn";
            this.danhMụcHóaĐơnToolStripMenuItem1.Font        = new Font("Segoe UI", 9.5F);
            this.danhMụcHóaĐơnToolStripMenuItem1.Click      += new System.EventHandler(danhMụcHóaĐơnToolStripMenuItem1_Click);
            this.danhMụcChiTiếtHóaĐơnToolStripMenuItem1.Text  = "Chi Tiết Hóa Đơn";
            this.danhMụcChiTiếtHóaĐơnToolStripMenuItem1.Font  = new Font("Segoe UI", 9.5F);
            this.danhMụcChiTiếtHóaĐơnToolStripMenuItem1.Click += new System.EventHandler(danhMụcChiTiếtHóaĐơnToolStripMenuItem1_Click);

            // ── Quản lý Danh mục theo nhóm ────────────────────
            this.quảnLýDanhMụcTheoNhómToolStripMenuItem.Text = "Quản lý Danh mục theo nhóm";
            this.quảnLýDanhMụcTheoNhómToolStripMenuItem.ForeColor = Color.White;
            this.quảnLýDanhMụcTheoNhómToolStripMenuItem.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            this.quảnLýDanhMụcTheoNhómToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                kháchhàngTheoThànhPhốToolStripMenuItem,
                hóaĐơnTheoKháchhàngToolStripMenuItem,
                hóaĐơnTheoSảnPhẩmToolStripMenuItem,
                hóaĐơnTheoNhânViênToolStripMenuItem,
                chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem
            });

            this.kháchhàngTheoThànhPhốToolStripMenuItem.Text      = "Khách Hàng theo Thành Phố";
            this.kháchhàngTheoThànhPhốToolStripMenuItem.Font      = new Font("Segoe UI", 9.5F);
            this.kháchhàngTheoThànhPhốToolStripMenuItem.Click    += new System.EventHandler(kháchhàngTheoThànhPhốToolStripMenuItem_Click);
            this.hóaĐơnTheoKháchhàngToolStripMenuItem.Text        = "Hóa Đơn theo Khách Hàng";
            this.hóaĐơnTheoKháchhàngToolStripMenuItem.Font        = new Font("Segoe UI", 9.5F);
            this.hóaĐơnTheoKháchhàngToolStripMenuItem.Click      += new System.EventHandler(hóaĐơnTheoKháchhàngToolStripMenuItem_Click);
            this.hóaĐơnTheoSảnPhẩmToolStripMenuItem.Text         = "Hóa Đơn theo Sản Phẩm";
            this.hóaĐơnTheoSảnPhẩmToolStripMenuItem.Font         = new Font("Segoe UI", 9.5F);
            this.hóaĐơnTheoSảnPhẩmToolStripMenuItem.Click       += new System.EventHandler(hóaĐơnTheoSảnPhẩmToolStripMenuItem_Click);
            this.hóaĐơnTheoNhânViênToolStripMenuItem.Text         = "Hóa Đơn theo Nhân Viên";
            this.hóaĐơnTheoNhânViênToolStripMenuItem.Font         = new Font("Segoe UI", 9.5F);
            this.hóaĐơnTheoNhânViênToolStripMenuItem.Click       += new System.EventHandler(hóaĐơnTheoNhânViênToolStripMenuItem_Click);
            this.chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem.Text  = "Chi Tiết Hóa Đơn theo Nhân Viên";
            this.chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem.Font  = new Font("Segoe UI", 9.5F);
            this.chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem.Click += new System.EventHandler(chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem_Click);

            // ── Giúp đỡ ───────────────────────────────────────
            this.giúpĐỡToolStripMenuItem.Text = "Giúp đỡ";
            this.giúpĐỡToolStripMenuItem.ForeColor = Color.White;
            this.giúpĐỡToolStripMenuItem.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            this.giúpĐỡToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                hướngDẫnSửDụngToolStripMenuItem,
                tácGiảToolStripMenuItem
            });

            this.hướngDẫnSửDụngToolStripMenuItem.Text   = "Hướng dẫn sử dụng";
            this.hướngDẫnSửDụngToolStripMenuItem.Font   = new Font("Segoe UI", 9.5F);
            this.hướngDẫnSửDụngToolStripMenuItem.Click += new System.EventHandler(hướngDẫnSửDụngToolStripMenuItem_Click);
            this.tácGiảToolStripMenuItem.Text   = "Tác giả";
            this.tácGiảToolStripMenuItem.Font   = new Font("Segoe UI", 9.5F);
            this.tácGiảToolStripMenuItem.Click += new System.EventHandler(tácGiảToolStripMenuItem_Click);

            // ── StatusStrip ───────────────────────────────────
            this.statusStrip1.BackColor  = Color.FromArgb(30, 55, 100);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.Items.Add(this.lblStatus);

            this.lblStatus.Font      = new Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = Color.FromArgb(180, 210, 255);
            this.lblStatus.Name      = "lblStatus";
            this.lblStatus.Text      = "  Chưa đăng nhập";

            // ── Form ──────────────────────────────────────────
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode       = AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1024, 640);
            this.MinimumSize         = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name          = "Form1";
            this.Text          = "Quản Lý Bán Hàng";
            this.BackColor     = Color.FromArgb(245, 246, 250);
            this.Load         += new System.EventHandler(this.Form1_Load);

            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            SetMenuIcons();
        }

        private void SetMenuIcons()
        {
            int sz  = 16;
            var wh  = Color.White;
            var dk  = Color.FromArgb(25, 38, 60);

            // ── Top-level (nền xanh → icon trắng) ─────────────
            hệThốngToolStripMenuItem.Image               = AppIcons.Get(IconType.Settings, sz, wh);
            xemDanhMụcToolStripMenuItem.Image            = AppIcons.Get(IconType.Search,   sz, wh);
            quảnLýDanhMụcĐơnToolStripMenuItem.Image      = AppIcons.Get(IconType.Edit,     sz, wh);
            quảnLýDanhMụcTheoNhómToolStripMenuItem.Image = AppIcons.Get(IconType.Chart,    sz, wh);
            giúpĐỡToolStripMenuItem.Image                = AppIcons.Get(IconType.Help,     sz, wh);

            foreach (ToolStripMenuItem item in new[] {
                hệThốngToolStripMenuItem, xemDanhMụcToolStripMenuItem,
                quảnLýDanhMụcĐơnToolStripMenuItem, quảnLýDanhMụcTheoNhómToolStripMenuItem,
                giúpĐỡToolStripMenuItem })
            {
                item.ImageAlign        = System.Drawing.ContentAlignment.MiddleLeft;
                item.TextImageRelation = TextImageRelation.ImageBeforeText;
                item.DisplayStyle      = ToolStripItemDisplayStyle.ImageAndText;
                item.Padding           = new System.Windows.Forms.Padding(6, 0, 6, 0);
            }

            // ── Hệ thống dropdown ─────────────────────────────
            câuHìnhHệThốngToolStripMenuItem.Image   = AppIcons.Get(IconType.Settings, sz, dk);
            quảnLýNgườiDùngToolStripMenuItem.Image  = AppIcons.Get(IconType.Users,    sz, dk);
            đăngNhậpToolStripMenuItem.Image         = AppIcons.Get(IconType.Login,    sz, dk);
            đổiMậtKhẩuToolStripMenuItem.Image       = AppIcons.Get(IconType.Key,      sz, dk);
            đăngXuấtToolStripMenuItem.Image         = AppIcons.Get(IconType.Logout,   sz, dk);
            thoátToolStripMenuItem.Image            = AppIcons.Get(IconType.Exit,     sz, dk);

            // ── Xem Danh mục dropdown ─────────────────────────
            danhMụcThànhPhốToolStripMenuItem.Image       = AppIcons.Get(IconType.City,    sz, dk);
            danhMụcKháchhàngToolStripMenuItem.Image      = AppIcons.Get(IconType.User,    sz, dk);
            danhMụcNhânViênToolStripMenuItem.Image       = AppIcons.Get(IconType.Users,   sz, dk);
            danhMụcSảnPhẩmToolStripMenuItem.Image       = AppIcons.Get(IconType.Product,  sz, dk);
            danhMụcHóaĐơnToolStripMenuItem.Image        = AppIcons.Get(IconType.Invoice,  sz, dk);
            danhMụcChiTiếtHóaĐơnToolStripMenuItem.Image = AppIcons.Get(IconType.Invoice,  sz, dk);

            // ── Quản lý đơn dropdown ──────────────────────────
            danhMụcThànhPhốToolStripMenuItem1.Image       = AppIcons.Get(IconType.City,    sz, dk);
            danhMụcKháchhàngToolStripMenuItem1.Image      = AppIcons.Get(IconType.User,    sz, dk);
            danhMụcNhânViênToolStripMenuItem1.Image       = AppIcons.Get(IconType.Users,   sz, dk);
            danhMụcSảnPhẩmToolStripMenuItem1.Image       = AppIcons.Get(IconType.Product,  sz, dk);
            danhMụcHóaĐơnToolStripMenuItem1.Image        = AppIcons.Get(IconType.Invoice,  sz, dk);
            danhMụcChiTiếtHóaĐơnToolStripMenuItem1.Image = AppIcons.Get(IconType.Invoice,  sz, dk);

            // ── Theo nhóm dropdown ────────────────────────────
            kháchhàngTheoThànhPhốToolStripMenuItem.Image      = AppIcons.Get(IconType.Chart, sz, dk);
            hóaĐơnTheoKháchhàngToolStripMenuItem.Image        = AppIcons.Get(IconType.Chart, sz, dk);
            hóaĐơnTheoSảnPhẩmToolStripMenuItem.Image         = AppIcons.Get(IconType.Chart, sz, dk);
            hóaĐơnTheoNhânViênToolStripMenuItem.Image         = AppIcons.Get(IconType.Chart, sz, dk);
            chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem.Image  = AppIcons.Get(IconType.Chart, sz, dk);

            // ── Giúp đỡ dropdown ──────────────────────────────
            hướngDẫnSửDụngToolStripMenuItem.Image = AppIcons.Get(IconType.Help, sz, dk);
            tácGiảToolStripMenuItem.Image         = AppIcons.Get(IconType.Info, sz, dk);
        }

        private MenuStrip menuStrip1;
        private ToolStripMenuItem hệThốngToolStripMenuItem, câuHìnhHệThốngToolStripMenuItem,
            quảnLýNgườiDùngToolStripMenuItem, đăngNhậpToolStripMenuItem,
            đổiMậtKhẩuToolStripMenuItem, đăngXuấtToolStripMenuItem, thoátToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem xemDanhMụcToolStripMenuItem,
            danhMụcThànhPhốToolStripMenuItem, danhMụcKháchhàngToolStripMenuItem,
            danhMụcNhânViênToolStripMenuItem, danhMụcSảnPhẩmToolStripMenuItem,
            danhMụcHóaĐơnToolStripMenuItem, danhMụcChiTiếtHóaĐơnToolStripMenuItem;
        private ToolStripMenuItem quảnLýDanhMụcĐơnToolStripMenuItem,
            danhMụcThànhPhốToolStripMenuItem1, danhMụcKháchhàngToolStripMenuItem1,
            danhMụcNhânViênToolStripMenuItem1, danhMụcSảnPhẩmToolStripMenuItem1,
            danhMụcHóaĐơnToolStripMenuItem1, danhMụcChiTiếtHóaĐơnToolStripMenuItem1;
        private ToolStripMenuItem quảnLýDanhMụcTheoNhómToolStripMenuItem,
            kháchhàngTheoThànhPhốToolStripMenuItem, hóaĐơnTheoKháchhàngToolStripMenuItem,
            hóaĐơnTheoSảnPhẩmToolStripMenuItem, hóaĐơnTheoNhânViênToolStripMenuItem,
            chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem;
        private ToolStripMenuItem giúpĐỡToolStripMenuItem,
            hướngDẫnSửDụngToolStripMenuItem, tácGiảToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
    }
}
