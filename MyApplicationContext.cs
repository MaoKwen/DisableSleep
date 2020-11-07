// using DisableSleep.Properties;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

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
    
        private readonly NotifyIcon trayIcon;
        private readonly ContextMenuStrip disableContextMenu;
        private readonly ContextMenuStrip allowContextMenu;

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags);

        public MyApplicationContext()
        {
            disableContextMenu = new ContextMenuStrip();
            disableContextMenu.Items.Add(new ToolStripMenuItem("Allow sleep", null, AllowSleep));
            disableContextMenu.Items.Add(new ToolStripMenuItem("Exit", null, Exit));

            allowContextMenu = new ContextMenuStrip();
            allowContextMenu.Items.Add(new ToolStripMenuItem("Disable sleep", null, DisableSleep));
            allowContextMenu.Items.Add(new ToolStripMenuItem("Exit", null, Exit));

            trayIcon = new NotifyIcon {
                Visible = true
            };
            DisableSleep(this, new EventArgs());
        }

        private void DisableSleep(object sender, EventArgs e)
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);

            trayIcon.Icon = GetIcon(false);
            trayIcon.ContextMenuStrip = disableContextMenu;
        }

        private void AllowSleep(object sender, EventArgs e)
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);

            trayIcon.Icon = GetIcon(true);
            trayIcon.ContextMenuStrip = allowContextMenu;
        }

        private void Exit(object sender, EventArgs e)
        {
            AllowSleep(this, new EventArgs());
            trayIcon.Dispose();
            Application.Exit();
        }

        private readonly bool useLightTheme = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0) == 1;
        private System.Drawing.Icon GetIcon(bool isAllow) {
            if (isAllow == false) {
                if (useLightTheme) {
                    return new System.Drawing.Icon("icons/disable-sleep-light.ico");
                } else{
                   return new System.Drawing.Icon("icons/disable-sleep-dark.ico");
                }
            } else {
                if (useLightTheme) {
                    return new System.Drawing.Icon("icons/allow-sleep-light.ico");
                } else{
                    return new System.Drawing.Icon("icons/allow-sleep-dark.ico");
                }
            }
        }
    }
}
