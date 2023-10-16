using HMS.Web.Data;
using HMS.Web.Data.Entities;
using HMS.Web.Helper;
using HMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;
using System.Text;
using static HMS.Web.Enums.Enum;

namespace HMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly HMSDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public AdminController(ILogger<AdminController> logger, HMSDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
			_signInManager = signInManager;
		}
        public IActionResult Index()
        {
            ViewBag.PatientsCount = _context.Patients.ToList().Count;
            ViewBag.AppCount = _context.Appointments.ToList().Count;
            ViewBag.DocCount = _context.Doctors.ToList().Count;
            return View();
        }

        #region Admin Profile
        [HttpGet]
        public IActionResult EditProfile()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EditProfile(int Id)
        {
            return View();
        }
        #endregion

        #region Schedules

        public IActionResult SchedulesList()
        {
            var schedule = _context.Schedules.Include(c => c.Doctors).ToList();

            List<ScheduleVM> Objlist = new List<ScheduleVM>();

            foreach (var m in schedule)
            {
                ScheduleVM Obj = new ScheduleVM();
                Obj.Id = m.Id;
                //Obj.Doctors.FullName = "";
                Obj.AvailableStartDay = m.AvailableStartDay;
                Obj.AvailableEndDay = m.AvailableEndDay;
                Obj.AvailableStartTime = m.AvailableStartTime;
                Obj.AvailableEndTime = m.AvailableEndTime;
                Obj.TimePerPatient = m.TimePerPatient;
                Obj.StrStatus = (m.Status ? "Active" : "Inactive");
                Objlist.Add(Obj);
            }
            return View(Objlist);
        }
        public IActionResult AddSchedule()
        {
            var DoctorList = new List<SelectListItem>();
            foreach (var m in DocList().Where(m => m.Status == true).ToList())
            {
                DoctorList.Add(new SelectListItem { Text = m.FullName, Value = Convert.ToString(m.Id) });
            }
            ViewBag.DoctorList = DoctorList;

            List<SelectListItem> AvailableDays = new()
            {
                new SelectListItem {Text = "Monday", Value = "Monday"},
                new SelectListItem {Text = "Tuesday", Value = "Tuesday"},
                new SelectListItem {Text = "Wednesday", Value = "Wednesday"},
                new SelectListItem {Text = "Thursday", Value = "Thursday"},
                new SelectListItem {Text = "Friday", Value = "Friday"},
                new SelectListItem {Text = "Saturday", Value = "Saturday"},
                new SelectListItem {Text = "Sunday", Value = "Sunday"}
            };
            ViewBag.AvailableDays = AvailableDays;

            List<SelectListItem> TimePerPatientList = new()
            {
                new SelectListItem {Text = "15 min", Value = "15 min"},
                new SelectListItem {Text = "30 min", Value = "30 min"},
                new SelectListItem {Text = "60 min", Value = "60 min"}
            };
            ViewBag.TimePerPatientList = TimePerPatientList;

            ScheduleVM Obj = new ScheduleVM();
            return View(Obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSchedule(ScheduleVM model)
        {
            Schedule Obj = new Schedule();
            
            Obj.DoctorId = model.DoctorId;

            Obj.AvailableStartDay = model.AvailableStartDay;
            Obj.AvailableEndDay = model.AvailableEndDay;

            Obj.AvailableStartTime = model.AvailableStartTime;
            Obj.AvailableEndTime = model.AvailableEndTime;

            Obj.TimePerPatient = model.TimePerPatient;
            Obj.Status = model.Status;

            _context.Schedules.Add(Obj);
            _context.SaveChanges();
            return RedirectToAction("SchedulesList");

            //return View(model);
        }

        public IActionResult EditSchedule(int id)
        {
            var DoctorList = new List<SelectListItem>();
            foreach (var m in DocList().Where(m => m.Status == true).ToList())
            {
                DoctorList.Add(new SelectListItem { Text = m.FullName, Value = Convert.ToString(m.Id) });
            }
            ViewBag.DoctorList = DoctorList;

            List<SelectListItem> AvailableDays = new()
            {
                new SelectListItem {Text = "Monday", Value = "Monday"},
                new SelectListItem {Text = "Tuesday", Value = "Tuesday"},
                new SelectListItem {Text = "Wednesday", Value = "Wednesday"},
                new SelectListItem {Text = "Thursday", Value = "Thursday"},
                new SelectListItem {Text = "Friday", Value = "Friday"},
                new SelectListItem {Text = "Saturday", Value = "Saturday"},
                new SelectListItem {Text = "Sunday", Value = "Sunday"}
            };
            ViewBag.AvailableDays = AvailableDays;

            List<SelectListItem> TimePerPatientList = new()
            {
                new SelectListItem {Text = "15 min", Value = "15 min"},
                new SelectListItem {Text = "30 min", Value = "30 min"},
                new SelectListItem {Text = "60 min", Value = "60 min"}
            };
            ViewBag.TimePerPatientList = TimePerPatientList;

            var model = _context.Schedules.Include(c => c.Doctors).SingleOrDefault(c => c.Id == id);

            ScheduleVM Obj = new ScheduleVM();
            Obj.Id = model.Id;

            Obj.DoctorId = model.DoctorId;

            Obj.AvailableStartDay = model.AvailableStartDay;
            Obj.AvailableEndDay = model.AvailableEndDay;

            Obj.AvailableStartTime = model.AvailableStartTime;
            Obj.AvailableEndTime = model.AvailableEndTime;

            Obj.TimePerPatient = model.TimePerPatient;
            Obj.Status = model.Status;

            return View(Obj);
        }
        [HttpPost]
        public IActionResult EditSchedule(ScheduleVM model)
        {
            var Obj = _context.Schedules.Include(c => c.Doctors).SingleOrDefault(c => c.Id == model.Id);

            Obj.DoctorId = model.DoctorId;

            Obj.AvailableStartDay = model.AvailableStartDay;
            Obj.AvailableEndDay = model.AvailableEndDay;

            Obj.AvailableStartTime = DateTime.Now;
            Obj.AvailableEndTime = DateTime.Now;

            Obj.TimePerPatient = model.TimePerPatient;
            Obj.Status = model.Status;

            _context.SaveChanges();
            return RedirectToAction("SchedulesList");
        }
        public IActionResult DeleteSchedule(int? id)
        {
            ScheduleVM Obj = new ScheduleVM();
            Obj.Id = Convert.ToInt32(id);
            return View(Obj);
        }
        [HttpPost, ActionName("DeleteSchedule")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSchedule(int id)
        {
            var schedule = _context.Schedules.Single(c => c.Id == id);
            _context.Schedules.Remove(schedule);
            _context.SaveChanges();
            return RedirectToAction("SchedulesList");
        }
        #endregion

        #region Start Doctor Section List,Add,Edit,Delete

        //List Of Doctors 
        private List<DoctorVM> DocList()
        {
            var model = _context.Doctors.ToList();

            List<DoctorVM> Objlist = new List<DoctorVM>();


            foreach (var m in model)
            {
                DoctorVM Obj = new DoctorVM();
                Obj.Id = m.Id;
                Obj.FullName = m.FirstName +" "+ m.LastName;
                Obj.Email = m.Email;
                Obj.Designation = m.Designation;
                Obj.Specialization = m.Specialization;
                Obj.Education = m.Education;
                Obj.ContactNo = m.ContactNo;
                Obj.BloodGroup = m.BloodGroup;
                Obj.Gender = m.Gender;
                Obj.Address = m.Address;
                Obj.Status = m.Status;
                Obj.StrStatus = (m.Status ? "Active" : "Inactive");
                Objlist.Add(Obj);
            }
            return Objlist;
        }
        [HttpGet]
        public IActionResult DoctorList()
        {
            return View(DocList());
        }
        public IActionResult AddDoctor()
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

            var collection = new DoctorCollectionVM
            {
                ApplicationUser = new RegisterViewModel(),
                Doctor = new DoctorVM()
            };
            return View(collection);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctor(DoctorCollectionVM model)
        {
            var user = new IdentityUser
            {
                UserName = model.ApplicationUser.Email,
                Email = model.ApplicationUser.Email
            };
            if (_context.Doctors.Any(c => c.Email == user.Email))
            {
                ModelState.AddModelError("Name", "User already present!");
                return View(model);
            }
            var result = await _userManager.CreateAsync(user, model.ApplicationUser.Password);
            if (result.Succeeded)
            {
                var docUser = _userManager.FindByNameAsync(user.Email).Result;
                var userRole = _userManager.AddToRolesAsync(docUser, new string[] { Enums.Enum.Roles.Doctor.ToString() }).Result;

                var doctor = new Doctor
                {
                    UserId = docUser.Id,
                    FirstName = model.Doctor.FirstName,
                    LastName = model.Doctor.LastName,
                    Email = model.ApplicationUser.Email,
                    ContactNo = model.Doctor.ContactNo,
                    Designation = model.Doctor.Designation,
                    Education = model.Doctor.Education,
                    Specialization = model.Doctor.Specialization,
                    Gender = model.Doctor.Gender,
                    BloodGroup = model.Doctor.BloodGroup,
                    DateOfBirth = model.Doctor.DateOfBirth,
                    Address = model.Doctor.Address,
                    Status = model.Doctor.Status
                };
                _context.Doctors.Add(doctor);
                _context.SaveChanges();

                StringBuilder sb = new StringBuilder();
                sb.Append("You have sucessfully register with us.");
                sb.Append("You user name is: " + model.ApplicationUser.Email);
                sb.Append("You password is: " + model.ApplicationUser.Password);
                EmailHelper.Sendmail(doctor.Email, "Thanks for Registration", sb.ToString());

                return RedirectToAction("DoctorList");
            }
            return RedirectToAction("DoctorList");
        }
        public IActionResult DoctorSetting(int id)
        {
            var model = _context.Doctors.SingleOrDefault(c => c.Id == id);
            DoctorVM obj = new DoctorVM();
            obj.Status = model.Status;

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoctorSetting(int id, DoctorVM model)
        {
            var doctor = _context.Doctors.Single(c => c.Id == id);
            doctor.Status = model.Status;

            _context.SaveChanges();
            return RedirectToAction("DoctorList");
        }
        public IActionResult DeleteDoctor(int? id)
        {
            DoctorVM Obj = new DoctorVM();
            Obj.Id = Convert.ToInt32(id);
            return View(Obj);
        }

        [HttpPost, ActionName("DeleteDoctor")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoctor(int id)
        {
            var Obj = _context.Doctors.SingleOrDefault(c => c.Id == id);
            _context.Doctors.Remove(Obj);
            _context.SaveChanges();
            return RedirectToAction("DoctorList");
        }

        #endregion

        #region Start Patient Section List,Edit,Delete
        private List<PatientVM> PatList()
        {
            var model = _context.Patients.ToList();
            List<PatientVM> Objlist = new List<PatientVM>();
            foreach (var m in model)
            {
                PatientVM Obj = new PatientVM();
                Obj.Id = m.Id;
                Obj.FullName = m.FirstName + m.LastName;
                Obj.Email = m.Email;
                Obj.ContactNo = m.ContactNo;
                Obj.BloodGroup = m.BloodGroup;
                Obj.Gender = m.Gender;
                Obj.Address = m.Address;
                Obj.StrStatus = (m.Status ? "Active" : "Inactive");

                Objlist.Add(Obj);
            }
            return Objlist;
        }
        public IActionResult PatientList()
        {
            return View(PatList());
        }

        public IActionResult EditPatient(int id)
        {
            PatientVM obj = new PatientVM();

            var model = _context.Patients.SingleOrDefault(c => c.Id == id);
            obj.FirstName = model.FirstName;
            obj.LastName = model.LastName;
            obj.Status = model.Status;

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPatient(int id, PatientVM model)
        {
            var patient = _context.Patients.Single(c => c.Id == id);
            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.Status = model.Status;

            _context.SaveChanges();
            return RedirectToAction("PatientList");
        }
        public IActionResult DeletePatient(int? id)
        {
            PatientVM Obj = new PatientVM();
            Obj.Id = Convert.ToInt32(id);
            return View(Obj);
        }

        [HttpPost, ActionName("DeletePatient")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePatient(int id)
        {
            var Obj = _context.Patients.SingleOrDefault(c => c.Id == id);
            _context.Patients.Remove(Obj);
            _context.SaveChanges();
            return RedirectToAction("PatientList");
        }
        #endregion

        #region Appointment

        private List<AppointmentVM> AllAppointmentList()
        {
            //var UserEmail = _userManager.GetUserName(User);
            //var doctor = _context.Doctors.Where(x => x.Email == UserEmail).FirstOrDefault();

            var ObjList = (from o in _context.Appointments
                           join p in _context.Patients on o.PatientId equals p.Id
                           join d in _context.Doctors on o.DoctorId equals d.Id
                           select new AppointmentVM
                           {
                               AppointmentDate = o.AppointmentDate,
                               PatientName = p.FirstName + " " + p.LastName,
                               DoctorName = d.FirstName + " " + d.LastName,
                               Id = o.Id,
                               PatientId = p.Id,
                               DoctorId = d.Id,
                               ReasonForSeeingDoc = o.Problem,
                               Status = o.Status,
                               StrStatus = o.Status == 0 ? Enums.Enum.AppointmentStatus.Inactive.ToString() : o.Status == 1 ? Enums.Enum.AppointmentStatus.Active.ToString() : Enums.Enum.AppointmentStatus.Pending.ToString(),

                           }).ToList();
            return ObjList;
        }
        public IActionResult ViewPatientDetails(int id)
        {
            var patient = _context.Patients.Single(c => c.Id == id);

            PatientVM Obj = new PatientVM();
            Obj.Id = patient.Id;
            Obj.FullName = patient.FirstName + " "+patient.LastName;
            Obj.Email = patient.Email;
            Obj.Address = patient.Address;
            Obj.ContactNo = patient.ContactNo;
            Obj.Gender = patient.Gender;
            Obj.BloodGroup = patient.BloodGroup;
            Obj.DateOfBirth = patient.DateOfBirth;

            return View(Obj);
        }
        public IActionResult AppointmentList()
        {
            return View(AllAppointmentList());
        }

        public IActionResult PendingAppointments()
        {
            return View(AllAppointmentList().Where(m => m.Status == 2));
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

		#region Password


		[HttpGet]
		public async Task<IActionResult> ChangePassword()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}			
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}
			var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
			if (!changePasswordResult.Succeeded)
			{
				foreach (var error in changePasswordResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return View();
			}
			
			await _signInManager.RefreshSignInAsync(user);
			_logger.LogInformation(LoggerEventIds.PasswordChanged, "User changed their password successfully.");
			return View();
		}

		#endregion

	}
}
