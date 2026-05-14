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
-- trong 1 lần gọi duy nhất, transaction nằm trong DB.
--
-- Tham số p_so_lieu_json là mảng JSON với format:
--   [{"so_dau_do": 1, "nhiet_do": 25.3, "do_am": 61.2}, ...]
CREATE OR REPLACE FUNCTION fn_luu_ket_qua_do(
    p_phien_id              INT,
    p_thoi_gian_do          TIMESTAMP,
    p_nhiet_do_tb           FLOAT,
    p_do_am_tb              FLOAT,
    p_do_dong_deu_nhiet     FLOAT,
    p_do_dong_deu_am        FLOAT,
    p_do_on_dinh_raw        VARCHAR,
    p_so_lieu_json          JSONB       -- mảng đầu đo
)
RETURNS INT
LANGUAGE plpgsql
AS $$
DECLARE
    v_ket_qua_id    INT;
    v_item          JSONB;
BEGIN
    -- 1. Insert bản ghi tổng hợp
    INSERT INTO ket_qua_do (
        phien_id, thoi_gian_do,
        nhiet_do_tb, do_am_tb,
        do_dong_deu_nhiet, do_dong_deu_am,
        do_on_dinh_raw
    )
    VALUES (
        p_phien_id, p_thoi_gian_do,
        p_nhiet_do_tb, p_do_am_tb,
        p_do_dong_deu_nhiet, p_do_dong_deu_am,
        p_do_on_dinh_raw
    )
    RETURNING id INTO v_ket_qua_id;

    -- 2. Loop qua từng đầu đo trong JSON và insert
    FOR v_item IN SELECT * FROM jsonb_array_elements(p_so_lieu_json)
    LOOP
        -- Bỏ qua nếu nhiet_do hoặc do_am là null (đầu đo lỗi)
        CONTINUE WHEN (v_item->>'nhiet_do') IS NULL;

        INSERT INTO so_lieu_dau_do (ket_qua_id, so_dau_do, nhiet_do, do_am)
        VALUES (
            v_ket_qua_id,
            (v_item->>'so_dau_do')::SMALLINT,
            (v_item->>'nhiet_do')::FLOAT,
            (v_item->>'do_am')::FLOAT
        );
    END LOOP;

    RETURN v_ket_qua_id;
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


-- ----------------------------------------------------------------
-- fn_lay_ket_qua_theo_phien: Lấy toàn bộ kết quả đo + số liệu
-- đầu đo của 1 phiên, sắp xếp theo thời gian và số đầu đo.
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_lay_ket_qua_theo_phien(p_phien_id INT)
RETURNS TABLE (
    ket_qua_id              INT,
    thoi_gian_do            TIMESTAMP,
    nhiet_do_tb             FLOAT,
    do_am_tb                FLOAT,
    do_dong_deu_nhiet       FLOAT,
    do_dong_deu_am          FLOAT,
    do_on_dinh_raw          VARCHAR,
    so_dau_do               SMALLINT,
    nhiet_do                FLOAT,
    do_am                   FLOAT
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        kq.id,
        kq.thoi_gian_do,
        kq.nhiet_do_tb,
        kq.do_am_tb,
        kq.do_dong_deu_nhiet,
        kq.do_dong_deu_am,
        kq.do_on_dinh_raw,
        sl.so_dau_do,
        sl.nhiet_do,
        sl.do_am
    FROM ket_qua_do kq
    JOIN so_lieu_dau_do sl ON kq.id = sl.ket_qua_id
    WHERE kq.phien_id = p_phien_id
    ORDER BY kq.thoi_gian_do, sl.so_dau_do;
$$;
