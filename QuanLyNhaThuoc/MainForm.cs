using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuanLyTichDiem_TNK47_Buoi3;

namespace QuanLyNhaThuoc
{
    public partial class MainForm : Form
    {
        QuanLyKhachHang qlkh = new QuanLyKhachHang();
        KhachHang khachHangHienTai;

        List<KhoVouCher> khoVoucherToanHeThong = new List<KhoVouCher>();

       
        List<string> khachDaNhanQuaSinhNhat = new List<string>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            qlkh.DocTuFile("Data.txt");
            HienThiDanhSachKhachHang(qlkh.DsKhachHang);

            
            cbDungVoucher.SelectedIndexChanged += (s, ev) => TinhToanHoaDon();
        }

        public void HienThiDanhSachKhachHang(List<KhachHang> dsKhachHang)
        {
            lvDanhSach.Items.Clear();
            foreach (var kh in dsKhachHang)
            {
                ListViewItem item = new ListViewItem(kh.Sdt);
                item.SubItems.Add(kh.HoTen);
                item.SubItems.Add(kh.DiemTL.ToString());
                lvDanhSach.Items.Add(item);
            }
        }

        private void txtTimKH_TextChanged(object sender, EventArgs e)
        {
            var maTim = txtTimKH.Text.Trim();
            var ketQua = qlkh.TimKhachHangTheoSDT(maTim);
            HienThiDanhSachKhachHang(ketQua);
        }

