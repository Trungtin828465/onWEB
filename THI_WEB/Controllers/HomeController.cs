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
                var checkLoc = db.Sanphams.Where(p => p.PhanLoaiSanPham.TenPhanLoai == tenLoaiSanPham)

                  .ToList();
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
            var LoaiSp = db.PhanLoaiSanPhams.ToList();
            ViewBag.Loaisp = LoaiSp;
            return View(sp);
        }
        [HttpPost]
        public ActionResult Edit(Sanpham id, HttpPostedFileBase IMG)
        {
            // Tạo đối tượng db context
            using (var db = new QuanLyBanQuanAoEntities())  // Đảm bảo context được khởi tạo đúng
            {
                // Tìm sản phẩm theo SanphamID
                var sp = db.Sanphams.FirstOrDefault(oi => oi.SanphamID == id.SanphamID);

                if (sp == null)
                {
                    return HttpNotFound(); // Nếu không tìm thấy sản phẩm, trả về lỗi
                }

                // Kiểm tra nếu model không hợp lệ
                if (!ModelState.IsValid)
                {
                    return View(id);
                }

                // Xử lý hình ảnh (nếu có)
                if (IMG != null && IMG.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(IMG.FileName);
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    if (fileExtension == ".jpg" || fileExtension == ".png")
                    {
                        string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                        IMG.SaveAs(filePath);
                        sp.AnhDaiDien = fileName; // Cập nhật ảnh đại diện
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

                // Kiểm tra loại sản phẩm hợp lệ
                var LoaiSp = db.PhanLoaiSanPhams.Any(oi => oi.PhanLoaiSanPhamID == id.PhanLoaiSanPhamID);
                if (!LoaiSp)
                {
                    ModelState.AddModelError("SanphamID", "Loại sản phẩm không hợp lệ.");
                }

                // Cập nhật thông tin sản phẩm
                sp.MoTa = id.MoTa;
                sp.TenSanpham = id.TenSanpham;
                sp.PhanLoaiSanPhamID = id.PhanLoaiSanPhamID; // Cập nhật đúng ID loại sản phẩm
                sp.Gia = id.Gia;

                // Cập nhật sản phẩm trong cơ sở dữ liệu
                db.Entry(sp).State = EntityState.Modified; // Cập nhật đối tượng đã có
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                return RedirectToAction("Index"); // Điều hướng về trang Index sau khi lưu thành công
            }
        }


        public ActionResult Create()
        {
            QuanLyBanQuanAoEntities db = new QuanLyBanQuanAoEntities();
            var LoaiSp = db.PhanLoaiSanPhams.ToList();
            ViewBag.Loaisp = LoaiSp;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sanpham sp, HttpPostedFileBase Img)
        {
           
            if (ModelState.IsValid)
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(sp.TenSanpham))
                {
                    ModelState.AddModelError("TenSanpham", "Họ tên không được để trống.");
                }
                string check = @"^[a-zA-Z].*03.*$";
                if (!Regex.IsMatch(sp.TenSanpham, check))
                {
                    ModelState.AddModelError("TenSanpham", "Họ tên phải bắt đầu bằng một chữ cái và 03.");
                }

                if ( !Regex.IsMatch(sp.TenSanpham, @"^[a-zA-Z]"))
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


                
                var LoaiSp = db.PhanLoaiSanPhams.Any(oi=>oi.PhanLoaiSanPhamID==sp.SanphamID);
                if (!LoaiSp) { ModelState.AddModelError("SanphamID", "Loai sp khong dc trong"); }
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
                    return View(sp); // Nếu có lỗi, trả về view với thông tin đã nhập.
                }
                db.Sanphams.Add(sp);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(sp);
        }
    }
}