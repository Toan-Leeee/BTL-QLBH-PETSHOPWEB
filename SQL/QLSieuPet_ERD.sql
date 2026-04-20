CREATE DATABASE QLSieuPet_ERD;
GO

USE QLSieuPet_ERD;
GO

IF OBJECT_ID('dbo.tblChiTietPhieuNhap', 'U') IS NOT NULL DROP TABLE dbo.tblChiTietPhieuNhap;
IF OBJECT_ID('dbo.tblPhieuNhap', 'U') IS NOT NULL DROP TABLE dbo.tblPhieuNhap;
IF OBJECT_ID('dbo.tblChiTietDonHang', 'U') IS NOT NULL DROP TABLE dbo.tblChiTietDonHang;
IF OBJECT_ID('dbo.tblDonHang', 'U') IS NOT NULL DROP TABLE dbo.tblDonHang;
IF OBJECT_ID('dbo.tblSanPham', 'U') IS NOT NULL DROP TABLE dbo.tblSanPham;
IF OBJECT_ID('dbo.tblDanhMuc', 'U') IS NOT NULL DROP TABLE dbo.tblDanhMuc;
IF OBJECT_ID('dbo.tblKhachHang', 'U') IS NOT NULL DROP TABLE dbo.tblKhachHang;
IF OBJECT_ID('dbo.tblNhanVien', 'U') IS NOT NULL DROP TABLE dbo.tblNhanVien;
GO

CREATE TABLE dbo.tblNhanVien
(
    PK_MaNV VARCHAR(15) PRIMARY KEY,
    sHoTen NVARCHAR(100) NOT NULL,
    sSoDienThoai VARCHAR(15) NULL,
    sEmail VARCHAR(100) NULL UNIQUE,
    sMatKhau VARCHAR(255) NOT NULL,
    sVaiTro NVARCHAR(50) NOT NULL,
    dNgayVaoLam DATE NULL
);
GO

CREATE TABLE dbo.tblKhachHang
(
    PK_MaKH VARCHAR(15) PRIMARY KEY,
    sTenKH NVARCHAR(100) NOT NULL,
    sSoDienThoai VARCHAR(15) NULL UNIQUE,
    sEmail VARCHAR(100) NULL UNIQUE,
    sMatKhau VARCHAR(20) NULL,
    CONSTRAINT CK_tblKhachHang_MatKhau CHECK (sMatKhau IS NULL OR LEN(sMatKhau) > 6)
);
GO

CREATE TABLE dbo.tblDanhMuc
(
    PK_MaDM VARCHAR(15) PRIMARY KEY,
    sTenDM NVARCHAR(100) NOT NULL,
    sMoTa NVARCHAR(255) NULL
);
GO

CREATE TABLE dbo.tblSanPham
(
    PK_MaSP VARCHAR(15) PRIMARY KEY,
    FK_MaDM VARCHAR(15) NOT NULL,
    sTenSP NVARCHAR(100) NOT NULL,
    mGiaBan DECIMAL(10,2) NOT NULL,
    iSoLuongTonKho INT NOT NULL DEFAULT 0,
    sMoTa NVARCHAR(MAX) NULL,
    sHinhAnh VARCHAR(255) NULL,
    bTrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_tblSanPham_tblDanhMuc FOREIGN KEY (FK_MaDM) REFERENCES dbo.tblDanhMuc(PK_MaDM),
    CONSTRAINT CK_tblSanPham_GiaBan CHECK (mGiaBan >= 0),
    CONSTRAINT CK_tblSanPham_TonKho CHECK (iSoLuongTonKho >= 0)
);
GO

CREATE TABLE dbo.tblDonHang
(
    PK_MaDH VARCHAR(15) PRIMARY KEY,
    FK_MaNV VARCHAR(15) NULL,
    FK_MaKH VARCHAR(15) NULL,
    dNgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    sTrangThai NVARCHAR(50) NOT NULL,
    mTongTien DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_tblDonHang_tblNhanVien FOREIGN KEY (FK_MaNV) REFERENCES dbo.tblNhanVien(PK_MaNV),
    CONSTRAINT FK_tblDonHang_tblKhachHang FOREIGN KEY (FK_MaKH) REFERENCES dbo.tblKhachHang(PK_MaKH),
    CONSTRAINT CK_tblDonHang_TongTien CHECK (mTongTien >= 0)
);
GO

CREATE TABLE dbo.tblChiTietDonHang
(
    PK_MaCTDH VARCHAR(15) PRIMARY KEY,
    FK_MaDH VARCHAR(15) NOT NULL,
    FK_MaSP VARCHAR(15) NOT NULL,
    iSoLuong INT NOT NULL,
    mDonGiaBan DECIMAL(10,2) NOT NULL,
    mThanhTien DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_tblChiTietDonHang_tblDonHang FOREIGN KEY (FK_MaDH) REFERENCES dbo.tblDonHang(PK_MaDH),
    CONSTRAINT FK_tblChiTietDonHang_tblSanPham FOREIGN KEY (FK_MaSP) REFERENCES dbo.tblSanPham(PK_MaSP),
    CONSTRAINT CK_tblChiTietDonHang_SoLuong CHECK (iSoLuong > 0),
    CONSTRAINT CK_tblChiTietDonHang_DonGia CHECK (mDonGiaBan >= 0),
    CONSTRAINT CK_tblChiTietDonHang_ThanhTien CHECK (mThanhTien >= 0)
);
GO

