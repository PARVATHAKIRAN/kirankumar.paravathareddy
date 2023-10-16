using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace HMS.Web.Data.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public string PatientEmail { get; set; }
		public string DoctorEmail { get; set; }
		public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Problem { get; set; }
        public int Status { get; set; }

        ///Medical history
        ///
        
        public string Drugallergies { get; set; }
        public string Otherillnesses { get; set; }        
        public string AnyOperations { get; set; }
        public string CurrentMedications { get; set; }
        public string Exercise { get; set; }
		public string Diet { get; set; }
		public string AConsumption { get; set; }
		public string CConsumption { get; set; }
		public string MedicalHistoryComment { get; set; }

        //
        public string? DoctorsComment { get; set; }
    }
}
