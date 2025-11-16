using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StationCheck.Services;

namespace StationCheck.Components
{
    public class Localizer : ComponentBase, IAsyncDisposable
    {
        [Inject] private LocalizationStateService StateService { get; set; } = null!;
        [Inject] private IJSRuntime JS { get; set; } = null!;
        
        public string CurrentLanguage => StateService?.CurrentLanguage ?? "vi";
        public bool IsLoaded => StateService?.IsLoaded ?? false;
        
        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("[Localizer.OnInitializedAsync] START");
            
            // Detect browser language
            string browserLanguage = "vi";
            try
            {
                var detected = await JS.InvokeAsync<string>("eval", "navigator.language || navigator.userLanguage");
                Console.WriteLine($"[Localizer] Detected browser language: {detected}");
                
                // Map browser language codes
                if (detected.StartsWith("vi"))
                    browserLanguage = "vi";
                else if (detected.StartsWith("en"))
                    browserLanguage = "en";
                else
                    browserLanguage = "vi"; // Default to Vietnamese
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Localizer] Error detecting language: {ex.Message}");
                browserLanguage = "vi"; // Fallback
            }
            
            // Load from localStorage if exists
            try
            {
                var savedLanguage = await JS.InvokeAsync<string>("localStorage.getItem", "preferredLanguage");
                if (!string.IsNullOrEmpty(savedLanguage))
                {
                    browserLanguage = savedLanguage;
                    Console.WriteLine($"[Localizer] Loaded language from localStorage: {browserLanguage}");
                }
            }
            catch { }
            
            Console.WriteLine($"[Localizer] Initializing with language: {browserLanguage}");
            await StateService.InitializeAsync(browserLanguage);
            Console.WriteLine($"[Localizer.OnInitializedAsync] DONE - IsLoaded={StateService.IsLoaded}");
            StateHasChanged();
        }
        
        public async Task SetLanguageAsync(string languageCode)
        {
            // Save to localStorage
            try
            {
                await JS.InvokeVoidAsync("localStorage.setItem", "preferredLanguage", languageCode);
            }
            catch { }
            
            await StateService.SetLanguageAsync(languageCode);
            StateHasChanged();
        }
        
        public async Task RefreshTranslationsAsync()
        {
            if (StateService != null)
            {
                await StateService.RefreshTranslationsAsync();
                StateHasChanged();
            }
        }
        
        public string this[string key] => StateService?[key] ?? key;
        
        public string T(string key) => StateService?.T(key) ?? key;
        
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
