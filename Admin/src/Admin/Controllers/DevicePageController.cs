﻿using Database;
using Database.Models;
using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels.Device;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Controllers
{

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DevicePageController : Controller
    {
        private readonly DeviceBookingContext context;
        private readonly DeviceService deviceService;
        private static readonly Dictionary<string, string> MapDevice = new()
        {
            {"Budget", "Бюджетный" },
            {"MidRange", "Среднебюджетный" },
            {"Flagship", "Флагман" }
        };
        public DevicePageController(DeviceBookingContext context,
            DeviceService deviceService)
        {
            this.context = context;
            this.deviceService = deviceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Message = TempData["shortMessage"]?.ToString();
            var devices = new DeviceList { Devices = await deviceService.GetDevicesFromDB() };
            return View(devices);
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost(long id)
        {
            var message = await deviceService.DeleteDevice(id);

            if (message != null)
            {
                TempData["shortMessage"] = message;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> DeleteAmountPost(DeviceList deviceList)
        {         
            var message = await deviceService.DeleteDevices(deviceList);

            if (message != null)
            {
                TempData["shortMessage"] = message;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var os = context.Os.Select(o => 
            new SelectListItem { 
                Value = o.Id.ToString(), 
                Text = o.Name, 
                Selected = false
            }).ToList();

            var classOfDevice = (Enum.GetValues(typeof(ClassOfDevice)).Cast<int>()
                                                                      .Select(e => 
                                                                      new SelectListItem() 
                                                                      { 
                                                                          Text = MapDevice[Enum.GetName(typeof(ClassOfDevice), e)], 
                                                                          Value = e.ToString(),
                                                                          Selected = false
                                                                      })).ToList();
            var department = context.Departments.Select(d => 
            new SelectListItem { 
                Value = d.Id.ToString(), 
                Text = d.Name, 
                Selected = false 
            }).ToList();

            var type = context.Types.Select(t => 
            new SelectListItem { 
                Value = t.Id.ToString(), 
                Text = t.Name, 
                Selected = false 
            }).ToList();

            var noneItem = new SelectListItem { Text = "Не выбран", Value = "", Selected = true };

            os.Add(noneItem);
            department.Add(noneItem);
            type.Add(noneItem);
            classOfDevice.Add(noneItem);

            ViewBag.OsId = os;
            ViewBag.DepartmentId = department;
            ViewBag.TypeId = type;
            ViewBag.ClassOfDevice = classOfDevice;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] DeviceToCreate formData)
        {
            if (ModelState.IsValid)
            {
                var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/device/info/full/";
                await deviceService.CreateDevice(formData, url);
                if (!formData.Stay)
                {
                    return RedirectToAction("Index");
                }
                ModelState.Clear();
            }
            var os = context.Os.Select(o => new SelectListItem 
            { 
                Value = o.Id.ToString(), 
                Text = o.Name, 
                Selected = formData.OsId != null && o.Id == formData.OsId 
            }).ToList();

            var department = context.Departments.Select(d => new SelectListItem 
            { 
                Value = d.Id.ToString(), 
                Text = d.Name, 
                Selected = formData.DepartmentId != null && d.Id == formData.DepartmentId
            }).ToList();

            var type = context.Types.Select(t => new SelectListItem 
            { 
                Value = t.Id.ToString(), 
                Text = t.Name, 
                Selected = formData.TypeId != null && t.Id == formData.TypeId
            }).ToList();

            var classOfDevice = (Enum.GetValues(typeof(ClassOfDevice)).Cast<int>()
                                                                      .Select(e =>
                                                                      new SelectListItem()
                                                                      {
                                                                          Text = MapDevice[Enum.GetName(typeof(ClassOfDevice), e)],
                                                                          Value = e.ToString(),
                                                                          Selected = false
                                                                      }))
                                                                      .ToList();

            os.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.OsId == null });
            department.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.DepartmentId == null });
            type.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.TypeId == null });
            classOfDevice.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.ClassOfDevice == null });

            ViewBag.OsId = os;
            ViewBag.DepartmentId = department;
            ViewBag.TypeId = type;
            ViewBag.ClassOfDevice = classOfDevice;

            return View("Create");
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var device = await deviceService.GetDeviceToEdit(id);

            if (device == null)
            {
                TempData["shortMessage"] = "Device is not found";
                return RedirectToAction("Index");
            }

            var os = context.Os.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.Name,
                Selected = device.OsId != null && o.Id == device.OsId
            }).ToList();

            var department = context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = device.DepartmentId != null && d.Id == device.DepartmentId
            }
            ).ToList();

            var type = context.Types.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name,
                Selected = device.TypeId != null && t.Id == device.TypeId
            }
            ).ToList();

            var classOfDevice = (Enum.GetValues(typeof(ClassOfDevice)).Cast<int>()
                                                          .Select(e =>
                                                          new SelectListItem()
                                                          {
                                                              Text = MapDevice[Enum.GetName(typeof(ClassOfDevice), e)],
                                                              Value = e.ToString(),
                                                              Selected = device.ClassOfDevice != null && ((int)device.ClassOfDevice) == e
                                                          }))
                                                          .ToList();

            os.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = device.OsId == null });
            department.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = device.DepartmentId == null });
            type.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = device.TypeId == null });
            classOfDevice.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = device.ClassOfDevice == null });

            ViewBag.OsId = os;
            ViewBag.DepartmentId = department;
            ViewBag.TypeId = type;
            ViewBag.ClassOfDevice = classOfDevice;
            ViewBag.Address = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return View("Edit", device);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] DeviceToEdit formData)
        {
            if (ModelState.IsValid)
            {
                var message = await deviceService.EditDevice(formData.Id, formData);

                if (message != null)
                {
                    TempData["shortMessage"] = message;
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }

            var os = context.Os.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.Name,
                Selected = formData.OsId != null && o.Id == formData.OsId
            }).ToList();

            var department = context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = formData.DepartmentId != null && d.Id == formData.DepartmentId
            }).ToList();

            var type = context.Types.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name,
                Selected = formData.TypeId != null && t.Id == formData.TypeId
            }).ToList();

            var classOfDevice = (Enum.GetValues(typeof(ClassOfDevice)).Cast<int>()
                                                                      .Select(e =>
                                                                      new SelectListItem()
                                                                      {
                                                                          Text = MapDevice[Enum.GetName(typeof(ClassOfDevice), e)],
                                                                          Value = e.ToString(),
                                                                          Selected = false
                                                                      }))
                                                                      .ToList();

            os.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.OsId == null });
            department.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.DepartmentId == null });
            type.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.TypeId == null });
            classOfDevice.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.ClassOfDevice == null });

            ViewBag.OsId = os;
            ViewBag.DepartmentId = department;
            ViewBag.TypeId = type;
            ViewBag.ClassOfDevice = classOfDevice;
            ViewBag.Address = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            formData.ImagePath ??= "/default.png";

            return View("Edit", formData);
        }
    }
}
