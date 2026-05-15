-- ----------------------------------------------------------------
-- fn_tao_phien: Tạo phiên hiệu chuẩn mới, trả về id vừa tạo.
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_tao_phien(
    p_ten_thiet_bi          VARCHAR,
    p_ky_hieu               VARCHAR,
    p_so_hieu               VARCHAR,
    p_so_tem                VARCHAR,
    p_noi_san_xuat          VARCHAR,
    p_nam_san_xuat          VARCHAR,
    p_don_vi_su_dung        VARCHAR,
    p_phuong_phap           VARCHAR,
    p_ngay_hieu_chuan       DATE,
    p_nhiet_do_moi_truong   VARCHAR,
    p_do_am_tuong_doi      VARCHAR,
    p_dac_tinh_ky_thuat     TEXT,
    p_thiet_bi_chuan        TEXT
)
RETURNS INT
LANGUAGE plpgsql
AS $$
DECLARE
    v_id INT;
BEGIN
    INSERT INTO phien_hieu_chuan (
        ten_thiet_bi, ky_hieu, so_hieu, so_tem,
        noi_san_xuat, nam_san_xuat, don_vi_su_dung, phuong_phap,
        ngay_hieu_chuan,
        nhiet_do_moi_truong, do_am_tuong_doi,
        dac_tinh_ky_thuat, thiet_bi_chuan
    )
    VALUES (
        p_ten_thiet_bi, p_ky_hieu, p_so_hieu, p_so_tem,
        p_noi_san_xuat, p_nam_san_xuat, p_don_vi_su_dung, p_phuong_phap,
        p_ngay_hieu_chuan,
        p_nhiet_do_moi_truong, p_do_am_tuong_doi,
        p_dac_tinh_ky_thuat, p_thiet_bi_chuan
    )
    RETURNING id INTO v_id;

    RETURN v_id;
END;
$$;


-- fn_luu_ket_qua_do: Lưu 1 block đo (tổng hợp + toàn bộ đầu đo)
DROP FUNCTION IF EXISTS fn_luu_ket_qua_do(
    INT, TIMESTAMP,
    DOUBLE PRECISION, DOUBLE PRECISION,
    DOUBLE PRECISION, DOUBLE PRECISION,
    VARCHAR, JSONB
);

CREATE OR REPLACE FUNCTION fn_luu_ket_qua_do(
    p_phien_id              INT,
    p_thoi_gian_do          TIMESTAMP,
    p_nhiet_do              FLOAT[],   -- array 10 phần tử
    p_do_am                 FLOAT[],   -- array 10 phần tử, NULL-able
    p_nhiet_do_tb           FLOAT,
    p_do_am_tb              FLOAT,
    p_do_dong_deu_nhiet     FLOAT,
    p_do_dong_deu_am        FLOAT,
    p_do_on_dinh_nhiet      FLOAT,
    p_do_on_dinh_am         FLOAT
)
RETURNS INT LANGUAGE plpgsql AS $$
DECLARE v_id INT;
BEGIN
    INSERT INTO ket_qua_do (
        phien_id, thoi_gian_do,
        nhiet_do_1,  nhiet_do_2,  nhiet_do_3,  nhiet_do_4,  nhiet_do_5,
        nhiet_do_6,  nhiet_do_7,  nhiet_do_8,  nhiet_do_9,  nhiet_do_10,
        do_am_1,     do_am_2,     do_am_3,     do_am_4,     do_am_5,
        do_am_6,     do_am_7,     do_am_8,     do_am_9,     do_am_10,
        nhiet_do_tb, do_am_tb,
        do_dong_deu_nhiet, do_dong_deu_am,
        do_on_dinh_nhiet,  do_on_dinh_am
    ) VALUES (
        p_phien_id, p_thoi_gian_do,
        p_nhiet_do[1],  p_nhiet_do[2],  p_nhiet_do[3],  p_nhiet_do[4],  p_nhiet_do[5],
        p_nhiet_do[6],  p_nhiet_do[7],  p_nhiet_do[8],  p_nhiet_do[9],  p_nhiet_do[10],
        p_do_am[1],     p_do_am[2],     p_do_am[3],     p_do_am[4],     p_do_am[5],
        p_do_am[6],     p_do_am[7],     p_do_am[8],     p_do_am[9],     p_do_am[10],
        p_nhiet_do_tb, p_do_am_tb,
        p_do_dong_deu_nhiet, p_do_dong_deu_am,
        p_do_on_dinh_nhiet,  p_do_on_dinh_am
    )
    RETURNING id INTO v_id;
    RETURN v_id;
