using HMS.Web.Data;
using HMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Web.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
		private readonly ILogger<DoctorController> _logger;
		private readonly HMSDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IWebHostEnvironment _hostEnvironment;
		public DoctorController(ILogger<DoctorController> logger, HMSDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
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

        #region Doctor profile

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

			var UserEmail = _userManager.GetUserName(User);
			var model = _context.Doctors.SingleOrDefault(c => c.Email == UserEmail);

			DoctorVM obj = new DoctorVM();
			obj.Id= model.Id;
			obj.FirstName = model.FirstName;
			obj.LastName = model.LastName;
			obj.ContactNo = model.ContactNo;

			obj.Designation = model.Designation;
			obj.Education = model.Education;
			obj.Specialization = model.Specialization;

			obj.BloodGroup = model.BloodGroup;
			obj.Gender = model.Gender;
			obj.DateOfBirth = model.DateOfBirth;
			obj.Address = model.Address;
			obj.ExistingImage = model.ProfilePhoto;

			return View(obj);
		}
        [HttpPost]
        public IActionResult EditProfile(DoctorVM model)
        {
			var UserEmail = _userManager.GetUserName(User);
			var doctor = _context.Doctors.SingleOrDefault(c => c.Email == UserEmail);

			if (model.ProfilePhoto != null) 
			{
				string Folderpath = Path.Combine(_hostEnvironment.WebRootPath, "DoctorImages");
				if (!Directory.Exists(Folderpath))
				{
					Directory.CreateDirectory(Folderpath);
				}

				string wwwRootPath = _hostEnvironment.WebRootPath;
				string fileName = Path.GetFileNameWithoutExtension(model.ProfilePhoto.FileName);
				string extension = Path.GetExtension(model.ProfilePhoto.FileName);
				String NewfileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
				string path = Path.Combine(wwwRootPath + "/DoctorImages/", NewfileName);
				using (var fileStream = new FileStream(path, FileMode.Create))
				{
					if (fileStream != null)
						model.ProfilePhoto.CopyTo(fileStream);
				}
				doctor.ProfilePhoto = NewfileName;
			}

			doctor.FirstName = model.FirstName;
			doctor.LastName = model.LastName;

			doctor.Designation = model.Designation;
			doctor.Education = model.Education;
			doctor.Specialization = model.Specialization;

			doctor.ContactNo = model.ContactNo;
			doctor.BloodGroup = model.BloodGroup;
			doctor.Gender = model.Gender;
			doctor.DateOfBirth = model.DateOfBirth;
			doctor.Address = model.Address;

			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		#endregion

		#region Patient

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

		#endregion

		#region Appointment List

		private List<AppointmentVM> AllAppointmentList() {

            var UserEmail = _userManager.GetUserName(User);
            var doctor = _context.Doctors.Where(x => x.Email == UserEmail).FirstOrDefault();

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
			if (doctor != null) { return ObjList.Where(m => m.DoctorId == doctor.Id).ToList(); }
			return ObjList;
        }
		public IActionResult AppointmentList()
		{
			//var UserEmail = _userManager.GetUserName(User);
			//var doctor = _context.Doctors.Where(x => x.Email == UserEmail).FirstOrDefault();

			return View(AllAppointmentList().Where(m => m.Status == 1));
		}

		public IActionResult PendingAppointments()
		{
			//var UserEmail = _userManager.GetUserName(User);
            return View(AllAppointmentList().Where(m => m.Status == 2));
        }
		public IActionResult ViewPatientDetails(int id) 
		{
            var patient = _context.Patients.Single(c => c.Id == id);

            PatientVM Obj = new PatientVM();
            Obj.Id = patient.Id;
            Obj.FullName = patient.FirstName +" " +patient.LastName;
            Obj.Email = patient.Email;
            Obj.Address = patient.Address;
            Obj.ContactNo = patient.ContactNo;
            Obj.Gender = patient.Gender;
            Obj.BloodGroup = patient.BloodGroup;
            Obj.DateOfBirth = patient.DateOfBirth;

            return View(Obj);
        }
        public IActionResult AppointmentStatus(int id)
        {
			List<SelectListItem> appointmentstatus = new()
			{
				new SelectListItem { Value = "0", Text = "Inactive" },
				new SelectListItem { Value = "1", Text = "Active" },
				new SelectListItem { Value = "2", Text = "Pending" }
			};
			ViewBag.appointmentstatus = appointmentstatus;

			var model = _context.Appointments.SingleOrDefault(c => c.Id == id);
			AppointmentVM obj = new AppointmentVM();
			obj.Status = model.Status;

			return View(obj);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AppointmentStatus(int id, AppointmentVM model)
		{
			var Appo = _context.Appointments.Single(c => c.Id == id);
			Appo.Status = model.Status;

			_context.SaveChanges();
			return RedirectToAction("AppointmentList");
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


    }
}
