using Admin.ViewModels.Employee;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Service
{
    public class EmployeeService
    {
        private readonly DeviceBookingContext context;

        public EmployeeService(DeviceBookingContext context)
        {
            this.context = context;
        }

        public async Task<EmployeeDTO> GetEmployeeFromDB(long id)
        {
            var employee = await context.Employees.FindAsync(id);

            if (employee == null)
                return null;

            return new EmployeeDTO { Login = employee.Login, Status = employee.Status };
        }

        public async Task<string> SavePassword(long id, string newPassword)
        {
            var user = await context.Employees.FindAsync(id);

            if (user == null)
                return "User is not found";

            user.Password = newPassword;

            context.Entry(user).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
                return null;
            }
            catch
            {
                return "Error saving to db";
            }
        }
    }
}
