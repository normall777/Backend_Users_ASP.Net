using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationUsers.Data;
using WebApplicationUsers.Models;
using WebApplicationUsers.ViewModels;

namespace WebApplicationUsers.Controllers
{
    public class UsersController : Controller
    {
        
        public UsersController(ApplicationDbContext context)
        {
            
        }
        public IActionResult Index([FromServices] ApplicationDbContext db)
        {
            var users = db.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Создание новых пользователей через ссылку в index
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model, 
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    Patronymic = model.Patronymic,
                    DateOfBirth = model.DateOfBirth
                };
                var result = await userManager.CreateAsync(user, 
                    new Random().Next(1000000).ToString() + DateTime.Now.ToLongDateString() + new Random().Next(1000000).ToString()); //Генерация пароля
                if (result.Succeeded)
                {
                    var _user = await userManager.FindByNameAsync(model.Email);

                    //Генерация токена сброса пароля
                    string code = await userManager.GeneratePasswordResetTokenAsync(_user);
                    var urlEncode = HttpUtility.UrlEncode(code);
                    var callbackUrl = $"{Request.Scheme}://{Request.Host.Value}/Identity/Account/ResetPassword?userId={user.Id}&code={urlEncode}";
                    //Отправка Email
                    EmailSender emailSender = new EmailSender();
                    await emailSender.SendEmailAsync(model.Email, "Reset Password",
                        $"Для создания пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
                    return View("ForgotPasswordConfirmation");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }




        /// <summary>
        /// Переход на страницу редактирования/регистрации пользователя
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(string id, 
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditViewModel model = new EditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Name = user.Name,
                Patronymic = user.Patronymic,
                Surname = user.Surname
            };
            return View(model);
        }
        /// <summary>
        /// Процедура регистрации/редактирования пользователя с сохранением
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model,
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.DateOfBirth = model.DateOfBirth;
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.Patronymic = model.Patronymic;

                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Remove(string id,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Показ формы регистрации
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Метод регистрации (при нажатии "Зарегистрироваться")
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] SignInManager<ApplicationUser> signInManager)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    Patronymic = model.Patronymic,
                    DateOfBirth = model.DateOfBirth
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false); //После регистрации пользователь входит
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

    }
}