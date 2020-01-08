using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SafeSpot.Model;
using System.Data;
using System.IO;


namespace SafeSpot.Controllers
{
    public class userController : Controller
    {
        db_SafeSpotEntities6 db = new db_SafeSpotEntities6();
        // GET: user
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int id)
        {
            return View();
        } 
        public ActionResult Userhome()
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
                if (type == "User")
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
            tb.password = collection["password"];
            //db.tb_reg.Add(tb);
            int i = db.SaveChanges();
            if (i > 0)
            {
                Response.Write("<script>alert('Updated successfully')</script>");
                return RedirectToAction("profile", "user");
            }
            else
            {
                Response.Write("<script>alert('Updation failed')</script>");
            }

            return View();
        }
        
        //public JsonResult GetAllLocation()
        //{
        //    var data = db.tb_freeslot.ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet); 
        //}
        //public JsonResult GetAllLocationBYId(int id)
        //{
        //    var data = db.tb_freeslot.Where(x => x.freeslot_id == id).FirstOrDefault();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Find()
        {
            return View(db.tb_freeslot.ToList());

        }
        [HttpPost]
        public ActionResult Find(FormCollection collection)
        {
                          
            tb_findspace find = new tb_findspace();
             find.arrival_time = Convert.ToDateTime(collection["arrival-time"]);
            find.vehicle_type = collection["vehicle-type"];
            find.color = collection["color"];
            find.geolocation = collection["getgeo"]+","+ collection["getgeo1"].ToString();
            find.parkslot = "0";
            Session["geo"] = collection["getgeo"];
            Session["geo1"] = collection["getgeo1"];
            //var getgeo = collection["getgeo"];
            db.tb_findspace.Add(find);
            int i = db.SaveChanges();
            if (i > 0)
            {
               
                return RedirectToAction("Route", "user");
            }
            else
            {
                ViewBag.f = "failed";
                return View();
            }


        }
        public ActionResult Route(string getgeo)
        {

            string geo = Session["geo"].ToString();
            string geo1 = Session["geo1"].ToString();
            ViewBag.g = geo;
            ViewBag.h = geo1;
            return View();

        }
        [HttpGet]
        public ActionResult Bookparking()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Bookparking(FormCollection collection)
        {
            tb_booking adddetails = new tb_booking();
            adddetails.username = Session["username"].ToString();
            adddetails.vehicle_no = collection["no."];
            adddetails.vehicle_type = collection["type"];
            adddetails.slot = collection["slot"];
            adddetails.color = collection["color"];
            adddetails.ctime = DateTime.Now;
            adddetails.duration = Convert.ToDateTime(collection["wanttime"]);
           
            db.tb_booking.Add(adddetails);

            int id = adddetails.bk_id;
            int i = db.SaveChanges();
            if(i > 0)
            {
                //ViewBag.f = "SUCCESS";
                return RedirectToAction( "Viewparking", "user", new { id = id });
            }
            else
            {
                ViewBag.f = "failed";
            }
            return View();
        }
        public ActionResult Viewparking()
        {
            if (IsUserLoggedIn(Session, Request) == false)
            {
                return RedirectToAction("Login", "reg");
            }
            else 
            {
                String id = Session["username"]?.ToString();
                //or
                //string id = (string)Session["username"];
                if (id != null)
                {
                    ViewBag.userid = id;
                    var checkbooking = db.tb_booking.FirstOrDefault(s => s.username == id);
                    if (checkbooking != null)
                    {
                        return View();
                    }
                }
                return RedirectToAction("Bookparking", "user");
            }
        }
        
        [HttpGet]
        public ActionResult Feedback()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Feedback(FormCollection collection)
        {
            tb_feedback givefeedback = new tb_feedback();
            givefeedback.username = collection["username"];
            givefeedback.message = collection["message"];
            db.tb_feedback.Add(givefeedback);
            int i = db.SaveChanges();
            if (i > 0)
            {
                ViewBag.s = "Your response have been saved";
                return View();
            }
            else
            {
                ViewBag.f = "failed";
                return View();
            }
        }
        //public JsonResult GetAllLocation()
        //{
        //    var data = db.tb_freeslot.ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetAllLocationById(int id)
        //{
        //    var data = db.tb_freeslot.Where(x => x.freeslot_id == id).FirstOrDefault();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public ActionResult Payment()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Payment(FormCollection collection)
        {
            tb_payment makepayment = new tb_payment();
            makepayment.card_no = Convert.ToInt64(collection["cardno"]);
            makepayment.exp_date = collection["exp"];
            makepayment.cvv_code = collection["cvv"];
            makepayment.owner = collection["owner"];
            db.tb_payment.Add(makepayment);
            if (db.tb_carddetails.Any(x => x.c_no != makepayment.card_no))
            {
                ViewBag.f = "Enter valid card No";
            }
            else { 
            int i = db.SaveChanges();
            
                ViewBag.s = "success";
                return View();
            }
            //else
            //{

            //    ViewBag.f = "failed";
                return View();
            //}
        }
        // GET: user/Details/5
        //public ActionResult Payment()
        //{
        //    #region Paypal

        //    Random r = new Random();
        //    int ij = r.Next();

        //    PayPalIntegration.PayPalHelper objPayPal = new PayPalIntegration.PayPalHelper();
        //    objPayPal.Amount = Convert.ToDecimal(lblTotal.Text) / 60;
        //    objPayPal.PayPalBaseUrl = "https://www.sandbox.paypal.com/us/cgi-bin/webscr?";
        //    objPayPal.AccountEmail = "saniltr-facilitator@gmail.com";

        //    objPayPal.InvoiceNo = "DSTS /" + ij.ToString();
        //    for (int j = 0; j < GridView1.Rows.Count; j++)
        //    {
        //        objPayPal.ItemName += "Code : " + GridView1.Rows[j].Cells[0].Text;
        //    }

        //    objPayPal.SuccessUrl = "http://localhost:61732/insight/Result.aspx?Res=Success";
        //    objPayPal.CancelUrl = "http://localhost:61732/insight/Result.aspx?Res=Cancelled";

        //    try
        //    {
        //        Response.Redirect(objPayPal.GetSubmitUrl());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    #endregion
        //    //return View();
        //}

        // GET: user/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: user/Create
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

        // GET: user/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: user/Edit/5
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

        // GET: user/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: user/Delete/5
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
        //public ActionResult Find2()
        //{

        //    return View();
        //}
        //public JsonResult getallLoc()
        //{
        //    var data = db.tb_freeslot.ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Find1()
        {
            return View(db.tb_freeslot.ToList());
        }

    }
}
