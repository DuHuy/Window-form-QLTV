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
    public partial class QL_Mượn_Trả : Form
    {
        public QL_Mượn_Trả()
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
                    string query = @"
                SELECT mt.*, tk.ten_tai_khoan, s.ten_sach 
                FROM muontra mt
                JOIN taikhoan tk ON mt.ma_nguoi_dung = tk.ma_nguoi_dung
                JOIN sach s ON mt.ma_sach = s.ma_sach";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    grvdata.AutoGenerateColumns = true;
                    grvdata.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
                }
            }
        }
        private void LoadNguoiDung()
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ma_nguoi_dung, ten_tai_khoan FROM taikhoan"; // Đổi tên bảng nếu cần
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cbbmuser.DataSource = dt; // Đổ dữ liệu vào ComboBox
                    cbbmuser.DisplayMember = "ten_tai_khoan"; // Hiển thị tên người dùng
                    cbbmuser.ValueMember = "ma_nguoi_dung"; // Lưu giá trị ID ẩn
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách người dùng: " + ex.Message);
                }
            }
        }

        private void LoadSach()
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ma_sach, ten_sach FROM sach"; // Đổi tên bảng nếu cần
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cbbmbook.DataSource = dt;
                    cbbmbook.DisplayMember = "ten_sach"; // Hiển thị tên sách
                    cbbmbook.ValueMember = "ma_sach"; // Lưu giá trị ID ẩn
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách sách: " + ex.Message);
                }
            }
        }
        private void QL_Mượn_Trả_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadNguoiDung(); // Tải danh sách người dùng vào ComboBox
            LoadSach(); // Tải danh sách sách vào ComboBox
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            Mượn_trả frmMuonTra = new Mượn_trả();

            // 🔹 Khi form đóng, cập nhật lại danh sách
            frmMuonTra.DataUpdated += LoadData;

            frmMuonTra.ShowDialog();
        }

        private void grvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Đảm bảo chỉ chọn dòng hợp lệ
            {
                DataGridViewRow row = grvdata.Rows[e.RowIndex];

                cbbmbook.SelectedValue = row.Cells["ma_sach"].Value;
                cbbmuser.SelectedValue = row.Cells["ma_nguoi_dung"].Value;
                cbbtrangthai.Text = row.Cells["trang_thai_hien_tai"].Value.ToString();
            }
        }

        private void btnhienthi_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnloc_Click(object sender, EventArgs e)
        {
            string trangThai = cbbtrangthai.SelectedItem?.ToString(); // Lấy trạng thái từ ComboBox
            if (string.IsNullOrEmpty(trangThai))
            {
                MessageBox.Show("Vui lòng chọn trạng thái để lọc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT mt.ma_muon, tk.ten_tai_khoan AS 'Người mượn', s.ten_sach AS 'Tên sách', 
                       mt.ngay_muon, mt.ngay_tra, mt.trang_thai_hien_tai 
                FROM muontra mt
                JOIN taikhoan tk ON mt.ma_nguoi_dung = tk.ma_nguoi_dung
                JOIN sach s ON mt.ma_sach = s.ma_sach
                WHERE mt.trang_thai_hien_tai = @tt";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@tt", trangThai);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    grvdata.DataSource = dt; // Hiển thị kết quả lọc lên DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lọc dữ liệu: " + ex.Message);
                }
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            if (grvdata.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = grvdata.SelectedRows[0];
            int maMuon = Convert.ToInt32(row.Cells["ma_muon"].Value); // ID mượn

            int maNguoiDung = Convert.ToInt32(cbbmuser.SelectedValue);
            int maSach = Convert.ToInt32(cbbmbook.SelectedValue);
            string trangThai = cbbtrangthai.Text;

            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                UPDATE muontra 
                SET ma_nguoi_dung = @mnd, ma_sach = @ms, trang_thai_hien_tai = @tt
                WHERE ma_muon = @mt";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mand", maNguoiDung);
                        cmd.Parameters.AddWithValue("@ms", maSach);
                        cmd.Parameters.AddWithValue("@tt", trangThai);
                        cmd.Parameters.AddWithValue("@mm", maMuon);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cập nhật thành công!", "Thông báo");
                    LoadData(); // Cập nhật lại danh sách
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật: " + ex.Message);
                }
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (grvdata.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = grvdata.SelectedRows[0];
            int maMuon = Convert.ToInt32(row.Cells["ma_muon"].Value);

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No) return;

            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM muontra WHERE ma_muon = @mm";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mm", maMuon);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Xóa thành công!", "Thông báo");
                    LoadData(); // Cập nhật lại danh sách
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
            }
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


    }
}
