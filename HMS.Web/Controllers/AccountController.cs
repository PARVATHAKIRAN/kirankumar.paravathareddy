using HMS.Web.Data;
using HMS.Web.Data.Entities;
using HMS.Web.Helper;
using HMS.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace HMS.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly IHttpContextAccessor _httpAccessor;
		private readonly IEmailSender _emailSender;
		private readonly HMSDbContext _context;
		public const string SessionProfilePhoto = "_ProfilePhoto";
		public const string SessionName = "_Name";
		public AccountController(ILogger<HomeController> logger, UserManager<IdentityUser> userMngr,
			SignInManager<IdentityUser> signInMngr, IHttpContextAccessor httpAccsr, IEmailSender emailSender, HMSDbContext context)
		{
			_logger = logger;
			_userManager = userMngr;
			_signInManager = signInMngr;
			_httpAccessor = httpAccsr;
			_emailSender = emailSender;
			_context = context;
		}
		[HttpGet]

		public IActionResult LogIn()
		{
			var model = new LoginViewModel();

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> LogIn(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByNameAsync(model.Email);

				var result = await _signInManager.PasswordSignInAsync(
					model.Email, model.Password, isPersistent: model.RememberMe,
					lockoutOnFailure: false);

				if (result.Succeeded)
				{
                    var roles = await _userManager.GetRolesAsync(user);

					if (roles != null && roles[0] == Enums.Enum.Roles.Admin.ToString())
					{
						return RedirectToAction("Index", "Admin");
					}

					else if (roles != null && roles[0] == Enums.Enum.Roles.Doctor.ToString())
					{
						var doctor = _context.Doctors.SingleOrDefault(c => c.Email == model.Email);
						//var Profile_Photo = _context.Doctors.SingleOrDefault(c => c.Email == model.Email).ProfilePhoto;
						if (doctor != null)
						{

							_httpAccessor.HttpContext.Session.SetString(SessionProfilePhoto, (doctor.ProfilePhoto != null ? doctor.ProfilePhoto : "Administrator.png"));
							_httpAccessor.HttpContext.Session.SetString(SessionName, doctor.FirstName + " " + doctor.LastName);
						}
						return RedirectToAction("Index", "Doctor");
					}
					else if (roles != null && roles[0] == Enums.Enum.Roles.Patient.ToString())
					{
						var patient = _context.Patients.SingleOrDefault(c => c.Email == model.Email);
						if (patient != null)
						{

							_httpAccessor.HttpContext.Session.SetString(SessionProfilePhoto, (patient.ProfilePhoto != null ? patient.ProfilePhoto : "Administrator.png"));
							_httpAccessor.HttpContext.Session.SetString(SessionName, patient.FirstName + " " + patient.LastName);
						}

						return RedirectToAction("Index", "Patient");
					}
				}
			}
			ModelState.AddModelError("", "Invalid username/password.");
			return View(model);
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			var user = new IdentityUser
			{
				UserName = model.Email,
				Email = model.Email
			};
			if (ModelState.IsValid)
			{
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					var patientUser = _userManager.FindByEmailAsync(user.Email).Result;
					var userRole = _userManager.AddToRolesAsync(patientUser, new string[] { Enums.Enum.Roles.Patient.ToString() }).Result;

					var patient = new Patient
					{
						UserId = patientUser.Id,
						FirstName = model.FirstName,
						LastName = model.LastName,
						Email = model.Email
					};
					_context.Patients.Add(patient);
					_context.SaveChanges();

					StringBuilder sb = new StringBuilder();
					sb.Append("You have sucessfully register with us.");
                    sb.Append("You user name is: " + model.Email);
                    sb.Append("You password is: " + model.Password);
                    EmailHelper.Sendmail(patient.Email, "Thanks for Registration", sb.ToString());

					return RedirectToAction("Index", "Home");
				}
			}
			return View(model);
		}

		public async Task<IActionResult> Logout()
		{
			if (User.Identity?.IsAuthenticated ?? false)
			{
				await _signInManager.SignOutAsync();
				return RedirectToAction("Logout", new RouteValueDictionary(
				new { controller = "Account", action = "Logout", returnUrl = "/Account/Logout" }));
			}
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Logout(string returnUrl = null)
		{
			await _signInManager.SignOutAsync();
			_logger.LogInformation("User logged out.");
			if (returnUrl != null)
			{
				return LocalRedirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}
		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}

		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					return View();
				}
				else {
					var code = await _userManager.GeneratePasswordResetTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ResetPassword",
						pageHandler: null,
						values: new { area = "", code },
						protocol: Request.Scheme)!;

					EmailHelper.Sendmail(
					user.Email,
					"Reset Password For" + user.UserName,
					$"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>." +
					$"You will be redirected to another page to Create your new password!");

					return RedirectToPage("ForgotPasswordConfirmation");
				}				
			}
			return View(model);
		}

		[HttpGet]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}
		
		[HttpGet]
		public IActionResult ResetPassword(string? code = null)
		{
			if (code == null)
			{
				return BadRequest("A code must be supplied for password reset.");
			}
			else
			{
				ResetPasswordVM obj = new ResetPasswordVM();
				obj.TokenCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
				
				return View(obj);
			}
			
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				return View(model);
			}

			var result = await _userManager.ResetPasswordAsync(user, model.TokenCode, model.Password);
			if (result.Succeeded)
			{
				return View(model);
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(model);
		}
	}
}
