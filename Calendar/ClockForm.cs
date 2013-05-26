using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Calendar.Util;
using Calendar.Properties;

namespace Calendar
{
    public partial class ClockForm : WidgetBase
    {
        Timer timer;

        // コンストラクタ
        public ClockForm()
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
            Bitmap bitMap = new Bitmap(200, 200);
            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 円弧（枠）の描画
            DrawCircleCenter(graphics, new Pen(Color.FromArgb(100, 255, 255, 255), 40), 100, 100, 40);
            DrawCircleCenter(graphics, new Pen(Color.Black, 10), 100, 100, 30);
            DrawCircleCenter(graphics, new Pen(Color.Black, 2), 100, 100, 50);

            foreach (var i in Enumerable.Range(0, 12))
            {
                var angle = i / 12.0 * (Math.PI * 2) - Math.PI / 2;
                graphics.DrawLine(new Pen(Color.Black, 1), 
                                  100 + (int)(30 * Math.Cos(angle)), 100 + (int)(30 * Math.Sin(angle)),
                                  100 + (int)(50 * Math.Cos(angle)), 100 + (int)(50 * Math.Sin(angle)));
            }

            // 時間の描画
            DateTime dt = DateTime.Now;
            var second = dt.Second;
            DrawHand(graphics, new Pen(Color.Red, 1), 100, 100, 100, dt.Second / 60.0 * (Math.PI * 2) - Math.PI / 2);
            DrawHand(graphics, new Pen(Color.Black, 2), 100, 100, 80, dt.Minute / 60.0 * (Math.PI * 2) - Math.PI / 2);
            DrawHand(graphics, new Pen(Color.Black, 4), 100, 100, 60, dt.Hour / 12.0 * (Math.PI * 2) - Math.PI / 2);

            SetLayeredWindow(bitMap);
        }

    }
}