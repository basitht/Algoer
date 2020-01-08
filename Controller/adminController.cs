using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeSpot.Model;
using System.IO;
using System.Net.Mail;

namespace SafeSpot.Controllers
{
    public class adminController : Controller
    {
        db_SafeSpotEntities6 db = new db_SafeSpotEntities6();
        // GET: admin
        public ActionResult Adminhome()
        {
            return View();
        }

        #region class defined to check user logged in or not
        public static Boolean IsUserLoggedIn(HttpSessionStateBase Session, HttpRequestBase Request)
        {
            if (Session.Count > 0)
            {
                if (Session["username"] != null)
                {
                    String user = Session["username"].ToString();
                    db_SafeSpotEntities6 db = new db_SafeSpotEntities6();
                    tb_log account = db.tb_log.Where(j => j.username == user).FirstOrDefault();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    HttpCookie CookieLogin = (HttpCookie)Request.Cookies["CookieLogin"];
                    if (CookieLogin != null)
                    {
                        if (CookieLogin.Values.Count > 0)
                        {
                            String user = CookieLogin.Values[0];
                            Session["username"] = user;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                catch { return false; }
            }
        }

        #endregion

        public ActionResult profile(String userid)
        {
            if (IsUserLoggedIn(Session, Request) == false)
            {
                return RedirectToAction("Login", "reg");
            }
            else
            {
                String id = Session["username"].ToString();
                ViewBag.userid = id;
                String type = Session["type"].ToString();
                if (type == "Admin")
                {
                    var tb = db.tb_reg.Where(t => t.username == id).FirstOrDefault();
                }

                return View();
            }
        }
        [HttpGet]
        public ActionResult Editprofile(String userid)
        {
            if (IsUserLoggedIn(Session, Request) == false)
            {
                return RedirectToAction("Login", "reg");
            }
            else
            {
                String id = Session["username"].ToString();
                ViewBag.userid = id;

                return View();

            }
        }
        [HttpPost]
        public ActionResult Editprofile(FormCollection collection)
        {
            String getusername = Session["username"].ToString();
            tb_reg tb = db.tb_reg.Where(ob => ob.username == getusername).FirstOrDefault();
            tb.name = collection["name"];
            tb.phone_no = collection["phone"];
            tb.gmail = collection["mail"];
            //db.tb_reg.Add(tb);
            int i = db.SaveChanges();
            if (i > 0)
            {
                Response.Write("<script>alert('Updated successfully')</script>");
                return RedirectToAction("profile", "reg");
            }
            else
            {
                Response.Write("<script>alert('Updation failed')</script>");
            }

            return View();
        }


        // GET: admin/Details/5
        public ActionResult Agentreg()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Agentreg(FormCollection collection)
        {
                Random rand = new Random();
                var password = rand.Next().ToString();
                var getrandomkey = "AGN" + password.Substring(0, 5);
                tb_agent Addagent = new tb_agent();
                Addagent.agent_name = collection["name"];
                Addagent.phone_no = collection["phone"];
                Addagent.gmail = collection["mail"];
                Addagent.username = collection["username"];
                Addagent.password = getrandomkey;
                if (db.tb_reg.Any(x => x.username == Addagent.username))
                {
                    ViewBag.f = "Username Already Exist";
                }
                else
                {
                    tb_log inse = new tb_log();
                    inse.username = collection["username"];
                    inse.password = getrandomkey;
                    inse.usertype = "Agent";
                    db.tb_agent.Add(Addagent);
                    db.tb_log.Add(inse);
                    db.SaveChanges();
                    ViewBag.s = "success";

                }

            MailMessage MyMailMessage = new MailMessage();

            MyMailMessage.From = new MailAddress("ociuztest@gmail.com");
            MyMailMessage.To.Add(collection["mail"]);
            MyMailMessage.Subject = "Membership request";
            MyMailMessage.Body = "Your request has been approved..Your username is : " + collection["username"] + "Your password is: " + getrandomkey;
            MyMailMessage.IsBodyHtml = true;
            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
            SMTPServer.Port = 587;
            SMTPServer.Credentials = new System.Net.NetworkCredential("ociuztest@gmail.com", "!123456789");
            SMTPServer.EnableSsl = true;
            try
            {
                SMTPServer.Send(MyMailMessage);
                //FormView1.DataBind();

                //Literal1.Text = "Verification Code Sent to the mail ";
                //Response.Redirect("Thankyou.aspx");
            }

            catch (Exception ex)
            {

                //Literal1.Text = "Sorry! pls check the entered email ";

            }

            return View();
        }

        public ActionResult Addslot()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Addslot(FormCollection collection)
        {
            tb_parkingslot addslot = new tb_parkingslot();
            addslot.slot_name = collection["slot"];
            db.tb_parkingslot.Add(addslot);
            int i = db.SaveChanges();
            if (i > 0)
            {
                ViewBag.s = "success";
                return View();
            }
            else
            {
                ViewBag.f = "failed";
                return View();
            }
        }

        // GET: admin/Create
        public ActionResult Addagentslot()
        {
            return View();
        }

        // POST: admin/Create
        [HttpPost]
        public ActionResult Addagentslot(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                
                    tb_freeslot addinfreeS = new tb_freeslot();
                    tb_agent addslot = new tb_agent();
                    addinfreeS.agentinfree_name =  collection["aname"].ToString();
                    addinfreeS.Lat_infree = collection["lat"];
                    addinfreeS.Long_infree = collection["long"];
                    addinfreeS.sllotinfree_name = collection["sname"].ToString();
                    //addslot.slot_name = collection["sname"];
                    //db.tb_agent.Add(addslot);
                    db.tb_freeslot.Add(addinfreeS);
                    int j = db.SaveChanges();
                    if (j > 0)
                    {
                        ViewBag.s = "success";
                        return View();
                    }
                    else
                    {
                        ViewBag.s = "failed";
                        return View();
                    }
                 }
            catch
            {
                return View();
            }
        }

       

        // GET: admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
