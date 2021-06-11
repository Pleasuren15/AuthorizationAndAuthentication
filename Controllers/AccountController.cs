using AuthorizationAndAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthorizationAndAuthentication.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<IdentityUser> _userManager { get; }
        public RoleManager<IdentityRole> _roleManager { get; }
        public SignInManager<IdentityUser> _signInManager { get; }

        private const int ITEMS_PER_PAGE = 7;
        private const string ROLE = "Admin";

        [TempData]
        public string Message { get; set; }

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // GET: /<controller>/
        public IActionResult Admins(int currentPage = 1)
        {
            ViewData["AdminAddedMessage"] = Message;
            return View(new UsersViewModel()
            {
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = currentPage,
                    ItemsPerPage = ITEMS_PER_PAGE,
                    TotalItems = _userManager.Users.Count()
                },
                Users = _userManager.Users.Skip(ITEMS_PER_PAGE * (currentPage - 1))
                                        .Take(ITEMS_PER_PAGE)
            });
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.FindByNameAsync(ROLE) == null)
                    await _roleManager.CreateAsync(new IdentityRole(ROLE));

                IdentityUser user = new IdentityUser()
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email,
                    PhoneNumber = registerViewModel.PhoneNumber,
                };
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, ROLE);
                    Message = $"{registerViewModel.Username} was successfully added";
                    return RedirectToAction("Admins", "Account");
                }
                else
                {
                    foreach (var error in result.Errors.Select(e => e.Description))
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByNameAsync(loginViewModel.Username);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid email or email");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    Message = $"User with Id {id} was deleted";
                    return RedirectToAction("Admins", "Account");
                
                }
                else
                {
                    foreach (var error in result.Errors.Select(e => e.Description))
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user");
            }
            return RedirectToAction("Admins", "Account", _userManager.Users);
        }
    }
}
