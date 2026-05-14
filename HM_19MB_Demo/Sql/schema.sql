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
