using Microsoft.AspNetCore.Identity;

namespace HMS.Web.Data.Entities
{
    public class UserLogin : IdentityUserLogin<Guid>
    {
        public virtual User User { get; set; }
    }
}