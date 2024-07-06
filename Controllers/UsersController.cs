using System.Net;
using System.Security.Claims;
using System.Text;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Posts");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "E-mail veya parola hatalı.");
                return View(model);
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password!, model.Password);
            if (result == PasswordVerificationResult.Failed)
            { 
                ModelState.AddModelError("", "E-mail veya parola hatalı");
                return View();
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name,user.UserName!),
                new Claim(ClaimTypes.GivenName,user.FullName!),
                new Claim(ClaimTypes.UserData,user.Image??"")
            };
            if (user.UserName=="Admin")
            {
                claims.Add(new Claim(ClaimTypes.Role,"admin"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
                
                RedirectUri = "/Home/Index",
            };

           await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
               new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Posts");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isUser = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email || x.UserName == model.UserName);
            if (isUser != null)
            {
                ModelState.AddModelError("", "Email adresiniz ve Kullanıcı Adınız benzersiz olmalıdır.");
                return View(model);
            }

            var enc = new PasswordHasher<User>();
            var passwordHash = enc.HashPassword(null, model.Password);

            var user = new User()
            {
                Email = model.Email,
                Name = model.Name,
                SurName = model.SurName,
                Password = passwordHash,
                UserName = model.UserName,
                Image = model.Image
            };
            await _userRepository.AddUserAsync(user);
            await Login(new LoginViewModel() { Email = model.Email, Password = model.Password });

            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult Profile(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                return NotFound();
            }
            
            var user=_userRepository.Users.Include(x=>x.Posts).ThenInclude(x=>x.Tags).Include(x=>x.Comments).FirstOrDefault(x => x.UserName == username);
            if (user==null)
            {
                return BadRequest();
            }
            return View(user);
        }
        [Authorize]
        public async Task<IActionResult>EditProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
           var user =_userRepository.Users.FirstOrDefault(x => x.UserId==userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserEditModel
            {
                Id=user.UserId,
                UserName=user.UserName,
                Email=user.Email,
                Name=user.Name,
                SurName=user.SurName,
                Image=user.Image,
                Password=user.Password,
            };
            ViewBag.success = "Profiliniz başarıyla güncellendi";
            return View(model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditProfile(IFormFile? File, UserEditModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("","Girilen bilgileri doğru formatta girdiğinizden emin olun!");
                return View(model);
            }



            var VerifiedUser=await _userRepository.Users.FirstOrDefaultAsync(x => x.UserId == model.Id);
            if (VerifiedUser==null)
            {
                return BadRequest();
            }

            var isExists = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == model.UserName.ToLower() || x.Email.ToLower() == model.Email);
            if (isExists!=null&&isExists!=VerifiedUser)
            {
                ModelState.AddModelError("","email adresi ve username alanı benzersiz olmalıdır.");
                return View(model);
            }

            if (File!=null)
            {
                var allowedExtensions= new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(File.FileName);
                if (allowedExtensions.Contains(extension))
                {
                    var randomFileName = String.Format($"{Guid.NewGuid()}{extension}");
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                    using (var stream = new FileStream(path, FileMode.Append))
                    {
                        await File.CopyToAsync(stream);
                    }
                    model.Image= randomFileName;

                }
            }
            var enc = new PasswordHasher<User>();
            
            var user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                Name = model.Name,
                SurName = model.SurName,
                Image = model.Image,
                UserId = model.Id,
                Password = model.Password == null ? VerifiedUser.Password : enc.HashPassword(VerifiedUser, model.Password),
                  
                
            };
           await _userRepository.UpdateAsync(user);

            return RedirectToAction("Successfull");
        }

        [HttpGet]
        public IActionResult Successfull()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

    }
}
