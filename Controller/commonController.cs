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
    public class regController : Controller
    {
        db_SafeSpotEntities6 db = new db_SafeSpotEntities6();
       
        // GET: reg
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }


        // GET: reg/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: reg/Create
        public ActionResult Register()
        {
            return View();
        }

        // POST: reg/Create
        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {

            // TODO: Add insert logic here
            tb_reg ins = new tb_reg();
            ins.name = collection["name"];
            ins.phone_no = collection["phone"];
            ins.gmail = collection["mail"];
            ins.username = collection["username"];
            ins.password = collection["password"];
            ins.confirmpassword = collection["confirmpassword"];
            //ins.usertype = collection["usertype"];
            ins.usertype = "User";
            if (db.tb_reg.Any(x => x.username == ins.username))
            {
                ViewBag.f = "Username Already Exist";
            }  
             
            else
            {
                tb_log inse = new tb_log();
                inse.username = collection["username"];
                inse.password = collection["password"];
                inse.usertype = "User";
                db.tb_reg.Add(ins);
                db.tb_log.Add(inse);
                db.SaveChanges();

                MailMessage MyMailMessage = new MailMessage();
                MyMailMessage.From = new MailAddress("ociuztest@gmail.com");
                MyMailMessage.To.Add(collection["mail"]);
                MyMailMessage.Subject = "Membership request";
                MyMailMessage.Body = "Your request has been approved..Your username is : " + collection["username"] + "Your password is: " + collection["password"];
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
                return RedirectToAction("Login", "reg");

            }


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

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                String user = collection["username"];
                String pass = collection["password"];
                var lgn = db.tb_log.Where(ob => (ob.username == user) && (ob.password == pass)).FirstOrDefault();
                if (lgn != null)
                {

                    if (lgn.usertype == ("Admin") )
                    {
                        Session["username"] = lgn.username;
                        Session["password"] = lgn.password;
                        Session["type"] = lgn.usertype;
                        string userid = lgn.username;
                        return RedirectToAction("Adminhome", "admin", new { userid = userid });
                    }
                    if (lgn.usertype == ("Agent") )
                    {
                        Session["username"] = lgn.username;
                        Session["password"] = lgn.password;
                        Session["type"] = lgn.usertype;
                        
                        string userid = lgn.username;
                        return RedirectToAction("Agenthome", "agent", new { userid = userid });
                    }
                    if (lgn.usertype == ("User"))
                    {
                        Session["username"] = lgn.username;
                        Session["password"] = lgn.password;
                        Session["type"] = lgn.usertype;
                        string userid = lgn.username;
                        return RedirectToAction("Userhome", "user", new { userid = userid });
                    }
                   
                }
                else
                {
                    Response.Write("<script>alert('username or password not match')</script>");
                }
            }
            return View();
        }

        //private string nametype(string username, string type)
        //{
        //    tb_log login = db.tb_log.Where(i => i.username == username && i.usertype == type).FirstOrDefault();
        //    return nametype(username, type);
        //}
        
        // GET: common/Edit/5
        [HttpGet]
        public ActionResult Forgotpassword1()
        {
            return View();
        }

        // POST: common/Edit/5
        [HttpPost]
        public ActionResult Forgotpassword1( FormCollection collection)
        {
                string username = collection["username"];
                string mail = collection["mail"];
                string type = collection["type"].ToString();
                     
                
                Random rand = new Random();
                var password = rand.Next().ToString();
                var getrandomkey = password.Substring(0, 5);
                var lgn = db.tb_log.Where(ob => (ob.username == username) && (ob.usertype == type)).FirstOrDefault();
                string userid = lgn.username;
                if (lgn != null)
                {

                if (lgn.usertype == ("User"))
                {
                    Session["username"] = lgn.username;
                    Session["type"] = lgn.usertype;
                    userid = lgn.username;
                    var useraccount = db.tb_reg.Where(i => i.gmail == mail && i.usertype == type && i.username == username).FirstOrDefault();
                    if (useraccount != null)
                    {
                         //nametype(username, type);
                        tb_log login = db.tb_log.Where(i => i.username == username && i.usertype == type).FirstOrDefault();
                        //}
                        if (login != null)
                        {

                            login.code = getrandomkey;
                            db.tb_log.Add(login);
                            int i = db.SaveChanges();
                            if (i > 0)
                            {
                                ViewBag.s = "Verification Code has been send to your registered mail id";
                            }
                            else
                            {
                                ViewBag.f = "Something went wrong";
                            }
                        }
                        else
                        {
                            ViewBag.f = "Username or type not match";
                        }
                    }
                    else
                    {
                        ViewBag.f = "Username or Mail not match";
                    }
                }
                
                else if (lgn.usertype == ("Agent"))

                {
                    Session["username"] = lgn.username;
                    Session["type"] = lgn.usertype;
                    userid = lgn.username;

                    var agentaccount = db.tb_agent.Where(i => i.gmail == mail ).FirstOrDefault();
                    if (agentaccount != null)
                    {
                        tb_log login = db.tb_log.Where(i => i.username == username && i.usertype == type).FirstOrDefault();
                        if (login != null)
                        {

                            login.code = getrandomkey;
                            db.tb_log.Add(login);
                            int j = db.SaveChanges();
                            if (j > 0)
                            {
                                ViewBag.s = "Verification Code has been send to your registered mail id";
                            }
                            else
                            {
                                ViewBag.f = "Something went wrong";
                            }
                        }
                    }
                }
            }
                                                                                               
                        MailMessage MyMailMessage = new MailMessage();
                        MyMailMessage.From = new MailAddress("ociuztest@gmail.com");
                        MyMailMessage.To.Add(collection["mail"]);
                        MyMailMessage.Subject = "Membership request";
                        MyMailMessage.Body = "Your request has been approved..Your Verification Code is:" + getrandomkey;
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
                        return RedirectToAction("Changeforgotpassword", "reg", new { userid = userid });

                 
            //return View();
        }
        [HttpGet]
        public ActionResult Changeforgotpassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Changeforgotpassword(FormCollection collection)
        {
            string newpassword = collection["newpass"];
            string verify = collection["verifypass"];
            string user = Session["username"].ToString();
            string type = Session["type"].ToString();
            tb_log login = db.tb_log.Where(i => i.username == user && i.code == verify).FirstOrDefault();
            if (login.usertype == "User")
            {
                tb_reg userd = db.tb_reg.Where(i => i.username == user && i.usertype == type).FirstOrDefault();
                if (login.code == verify && userd.username == user)
                {
                    login.password = collection["newpass"];
                    userd.password = collection["newpass"];
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
            }
            else if (login.usertype == "Agent")
            {
                tb_agent agentc = db.tb_agent.Where(i => i.username == user).FirstOrDefault();
                if (login.code == verify && agentc.username == user)
                {
                    login.password = collection["newpass"];
                    agentc.password = collection["newpass"];
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
            }
            return View();
        }
                            

        // GET: reg/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: reg/Delete/5
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
        public ActionResult Logout()
        {
            Logoutmethod();
            return View();//return RedirectToAction("login","settings")
        }
        private void Logoutmethod()
        {
            Session.Contents.Clear();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            try
            {
                HttpCookie CookieLogin = (HttpCookie)Request.Cookies["CookieLogin"];
                CookieLogin.Values.Clear();
                CookieLogin.Value = null;
                CookieLogin.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(CookieLogin);

            }
            catch
            {

            }

        }
    }
}