CREATE TABLE dbo.tblPhieuNhap
(
    PK_MaPN VARCHAR(15) PRIMARY KEY,
    FK_MaNV VARCHAR(15) NOT NULL,
    dNgayNhap DATETIME NOT NULL DEFAULT GETDATE(),
    sNhaCungCap NVARCHAR(200) NULL,
    CONSTRAINT FK_tblPhieuNhap_tblNhanVien FOREIGN KEY (FK_MaNV) REFERENCES dbo.tblNhanVien(PK_MaNV)
);
GO

CREATE TABLE dbo.tblChiTietPhieuNhap
(
    FK_MaPN VARCHAR(15) NOT NULL,
    FK_MaSP VARCHAR(15) NOT NULL,
    iSoLuongNhap INT NOT NULL,
    mGiaNhap DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_tblChiTietPhieuNhap PRIMARY KEY (FK_MaPN, FK_MaSP),
    CONSTRAINT FK_tblChiTietPhieuNhap_tblPhieuNhap FOREIGN KEY (FK_MaPN) REFERENCES dbo.tblPhieuNhap(PK_MaPN),
    CONSTRAINT FK_tblChiTietPhieuNhap_tblSanPham FOREIGN KEY (FK_MaSP) REFERENCES dbo.tblSanPham(PK_MaSP),
    CONSTRAINT CK_tblChiTietPhieuNhap_SoLuong CHECK (iSoLuongNhap > 0),
    CONSTRAINT CK_tblChiTietPhieuNhap_GiaNhap CHECK (mGiaNhap >= 0)
);
GO

INSERT INTO dbo.tblNhanVien (PK_MaNV, sHoTen, sSoDienThoai, sEmail, sMatKhau, sVaiTro, dNgayVaoLam)
VALUES
('NV001', N'Le Xuan Toan', '0912345678', 'admin@sieupet.vn', '123456', N'Admin', '2025-01-01'),
('NV002', N'Nguyen Minh Anh', '0909111222', 'ops@sieupet.vn', '123456', N'Nhan vien', '2025-04-01'),
('NV003', N'Le Thu Ha', '0909888777', 'support@sieupet.vn', '123456', N'CSKH', '2025-08-15');
GO

INSERT INTO dbo.tblKhachHang (PK_MaKH, sTenKH, sSoDienThoai, sEmail, sMatKhau)
VALUES
('KH001', N'Le Thu Ha', '0909888777', 'khachhang@sieupet.vn', '1234567'),
('KH002', N'Nguyen Minh Anh', '0911222333', 'minhanh@gmail.com', '1234567'),
('KH003', N'Tran Quoc Bao', '0988333444', 'quocbao@gmail.com', '1234567');
GO

INSERT INTO dbo.tblDanhMuc (PK_MaDM, sTenDM, sMoTa)
VALUES
('DM001', N'Cho nho', N'Cac giong cho nho, de cham soc'),
('DM002', N'Cho con', N'Cac be cun trong giai doan so sinh den 3 thang'),
('DM003', N'Phu kien', N'Do choi, bat an, day dat');
GO

