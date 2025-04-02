using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLTV_sach_so
{
    internal class TaiKhoan
    {
        public int ID { get; set; }  // Thêm ID nếu CSDL có cột ID
        public string TenTaiKhoan { get; set; }
        public string Matkhau { get; set; }

        public TaiKhoan() { }

        public TaiKhoan(int id, string tenTaiKhoan, string matkhau)
        {
            this.ID = id;
            this.TenTaiKhoan = tenTaiKhoan;
            this.Matkhau = matkhau;
        }

    }
}
