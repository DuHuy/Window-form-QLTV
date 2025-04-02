using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QLTV_sach_so
{
    public partial class Mượn_trả : Form
    {
        public int manguoidung { get; private set; }
        public int masach { get; private set; }
        public string trangthai { get; private set; }
        public string ngaymuon { get; private set; }
        public string ngaytra { get; private set; }
        public string dienthoai { get; private set; }

        public event Action DataUpdated;
        public Mượn_trả()
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
        private void Mượn_trả_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int manguoidung, masach;

                // Kiểm tra nhập liệu hợp lệ
                if (!int.TryParse(txtmand.Text, out manguoidung))
                {
                    MessageBox.Show("Mã người dùng không hợp lệ!");
                    return;
                }

                if (!int.TryParse(txtmasach.Text, out masach))
                {
                    MessageBox.Show("Mã sách không hợp lệ!");
                    return;
                }

                ngaymuon = datetmuon.Value.ToString("yyyy-MM-dd");
                ngaytra = datettra.Value.ToString("yyyy-MM-dd");
                trangthai = cbbtrangthai.SelectedItem?.ToString() ?? "Chưa chọn";
                dienthoai = txtdienthoai.Text.Trim();

                if (string.IsNullOrEmpty(dienthoai))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 🔹 Kết nối MySQL
                using (MySqlConnection conn = Connection.GetMySqlConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO muontra (ma_nguoi_dung, ma_sach, ngay_muon, ngay_tra, trang_thai_hien_tai, dien_thoai) " +
                                   "VALUES (@mnd, @ms, @nm, @nt, @tt, @dt)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mnd", manguoidung);
                        cmd.Parameters.AddWithValue("@ms", masach);
                        cmd.Parameters.AddWithValue("@nm", ngaymuon);
                        cmd.Parameters.AddWithValue("@nt", ngaytra);
                        cmd.Parameters.AddWithValue("@tt", trangthai);
                        cmd.Parameters.AddWithValue("@dt", dienthoai);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm mới thành công!", "Thông báo");

                // 🔹 Gọi sự kiện cập nhật danh sách (nếu có)
                DataUpdated?.Invoke();

                this.DialogResult = DialogResult.OK; // Trả về OK khi thành công
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm dữ liệu: " + ex.Message);
            }
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


    }
}
