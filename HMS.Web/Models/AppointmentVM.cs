using HMS.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models
{
    public class AppointmentVM
    {
        public int Id { get; set; }

        public Patient Patient { get; set; }
        [Display(Name = "Patient Name")]
        public int PatientId { get; set; }

        public string PatientEmail { get; set; }
        public string PatientName { get; set; }
        public Doctor Doctor { get; set; }
        [Display(Name = "Doctor Name")]
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorEmail { get; set; }

		[Display(Name = "Appointment Date")]       
        
        public DateTime? AppointmentDate { get; set; }
        [Display(Name = "Reason for seeing the doctor")]
        public string ReasonForSeeingDoc { get; set; }

        public int Status { get; set; }
        public string StrStatus { get; set; }


        ///Medical history
        ///

        [Display(Name = "Please list any drug allergies")]
        public string Drugallergies { get; set; }

        [Display(Name = "Other illnesses")]
        public string Otherillnesses { get; set; }

        [Display(Name = "Please list any Operations")]
        public string AnyOperations { get; set; }

        [Display(Name = "Please list your Current Medications")]
        public string CurrentMedications { get; set; }

        [Display(Name = "Any Exercise")]
        public string Exercise { get; set; }

		[Display(Name = "Any Diet")]
		public string Diet { get; set; }

		[Display(Name = "Alcohol Consumption")]
        public string AConsumption { get; set; }
		[Display(Name = "Caffeine Consumption")]
		public string CConsumption { get; set; }

		[Display(Name = "Include other comments regarding your Medical History")]
        public string MedicalHistoryComment { get; set; }

        public string DoctorsComment { get; set; }

    }
}
