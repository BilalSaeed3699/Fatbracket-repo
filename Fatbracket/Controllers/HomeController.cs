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
    public class HomeController : Controller
    {

        private FatbracketEntities dbEntities = new FatbracketEntities();
        
        public ActionResult Index(int? isContact = 0)
        {
            if (isContact > 0)
            {
                ViewBag.isContact = isContact;
            }
            else
            {
                ViewBag.isContact = 0;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Contact(string email, string name, string subject, string message, string phone)
        {
            try
            {
                tblEmailSetting sa = dbEntities.tblEmailSettings.Find(1);

                using (MailMessage mm = new MailMessage(sa.Email, sa.CCEmail))
                {

                    mm.Subject = subject;
                    string body = "Hello,";
                    body += "<br /><br />You received a message from " + name + "(" + email + ")";
                    body += "<br /><br />Phone Number: " + phone + "<br /><br />";
                    body += message;
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
                using (MailMessage mm = new MailMessage(sa.Email, email))
                {
                    mm.Subject = "Thank you";
                    string body1 = "";
                    body1 += "Thank you for contacting us.";
                    body1 += "We will entertain your concern soon.";
                    body1 += "<br /><br />Yours,<br />The FatBracket Team";

                    string body = "";
                    body += "<body  style='background-color:white !important'>";
                    body += " <div>";
                    //body += "<h3>Hello " + sa.ReceiveName + ",</h3>";
                    body += " <table style='background-color: #f2f3f8; max-width:670px;' width='100%' border='0'  cellpadding='0' cellspacing='0'>";
                    body += " <tbody> <tr style='background-color:black;'><td style='padding: 0 35px; background-color:black;'><a><h1 style ='color:white' > FatBracket </h1>   </a></td> </tr>";
                    body += "<tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'> <b>Hello " + name + ",</b></td></tr><tr style='color:#455056; font-size:15px;line-height:35px;'><td style='padding:6px;text-align: center;'>" + body1 + "</td></tr>";
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

                return Json(new { status = 1 });
                //return RedirectToAction("Index", "Home", new { isContact = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { status = -1, data = ex.Message });
                //return RedirectToAction("Index", "Home", new { isContact = 2 });
                throw;
            }
            return Json(new { status = 1 });
        }
       
    }
}