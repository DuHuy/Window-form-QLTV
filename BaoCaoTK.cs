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
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace QLTV_sach_so
{
    public partial class BaoCaoTK : Form
    {
        public BaoCaoTK()
        {
            InitializeComponent();
            btnguilm.Click += new EventHandler(btnguilm_Click);
        }

        private DataTable LayThongKe(string loaiBaoCao, out string ketQuaTong)
        {
            ketQuaTong = "";
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "";
                    DataTable dt = new DataTable();

                    if (loaiBaoCao == "summary_stats") // Báo cáo tổng hợp
                    {
                        query = @"
                SELECT 
                    (SELECT COUNT(DISTINCT ma_nguoi_dung) FROM muontra WHERE trang_thai_hien_tai = 'đang mượn') AS tong_nguoi_muon,
                    (SELECT COUNT(DISTINCT ma_nguoi_dung) FROM muontra WHERE trang_thai_hien_tai = 'đã trả') AS tong_nguoi_tra,
                    (SELECT COUNT(DISTINCT ma_nguoi_dung) FROM muontra WHERE trang_thai_hien_tai = 'quá hạn') AS tong_nguoi_qua_han";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int muon = reader.GetInt32("tong_nguoi_muon");
                            int tra = reader.GetInt32("tong_nguoi_tra");
                            int quaHan = reader.GetInt32("tong_nguoi_qua_han");

                            ketQuaTong = $"Người đang mượn: {muon}, Đã trả: {tra}, Quá hạn: {quaHan}";
                        }
                        reader.Close();

                        query = @"
                SELECT 
                    ma_nguoi_dung, 
                    SUM(CASE WHEN trang_thai_hien_tai = 'đang mượn' THEN 1 ELSE 0 END) AS so_sach_dang_muon,
                    SUM(CASE WHEN trang_thai_hien_tai = 'đã trả' THEN 1 ELSE 0 END) AS so_sach_da_tra,
                    SUM(CASE WHEN trang_thai_hien_tai = 'quá hạn' THEN 1 ELSE 0 END) AS so_sach_qua_han
                FROM muontra 
                GROUP BY ma_nguoi_dung";
                    }
                    else if (loaiBaoCao == "borrow_stats") // Danh sách người đang mượn
                    {
                        query = "SELECT ma_nguoi_dung, ma_sach, trang_thai_hien_tai, ngay_muon FROM muontra WHERE trang_thai_hien_tai = 'đang mượn'";
                    }
                    else if (loaiBaoCao == "return_stats") // Danh sách người đã trả
                    {
                        query = "SELECT ma_nguoi_dung, ma_sach, trang_thai_hien_tai, ngay_tra FROM muontra WHERE trang_thai_hien_tai = 'đã trả'";
                    }
                    else if (loaiBaoCao == "overdue")
                    {
                        query = "SELECT ma_nguoi_dung, ma_sach, trang_thai_hien_tai, ngay_muon, ngay_tra FROM muontra WHERE trang_thai_hien_tai = 'quá hạn'";
                    }    

                    MySqlCommand cmd2 = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd2);
                    adapter.Fill(dt);

                    // Kiểm tra nếu không có dữ liệu
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không có dữ liệu để hiển thị!", "Thông báo");
                    }

                    return dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lấy dữ liệu thống kê: " + ex.Message);
                    return null;
                }
            }
        }

        private void btnxuatbaocao_Click(object sender, EventArgs e)
        {
            if (cbbtk.SelectedItem != null)
            {
                string loaiBaoCao = cbbtk.SelectedItem.ToString();

                // Gọi hàm thống kê (không truyền ngày vào)
                string ketQuaTong;
                DataTable dt = LayThongKe(loaiBaoCao, out ketQuaTong);

                // Hiển thị kết quả tổng lên TextBox
                txtthongke.Text = ketQuaTong;

                // Hiển thị danh sách chi tiết lên DataGridView
                if (dt != null)
                {
                    grvdata.DataSource = dt;
                    grvdata.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn loại báo cáo!");
            }
        }
        
        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (grvdata.SelectedRows.Count > 0)
            {
                int maBaoCao = Convert.ToInt32(grvdata.SelectedRows[0].Cells["ma_bao_cao"].Value);

                using (MySqlConnection conn = Connection.GetMySqlConnection())
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM baocao WHERE ma_bao_cao = @mabaocao";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@mabaocao", maBaoCao);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Xóa báo cáo thành công!");
                        LoadDanhSachBaoCao(); // Cập nhật danh sách
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa báo cáo: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một báo cáo để xóa!");
            }
        }
        private void LoadDanhSachBaoCao()
        {
            using (MySqlConnection conn = Connection.GetMySqlConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ma_bao_cao, loai_bao_cao, ngay_tao, noi_dung FROM baocao ORDER BY ngay_tao DESC";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    grvdata.DataSource = dt; // Cập nhật dữ liệu vào DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách báo cáo: " + ex.Message);
                }
            }
        }

        private void BaoCaoTK_Load(object sender, EventArgs e)
        {
            cbbtk.Items.Add("borrow_stats");  // Số người đang mượn sách
            cbbtk.Items.Add("return_stats");  // Số người đã trả sách
            cbbtk.Items.Add("overdue"); // số người quá hạn
            cbbtk.Items.Add("summary_stats"); // Thống kê tổng hợp
            cbbtk.SelectedIndex = 0; // Chọn mặc định
            LoadDanhSachBaoCao();
        }
        private static readonly HttpClient client = new HttpClient();

        public class ApiResponse
        {
            public string Id { get; set; }
            public string Object { get; set; }
            public long Created { get; set; }
            public string Model { get; set; }
            public List<Choice> Choices { get; set; }
            public string Text { get; set; }
            public string FinishReason { get; set; }
            public string Error { get; set; } // Để xử lý lỗi từ API
        }

        public class Choice
        {
            public string Text { get; set; }
            public int Index { get; set; }
            public object Logprobs { get; set; }
            public string FinishReason { get; set; }
        }
        private async Task<string> GenerateSummary(string inputText)
        {
            try
            {
                string url = "http://localhost:1234/v1/completions";
                var requestBody = new
                {
                    model = "vistral-7b-chat",
                    prompt = inputText,
                    max_tokens = 200,
                    temperature = 0.7
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    return "Lỗi: Yêu cầu API thất bại với trạng thái " + response.StatusCode + ". Phản hồi: " + errorResponse;
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Ghi lại phản hồi JSON thô để gỡ lỗi
                MessageBox.Show("Phản hồi API: " + jsonResponse, "Gỡ lỗi");

                var parsedResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                // Kiểm tra lỗi từ API
                if (!string.IsNullOrEmpty(parsedResponse.Error))
                {
                    return "Lỗi từ API: " + parsedResponse.Error;
                }

                // Kiểm tra 'choices' hoặc 'text'
                if (parsedResponse.Choices != null && parsedResponse.Choices.Count > 0)
                {
                    return parsedResponse.Choices[0].Text;
                }
                else if (!string.IsNullOrEmpty(parsedResponse.Text))
                {
                    return parsedResponse.Text;
                }
                else
                {
                    return "Lỗi: Phản hồi API không chứa 'choices' hoặc 'text'.";
                }

                //dynamic parsedResponse = JsonConvert.DeserializeObject(jsonResponse);
                //if (parsedResponse.choices != null && parsedResponse.choices.Count > 0)
                //{
                //    return parsedResponse.choices[0].text.ToString();
                //}
                //else
                //{
                //    return "Lỗi: Phản hồi API ko chứa 'choices'.";
                //}
            }
            catch (Exception ex)
            {
                return "Lỗi API: " + ex.Message;
            }
        }

        private bool isProcessing = false;
        private async void btnguilm_Click(object sender, EventArgs e)
        {
            if (isProcessing) return; // Nếu đang xử lý, bỏ qua các lần nhấn tiếp theo

            isProcessing = true;
            string thongKeText = txtthongke.Text.Trim(); // Lấy nội dung từ TextBox

            if (string.IsNullOrEmpty(thongKeText))
            {
                MessageBox.Show("Không có dữ liệu thống kê để tóm tắt!", "Thông báo");
                return;
            }

            // Vô hiệu hóa nút để tránh gửi nhiều lần
            btnguilm.Enabled = false;
            btnguilm.Text = "Đang gửi...";

            try
            {
                // Gọi API sinh tóm tắt
                string summary = await GenerateSummary(thongKeText);

                // Hiển thị tóm tắt trong RichTextBox
                rtbphanhoi.Text = summary;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi API: " + ex.Message, "Lỗi");
            }
            finally
            {
                // Khôi phục nút
                btnguilm.Enabled = true;
                btnguilm.Text = "Gửi LM";
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