        private void lvDanhSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDanhSach.SelectedItems.Count > 0)
            {
                var selectedItem = lvDanhSach.SelectedItems[0];
                if (selectedItem != null)
                {
                    mtbSdt.Text = selectedItem.SubItems[0].Text;
                    txtHoTen.Text = selectedItem.SubItems[1].Text;
                    nudDiem.Value = decimal.Parse(selectedItem.SubItems[2].Text);

                   
                    khachHangHienTai = qlkh.Tim1KhachHangTheoSDT(selectedItem.SubItems[0].Text);

                    if (khachHangHienTai != null)
                    {
                       
                        rdbGiam50k.Checked = false;
                        rdbTangSui.Checked = false;

                        KiemTraVaCapVoucher(khachHangHienTai);
                        TinhToanHoaDon();
                    }
                }
            }
        }

        private void KiemTraVaCapVoucher(KhachHang kh)
        {
            if (kh.TongChiTieu12Thang == 0)
            {
                kh.TongChiTieu12Thang = 3500000;
                kh.SoLuotMua12Thang = 15;
            }

            bool daCoVoucher = false;
            foreach (var v in khoVoucherToanHeThong)
            {
                if (v.Sdt == kh.Sdt) { daCoVoucher = true; break; }
            }

            if (!daCoVoucher)
            {
                decimal chiTieu = kh.TongChiTieu12Thang;
                int luotMua = kh.SoLuotMua12Thang;
                decimal trungBinhDon = luotMua > 0 ? chiTieu / luotMua : 0;

                if (chiTieu >= 5000000 && luotMua >= 12 && trungBinhDon >= 300000)
                {
                    kh.HangThe = "Kim Cương";
                    ThemVoucherVaoKho(kh.Sdt, "Voucher 30k", 30000, 5);
                    ThemVoucherVaoKho(kh.Sdt, "Voucher 50k", 50000, 3);
                }
                else if (chiTieu >= 3000000 && luotMua >= 12 && trungBinhDon >= 250000)
                {
                    kh.HangThe = "Bạch Kim";
                    ThemVoucherVaoKho(kh.Sdt, "Voucher 10k", 10000, 9);
                    ThemVoucherVaoKho(kh.Sdt, "Voucher 20k", 20000, 3);
                }
                else if (chiTieu >= 2000000 && luotMua >= 6 && trungBinhDon >= 200000)
                {
                    kh.HangThe = "Vàng";
                    ThemVoucherVaoKho(kh.Sdt, "Voucher 10k", 10000, 6);
                    ThemVoucherVaoKho(kh.Sdt, "Voucher 15k", 15000, 3);
                }
                else if (chiTieu >= 1000000 && luotMua >= 6 && trungBinhDon >= 150000)
                {
                    kh.HangThe = "Titan";
                    ThemVoucherVaoKho(kh.Sdt, "Voucher 10k", 10000, 6);
                }
                else if (chiTieu >= 500000)
                {
                    kh.HangThe = "Bạc";
                    ThemVoucherVaoKho(kh.Sdt, "Voucher 10k", 10000, 3);
                }

                if (chiTieu >= 500000)
                {
                    ThemVoucherVaoKho(kh.Sdt, "Quyền mua SP thứ 2 giá 1k (Vô thời hạn)", 1, 1);
                }
            }

            cbDungVoucher.Items.Clear();
            cbDungVoucher.Items.Add("-- Không dùng Voucher --");
            foreach (var vc in khoVoucherToanHeThong)
            {
                if (vc.Sdt == kh.Sdt)
                {
                    cbDungVoucher.Items.Add($"{vc.LoaiVoucher} - Giảm {vc.GhiChu}");
                }
            }
            cbDungVoucher.SelectedIndex = 0;
        }

        private void ThemVoucherVaoKho(string sdt, string tenVoucher, int giaTri, int soLuong)
        {
            for (int i = 0; i < soLuong; i++)
            {
                khoVoucherToanHeThong.Add(new KhoVouCher(sdt, tenVoucher, giaTri));
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            var sdt = mtbSdt.Text.Trim();
            var hoTen = txtHoTen.Text.Trim();
            var diemTL = (int)nudDiem.Value;

            if (string.IsNullOrEmpty(sdt) || sdt.Length != 10)
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(hoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var kh = qlkh.Tim1KhachHangTheoSDT(sdt);
            if (kh != null)
            {
                MessageBox.Show("Khách hàng đã tồn tại. Vui lòng nhập số điện thoại khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                kh = new KhachHang { Sdt = sdt, HoTen = hoTen, DiemTL = diemTL };
                qlkh.ThemKhachHang(kh);
                HienThiDanhSachKhachHang(qlkh.DsKhachHang);
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            var sdt = mtbSdt.Text.Trim();
            var diemThem = (int)nudDiem.Value;

            if (string.IsNullOrEmpty(sdt) || sdt.Length != 10)
            {
                MessageBox.Show("Vui lòng chọn hoặc nhập số điện thoại hợp lệ để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var kh = qlkh.Tim1KhachHangTheoSDT(sdt);
            if (kh != null)
            {
                kh.DiemTL += diemThem;
                HienThiDanhSachKhachHang(qlkh.DsKhachHang);
                MessageBox.Show("Cập nhật điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy khách hàng này trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TinhToanHoaDon()
        {
            decimal tongTienHoaDon = 0;
            decimal.TryParse(txtTongHD.Text, out tongTienHoaDon);

            txtTongTienHang.Text = tongTienHoaDon.ToString("N0");

            decimal tienGiamTuDiem = nudDungDiem.Value;

            decimal tienGiamVoucher = 0;
            if (cbDungVoucher.SelectedIndex > 0 && cbDungVoucher.SelectedItem != null)
            {
                string voucherText = cbDungVoucher.SelectedItem.ToString();
                string[] parts = voucherText.Split(new string[] { "Giảm " }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    int.TryParse(parts[1], out int giaTriTru);
                    if (giaTriTru == 1)
                    {
                        tienGiamVoucher = 0;
                    }
                    else
                    {
                        tienGiamVoucher = giaTriTru;
                    }
                }
            }

            decimal tienGiamSinhNhat = 0;
            if (rdbGiam50k.Checked)
            {
                tienGiamSinhNhat = 50000;
            }

            decimal tongGiamTru = tienGiamTuDiem + tienGiamVoucher + tienGiamSinhNhat;

            if (tongGiamTru > tongTienHoaDon)
            {
                tongGiamTru = tongTienHoaDon;
            }

            txtTienGiamTru.Text = tongGiamTru.ToString("N0");

            decimal khachCanTra = tongTienHoaDon - tongGiamTru;
            txtKhachCanTra.Text = khachCanTra.ToString("N0");
        }

        private void txtTongHD_TextChanged(object sender, EventArgs e)
        {
            TinhToanHoaDon();
        }

        
        private void KiemTraHopLeSinhNhat(RadioButton rdb)
        {
            if (rdb.Checked)
            {
                if (khachHangHienTai == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng trước khi áp dụng ưu đãi sinh nhật!", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rdb.Checked = false;
                    return;
                }

                // Kiểm tra xem có đúng tháng sinh không[cite: 2]
                if (khachHangHienTai.NgaySinh.Month != DateTime.Now.Month)
                {
                    MessageBox.Show($"Khách hàng sinh tháng {khachHangHienTai.NgaySinh.Month}, hiện tại là tháng {DateTime.Now.Month}.\nKhông đủ điều kiện áp dụng quà sinh nhật!", "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    rdb.Checked = false;
                    return;
                }

                if (khachDaNhanQuaSinhNhat.Contains(khachHangHienTai.Sdt))
                {
                    MessageBox.Show("Khách hàng này ĐÃ SỬ DỤNG ưu đãi sinh nhật rồi. Mỗi người chỉ được dùng 1 lần duy nhất!", "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rdb.Checked = false;
                    return;
                }

                if (rdb == rdbTangSui)
                {
                    MessageBox.Show("Áp dụng thành công: Tặng 1 tuýp sủi C!", "Sinh nhật", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            TinhToanHoaDon();
        }

        private void rdbGiam50k_CheckedChanged(object sender, EventArgs e)
        {
            KiemTraHopLeSinhNhat(rdbGiam50k);
        }

        private void rdbTangSui_CheckedChanged(object sender, EventArgs e)
        {
            KiemTraHopLeSinhNhat(rdbTangSui);
        }

        private void nudDungDiem_ValueChanged(object sender, EventArgs e)
        {
            decimal tongTienHoaDon = 0;
            decimal.TryParse(txtTongHD.Text, out tongTienHoaDon);

            decimal soDiemMuonDung = nudDungDiem.Value;
            decimal gioiHan50PhanTram = tongTienHoaDon * 0.5m;

            if (soDiemMuonDung > gioiHan50PhanTram)
            {
                MessageBox.Show("Chỉ được dùng điểm giảm trừ tối đa 50% giá trị đơn hàng!", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudDungDiem.Value = gioiHan50PhanTram - (gioiHan50PhanTram % 5000);
                return;
            }

            if (soDiemMuonDung % 5000 != 0 && soDiemMuonDung > 0)
            {
                nudDungDiem.Value = soDiemMuonDung - (soDiemMuonDung % 5000);
                return;
            }

            if (khachHangHienTai != null && soDiemMuonDung > khachHangHienTai.DiemTL)
            {
                nudDungDiem.Value = khachHangHienTai.DiemTL - (khachHangHienTai.DiemTL % 5000);
                return;
            }

            TinhToanHoaDon();
        }

        private void btnTaoNhom_Click(object sender, EventArgs e)
        {
            string sdtChuHo = mstSdtChuHo.Text.Trim();

            if (sdtChuHo.Length != 10)
            {
                MessageBox.Show("Vui lòng nhập số điện thoại chủ hộ (Mã nhóm) gồm 10 chữ số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lvDanhSach.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn 1 khách hàng từ danh sách bên trên để thêm vào nhóm này!", "Hướng dẫn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedSdt = lvDanhSach.SelectedItems[0].SubItems[0].Text;
            var kh = qlkh.Tim1KhachHangTheoSDT(selectedSdt);

            if (kh != null)
            {
                kh.MaGiaDinh = sdtChuHo;

                int soThanhVien = 0;
                List<KhachHang> dsThanhVienTrongNhom = new List<KhachHang>();
                foreach (var k in qlkh.DsKhachHang)
                {
                    if (k.MaGiaDinh == sdtChuHo)
                    {
                        soThanhVien++;
                        dsThanhVienTrongNhom.Add(k);
                    }
                }

                string trangThai = soThanhVien >= 3
                    ? "ĐÃ KÍCH HOẠT TÍCH ĐIỂM GIA ĐÌNH"
                    : $"CHƯA KÍCH HOẠT (Cần thêm {3 - soThanhVien} người)";

                MessageBox.Show($"Đã thêm [{kh.HoTen}] vào nhóm gia đình: {sdtChuHo}\n\nTrạng thái nhóm: {trangThai}", "Quản lý nhóm", MessageBoxButtons.OK, MessageBoxIcon.Information);

                HienThiDanhSachKhachHang(dsThanhVienTrongNhom);
            }
        }
    }
}
