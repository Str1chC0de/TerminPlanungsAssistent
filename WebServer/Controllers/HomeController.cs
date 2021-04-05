﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebServer.Models;
using WebServer.Services;
using System.Security.Claims;

namespace WebServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginView()
        {
            return View("~/Views/User/LoginView.cshtml");
        }
        public IActionResult RegisterView()
        {
            return View("~/Views/User/RegisterView.cshtml");
        }
        [Authorized]
        public IActionResult Meetings()
        {
            var meetingConnector = new MeetingConnector();
            string email = "";
            if (User.Identity.IsAuthenticated)
            {
                var dict = User.Claims.Where(p => p.Type == ClaimTypes.Email).ToDictionary(p => p.Type, p => p.Value);
                email = dict.Values.First();
            }
            var meetings = meetingConnector.GetUserMeetings(email).Value;
            if (User.HasClaim(Roles.ADMIN, Roles.ADMIN))
            {
                var result = meetingConnector.GetAllMeetings().Value;
                meetings = result.FindAll(m => m.Taken);
            }
            //ViewData["meetings"] = meetings;
            DateTime date = DateTime.Now;
            meetings.Add(new MeetingModel() { ID = new Guid(), DateTime = date, TimeSpan = new TimeSpan(), Taken = false});
            ViewData["meetings"] = meetings;
            return View();
        }
        [Authorized]
        public IActionResult AdminMeetings()
        {
            var meetingConnector = new MeetingConnector();
            var meetings = meetingConnector.GetAllMeetings().Value;
            ViewData["meetings"] = meetings;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
