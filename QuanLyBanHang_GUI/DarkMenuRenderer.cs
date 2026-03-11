using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Custom renderer: MenuStrip nền xanh đậm, chữ trắng.
    /// Dropdown nền trắng, chữ tối, hover xanh nhạt.
    /// </summary>
    public class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        private static readonly Color NavBg = Color.FromArgb(30, 55, 100);
        private static readonly Color NavHover = Color.FromArgb(50, 80, 140);
        private static readonly Color NavActive = Color.FromArgb(20, 42, 82);
        private static readonly Color DropBg = Color.White;
        private static readonly Color DropHover = Color.FromArgb(225, 235, 255);
        private static readonly Color DropBorder = Color.FromArgb(200, 210, 230);
        private static readonly Color SeparatorClr = Color.FromArgb(215, 220, 235);

        public DarkMenuRenderer() : base(new DarkMenuColorTable()) { }

        // ── MenuStrip background ─────────────────────────────
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is MenuStrip)
            {
                using (var brush = new SolidBrush(NavBg))
                    e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
            else
            {
                base.OnRenderToolStripBackground(e);
            }
        }

        // ── Dropdown background ──────────────────────────────
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            // ẩn dải icon trái mặc định
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is ToolStripDropDown)
            {
                using (var pen = new System.Drawing.Pen(DropBorder))
                    e.Graphics.DrawRectangle(pen, 0, 0,
                        e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            }
        }

        // ── Top-level item hover/selected ────────────────────
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var item = e.Item;
            var bounds = new Rectangle(Point.Empty, item.Size);

            if (item.OwnerItem == null) // top-level
            {
                Color bg = item.Pressed ? NavActive
                         : item.Selected ? NavHover
                         : NavBg;
                using (var brush = new SolidBrush(bg))
                    e.Graphics.FillRectangle(brush, bounds);
            }
            else // dropdown item
            {
                if (item.Selected)
                {
                    using (var brush = new SolidBrush(DropHover))
                        e.Graphics.FillRectangle(brush, bounds);
                    using (var pen = new System.Drawing.Pen(Color.FromArgb(180, 200, 240)))
                        e.Graphics.DrawRectangle(pen,
                            bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
                else
                {
                    using (var brush = new SolidBrush(DropBg))
                        e.Graphics.FillRectangle(brush, bounds);
                }
            }
        }

        // ── Separator ────────────────────────────────────────
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using (var pen = new System.Drawing.Pen(SeparatorClr))
                e.Graphics.DrawLine(pen, 8, y, e.Item.Width - 8, y);
        }

        // ── Arrow (submenu indicator) ─────────────────────────
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = e.Item.OwnerItem == null ? Color.White : Color.FromArgb(80, 100, 140);
            base.OnRenderArrow(e);
        }
    }

    internal class DarkMenuColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Color.FromArgb(225, 235, 255);
        public override Color MenuItemBorder => Color.FromArgb(180, 200, 240);
        public override Color MenuBorder => Color.FromArgb(200, 210, 230);
        public override Color ToolStripDropDownBackground => Color.White;
        public override Color ImageMarginGradientBegin => Color.White;
        public override Color ImageMarginGradientMiddle => Color.White;
        public override Color ImageMarginGradientEnd => Color.White;
    }
}