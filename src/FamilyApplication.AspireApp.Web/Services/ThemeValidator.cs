using System.Drawing;

namespace FamilyApplication.AspireApp.Web.Services
{
    public static class ThemeValidator
    {
        // Extracted from app.css (update if your var names/hexes differ)
        public static readonly string[] BackgroundColors = new[]
        {
        "#FFFFFF", // --bg1
        "#F3F2F1", // --bg2
        "#E1DFDD", // --bg3
        "#605E5C", // --bg4
        "#323130", // --bg5
        "#000000", // --bg6
        "#FAF9F8", // --bg7
        "#EDF4FD"  // --bg8
    };

        public static readonly string[] AccentColors = new[]
        {
        "#0078D4", // --accent1
        "#107C10", // --accent2
        "#D13438", // --accent3
        "#FFB900", // --accent4
        "#5B5FC6"  // --accent5
    };

        // Precompute safe combos (run once at app startup)
        public static List<ThemePalette> SafePalettes { get; private set; } = new();

        public static void Initialize()
        {
            SafePalettes.Clear();
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
                    // Check accent bg with white text (common for buttons)
                    var accContrast = GetContrastRatio(accColor, Color.White);
                    if (accContrast >= 4.5)
                    {
                        // Additional check: Accent vs bg luminance delta > 0.3 for visibility
                        var accLuminance = GetRelativeLuminance(accColor);
                        if (Math.Abs(accLuminance - bgLuminance) > 0.3)
                        {
                            SafePalettes.Add(new ThemePalette
                            {
                                Name = $"Theme {SafePalettes.Count + 1}",
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
