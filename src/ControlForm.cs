using System;
using System.Drawing;
using System.Windows.Forms;
namespace MotionCuesPrototype
{
    internal sealed class ControlForm : Form
    {
        readonly CueSettings settings; readonly Action<bool> suspendOverlay; readonly Action exitAction; bool syncing;
        CheckBox enabled, invert; TrackBar interval, size, count, movement, opacity; Label intervalValue, sizeValue, countValue, moveValue, opacityValue; Button colorButton;
        readonly Color textColor = Color.FromArgb(30, 36, 48); readonly Color mutedColor = Color.FromArgb(82, 91, 110); readonly Font regularFont = new Font("Segoe UI", 10f);
        public ControlForm(CueSettings s, Action<bool> suspend, Action exit) { settings = s; suspendOverlay = suspend; exitAction = exit; Build(); Sync(); settings.Changed += delegate { Sync(); }; }
        protected override void OnFormClosing(FormClosingEventArgs e) { if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; Hide(); return; } base.OnFormClosing(e); }
        void Build()
        {
            Text = "Motion Cues"; ClientSize = new Size(700, 828); MinimumSize = new Size(716, 867); StartPosition = FormStartPosition.CenterScreen; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false; BackColor = Color.FromArgb(242, 245, 250); Font = regularFont; AutoScaleMode = AutoScaleMode.Dpi; TopMost = true;
            Panel header = new Panel { Location = new Point(0, 0), Size = new Size(700, 130), BackColor = Color.FromArgb(25, 31, 45) }; Controls.Add(header);
            header.Controls.Add(new Label { Text = "Motion Cues", ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 22f, FontStyle.Bold), AutoSize = true, Location = new Point(25, 13) });
            header.Controls.Add(new Label { Text = "Private, offline, full-screen visual motion for passengers", ForeColor = Color.FromArgb(207, 216, 235), Font = new Font("Segoe UI", 10.5f), Location = new Point(28, 76), Size = new Size(640, 32) });
            enabled = new CheckBox { Text = "Show motion cues on screen", Font = new Font("Segoe UI Semibold", 11f, FontStyle.Bold), ForeColor = textColor, AutoSize = true, Location = new Point(25, 147) }; enabled.CheckedChanged += HandleEnabledChanged; Controls.Add(enabled);

            GroupBox flowCard = Card("Flow", 183, 126);
            Slider(flowCard, "Motion interval", 30, 2, 30, out interval, out intervalValue); interval.ValueChanged += IntervalChanged;
            invert = new CheckBox { Text = "Invert overall direction", Font = regularFont, ForeColor = textColor, AutoSize = true, Location = new Point(20, 80), Padding = new Padding(0, 2, 0, 2) }; invert.CheckedChanged += InvertChanged; flowCard.Controls.Add(invert);

            GroupBox appearanceCard = Card("Appearance", 322, 390);
            Slider(appearanceCard, "Dot size", 30, 6, 42, out size, out sizeValue); size.ValueChanged += HandleSizeChanged;
            Slider(appearanceCard, "Dots on screen", 82, 12, 80, out count, out countValue); count.ValueChanged += CountChanged;
            Slider(appearanceCard, "Flow speed", 134, 12, 110, out movement, out moveValue); movement.ValueChanged += MovementChanged;
            Slider(appearanceCard, "Opacity", 186, 60, 255, out opacity, out opacityValue); opacity.ValueChanged += OpacityChanged;
            appearanceCard.Controls.Add(new Label { Text = "Dot color", Font = regularFont, ForeColor = textColor, Location = new Point(20, 268), Size = new Size(125, 28) });
            colorButton = new Button { Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(155, 258), Size = new Size(370, 44), UseVisualStyleBackColor = false, Cursor = Cursors.Hand }; colorButton.FlatAppearance.BorderColor = Color.FromArgb(110, 119, 138); colorButton.FlatAppearance.BorderSize = 2; colorButton.Click += PickColor; appearanceCard.Controls.Add(colorButton);
            
            
            appearanceCard.Controls.Add(new Label { Text = "Presets", Font = regularFont, ForeColor = textColor, Location = new Point(20, 326), Size = new Size(125, 28) });
            AddColorPresets(appearanceCard);

