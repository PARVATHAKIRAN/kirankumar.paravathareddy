using Microsoft.AspNetCore.Identity;

namespace HMS.Web.Data.Entities
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public Guid ActionId { get; set; }
        public Guid PageId { get; set; }
        public virtual Role Role { get; set; }
    }
}
