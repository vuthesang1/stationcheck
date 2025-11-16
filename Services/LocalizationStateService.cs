using StationCheck.Interfaces;

namespace StationCheck.Services
{
    public class LocalizationStateService
    {
        private readonly ILocalizationService _localizationService;
        private Dictionary<string, string> _translations = new();
        private string _currentLanguage = "vi";
        private bool _isLoaded = false;

        public event Action? OnLanguageChanged;

        public string CurrentLanguage => _currentLanguage;
        public bool IsLoaded => _isLoaded;

        public LocalizationStateService(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public async Task InitializeAsync(string? preferredLanguage = null)
        {
            Console.WriteLine($"[LocalizationStateService.InitializeAsync] START - Language: {preferredLanguage}");
            
            if (!string.IsNullOrEmpty(preferredLanguage))
                _currentLanguage = preferredLanguage;

            await LoadTranslationsAsync();
            _isLoaded = true;
            
            Console.WriteLine($"[LocalizationStateService.InitializeAsync] DONE - Loaded {_translations.Count} translations for '{_currentLanguage}'");
        }

        public async Task SetLanguageAsync(string languageCode)
        {
            _currentLanguage = languageCode;
            await LoadTranslationsAsync();
            _isLoaded = true;
            OnLanguageChanged?.Invoke();
        }

        public async Task RefreshTranslationsAsync()
        {
            await LoadTranslationsAsync();
            OnLanguageChanged?.Invoke();
        }

        private async Task LoadTranslationsAsync()
        {
            Console.WriteLine($"[LocalizationStateService.LoadTranslationsAsync] Loading for language: {_currentLanguage}");
            _translations = await _localizationService.GetAllTranslationsForLanguageAsync(_currentLanguage);
            Console.WriteLine($"[LocalizationStateService.LoadTranslationsAsync] Loaded {_translations.Count} translations");
            
            // Debug: print first 5 translations
            foreach (var kvp in _translations.Take(5))
            {
                Console.WriteLine($"  - {kvp.Key} = {kvp.Value}");
            }
        }

        public string this[string key]
        {
            get
            {
                if (_translations.TryGetValue(key, out var value))
                    return value;

                return key; // Return key if translation not found
            }
        }

        public string T(string key) => this[key];

        public string GetText(string key, string defaultText)
        {
            if (!_isLoaded)
                return defaultText;

            var translation = this[key];
            return !string.IsNullOrEmpty(translation) && translation != key ? translation : defaultText;
        }
    }
}
