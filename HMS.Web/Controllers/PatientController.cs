using HMS.Web.Data;
using HMS.Web.Data.Entities;
using HMS.Web.Helper;
using HMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using static HMS.Web.Enums.Enum;

namespace HMS.Web.Controllers
{
	[Authorize(Roles = "Patient")]
	public class PatientController : Controller
	{
		private readonly ILogger<PatientController> _logger;
		private readonly HMSDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IWebHostEnvironment _hostEnvironment;
		const string SessionName = "_ProfilePhoto";
		public PatientController(ILogger<PatientController> logger, HMSDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
		{
			_logger = logger;
			_context = context;
			_userManager = userManager;
			_hostEnvironment = hostEnvironment;
		}
		public IActionResult Index()
		{
			ViewBag.PatientsCount = _context.Patients.ToList().Count;
			ViewBag.AppCount = _context.Appointments.ToList().Count;
			ViewBag.DocCount = _context.Doctors.ToList().Count;
			return View();
		}

		#region Patient Profile
		[HttpGet]
		public IActionResult EditProfile()
		{
			List<SelectListItem> bloodlist = new()
			{
				new SelectListItem {Text = "A+", Value = "A+"},
				new SelectListItem {Text = "O+", Value = "O+"},
				new SelectListItem {Text = "B+", Value = "B+"},
				new SelectListItem {Text = "AB+",Value = "AB+"},
				new SelectListItem {Text = "A-",Value = "A-"},
				new SelectListItem {Text = "O-",Value = "O-"},
				new SelectListItem {Text = "B-",Value = "B-"},
				new SelectListItem {Text = "AB-",Value = "AB-"}
			};
			ViewBag.bloodlist = bloodlist;
			List<SelectListItem> genderlist = new()
			{
				new SelectListItem { Value = "Male", Text = "Male" },
				new SelectListItem { Value = "Female", Text = "Female" }
			};
			ViewBag.genderlist = genderlist;

			PatientVM obj = new PatientVM();
			var UserEmail = _userManager.GetUserName(User);
			var model = _context.Patients.SingleOrDefault(c => c.Email == UserEmail);
			obj.Id = model.Id;
			obj.FirstName = model.FirstName;
			obj.LastName = model.LastName;
			obj.ContactNo = model.ContactNo;
			obj.BloodGroup = model.BloodGroup;
			obj.Gender = model.Gender;
			obj.DateOfBirth = model.DateOfBirth;
			obj.Address = model.Address;
			obj.Height = model.Height;
			obj.Weight = model.Weight;
			obj.ExistingImage = model.ProfilePhoto;

			return View(obj);
		}
		[HttpPost]
		public IActionResult EditProfile(PatientVM model)
		{
			if (_context.Patients.Any(c => c.Email == model.Email))
			{
				ModelState.AddModelError("Name", "User already present!");
				return View(model);
			}
			var UserEmail = _userManager.GetUserName(User);
			var patient = _context.Patients.SingleOrDefault(c => c.Email == UserEmail);

			if (model.ProfilePhoto != null)
			{
				string Folderpath = Path.Combine(_hostEnvironment.WebRootPath, "PatientImages");
				if (!Directory.Exists(Folderpath))
				{
					Directory.CreateDirectory(Folderpath);
				}

				string wwwRootPath = _hostEnvironment.WebRootPath;
				string fileName = Path.GetFileNameWithoutExtension(model.ProfilePhoto.FileName);
				string extension = Path.GetExtension(model.ProfilePhoto.FileName);
				String NewfileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
				string path = Path.Combine(wwwRootPath + "/PatientImages/", NewfileName);
				using (var fileStream = new FileStream(path, FileMode.Create))
				{
					if (fileStream != null)
						model.ProfilePhoto.CopyTo(fileStream);
				}
				patient.ProfilePhoto = NewfileName;

			}

			patient.FirstName = model.FirstName;
			patient.LastName = model.LastName;
			patient.ContactNo = model.ContactNo;
			patient.BloodGroup = model.BloodGroup;
			patient.Gender = model.Gender;
			patient.DateOfBirth = model.DateOfBirth;
			patient.Address = model.Address;
			patient.Height = model.Height;
			patient.Weight = model.Weight;

			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		#endregion

		#region Available Doctors
		public IActionResult AvailableDoctorsList()
		{

			var doctor = _context.Doctors.Where(c => c.Status == true).ToList();

			List<DoctorVM> Objlist = new List<DoctorVM>();

			foreach (var m in doctor)
			{
				DoctorVM Obj = new DoctorVM();
				Obj.Id = m.Id;
				Obj.FullName = m.FirstName + m.LastName;
				Obj.Email = m.Email;
				Obj.Designation = m.Designation;
				Obj.ContactNo = m.ContactNo;
				Obj.Gender = m.Gender;
				Obj.Education = m.Education;
				Obj.StrStatus = (m.Status ? "Active" : "Inactive");
				Objlist.Add(Obj);
			}
			return View(Objlist);
		}

		public IActionResult DoctorSchedule(int id)
		{
            ScheduleVM Obj = new ScheduleVM();

            var ObjCount = (from s in _context.Schedules
						   join d in _context.Doctors on s.DoctorId equals d.Id
							select new ScheduleVM
                            { 
								Id = s.Id,

							}).Count();
            
            if (ObjCount > 0) 
			{
                var schedule = _context.Schedules.Include(c => c.Doctors).Single(c => c.DoctorId == id);
                string DoctorsFullName = "";
                var doctor = _context.Doctors.SingleOrDefault(c => c.Id == schedule.DoctorId);

                if (doctor != null)
                    DoctorsFullName = doctor.FirstName + " " + doctor.LastName;

                ViewBag.FullName = DoctorsFullName;

                
                Obj.Id = schedule.Id;
                Obj.DoctorId = schedule.DoctorId;
                Obj.AvailableStartDay = schedule.AvailableStartDay;
                Obj.AvailableEndDay = schedule.AvailableEndDay;
                Obj.AvailableStartTime = schedule.AvailableStartTime;
                Obj.AvailableEndTime = schedule.AvailableEndTime;
                Obj.TimePerPatient = schedule.TimePerPatient;
                Obj.StrStatus = (schedule.Status ? "Active" : "Inactive");
            }

            return View(Obj);
		}

		public IActionResult DoctorDetail(int id)
		{
			var doctor = _context.Doctors.Single(c => c.Id == id);

			DoctorVM Obj = new DoctorVM();
			Obj.Id = doctor.Id;
			Obj.FullName = doctor.FirstName + " " + doctor.LastName;
			Obj.Email = doctor.Email;
			Obj.Designation = doctor.Designation;
			Obj.Address = doctor.Address;
			Obj.Specialization = doctor.Specialization;
			Obj.ContactNo = doctor.ContactNo;
			Obj.Gender = doctor.Gender;
			Obj.BloodGroup = doctor.BloodGroup;
			Obj.DateOfBirth = doctor.DateOfBirth;
			Obj.Education = doctor.Education;

			return View(Obj);
		}
		#endregion

		#region Schedule Appointment

		private List<DoctorVM> AvailableDoctors()
		{
			var model = _context.Doctors.ToList();

			List<DoctorVM> Objlist = new List<DoctorVM>();

			foreach (var m in model)
			{
				DoctorVM Obj = new DoctorVM();
				Obj.Id = m.Id;
				Obj.FullName = m.FirstName + m.LastName;
				Obj.Email = m.Email;
				Obj.Designation = m.Designation;
				Obj.Specialization = m.Specialization;
				Obj.Education = m.Education;
				Obj.ContactNo = m.ContactNo;
				Obj.BloodGroup = m.BloodGroup;
				Obj.Gender = m.Gender;
				Obj.Address = m.Address;
				Objlist.Add(Obj);
			}
			return Objlist;
		}

		[HttpGet]
		public IActionResult ScheduleAppointment()
		{
			var DocList = new List<SelectListItem>();
			foreach (var m in AvailableDoctors())
			{
				DocList.Add(new SelectListItem { Text = m.FullName, Value = Convert.ToString(m.Id) });
			}
			ViewBag.DoctorList = DocList;
			AppointmentVM Obj = new AppointmentVM();
			return View(Obj);
		}
		[HttpPost]
		public IActionResult ScheduleAppointment(AppointmentVM model)
		{
			if (model.AppointmentDate >= DateTime.Now.Date)
			{
				var UserEmail = _userManager.GetUserName(User);
				string fullname = _context.Patients.Where(x => x.Email == UserEmail).FirstOrDefault().FirstName + " " + _context.Patients.Where(x => x.Email == UserEmail).FirstOrDefault().LastName;

				Appointment appointment = new Appointment();
				appointment.PatientId = _context.Patients.Where(x => x.Email == UserEmail).FirstOrDefault().Id;
				appointment.PatientEmail = UserEmail;
				appointment.DoctorId = model.DoctorId;
				appointment.DoctorEmail = _context.Doctors.Where(x => x.Id == model.DoctorId).FirstOrDefault().Email;
				appointment.AppointmentDate = model.AppointmentDate;
				appointment.Problem = model.ReasonForSeeingDoc;

				appointment.Drugallergies = model.Drugallergies;
				appointment.Otherillnesses = model.Otherillnesses;
				appointment.AnyOperations = model.AnyOperations;
				appointment.CurrentMedications = model.CurrentMedications;
				appointment.Exercise = model.Exercise;
				appointment.Diet = model.Diet;
				appointment.AConsumption = model.AConsumption;
				appointment.CConsumption = model.CConsumption;
				appointment.MedicalHistoryComment = model.MedicalHistoryComment;

				appointment.Status = (int)Enums.Enum.AppointmentStatus.Pending;

				_context.Appointments.Add(appointment);
				_context.SaveChanges();

				StringBuilder sbdoctor = new StringBuilder();
				sbdoctor.Append("Patient Details as below");
				sbdoctor.Append("Patient Name : " + fullname);
				sbdoctor.Append("Patient Problem : " + appointment.Problem);
				EmailHelper.Sendmail(appointment.DoctorEmail, "Appointment booked", sbdoctor.ToString());

				StringBuilder sbpatient = new StringBuilder();
				sbpatient.Append("You have sucessfully bookes appointment with us.");
				EmailHelper.Sendmail(appointment.PatientEmail, "Appointment booked", sbpatient.ToString());

				return RedirectToAction("AppointmentList");
			}
			ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

			return RedirectToAction("ScheduleAppointment");

		}

        public IActionResult AppointmentList()
        {
            var UserEmail = _userManager.GetUserName(User);

            var appointment = _context.Appointments.Where(m => m.PatientEmail == UserEmail).ToList();
            List<AppointmentVM> ObjList = new List<AppointmentVM>();

            foreach (var m in appointment)
            {
                AppointmentVM Obj = new AppointmentVM();
                Obj.Id = m.Id;
                Obj.AppointmentDate = m.AppointmentDate;
                Obj.ReasonForSeeingDoc = m.Problem;
                Obj.StrStatus = m.Status == 0 ? Enums.Enum.AppointmentStatus.Inactive.ToString() : m.Status == 1 ? Enums.Enum.AppointmentStatus.Active.ToString() : Enums.Enum.AppointmentStatus.Pending.ToString();

                ObjList.Add(Obj);
            }

            return View(ObjList);
        }

        [HttpGet]
		public IActionResult EditAppointment(int id)
		{
			var DocList = new List<SelectListItem>();
			foreach (var m in AvailableDoctors())
			{
				DocList.Add(new SelectListItem { Text = m.FullName, Value = Convert.ToString(m.Id) });
			}
			ViewBag.DoctorList = DocList;

			AppointmentVM Obj = new AppointmentVM();
			var model = _context.Appointments.SingleOrDefault(c => c.Id == id);
			Obj.Id = model.Id;
			Obj.AppointmentDate = model.AppointmentDate;
			Obj.ReasonForSeeingDoc = model.Problem;

			Obj.Drugallergies = model.Drugallergies;
			Obj.Otherillnesses = model.Otherillnesses;
			Obj.AnyOperations = model.AnyOperations;
			Obj.CurrentMedications = model.CurrentMedications;
			Obj.Exercise = model.Exercise;
			Obj.Diet = model.Diet;
			Obj.AConsumption = model.AConsumption;
			Obj.CConsumption = model.CConsumption;
			Obj.MedicalHistoryComment = model.MedicalHistoryComment;

			return View(Obj);
		}
		[HttpPost]
		public IActionResult EditAppointment(AppointmentVM model)
		{
			if (model.AppointmentDate >= DateTime.Now.Date)
			{
				var UserEmail = _userManager.GetUserName(User);
				var appointment = _context.Appointments.SingleOrDefault(c => c.Id == model.Id);

				appointment.PatientId = _context.Patients.Where(x => x.Email == UserEmail).FirstOrDefault().Id;
				appointment.PatientEmail = UserEmail;
				appointment.DoctorId = model.DoctorId;
				appointment.DoctorEmail = _context.Doctors.Where(x => x.Id == model.DoctorId).FirstOrDefault().Email;
				appointment.AppointmentDate = model.AppointmentDate;
				appointment.Problem = model.ReasonForSeeingDoc;

				appointment.Drugallergies = model.Drugallergies;
				appointment.Otherillnesses = model.Otherillnesses;
				appointment.AnyOperations = model.AnyOperations;
				appointment.CurrentMedications = model.CurrentMedications;
				appointment.Exercise = model.Exercise;
				appointment.Diet = model.Diet;
				appointment.AConsumption = model.AConsumption;
				appointment.CConsumption = model.CConsumption;
				appointment.MedicalHistoryComment = model.MedicalHistoryComment;

				appointment.Status = (int)Enums.Enum.AppointmentStatus.Pending;

				_context.SaveChanges();
				return RedirectToAction("AppointmentList");
			}
			ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

			return RedirectToAction("ScheduleAppointment");

		}
		
		public IActionResult DeleteAppointment(int? id)
		{
			AppointmentVM Obj = new AppointmentVM();
			Obj.Id = Convert.ToInt32(id);
			return View(Obj);
		}

		[HttpPost, ActionName("DeleteAppointment")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteAppointment(int id)
		{
			var appointment = _context.Appointments.Single(c => c.Id == id);
			_context.Appointments.Remove(appointment);
			_context.SaveChanges();
			return RedirectToAction("AppointmentList");
		}
		#endregion

		#region Patient Feedback

		public IActionResult PatientFeedback()
		{
			PatientFeedbackVM Obj = new PatientFeedbackVM();
			return View(Obj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult PatientFeedback(PatientFeedbackVM model)
		{
			Feedback Obj = new Feedback();
			Obj.PatientName = model.PatientName;
			Obj.PatientFeedback = model.PatientFeedback;
			_context.Feedbacks.Add(Obj);
			_context.SaveChanges();

			StringBuilder sbfb = new StringBuilder();
			sbfb.Append("Patient Name: " + Obj.PatientName);
			sbfb.Append("Feedback from patient: " + Obj.PatientFeedback);
			EmailHelper.Sendmail("AdminEmail", "Feedback from patient", sbfb.ToString());

			return RedirectToAction("Index");
		}

		#endregion

	}
}
