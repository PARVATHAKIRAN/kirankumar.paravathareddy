using Microsoft.AspNetCore.Identity;

namespace HMS.Web.Data.Entities
{
    public class UserClaim : IdentityUserClaim<Guid>
    {
        public Guid ActionId { get; set; }
        public Guid PageId { get; set; }
        public virtual User User { get; set; }
    }
}
