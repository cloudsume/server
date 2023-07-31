namespace Candidate.Globalization;

using System.Collections.Generic;
using System.Globalization;

public static class RegionNames
{
    private static readonly Dictionary<string, string> Estonia = new()
    {
        { "en", "Estonia" },
        { "th", "เอสโตเนีย" },
    };

    private static readonly Dictionary<string, string> India = new()
    {
        { "en", "India" },
        { "th", "อินเดีย" },
    };

    private static readonly Dictionary<string, string> Japan = new()
    {
        { "en", "Japan" },
        { "th", "ญี่ปุ่น" },
    };

    private static readonly Dictionary<string, string> Nigeria = new()
    {
        { "en", "Nigeria" },
        { "th", "ไนจีเรีย" },
    };

    private static readonly Dictionary<string, string> Pakistan = new()
    {
        { "en", "Pakistan" },
        { "th", "ปากีสถาน" },
    };

    private static readonly Dictionary<string, string> Philippines = new()
    {
        { "en", "Philippines" },
        { "th", "ฟิลิปปินส์" },
    };

    private static readonly Dictionary<string, string> Singapore = new()
    {
        { "en", "Singapore" },
        { "th", "สิงคโปร์" },
    };

    private static readonly Dictionary<string, string> SouthKorea = new()
    {
        { "en", "South Korea" },
        { "th", "เกาหลีใต้" },
    };

    private static readonly Dictionary<string, string> Thailand = new()
    {
        { "en", "Thailand" },
        { "th", "ไทย" },
    };

    private static readonly Dictionary<string, string> UnitedStates = new()
    {
        { "en", "United States" },
        { "th", "สหรัฐอเมริกา" },
    };

    private static readonly Dictionary<string, string> Vietnam = new()
    {
        { "en", "Vietnam" },
        { "th", "เวียดนาม" },
    };

    private static readonly Dictionary<string, Dictionary<string, string>> Table = new()
    {
        { "EE", Estonia },
        { "IN", India },
        { "JP", Japan },
        { "KR", SouthKorea },
        { "NG", Nigeria },
        { "PH", Philippines },
        { "PK", Pakistan },
        { "SG", Singapore },
        { "TH", Thailand },
        { "US", UnitedStates },
        { "VN", Vietnam },
    };

    public static string? FindByCode(string code, CultureInfo culture)
    {
        if (!Table.TryGetValue(code, out var table))
        {
            return null;
        }

        if (!table.TryGetValue(culture.TwoLetterISOLanguageName, out var name))
        {
            return null;
        }

        return name;
    }
}
