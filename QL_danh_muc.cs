using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV_sach_so
{
    public partial class QL_danh_muc : Form
    {
        public QL_danh_muc()
        {
            InitializeComponent();
        }
        private void LoadData()
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection()) // 🔹 Sử dụng Connection.cs
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM sach"; // Đổi thành tên bảng của bạn
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    grvdata.AutoGenerateColumns = true;
                    grvdata.DataSource = dt;  // Chỉ gán DataSource mà không cần dấu ()
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO sach (ten_sach, tac_gia, nam_xuat_ban, tinh_trang_sach) VALUES (@tensach, @tacgia, @namxuatban, @tinhtrangsach)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@tensach", txtTenSach.Text);
                    cmd.Parameters.AddWithValue("@tacgia", txtTenTacGia.Text);
                    cmd.Parameters.AddWithValue("@namxuatban", txtNamXuatBan.Text);
                    cmd.Parameters.AddWithValue("@tinhtrangsach", cbbTrangThai.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm sách thành công!");

                    LoadData(); // 🔹 Cập nhật lại DataGridView ngay sau khi thêm
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm sách: " + ex.Message);
                }
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE sach SET ten_sach = @tensach, tac_gia = @tacgia, nam_xuat_ban = @namxuatban, tinh_trang_sach = @tinhtrangsach WHERE ma_sach = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", txtID.Text); // ID được lấy từ TextBox
                    cmd.Parameters.AddWithValue("@tensach", txtTenSach.Text);
                    cmd.Parameters.AddWithValue("@tacgia", txtTenTacGia.Text);
                    cmd.Parameters.AddWithValue("@namxuatban", txtNamXuatBan.Text);
                    cmd.Parameters.AddWithValue("@tinhtrangsach", cbbTrangThai.Text);
                    
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật sách thành công!");

                    LoadData(); // Cập nhật lại DataGridView sau khi sửa
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật sách: " + ex.Message);
                }
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM sach WHERE ma_sach = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", txtID.Text); // ID của tài khoản muốn xóa
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Xóa sách thành công!");

                    LoadData(); // Cập nhật lại DataGridView sau khi xóa
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa sách: " + ex.Message);
                }
            }
        }

        private void grvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Đảm bảo chỉ chọn dòng hợp lệ
            {
                DataGridViewRow row = grvdata.Rows[e.RowIndex];
                txtID.Text = row.Cells["ma_sach"].Value.ToString();
                txtTenSach.Text = row.Cells["ten_sach"].Value.ToString();
                txtTenTacGia.Text = row.Cells["tac_gia"].Value.ToString();
                txtNamXuatBan.Text = row.Cells["nam_xuat_ban"].Value.ToString();
                cbbTrangThai.Text = row.Cells["trang_thai"].Value.ToString();
                
            }
        }

        private void btnhienthi_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void llbtrangchu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form TrangChuAdmin = Application.OpenForms["TrangChuAdmin"];
            if (TrangChuAdmin==null)
            {
                TrangChuAdmin trangChuAdmin = new TrangChuAdmin();
                trangChuAdmin.ShowDialog();
            }
            else
            {
                TrangChuAdmin.Activate();
            }    
            
            this.Close();
        }

        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "PDF files|*.pdf|All files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string localPath = ofd.FileName;
                    string fileName = Path.GetFileName(localPath);
                    string ebooksFolder = Path.Combine(Application.StartupPath, "Resources", "Ebooks");

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(ebooksFolder))
                    {
                        Directory.CreateDirectory(ebooksFolder);
                    }
                    string newPath = Path.Combine(ebooksFolder, fileName);

                    try
                    {
                        File.Copy(localPath, newPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi copy file: " + ex.Message);
                        return;
                    }

                    // Debug: Hiển thị đường dẫn mới
                    MessageBox.Show("Đường dẫn file sau copy: " + newPath);

                    // Lưu đường dẫn (newPath) vào cột `duong_dan_file` trong DB
                    using (MySqlConnection conn = Connection.GetMySqlConnection())
                    {
                        try
                        {
                            conn.Open();
                            string query = "UPDATE sach SET duong_dan_file = @filePath WHERE ma_sach = @id";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@filePath", newPath);
                            cmd.Parameters.AddWithValue("@id", txtID.Text); // Đảm bảo txtID có giá trị đúng
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Tải file thành công và cập nhật CSDL thành công!");
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy bản ghi sách với mã: " + txtID.Text);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi cập nhật CSDL: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void btnDocSach_Click(object sender, EventArgs e)
        {
            // Lấy duong_dan_file từ DB
            string filePath = "";
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                conn.Open();
                string query = "SELECT duong_dan_file FROM sach WHERE ma_sach = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", txtID.Text);
                filePath = cmd.ExecuteScalar()?.ToString();
            }

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                // Mở file bằng trình xem PDF mặc định
                System.Diagnostics.Process.Start(filePath);
            }
            else
            {
                MessageBox.Show("File không tồn tại hoặc chưa được upload!");
            }
        }
    }
}