END;
$$;


-- ----------------------------------------------------------------
-- fn_lay_phien: Lấy metadata của 1 phiên theo id.
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_lay_phien(p_phien_id INT)
RETURNS TABLE (
    ten_thiet_bi            VARCHAR,
    ky_hieu                 VARCHAR,
    so_hieu                 VARCHAR,
    so_tem                  VARCHAR,
    noi_san_xuat            VARCHAR,
    nam_san_xuat            VARCHAR,
    don_vi_su_dung          VARCHAR,
    phuong_phap             VARCHAR,
    ngay_hieu_chuan         DATE,
    nhiet_do_moi_truong     VARCHAR,
    do_am_tuong_doi        VARCHAR,
    dac_tinh_ky_thuat       TEXT,
    thiet_bi_chuan          TEXT
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        ten_thiet_bi, ky_hieu, so_hieu, so_tem,
        noi_san_xuat, nam_san_xuat, don_vi_su_dung, phuong_phap,
        ngay_hieu_chuan,
        nhiet_do_moi_truong, do_am_tuong_doi,
        dac_tinh_ky_thuat, thiet_bi_chuan
    FROM phien_hieu_chuan
    WHERE id = p_phien_id;
$$;


DROP FUNCTION IF EXISTS fn_lay_ket_qua_theo_phien(INT);

CREATE OR REPLACE FUNCTION fn_lay_ket_qua_theo_phien(p_phien_id INT)
RETURNS TABLE (
    ket_qua_id              INT,
    thoi_gian_do            TIMESTAMP,
    nhiet_do_1              FLOAT,
    nhiet_do_2              FLOAT,
    nhiet_do_3              FLOAT,
    nhiet_do_4              FLOAT,
    nhiet_do_5              FLOAT,
    nhiet_do_6              FLOAT,
    nhiet_do_7              FLOAT,
    nhiet_do_8              FLOAT,
    nhiet_do_9              FLOAT,
    nhiet_do_10             FLOAT,
    do_am_1                 FLOAT,
    do_am_2                 FLOAT,
    do_am_3                 FLOAT,
    do_am_4                 FLOAT,
    do_am_5                 FLOAT,
    do_am_6                 FLOAT,
    do_am_7                 FLOAT,
    do_am_8                 FLOAT,
    do_am_9                 FLOAT,
    do_am_10                FLOAT,
    nhiet_do_tb             FLOAT,
    do_am_tb                FLOAT,
    do_dong_deu_nhiet       FLOAT,
    do_dong_deu_am          FLOAT,
    do_on_dinh_nhiet        FLOAT,
    do_on_dinh_am           FLOAT
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        kq.id,
        kq.thoi_gian_do,
        kq.nhiet_do_1,
        kq.nhiet_do_2,
        kq.nhiet_do_3,
        kq.nhiet_do_4,
        kq.nhiet_do_5,
        kq.nhiet_do_6,
        kq.nhiet_do_7,
        kq.nhiet_do_8,
        kq.nhiet_do_9,
        kq.nhiet_do_10,
        kq.do_am_1,
        kq.do_am_2,
        kq.do_am_3,
        kq.do_am_4,
        kq.do_am_5,
        kq.do_am_6,
        kq.do_am_7,
        kq.do_am_8,
        kq.do_am_9,
        kq.do_am_10,
        kq.nhiet_do_tb,
        kq.do_am_tb,
        kq.do_dong_deu_nhiet,
        kq.do_dong_deu_am,
        kq.do_on_dinh_nhiet,
        kq.do_on_dinh_am
    FROM ket_qua_do kq
    WHERE kq.phien_id = p_phien_id
    ORDER BY kq.thoi_gian_do, kq.id;
$$;


-- ================================================================
-- FUNCTIONS VERSION 5 — ket_qua_hieu_chuan
-- Thay đổi: vi_tri_1..9 → kenh_1..10
-- ================================================================

-- ----------------------------------------------------------------
-- fn_luu_ket_qua_hieu_chuan
-- ----------------------------------------------------------------
DROP FUNCTION IF EXISTS fn_luu_ket_qua_hieu_chuan(
    INT, INT, FLOAT, FLOAT,
    FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT,
    FLOAT, FLOAT, FLOAT, FLOAT, FLOAT,
    FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT,
    INT, INT, VARCHAR
);

-- Xoá signature cũ (9 kênh) nếu tồn tại
DROP FUNCTION IF EXISTS fn_luu_ket_qua_hieu_chuan(
    INT, INT, FLOAT, FLOAT,
    FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT,
    FLOAT, FLOAT, FLOAT, FLOAT, FLOAT,
    FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT, FLOAT,
    INT, INT, VARCHAR
);

CREATE OR REPLACE FUNCTION fn_luu_ket_qua_hieu_chuan(
    -- Khoá
    p_phien_id              INT,
    p_stt                   INT,

    -- Người dùng nhập
    p_gia_tri_dat           FLOAT,
    p_gia_tri_chi_thi       FLOAT,

    -- Giá trị từng kênh chuẩn (NULL nếu không dùng) — tối đa 10
    p_kenh_1                FLOAT,
    p_kenh_2                FLOAT,
    p_kenh_3                FLOAT,
    p_kenh_4                FLOAT,
    p_kenh_5                FLOAT,
    p_kenh_6                FLOAT,
    p_kenh_7                FLOAT,
    p_kenh_8                FLOAT,
    p_kenh_9                FLOAT,
    p_kenh_10               FLOAT,

    -- Kết quả tổng hợp
    p_gia_tri_trung_binh    FLOAT,
    p_so_hieu_chinh         FLOAT,
    p_do_on_dinh            FLOAT,
    p_do_dong_deu           FLOAT,
    p_do_khong_dam_bao      FLOAT,

    -- Thành phần trung gian
    p_uch1                  FLOAT,
    p_uch2                  FLOAT,
    p_uch                   FLOAT,
    p_ubk1                  FLOAT,
    p_ubk2                  FLOAT,
    p_ubk3                  FLOAT,
    p_ubk4                  FLOAT,
    p_ubk                   FLOAT,

    -- Metadata tính toán
    p_so_kenh               INT,
    p_so_lan_do             INT,
    p_phuong_phap_b         VARCHAR
)
RETURNS INT
LANGUAGE plpgsql
AS $$
DECLARE
    v_id INT;
BEGIN
    INSERT INTO ket_qua_hieu_chuan (
        phien_id, stt,
        gia_tri_dat, gia_tri_chi_thi,
        kenh_1, kenh_2, kenh_3, kenh_4, kenh_5,
        kenh_6, kenh_7, kenh_8, kenh_9, kenh_10,
        gia_tri_trung_binh, so_hieu_chinh,
        do_on_dinh, do_dong_deu, do_khong_dam_bao,
        uch1, uch2, uch,
        ubk1, ubk2, ubk3, ubk4, ubk,
        so_kenh, so_lan_do, phuong_phap_b
    )
    VALUES (
        p_phien_id, p_stt,
        p_gia_tri_dat, p_gia_tri_chi_thi,
        p_kenh_1, p_kenh_2, p_kenh_3, p_kenh_4, p_kenh_5,
        p_kenh_6, p_kenh_7, p_kenh_8, p_kenh_9, p_kenh_10,
        p_gia_tri_trung_binh, p_so_hieu_chinh,
        p_do_on_dinh, p_do_dong_deu, p_do_khong_dam_bao,
        p_uch1, p_uch2, p_uch,
        p_ubk1, p_ubk2, p_ubk3, p_ubk4, p_ubk,
        p_so_kenh, p_so_lan_do, p_phuong_phap_b
    )
    ON CONFLICT (phien_id, stt) DO UPDATE SET
        gia_tri_dat          = EXCLUDED.gia_tri_dat,
        gia_tri_chi_thi      = EXCLUDED.gia_tri_chi_thi,
        kenh_1               = EXCLUDED.kenh_1,
        kenh_2               = EXCLUDED.kenh_2,
        kenh_3               = EXCLUDED.kenh_3,
        kenh_4               = EXCLUDED.kenh_4,
        kenh_5               = EXCLUDED.kenh_5,
        kenh_6               = EXCLUDED.kenh_6,
        kenh_7               = EXCLUDED.kenh_7,
        kenh_8               = EXCLUDED.kenh_8,
        kenh_9               = EXCLUDED.kenh_9,
        kenh_10              = EXCLUDED.kenh_10,
        gia_tri_trung_binh   = EXCLUDED.gia_tri_trung_binh,
        so_hieu_chinh        = EXCLUDED.so_hieu_chinh,
        do_on_dinh           = EXCLUDED.do_on_dinh,
        do_dong_deu          = EXCLUDED.do_dong_deu,
        do_khong_dam_bao     = EXCLUDED.do_khong_dam_bao,
        uch1                 = EXCLUDED.uch1,
        uch2                 = EXCLUDED.uch2,
        uch                  = EXCLUDED.uch,
        ubk1                 = EXCLUDED.ubk1,
        ubk2                 = EXCLUDED.ubk2,
        ubk3                 = EXCLUDED.ubk3,
        ubk4                 = EXCLUDED.ubk4,
        ubk                  = EXCLUDED.ubk,
        so_kenh              = EXCLUDED.so_kenh,
        so_lan_do            = EXCLUDED.so_lan_do,
        phuong_phap_b        = EXCLUDED.phuong_phap_b,
        ngay_tao             = NOW()
    RETURNING id INTO v_id;

    RETURN v_id;
END;
$$;


-- ----------------------------------------------------------------
-- fn_lay_ket_qua_hieu_chuan
-- ----------------------------------------------------------------
DROP FUNCTION IF EXISTS fn_lay_ket_qua_hieu_chuan(INT);

CREATE OR REPLACE FUNCTION fn_lay_ket_qua_hieu_chuan(p_phien_id INT)
RETURNS TABLE (
    id                  INT,
    stt                 INT,
    gia_tri_dat         FLOAT,
    gia_tri_chi_thi     FLOAT,
    kenh_1              FLOAT,
    kenh_2              FLOAT,
    kenh_3              FLOAT,
    kenh_4              FLOAT,
    kenh_5              FLOAT,
    kenh_6              FLOAT,
    kenh_7              FLOAT,
    kenh_8              FLOAT,
    kenh_9              FLOAT,
    kenh_10             FLOAT,
    gia_tri_trung_binh  FLOAT,
    so_hieu_chinh       FLOAT,
    do_on_dinh          FLOAT,
    do_dong_deu         FLOAT,
    do_khong_dam_bao    FLOAT,
    uch1                FLOAT,
    uch2                FLOAT,
    uch                 FLOAT,
    ubk1                FLOAT,
    ubk2                FLOAT,
    ubk3                FLOAT,
    ubk4                FLOAT,
    ubk                 FLOAT,
    so_kenh             INT,
    so_lan_do           INT,
    phuong_phap_b       VARCHAR
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        id, stt,
        gia_tri_dat, gia_tri_chi_thi,
        kenh_1, kenh_2, kenh_3, kenh_4, kenh_5,
        kenh_6, kenh_7, kenh_8, kenh_9, kenh_10,
        gia_tri_trung_binh, so_hieu_chinh,
        do_on_dinh, do_dong_deu, do_khong_dam_bao,
        uch1, uch2, uch,
        ubk1, ubk2, ubk3, ubk4, ubk,
        so_kenh, so_lan_do, phuong_phap_b
    FROM ket_qua_hieu_chuan
    WHERE phien_id = p_phien_id
    ORDER BY stt;
$$;


-- ----------------------------------------------------------------
-- fn_xoa_ket_qua_hieu_chuan
-- ----------------------------------------------------------------
DROP FUNCTION IF EXISTS fn_xoa_ket_qua_hieu_chuan(INT, INT);

CREATE OR REPLACE FUNCTION fn_xoa_ket_qua_hieu_chuan(
    p_phien_id  INT,
    p_stt       INT
)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM ket_qua_hieu_chuan
    WHERE phien_id = p_phien_id AND stt = p_stt;

    -- Cập nhật lại stt cho các dòng phía sau để liên tục
    UPDATE ket_qua_hieu_chuan
    SET stt = stt - 1
    WHERE phien_id = p_phien_id AND stt > p_stt;
END;
$$;


-- ----------------------------------------------------------------
-- fn_lay_stt_tiep_theo
-- ----------------------------------------------------------------
DROP FUNCTION IF EXISTS fn_lay_stt_tiep_theo(INT);

CREATE OR REPLACE FUNCTION fn_lay_stt_tiep_theo(p_phien_id INT)
RETURNS INT
LANGUAGE sql
AS $$
    SELECT COALESCE(MAX(stt), 0) + 1
    FROM ket_qua_hieu_chuan
    WHERE phien_id = p_phien_id;
$$;
