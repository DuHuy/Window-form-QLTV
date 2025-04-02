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

namespace QLTV_sach_so
{
    public partial class Nhap_dat_truoc: Form
    {
        private int? maNguoiDung = null; // Nullable: Xác định thêm mới hay sửa
        private int? maSach = null;

        public event Action DataUpdated;
        public Nhap_dat_truoc()
        {
            InitializeComponent();
            this.Text = "Thêm mới đặt trước";

            btnthem.Visible = true; // Hiển thị nút "Thêm mới"
            btnsua.Visible = false; // Ẩn nút "Sửa"
        }
        public Nhap_dat_truoc(int mnd, int ms, string trangThai, string ngayDat)
        {
            InitializeComponent();
            this.Text = "Sửa đặt trước";

            maNguoiDung = mnd;
            maSach = ms;

            txtmand.Text = mnd.ToString();
            txtmasach.Text = ms.ToString();
            cbbtrangthai.SelectedItem = trangThai;
            dtpdattruoc.Value = DateTime.Parse(ngayDat);

            // Khóa chọn người dùng và sách khi sửa
            cbbmuser.Enabled = false;
            cbbmbook.Enabled = false;

            btnthem.Visible = false; // Ẩn nút "Thêm mới"
            btnsua.Visible = true; // Hiển thị nút "Sửa"
        }
        private void LoadData()
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();

                    // 🔹 Load danh sách người dùng
                    string queryUser = "SELECT ma_nguoi_dung, ten_tai_khoan FROM taikhoan";
                    MySqlDataAdapter userAdapter = new MySqlDataAdapter(queryUser, conn);
                    DataTable dtUser = new DataTable();
                    userAdapter.Fill(dtUser);
                    cbbmuser.DataSource = dtUser;
                    cbbmuser.DisplayMember = "ten_tai_khoan";
                    cbbmuser.ValueMember = "ma_nguoi_dung";

                    // 🔹 Load danh sách sách
                    string queryBook = "SELECT ma_sach, ten_sach FROM sach";
                    MySqlDataAdapter bookAdapter = new MySqlDataAdapter(queryBook, conn);
                    DataTable dtBook = new DataTable();
                    bookAdapter.Fill(dtBook);
                    cbbmbook.DataSource = dtBook;
                    cbbmbook.DisplayMember = "ten_sach";
                    cbbmbook.ValueMember = "ma_sach";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void Nhap_dat_truoc_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cbbmuser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbmuser.SelectedValue != null)
            {
                txtmand.Text = cbbmuser.SelectedValue.ToString();
            }
        }

        private void cbbmbook_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbmbook.SelectedValue != null)
            {
                txtmasach.Text = cbbmbook.SelectedValue.ToString();
            }    
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            try
            {
                int manguoidung, masach;
                if (!int.TryParse(txtmand.Text, out manguoidung) || !int.TryParse(txtmasach.Text, out masach))
                {
                    MessageBox.Show("Mã người dùng hoặc mã sách không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string ngaydat = dtpdattruoc.Value.ToString("yyyy-MM-dd");
                string trangthai = cbbtrangthai.SelectedItem?.ToString() ?? "Chưa chọn";

                using (MySqlConnection conn = Connection.GetMySqlConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO dattruoc (ma_nguoi_dung, ma_sach, ngay_dat_truoc, trang_thai_dat) " +
                                   "VALUES (@mnd, @ms, @nd, @tt)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mnd", manguoidung);
                        cmd.Parameters.AddWithValue("@ms", masach);
                        cmd.Parameters.AddWithValue("@nd", ngaydat);
                        cmd.Parameters.AddWithValue("@tt", trangthai);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DataUpdated?.Invoke(); // Cập nhật danh sách ở form chính
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            try
            {
                int manguoidung = Convert.ToInt32(txtmand.Text);
                int masach = Convert.ToInt32(txtmasach.Text);
                string ngaydat = dtpdattruoc.Value.ToString("yyyy-MM-dd");
                string trangthai = cbbtrangthai.SelectedItem?.ToString() ?? "Chưa chọn";

                using (MySqlConnection conn = Connection.GetMySqlConnection())
                {
                    conn.Open();
                    string query = "UPDATE dattruoc SET ngay_dat_truoc = @nd, trang_thai_dat = @tt " +
                                   "WHERE ma_nguoi_dung = @mnd AND ma_sach = @ms";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mnd", manguoidung);
                        cmd.Parameters.AddWithValue("@ms", masach);
                        cmd.Parameters.AddWithValue("@nd", ngaydat);
                        cmd.Parameters.AddWithValue("@tt", trangthai);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DataUpdated?.Invoke(); // Cập nhật danh sách ở form chính
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
