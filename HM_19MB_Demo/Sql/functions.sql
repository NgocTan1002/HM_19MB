-- ----------------------------------------------------------------
-- fn_tao_phien: Tạo phiên hiệu chuẩn mới, trả về id vừa tạo.
--
-- Tại sao dùng function thay vì INSERT trực tiếp:
--   • Cho phép thêm validation (ví dụ: kiểm tra ngày hợp lệ) mà
--     không cần sửa code C# hay câu SQL phía client.
--   • Có thể thêm logic mặc định / audit log về sau chỉ ở 1 chỗ.
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
STABLE  -- không sửa dữ liệu, PostgreSQL có thể tối ưu cache
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
