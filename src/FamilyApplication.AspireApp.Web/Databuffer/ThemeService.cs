using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    public string AccentColor { get; private set; } = "#4CAF50";
    public string FillColor { get; private set; } = "#C8E6C9";
    public string NeutralBaseColor { get; private set; } = "#FFFFFF";

    public string ValgtPalett { get; set; } = "Standard";

    public event EventHandler ThemeChanged;

    // Define palettes with lighter, playful colors for kids and subdued for adults
    public readonly Dictionary<string, (string Accent, string Fill, string Neutral)> _palettes = new()
    {
        { "Standard", ("#4CAF50", "#C8E6C9", "#FFFFFF") },
        { "Sprø Banan", ("#FFD700", "#FFFACD", "#FFFFF0") },  // Bright yellow banana
        { "Galaktisk Godteri", ("#FF99CC", "#FFC1CC", "#FFF0F5") },  // Light pink candy
        { "Søt Sky", ("#87CEFA", "#B0E0E6", "#F0F8FF") },  // Light blue sky
        { "Vill Vaffel", ("#FFA07A", "#FFEFD5", "#FFF5E1") },  // Light orange waffle
        { "Tøff Traktor", ("#98FB98", "#C1E1C1", "#F0FFF0") },  // Light green tractor
        { "Milde Morgen", ("#B0C4DE", "#E6E6FA", "#F5F5FF") },  // Soft pastel morning
        { "Stille Hav", ("#ADD8E6", "#E0FFFF", "#F0FFFF") },  // Light sea tones
        { "Dempet Drøm", ("#D8BFD8", "#F0E68C", "#FAFAD2") }, // Muted pastel dream
        { "Gal Kake", ("#FF6347", "#FFE4B5", "#FFF5EE") },  // Crazy cake - tomato orange
        { "Hoppende Hest", ("#DA70D6", "#DDA0DD", "#F8F1FF") },  // Jumping horse - orchid purple
        { "Søt Slange", ("#90EE90", "#98FB98", "#F0FFF0") },  // Sweet snake - light green
        { "Latterlig Løve", ("#FF8C00", "#FFDAB9", "#FFFACD") },  // Ridiculous lion - orange yellow
        { "Boble Bad", ("#00BFFF", "#B0E0E6", "#F0F8FF") },  // Bubble bath - deep sky blue
        { "Ro Lig Natt", ("#6495ED", "#E0FFFF", "#F5F5FF") },  // Calm night - soft blue
        { "Varm Kaffe", ("#D2B48C", "#F5F5DC", "#FAF0E6") },  // Warm coffee - tan beige
        { "Silly Sokker", ("#FF00FF", "#FFC0CB", "#FFF0F5") }   // Silly socks - magenta pink
    };

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task LoadUserPreferences(bool themeChangedEvent = true)
    {
        var savedPalette = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userPalette");
        if (!string.IsNullOrEmpty(savedPalette) && _palettes.ContainsKey(savedPalette))
        {
            var colors = _palettes[savedPalette];
            AccentColor = colors.Accent;
            FillColor = colors.Fill;
            NeutralBaseColor = colors.Neutral;
            ValgtPalett = savedPalette;
        }
        if (themeChangedEvent)
            ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task ApplyPalette(string paletteName)
    {
        if (_palettes.ContainsKey(paletteName))
        {
            var colors = _palettes[paletteName];
            AccentColor = colors.Accent;
            FillColor = colors.Fill;
            NeutralBaseColor = colors.Neutral;
            ValgtPalett = paletteName;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userPalette", paletteName);
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Console.WriteLine($"Invalid palette name: {paletteName}");
        }
    }

    public async Task ResetUserPreferences()
    {
        AccentColor = _palettes["Standard"].Accent;
        FillColor = _palettes["Standard"].Fill;
        NeutralBaseColor = _palettes["Standard"].Neutral;
        ValgtPalett = "Standard";


        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userPalette");

        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<string> GetCurrentPaletteName()
    {
        foreach (var kvp in _palettes)
        {
            if (kvp.Value.Accent == AccentColor && kvp.Value.Fill == FillColor && kvp.Value.Neutral == NeutralBaseColor)
            {
                return kvp.Key;
            }
        }
        return "Sprø Banan"; // Default if no match
    }
}