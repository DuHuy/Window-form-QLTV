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
    public partial class QLThongTinTaiKhoan : Form
    {
        public QLThongTinTaiKhoan()
        {
            InitializeComponent();
        }
        private void LoadData()
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM taikhoan";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    grvnguoidung.DataSource = dt;

                    // 🔹 Căn chỉnh tự động
                    grvnguoidung.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    grvnguoidung.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
                }
            }
        }
        private void grvnguoidung_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Đảm bảo chỉ chọn dòng hợp lệ
            {
                DataGridViewRow row = grvnguoidung.Rows[e.RowIndex];
                txtma.Text = row.Cells["ma_nguoi_dung"].Value.ToString();
                txtTen.Text = row.Cells["ten_tai_khoan"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                txtSoDienThoai.Text = row.Cells["dien_thoai"].Value.ToString();
                cbbVaiTro.Text = row.Cells["vai_tro"].Value.ToString();
            }
        }

        private void btnthemtk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text) ||
       string.IsNullOrWhiteSpace(txtEmail.Text) ||
       string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // 🔹 Dừng thực hiện nếu có ô trống
            }

            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO taikhoan (ten_tai_khoan, email, dien_thoai, vai_tro) VALUES (@ten, @email, @sdt, @vaitro)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ten", txtTen.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@sdt", txtSoDienThoai.Text);
                    cmd.Parameters.AddWithValue("@vaitro", cbbVaiTro.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm tài khoản thành công!");

                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm tài khoản: " + ex.Message);
                }
            }
        }

        private void btnsuatk_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE taikhoan SET ten_tai_khoan = @ten, email = @email, dien_thoai = @sdt, vai_tro = @vaitro WHERE ma_nguoi_dung = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", txtma.Text); // ID được lấy từ TextBox
                    cmd.Parameters.AddWithValue("@ten", txtTen.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@sdt", txtSoDienThoai.Text);
                    cmd.Parameters.AddWithValue("@vaitro", cbbVaiTro.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật tài khoản thành công!");

                    LoadData(); // Cập nhật lại DataGridView sau khi sửa
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật tài khoản: " + ex.Message);
                }
            }
        }

        private void btnxoatk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtma.Text))
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (MySqlConnection conn = Connection.GetMySqlConnection())
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM taikhoan WHERE ma_nguoi_dung = @id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", txtma.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Xóa tài khoản thành công!");

                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa tài khoản: " + ex.Message);
                    }
                }
            }
        }

        private void btncapnhattk_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void llbtrangchu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form TrangChuAdmin = Application.OpenForms["TrangChuAdmin"];
            if (TrangChuAdmin == null)
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

        private void QLThongTinTaiKhoan_Load(object sender, EventArgs e)
        {

            cbbVaiTro.Items.Clear();
            cbbVaiTro.Items.Add("student");
            cbbVaiTro.Items.Add("teacher");
            cbbVaiTro.Items.Add("admin");
            cbbVaiTro.SelectedIndex = 0;

            LoadData();

            // 🔹 Kiểm tra quyền của người dùng đăng nhập
            if (CurrentUser.Role != "admin")
            {
                btnthemtk.Enabled = false;
                btnsuatk.Enabled = false;
                btnxoatk.Enabled = false;
            }

        }

        private void Fill(object sender, DataGridViewAutoSizeColumnsModeEventArgs e)
        {

        }

        
    }
}
