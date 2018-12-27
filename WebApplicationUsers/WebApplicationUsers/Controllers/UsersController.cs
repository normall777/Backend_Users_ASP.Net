using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        /// <summary>
        /// Переход на страницу редактирования/регистрации пользователя
        /// </summary>
        [HttpGet]
        public IActionResult Edit(string id, [FromServices] ApplicationDbContext db)
        {
            //TODO:

            return View("Index");
        }
        /// <summary>
        /// Процедура регистрации/редактирования пользователя с сохранением
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user,
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            //TODO:
            return View("Index");
        }

        public async Task<IActionResult> Remove(string id,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            //TODO:
            return View("Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
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
                    await signInManager.SignInAsync(user, false);
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