INSERT INTO dbo.tblSanPham (PK_MaSP, FK_MaDM, sTenSP, mGiaBan, iSoLuongTonKho, sMoTa, sHinhAnh, bTrangThai)
VALUES
('SP001', 'DM001', N'M0231 - Pomeranian Trang', 6900000, 1, N'Giong duc, 02 thang, mau trang', '/images/products/pomeranian-white.svg', 1),
('SP002', 'DM001', N'M0502 - Poodle Tiny Vang', 3900000, 1, N'Giong cai, 02 thang, mau vang kem', '/images/products/poodle-gold.svg', 1),
('SP003', 'DM001', N'M0102 - Poodle Tiny Sepia', 4000000, 1, N'Giong duc, 02 thang, mau nau do', '/images/products/poodle-brown.svg', 1),
('SP004', 'DM002', N'M0512 - Alaskan Malamute', 8900000, 1, N'Giong duc, 03 thang, mau xam trang', '/images/products/alaskan.svg', 1),
('SP005', 'DM002', N'M0128 - Pembroke Corgi', 7900000, 1, N'Giong duc, 02 thang, chan ngan', '/images/products/corgi.svg', 1),
('SP006', 'DM002', N'#1000078 - Shiba Inu Nau Do', 12000000, 1, N'Giong cai, 02 thang, nau do', '/images/products/shiba.svg', 1),
('SP007', 'DM001', N'Pomeranian Trang Tuyet', 7300000, 1, N'Long day, mat nho, dang yeu', '/images/products/Frame 7.png', 1),
('SP008', 'DM002', N'Pembroke Corgi Nau Trang', 8100000, 1, N'Chan ngan, hoat bat, than thien', '/images/products/image 6.jpg', 1),
('SP009', 'DM001', N'Phoc Soc Den Trang Mini', 7600000, 1, N'Long xu, nho gon, lanh loi', '/images/products/image 7.jpg', 1),
('SP010', 'DM001', N'Chihuahua Long Vang Kem', 5400000, 1, N'Nho nhan, nhanh nhen, de nuoi', '/images/products/image 22.jpg', 1),
('SP011', 'DM001', N'Poodle Tiny No Xanh', 4200000, 1, N'Giong nho, phu hop nuoi can ho', '/images/products/image 23.jpg', 1),
('SP012', 'DM001', N'Poodle Nau Do Mini', 4600000, 1, N'Long xoan dep, thong minh', '/images/products/image 24.jpg', 1),
('SP013', 'DM002', N'Alaskan Puppy Mat Cuoi', 9300000, 1, N'Than hinh khoe, long day', '/images/products/image 25.jpg', 1),
('SP014', 'DM001', N'Be Cun Trang Den Mini', 6100000, 1, N'Hien, de cham soc, hop gia dinh', '/images/products/image 26.jpg', 1),
('SP015', 'DM001', N'Poodle Tiny Kem Sang', 4300000, 1, N'Bo long sang, ngoan va quan chu', '/images/products/image 27.jpg', 1),
('SP016', 'DM001', N'Chihuahua Tai Dung', 5600000, 1, N'Kich thuoc nho, lanh loi', '/images/products/image 28.jpg', 1),
('SP017', 'DM002', N'Corgi Puppy Nau Trang', 8300000, 1, N'Dang thap, chan ngan, than thien', '/images/products/image 29.jpg', 1),
('SP018', 'DM002', N'Alaskan Xam Trang Mini', 9700000, 1, N'Long day, mau dep, khoe manh', '/images/products/image 30.jpg', 1),
('PK001', 'DM003', N'Bat an chong truot', 320000, 18, N'Bat an cho thu cung', '/images/products/bowl.svg', 1),
('PK002', 'DM003', N'Day dat di dao', 180000, 25, N'Size M, mau navy', '/images/products/leash.svg', 1),
('PK003', 'DM003', N'Vong co thu cung', 120000, 30, N'Chat lieu mem, nhieu mau', '/images/products/default.svg', 1),
('PK004', 'DM003', N'Ao thu cung mua dong', 210000, 20, N'Giu am tot, vai mem', '/images/products/default.svg', 1),
('PK005', 'DM003', N'Luoc chai long', 95000, 35, N'Giup lam muot va go roi long', '/images/products/default.svg', 1),
('PK006', 'DM003', N'Sua tam thu cung', 135000, 40, N'Huong diu nhe, an toan da', '/images/products/default.svg', 1),
('PK007', 'DM003', N'Do choi bong cao su', 70000, 50, N'Giup thu cung van dong tot hon', '/images/products/default.svg', 1),
('PK008', 'DM003', N'Khay ve sinh thu cung', 280000, 16, N'Nhua ben, de lau chui', '/images/products/default.svg', 1);
GO

INSERT INTO dbo.tblDonHang (PK_MaDH, FK_MaNV, FK_MaKH, dNgayTao, sTrangThai, mTongTien)
VALUES
('DH001', 'NV001', 'KH002', GETDATE(), N'Hoan tat', 12200000),
('DH002', 'NV002', 'KH001', DATEADD(HOUR, -2, GETDATE()), N'Cho xac nhan', 8900000),
('DH003', 'NV003', 'KH003', DATEADD(HOUR, -1, GETDATE()), N'Cho thanh toan', 500000);
GO

INSERT INTO dbo.tblChiTietDonHang (PK_MaCTDH, FK_MaDH, FK_MaSP, iSoLuong, mDonGiaBan, mThanhTien)
VALUES
('CT001', 'DH001', 'SP006', 1, 12000000, 12000000),
('CT002', 'DH001', 'PK001', 1, 320000, 320000),
('CT003', 'DH001', 'PK002', 1, 180000, 180000),
('CT004', 'DH002', 'SP004', 1, 8900000, 8900000),
('CT005', 'DH003', 'PK001', 1, 320000, 320000),
('CT006', 'DH003', 'PK002', 1, 180000, 180000);
GO

INSERT INTO dbo.tblPhieuNhap (PK_MaPN, FK_MaNV, dNgayNhap, sNhaCungCap)
VALUES
('PN001', 'NV001', DATEADD(DAY, -10, GETDATE()), N'Trai giong Happy Paws'),
('PN002', 'NV002', DATEADD(DAY, -6, GETDATE()), N'Kho phu kien Pet World'),
('PN003', 'NV002', DATEADD(DAY, -2, GETDATE()), N'Nha phan phoi Doggy House');
GO

INSERT INTO dbo.tblChiTietPhieuNhap (FK_MaPN, FK_MaSP, iSoLuongNhap, mGiaNhap)
VALUES
('PN001', 'SP004', 1, 7200000),
('PN001', 'SP005', 1, 6300000),
('PN002', 'PK001', 20, 190000),
('PN002', 'PK002', 30, 95000),
('PN003', 'SP006', 1, 9800000);
GO
