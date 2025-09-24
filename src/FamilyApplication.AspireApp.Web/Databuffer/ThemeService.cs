using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    public string AccentColor { get;  set; } = "#4CAF50"; // Original green from --primary-color
    public string FillColor { get;  set; } = "#C8E6C9"; // Original green from --primary-color
    public string NeutralBaseColor { get;  set; } = "#FFFFFF"; // Original light green from --secondary-color
    public event EventHandler ThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task LoadUserPreferences()
    {
        var savedAccent = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userAccentColor");
        if (!string.IsNullOrEmpty(savedAccent) && Regex.IsMatch(savedAccent, @"^#[0-9A-Fa-f]{6}$"))
        {
            AccentColor = savedAccent;
        }

        var savedFill = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userFillColor");
        if (!string.IsNullOrEmpty(savedFill) && Regex.IsMatch(savedFill, @"^#[0-9A-Fa-f]{6}$"))
        {
            FillColor = savedFill;
        }

        var savedNeutral = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userNeutralBaseColor");
        if (!string.IsNullOrEmpty(savedNeutral) && Regex.IsMatch(savedNeutral, @"^#[0-9A-Fa-f]{6}$"))
        {
            NeutralBaseColor = savedNeutral;
        }

        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task SaveAndApplyAccentColor(string newAccent)
    {
        if (Regex.IsMatch(newAccent, @"^#[0-9A-Fa-f]{6}$"))
        {
            AccentColor = newAccent;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userAccentColor", newAccent);
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Console.WriteLine($"Invalid accent color format: {newAccent}");
        }
    }

    public async Task SaveAndApplyFillColor(string newFill)
    {
        if (Regex.IsMatch(newFill, @"^#[0-9A-Fa-f]{6}$"))
        {
            FillColor = newFill;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userFillColor", newFill);
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Console.WriteLine($"Invalid fill color format: {newFill}");
        }
    }

    public async Task SaveAndApplyNeutralBaseColor(string newNeutral)
    {
        if (Regex.IsMatch(newNeutral, @"^#[0-9A-Fa-f]{6}$"))
        {
            NeutralBaseColor = newNeutral;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userNeutralBaseColor", newNeutral);
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Console.WriteLine($"Invalid neutral base color format: {newNeutral}");
        }
    }

    public async Task ResetUserPreferences()
    {
        // Reset to original colors from app.css
        AccentColor = "#4CAF50";
        FillColor = "#4CAF50";
        NeutralBaseColor = "#C8E6C9";

        // Clear from localStorage
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userAccentColor");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userFillColor");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userNeutralBaseColor");

        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }
}