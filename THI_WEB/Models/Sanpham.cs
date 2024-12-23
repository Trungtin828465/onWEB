﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace THI_WEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Sanpham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sanpham()
        {
            this.ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            this.MauSacs = new HashSet<MauSac>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SanphamID { get; set; }
        [DisplayName("ten san pham")]
        [Required(ErrorMessage ="Tên sp không đc trống")]
        [RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Tên sản phẩm phải bắt đầu bằng chữ cái")]

        public string TenSanpham { get; set; }
        [DisplayName("Mo ta")]
        [Required(ErrorMessage = "Mota sp không đc trống")]
        public string MoTa { get; set; }
        [DisplayName("Giá sản phẩm")]
        [Required(ErrorMessage = "Giá không được để trống")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Giá chỉ được chứa số và tối đa 2 chữ số thập phân")]
        public decimal Gia { get; set; }
        public Nullable<byte> TrangThai { get; set; }
        [DisplayName("Anh sản phẩm")]
        [Required(ErrorMessage = "Anh không được để trống")]
        public string AnhDaiDien { get; set; }
        public Nullable<byte> NoiBat { get; set; }
        public Nullable<int> PhanLoaiSanPhamID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual PhanLoaiSanPham PhanLoaiSanPham { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MauSac> MauSacs { get; set; }
    }
}
