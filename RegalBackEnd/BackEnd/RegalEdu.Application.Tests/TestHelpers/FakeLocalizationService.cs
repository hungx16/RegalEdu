using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.Tests.TestHelpers
{
    public class FakeLocalizationService : ILocalizationService
    {
        public string this[string key] => key;

        public string Format(string key, params object[] args)
        {
            if (args == null || args.Length == 0)
                return key;

            return $"{key}: {string.Join (", ", args)}";
        }

        public string GetCurrentLanguage( )
        {
            throw new NotImplementedException ( );
        }
    }
}
