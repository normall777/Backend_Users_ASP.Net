using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationUsers.Data;
using WebApplicationUsers.Models;

namespace WebApplicationUsers.Controllers
{
    public class UsersController : Controller
    {
        ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
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


    }
}