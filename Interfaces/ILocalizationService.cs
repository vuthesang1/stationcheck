using StationCheck.Models;

namespace StationCheck.Interfaces
{
    public interface ILocalizationService
    {
        Task<List<Language>> GetLanguagesAsync();
        Task<Language?> GetLanguageAsync(string code);
        Task<Language> CreateLanguageAsync(Language language);
        Task<Language> UpdateLanguageAsync(string code, Language language);
        Task DeleteLanguageAsync(string code);
        
        Task<List<Translation>> GetTranslationsAsync(string languageCode, string? category = null);
        Task<Translation?> GetTranslationAsync(int id);
        Task<Translation> CreateTranslationAsync(Translation translation);
        Task<Translation> UpdateTranslationAsync(int id, Translation translation);
        Task DeleteTranslationAsync(int id);
        
        Task<string> GetTranslationValueAsync(string key, string languageCode);
        Task<Dictionary<string, string>> GetAllTranslationsForLanguageAsync(string languageCode);
    }
}
