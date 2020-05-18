using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DisableSleep.Properties;
using Microsoft.Win32;

namespace DisableSleep
{
    [Flags]
    enum EXECUTION_STATE : uint
    {
        ES_CONTINUOUS = 0x80000000,
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001,
    }

    public class MyApplicationContext : ApplicationContext
    {
        private readonly bool useLightTheme;
        private readonly NotifyIcon trayIcon;
        private readonly ContextMenu awakeContextMenu;
        private readonly ContextMenu sleepContextMenu;

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags);

        public MyApplicationContext()
        {
            useLightTheme = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0) == 1;

            awakeContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Allow sleep", AllowSleep),
                    new MenuItem("Exit", Exit)
                });
            sleepContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Disable sleep", DisableSleep),
                    new MenuItem("Exit", Exit)
                });
            trayIcon = new NotifyIcon
            {
                Visible = true
            };
            DisableSleep(this, new EventArgs());
        }

        private void DisableSleep(object sender, EventArgs e)
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);

            trayIcon.Icon = (useLightTheme) ? Resources.IconDisableSleepLight : Resources.IconDisableSleepDark;
            trayIcon.ContextMenu = awakeContextMenu;
        }

        private void AllowSleep(object sender, EventArgs e)
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);

            trayIcon.Icon = (useLightTheme) ? Resources.IconAllowSleepLight : Resources.IconAllowSleepDark;
            trayIcon.ContextMenu = sleepContextMenu;
        }


        void Exit(object sender, EventArgs e)
        {
            AllowSleep(this, new EventArgs());
            trayIcon.Dispose();
            Application.Exit();
        }
    }
}
