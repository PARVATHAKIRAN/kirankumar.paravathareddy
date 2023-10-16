using Microsoft.AspNetCore.Identity;

namespace HMS.Web.Data.Entities
{
    public class UserToken : IdentityUserToken<Guid>
    {
        public virtual User User { get; set; } = null;
    }
}
