using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.EnterpriseServices.CompensatingResourceManager;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using THI_WEB.Models;

namespace THI_WEB.Controllers
{
    public class HomeController : Controller

    {
        QuanLyBanQuanAoEntities db = new QuanLyBanQuanAoEntities();
        [HttpPost]
        public ActionResult XuHuong(string imageName)
        {
            Session["ImageName"] = imageName;
            return Json(new { imageName = imageName });
        }
        public ActionResult Index()
        {
            QuanLyBanQuanAoEntities db = new QuanLyBanQuanAoEntities();
            string tenLoaiSanPham = Session["ImageName"] as string;
            var phanLoaiSanPhams = db.PhanLoaiSanPhams.ToList();
            var sanPham = db.Sanphams.ToList();
            if (!string.IsNullOrEmpty(tenLoaiSanPham))
            {
                var checkLoc = db.Sanphams.Where(p => p.PhanLoaiSanPham.TenPhanLoai == tenLoaiSanPham).ToList();
                ViewBag.checkLoc = checkLoc;
            }
            ViewBag.sanPham = sanPham;
            return View(phanLoaiSanPhams);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
     
        public ActionResult Chitiet(int id)
        {
            QuanLyBanQuanAoEntities db = new QuanLyBanQuanAoEntities();
         
            var sp = db.Sanphams.Where(oi=> oi.SanphamID== id)
                .FirstOrDefault();
            ViewBag.sp = sp;
            return View(sp);
        }
        public ActionResult Edit(int id)
        {
            QuanLyBanQuanAoEntities db = new QuanLyBanQuanAoEntities();

            var sp = db.Sanphams.Where(oi => oi.SanphamID == id)
                .FirstOrDefault();
          
            ViewBag.category = new SelectList(db.PhanLoaiSanPhams.ToList(), "PhanLoaiSanPhamID", "TenPhanLoai");
            return View(sp);
        }
        [HttpPost]
      
        public ActionResult Edit(Sanpham sp, HttpPostedFileBase Img)
        {
            QuanLyBanQuanAoEntities db = new QuanLyBanQuanAoEntities();
            if (Img != null && Img.ContentLength > 0)
            {
                string fileName = Path.GetFileName(Img.FileName);
                string fileExtension = Path.GetExtension(fileName).ToLower();

                if (fileExtension == ".jpg" || fileExtension == ".png")
                {
                    string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                    Img.SaveAs(filePath);
                    sp.AnhDaiDien = fileName;
                }
                else
                {
                    ModelState.AddModelError("AnhDaiDien", "Chỉ chấp nhận file .jpg hoặc .png.");
                }
            }
            if (ModelState.IsValid)
            {
                db.Sanphams.AddOrUpdate(sp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.category = new SelectList(db.PhanLoaiSanPhams.ToList(), "PhanLoaiSanPhamID", "TenPhanLoai", sp.PhanLoaiSanPhamID);
            return View(sp);
        }


        public ActionResult Create()
        {
            QuanLyBanQuanAoEntities db = new QuanLyBanQuanAoEntities();
            var LoaiSp = db.PhanLoaiSanPhams.ToList();
            ViewBag.category = new SelectList(db.PhanLoaiSanPhams.ToList(), "PhanLoaiSanPhamID", "TenPhanLoai");
            return View();
        }
        [HttpPost]
        public ActionResult Create(Sanpham sp, HttpPostedFileBase Img)
        {
            if (ModelState.IsValid)
            {
                if (Img != null && Img.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(Img.FileName);
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".png")
                    {
                        string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                        Img.SaveAs(filePath);
                        sp.AnhDaiDien = fileName;
                    }
                }
               
                if (ModelState.IsValid)
                {
                    db.Sanphams.Add(sp);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

            }
            ViewBag.category = new SelectList(db.PhanLoaiSanPhams.ToList(), "PhanLoaiSanPhamID", "TenPhanLoai");

            return View(sp);
        }
     
    }
}