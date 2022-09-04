namespace BadgeCounters.Endpoints;

public static class HttpContextExtensions
{
    public static string GetColorQuery(this HttpContext ctx)
    {
        if (ctx.Request.Query.ContainsKey("color"))
        {
            var colorQuery = ctx.Request.Query["color"];
            if (ProfileCountEndpoints.ColorMapping.ContainsKey(colorQuery))
                return ProfileCountEndpoints.ColorMapping[colorQuery];
        }
        return ProfileCountEndpoints.ColorMapping["blue"];
    }

    public static string GetHexColorQuery(this HttpContext ctx)
    {
        if (ctx.Request.Query.ContainsKey("hex-color"))
            return $"#{ctx.Request.Query["hex-color"]}";

        return null;
    }

    public static string GetLabelQuery(this HttpContext ctx)
    {
        if (ctx.Request.Query.ContainsKey("label"))
            return ctx.Request.Query["label"];

        return "Profile views";
    }
}
