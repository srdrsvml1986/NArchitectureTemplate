namespace WebAPI;

using System;
using System.Text.Json;

public static class JsonExtensions
{
    public static bool IsValidJson(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim();

        // Quick check for JSON object or array
        if ((input.StartsWith("{") && input.EndsWith("}")) ||
            (input.StartsWith("[") && input.EndsWith("]")))
        {
            try
            {
                JsonDocument.Parse(input);
                return true;
            }
            catch (JsonException)
            {
                // Invalid JSON syntax
            }
        }

        return false;
    }
}