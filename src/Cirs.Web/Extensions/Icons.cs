using Microsoft.AspNetCore.Html;

namespace Cirs.Web.Extensions;

// Small inline SVG icon set, so the pages need no icon font or external download.
public static class Icons
{
    private static readonly Dictionary<string, string> Shapes = new()
    {
        ["mail"] = "<rect x=\"3\" y=\"5\" width=\"18\" height=\"14\" rx=\"2\"/><path d=\"m3 7 9 6 9-6\"/>",
        ["lock"] = "<rect x=\"5\" y=\"11\" width=\"14\" height=\"9\" rx=\"2\"/><path d=\"M8 11V8a4 4 0 0 1 8 0v3\"/>",
        ["user"] = "<circle cx=\"12\" cy=\"8\" r=\"4\"/><path d=\"M4 20c0-4 4-6 8-6s8 2 8 6\"/>",
        ["eye"] = "<path d=\"M2 12s4-7 10-7 10 7 10 7-4 7-10 7S2 12 2 12z\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/>",
        ["eye-off"] = "<path d=\"M3 3l18 18\"/><path d=\"M10.6 6.1A9.7 9.7 0 0 1 12 5c6 0 10 7 10 7a17 17 0 0 1-3.2 3.9M6.4 7.5A17 17 0 0 0 2 12s4 7 10 7a9.6 9.6 0 0 0 4-.9\"/><path d=\"M9.9 9.9a3 3 0 0 0 4.2 4.2\"/>",
        ["alert"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M12 7.5v5.5\"/><circle cx=\"12\" cy=\"16.4\" r=\".6\" fill=\"currentColor\"/>",
        ["check"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"m8 12.5 2.8 2.8L16.2 9.5\"/>",
        ["network"] = "<circle cx=\"6\" cy=\"6\" r=\"2.4\"/><circle cx=\"18\" cy=\"7\" r=\"2.4\"/><circle cx=\"12\" cy=\"18\" r=\"2.4\"/><path d=\"M8 7l8 .6M7.4 8l3.4 8M16.8 9l-3.6 7.2\"/>",
        ["target"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><circle cx=\"12\" cy=\"12\" r=\"4.5\"/><circle cx=\"12\" cy=\"12\" r=\"1\" fill=\"currentColor\"/>",
        ["route"] = "<circle cx=\"6\" cy=\"18\" r=\"2.2\"/><circle cx=\"18\" cy=\"6\" r=\"2.2\"/><path d=\"M8.2 18H15a3 3 0 0 0 0-6H9a3 3 0 0 1 0-6h6.8\"/>",
        ["layers"] = "<path d=\"m12 3 9 5-9 5-9-5 9-5z\"/><path d=\"m3 13 9 5 9-5\"/>"
    };

    public static IHtmlContent Svg(string name)
    {
        Shapes.TryGetValue(name, out var body);

        return new HtmlString(
            "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.8\" " +
            "stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\" focusable=\"false\">" +
            body + "</svg>");
    }
}
