

using System.Globalization;
using System.Text;

namespace BaseCore.Common.Helpers
{

    // ════════════════════════════════════════════════════════════
    // HELPER TÌM KIẾM (CHUẨN HÓA VĂN BẢN)
    // ════════════════════════════════════════════════════════════

    public static class SearchHelper
    {

        
        
        public const string Collation = "Vietnamese_CI_AI";

        

        
        
        public static string Normalize(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var s = input.ToLowerInvariant().Replace('đ', 'd').Replace('Đ', 'd');
            var nfd = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(nfd.Length);
            foreach (var c in nfd)
            {
                
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
