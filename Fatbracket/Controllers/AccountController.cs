using Fatbracket.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Fatbracket.Controllers
{
    public class AccountController : Controller
    {

        private FatbracketEntities dbEntities = new FatbracketEntities();


        // GET: Account
        public ActionResult Login()
        {
            try
            {

                return View();
            }
            catch
            {

            }
            finally
            {

            }
            return View();
        }

        public ActionResult Register()
        {
            try
            {

                return View();
            }
            catch
            {

            }
            finally
            {

            }
            return View();
        }

        public ActionResult Forgetpassword()
        {
            try
            {

                return View();
            }
            catch
            {

            }
            finally
            {

            }
            return View();
        }



        [HttpPost]
        public ActionResult registeruser(tbluser UserInformation)
        {

            try
            {
                UserInformation.Createddate = DateTime.Now;
                UserInformation.Isactive = 1;
                dbEntities.tblusers.Add(UserInformation);
                dbEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = -1, data = ex.Message });
            }

            //tbluser newuser = new tbluser();
            //newuser.Firstname = UserInformation.Firstname;
            //newuser.Lastname = UserInformation.Lastname;
            //newuser.Email = UserInformation.Email;
            //newuser.Phone = UserInformation.Phone;
            //newuser.Handle = UserInformation.Handle;
            //newuser.Textoptin = UserInformation.Textoptin;
            //newuser.Imagepath = UserInformation.Imagepath;
            //newuser.Imagename = UserInformation.Imagename;
            //newuser.Password = UserInformation.Password;



            //new changes cookie implementation
            HttpCookie UserEmail = Request.Cookies["UserEmail"];
            if (UserEmail == null)
            {
                UserEmail = new HttpCookie("UserEmail");
                UserEmail.Value = Convert.ToString(UserInformation.Email);
                UserEmail.Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                UserEmail.Value = Convert.ToString(UserInformation.Email);
            }
            Response.Cookies.Add(UserEmail);

            //return Json(new { status = 2 });
            return Json(new { status = 1, url = Url.Action("Home", "Dashboard") });
        }
        [HttpPost]
        public ActionResult verifyregistraton(tbluser UserInformation)
        {
            try
            {

                tbluser User = dbEntities.tblusers.Where(x => x.Email == UserInformation.Email).FirstOrDefault();

                if (User != null)
                {
                    return Json(new { status = 2 });
                }
                else
                {
                    return Json(new { status = 1 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = -1, data = ex.Message });
            }
            return Json(new { status = 1 });


        }


        public ActionResult Sendemail(tbluser UserInformation)
        {
            try
            {

                tbluser User = dbEntities.tblusers.Where(x => x.Email == UserInformation.Email).FirstOrDefault();

                if (User != null)
                {

                    string link = Request.Url.ToString();
                    link = link.Replace("Sendemail", "ResetPassword");

                    byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(User.Email);
                    string encrypted = Convert.ToBase64String(b);


                    tblEmailSetting sa = dbEntities.tblEmailSettings.Find(1);
                    using (MailMessage mm = new MailMessage(sa.Email, User.Email))
                    {

                        mm.Subject = "Password reset";

                        string body = "";
                        body += "<body  style='background-color:white !important'>";
                        body += " <div>";
                        body += " <table style='background-color: #f2f3f8; max-width:670px;' width='100%' border='0'  cellpadding='0' cellspacing='0'>";
                        body += " <tbody> <tr style='background-color:black;'><td style='padding: 0 35px; background-color:black;'><a><h1 style ='color:white' > FatBracket </h1>   </a></td> </tr>";
                        body += "<tr style='color:#455056; font-size:15px;text-align: center;'> <td style='text-align: center;'> <h3><b>Forgot your password?</b> </h3></td></tr>";
                        body += "<tr style='color:#455056; font-size:15px;text-align: center;'> <td style='text-align: center;'> <p>That's okay, it happens! Click on the button below to reset your password.</p> </td></tr>";
                        body += "<tr style='color:#455056; font-size:15px;text-align: center;'> <td style='text-align: center;'> <p>Please click the following link to reset your password </p> </td></tr>";
                        body += "<tr style='color:#455056; font-size:15px;text-align: center;'> <td style='text-align: center;'> <a style='background:#ff7117;border-color:#ff7117;text-decoration:none !important; font-weight:500; color:#fff;text-transform:uppercase;font-size:14px;padding:10px 24px;display:inline-block;' href = '" + link + "?Email=" + encrypted + "'>Reset Password</a> </td></tr>";
                        body += "<tr style='color:#455056; font-size:15px;text-align: center;'> <td style='text-align: center;'>Thanks </td></tr>";
                        body += "  </tbody></table>";
                        body += "</body>";
                        mm.Body = body;
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = sa.Host;
                        smtp.EnableSsl = Convert.ToBoolean(sa.SSLEnable);
                        NetworkCredential NetworkCred = new NetworkCredential(sa.Email, sa.Password);
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = Convert.ToInt32(sa.Port);
                        smtp.Send(mm);
                    }



                    return Json(new { status = 2 });
                }
                else
                {
                    return Json(new { status = 1 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = -1, data = ex.Message });
            }
            return Json(new { status = 1 });


        }
        [HttpPost]
        public ActionResult UserLogin(tbluser UserInformation)
        {
            var UserLoginReult = "";
            try
            {

                UserLoginReult = (from UserLoginStatus in dbEntities.Sp_User_Login(UserInformation.Email, UserInformation.Password)
                                  select UserLoginStatus.UserStatus).FirstOrDefault();

                if (UserLoginReult == "-1" || UserLoginReult == "-2" || UserLoginReult == "-3")
                {
                    return Json(new { status = UserLoginReult, UserName = UserInformation.Firstname + UserInformation.Lastname, url = Url.Action("Index", "Home") });
                }
                else if (UserLoginReult == "1")
                {
                    tbluser User = dbEntities.tblusers.Where(x => x.Email == UserInformation.Email).FirstOrDefault();


                    HttpCookie cookie1 = new HttpCookie("User");
                    cookie1["Email"] = User.Email;
                    cookie1["UserID"] = User.Playerid.ToString();
                    cookie1["UserType"] = User.Usertype.ToString();
                    cookie1["UserName"] = User.Firstname + " " + User.Lastname;
                    cookie1["Imagepath"] = User.Imagepath;
                   
                    cookie1.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(cookie1);


                    //HttpCookie UserEmail = Request.Cookies["UserEmail"];
                    //if (UserEmail == null)
                    //{
                    //    UserEmail = new HttpCookie("UserEmail");
                    //    UserEmail.Value = Convert.ToString(UserInformation.Email);
                    //    UserEmail.Expires = DateTime.Now.AddDays(30);
                    //}
                    //else
                    //{
                    //    UserEmail.Value = Convert.ToString(UserInformation.Email);
                    //}
                    //Response.Cookies.Add(UserEmail);

                    ////Session["UserEmail"] = UserInformation.Email;

                    
                    //HttpCookie UserName = Request.Cookies["UserName"];
                    //HttpCookie IsAdmin = Request.Cookies["IsAdmin"];
                    //HttpCookie UserId = Request.Cookies["UserId"];

                    //if (UserName == null)
                    //{
                    //    UserName = new HttpCookie("UserName");
                    //    UserName.Value = Convert.ToString(User.Firstname + User.Lastname);
                    //    UserName.Expires = DateTime.Now.AddDays(30);
                    //}
                    //else
                    //{
                    //    UserName.Value = Convert.ToString(User.Firstname + User.Lastname);
                    //}
                    //Response.Cookies.Add(UserName);

                    //if (IsAdmin == null)
                    //{
                    //    IsAdmin = new HttpCookie("IsAdmin");
                    //    IsAdmin.Value = Convert.ToString(User.Usertype);
                    //    IsAdmin.Expires = DateTime.Now.AddDays(30);
                    //}
                    //else
                    //{
                    //    IsAdmin.Value = Convert.ToString(User.Usertype);
                    //}
                    //Response.Cookies.Add(IsAdmin);

                    //if (UserId == null)
                    //{
                    //    UserId = new HttpCookie("UserId");
                    //    UserId.Value = Convert.ToString(User.Playerid);
                    //    UserId.Expires = DateTime.Now.AddDays(30);
                    //}
                    //else
                    //{
                    //    UserId.Value = Convert.ToString(User.Playerid);
                    //}
                    //Response.Cookies.Add(UserId);

                    //Session["UserName"] = User.Name;
                    //Session["IsAdmin"] = User.IsAdmin;
                    //Session["UserId"] = User.UserId;

                    //if (User.Usertype == 1)
                    //{
                    //    User.Playerid = 0;
                    //}
                    return Json(new { status = UserLoginReult, UserName = User.Firstname + User.Lastname, url = Url.Action("Home", "Dashboard", new { id = User.Playerid }) });

                }


            }
            catch (Exception ex)
            {
                return Json(new { status = -1, data = ex.Message });
            }

            return Json(new { status = UserLoginReult, UserName = UserInformation.Firstname + UserInformation.Lastname, url = Url.Action("Home", "Dashboard") });
        }


        public ActionResult ResetPassword(string Email)
        {
            try
            {
                byte[] b = Convert.FromBase64String(Email);
                string decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);

                ViewBag.Email = decrypted;
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string NewPassword, string ConfirmPassword, string Email)
        {
            string pass = NewPassword;
            try
            {


                byte[] EncDataBtye = null;
                tbluser Data = new tbluser();
                Data = dbEntities.tblusers.Select(r => r).Where(x => x.Email == Email).FirstOrDefault();
                if (Data != null)
                {

                    if (NewPassword != ConfirmPassword)
                    {
                        ViewBag.PError = "New Password and Confirm Password not Matched!!!";
                        return View();
                    }

                    Data.Password = pass;
                    dbEntities.Entry(Data);
                    dbEntities.SaveChanges();
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.Error = "Old password is not Correct!!!";
                    return View();
                }
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
            }
            return View();
        }

    }
}