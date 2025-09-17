using System.Drawing;

namespace FamilyApplication.AspireApp.Web.Services
{
    public static class ThemeValidator
    {
        // Updated with more childish, vibrant, and playful color choices, focusing on light pastels and avoiding dark backgrounds
        // Added defaults from screenshot: background #F0FFF0 (light green), accent #008000 (green for top bar)
        public static readonly string[] BackgroundColors = new[]
        {
            "#FFFFFF", // white
            "#FFFFE0", // light yellow
            "#E0FFE0", // light green
            "#EDF4FD", // light blue
            "#FFE4E1", // light pink
            "#E6E6FA", // light purple
            "#FFE4B5", // light orange
            "#E0FFFF", // light cyan
            "#FFDEAD", // light peach
            "#C8E6C9"  // greenish (default background from screenshot)
        };

        public static readonly string[] AccentColors = new[]
        {
            "#0078D4", // blue
            "#FFB900", // yellow
            "#107C10", // green
            "#D13438", // red
            "#FF69B4", // hot pink
            "#5B5FC6", // purple
            "#FF4500", // orange
            "#00FA9A", // spring green
            "#FF00FF", // magenta
            "#008000"  // green (default accent from screenshot top bar)
        };

        // Precompute safe combos (run once at app startup)
        public static List<ThemePalette> SafePalettes { get; private set; } = new();

        // Default palette from screenshot, to be used if none are chosen
        public static ThemePalette DefaultPalette { get; } = new ThemePalette
        {
            Name = "Default",
            BackgroundHex = "#F0FFF0",
            AccentHex = "#008000",
            BackgroundColor = ColorTranslator.FromHtml("#F0FFF0"),
            AccentColor = ColorTranslator.FromHtml("#008000"),
            IsValid = true
        };

        public static void Initialize()
        {
            SafePalettes.Clear();
            // Add default first
            SafePalettes.Add(DefaultPalette);
            for (int bgIdx = 0; bgIdx < BackgroundColors.Length; bgIdx++)
            {
                var bgHex = BackgroundColors[bgIdx];
                var bgColor = ColorTranslator.FromHtml(bgHex);
                var bgLuminance = GetRelativeLuminance(bgColor);
                var textColor = bgLuminance > 0.179 ? Color.Black : Color.White; // Dynamic text based on bg
                var bgContrast = GetContrastRatio(bgColor, textColor);

                if (bgContrast < 4.5) continue; // Skip invalid bgs (e.g., mid-gray might fail)

                for (int accIdx = 0; accIdx < AccentColors.Length; accIdx++)
                {
                    var accHex = AccentColors[accIdx];
                    var accColor = ColorTranslator.FromHtml(accHex);
                    var accLuminance = GetRelativeLuminance(accColor);
                    var accentTextColor = accLuminance > 0.179 ? Color.Black : Color.White; // Dynamic text for accent
                    var accContrast = GetContrastRatio(accColor, accentTextColor);
                    if (accContrast >= 4.5)
                    {
                        // Additional check: Accent vs bg luminance delta > 0.3 for visibility
                        if (Math.Abs(accLuminance - bgLuminance) > 0.3)
                        {
                            // Skip adding the default again
                            if (bgHex == DefaultPalette.BackgroundHex && accHex == DefaultPalette.AccentHex) continue;

                            SafePalettes.Add(new ThemePalette
                            {
                                Name = $"Theme {SafePalettes.Count}",
                                BackgroundHex = bgHex,
                                AccentHex = accHex,
                                BackgroundColor = bgColor,
                                AccentColor = accColor,
                                IsValid = true
                            });
                        }
                    }
                }
            }
        }

        public static double GetRelativeLuminance(Color c)
        {
            double r = c.R / 255.0, g = c.G / 255.0, b = c.B / 255.0;
            r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
            g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
            b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private static double GetContrastRatio(Color c1, Color c2)
        {
            double l1 = GetRelativeLuminance(c1), l2 = GetRelativeLuminance(c2);
            return (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05);
        }
    }
    public class ThemePalette
    {
        public string Name { get; set; } = string.Empty;
        public string BackgroundHex { get; set; } = string.Empty;
        public string AccentHex { get; set; } = string.Empty;
        public Color BackgroundColor { get; set; }
        public Color AccentColor { get; set; }
        public bool IsValid { get; set; }
    }
}