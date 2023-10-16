using HMS.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models
{
    public class PatientVM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string BloodGroup { get; set; }
        public string Gender { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public bool Status { get; set; }
        public string StrStatus { get; set; }
        public bool IsDeleted { get; set; }
        public string ExistingImage { get; set; }

		public IFormFile? ProfilePhoto { get; set; }
    }
}
