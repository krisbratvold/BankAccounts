using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BankAccounts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private BankAccountsContext db;

        public HomeController(BankAccountsContext context)
        {
            db = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Success");
            }
            return View("Index");
        }
        [HttpGet("/home")]
        public IActionResult Success()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Balance = (from trans in db.Transactions where trans.UserId == (int)HttpContext.Session.GetInt32("UserId") select trans.Amount).Sum();
            ViewBag.allTrans = db.Transactions
            .Where(t => t.UserId == (int)HttpContext.Session.GetInt32("UserId"))
            .OrderByDescending(u => u.CreatedAt)
            .ToList();
            return View("Success");
        }

        [HttpPost("/register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "is taken");
                }
            }
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);

            db.Users.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("Name", newUser.FirstName);
            return RedirectToAction("Success");
        }

        [HttpPost("/login")]
        public IActionResult Login(LoginUser newLogin)
        {
            if (ModelState.IsValid == false)
            {
                return View("Index");

            }
            User dbUser = db.Users.FirstOrDefault(user => user.Email == newLogin.LoginEmail);

            if (dbUser == null)
            {
                ModelState.AddModelError("LoginEmail", "incorrect credntials");
                return View("Index");
            }
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            PasswordVerificationResult pwCompareResult = hasher.VerifyHashedPassword(newLogin, dbUser.Password, newLogin.LoginPassword);

            if (pwCompareResult == 0)
            {
                ModelState.AddModelError("LoginEmail", "incorrect credntials");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            HttpContext.Session.SetString("Name", dbUser.FirstName);
            return RedirectToAction("Success");
        }

        [HttpPost("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost("/transaction")]
        public IActionResult Transaction(Transaction newTransaction)
        {
            if (ModelState.IsValid)
            {
                decimal Balance = (from trans in db.Transactions where trans.UserId == (int)HttpContext.Session.GetInt32("UserId") select trans.Amount).Sum();

                if (Balance + newTransaction.Amount < 0)
                {
                    ModelState.AddModelError("Amount", "not enough funds");
                }
            }
            if (ModelState.IsValid == false)
            {
                ViewBag.Balance = (from trans in db.Transactions where trans.UserId == (int)HttpContext.Session.GetInt32("UserId") select trans.Amount).Sum();
                ViewBag.allTrans = db.Transactions
                .Where(t => t.UserId == (int)HttpContext.Session.GetInt32("UserId"))
                .OrderByDescending(u => u.CreatedAt)
                .ToList();
                return View("Success");
            }
            newTransaction.UserId = (int)HttpContext.Session.GetInt32("UserId");
            db.Transactions.Add(newTransaction);
            db.SaveChanges();
            return RedirectToAction("Success");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
