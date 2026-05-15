CREATE TABLE IF NOT EXISTS phien_hieu_chuan (
    id                      SERIAL          PRIMARY KEY,
    ten_thiet_bi            VARCHAR(255)    NOT NULL DEFAULT '',
    ky_hieu                 VARCHAR(100)    NOT NULL DEFAULT '',
    so_hieu                 VARCHAR(100)    NOT NULL DEFAULT '',
    so_tem                  VARCHAR(100)    NOT NULL DEFAULT '',
    noi_san_xuat            VARCHAR(255)    NOT NULL DEFAULT '',
    nam_san_xuat            VARCHAR(10)     NOT NULL DEFAULT '',
    don_vi_su_dung          VARCHAR(255)    NOT NULL DEFAULT '',
    phuong_phap             VARCHAR(255)    NOT NULL DEFAULT '',
    ngay_hieu_chuan         DATE            NOT NULL DEFAULT CURRENT_DATE,
    nhiet_do_moi_truong     VARCHAR(50)     NOT NULL DEFAULT '',
    do_am_tuong_doi        VARCHAR(50)     NOT NULL DEFAULT '',
    dac_tinh_ky_thuat       TEXT            NOT NULL DEFAULT '',
    thiet_bi_chuan          TEXT            NOT NULL DEFAULT '',
    ngay_tao                TIMESTAMP       NOT NULL DEFAULT NOW()
);


-- ----------------------------------------------------------------

