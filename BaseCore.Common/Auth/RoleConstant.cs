

namespace BaseCore.Common
{
    /// <summary>
    /// Role names dùng trong [Authorize(Roles = ...)].
    /// Phải khớp với UserRole enum trong BaseCore.Entities/Enums/UserRole.cs.
    /// </summary>

    // ════════════════════════════════════════════════════════════
    // HẰNG SỐ ROLE (PHÂN QUYỀN)
    // ════════════════════════════════════════════════════════════

    public static class RoleConstant
    {

        // ════════════════════════════════════════════════════════════
        // TÊN ROLE
        // ════════════════════════════════════════════════════════════

        public const string Customer = "Customer";

        public const string WarehouseStaff = "WarehouseStaff";

        public const string Marketing = "Marketing";

        public const string Admin = "Admin";

        // ════════════════════════════════════════════════════════════
        // LOẠI CLAIM
        // ════════════════════════════════════════════════════════════

        public const string ClaimTypeRole = "role";

        // ════════════════════════════════════════════════════════════
        // NHÓM ROLE TỔNG HỢP
        // ════════════════════════════════════════════════════════════

        public const string AdminOrMarketing = Admin + "," + Marketing;

        public const string AdminOrWarehouse = Admin + "," + WarehouseStaff;

        
        public const string Staff = Admin + "," + WarehouseStaff + "," + Marketing;
    }
}
