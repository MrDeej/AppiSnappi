using Blazored.LocalStorage;

namespace FamilyApplication.AspireApp.Web.Services
{
    public class ThemeService : IAsyncDisposable
    {
        public string CurrentAccent { get; private set; } = "#1A7B27"; // Default
        public string CurrentNeutral { get; private set; } = "#C8E6C9"; // Default

        public event Func<Task>? ThemeChanged;

        private readonly ILocalStorageService _localStorage;

        public ThemeService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task ApplyPaletteAsync(ThemePalette palette)
        {
            CurrentAccent = palette.AccentHex;
            CurrentNeutral = palette.BackgroundHex;
            await _localStorage.SetItemAsync("selectedTheme", palette.Name);
            await (ThemeChanged?.Invoke() ?? Task.CompletedTask); // Notify subscribers
        }

        public async Task LoadSavedPaletteAsync()
        {
            var savedName = await _localStorage.GetItemAsync<string>("selectedTheme");
            if (!string.IsNullOrEmpty(savedName))
            {
                var savedPalette = ThemeValidator.SafePalettes.FirstOrDefault(p => p.Name == savedName);
                if (savedPalette != null)
                {
                    await ApplyPaletteAsync(savedPalette);
                }
            }
        }

        ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;
    }
}
