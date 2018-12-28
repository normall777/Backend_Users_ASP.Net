using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationUsers.Models;
using WebApplicationUsers.ViewModels;

namespace WebApplicationUsers.Controllers
{
    public class RegisterController : Controller
    {
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