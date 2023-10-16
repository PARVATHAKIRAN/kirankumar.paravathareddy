using HMS.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models
{
    public class ScheduleVM
    {
        public int Id { get; set; }
        public DoctorVM Doctors { get; set; }
        public int DoctorId { get; set; }
        public string AvailableStartDay { get; set; }
        public string AvailableEndDay { get; set; }
		[Required]
		[DataType(DataType.Time)]
		[Display(Name = "Start Time")]
		public DateTime AvailableStartTime { get; set; }
		[Required]
		[DataType(DataType.Time)]
		[Display(Name = "End Time")]
		public DateTime AvailableEndTime { get; set; }
        public string TimePerPatient { get; set; }
        public bool Status { get; set; }
        public string? StrStatus { get; set; }
        
    }
}
