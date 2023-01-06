using Fatbracket.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fatbracket.Controllers
{
    public class DashboardController : Controller
    {
        private FatbracketEntities dbEntities = new FatbracketEntities();

        public string Cpath1 { get; private set; }

        public ActionResult Home(int id=0)
        {
            
            tbluser user = dbEntities.tblusers.Find(id);
            ViewBag.message = "";

            return View(user);
        }
        [HttpPost]
        public ActionResult Home(tbluser tbluser , HttpPostedFileBase ImagePath)
        {
            tbluser user = dbEntities.tblusers.Find(tbluser.Playerid);
            string Textoptin = (Request["Textoptin"]);
            if (ImagePath != null)
            {
                var fileName = Path.GetFileName(ImagePath.FileName); 
                var ext = Path.GetExtension(ImagePath.FileName); 
                string name = Path.GetFileNameWithoutExtension(fileName);                                                                         // store the file inside ~/project folder(Img)  
                var path = Path.Combine(Server.MapPath("~/UploadImages"), fileName);
                Cpath1 = Path.Combine(("\\UploadImages"), fileName);
                user.Imagename = fileName;
                user.Imagepath = Cpath1;
                ImagePath.SaveAs(path);

            }

            if (Textoptin == null)
            {
                user.Textoptin = 0;
            }
            else
            {
                user.Textoptin = 1;
            }
                user.Email = tbluser.Email;
                user.Firstname = tbluser.Firstname;
                user.Lastname = tbluser.Lastname;
                user.Phone = tbluser.Phone;
                user.Password = tbluser.Password;
                user.Handle = tbluser.Handle;
                

                dbEntities.Entry(user).State=System.Data.Entity.EntityState.Modified;
                dbEntities.SaveChanges();

                HttpCookie cookie1 = new HttpCookie("User");
                cookie1["Email"] = user.Email;
                cookie1["UserID"] = user.Playerid.ToString();
                cookie1["UserType"] = user.Usertype.ToString();
                cookie1["UserName"] = user.Firstname + " " + user.Lastname;
                cookie1["Imagepath"] = user.Imagepath;

                cookie1.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(cookie1);
                ViewBag.message = "Successfully Record Updated.";
                return View(user);
        }
    }
}