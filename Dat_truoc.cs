using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV_sach_so
{
    public partial class Dat_truoc : Form
    {
        public event Action DataUpdated;
        public Dat_truoc()
        { 
        InitializeComponent();
            grvdata.CellValueChanged += grvdata_CellValueChanged;
            grvdata.CurrentCellDirtyStateChanged += grvdata_CurrentCellDirtyStateChanged;
        }
        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = Connection.GetMySqlConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM dattruoc";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    grvdata.DataSource = dt;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }


        private void btnthem_Click(object sender, EventArgs e)
        {
            Nhap_dat_truoc formThem = new Nhap_dat_truoc();
            if (formThem.ShowDialog() == DialogResult.OK)
            {
                LoadData(); // Cập nhật lại danh sách
            }
        }

        private void Dat_truoc_Load(object sender, EventArgs e)
        {
            LoadData();
            AddStatusComboBox();
        }
        private void AddStatusComboBox()
        {
            if (!grvdata.Columns.Contains("trang_thai_dat"))
                return;

            // Tìm index của cột trạng thái
            int colIndex = grvdata.Columns["trang_thai_dat"].Index;

            // Nếu đã có cột ComboBox thì không cần tạo lại
            if (grvdata.Columns[colIndex] is DataGridViewComboBoxColumn)
                return;

            // Tạo ComboBoxColumn
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
            comboBoxColumn.HeaderText = "Trạng thái";
            comboBoxColumn.Name = "trang_thai_dat";
            comboBoxColumn.DataPropertyName = "trang_thai_dat";
            comboBoxColumn.Items.AddRange("đang chờ", "hoàn thành", "đã hủy");

            // Xóa cột cũ và chèn cột mới
            grvdata.Columns.RemoveAt(colIndex);
            grvdata.Columns.Insert(colIndex, comboBoxColumn);
        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM dattruoc WHERE 1=1";
                    
                    // Kiểm tra nếu người dùng chọn trạng thái
                    if (!string.IsNullOrEmpty(cbbchontrangthai.Text))
                    {
                        query += " AND trang_thai_dat = @trangthai";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(cbbchontrangthai.Text))
                        cmd.Parameters.AddWithValue("@trangthai", cbbchontrangthai.Text);

                    // Đổ dữ liệu vào DataTable
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Hiển thị dữ liệu lên DataGridView
                    grvdata.DataSource = dt;

                    // Nếu không có dữ liệu, hiển thị thông báo
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy kết quả phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (grvdata.SelectedRows.Count > 0)
            {
                int maNguoiDung = Convert.ToInt32(grvdata.SelectedRows[0].Cells["ma_nguoi_dung"].Value);

                using (MySqlConnection conn = Connection.GetMySqlConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM dattruoc WHERE ma_nguoi_dung = @mnd";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@mnd", maNguoiDung);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Xóa thành công!");
                LoadData(); // Cập nhật lại bảng sau khi xóa
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa!");
            }
        }

        private void bntsua_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có chọn dòng nào không
            if (grvdata.SelectedRows.Count > 0)
            {
                try
                {
                    // Lấy dữ liệu từ dòng đang chọn
                    DataGridViewRow row = grvdata.SelectedRows[0];

                    int maNguoiDung = Convert.ToInt32(row.Cells["ma_nguoi_dung"].Value);
                    int maSach = Convert.ToInt32(row.Cells["ma_sach"].Value);
                    string trangThai = row.Cells["trang_thai_dat"].Value?.ToString() ?? "";
                    string ngayDat = row.Cells["ngay_dat_truoc"].Value?.ToString() ?? DateTime.Now.ToString("yyyy-MM-dd");

                    // Mở form sửa
                    Nhap_dat_truoc formSua = new Nhap_dat_truoc(maNguoiDung, maSach, trangThai, ngayDat);
                    formSua.DataUpdated += LoadData; // Gọi lại danh sách sau khi sửa
                    formSua.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi mở form sửa: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void btnxacnhan_Click(object sender, EventArgs e)
        {
            if (grvdata.SelectedRows.Count > 0)
            {
                try
                {
                    int maDatTruoc = Convert.ToInt32(grvdata.SelectedRows[0].Cells["ma_dat_truoc"].Value);
                    string trangThaiMoi = "hoàn thành"; // Hoặc "đã hủy" tùy theo tình huống

                    using (MySqlConnection conn = Connection.GetMySqlConnection())
                    {
                        conn.Open();
                        string query = "UPDATE dattruoc SET trang_thai_dat = @trangThai WHERE ma_dat_truoc = @maDatTruoc";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@trangThai", trangThaiMoi);
                        cmd.Parameters.AddWithValue("@maDatTruoc", maDatTruoc);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cập nhật trạng thái thành công!");
                    LoadData(); // Cập nhật lại danh sách
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi cập nhật trạng thái: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một yêu cầu để xác nhận!");
            }
        }

        private void grvdata_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra index hợp lệ
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (grvdata.Columns[e.ColumnIndex].Name == "trang_thai_dat")
            {
                try
                {
                    int maDatTruoc = Convert.ToInt32(grvdata.Rows[e.RowIndex].Cells["ma_dat_truoc"].Value);
                    string trangThaiMoi = grvdata.Rows[e.RowIndex].Cells["trang_thai_dat"].Value?.ToString() ?? "";

                    using (MySqlConnection conn = Connection.GetMySqlConnection())
                    {
                        conn.Open();
                        string query = "UPDATE dattruoc SET trang_thai_dat = @trangThai WHERE ma_dat_truoc = @maDatTruoc";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@trangThai", trangThaiMoi);
                        cmd.Parameters.AddWithValue("@maDatTruoc", maDatTruoc);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cập nhật trạng thái thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi cập nhật trạng thái: " + ex.Message);
                }
            }
        }

        private void grvdata_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (grvdata.IsCurrentCellDirty)
            {
                grvdata.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
