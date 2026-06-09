

using System.ComponentModel.DataAnnotations;

namespace BaseCore.Common
{

    // ════════════════════════════════════════════════════════════
    // KIỂU ENUM DÙNG CHUNG
    // ════════════════════════════════════════════════════════════

    public class Enums
    {

        
        public enum MediaType
        {
            [Display(Name = "Không xác định")]
            Unkown = 0,   

            [Display(Name = "Hình ảnh")]
            Image = 1,    

            [Display(Name = "Video")]
            Video = 2,    

            [Display(Name = "Doc")]
            Doc = 3,      

            [Display(Name = "Pdf")]
            Pdf = 4,      

            [Display(Name = "File")]
            File = 5,     
        }
    }
}
