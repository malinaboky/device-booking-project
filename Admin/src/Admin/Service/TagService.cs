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
            foreach (var tag in info)
                if (tag.Selected)
                    list.Add(await context.Tags.Include(t => t.TagInfos).FirstOrDefaultAsync(t => t.Id == tag.Id));

            var tagInfos = list.Where(t => t.TagInfos != null).SelectMany(t => t.TagInfos).ToList();

            if (tagInfos.Count > 0)
                context.TagInfos.RemoveRange(tagInfos);
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
            foreach (var type in info)
                if (type.Selected)
                    list.Add(await context.Types.FindAsync(type.Id));

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
            foreach (var os in info)
                if (os.Selected)
                    list.Add(await context.Os.Include(o => o.Devices).FirstOrDefaultAsync( o => o.Id == os.Id));

            if (list.Count > 0)
            { 
                context.Os.RemoveRange(list);
            }
                
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

        public async Task EditOs(ChangeTag tag)
        {
            var os = await context.Os.FindAsync(tag.Id);

            if (os == null)
                return;
            os.Name = tag.Name;

            context.Entry(os).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return;
            }

            return;
        }
    }
}
