using Microsoft.AspNetCore.Identity;

namespace HMS.Web.Data.Entities
{
    public class Doctor
    {
        public int Id { get; set; }

        public IdentityUser ApplicationUser { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Designation { get; set; }
        public string? Address { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? ContactNo { get; set; }
        public string? Specialization { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Education { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
