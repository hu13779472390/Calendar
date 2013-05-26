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
    public partial class CalendarForm : WidgetBase
    {
        Timer timer;

        public CalendarForm()
        {
            // 読み込み時、描画時に再描画
            Load += (sender, e) => Render();
            Paint += (sender, e) => Render();

            // 1日ごとにリフレッシュ
            timer = new Timer();
            var now = DateTime.Now;
            timer.Tick += (sender, e) =>
            {
                timer.Interval = (24 * 60 * 60 * 1000)
                    - (now.Hour * 60 * 60 * 1000)
                    - (now.Minute * 60 * 1000)
                    - (now.Second * 1000); 
                Refresh();
            };
            timer.Interval = 1;
            timer.Start();

            // 座標を取得、更新する
            this.StartPosition = FormStartPosition.Manual;
            this.Left = Settings.Default.CalendarPos.X;
            this.Top = Settings.Default.CalendarPos.Y;
            Move += (sender, e) => Settings.Default.CalendarPos = new Point(Left, Top);

            // タスクバーに表示しない、最小化を無効に
            ShowInTaskbar = false;
            MinimizeBox = false;
        }

        private void Render()
        {
            Bitmap bitMap = new Bitmap(240, 200);

            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 255, 255, 255)), 0, 0, 230, 160);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 255, 255, 255)), 130, 165, 120, 40);

            // 今日の日付の枠の描画
            var now = DateTime.Now;
            var start = (int)new DateTime(now.Year, now.Month, 1).DayOfWeek;
            var nowOfWeek = (int)now.DayOfWeek;
            graphics.DrawRectangle(new Pen(Color.Black, 1), 10 + 30 * nowOfWeek, 10 + 30 * ((start + now.Day - 1) / 7), 25, 25);

            // 年・月の描画
            var font = new Font("メイリオ", 12);
            graphics.DrawString(now.Year + "年 " + now.Month + "月", font, Brushes.Black, 140, 170);

            // 日にちの描画
            foreach (var i in Enumerable.Range(1, DateTime.DaysInMonth(now.Year, now.Month)))
            {
                var day = new DateTime(now.Year, now.Month, i);
                var row = (start + i - 1) / 7;
                var ofWeek = (int)day.DayOfWeek;
                Brush brush;
                switch (ofWeek)
                {
                    case 0:
                        brush = Brushes.Red;
                        break;
                    case 6:
                        brush = Brushes.Blue;
                        break;
                    default:
                        brush = Brushes.Black;
                        break;
                }
                graphics.DrawString(i.ToString(), font, brush, 10 + 30 * ofWeek, 10 + 30 * row);
            }

            // 画像の適用
            SetLayeredWindow(bitMap);
        }
    }
}