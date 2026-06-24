

using System.Globalization;

using System.Text;

using System.Text.RegularExpressions;

namespace BaseCore.Common.Helpers
{

    // ════════════════════════════════════════════════════════════
    // HELPER SLUG (TẠO CHUỖI THÂN THIỆN URL)
    // ════════════════════════════════════════════════════════════

    public static class SlugHelper
    {

        

        public static string Generate(string input)
        {

            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            
            
            var s = input.Trim().ToLowerInvariant();

            

            s = s.Replace('đ', 'd').Replace('Đ', 'd');

            
            
            var normalized = s.Normalize(NormalizationForm.FormD);

            
            var sb = new StringBuilder(normalized.Length);

            foreach (var c in normalized)
            {

                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            
            s = sb.ToString().Normalize(NormalizationForm.FormC);

            
            
            s = Regex.Replace(s, @"[^a-z0-9\s-]", "");

            
            s = Regex.Replace(s, @"[\s-]+", "-").Trim('-');

            return s;
        }
    }
}
