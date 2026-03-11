using System.Drawing;
using System.Drawing.Drawing2D;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Vẽ icon bằng GDI+ thuần — không cần file ảnh hay emoji.
    /// Tất cả icon trả về Bitmap có nền trong suốt.
    /// </summary>
    public static class AppIcons
    {
        // ── Factory chính ─────────────────────────────────────
        public static Bitmap Get(IconType type, int size = 16, Color? color = null)
        {
            var c = color ?? Color.White;
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode      = SmoothingMode.AntiAlias;
                g.InterpolationMode  = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode    = PixelOffsetMode.HighQuality;

                float s = size;
                float m = s * 0.08f; // margin

                using (var pen  = new Pen(c, s * 0.1f) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round })
                using (var fill = new SolidBrush(c))
                {
                    switch (type)
                    {
                        case IconType.Reload:     DrawReload(g, pen, s, m);     break;
                        case IconType.Add:        DrawAdd(g, pen, s, m);        break;
                        case IconType.Edit:       DrawEdit(g, pen, fill, s, m); break;
                        case IconType.Save:       DrawSave(g, pen, fill, s, m); break;
                        case IconType.Cancel:     DrawCancel(g, pen, s, m);     break;
                        case IconType.Delete:     DrawDelete(g, pen, fill, s, m); break;
                        case IconType.Back:       DrawBack(g, pen, fill, s, m);  break;
                        case IconType.Search:     DrawSearch(g, pen, s, m);     break;
                        case IconType.Login:      DrawLogin(g, pen, fill, s, m);  break;
                        case IconType.Logout:     DrawLogout(g, pen, fill, s, m); break;
                        case IconType.Password:   DrawPassword(g, pen, fill, s, m); break;
                        case IconType.Settings:   DrawSettings(g, pen, fill, s, m); break;
                        case IconType.User:       DrawUser(g, pen, fill, s, m); break;
                        case IconType.Users:      DrawUsers(g, pen, fill, s, m); break;
                        case IconType.Exit:       DrawExit(g, pen, fill, s, m);  break;
                        case IconType.Connect:    DrawConnect(g, pen, fill, s, m); break;
                        case IconType.Filter:     DrawFilter(g, pen, s, m);     break;
                        case IconType.Photo:      DrawPhoto(g, pen, fill, s, m); break;
                        case IconType.Invoice:    DrawInvoice(g, pen, fill, s, m); break;
                        case IconType.Product:    DrawProduct(g, pen, fill, s, m); break;
                        case IconType.City:       DrawCity(g, pen, fill, s, m);  break;
                        case IconType.Chart:      DrawChart(g, pen, fill, s, m); break;
                        case IconType.Help:       DrawHelp(g, pen, fill, s, m);  break;
                        case IconType.Info:       DrawInfo(g, pen, fill, s, m);  break;
                        case IconType.Key:        DrawKey(g, pen, fill, s, m);   break;
                        case IconType.Test:       DrawTest(g, pen, fill, s, m);  break;
                    }
                }
            }
            return bmp;
        }

        // ═══════════════════════════════════════════════════════
        // ICON DRAWINGS
        // ═══════════════════════════════════════════════════════

        // ↺ Reload — vòng cung với mũi tên
        static void DrawReload(Graphics g, Pen p, float s, float m)
        {
            float r = s * 0.36f;
            float cx = s / 2, cy = s / 2;
            var rect = new RectangleF(cx - r, cy - r, r * 2, r * 2);
            // Vẽ 270° cung
            g.DrawArc(p, rect, -60, 300);
            // Mũi tên nhỏ ở đầu cung
            float ax = cx + r * 0.5f, ay = cy - r;
            var arrow = new PointF[] {
                new PointF(ax, ay - s*0.12f),
                new PointF(ax + s*0.18f, ay + s*0.04f),
                new PointF(ax - s*0.06f, ay + s*0.04f)
            };
            using (var b = new SolidBrush(p.Color)) g.FillPolygon(b, arrow);
        }

        // + Add — dấu cộng
        static void DrawAdd(Graphics g, Pen p, float s, float m)
        {
            float cx = s / 2, cy = s / 2, half = s * 0.38f;
            g.DrawLine(p, cx, cy - half, cx, cy + half);
            g.DrawLine(p, cx - half, cy, cx + half, cy);
        }

        // ✎ Edit — bút chì
        static void DrawEdit(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float pw = s * 0.18f;
            var body = new PointF[] {
                new PointF(m + pw, s - m - pw),
                new PointF(m,      s - m),
                new PointF(m + pw, s - m),
            };
            // thân bút
            var pts = new PointF[] {
                new PointF(m + pw,      s - m - pw),
                new PointF(s - m - pw*2, m + pw),
                new PointF(s - m,        m + pw*2),
                new PointF(s - m - pw,   m),
                new PointF(m + pw*2,     s - m - pw*2),
            };
            // Vẽ đơn giản hơn: hình chữ nhật xoay
            float len = s * 0.72f;
            g.TranslateTransform(s / 2, s / 2);
            g.RotateTransform(-45);
            var rect = new RectangleF(-pw / 2, -len / 2, pw, len * 0.82f);
            g.FillRectangle(b, rect);
            g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
            // mũi bút
            var tip = new PointF[] {
                new PointF(-pw/2, len*0.82f/2),
                new PointF(0,     len*0.82f/2 + s*0.18f),
                new PointF(pw/2,  len*0.82f/2)
            };
            g.FillPolygon(b, tip);
            g.ResetTransform();
        }

        // Save — hình đĩa mềm (floppy)
        static void DrawSave(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            // Outer box
            var outer = new RectangleF(m, m, s - m * 2, s - m * 2);
            g.FillRectangle(b, outer);
            g.DrawRectangle(p, outer.X, outer.Y, outer.Width, outer.Height);
            // Notch top-right
            using (var wb = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
                g.FillRectangle(wb, s - m - s*0.3f, m, s*0.3f, s*0.32f);
            // Inner box (label area)
            float iw = s*0.55f, ih = s*0.3f;
            float ix = m + s*0.08f, iy = s - m - ih - s*0.04f;
            using (var wb = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                g.FillRectangle(wb, ix, iy, iw, ih);
        }

        // Cancel — dấu X
        static void DrawCancel(Graphics g, Pen p, float s, float m)
        {
            float d = s * 0.28f;
            float cx = s / 2, cy = s / 2;
            g.DrawLine(p, cx - d, cy - d, cx + d, cy + d);
            g.DrawLine(p, cx + d, cy - d, cx - d, cy + d);
        }

        // Delete — thùng rác
        static void DrawDelete(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float bw = s * 0.5f, bh = s * 0.52f;
            float bx = (s - bw) / 2, by = s * 0.36f;
            // thân thùng
            g.DrawRectangle(p, bx, by, bw, bh - m);
            // 3 thanh dọc bên trong
            float gap = bw / 4;
            for (int i = 1; i <= 2; i++)
                g.DrawLine(p, bx + gap * i, by + s * 0.08f, bx + gap * i, by + bh - m - s * 0.08f);
            // nắp
            float lx = bx - s*0.06f, lw = bw + s*0.12f;
            g.DrawLine(p, lx, by, lx + lw, by);
            // tay cầm
            float hx = s/2 - s*0.12f, hw = s*0.24f, hh = s*0.14f;
            g.DrawArc(p, hx, m, hw, hh * 2, 180, 180);
        }

        // Back — mũi tên trái
        static void DrawBack(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float cx = s / 2, cy = s / 2;
            float aw = s * 0.38f, ah = s * 0.26f;
            // Mũi tên
            var arrow = new PointF[] {
                new PointF(cx - aw, cy),
                new PointF(cx, cy - ah),
                new PointF(cx, cy - ah * 0.4f),
                new PointF(cx + aw * 0.6f, cy - ah * 0.4f),
                new PointF(cx + aw * 0.6f, cy + ah * 0.4f),
                new PointF(cx, cy + ah * 0.4f),
                new PointF(cx, cy + ah),
            };
            g.FillPolygon(b, arrow);
        }

        // Search — kính lúp
        static void DrawSearch(Graphics g, Pen p, float s, float m)
        {
            float r = s * 0.28f;
            float cx = s * 0.4f, cy = s * 0.4f;
            g.DrawEllipse(p, cx - r, cy - r, r * 2, r * 2);
            float hx = cx + r * 0.7f, hy = cy + r * 0.7f;
            float hlen = s * 0.22f;
            g.DrawLine(p, hx, hy, hx + hlen, hy + hlen);
        }

        // Login — người + mũi tên vào
        static void DrawLogin(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            // cửa
            float dw = s*0.38f, dh = s*0.6f;
            float dx = m, dy = (s - dh) / 2;
            g.DrawRectangle(p, dx, dy, dw, dh);
            // mũi tên vào
            float ax = dx + dw + s*0.06f, ay = s / 2;
            float aw = s * 0.3f, ah = s * 0.2f;
            g.DrawLine(p, ax, ay, ax + aw, ay);
            g.DrawLine(p, ax + aw - ah*0.6f, ay - ah*0.6f, ax + aw, ay);
            g.DrawLine(p, ax + aw - ah*0.6f, ay + ah*0.6f, ax + aw, ay);
        }

        // Logout — mũi tên ra ngoài
        static void DrawLogout(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float dw = s*0.38f, dh = s*0.6f;
            float dx = s - m - dw, dy = (s - dh) / 2;
            g.DrawRectangle(p, dx, dy, dw, dh);
            float ax = m, ay = s / 2;
            float aw = s * 0.3f, ah = s * 0.2f;
            g.DrawLine(p, ax + dw * 0.2f, ay, ax + dw*0.2f + aw, ay);
            g.DrawLine(p, ax + dw*0.2f + aw - ah*0.6f, ay - ah*0.6f, ax + dw*0.2f + aw, ay);
            g.DrawLine(p, ax + dw*0.2f + aw - ah*0.6f, ay + ah*0.6f, ax + dw*0.2f + aw, ay);
        }

        // Password — khóa
        static void DrawPassword(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            DrawKey(g, p, b, s, m);
        }

        // Key — chìa khóa
        static void DrawKey(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float r = s * 0.22f;
            float cx = s * 0.35f, cy = s * 0.38f;
            g.DrawEllipse(p, cx - r, cy - r, r * 2, r * 2);
            // thân chìa
            float kx = cx + r * 0.7f, ky = cy + r * 0.7f;
            float klen = s * 0.38f;
            g.DrawLine(p, kx, ky, kx + klen, ky + klen);
            // răng
            g.DrawLine(p, kx + klen * 0.45f, ky + klen * 0.45f,
                          kx + klen * 0.45f + s*0.1f, ky + klen * 0.45f - s*0.1f);
            g.DrawLine(p, kx + klen * 0.7f, ky + klen * 0.7f,
                          kx + klen * 0.7f + s*0.1f, ky + klen * 0.7f - s*0.1f);
        }

        // Settings — bánh răng
        static void DrawSettings(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float cx = s / 2, cy = s / 2;
            float ro = s * 0.38f, ri = s * 0.2f, teeth = 8;
            float tw = s * 0.13f;
            using (var path = new GraphicsPath())
            {
                for (int i = 0; i < teeth; i++)
                {
                    float angle1 = (float)(i * 360 / teeth - tw / ro * 57.3f);
                    float angle2 = (float)(i * 360 / teeth + tw / ro * 57.3f);
                    float a1r = angle1 * 3.14159f / 180;
                    float a2r = angle2 * 3.14159f / 180;
                    path.AddLine(
                        cx + ri * (float)System.Math.Cos(a1r), cy + ri * (float)System.Math.Sin(a1r),
                        cx + ro * (float)System.Math.Cos(a1r), cy + ro * (float)System.Math.Sin(a1r));
                    path.AddLine(
                        cx + ro * (float)System.Math.Cos(a1r), cy + ro * (float)System.Math.Sin(a1r),
                        cx + ro * (float)System.Math.Cos(a2r), cy + ro * (float)System.Math.Sin(a2r));
                    path.AddLine(
                        cx + ro * (float)System.Math.Cos(a2r), cy + ro * (float)System.Math.Sin(a2r),
                        cx + ri * (float)System.Math.Cos(a2r), cy + ri * (float)System.Math.Sin(a2r));
                }
                path.CloseFigure();
                g.FillPath(b, path);
                g.DrawPath(p, path);
            }
            // lỗ giữa
            using (var bg = new SolidBrush(Color.Transparent))
                g.FillEllipse(new SolidBrush(Color.FromArgb(200, b.Color.R > 200 ? 30 : 255,
                    b.Color.R > 200 ? 55 : 255, b.Color.R > 200 ? 100 : 255)),
                    cx - s*0.13f, cy - s*0.13f, s*0.26f, s*0.26f);
        }

        // User — người dùng
        static void DrawUser(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float cx = s / 2;
            float hr = s * 0.22f; // head radius
            float hy = m + hr + s*0.02f;
            g.DrawEllipse(p, cx - hr, hy, hr * 2, hr * 2);
            // thân
            float bw = s * 0.55f, bh = s * 0.28f;
            g.DrawArc(p, cx - bw/2, s - m - bh, bw, bh * 2, 180, 180);
        }

        // Users — nhiều người
        static void DrawUsers(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float hr = s * 0.18f;
            // người sau
            using (var p2 = new Pen(Color.FromArgb(160, p.Color.R, p.Color.G, p.Color.B), p.Width))
            {
                float cx2 = s * 0.62f;
                g.DrawEllipse(p2, cx2 - hr, m + s*0.04f, hr * 2, hr * 2);
                g.DrawArc(p2, cx2 - s*0.22f, s - m - s*0.25f, s*0.44f, s*0.5f, 180, 180);
            }
            // người trước
            float cx = s * 0.38f;
            g.DrawEllipse(p, cx - hr, m, hr * 2, hr * 2);
            g.DrawArc(p, cx - s*0.22f, s - m - s*0.28f, s*0.44f, s*0.56f, 180, 180);
        }

        // Exit — cửa + mũi tên
        static void DrawExit(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            // X trong vòng tròn
            float r = s * 0.36f, cx = s/2, cy = s/2;
            g.DrawEllipse(p, cx-r, cy-r, r*2, r*2);
            float d = r * 0.55f;
            g.DrawLine(p, cx-d, cy-d, cx+d, cy+d);
            g.DrawLine(p, cx+d, cy-d, cx-d, cy+d);
        }

        // Connect — plug
        static void DrawConnect(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            // 2 vòng tròn nhỏ nối với nhau (link/chain)
            float r = s * 0.18f, gap = s * 0.14f;
            float cy = s / 2;
            float cx1 = s/2 - gap - r, cx2 = s/2 + gap + r;
            g.DrawEllipse(p, cx1-r, cy-r, r*2, r*2);
            g.DrawEllipse(p, cx2-r, cy-r, r*2, r*2);
            g.DrawLine(p, cx1+r, cy, cx2-r, cy);
            // kiểm tra dấu check nhỏ ở góc
            float ck = s * 0.2f, ckx = s - m - ck, cky = s - m - ck;
            g.DrawLine(p, ckx, cky + ck*0.5f, ckx + ck*0.4f, cky + ck);
            g.DrawLine(p, ckx + ck*0.4f, cky + ck, ckx + ck, cky);
        }

        // Filter — phễu
        static void DrawFilter(Graphics g, Pen p, float s, float m)
        {
            float tw = s - m*2, bw = s*0.25f, h = s - m*2;
            var pts = new PointF[] {
                new PointF(m, m),
                new PointF(s-m, m),
                new PointF(s/2 + bw/2, s/2),
                new PointF(s/2 + bw/2, s-m),
                new PointF(s/2 - bw/2, s-m),
                new PointF(s/2 - bw/2, s/2),
                new PointF(m, m)
            };
            g.DrawLines(p, pts);
        }

        // Photo — camera
        static void DrawPhoto(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float bw = s*0.8f, bh = s*0.58f;
            float bx = (s-bw)/2, by = s*0.26f;
            // thân camera
            g.DrawRectangle(p, bx, by, bw, bh);
            // ống kính
            float lr = s*0.2f, cx = s/2, cy = by + bh/2;
            g.DrawEllipse(p, cx-lr, cy-lr, lr*2, lr*2);
            // hump
            float hw = s*0.28f, hh = s*0.14f;
            var hump = new RectangleF(cx - hw/2, by - hh, hw, hh);
            g.DrawRectangle(p, hump.X, hump.Y, hump.Width, hump.Height);
        }

        // Invoice — hóa đơn
        static void DrawInvoice(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float pw = s*0.62f, ph = s*0.76f;
            float px = (s-pw)/2, py = (s-ph)/2;
            g.DrawRectangle(p, px, py, pw, ph);
            float lx = px + s*0.1f, rw = pw - s*0.2f;
            float dy = ph / 5;
            for (int i = 1; i <= 3; i++)
                g.DrawLine(p, lx, py + dy*i, lx + rw, py + dy*i);
            // dog-ear
            float de = s*0.16f;
            g.DrawLine(p, px+pw-de, py, px+pw-de, py+de);
            g.DrawLine(p, px+pw-de, py+de, px+pw, py+de);
        }

        // Product — hộp sản phẩm
        static void DrawProduct(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float bw = s*0.6f, bh = s*0.5f;
            float bx = (s-bw)/2, by = s/2 - bh/2 + s*0.06f;
            // hộp dưới
            g.DrawRectangle(p, bx, by, bw, bh);
            // nắp
            float tw = bw*1.1f, th = bh*0.22f;
            float tx = (s-tw)/2;
            g.DrawRectangle(p, tx, by - th, tw, th);
            // dải giữa
            g.DrawLine(p, s/2, by-th, s/2, by);
        }

        // City — tòa nhà thành phố
        static void DrawCity(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            // Tòa nhà chính
            float bw = s*0.36f, bh = s*0.56f;
            float bx = (s-bw)/2, by = s - m - bh;
            g.DrawRectangle(p, bx, by, bw, bh);
            // Cửa sổ
            float ww = s*0.1f, wh = s*0.1f, wgap = s*0.08f;
            for (int row = 0; row < 2; row++)
                for (int col = 0; col < 2; col++)
                    g.DrawRectangle(p, bx + wgap + col*(ww+wgap), by + wgap + row*(wh+wgap), ww, wh);
            // Tòa nhỏ bên trái
            float sw = s*0.22f, sh = s*0.36f;
            g.DrawRectangle(p, m, s-m-sh, sw, sh);
            // Tòa nhỏ bên phải
            g.DrawRectangle(p, s-m-sw, s-m-sh, sw, sh);
        }

        // Chart — biểu đồ cột
        static void DrawChart(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float bw = s*0.16f, gap = s*0.06f;
            float by = s - m;
            float[] heights = { s*0.35f, s*0.55f, s*0.72f, s*0.45f };
            float totalW = heights.Length * (bw + gap) - gap;
            float startX = (s - totalW) / 2;
            for (int i = 0; i < heights.Length; i++)
            {
                float x = startX + i * (bw + gap);
                g.DrawRectangle(p, x, by - heights[i], bw, heights[i]);
            }
            // Trục X
            g.DrawLine(p, m, by, s-m, by);
        }

        // Help — dấu hỏi
        static void DrawHelp(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float r = s * 0.42f, cx = s/2, cy = s/2;
            g.DrawEllipse(p, cx-r, cy-r, r*2, r*2);
            // dấu ?
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var f = new Font("Segoe UI Semibold", s * 0.42f, FontStyle.Bold, GraphicsUnit.Pixel))
                g.DrawString("?", f, b, new RectangleF(m, m - s*0.04f, s-m*2, s-m*2), sf);
        }

        // Info — chữ i
        static void DrawInfo(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float r = s * 0.42f, cx = s/2, cy = s/2;
            g.DrawEllipse(p, cx-r, cy-r, r*2, r*2);
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var f = new Font("Segoe UI Semibold", s * 0.42f, FontStyle.Bold, GraphicsUnit.Pixel))
                g.DrawString("i", f, b, new RectangleF(m, m + s*0.02f, s-m*2, s-m*2), sf);
        }

        // Test — dấu check trong vòng tròn
        static void DrawTest(Graphics g, Pen p, SolidBrush b, float s, float m)
        {
            float r = s * 0.42f, cx = s/2, cy = s/2;
            g.DrawEllipse(p, cx-r, cy-r, r*2, r*2);
            // checkmark
            float ck = s * 0.22f;
            g.DrawLine(p, cx - ck, cy, cx - ck*0.2f, cy + ck*0.7f);
            g.DrawLine(p, cx - ck*0.2f, cy + ck*0.7f, cx + ck, cy - ck*0.5f);
        }
    }

    // ── Enum các loại icon ────────────────────────────────────
    public enum IconType
    {
        Reload, Add, Edit, Save, Cancel, Delete, Back,
        Search, Login, Logout, Password, Settings,
        User, Users, Exit, Connect, Filter, Photo,
        Invoice, Product, City, Chart, Help, Info, Key, Test
    }
}
