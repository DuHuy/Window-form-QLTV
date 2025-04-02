using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLTV_sach_so
{
    public static class CurrentUser
    {
        public static string Username { get; set; }  // Lưu tên tài khoản
        public static string Role { get; set; }      // Lưu vai trò: Admin, Sinh viên, Giảng viên
    }
}