            Button reset = MainButton("Reset defaults", 22, 730, 130); reset.Click += delegate { settings.Reset(); };
            Button hide = MainButton("Hide controls", 446, 730, 128); hide.Click += delegate { Hide(); };
            Button quit = MainButton("Exit", 584, 730, 94); quit.Click += delegate { exitAction(); };
            Controls.Add(new Label { Text = "Offline mode  •  Settings save automatically  •  No network access", Font = new Font("Segoe UI", 9.5f), ForeColor = mutedColor, Location = new Point(25, 790), Size = new Size(650, 24) });
        }
        GroupBox Card(string title, int y, int height) { GroupBox box = new GroupBox { Text = title, Font = new Font("Segoe UI Semibold", 10.5f, FontStyle.Bold), ForeColor = textColor, BackColor = Color.White, Location = new Point(22, y), Size = new Size(656, height), Padding = new Padding(14) }; Controls.Add(box); return box; }
        Button MainButton(string text, int x, int y, int width) { Button b = new Button { Text = text, Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold), Location = new Point(x, y), Size = new Size(width, 38) }; Controls.Add(b); return b; }
        void Slider(Control parent, string text, int y, int min, int max, out TrackBar track, out Label value)
        {
            parent.Controls.Add(new Label { Text = text, Font = regularFont, ForeColor = textColor, Location = new Point(20, y + 4), Size = new Size(135, 27) });
            track = new TrackBar { Minimum = min, Maximum = max, TickStyle = TickStyle.None, Location = new Point(155, y - 2), Size = new Size(370, 36) }; parent.Controls.Add(track);
            value = new Label { Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold), ForeColor = textColor, TextAlign = ContentAlignment.MiddleRight, Location = new Point(545, y), Size = new Size(80, 29) }; parent.Controls.Add(value);
        }
        void AddColorPresets(Control parent)
        {
            Color[] colors = new Color[] { Color.FromArgb(96,92,255), Color.FromArgb(30,136,229), Color.FromArgb(0,172,193), Color.FromArgb(46,160,67), Color.FromArgb(255,145,0), Color.FromArgb(235,68,90), Color.FromArgb(245,245,245), Color.FromArgb(38,39,43) };
            for (int i = 0; i < colors.Length; i++)
            {
                Button b = new Button { BackColor = colors[i], Tag = colors[i], Location = new Point(155 + i * 52, 316), Size = new Size(38, 38), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, TabStop = false };
                b.FlatAppearance.BorderColor = Color.FromArgb(150, 157, 171); b.FlatAppearance.BorderSize = 1; b.Click += PresetColorClick; parent.Controls.Add(b);
            }
        }
        void Sync()
        {
            if (IsDisposed) return; syncing = true; enabled.Checked = settings.Enabled; invert.Checked = settings.InvertMotion; interval.Value = settings.CycleSeconds; size.Value = settings.DotSize; count.Value = settings.DotCount; movement.Value = settings.Sensitivity; opacity.Value = settings.Opacity;
            intervalValue.Text = settings.CycleSeconds + " s"; sizeValue.Text = settings.DotSize + " px"; countValue.Text = settings.DotCount.ToString(); moveValue.Text = settings.Sensitivity + "%"; opacityValue.Text = Math.Round(settings.Opacity / 255.0 * 100) + "%"; colorButton.BackColor = settings.DotColor; colorButton.ForeColor = ContrastColor(settings.DotColor); colorButton.Text = "Choose custom color   " + ColorHex(settings.DotColor); syncing = false;
        }
        void Commit() { if (!syncing) settings.NotifyChanged(); }
        void HandleEnabledChanged(object s, EventArgs e) { settings.Enabled = enabled.Checked; Commit(); }
        void IntervalChanged(object s, EventArgs e) { settings.CycleSeconds = interval.Value; intervalValue.Text = settings.CycleSeconds + " s"; Commit(); }
        void InvertChanged(object s, EventArgs e) { settings.InvertMotion = invert.Checked; Commit(); }
        void HandleSizeChanged(object s, EventArgs e) { settings.DotSize = size.Value; sizeValue.Text = settings.DotSize + " px"; Commit(); }
        void CountChanged(object s, EventArgs e) { settings.DotCount = count.Value; countValue.Text = settings.DotCount.ToString(); Commit(); }
        void MovementChanged(object s, EventArgs e) { settings.Sensitivity = movement.Value; moveValue.Text = settings.Sensitivity + "%"; Commit(); }
        void OpacityChanged(object s, EventArgs e) { settings.Opacity = opacity.Value; opacityValue.Text = Math.Round(settings.Opacity / 255.0 * 100) + "%"; Commit(); }
        void PresetColorClick(object sender, EventArgs e) { Button b = sender as Button; if (b != null && b.Tag is Color) ApplyColor((Color)b.Tag); }
        void ApplyColor(Color color) { settings.DotColor = color; settings.NotifyChanged(); }
        void PickColor(object sender, EventArgs e)
        {
            suspendOverlay(true);
            try
            {
                using (ColorDialog d = new ColorDialog { Color = settings.DotColor, FullOpen = true, AnyColor = true, SolidColorOnly = true })
                    if (d.ShowDialog(this) == DialogResult.OK) ApplyColor(d.Color);
            }
            finally { suspendOverlay(false); BringToFront(); Activate(); }
        }
        static string ColorHex(Color c) { return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2"); }
        static Color ContrastColor(Color c) { return (c.R * 299 + c.G * 587 + c.B * 114) / 1000 >= 150 ? Color.FromArgb(24, 30, 42) : Color.White; }
    }
}
