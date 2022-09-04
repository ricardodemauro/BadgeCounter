using BadgeCounters.Db;
using BadgeCounters.HandlebarsTemplates;
using BadgeCounters.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BadgeCounters.Endpoints;

public static class ProfileCountEndpoints
{
    internal static readonly Dictionary<string, string> ColorMapping = new Dictionary<string, string>
    {
        ["brightgreen"] = "#44cc11",
        ["green"] = "#97ca00",
        ["yellowgreen"] = "#a4a61d",
        ["yellow"] = "#dfb317",
        ["orange"] = "#fe7d37",
        ["red"] = "#e05d44",
        ["blue"] = "#007ec6",
        ["lightgray"] = "#9f9f9f",
        ["lightgrey"] = "#9f9f9f",
        ["gray"] = "#555555",
        ["grey"] = "#555555",
        ["blueviolet"] = "#8a2be2",
        ["success"] = "#97ca00",
        ["important"] = "#fe7d37",
        ["critical"] = "#e05d44",
        ["informational"] = "#007ec6",
        ["inactive"] = "#9f9f9f"
    };

    public static IEndpointConventionBuilder MapHandlebars(
       this IEndpointRouteBuilder endpoints,
       string pattern)
    {
        return endpoints.MapGet($"{pattern}/{{profile}}",
            async (
                HandlebarsProvider handleProvider,
                ApplicationDbContext dbContext,
                string profile,
                HttpContext context,
                CancellationToken cancellationToken
                ) =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dbProfile = new GithubProfile(0, profile, 1, true, DateTime.Now);
            dbContext.Profiles.Add(dbProfile);

            await dbContext.SaveChangesAsync(cancellationToken);

            var count = await dbContext.Profiles.CountAsync(
                x => x.Name == profile && x.Active == true,
                cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var Color = context.GetHexColorQuery() ?? context.GetColorQuery();
            var label = context.GetLabelQuery();

            string stringCount = count.ToString("0,0", CultureInfo.CurrentCulture);

            int CounterWidth = stringCount.Length * 10;
            int FullWidth = CounterWidth + 80;
            int colorWidth = CounterWidth - 8;
            int ConterPosition = 80 + CounterWidth / 2;

            var arguments = new
            {
                Profile = profile,
                Label = label,
                Count = stringCount,
                Color,
                FullWidth,
                CounterWidth,
                ConterPosition
            };

            var output = await handleProvider.GetHtml("regular_template.hbs", arguments);

            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(output, cancellationToken);
            await context.Response.CompleteAsync();
        });
    }


}
