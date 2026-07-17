using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
namespace MotionCuesPrototype
{
    internal sealed class MotionCuesContext : ApplicationContext
    {
        static readonly IntPtr HwndTopmost = new IntPtr(-1);
        const uint SwpNoSize = 0x0001, SwpNoMove = 0x0002, SwpNoActivate = 0x0010, SwpShowWindow = 0x0040;
        [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr hwnd, IntPtr insertAfter, int x, int y, int cx, int cy, uint flags);
        readonly CueSettings settings; readonly MotionEngine engine; readonly List<OverlayWindow> overlays = new List<OverlayWindow>(); readonly Timer timer; readonly NotifyIcon tray; readonly ToolStripMenuItem showItem; readonly ControlForm controls; bool exiting, overlaySuspended; int zOrderTicks;
        public MotionCuesContext()
        {
            settings = CueSettings.Load(); engine = new MotionEngine(); settings.Changed += SettingsChanged;
            showItem = new ToolStripMenuItem("Show motion cues") { CheckOnClick = true }; showItem.Click += delegate { settings.Enabled = showItem.Checked; settings.NotifyChanged(); };
            ContextMenuStrip menu = new ContextMenuStrip(); menu.Items.Add("Open controls", null, delegate { ShowControls(); }); menu.Items.Add(new ToolStripSeparator()); menu.Items.Add(showItem); menu.Items.Add(new ToolStripSeparator()); menu.Items.Add("Exit", null, delegate { RequestExit(); });
            tray = new NotifyIcon { Icon = SystemIcons.Information, Text = "Motion Cues", ContextMenuStrip = menu, Visible = true }; tray.DoubleClick += delegate { ShowControls(); };
            controls = new ControlForm(settings, SetOverlaySuspended, RequestExit); controls.Show(); CreateOverlays(); SyncMenu();
            timer = new Timer { Interval = 33 }; timer.Tick += Tick; timer.Start(); SystemEvents.DisplaySettingsChanged += DisplayChanged; KeepControlsAboveOverlay();
        }
        void CreateOverlays() { foreach (Screen screen in Screen.AllScreens) { OverlayWindow o = new OverlayWindow(screen, settings); overlays.Add(o); if (settings.Enabled && !overlaySuspended) o.Show(); o.RefreshAppearance(); if (overlaySuspended) o.Hide(); } }
        void DestroyOverlays() { foreach (OverlayWindow o in overlays) o.Close(); overlays.Clear(); }
        void Tick(object sender, EventArgs e) { if (!overlaySuspended) { MotionVector motion = engine.Update(settings.CycleSeconds, settings.InvertMotion); foreach (OverlayWindow o in overlays) o.UpdateMotion(motion); } if (++zOrderTicks >= 15) { zOrderTicks = 0; KeepControlsAboveOverlay(); } }
        void SettingsChanged(object sender, EventArgs e) { settings.Save(); foreach (OverlayWindow o in overlays) { o.RefreshAppearance(); if (overlaySuspended) o.Hide(); } SyncMenu(); KeepControlsAboveOverlay(); }
        void SetOverlaySuspended(bool suspended) { overlaySuspended = suspended; foreach (OverlayWindow o in overlays) { if (suspended) o.Hide(); else if (settings.Enabled) { o.Show(); o.RefreshAppearance(); } } KeepControlsAboveOverlay(); }
        void SyncMenu() { showItem.Checked = settings.Enabled; }
        void ShowControls() { if (!controls.Visible) controls.Show(); if (controls.WindowState == FormWindowState.Minimized) controls.WindowState = FormWindowState.Normal; KeepControlsAboveOverlay(); controls.BringToFront(); controls.Activate(); }
        void KeepControlsAboveOverlay() { if (!controls.Visible || controls.IsDisposed || !controls.IsHandleCreated) return; SetWindowPos(controls.Handle, HwndTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpNoActivate | SwpShowWindow); }
        void DisplayChanged(object sender, EventArgs e) { DestroyOverlays(); CreateOverlays(); }
        void RequestExit() { if (exiting) return; exiting = true; ExitThread(); }
        protected override void ExitThreadCore() { SystemEvents.DisplaySettingsChanged -= DisplayChanged; timer.Stop(); timer.Dispose(); settings.Save(); DestroyOverlays(); tray.Visible = false; tray.Dispose(); controls.Dispose(); base.ExitThreadCore(); }
    }
}