CREATE TABLE IF NOT EXISTS ket_qua_do (
    id                      SERIAL          PRIMARY KEY,
    phien_id                INT             NOT NULL
                                REFERENCES phien_hieu_chuan(id) ON DELETE CASCADE,
    thoi_gian_do            TIMESTAMP       NOT NULL,

    -- 10 đầu đo nhiệt độ
    nhiet_do_1              FLOAT           NULL,
    nhiet_do_2              FLOAT           NULL,
    nhiet_do_3              FLOAT           NULL,
    nhiet_do_4              FLOAT           NULL,
    nhiet_do_5              FLOAT           NULL,
    nhiet_do_6              FLOAT           NULL,
    nhiet_do_7              FLOAT           NULL,
    nhiet_do_8              FLOAT           NULL,
    nhiet_do_9              FLOAT           NULL,
    nhiet_do_10             FLOAT           NULL,

    -- 10 đầu đo độ ẩm (nullable — thiết bị chỉ đo nhiệt)
    do_am_1                 FLOAT           NULL,
    do_am_2                 FLOAT           NULL,
    do_am_3                 FLOAT           NULL,
    do_am_4                 FLOAT           NULL,
    do_am_5                 FLOAT           NULL,
    do_am_6                 FLOAT           NULL,
    do_am_7                 FLOAT           NULL,
    do_am_8                 FLOAT           NULL,
    do_am_9                 FLOAT           NULL,
    do_am_10                FLOAT           NULL,

    -- Kết quả tổng hợp
    nhiet_do_tb             FLOAT           NOT NULL DEFAULT 0,
    do_am_tb                FLOAT           NULL,
    do_dong_deu_nhiet       FLOAT           NOT NULL DEFAULT 0,
    do_dong_deu_am          FLOAT           NULL,
    do_on_dinh_nhiet        FLOAT           NULL,
    do_on_dinh_am           FLOAT           NULL,

    ngay_tao                TIMESTAMP       NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_ket_qua_do_phien
    ON ket_qua_do(phien_id);

CREATE INDEX IF NOT EXISTS idx_ket_qua_do_thoi_gian
    ON ket_qua_do(thoi_gian_do);

ALTER TABLE ket_qua_do
    ADD COLUMN IF NOT EXISTS nhiet_do_1 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_2 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_3 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_4 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_5 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_6 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_7 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_8 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_9 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS nhiet_do_10 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_1 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_2 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_3 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_4 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_5 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_6 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_7 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_8 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_9 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_am_10 FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_on_dinh_nhiet FLOAT NULL,
    ADD COLUMN IF NOT EXISTS do_on_dinh_am FLOAT NULL;


-- ================================================================
-- SCHEMA VERSION 4 — Bảng kết quả hiệu chuẩn tổng hợp
-- ================================================================

-- ----------------------------------------------------------------
-- Bảng ket_qua_hieu_chuan
-- Mỗi dòng = 1 điểm kiểm tra (1 giá trị đặt) trong 1 phiên.
-- Tương ứng với 1 dòng trong:
--   • Bảng A.1 Biên bản hiệu chuẩn (Phụ lục A)
--   • Bảng kết quả Giấy chứng nhận hiệu chuẩn (Phụ lục B)
-- ----------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ket_qua_hieu_chuan (
    id                  SERIAL          PRIMARY KEY,
    phien_id            INT             NOT NULL
                            REFERENCES phien_hieu_chuan(id) ON DELETE CASCADE,

    -- Thứ tự điểm kiểm tra trong phiên (1, 2, 3, ...)
    stt                 INT             NOT NULL,

    -- ── Người dùng nhập ────────────────────────────────────────
    -- Nhiệt độ cài đặt trên tủ
    gia_tri_dat         FLOAT           NOT NULL,

    -- Giá trị chỉ thị trung bình của tủ nhiệt (t̄_tn, CT3)
    gia_tri_chi_thi     FLOAT           NOT NULL,

    -- ── Giá trị đọc trên chuẩn tại từng vị trí (t̄_j, CT2) ────
    -- Tối đa 9 vị trí theo QTHC 1.013:2019
    -- NULL nếu không dùng vị trí đó (tuỳ 3/5/9 đầu đo)
    vi_tri_1            FLOAT           NULL,
    vi_tri_2            FLOAT           NULL,
    vi_tri_3            FLOAT           NULL,
    vi_tri_4            FLOAT           NULL,
    vi_tri_5            FLOAT           NULL,
    vi_tri_6            FLOAT           NULL,
    vi_tri_7            FLOAT           NULL,
    vi_tri_8            FLOAT           NULL,
    vi_tri_9            FLOAT           NULL,

    -- ── Kết quả tính toán tổng hợp ─────────────────────────────
    -- t̄_ch: trung bình các vị trí chuẩn đã hiệu chính (CT1)
    gia_tri_trung_binh  FLOAT           NOT NULL,

    -- Δt = t̄_ch − t̄_tn (CT4) — số hiệu chính của tủ nhiệt
    so_hieu_chinh       FLOAT           NOT NULL,

    -- δt_od: độ ổn định (CT5)
    do_on_dinh          FLOAT           NOT NULL,

    -- δt_dd: độ đồng đều (CT6)
    do_dong_deu         FLOAT           NOT NULL,

    -- U: độ không đảm bảo đo mở rộng (CT19, k=2, P=95%)
    do_khong_dam_bao    FLOAT           NOT NULL,

    -- ── Thành phần trung gian (để truy vết, tái hiện tính toán) 
    uch1                FLOAT           NULL,   -- loại A (CT7)
    uch2                FLOAT           NULL,   -- loại B (CT10/CT11)
    uch                 FLOAT           NULL,   -- liên hợp chuẩn (CT12)
    ubk1                FLOAT           NULL,   -- tản mát chỉ thị (CT13)
    ubk2                FLOAT           NULL,   -- theo ổn định (CT15)
    ubk3                FLOAT           NULL,   -- theo đồng đều (CT16)
    ubk4                FLOAT           NULL,   -- theo phân giải (CT17)
    ubk                 FLOAT           NULL,   -- liên hợp tủ (CT18)

    -- ── Metadata tính toán ──────────────────────────────────────
    so_kenh             INT             NULL,   -- j: số kênh chuẩn (3/5/9)
    so_lan_do           INT             NULL,   -- n: số lần đọc
    phuong_phap_b       VARCHAR(10)     NULL,   -- 'U' hoặc 'Delta'

    ngay_tao            TIMESTAMP       NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_kqhc_phien
    ON ket_qua_hieu_chuan(phien_id);

CREATE INDEX IF NOT EXISTS idx_kqhc_phien_stt
    ON ket_qua_hieu_chuan(phien_id, stt);

-- Không cho trùng STT trong cùng 1 phiên
ALTER TABLE ket_qua_hieu_chuan
    DROP CONSTRAINT IF EXISTS uq_kqhc_phien_stt;

ALTER TABLE ket_qua_hieu_chuan
    ADD CONSTRAINT uq_kqhc_phien_stt UNIQUE (phien_id, stt);
