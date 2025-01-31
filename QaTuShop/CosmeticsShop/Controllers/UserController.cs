﻿using CosmeticsShop.Extensions;
using CosmeticsShop.Models;
using DocumentFormat.OpenXml.EMMA;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CosmeticsShop.Controllers
{
    public class UserController : Controller
    {
        ShoppingEntities db = new ShoppingEntities();
        public bool CheckRole(string type)
        {
            Models.User user = Session["User"] as Models.User;
            if (user.UserType.Name == type)
            {
                return true;
            }
            return false;
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CheckoutOrder()
        {
            if (CheckRole("Khách Hàng"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            Models.User user = Session["User"] as Models.User;
            List<Order> orders = db.Orders.Where(x => x.UserID == user.ID).ToList();
            return View(orders);
        }
        public ActionResult OrderDetails(int ID)
        {
            if (CheckRole("Khách Hàng"))
            {

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            List<OrderDetail> orderDetails = db.OrderDetails.Where(x => x.OrderID.Value == ID).ToList();
            return View(orderDetails);
        }
        public ActionResult Complete(int ID)
        {
            Order order = db.Orders.Find(ID);
            order.Status = "Complete";
            order.DateShip = DateTime.Now;
            db.SaveChanges();

            // Cập nhật sản phẩm
            List<OrderDetail> orderDetails = db.OrderDetails.Where(x => x.OrderID.Value == ID).ToList();
            foreach (var item in orderDetails)
            {
                Product product = db.Products.Find(item.ProductID);
                product.Quantity -= item.Quantity;
                product.PurchasedCount += item.Quantity;
                db.SaveChanges();
            }
            return RedirectToAction("CheckoutOrder");
        }
        public ActionResult Infor()
        {
            Models.User user = Session["User"] as Models.User;
            return View(user);
        }
        [HttpPost]
        public ActionResult Infor(User user)
        {
            var u = db.Users.Find((Session["User"] as Models.User).ID);
            u.Name = user.Name;
            u.Address = user.Address;
            u.Email = user.Email;
            u.Phone = user.Phone;

            db.SaveChanges();
            ViewBag.Message = "Cập nhật thành công!";
            Session["User"] = u;
            return View(user);
        }
        //[HttpPost]
        //public ActionResult ChangePassword(User user, string OldPassword, string Password)
        //{
        //    var u = db.Users.Find((Session["User"] as Models.User).ID);
        //    if (u == null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    {

        //        var pass = user.Password;
        //        if (pass == HashMD5.ToMD5(OldPassword))
        //        {
        //            string passnew = (HashMD5.ToMD5(Password));
        //            u.Password = passnew;
        //            db.SaveChanges();
        //            ViewBag.Message("Đổi mật khẩu thành công");
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    ViewBag.Message("Thay đổi mật khẩu không thành công");
        //    return RedirectToAction("SignIn", "Home");
        //}
        [HttpPost]
        public ActionResult ChangePassword(string OldPassword, string NewPassword)
        {
            var u = db.Users.Find((Session["User"] as User).ID);
            if (u == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                // Kiểm tra mật khẩu hiện tại của người dùng
                if (u.Password == HashMD5.ToMD5(OldPassword))
                {
                    // Thực hiện thay đổi mật khẩu
                    u.Password = HashMD5.ToMD5(NewPassword);
                    db.SaveChanges();

                    ViewBag.Message = "Đổi mật khẩu thành công";
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Message = "Thay đổi mật khẩu không thành công";
            return RedirectToAction("SignIn", "Home");
        }
    }
}