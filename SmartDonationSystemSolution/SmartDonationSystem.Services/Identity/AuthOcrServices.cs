using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using SmartDonationSystem.Core.Auth.DTOs;
using SmartDonationSystem.Core.Auth.Interfaces;
using Tesseract;
namespace SmartDonationSystem.Services.Identity;

public class AuthOcrService : IAuthOcrService
{
    public async Task<ExtractedIdentityDto> ExtractAsync(IFormFile image)
    {
        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        var bytes = ms.ToArray();

        var tessDataPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "tessdata"
        );

        using var engine = new TesseractEngine(
            tessDataPath,
            "ara",
            EngineMode.Default
        );

        engine.DefaultPageSegMode = PageSegMode.Auto;

        using var img = Pix.LoadFromMemory(bytes);
        using var page = engine.Process(img);

        var text = page.GetText();

        var nationalId = ExtractNationalId(text);

        return new ExtractedIdentityDto
        {
            IdentityNumber = nationalId,
            BirthDate = ExtractBirthDateFromNationalId(nationalId),
            UserName = ExtractName(text)
        };
    }


    private string? ExtractNationalId(string text)
    {
        var match = Regex.Match(text, @"[\d\u0660-\u0669]{14}");
        return match.Success ? NormalizeDigits(match.Value) : null;
    }

    private DateTime? ExtractBirthDateFromNationalId(string? id)
    {
        if (string.IsNullOrEmpty(id) || id.Length != 14)
            return null;

        id = NormalizeDigits(id);

        int century = id[0] == '2' ? 1900 : 2000;
        int year = century + int.Parse(id.Substring(1, 2));
        int month = int.Parse(id.Substring(3, 2));
        int day = int.Parse(id.Substring(5, 2));

        try
        {
            return new DateTime(year, month, day);
        }
        catch
        {
            return null;
        }
    }

    private string? ExtractName(string text)
    {
        var match = Regex.Match(
            text,
            @"الاسم\s*[:\-]?\s*([^\n]+)"
        );

        return match.Success
            ? match.Groups[1].Value.Trim()
            : null;
    }

    private string NormalizeDigits(string input)
    {
        return input
            .Replace('٠', '0')
            .Replace('١', '1')
            .Replace('٢', '2')
            .Replace('٣', '3')
            .Replace('٤', '4')
            .Replace('٥', '5')
            .Replace('٦', '6')
            .Replace('٧', '7')
            .Replace('٨', '8')
            .Replace('٩', '9');
    }
}
