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
    class MainForm : Form
    {
        NotifyIcon notifyIcon;
        Form clockForm;
        CalendarForm calendarForm;

        public MainForm()
        {
            // タスクトレイに格納
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Resources.Icon;
            notifyIcon.Text = "Clock";

            // コンテキストメニューを追加

            MenuItem miDigital = new MenuItem("デジタル時計を使う(&D)");
            miDigital.Click +=
                (seneder, e) =>
                {
                    clockForm.Close();
                    if (!Settings.Default.DigitalClockFlag) clockForm = new DigitalClockForm();
                    else clockForm = new ClockForm();
                    clockForm.Show();
                    miDigital.Checked = !miDigital.Checked;
                    Settings.Default.DigitalClockFlag = miDigital.Checked;
                };

            MenuItem miClock = new MenuItem("時計を表示(&C)");
            miClock.Click += 
                (seneder, e) => 
                {
                    if (!miClock.Checked)
                    {
                        if (Settings.Default.DigitalClockFlag) clockForm = new DigitalClockForm();
                        else clockForm = new ClockForm();

                        clockForm.Show();
                    }
                    else clockForm.Close();
                    miClock.Checked = !miClock.Checked;
                    Settings.Default.ClockVisible = miClock.Checked;
                };

            MenuItem miCalendar = new MenuItem("カレンダーを表示(&V)");
            miCalendar.Click +=
                (seneder, e) =>
                {
                    if (!miCalendar.Checked)
                    {
                        calendarForm = new CalendarForm();
                        calendarForm.Show();
                    }
                    else calendarForm.Close();
                    miCalendar.Checked = !miCalendar.Checked;
                    Settings.Default.CalendarVisible = miCalendar.Checked;
                };

            MenuItem miExit = new MenuItem("終了(&X)");
            miExit.Click += (seneder, e) => { Close(); };

            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { miDigital, new MenuItem("-"), miClock, miCalendar, new MenuItem("-"), miExit });

            
            // 設定の表示フラグによってフォームを追加する
            this.Load += (sender, e) =>
            {
                notifyIcon.Visible = true;
                if (Settings.Default.ClockVisible)
                {
                    miDigital.Checked = Settings.Default.DigitalClockFlag;
                    if (Settings.Default.DigitalClockFlag) clockForm = new DigitalClockForm();
                    else clockForm = new ClockForm();
                    clockForm.Show();
                    miClock.Checked = true;
                }
                if (Settings.Default.CalendarVisible)
                {
                    calendarForm = new CalendarForm();
                    calendarForm.Show();
                    miCalendar.Checked = true;
                }
            };

            // クリックされた時に最前面に表示する
            notifyIcon.Click += (sender, e) => Activate();

            // タスクバーに表示しない、最小化を無効に
            ShowInTaskbar = false;
            MinimizeBox = false;

            // 非表示にする
            this.Opacity = 0;

            // 終了時に設定を保存
            this.FormClosing += (sender, e) => Settings.Default.Save();
        }
    }
}
