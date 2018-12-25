using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationUsers.Models
{
    public class UserClass : IdentityUser
    {
        public string Name { set; get; }
        public string Surname { set; get; }
        public string Patronymic { set; get; }

    }
}
