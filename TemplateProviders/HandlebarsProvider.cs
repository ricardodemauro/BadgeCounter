using HandlebarsDotNet;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadgeCounters.HandlebarsTemplates
{
    public class HandlebarsProvider
    {
        readonly IFileProvider _fileProvider;

        readonly ILogger<HandlebarsProvider> _logger;

        public HandlebarsProvider(IFileProvider fileProvider, ILogger<HandlebarsProvider> logger)
        {
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async Task<string> GetTemplate(string template)
        {
            _logger.LogInformation("Loading template {Template}", template);

            var templateInfo = _fileProvider.GetFileInfo(template);

            using var stream = templateInfo.CreateReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var stringTemplate = await reader.ReadToEndAsync();

            return stringTemplate;
        }

        public async Task<string> GetHtml(string template, object data)
        {
            var stringTemplate = await GetTemplate(template);

            var compiledTemplate = Handlebars.Compile(stringTemplate);

            var output = compiledTemplate(data);

            return output;
        }
    }
}
