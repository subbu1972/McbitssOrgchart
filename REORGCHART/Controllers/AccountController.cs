using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.DirectoryServices.AccountManagement;

using REORGCHART.Models;
using REORGCHART.Data;
using REORGCHART.Helper;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace REORGCHART.Controllers
{
    public class EmailManager
    {
        public static void AppSettings(out string UserID, out string Password, out string SMTPPort, out string Host)
        {
            UserID = ConfigurationManager.AppSettings.Get("UserID");
            Password = ConfigurationManager.AppSettings.Get("Password");
            SMTPPort = ConfigurationManager.AppSettings.Get("SMTPPort");
            Host = ConfigurationManager.AppSettings.Get("Host");
        }
        public static void SendEmail(string From, string Subject, string Body, string To, string UserID, string Password, string SMTPPort, string Host)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(To);
            mail.From = new MailAddress(From);
            mail.Subject = Subject;
            mail.Body = Body;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = Host;
            smtp.Port = Convert.ToInt16(SMTPPort);
            smtp.Credentials = new NetworkCredential(UserID, Password);
            smtp.EnableSsl = true;
            mail.IsBodyHtml = true;
            smtp.Send(mail);
        }
    }

    public class AccountController : Controller
    {
        string ConfigureCompany = ConfigurationManager.AppSettings.Get("ConfigureCompany");

        private DBContext db = new DBContext();
        ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
                return _signInManager;
            }
        }

        ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                return _userManager;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

            }

            string wcCompanyName = ConfigurationManager.AppSettings["wcCompanyName"].ToString();
            LoginModel model = new LoginModel();
            model.UserName = "";
            model.Password = "";

            List<InitializeTables> INI = (from ini in db.InitializeTables select ini).ToList();
            List<SelectListItem> listIT = new List<SelectListItem>();
            foreach (var item in INI)
            {
                if (item.CompanyName.ToString()==wcCompanyName || wcCompanyName=="")
                    listIT.Add(new SelectListItem() { Value = item.CompanyName.ToString(), Text = item.CompanyName });
            }
            ViewBag.LstCompanyName = listIT;

            ViewBag.Title = "Login";
            HttpContext.Session["LoginUserInf"] = null;

            return View(model);
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl, string cbxDataPrivacy)
        {
            returnUrl = "/";
            HttpContext.Session["LoginUserInf"] = null;
            if (cbxDataPrivacy == "on")
            {
                if (ModelState.IsValid)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, change to shouldLockout: true
                    var result = await SignInManager.PasswordSignInAsync(model.UserName, 
                                                                         model.Password, 
                                                                         model.RememberMe.Value, 
                                                                         shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            return RedirectToLocal(returnUrl);
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ViewBag.ErrorMessage = "The user name or password provided is incorrect";
                            return View(model);
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Please accept data privacy policy";

            }

            string wcCompanyName = ConfigurationManager.AppSettings["wcCompanyName"].ToString();
            List<InitializeTables> INI = (from ini in db.InitializeTables select ini).ToList();
            List<SelectListItem> listIT = new List<SelectListItem>();
            foreach (var item in INI)
            {
                if (item.CompanyName.ToString() == wcCompanyName || wcCompanyName == "")
                    listIT.Add(new SelectListItem() { Value = item.CompanyName.ToString(), Text = item.CompanyName });
            }
            ViewBag.LstCompanyName = listIT;

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/LogOff
        public ActionResult LogOff()
        {
            HttpContext.Session["LoginUserInf"] = null;
            Models.DBContext db = new Models.DBContext();
            string wcCompanyName = ConfigurationManager.AppSettings["wcCompanyName"].ToString();
            var INI = (from ini in db.InitializeTables
                       where ini.Authentication.ToUpper() == "WINDOWS" && ini.CompanyName == wcCompanyName
                       select ini).FirstOrDefault();
            if (INI != null)
            {
                string[] LogonUser = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.ToString().Split('\\');
                ViewBag.UserName = LogonUser[1];
                ViewBag.URL = "/";
            }
            else
            {
                ApplicationUser UserData = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                ViewBag.UserName = UserData.UserName;
                ViewBag.URL = "Login";
            }
            ViewBag.Title = "Log Off";

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return View();
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model, string cbxDataPrivacy)
        {
            if (cbxDataPrivacy == "on")
            {
                Models.DBContext dbContext = new Models.DBContext();
                Models.CompanyDetails cmp = new Models.CompanyDetails();
                var FindCompany = (from d in dbContext.Company
                                   where d.CompanyName == model.CompanyName
                                   select new { d.CompanyName }).ToList();

                string wcCompanyName = ConfigurationManager.AppSettings["wcCompanyName"].ToString();
                var INI = (from ini in dbContext.InitializeTables
                           where ini.Authentication.ToUpper() == "WINDOWS" && ini.CompanyName == wcCompanyName
                           select ini).FirstOrDefault();
                if (INI != null)
                {
                    string[] WindowsUser = Request.LogonUserIdentity.User.ToString().Split('\\');
                    string WindowUserName = WindowsUser[0];
                    if (WindowsUser.Length >= 2) WindowUserName = WindowsUser[1];
                    model.UserName = WindowUserName;
                }

                if (ModelState.IsValid)
                {
                    // Attempt to register the user
                    if (FindCompany.Count == 0)
                    {
                        var user = new ApplicationUser { UserName = model.UserName,
                                                         Email = model.Email,
                                                         CompanyName = model.CompanyName };
                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            cmp.CompanyName = user.CompanyName;
                            cmp.EmailID = user.Email;
                            cmp.UserName = user.UserName;
                            cmp.Logo = null;
                            dbContext.Company.Add(cmp);

                            Models.UserRoles usr = new Models.UserRoles();
                            usr.UserId = model.UserName;
                            usr.Role = model.Roles;
                            usr.CompanyName = user.CompanyName;
                            usr.Email = model.Email;
                            dbContext.UserRoles.Add(usr);
                            dbContext.SaveChanges();

                            using (SqlCommand cmd = new SqlCommand())
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;
                                cmd.CommandText = "PROC_CONFIGURE_COMPANY";

                                cmd.Parameters.Add("@CURRENT_COMPANY", SqlDbType.VarChar, 500).Value = ConfigureCompany;
                                cmd.Parameters.Add("@CONFIGURE_COMPANY", SqlDbType.VarChar, 500).Value = user.CompanyName;

                                Common csobj = new Common();
                                csobj.SPReturnDataTable(cmd);
                            }


                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            string EMail = ConfigurationManager.AppSettings.Get("EmailID");
                            string To = model.Email, UserID, Password, SMTPPort, Host;

                            var request = HttpContext.Request;
                            string lnkHref = "<a href='" + HttpContext.Request.Url.Scheme + "://" +
                                       HttpContext.Request.Url.Authority +
                                        "/Home/ApprovalList/?CustRegId=" + user.Id +
                                       "'>" + "Approval link" + "</a>";
                            
                            //HTML Template for Send email 
                            string subject = "Request for Approval of subscription";
                            string body = "<b>Dear Admin, </b><br/><br/>";
                            body = body + "<b>Kindly Approve The Customer: " + user.CompanyName + "&nbsp; Request for enrollement.</b><br/>" + "<b>Please find the Customers list </b>&nbsp; : " + lnkHref;
                            body = body + "<br/><br/><b>Thanks,</b><br/>" + "<b>Mcbitss Team.</b>";
                            
                            //Get and set the AppSettings using configuration manager.  
                            EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);
                            
                            //Call send email methods.  
                            EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);

                            return Redirect("~/");
                        }

                        ViewBag.ErrorMessage = result.Errors.First();
                    }
                    else
                    {

                        ViewBag.ErrorMessage = "Company details are already added";
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Please accept data privacy policy && Terms";
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult UserRegister()
        {
            Models.DBContext dbContext = new Models.DBContext();
            Models.CompanyDetails cmp = new Models.CompanyDetails();
            var FindCompany = (from d in dbContext.Company
                               where d.UserName == User.Identity.Name
                               select new { d.CompanyName }).ToList();

            string CompanyName = FindCompany[0].CompanyName;
            ViewBag.CompanyName = CompanyName;
            ViewBag.ErrorMessage = "";
            ViewBag.InitialValues = JsonConvert.SerializeObject((from iv in db.InitializeTables where iv.CompanyName == CompanyName select iv).FirstOrDefault());

            RegisterModel model = new RegisterModel();
            model.UserName = "";
            model.CompanyName = ViewBag.CompanyName;
            model.Email = "";
            model.Upload = "";
            model.Password = "";
            model.ConfirmPassword = "";
            model.ConfirmPassword = "";
            model.ConfirmPasswordName = "";
            model.RequestType = "Individual";

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserRegister(RegisterModel model, HttpPostedFileBase Upload, string cbxRequestType)
        {
            DataTable dtUsers = new DataTable();
            ViewBag.CompanyName = model.CompanyName;
            string UploadFileName = "";

            model.RequestType = cbxRequestType;
            if (cbxRequestType == "Individual")
            {
                model.Upload = "No File";
                model.PasswordName = model.Password;
                model.ConfirmPasswordName = model.ConfirmPassword;
                if (ModelState.IsValid)
                {
                    // Attempt to register the user
                    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, CompanyName = model.CompanyName };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        Models.DBContext dbContext = new Models.DBContext();
                        Models.UserRoles usr = new Models.UserRoles();

                        usr.UserId = model.UserName;
                        usr.Role = model.Roles.Replace("Planner", "Player");
                        usr.CompanyName = model.CompanyName;
                        usr.Email = model.Email;

                        dbContext.UserRoles.Add(usr);
                        dbContext.SaveChanges();

                        try
                        {
                            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            string EMail = ConfigurationManager.AppSettings.Get("EmailID");
                            string To = model.Email, UserID, Password, SMTPPort, Host;

                            var request = HttpContext.Request;
                            string lnkHref = "<a href='" + HttpContext.Request.Url.Scheme + "://" +
                                       HttpContext.Request.Url.Authority +
                                        "'> " + "link" + "</a>";

                            //HTML Template for Send email 
                            string subject = "Login Created";
                            string body = "<b>Dear " + model.UserName + ", </b><br/><br/>";
                            body = body + "<b>Your login credentials created, Kindly change the pasword at the earliest" + lnkHref + "</b><br/>";
                            body = body + "<b>User Name :  " + model.UserName + "</b><br/>"; ;
                            body = body + "<b>Password :  " + model.Password + "</b><br/>"; ;
                            body = body + "<br/><br/><b>Thanks,</b><br/>" + "<b>" + model.CompanyName + "Team.</b>";

                            //Get and set the AppSettings using configuration manager.  
                            EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);

                            //Call send email methods.  
                            EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
                        }
                        catch(Exception ex)
                        {
                            UploadFileName += model.UserName + " email not sent to this user(" + ex.Message + ")\n";
                        }
                    }
                    else UploadFileName += result.Errors.First()+"\n";

                    if (UploadFileName == "") UploadFileName = "User Successfuly loaded.";
                    ViewBag.ErrorMessage = UploadFileName;
                }
            }
            else if (cbxRequestType == "Mass")
            {
                string fname;

                // Checking for Internet Explorer  
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = Upload.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = Upload.FileName;
                }

                // Get the complete folder path and store the file inside it.  
                fname = Path.Combine(Server.MapPath("~/App_Data/Uploads/"), fname);
                Upload.SaveAs(fname);

                ExcelAPI API = new ExcelAPI();
                API.CreateDataTableFromExcel(fname, dtUsers);

                foreach(DataRow dr in dtUsers.Rows) {
                    model.UserName = dr["Name"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.Roles = dr["Roles"].ToString();
                    model.Password = model.PasswordName;
                    model.ConfirmPassword = model.ConfirmPasswordName;

                    // Attempt to register the user
                    var user = new ApplicationUser { UserName = dr["Name"].ToString(),
                                                     Email = dr["Email"].ToString(),
                                                     CompanyName = model.CompanyName };
                    var result = await UserManager.CreateAsync(user, model.PasswordName);
                    if (result.Succeeded)
                    {
                        Models.DBContext dbContext = new Models.DBContext();
                        Models.UserRoles usr = new Models.UserRoles();

                        usr.UserId = model.UserName;
                        usr.Role = model.Roles.Replace("Planner", "Player");
                        usr.CompanyName = model.CompanyName;
                        usr.Email = model.Email;

                        dbContext.UserRoles.Add(usr);
                        dbContext.SaveChanges();

                        try
                        {
                            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            string EMail = ConfigurationManager.AppSettings.Get("EmailID");
                            string To = model.Email, UserID, Password, SMTPPort, Host;

                            var request = HttpContext.Request;
                            string lnkHref = "<a href='" + HttpContext.Request.Url.Scheme + "://" +
                                       HttpContext.Request.Url.Authority +
                                        "'> " + "link" + "</a>";

                            //HTML Template for Send email 
                            string subject = "Login Created";
                            string body = "<b>Dear " + model.UserName + ", </b><br/><br/>";
                            body = body + "<b>Your login credentials created, Kindly change the pasword at the earliest" + lnkHref + "</b><br/>";
                            body = body + "<b>User Name :  " + model.UserName + "</b><br/>"; ;
                            body = body + "<b>Password :  " + model.Password + "</b><br/>"; ;
                            body = body + "<br/><br/><b>Thanks,</b><br/>" + "<b>" + model.CompanyName + "Team.</b>";

                            //Get and set the AppSettings using configuration manager.  
                            EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);

                            //Call send email methods.  
                            EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
                        }
                        catch (Exception ex)
                        {
                            UploadFileName += model.UserName + " email not sent to this user("+ex.Message+")\n";
                        }
                    }
                    else UploadFileName += result.Errors.First() +"\n";
                }
                if (UploadFileName == "") UploadFileName = "Users Successfuly loaded.";
                ViewBag.ErrorMessage = UploadFileName;
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/UploadLogo
        public ActionResult UploadLogo()
        {

            Models.DBContext dbContext = new Models.DBContext();
            Models.CompanyDetails cmp = new Models.CompanyDetails();
            var FindCompany = (from d in dbContext.Company
                               where d.UserName == User.Identity.Name
                               select new { d.CompanyName }).ToList();

            ViewBag.CompanyName = FindCompany[0].CompanyName;

            return View();
        }

        [HttpPost]
        // POST: /Account/UploadLogo
        public ActionResult UploadLogo(HttpPostedFileBase file)
        {
            Models.DBContext dbContext = new Models.DBContext();
            Models.CompanyDetails cmp = new Models.CompanyDetails();
            var FindCompany = dbContext.Company.Where(C => C.UserName == User.Identity.Name).FirstOrDefault();
            if (file != null)
            {

                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Content/assets/CompanyLogo/"), pic);
                // file is uploaded
                file.SaveAs(path);

                if (FindCompany != null)
                {
                    FindCompany.Logo = pic;
                    dbContext.SaveChanges();
                }
                ViewBag.UploadSuccess = "Logo Uploaded Successfully, Please click on the link to dashboard list";

            }

            return View();
        }

        //
        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ViewBag.ErrorMessage = result.Errors.First();
                return View(model);

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("Code", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null && !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                else
                {
                    string code = UserManager.GeneratePasswordResetToken(user.Id);
                    string To = model.Email, UserID, Password, SMTPPort, Host;

                    //Create URL with above token  
                    //  var lnkHref = "<a href='" + Url.Action("ResetPassword", "Account", new {code = user.UserName }, "http") + "'>Reset Password</a>";
                    var request = HttpContext.Request;
                    string lnkHref = "<a href='" + HttpContext.Request.Url.Scheme + "://" +
                               HttpContext.Request.Url.Authority +
                               HttpContext.Request.Url.Segments[0] +
                               HttpContext.Request.Url.Segments[1] +
                               "ResetPassword?code=" + user.UserName + "'>Reset Password</a>";



                    //HTML Template for Send email 
                    string subject = "Your changed password";
                    string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;
                    //Get and set the AppSettings using configuration manager.  
                    EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);
                    //Call send email methods.  
                    EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);

                    return View("ForgotPasswordConfirmation");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "No user found");
                return View(model);
            }
            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Login");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        public ActionResult SetAdminRole()
        {
            ApplicationUser UserData = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            SqlConnection conn = new SqlConnection();
            string sqlConn = WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            conn.ConnectionString = sqlConn;
            //string sqlQuery = "select * from ASPNetUsers where UserName != '" + User.Identity.Name + "' AND CompanyName='"+ UserData.CompanyName+"'";
            string sqlQuery = "SELECT b.[UserName],b.[Email]  " +
                                     ",b.[CompanyName] " +
                                     ",ISNULL(a.CompanyName, '') as CompanyAdmin " +
                               " FROM [DevExpDash].[dbo].[AspNetUsers] b " +
                                     " LEFT JOIN[DevExpDash].[dbo].[CompanyDetails] a on  a.UserName=b.UserName and a.CompanyName=b.CompanyName " +
                               " WHERE b.UserName != '" + User.Identity.Name + "' AND b.CompanyName = '" + UserData.CompanyName + "'";
            SqlCommand cmd = new SqlCommand(sqlQuery, conn);
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            List<UserListDetails> list = new List<UserListDetails>();


            while (dr.Read())
            {
                list.Add(new UserListDetails()
                {
                    UserName = dr["UserName"].ToString(),
                    CompanyName = dr["CompanyName"].ToString(),
                    Role = dr["CompanyAdmin"].ToString()
                });
            }
            dr.Close();
            conn.Close();

            return View(list);
        }

        [HttpPost]
        public ActionResult SetAdminRole(string sType, string sUser)
        {

            ApplicationUser user = UserManager.FindByName<ApplicationUser, string>(sUser);
            Models.DBContext dbContext = new Models.DBContext();
            Models.CompanyDetails cmp = new Models.CompanyDetails();
            var FindCompany = (from d in dbContext.Company
                               where d.CompanyName == user.CompanyName
                               select new { d.CompanyName, d.Logo }).FirstOrDefault();
            if (sType == "Y")
            {
                cmp.CompanyName = user.CompanyName;
                cmp.EmailID = user.Email;
                cmp.UserName = user.UserName;
                cmp.Logo = FindCompany.Logo;
                dbContext.Company.Add(cmp);
                dbContext.SaveChanges();
            }
            else
            {
                var userdata = dbContext.Company.Where(D => D.UserName == sUser).FirstOrDefault();
                dbContext.Company.Remove(userdata);
                dbContext.SaveChanges();

            }

            return View();
        }


        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                if (returnUrl.Trim()=="/")
                    return Redirect("~/");
                else
                    return Redirect(returnUrl);
            }
            return Redirect("~/");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}