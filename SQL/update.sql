USE QLSieuPet_ERD;
GO

IF COL_LENGTH('dbo.tblNhanVien', 'bDaXoa') IS NULL
    ALTER TABLE dbo.tblNhanVien ADD bDaXoa BIT NOT NULL CONSTRAINT DF_tblNhanVien_bDaXoa DEFAULT 0;
GO

IF COL_LENGTH('dbo.tblKhachHang', 'bDaXoa') IS NULL
    ALTER TABLE dbo.tblKhachHang ADD bDaXoa BIT NOT NULL CONSTRAINT DF_tblKhachHang_bDaXoa DEFAULT 0;
GO

IF COL_LENGTH('dbo.tblDanhMuc', 'bDaXoa') IS NULL
    ALTER TABLE dbo.tblDanhMuc ADD bDaXoa BIT NOT NULL CONSTRAINT DF_tblDanhMuc_bDaXoa DEFAULT 0;
GO

IF COL_LENGTH('dbo.tblSanPham', 'bDaXoa') IS NULL
    ALTER TABLE dbo.tblSanPham ADD bDaXoa BIT NOT NULL CONSTRAINT DF_tblSanPham_bDaXoa DEFAULT 0;
GO

IF COL_LENGTH('dbo.tblDonHang', 'bDaXoa') IS NULL
    ALTER TABLE dbo.tblDonHang ADD bDaXoa BIT NOT NULL CONSTRAINT DF_tblDonHang_bDaXoa DEFAULT 0;
GO

IF COL_LENGTH('dbo.tblPhieuNhap', 'bDaXoa') IS NULL
    ALTER TABLE dbo.tblPhieuNhap ADD bDaXoa BIT NOT NULL CONSTRAINT DF_tblPhieuNhap_bDaXoa DEFAULT 0;
GO

IF OBJECT_ID('dbo.tblNhaCungCap', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblNhaCungCap
    (
        PK_MaNCC VARCHAR(15) PRIMARY KEY,
        sTenNCC NVARCHAR(150) NOT NULL,
        sSoDienThoai VARCHAR(15) NULL,
        sDiaChi NVARCHAR(255) NULL,
        bDaXoa BIT NOT NULL DEFAULT 0
    );
END
GO

IF COL_LENGTH('dbo.tblPhieuNhap', 'FK_MaNCC') IS NULL
    ALTER TABLE dbo.tblPhieuNhap ADD FK_MaNCC VARCHAR(15) NULL;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_tblPhieuNhap_tblNhaCungCap'
)
BEGIN
    ALTER TABLE dbo.tblPhieuNhap
    ADD CONSTRAINT FK_tblPhieuNhap_tblNhaCungCap
    FOREIGN KEY (FK_MaNCC) REFERENCES dbo.tblNhaCungCap(PK_MaNCC);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tblNhaCungCap WHERE PK_MaNCC = 'NCC001')
    INSERT INTO dbo.tblNhaCungCap (PK_MaNCC, sTenNCC, sSoDienThoai, sDiaChi, bDaXoa)
    VALUES ('NCC001', N'Trai giong Happy Paws', '0911000200', N'12 Nguyen Trai, Ha Noi', 0);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tblNhaCungCap WHERE PK_MaNCC = 'NCC002')
    INSERT INTO dbo.tblNhaCungCap (PK_MaNCC, sTenNCC, sSoDienThoai, sDiaChi, bDaXoa)
    VALUES ('NCC002', N'Kho phu kien Pet World', '0911000201', N'25 Le Loi, Ha Noi', 0);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tblNhaCungCap WHERE PK_MaNCC = 'NCC003')
    INSERT INTO dbo.tblNhaCungCap (PK_MaNCC, sTenNCC, sSoDienThoai, sDiaChi, bDaXoa)
    VALUES ('NCC003', N'Nha phan phoi Doggy House', '0911000202', N'88 Tran Phu, Ha Noi', 0);
GO
