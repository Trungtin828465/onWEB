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
        public ActionResult Edit(Sanpham id, HttpPostedFileBase IMG)
        {
            if (ModelState.IsValid)
            {
                var sp = db.Sanphams.Find(id.SanphamID);
                if (sp == null)
                {
                    return HttpNotFound();
                }

              
                sp.TenSanpham = id.TenSanpham;
                sp.MoTa = id.MoTa;
                sp.Gia = id.Gia;
                sp.TrangThai = id.TrangThai;
                sp.NoiBat = id.NoiBat;
                sp.PhanLoaiSanPhamID = id.PhanLoaiSanPhamID;

                if (IMG != null && IMG.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(IMG.FileName);
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    if (fileExtension == ".jpg" || fileExtension == ".png")
                    {
                        string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                        IMG.SaveAs(filePath);
                        sp.AnhDaiDien = fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("IMG", "Chỉ hỗ trợ các định dạng hình ảnh .jpg và .png.");
                    }
                }

                db.Sanphams.AddOrUpdate(sp);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.category = new SelectList(db.PhanLoaiSanPhams.ToList(), "PhanLoaiSanPhamID", "TenPhanLoai");
            return View(id);
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
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(sp.TenSanpham)==true)
                {
                    ModelState.AddModelError("TenSanpham", "Họ tên không được để trống.");
                }
               

                else if ( !Regex.IsMatch(sp.TenSanpham, @"^[a-zA-Z]"))
                {
                    ModelState.AddModelError("TenSanpham", "Họ tên phải bắt đầu bằng một chữ cái.");
                }


                if (string.IsNullOrEmpty(sp.MoTa))
                {
                    ModelState.AddModelError("MoTa", "MoTa không được để trống.");
                }

                // Kiểm tra giá trị của Giá
                if (!decimal.TryParse(sp.Gia.ToString(), out decimal giaValue))
                {
                    ModelState.AddModelError("Gia", "Giá phải là một số hợp lệ.");
                }
                else if (sp.Gia < 0)
                {
                    ModelState.AddModelError("Gia", "Giá phải lớn hơn hoặc bằng 0.");
                }


                
               
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
                        ModelState.AddModelError("Img", "Chỉ hỗ trợ các định dạng hình ảnh .jpg và .png.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Img", "Vui lòng chọn một tệp hình ảnh.");
                }
                if (!ModelState.IsValid)
                {
                    // Đổ lại ViewBag.category khi có lỗi
                    ViewBag.category = new SelectList(db.PhanLoaiSanPhams.ToList(), "PhanLoaiSanPhamID", "TenPhanLoai");
                    return View(sp);
                }
                db.Sanphams.Add(sp);
                db.SaveChanges();
             
                return RedirectToAction("Index");

            }
            ViewBag.category = new SelectList(db.PhanLoaiSanPhams.ToList(), "PhanLoaiSanPhamID", "TenPhanLoai");

            return View(sp);
        }
     
    }
}