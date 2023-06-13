using Admin.ViewModels.Department;
using Database;
using Database.Models;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Service
{
    public class DepartmentService
    {
        private readonly DeviceBookingContext context;

        public DepartmentService(DeviceBookingContext context)
        {
            this.context = context;
        }

        public async Task<List<DepartmentDTO>> GetDepartmentsFromDB()
        {
            return await context.Departments.Select(d => new DepartmentDTO { Id = d.Id, Name = d.Name }).ToListAsync();
        }

        public async Task<string> CreateDepartment(DepartmentToCreate info)
        {
            context.Departments.Add(new Department { Name = info.Name });
            try
            {
                await context.SaveChangesAsync();
                return null;
            }
            catch
            {
                return "Error saving to database";
            }
        }

        public async Task<DepartmentToEdit> GetDepartmentToEdit(long id)
        {
            var department = await context.Departments.FindAsync(id);
            if (department == null)
                return null;
            return new DepartmentToEdit { Id = department.Id, Name = department.Name };
        }

        public async Task<string> EditDepartment(DepartmentToEdit info)
        {
            var department = await context.Departments.FindAsync(info.Id);

            if (department == null)
                return "Department is not found";

            department.Name = info.Name;

            context.Entry(department).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return "Error saving to database";
            }

            return null;
        }

        public async Task<string> DeleteDepartment(long id)
        {
            var department = await context.Departments.Include(d => d.Devices)
                                                      .Include(d => d.Users)
                                                      .Include(d => d.Records)
                                                      .FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
                return null;

            _ = department.Devices.Select(d => d.DepartmentId = null);
            _ = department.Users.Select(u => u.DepartmentId = null);
            _ = department.Records.Select(r => r.DepartmentId = null);

            context.Departments.Remove(department);
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

        public async Task<string> DeleteDepartments(DepartmentsList list)
        {
            var departments = new List<Department>();

            if (list.Departments == null)
                return null;

            foreach(var department in list.Departments)
                if (department.Selected)
                {
                    var info = await context.Departments.Include(d => d.Devices)
                                                      .Include(d => d.Users)
                                                      .Include(d => d.Records)
                                                      .FirstOrDefaultAsync(d => department.Id == d.Id);
                    if (info == null)
                        continue;
                    _ = info.Devices.Select(d => d.DepartmentId = null);
                    _ = info.Users.Select(u => u.DepartmentId = null);
                    _ = info.Records.Select(r => r.DepartmentId = null);
                    departments.Add(info);
                }
          
            if (departments.Count > 0)
                context.Departments.RemoveRange(departments);
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
