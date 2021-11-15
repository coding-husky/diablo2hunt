using System.Text.RegularExpressions;

namespace D2Hunt.App.Infrastructure.Helpers;

public static class StringHelper
{
    public static bool IsValidIpAddress(string? input) => Regex.IsMatch(input ?? string.Empty, @"(\b25[0-5]|\b2[0-4][0-9]|\b[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}");
}