using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeSpot.Model;
using System.IO;

namespace SafeSpot.Controllers
{
    public class agentController : Controller
      {
        db_SafeSpotEntities6 db = new db_SafeSpotEntities6();
        // GET: agent
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Agenthome()
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
                if (type == "Agent")
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
                return RedirectToAction("login");
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
            tb_agent tb = db.tb_agent.Where(ob => ob.username == getusername).FirstOrDefault();
            tb.agent_name = collection["aname"];
            tb.gmail = collection["mail"];
            tb.phone_no = collection["phone"];
            //db.tb_agent.Add(tb);
            int i = db.SaveChanges();
            if (i > 0)
            {
                Response.Write("<script>alert('Updated successfully')</script>");
                return RedirectToAction("profile", "agent");
            }
            else
            {
                Response.Write("<script>alert('Updation failed')</script>");
            }

            return View();
        }
        
        public ActionResult Viewslot()
        {
            string getusername = Session["username"].ToString();
            return View(db.tb_freeslot.Where(i => i.agentinfree_name == getusername).ToList());
        }
        public ActionResult Freeslots()
        {
            string getusername = Session["username"].ToString();            
            return View(db.tb_freeslot.Where(i => i.agentinfree_name == getusername).ToList());
        }

        // GET: new/Edit/5
        [HttpGet]
        public ActionResult Updatefreeslots(int id)
        {
            return View(db.tb_freeslot.Where(im => im.freeslot_id == id).FirstOrDefault());
        }

        // POST: new/Edit/5
        [HttpPost]
        public ActionResult Updatefreeslots(int id, FormCollection collection)
        {
            try
            {

                // id = Convert.ToInt32(collection["id"]);
                tb_freeslot getslots = db.tb_freeslot.Where(im => im.freeslot_id == id).FirstOrDefault();
                getslots.car_freeslot = Convert.ToInt32(collection["car"]);
                getslots.bike_freeslot = Convert.ToInt32(collection["bike"]);
                int i = db.SaveChanges();
                if (i > 0)
                {
                    ViewBag.s = "success";
                    return RedirectToAction("Freeslots");
                }
                else
                {
                    ViewBag.f = "failed";
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: common/Edit/5
        public ActionResult Changepassword()
        {
            return View();
        }

        // POST: common/Edit/5
        [HttpPost]
        public ActionResult Changepassword(FormCollection collection)
        {
            //tb_login lg = new tb_login();
            string user = Session["username"].ToString();
            string type = Session["type"].ToString();
            ViewBag.type = type;
            string paswd = Session["password"].ToString();
            string pass = collection["oldpass"];
            tb_log login = db.tb_log.Where(i => i.username == user && i.password == paswd).FirstOrDefault();
            tb_agent agent = db.tb_agent.Where(i => i.username == user && i.password == paswd).FirstOrDefault();
            if (login.password == pass && agent.password == pass)
            {
                login.password = collection["newpass"];
                agent.password = collection["newpass"];
                int i = db.SaveChanges();
                if (i > 0)
                {
                    ViewBag.msg = "Password updated Successfully";
                }
                else
                {
                    ViewBag.msg = "Something went wrong";
                }

            }
            else
            {
                ViewBag.msg = "Wrong Current password";
            }

            return View();
        }

        // GET: agent/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

              
        // POST: agent/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       


        // GET: agent/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: agent/Delete/5
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
