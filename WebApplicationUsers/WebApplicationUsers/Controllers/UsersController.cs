using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationUsers.Data;
using WebApplicationUsers.Models;
using WebApplicationUsers.ViewModels;

namespace WebApplicationUsers.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        /// <summary>
        /// Возвращает представление Users.Index
        /// </summary>
        public IActionResult Index([FromServices] ApplicationDbContext db)
        {
            var users = db.Users.ToList();
            return View(users);
        }

        /// <summary>
        /// Генерация токена сброса пароля
        /// </summary>
        public async void ResetPassword(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            string code = await userManager.GeneratePasswordResetTokenAsync(user);
            var urlEncode = HttpUtility.UrlEncode(code);
            var callbackUrl = $"{Request.Scheme}://{Request.Host.Value}/Identity/Account/ResetPassword?userId={user.Id}&code={urlEncode}";
            //Отправка Email
            EmailSender emailSender = new EmailSender();
            await emailSender.SendEmailAsync(user.Email, "Reset Password",
                $"{user.Name} {user.Surname}!<p>Для задания пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
        }



        /// <summary>
        /// Возвращает представление Create - создание пользователя администратором
        /// </summary>
        /// <returns></returns>
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
                    // Отправить пользователю пароль
                    ResetPassword(_user, userManager);
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

        /// <summary>
        /// Кнопка для удаления пользователя
        /// </summary>
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
        /// Кнопка сброса пароля
        /// </summary>
        public async Task<IActionResult> ResetPasswordButton(string id,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                ResetPassword(user, userManager);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Изменение роли
        /// </summary>
        public async Task<IActionResult> ToggleAdmin(string id,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync("Admin");
            if (role == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = await userManager.FindByIdAsync(id);
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }

    }
}