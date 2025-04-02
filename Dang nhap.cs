using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QLTV_sach_so.QLThongTinTaiKhoan;

namespace QLTV_sach_so
{
    public partial class Dang_nhap : Form
    {
        public Dang_nhap()
        {
            InitializeComponent();
        }

        private void linkLabel_quenmatkhau_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Quên_Mật_Khẩu quenMatKhau = new Quên_Mật_Khẩu();
            quenMatKhau.ShowDialog();
        }

        
        Modify modify = new Modify();
        private void btndangnhap_Click(object sender, EventArgs e)
        {
            string tentk = textBox_tentaikhoan.Text;
            string matkhau = textBox_matkhau.Text;

            if (tentk.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!");
                return;
            }
            if (matkhau.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                return;
            }

            // Truy vấn kiểm tra tài khoản và lấy vai trò
            string query = "SELECT ten_tai_khoan, vai_tro FROM TaiKhoan WHERE ten_tai_khoan = @ten AND mat_khau = @mk";

            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ten", tentk);
                    cmd.Parameters.AddWithValue("@mk", matkhau);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) // Kiểm tra nếu có dữ liệu trả về
                    {
                        // Lưu thông tin vào CurrentUser
                        CurrentUser.Username = reader["ten_tai_khoan"].ToString();
                        CurrentUser.Role = reader["vai_tro"].ToString();

                        MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Mở trang chính sau khi đăng nhập
                        TrangChuAdmin trangChuAdmin = new TrangChuAdmin();
                        this.Hide();
                        trangChuAdmin.Show();
                    }
                    else
                    {
                        MessageBox.Show("Tên tài khoản hoặc mật khẩu không chính xác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                Modify modify = new Modify();
                List<TaiKhoan> list = modify.Taikhoans("SELECT * FROM library_management.taikhoan");

                if (list.Count > 0)
                {
                    MessageBox.Show("Kết nối CSDL thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    foreach (var tk in list)
                    {
                        MessageBox.Show($"User: {tk.TenTaiKhoan}, Password: {tk.Matkhau}");
                    }
                }
                else
                {
                    MessageBox.Show("Kết nối CSDL thành công nhưng không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}