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
    nhiet_do_tb             FLOAT           NOT NULL DEFAULT 0,
    do_am_tb                FLOAT           NULL,
    do_dong_deu_nhiet       FLOAT           NOT NULL DEFAULT 0,
    do_dong_deu_am          FLOAT           NULL,
    do_on_dinh_raw          VARCHAR(50)     NOT NULL DEFAULT '',
    ngay_tao                TIMESTAMP       NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_ket_qua_do_phien
    ON ket_qua_do(phien_id);

CREATE INDEX IF NOT EXISTS idx_ket_qua_do_thoi_gian
    ON ket_qua_do(thoi_gian_do);

-- ----------------------------------------------------------------

CREATE TABLE IF NOT EXISTS so_lieu_dau_do (
    id                      SERIAL          PRIMARY KEY,
    ket_qua_id              INT             NOT NULL
                                REFERENCES ket_qua_do(id) ON DELETE CASCADE,
    so_dau_do               SMALLINT        NOT NULL,
    nhiet_do                FLOAT           NOT NULL,
    do_am                   FLOAT           NULL,

    CONSTRAINT chk_so_dau_do CHECK (so_dau_do BETWEEN 1 AND 10)
);

CREATE INDEX IF NOT EXISTS idx_so_lieu_dau_do_ket_qua
    ON so_lieu_dau_do(ket_qua_id);

CREATE INDEX IF NOT EXISTS idx_so_lieu_dau_do_so_dau
    ON so_lieu_dau_do(so_dau_do);
