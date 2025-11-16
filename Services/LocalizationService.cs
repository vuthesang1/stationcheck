using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILogger<LocalizationService> _logger;

        public LocalizationService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            ILogger<LocalizationService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        // Language Management
        public async Task<List<Language>> GetLanguagesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Languages
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<Language?> GetLanguageAsync(string code)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Languages
                .Include(l => l.Translations)
                .FirstOrDefaultAsync(l => l.Code == code);
        }

        public async Task<Language> CreateLanguageAsync(Language language)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            language.CreatedAt = DateTime.UtcNow;
            context.Languages.Add(language);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Created language {Code} - {Name}", language.Code, language.Name);
            return language;
        }

        public async Task<Language> UpdateLanguageAsync(string code, Language language)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var existing = await context.Languages.FindAsync(code);
            if (existing == null)
                throw new KeyNotFoundException($"Language {code} not found");
            
            existing.Name = language.Name;
            existing.NativeName = language.NativeName;
            existing.IsActive = language.IsActive;
            existing.IsDefault = language.IsDefault;
            existing.FlagIcon = language.FlagIcon;
            
            await context.SaveChangesAsync();
            _logger.LogInformation("Updated language {Code}", code);
            return existing;
        }

        public async Task DeleteLanguageAsync(string code)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var language = await context.Languages.FindAsync(code);
            if (language == null)
                throw new KeyNotFoundException($"Language {code} not found");
            
            context.Languages.Remove(language);
            await context.SaveChangesAsync();
            _logger.LogInformation("Deleted language {Code}", code);
        }

        // Translation Management
        public async Task<List<Translation>> GetTranslationsAsync(string languageCode, string? category = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var query = context.Translations
                .Where(t => t.LanguageCode == languageCode);
            
            if (!string.IsNullOrEmpty(category))
                query = query.Where(t => t.Category == category);
            
            return await query
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Key)
                .ToListAsync();
        }

        public async Task<Translation?> GetTranslationAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Translations
                .Include(t => t.Language)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Translation> CreateTranslationAsync(Translation translation)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            translation.CreatedAt = DateTime.UtcNow;
            context.Translations.Add(translation);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Created translation {Key} for {LanguageCode}", 
                translation.Key, translation.LanguageCode);
            return translation;
        }

        public async Task<Translation> UpdateTranslationAsync(int id, Translation translation)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var existing = await context.Translations.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Translation {id} not found");
            
            existing.Value = translation.Value;
            existing.Category = translation.Category;
            existing.ModifiedAt = DateTime.UtcNow;
            
            await context.SaveChangesAsync();
            _logger.LogInformation("Updated translation {Id} - {Key}", id, existing.Key);
            return existing;
        }

        public async Task DeleteTranslationAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var translation = await context.Translations.FindAsync(id);
            if (translation == null)
                throw new KeyNotFoundException($"Translation {id} not found");
            
            context.Translations.Remove(translation);
            await context.SaveChangesAsync();
            _logger.LogInformation("Deleted translation {Id}", id);
        }

        // Helper methods for runtime translation
        public async Task<string> GetTranslationValueAsync(string key, string languageCode)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var translation = await context.Translations
                .FirstOrDefaultAsync(t => t.Key == key && t.LanguageCode == languageCode);
            
            return translation?.Value ?? key; // Return key if translation not found
        }

        public async Task<Dictionary<string, string>> GetAllTranslationsForLanguageAsync(string languageCode)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.Translations
                .Where(t => t.LanguageCode == languageCode)
                .ToDictionaryAsync(t => t.Key, t => t.Value);
        }
    }
}
