using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace HMS.Web.Data.Entities
{
    public class Schedule
    {
        public int Id { get; set; }
        public Doctor Doctors { get; set; }
        public int DoctorId { get; set; }
        public string AvailableStartDay { get; set; }
        public string AvailableEndDay { get; set; }
        public DateTime AvailableStartTime { get; set; }
        public DateTime AvailableEndTime { get; set; }
        public string TimePerPatient { get; set; }
        public bool Status { get; set; }
    }
}
