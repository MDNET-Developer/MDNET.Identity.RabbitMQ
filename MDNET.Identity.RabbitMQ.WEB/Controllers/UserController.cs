using MDNET.Identity.RabbitMQ.Web.Models;
using MDNET.Identity.RabbitMQ.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MDNET.Identity.RabbitMQ.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly SignInManager<AppUser> _signInManager;
        public UserController(UserManager<AppUser> userManager, ILogger<UserController> logger, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
                _logger.LogError($"Model state is not valide {DateTime.UtcNow}");
            }
            var user = new AppUser()
            {
                UserName = signUpViewModel.UserName,
                Email = signUpViewModel.Email,
                PhoneNumber = signUpViewModel.PhoneNumber,
            };
            var identityResult = await _userManager.CreateAsync(user, password: signUpViewModel.PasswordConfirm);
            if (identityResult.Succeeded)
            {
                TempData["ResultMessage"] = "Qeydiyyat uğurla başa çatdı !";
                return RedirectToAction(nameof(UserController.SignUp));
                _logger.LogError($"Sign up successfully {DateTime.UtcNow}");
            }
            else
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View();
            }
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(LoginViewModel loginViewModel, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Action(nameof(HomeController), nameof(HomeController.Index));
            var isUser = _userManager.FindByEmailAsync(loginViewModel.Email);
            if (isUser != null) {
                var result = _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, false);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email və ya şifrə yanlışdır");
            }
            
            return View();
        }
    }
}
