using StationCheck.Models;

namespace StationCheck.Helpers
{
    public static class AuditHelper
    {
        public static void SetCreated(BaseAuditEntity entity, string? createdBy)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = createdBy;
        }

        public static void SetModified(BaseAuditEntity entity, string? modifiedBy)
        {
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = modifiedBy;
        }

        public static void SetDeleted(BaseAuditEntity entity, string? deletedBy)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = deletedBy;
        }
    }
}
