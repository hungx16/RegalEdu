using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for localization service.
    /// Abstract localization from API layer for Clean Architecture.
    /// </summary>
    public interface ILocalizationService
    {
        string this[string key] { get; }
        string this[Enum key] => this[key.ToString ( )];

        string Format(string key, params object[] arguments);
        string FormatWithCulture(string key, string cultureName, params object[] arguments);
        // Gợi ý mở rộng thêm cho enum
        string this[LocalizationKey key] => this[key.ToString ( )];
        string Format(LocalizationKey key, params object[] arguments) => Format (key.ToString ( ), arguments);
        public string GetCurrentLanguage( );
    }
}
