using Database;
using Database.Models;
using DotNetEd.CoreAdmin.ViewModels.Tag;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = Database.Models.Type;

namespace DotNetEd.CoreAdmin.Service
{
    public class TagService
    {
        private readonly DeviceBookingContext context;

        public TagService(DeviceBookingContext context)
        {
            this.context = context;
        }

        public async Task<ListOfTags> GetTags()
        {
            var types = await context.Types.Select(t => new TagDTO { Id = t.Id, Name = t.Name, Type="type" }).ToListAsync();
            var os = await context.Os.Select(o => new TagDTO { Id = o.Id, Name = o.Name, Type = "os" }).ToListAsync();
            var tags = await context.Tags.Select(t => new TagDTO { Id = t.Id, Name = t.Name, Type = "tag" }).ToListAsync();
            return new ListOfTags { ListOs = os, ListTag = tags, ListType = types };
        }

        public async Task<string> SaveOs(CreateTag info)
        {
            var os = new Os { Name = info.Name };
            var tags = await context.Os.ToListAsync();

            if (tags.Any(t => t.Name.ToLower() == info.Name.ToLower()))
                return "This tag already exists";

            context.Os.Add(os);

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

        public async Task<string> SaveType(CreateTag info)
        {
            var type = new Type { Name = info.Name };

            var tags = await context.Types.ToListAsync();

            if (tags.Any(t => t.Name.ToLower() == info.Name.ToLower()))
                return "This tag already exists";

            context.Types.Add(type);
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

        public async Task<string> SaveTag(CreateTag info)
        {
            var tag = new Tag { Name = info.Name };

            var tags = await context.Tags.ToListAsync();

            if (tags.Any(t => t.Name.ToLower() == info.Name.ToLower()))
                return "This tag already exists";

            context.Tags.Add(tag);
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

        public async Task DeleteTags(List<TagDTO> info)
        {
            var list = new List<Tag>();

            if (info == null)
                return;

            foreach (var tag in info)
                if (tag.Selected)
                    list.Add(await context.Tags.FindAsync(tag.Id));

            if (list.Count > 0)
                context.Tags.RemoveRange(list);
            try
            {
                await context.SaveChangesAsync();
                return;
            }
            catch
            {
                return;
            }
        }
        public async Task DeleteTypes(List<TagDTO> info)
        {
            var list = new List<Type>();

            if (info == null)
                return;

            foreach (var type in info)
                if (type.Selected)
                {
                    var tag = await context.Types.Include(t => t.Devices).FirstOrDefaultAsync(t => t.Id == type.Id);
                    if (tag != null)
                    {
                        _ = tag.Devices.Select(d => d.TypeId = null);
                        list.Add(tag);
                    }
                }
                    
            if (list.Count > 0)
                context.Types.RemoveRange(list);

            try
            {
                await context.SaveChangesAsync();
                return;
            }
            catch
            {
                return;
            }
        }

        public async Task DeleteOsAmount(List<TagDTO> info)
        {
            var list = new List<Os>();

            if (info == null)
                return;

            foreach (var os in info)
                if (os.Selected)
                {
                    var tag = await context.Os.Include(o => o.Devices).FirstOrDefaultAsync(o => o.Id == os.Id);
                    if (tag != null)
                    {
                        _ = tag.Devices.Select(d => d.OsId = null);
                        list.Add(tag);
                    }
                }
           
            if (list.Count > 0)
                context.Os.RemoveRange(list);              
            try
            {
                await context.SaveChangesAsync();
                return;
            }
            catch
            {
                return;
            }
        }

        public async Task<string> EditOs(ChangeTag tag)
        {
            var os = await context.Os.FindAsync(tag.Id);

            if (os == null)
                return null;

            var tags = await context.Os.ToListAsync();
            if (tags.Any(t => t.Name == tag.Name))
                return "This tag already exists";

            os.Name = tag.Name;

            context.Entry(os).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch 
            {
                return "Error saving to database";
            }

            return "Ok";
        }

        public async Task<string> EditType(ChangeTag tag)
        {
            var type = await context.Types.FindAsync(tag.Id);

            if (type == null)
                return null;

            var tags = await context.Types.ToListAsync();
            if (tags.Any(t => t.Name == tag.Name))
                return "This tag already exists";

            type.Name = tag.Name;

            context.Entry(type).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch 
            {
                return "Error saving to database";
            }

            return "Ok";
        }

        public async Task<string> EditTag(ChangeTag info)
        {
            var tag = await context.Tags.FindAsync(info.Id);

            if (tag == null)
                return null;

            var tags = await context.Tags.ToListAsync();
            if (tags.Any(t => t.Name == info.Name))
                return "This tag already exists";

            tag.Name = info.Name;

            context.Entry(tag).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch 
            {
                return "Error saving to database";
            }

            return "Ok";
        }

        public async Task DeleteTag(long id)
        {
            var tag = await context.Tags.FindAsync(id);

            if (tag == null)
                return;
            
            context.Tags.Remove(tag);

            try
            {
                await context.SaveChangesAsync();
                return;
            }
            catch
            {
                return;
            }
        }

        public async Task DeleteOs(long id)
        {
            var os = await context.Os.Include(o => o.Devices).FirstOrDefaultAsync(o => o.Id == id);

            if (os == null)
                return;

            _ = os.Devices.Select(d => d.OsId = null);
            context.Os.Remove(os);

            try
            {
                await context.SaveChangesAsync();
                return;
            }
            catch
            {
                return;
            }
        }

        public async Task DeleteType(long id)
        {
            var type = await context.Types.Include(o => o.Devices).FirstOrDefaultAsync(o => o.Id == id);

            if (type == null)
                return;

            _ = type.Devices.Select(d => d.TypeId = null);
            context.Types.Remove(type);

            try
            {
                await context.SaveChangesAsync();
                return;
            }
            catch
            {
                return;
            }
        }
    }
}
