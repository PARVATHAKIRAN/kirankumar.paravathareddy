using HMS.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models
{
    public class DoctorVM
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Designation { get; set; }
        
        public string Address { get; set; }

        [Required]
        [Display(Name = "Mobile No")]
        public string ContactNo { get; set; }

        [Required]
        public string Specialization { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Education/Degree")]
        public string Education { get; set; }
        public bool Status { get; set; }
        public string StrStatus { get; set; }
		public string ExistingImage { get; set; }
        public IFormFile? ProfilePhoto { get; set; }        
    }
}
