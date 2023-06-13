using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Service
{
    public class AuthorizationService
    {
        private readonly DeviceBookingContext context;
        public AuthorizationService(DeviceBookingContext context)
        {
            this.context = context;
        }

        public async Task<Employee> CheckUser(string username, string password)
        {
            var user = await context.Employees.FirstOrDefaultAsync(u => u.Login == username);

            if (user == null)
                return null;

            if (password != user.Password)
                return null;

            return user;
        }
    }
}
