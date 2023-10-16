
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Web.Models
{
    public class DoctorCollectionVM
    {
        public RegisterViewModel ApplicationUser { get; set; }
        public DoctorVM Doctor { get; set; }
    }
}
