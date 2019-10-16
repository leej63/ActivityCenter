using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CsharpBeltExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CsharpBeltExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

// ************************************************************************************************************
        
        // Login & Register Page
        [HttpGet("signin")]
        public IActionResult Signin()
        {
            return View("Signin");
        }

        //Register User
        [HttpPost("register")]
        public IActionResult RegisterUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("Signin");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetString("LoginUserEmail", newUser.Email);
                HttpContext.Session.SetInt32("CurUserId", newUser.UserId);
                return RedirectToAction("Home");
            }
            return View("Signin");
        }

        // Login User
        [HttpPost("login")]
        public IActionResult LoginUser(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Signin");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Password is incorrect, please try again.");
                    return View("Signin");
                }
                HttpContext.Session.SetString("LoginUserEmail", userSubmission.Email);
                HttpContext.Session.SetInt32("CurUserId", userInDb.UserId);
                return RedirectToAction("Home");
            }
            return View("Signin");
        }

        // Home - shows all activities
        [HttpGet("home")]
        public IActionResult Home()
        {
            if(HttpContext.Session.GetInt32("CurUserId") == null)
            {
                return RedirectToAction("Signin");
            }
            int CurUser = (int)HttpContext.Session.GetInt32("CurUserId");
            User user = dbContext.Users.Include(u => u.CreatedActivities)
                .Include(u => u.Participating)
                .ThenInclude(u => u.Activity)
                .FirstOrDefault(u => u.UserId == CurUser);
            ViewBag.user = user;
            List<Activityy> AllActivities = dbContext.Activities
                .Include(a => a.Participants)
                .ThenInclude(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
            List<Activityy> participating = new List<Activityy>();
            List<Activityy> notParticipating = new List<Activityy>();
            foreach(Participant p in user.Participating)
            {
                participating.Add(p.Activity);
            }
            foreach(Activityy activity in AllActivities)
            {
                if(!participating.Contains(activity))
                {
                    notParticipating.Add(activity);
                }
            }
            ViewBag.notParticipating = notParticipating;
            // show Activity Organizer on Hom page
            return View("Home", AllActivities);
        }

        // Create activity page
        [HttpGet("new")]
        public IActionResult Activity()
        {
            if(HttpContext.Session.GetInt32("CurUserId") == null)
            {
                return RedirectToAction("Signin");
            }
            return View("Activity");
        }

        // Create a activity
        [HttpPost("activity/create")]
        public IActionResult CreateActivity(Activityy newActivity)
        {
            if(HttpContext.Session.GetInt32("CurUserId") == null)
            {
                return RedirectToAction("Signin");
            }
            newActivity.UserId = (int)HttpContext.Session.GetInt32("CurUserId");
            if(ModelState.IsValid)
            {
                dbContext.Add(newActivity);
                dbContext.SaveChanges();
                return RedirectToAction("Home");
            }
            return View("Activity");
        }

        // Details of one activity page
        [HttpGet("activity/{activityId}")]
        public IActionResult OneActivity(int activityId)
        {
            if(HttpContext.Session.GetInt32("CurUserId") == null)
            {
                return RedirectToAction("Signin");
            }
            if(!dbContext.Activities.Any(u => u.ActivityId == activityId))
            {
                return RedirectToAction("Home");
            }
            Activityy oneActivity = dbContext.Activities
                .Include(a => a.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefault(a => a.ActivityId == activityId);
            User selectedUser = dbContext.Users
                .Include(u => u.CreatedActivities)
                .FirstOrDefault(u => u.UserId == oneActivity.UserId);
            ViewBag.Creator = selectedUser.FirstName;
            int CurUser = (int)HttpContext.Session.GetInt32("CurUserId");
            User user = dbContext.Users.Include(u => u.CreatedActivities)
                .Include(u => u.Participating)
                .ThenInclude(u => u.Activity)
                .FirstOrDefault(u => u.UserId == CurUser);
            ViewBag.user = user;
            ///////////////
            List<Activityy> AllActivities = dbContext.Activities
                .Include(a => a.Participants)
                .ThenInclude(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
            List<Activityy> participating = new List<Activityy>();
            List<Activityy> notParticipating = new List<Activityy>();
            foreach(Participant p in user.Participating)
            {
                participating.Add(p.Activity);
            }
            foreach(Activityy activity in AllActivities)
            {
                if(!participating.Contains(activity))
                {
                    notParticipating.Add(activity);
                }
            }
            ViewBag.notParticipating = notParticipating;

            return View("Details", oneActivity);
        }

        // Logout
        [HttpGet("logout")]
        public IActionResult LogoutUser()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Signin");
        }

        // Delete Activity
        [HttpGet("delete/{activityId}")]
        public IActionResult DeleteActivity(int activityId)
        {
            if(HttpContext.Session.GetInt32("CurUserId") == null)
            {
                return RedirectToAction("Signin");
            }
            if(!dbContext.Activities.Any(u => u.ActivityId == activityId))
            {
                return RedirectToAction("Home");
            }
            Activityy deleteActivity = dbContext.Activities.FirstOrDefault(a => a.ActivityId == activityId);
            int curUser = (int)HttpContext.Session.GetInt32("CurUserId");
            if(deleteActivity.UserId != curUser)
            {
                return RedirectToAction("Home");
            }
            dbContext.Activities.Remove(deleteActivity);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        // Join Activity
        [HttpGet("join/{activityId}")]
        public IActionResult JoinActivity(int activityId)
        {
            if(HttpContext.Session.GetInt32("CurUserId") == null)
            {
                return RedirectToAction("Signin");
            }
            if(!dbContext.Activities.Any(u => u.ActivityId == activityId))
            {
                return RedirectToAction("Home");
            }
            int curUser = (int)HttpContext.Session.GetInt32("CurUserId");
            Participant selectedParticipant = new Participant();
            selectedParticipant.UserId = curUser;
            selectedParticipant.ActivityId = activityId;
            dbContext.Add(selectedParticipant);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        // un-Join Activity
        [HttpGet("cancel/{activityId}")]
        public IActionResult CancelJoinActivity(int activityId)
        {
            if(HttpContext.Session.GetInt32("CurUserId") == null)
            {
                return RedirectToAction("Signin");
            }
            if(!dbContext.Activities.Any(u => u.ActivityId == activityId))
            {
                return RedirectToAction("Home");
            }
            int curUser = (int)HttpContext.Session.GetInt32("CurUserId");
            Participant selectedParticipant = dbContext.Participants.FirstOrDefault(p => p.ActivityId == activityId && p.UserId == curUser);
            if(selectedParticipant.Equals(default(Participant)))
            {
                return RedirectToAction("Home");
            }
            dbContext.Participants.Remove(selectedParticipant);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

// ************************************************************************************************************

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
