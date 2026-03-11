using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Shared;

namespace RegalEdu.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IHttpContextAccessor _httpContext;

        public LocalizationService(IStringLocalizer<Messages> localizer, IHttpContextAccessor httpContext)
        {
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _httpContext = httpContext ?? throw new ArgumentNullException (nameof (httpContext));
        }

        public string this[string key] => _localizer[key];

        public string Format(string key, params object[] arguments)
        {
            var formattedArgs = NormalizeArguments (arguments);
            return _localizer[key, formattedArgs];
        }

        public string FormatWithCulture(string key, string cultureName, params object[] arguments)
        {
            var culture = string.IsNullOrWhiteSpace (cultureName)
                ? CultureInfo.InvariantCulture
                : new CultureInfo (cultureName);
            var formattedArgs = NormalizeArguments (arguments);
            var originalCulture = CultureInfo.CurrentCulture;
            var originalUiCulture = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                return _localizer[key, formattedArgs];
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUiCulture;
            }
        }

        public string GetCurrentLanguage( )
        {
            var lang = _httpContext.HttpContext?.Request.Headers["Accept-Language"].ToString ( );

            if (string.IsNullOrEmpty (lang))
                return "vi"; // default

            if (lang.StartsWith ("en", StringComparison.OrdinalIgnoreCase))
                return "en";

            return "vi"; // fallback
        }

        private static object[] NormalizeArguments(object[] arguments)
        {
            return arguments
                .Select (arg => arg is Enum e ? e.ToString ( ) : arg?.ToString ( ) ?? string.Empty)
                .Cast<object> ( )
                .ToArray ( );
        }
    }
}
