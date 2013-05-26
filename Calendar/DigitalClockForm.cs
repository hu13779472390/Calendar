using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Calendar.Util;
using Calendar.Properties;

namespace Calendar
{
    public partial class DigitalClockForm : WidgetBase
    {
        Timer timer;

        // コンストラクタ
        public DigitalClockForm()
        {
            // 読み込み時、描画時に再描画
            Load += (sender, e) => Render();
            Paint += (sender, e) => Render();

            // 1秒ごとにリフレッシュ
            timer = new Timer();
            timer.Tick += (sender, e) => { timer.Interval = 1000 - DateTime.Now.Millisecond; Refresh(); };
            timer.Interval = 1;
            timer.Start();

            // 座標を取得、更新する
            this.StartPosition = FormStartPosition.Manual;
            this.Left = Settings.Default.ClockPos.X;
            this.Top = Settings.Default.ClockPos.Y;
            Move += (sender, e) => Settings.Default.ClockPos = new Point(Left, Top);

            // タスクバーに表示しない、最小化を無効に
            ShowInTaskbar = false;
            MinimizeBox = false;
        }

        // 中心点を指定して再描画する
        void DrawCircleCenter(Graphics graphics, Pen pen, int x, int y, int r)
        {
            graphics.DrawArc(pen, x - r, y - r, r * 2 - 1,  r * 2 - 1, 0, 360);
        }

        void DrawHand(Graphics graphics, Pen pen, int x, int y, int r, double angle)
        {
            graphics.DrawLine(pen, x + (int)(r * Math.Cos(angle)), y + (int)(r * Math.Sin(angle)), x, y);
        }

        // 再描画する
        void Render()
        {
            // ビットマップの取得
            Bitmap bitMap = new Bitmap(200, 50);
            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            // 枠の描画
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 255, 255, 255)), 0, 0, 200, 50);

            // 数字の描画
            DateTime now = DateTime.Now;
            string amPm;
            if (now.Hour <= 12) amPm = "AM";
            else amPm = "PM";
            graphics.DrawString(amPm + " " + now.Hour % 12 + " : " + now.Minute + " : " + now.Second, new Font("メイリオ", 16), Brushes.Black, 10, 10);

            SetLayeredWindow(bitMap);
        }

    }
}