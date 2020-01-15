using Google.Authenticator;
using GoogleAuthenticator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoogleAuthenticator.Controllers
{
    // https://medium.com/@fleekdeveloper/2fa-with-google-authenticator-in-asp-mvc-4788c79c47
    // https://bitbucket.org/fleekdeveloper/fleekdeveloper.authenticator.mvc/src/master/
    public class HomeController : Controller
    {
        private const string Key = "test123";

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel login)
        {
            string message;
            var status = false;
            if (login.Username == "Admin" && login.Password == "password")
            {
                status = true;
                message = "Use Google Authenticator App to Scan the QR Code:";
                Session["UserName"] = login.Username;
                var authenticator = new TwoFactorAuthenticator();
                var result = authenticator.GenerateSetupCode("Developer", "Nitin", Key, 300, 300);
                ViewBag.BarcodeImageUrl = result.QrCodeSetupImageUrl;
                ViewBag.SetupCode = result.ManualEntryKey;
            }
            else
            {
                message = "Invalid Credentials";
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }

        public ActionResult UserProfile()
        {
            if (Session["UserName"] == null || Session["IsValid2FA"] == null || !(bool)Session["IsValid2FA"])
            {
                return RedirectToAction("Login");
            }
            ViewBag.Message = $"Welcome {Session["UserName"]}. You have logged in successfully!";
            return View();
        }

        public ActionResult Verify2Fa()
        {
            var token = Request["passcode"];
            var authenticator = new TwoFactorAuthenticator();
            var x = authenticator.GetCurrentPIN(Key);
            var isValid = authenticator.ValidateTwoFactorPIN(Key, token);
            if (isValid)
            {
                Session["IsValid2FA"] = true;
                return RedirectToAction("UserProfile", "Home");
            }
            return RedirectToAction("Login", "Home");
        }
    }
}