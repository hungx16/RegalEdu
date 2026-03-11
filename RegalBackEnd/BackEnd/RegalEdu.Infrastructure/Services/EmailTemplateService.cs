using RazorLight;
using RegalEdu.Application.Common.Interfaces;
using System.Globalization;

namespace RegalEdu.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly RazorLightEngine _engine;

        public EmailTemplateService( )
        {
            _engine = new RazorLightEngineBuilder ( )
                .UseFileSystemProject (Path.Combine (Directory.GetCurrentDirectory ( ), "EmailTemplates"))
                .UseMemoryCachingProvider ( )
                .Build ( );
        }

        public async Task<string> RenderTemplateAsync<TModel>(string templateName, TModel model)
        {
            try
            {
                var currentCulture = CultureInfo.CurrentUICulture.Name;
                string templatePath = Path.Combine (currentCulture, $"{templateName}.cshtml");

                if (!File.Exists (Path.Combine ("EmailTemplates", templatePath)))
                {
                    // Fallback sang tiếng Anh nếu không có template culture hiện tại
                    templatePath = Path.Combine ("en", $"{templateName}.cshtml");
                }

                return await _engine.CompileRenderAsync (templatePath, model);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
