using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Reflection;

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
            trayIcon.Icon = GetIcon(false);
            trayIcon.ContextMenuStrip = disableContextMenu;
            DisableSleep(this, new EventArgs());
        }

        private void DisableSleep(object sender, EventArgs e)
        {
            var r = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            if (r == 0) return;

            trayIcon.Icon = GetIcon(false);
            trayIcon.ContextMenuStrip = disableContextMenu;
        }

        private void AllowSleep(object sender, EventArgs e)
        {
            var r = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            if (r == 0) return;

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
            var assembly = typeof(MyApplicationContext).GetTypeInfo().Assembly;
            if (isAllow == false) {
                if (useLightTheme) {
                    var resource = assembly.GetManifestResourceStream("Icon.disable-sleep-light");
                    return new System.Drawing.Icon(resource);
                } else{
                    var resource = assembly.GetManifestResourceStream("Icon.disable-sleep-dark");
                   return new System.Drawing.Icon(resource);
                }
            } else {
                if (useLightTheme) {
                    var resource = assembly.GetManifestResourceStream("Icon.allow-sleep-light");
                    return new System.Drawing.Icon(resource);
                } else{
                    var resource = assembly.GetManifestResourceStream("Icon.allow-sleep-dark");
                    return new System.Drawing.Icon(resource);
                }
            }
        }
    }
}
