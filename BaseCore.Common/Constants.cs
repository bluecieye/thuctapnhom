

namespace BaseCore.Common
{

    // ════════════════════════════════════════════════════════════
    // HẰNG SỐ TOÀN CỤC
    // ════════════════════════════════════════════════════════════

    public static class Constants
    {

        // ════════════════════════════════════════════════════════════
        // PHÂN TRANG
        // ════════════════════════════════════════════════════════════

        public const int PAGE_SIZE_DEFAULT = 12;

        public const int PAGE_SIZE_ADMIN = 20;

        // ════════════════════════════════════════════════════════════
        // KHO & TỒN KHO
        // ════════════════════════════════════════════════════════════

        public const int LOW_STOCK_THRESHOLD = 5;


        public const int STOCK_RESERVATION_MINUTES = 30;

        // ════════════════════════════════════════════════════════════
        // VẬN CHUYỂN & GIÁ
        // ════════════════════════════════════════════════════════════

        public const decimal FREE_SHIPPING_FROM = 500_000m;

        public const decimal SHIPPING_FEE_INNER_CITY = 25_000m;

        public const decimal SHIPPING_FEE_OUTER = 35_000m;

        // ════════════════════════════════════════════════════════════
        // ĐỊNH DẠNG NGÀY THÁNG
        // ════════════════════════════════════════════════════════════

        public const string FORMAT_DATE_TIME = "dd/MM/yyyy HH:mm:ss";

        public const string FORMAT_DATE = "dd/MM/yyyy";

        // ════════════════════════════════════════════════════════════
        // KHÓA CACHE
        // ════════════════════════════════════════════════════════════

        public const string CachePrefix = "BC_";



        public const string KeyProductsList = CachePrefix + "products:list:{0}";

        public const string KeyProductDetail = CachePrefix + "products:{0}";

        public const string KeyCategoriesList = CachePrefix + "categories:list";

        public const string KeyCollectionsActive = CachePrefix + "collections:active";

        public const string KeyPromotionsActive = CachePrefix + "promotions:active";
    }
}
