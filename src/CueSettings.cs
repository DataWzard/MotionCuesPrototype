using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
namespace MotionCuesPrototype
{
    internal sealed class CueSettings
    {
        public bool Enabled = true;
        public bool InvertMotion = false;
        public int CycleSeconds = 8;
        public int DotSize = 16;
        public int DotCount = 32;
        public int Sensitivity = 52;
        public int Opacity = 205;
        public Color DotColor = Color.FromArgb(96, 92, 255);
        public event EventHandler Changed;
        public static string SettingsPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MotionCuesPrototype", "settings.txt"); } }
        public void NotifyChanged() { Clamp(); EventHandler h = Changed; if (h != null) h(this, EventArgs.Empty); }
        public void Reset() { Enabled = true; InvertMotion = false; CycleSeconds = 8; DotSize = 16; DotCount = 32; Sensitivity = 52; Opacity = 205; DotColor = Color.FromArgb(96, 92, 255); NotifyChanged(); }
        public void Save()
        {
            try
            {
                string d = Path.GetDirectoryName(SettingsPath); if (!Directory.Exists(d)) Directory.CreateDirectory(d);
                File.WriteAllLines(SettingsPath, new string[] { "SettingsVersion=5", "Enabled=" + Enabled, "InvertMotion=" + InvertMotion, "CycleSeconds=" + CycleSeconds, "DotSize=" + DotSize, "DotCount=" + DotCount, "Sensitivity=" + Sensitivity, "Opacity=" + Opacity, "DotColor=" + DotColor.ToArgb() });
            }
            catch { }
        }
        public static CueSettings Load()
        {
            CueSettings r = new CueSettings();
            try
            {
                if (!File.Exists(SettingsPath)) return r;
                Dictionary<string, string> v = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string line in File.ReadAllLines(SettingsPath)) { int s = line.IndexOf('='); if (s > 0) v[line.Substring(0, s).Trim()] = line.Substring(s + 1).Trim(); }
                bool b; int n;
                if (Get(v, "Enabled", out b)) r.Enabled = b; if (Get(v, "InvertMotion", out b)) r.InvertMotion = b;
                if (Get(v, "CycleSeconds", out n)) r.CycleSeconds = n; if (Get(v, "DotSize", out n)) r.DotSize = n; if (Get(v, "DotCount", out n)) r.DotCount = n; if (Get(v, "Sensitivity", out n)) r.Sensitivity = n; if (Get(v, "Opacity", out n)) r.Opacity = n; if (Get(v, "DotColor", out n)) r.DotColor = Color.FromArgb(n);
                int version; if (!Get(v, "SettingsVersion", out version) || version < 3) r.DotCount = 32;
            }
            catch { return new CueSettings(); }
            r.Clamp(); return r;
        }
        static bool Get(Dictionary<string, string> v, string k, out bool x) { x = false; string t; return v.TryGetValue(k, out t) && bool.TryParse(t, out x); }
        static bool Get(Dictionary<string, string> v, string k, out int x) { x = 0; string t; return v.TryGetValue(k, out t) && int.TryParse(t, NumberStyles.Integer, CultureInfo.InvariantCulture, out x); }
        void Clamp() { CycleSeconds = Math.Max(2, Math.Min(30, CycleSeconds)); DotSize = Math.Max(6, Math.Min(42, DotSize)); DotCount = Math.Max(12, Math.Min(80, DotCount)); Sensitivity = Math.Max(12, Math.Min(110, Sensitivity)); Opacity = Math.Max(60, Math.Min(255, Opacity)); }
    }
}
