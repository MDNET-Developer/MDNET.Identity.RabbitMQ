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
                _logger.LogInformation($"Sign up successfully {DateTime.UtcNow}");
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
        public async Task<IActionResult> LogIn(LoginViewModel loginViewModel, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Login faild {DateTime.UtcNow}");
                return View(loginViewModel);
            }

            
                returnUrl = returnUrl?? Url.Action(nameof(HomeController.Index), "Home");
            var hasUser = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (await _userManager.IsLockedOutAsync(hasUser))
            {
                ModelState.AddModelError(string.Empty, "Hesabınız bloklanıb. Zəhmət olmasa bir müddət sonra yenidən cəhd edin.");
                return View(loginViewModel);
            }
            if (hasUser != null)
            {
                var result = await _signInManager.PasswordSignInAsync(hasUser, loginViewModel.Password, loginViewModel.RememberMe, true);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Login succesfully {DateTime.UtcNow}");
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Çox sayda uğursuz cəhd səbəbilə hesabınız bloklanıb.");
                }
                else
                {
                    // Maksimum cəhd sayı
                    int maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;

                    // Hazırda neçə dəfə uğursuz cəhd olub
                    int failedAttempts = await _userManager.GetAccessFailedCountAsync(hasUser);

                    int remainingAttempts = maxAttempts - failedAttempts;

                    if (remainingAttempts == 1)
                    {
                        ModelState.AddModelError(string.Empty, $"Qalan cəhd sayı: {remainingAttempts}. Zəhmət olmasa diqqətli olun, əks halda hesabınız müvəqqəti bloklanacaq, bu sizin son cəhdinizdir");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Qalan cəhd sayı: {remainingAttempts}.");
                    }

                }
            }
            ModelState.AddModelError(string.Empty, "Email və ya şifrə yanlışdır");
            return View(loginViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        // ValidateAntiForgeryToken helps protect against CSRF attacks, especially important for logout.
    }
}
