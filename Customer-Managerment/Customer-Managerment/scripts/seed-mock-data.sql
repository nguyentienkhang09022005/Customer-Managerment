-- =========================================================
-- SEED MOCK DATA - Real Estate CRM (Vietnamese)
-- Generated: 2026-06-21
-- Staff (3): Nguyen Van A (ADMIN), Nguyen Van B (STAFF), Tran Van B Updated (STAFF)
-- Password (all): 321321
-- Active Leads (4) | Active Customers (3)
-- ~50 records per table | Theme: Bất động sản Việt Nam
-- =========================================================

BEGIN;

-- =========================================================
-- Clean slate: xóa data cũ của 11 bảng phụ thuộc (giữ nguyên persons).
-- CASCADE xử lý FK: event_participants -> calendar_events,
--                    note_mentions -> notes,
--                    notes -> notes (parent_note_id self-ref).
-- Persons KHÔNG bị ảnh hưởng vì TRUNCATE CASCADE chỉ xóa tables
-- REFERENCE TO các bảng được liệt kê, không xóa referenced tables.
-- =========================================================
TRUNCATE TABLE
    event_participants,
    note_mentions,
    audit_logs,
    team_members,
    staff_activity_logs,
    calendar_events,
    notifications,
    notes,
    tasks,
    deals,
    contacts
CASCADE;

-- =========================================================
-- Index existing persons for round-robin FK
-- =========================================================
CREATE TEMP TABLE _staff ON COMMIT DROP AS
    SELECT id, username, ROW_NUMBER() OVER (ORDER BY username) AS rn,
           count(*) OVER () AS total
    FROM persons WHERE discriminator=0 AND "IsDeleted"=false;

CREATE TEMP TABLE _leads ON COMMIT DROP AS
    SELECT id, fullname, ROW_NUMBER() OVER (ORDER BY "CreatedAt") AS rn,
           count(*) OVER () AS total
    FROM persons WHERE discriminator=1 AND "IsDeleted"=false;

CREATE TEMP TABLE _customers ON COMMIT DROP AS
    SELECT id, fullname, ROW_NUMBER() OVER (ORDER BY "CreatedAt") AS rn,
           count(*) OVER () AS total
    FROM persons WHERE discriminator=2 AND "IsDeleted"=false;

-- =========================================================
-- 1. CONTACTS (50) - Hoạt động liên hệ giữa Staff ↔ Lead
-- =========================================================
INSERT INTO contacts (id_contact, type, title, content, status, id_staff, id_lead, created_at, updated_at)
SELECT
    gen_random_uuid(),
    (ARRAY['Cuộc gọi','Email','Gặp mặt','Khảo sát','Tư vấn'])[1 + (i % 5)],
    (ARRAY[
        'Tư vấn mua căn hộ Quận 2',
        'Gửi báo giá dự án Vinhomes',
        'Xác nhận lịch xem nhà',
        'Theo dõi nhu cầu khách hàng',
        'Gọi điện chăm sóc lead',
        'Email thông tin pháp lý',
        'Hẹn gặp tại văn phòng',
        'Khảo sát nhu cầu mua đất',
        'Tư vấn đầu tư shophouse',
        'Follow-up sau xem nhà',
        'Giới thiệu dự án mới',
        'Báo giá căn hộ mới'
    ])[1 + (i % 12)],
    (ARRAY[
        'Khách quan tâm dự án tại Quận 2, hẹn gọi lại sau 2 tiếng',
        'Đã gửi email với thông tin chi tiết và bảng giá',
        'Khách xác nhận lịch xem nhà vào cuối tuần',
        'Lead có nhu cầu mua để ở, ngân sách 5-7 tỷ VND',
        'Tư vấn về pháp lý và tiến độ thanh toán',
        'Khách hàng cân nhắc, sẽ phản hồi trong tuần',
        'Đã gặp trực tiếp tại văn phòng, có nhu cầu thực',
        'Lead quan tâm đầu tư, hỏi về tiềm năng sinh lời',
        'Đã gửi hợp đồng mẫu và điều khoản',
        'Khách hẹn đưa gia đình đi xem thực tế dự án'
    ])[1 + (i % 10)],
    (ARRAY['Hoàn thành','Đang xử lý','Chờ phản hồi','Đã lên lịch'])[1 + (i % 4)],
    s.id,
    l.id,
    NOW() - ((i * 1.2)::int || ' days')::interval - ((i % 18) || ' hours')::interval,
    NOW() - ((i * 0.6)::int || ' days')::interval
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1
JOIN _leads l ON l.rn = (i % (SELECT total FROM _leads LIMIT 1)) + 1;

-- =========================================================
-- 2. DEALS (50) - Giao dịch BĐS giữa Staff ↔ Customer
-- =========================================================
INSERT INTO deals (id_deal, title, content, price, status, id_staff, id_customer, created_at, updated_at)
SELECT
    gen_random_uuid(),
    properties[1 + (i % 10)] || ' - ' || locations[1 + (i % 8)],
    descriptions[1 + (i % 8)],
    base_prices[1 + (i % 6)] + (i::bigint * 75000000)::numeric,
    deal_statuses[1 + (i % 5)],
    s.id,
    c.id,
    NOW() - ((i * 1.7)::int || ' days')::interval,
    NOW() - ((i * 0.9)::int || ' days')::interval
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1
JOIN _customers c ON c.rn = (i % (SELECT total FROM _customers LIMIT 1)) + 1
CROSS JOIN (VALUES (
    ARRAY[
        'Căn hộ 2PN Vinhomes Grand Park',
        'Căn hộ 3PN Masteri Thảo Điền',
        'Shophouse Sunrise City',
        'Biệt thự Saigon Pearl',
        'Nhà phố Quận 7',
        'Đất nền Nhơn Trạch',
        'Căn hộ The Sun Avenue',
        'Penthouse River Gate',
        'Condotel Hà Đô Centrosa',
        'Officetel Sunshine City'
    ],
    ARRAY[
        'Quận 9, TP.HCM',
        'Quận 2, TP.HCM',
        'Quận 7, TP.HCM',
        'Quận Bình Thạnh, TP.HCM',
        'Thủ Đức, TP.HCM',
        'Đồng Nai',
        'Quận Gò Vấp, TP.HCM',
        'Quận 1, TP.HCM'
    ],
    ARRAY[
        'Căn hộ view sông, nội thất cơ bản, sổ hồng lâu dài',
        'Biệt thự song lập, hoàn thiện cao cấp, vị trí đắc địa',
        'Shophouse mặt tiền đường lớn, kinh doanh tốt',
        'Đất nền dự án, pháp lý minh bạch, tiềm năng tăng giá',
        'Căn hộ cao cấp, đầy đủ tiện ích 5 sao',
        'Nhà phố 4 tầng, hẻm xe hơi, khu dân cư đông đúc',
        'Penthouse tầng cao, view panorama toàn thành phố',
        'Condotel cam kết lợi nhuận 8%/năm trong 5 năm'
    ],
    ARRAY[3500000000::numeric, 8500000000::numeric, 12000000000::numeric,
          2500000000::numeric, 5500000000::numeric, 1800000000::numeric],
    ARRAY['WON','NEGOTIATING','OPEN','NEGOTIATING','LOST']
)) AS v(properties, locations, descriptions, base_prices, deal_statuses);

-- =========================================================
-- 3. TASKS (50) - Công việc được giao cho Staff
-- =========================================================
INSERT INTO tasks (id_task, title, description, due_date, priority, status,
                   id_staff_assigned, linked_entity_type, linked_entity_id,
                   created_at, updated_at)
SELECT
    gen_random_uuid(),
    task_titles[1 + (i % 12)],
    task_descs[1 + (i % 8)],
    CURRENT_DATE + ((((i * 0.6)::int % 30) + 1) || ' days')::interval,
    ((i % 5) + 1),
    (i % 4),
    s.id,
    CASE WHEN i % 3 = 0 THEN 'Lead' ELSE 'Deal' END,
    CASE WHEN i % 3 = 0
         THEN (SELECT id FROM _leads ORDER BY id OFFSET (i % (SELECT total FROM _leads LIMIT 1)) LIMIT 1)
         ELSE (SELECT id FROM deals ORDER BY id LIMIT 1 OFFSET (i - 1))
    END,
    NOW() - ((i * 0.55)::int || ' days')::interval,
    NOW() - ((i * 0.3)::int || ' days')::interval
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1
CROSS JOIN (VALUES (
    ARRAY[
        'Gọi điện tư vấn khách hàng về dự án',
        'Chuẩn bị hợp đồng đặt cọc',
        'Đưa khách đi xem nhà thực tế',
        'Gửi báo giá dự án qua email',
        'Kiểm tra pháp lý dự án',
        'Liên hệ chủ đầu tư xác nhận thông tin',
        'Chuẩn bị hồ sơ vay vốn ngân hàng',
        'Tư vấn thủ tục sang tên sổ đỏ',
        'Theo dõi tiến độ thanh toán đợt 1',
        'Hỗ trợ khách ký hợp đồng mua bán',
        'Khảo sát giá thị trường khu vực',
        'Cập nhật danh sách tin đăng mới'
    ],
    ARRAY[
        'Liên hệ khách hàng qua điện thoại, xác nhận nhu cầu và ngân sách',
        'Chuẩn bị đầy đủ hồ sơ pháp lý và điều khoản hợp đồng',
        'Hẹn khách tại sảnh dự án, đưa đi xem căn mẫu và thực tế',
        'Gửi email kèm bảng giá chi tiết các căn còn trống',
        'Rà soát giấy phép xây dựng, quy hoạch và sổ hồng',
        'Xác nhận lịch bàn giao, tiến độ và chính sách ưu đãi',
        'Hỗ trợ khách chuẩn bị hồ sơ vay vốn ngân hàng',
        'Hướng dẫn thủ tục sang tên, lệ phí trước bạ',
        'Theo dõi tiến độ thanh toán, nhắc nhở khách đúng hạn',
        'Hỗ trợ khách ký hợp đồng tại văn phòng công chứng'
    ]
)) AS t(task_titles, task_descs);

-- =========================================================
-- 4. NOTES (50) - Ghi chú về Lead/Customer/Deal
-- =========================================================
INSERT INTO notes (id_note, content, type, is_pinned, id_staff,
                   linked_entity_type, linked_entity_id,
                   parent_note_id, created_at, updated_at)
SELECT
    gen_random_uuid(),
    note_contents[1 + (i % 15)],
    note_types[1 + (i % 3)],
    (i % 7 = 0),
    s.id,
    entity_types[1 + (i % 3)],
    entity_id,
    CASE WHEN i > 40 AND i % 3 = 0
         THEN (SELECT id_note FROM notes ORDER BY created_at DESC LIMIT 1 OFFSET (i - 41))
         ELSE NULL
    END,
    NOW() - ((i * 0.85)::int || ' days')::interval - ((i % 10) || ' hours')::interval,
    NOW() - ((i * 0.5)::int || ' days')::interval
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1
CROSS JOIN (VALUES (
    ARRAY[
        'Khách quan tâm căn 3PN view sông, hẹn xem thực tế cuối tuần',
        'Đã xác nhận lịch xem vào 14h chiều nay tại dự án',
        'Cần check lại pháp lý dự án trước khi tư vấn khách',
        'Khách hẹn đưa gia đình đi xem thực tế, cần chuẩn bị xe',
        'Đã gửi báo giá qua email, chờ khách phản hồi trong 24h',
        'Khách so sánh giá với dự án lân cận, cần phân tích ưu điểm',
        'Tiềm năng đầu tư cao, khu vực đang phát triển mạnh',
        'Khách ký hợp đồng đặt cọc 100 triệu vào sáng nay',
        'Hỗ trợ khách liên hệ ngân hàng VPBank vay vốn 70%',
        'Khách cần tư vấn thêm về thuế phí và phí bảo trì',
        'Cảnh báo: khách có dấu hiệu so sánh với dự án đối thủ',
        'Đề xuất tặng nội thất để chốt deal trong tuần này',
        'Khách yêu cầu xem giấy tờ pháp lý trước khi quyết định',
        'Follow-up sau 1 tuần chưa thấy phản hồi từ khách',
        'Khách đồng ý ký hợp đồng, lên lịch ký vào thứ 6 tuần sau'
    ],
    ARRAY['Lead','Customer','Deal'],
    ARRAY['Lead','Customer','Deal']
)) AS n(note_contents, note_types, entity_types)
CROSS JOIN LATERAL (
    SELECT CASE
        WHEN i % 3 = 0 THEN (SELECT id FROM _leads ORDER BY id OFFSET (i % (SELECT total FROM _leads LIMIT 1)) LIMIT 1)
        WHEN i % 3 = 1 THEN (SELECT id FROM _customers ORDER BY id OFFSET (i % (SELECT total FROM _customers LIMIT 1)) LIMIT 1)
        ELSE (SELECT id FROM deals ORDER BY id LIMIT 1 OFFSET (LEAST(i - 1, 49)))
    END AS entity_id
) e;

-- =========================================================
-- 5. NOTE_MENTIONS (50) - Mention staff trong notes
-- =========================================================
INSERT INTO note_mentions (id_mention, id_note, id_staff_mentioned, created_at)
SELECT
    gen_random_uuid(),
    n.id_note,
    s.id,
    n.created_at
FROM (
    SELECT id_note, id_staff, created_at, ROW_NUMBER() OVER (ORDER BY created_at DESC) AS rn
    FROM notes LIMIT 50
) n
CROSS JOIN LATERAL (
    SELECT id FROM _staff
    WHERE id != n.id_staff
    ORDER BY rn
    LIMIT 1 + (n.rn % 2)
) s;

-- =========================================================
-- 6. NOTIFICATIONS (50) - Thông báo tới Staff
-- =========================================================
INSERT INTO notifications (id_notification, title, message, type, is_read, is_pinned,
                           id_staff, related_entity_type, related_entity_id, created_at)
SELECT
    gen_random_uuid(),
    notif_titles[1 + (i % 10)],
    notif_messages[1 + (i % 8)],
    notif_types[1 + (i % 5)],
    (i % 3 = 0),
    (i % 11 = 0),
    s.id,
    CASE WHEN i % 2 = 0 THEN 'Deal' ELSE 'Task' END,
    CASE WHEN i % 2 = 0
         THEN (SELECT id FROM deals ORDER BY id LIMIT 1 OFFSET (LEAST(i - 1, 49)))
         ELSE (SELECT id FROM tasks ORDER BY id LIMIT 1 OFFSET (LEAST(i - 1, 49)))
    END,
    NOW() - ((i * 0.28)::int || ' days')::interval - ((i % 14) || ' hours')::interval
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1
CROSS JOIN (VALUES (
    ARRAY[
        'Deal mới được tạo',
        'Task được giao cho bạn',
        'Có người nhắc đến bạn trong ghi chú',
        'Lời mời sự kiện mới',
        'Lead mới được phân công',
        'Đặt cọc thành công',
        'Cập nhật tiến độ dự án',
        'Phản hồi mới từ khách hàng',
        'Deadline công việc sắp đến',
        'Báo cáo tuần đã sẵn sàng'
    ],
    ARRAY[
        'Deal mới đã được tạo, vui lòng kiểm tra và cập nhật',
        'Bạn được giao một task mới, deadline trong 3 ngày',
        '@mention trong ghi chú của dự án Vinhomes',
        'Bạn được mời tham gia buổi xem nhà vào 14h chiều nay',
        'Lead mới đã được phân công cho bạn, vui lòng liên hệ',
        'Khách hàng đã đặt cọc thành công căn hộ 2PN',
        'Dự án Vinhomes Grand Park đã cập nhật tiến độ mới',
        'Có phản hồi mới từ khách hàng về báo giá',
        'Task "Gửi báo giá dự án" sắp đến hạn trong 24h',
        'Báo cáo doanh thu tuần đã được cập nhật'
    ],
    ARRAY['DEAL_CREATED','TASK_ASSIGNED','NOTE_MENTION','EVENT_INVITE','LEAD_ASSIGNED']
)) AS nf(notif_titles, notif_messages, notif_types);

-- =========================================================
-- 7. CALENDAR_EVENTS (50) - Sự kiện lịch
-- =========================================================
INSERT INTO calendar_events (id_event, title, description, event_type,
                              start_time, end_time, location, is_all_day,
                              reminder_minutes, status,
                              id_staff, related_entity_type, related_entity_id,
                              "IsDeleted", "DeletedAt",
                              created_at, updated_at)
SELECT
    gen_random_uuid(),
    event_titles[1 + (i % 12)],
    event_descs[1 + (i % 8)],
    (i % 5),
    NOW() + ((i * 0.55)::int || ' days')::interval + ((10 + (i % 8)) || ' hours')::interval,
    NOW() + ((i * 0.55)::int || ' days')::interval + ((12 + (i % 8)) || ' hours')::interval,
    event_locations[1 + (i % 8)],
    false,
    CASE WHEN i % 3 = 0 THEN 30 WHEN i % 3 = 1 THEN 60 ELSE 1440 END,
    (i % 3),
    s.id,
    CASE WHEN i % 2 = 0 THEN 'Deal' ELSE 'Lead' END,
    CASE WHEN i % 2 = 0
         THEN (SELECT id FROM deals ORDER BY id LIMIT 1 OFFSET (LEAST(i - 1, 49)))
         ELSE (SELECT id FROM _leads ORDER BY id OFFSET (i % (SELECT total FROM _leads LIMIT 1)) LIMIT 1)
    END,
    false,
    NULL,
    NOW() - ((i * 0.4)::int || ' days')::interval,
    NOW() - ((i * 0.2)::int || ' days')::interval
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1
CROSS JOIN (VALUES (
    ARRAY[
        'Xem căn hộ mẫu dự án Vinhomes',
        'Họp team tuần',
        'Gặp khách ký hợp đồng đặt cọc',
        'Khảo sát thực tế dự án mới',
        'Gọi điện follow-up khách hàng',
        'Ký hợp đồng mua bán tại công chứng',
        'Đào tạo nhân viên mới',
        'Họp với chủ đầu tư dự án',
        'Đánh giá hiệu suất kinh doanh Q2',
        'Sự kiện ra mắt dự án Sunshine',
        'Tư vấn pháp lý cho khách VIP',
        'Gặp đối tác ngân hàng về vay vốn'
    ],
    ARRAY[
        'Đưa khách xem căn hộ mẫu 3PN view sông tại tầng 18',
        'Họp team sales tuần, review KPI và target tháng',
        'Hỗ trợ khách ký hợp đồng đặt cọc căn hộ 2PN',
        'Khảo sát thực địa dự án mới tại Nhơn Trạch',
        'Gọi điện follow-up các lead đã gửi báo giá',
        'Hỗ trợ khách ký hợp đồng mua bán tại VPCC',
        'Đào tạo nhân viên mới về quy trình tư vấn',
        'Họp với chủ đầu tư Vinhomes về chính sách mới',
        'Review doanh số Q2 và lên kế hoạch Q3',
        'Tham dự sự kiện ra mắt dự án Sunshine City',
        'Tư vấn pháp lý cho khách VIP mua biệt thự',
        'Gặp đối tác VPBank về chương trình vay vốn ưu đãi'
    ],
    ARRAY[
        'Vinhomes Grand Park, Quận 9',
        'Văn phòng công ty, Quận 1',
        'VP Công chứng Quận 3',
        'Dự án Nhơn Trạch, Đồng Nai',
        'Online qua Zoom',
        'Masteri Thảo Điền, Quận 2',
        'Trụ sở chính, Quận 1',
        'Showroom Sunshine, Quận 7'
    ]
)) AS ev(event_titles, event_descs, event_locations);

-- =========================================================
-- 8. EVENT_PARTICIPANTS (~50) - Mỗi event có 1-2 participants
-- =========================================================
INSERT INTO event_participants (id, id_event, id_staff, status, responded_at)
SELECT
    gen_random_uuid(),
    id_event,
    id_staff,
    1,  -- Accepted
    start_time - ((row_number() OVER (ORDER BY start_time) * 2) || ' hours')::interval
FROM calendar_events
ORDER BY start_time
LIMIT 30;

-- Thêm participants phụ cho 1/3 events
INSERT INTO event_participants (id, id_event, id_staff, status, responded_at)
SELECT
    gen_random_uuid(),
    ce.id_event,
    s.id,
    ((row_number() OVER ())::int % 3),
    CASE WHEN (row_number() OVER () % 3) = 0 THEN NULL
         ELSE NOW() - ((row_number() OVER () * 3) || ' hours')::interval
    END
FROM calendar_events ce
CROSS JOIN _staff s
WHERE s.id != ce.id_staff
LIMIT 20;

-- =========================================================
-- 9. AUDIT_LOGS (50) - Lịch sử thay đổi
-- =========================================================
INSERT INTO audit_logs (id_log, action, entity_type, entity_id,
                        old_values, new_values, id_staff, staff_name,
                        ip_address, user_agent, description, timestamp)
SELECT
    gen_random_uuid(),
    actions[1 + (i % 5)],
    entity_types[1 + (i % 4)],
    CASE
        WHEN i % 4 = 0 THEN (SELECT id_contact FROM contacts ORDER BY id_contact LIMIT 1 OFFSET (LEAST(i - 1, 49)))
        WHEN i % 4 = 1 THEN (SELECT id_deal FROM deals ORDER BY id_deal LIMIT 1 OFFSET (LEAST(i - 1, 49)))
        WHEN i % 4 = 2 THEN (SELECT id_task FROM tasks ORDER BY id_task LIMIT 1 OFFSET (LEAST(i - 1, 49)))
        ELSE (SELECT id_note FROM notes ORDER BY id_note LIMIT 1 OFFSET (LEAST(i - 1, 49)))
    END,
    CASE WHEN actions[1 + (i % 5)] IN ('UPDATE','DELETE')
         THEN jsonb_build_object('status', 'pending', 'price', 3000000000)
         ELSE NULL END,
    CASE WHEN actions[1 + (i % 5)] IN ('CREATE','UPDATE')
         THEN jsonb_build_object('status', 'active', 'price', 3500000000)
         ELSE NULL END,
    s.id,
    (SELECT fullname FROM persons WHERE id = s.id),
    '192.168.1.' || (10 + (i % 200))::text,
    'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    action_descs[1 + (i % 8)],
    NOW() - ((i * 1.3)::int || ' days')::interval - ((i % 18) || ' hours')::interval
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1
CROSS JOIN (VALUES (
    ARRAY['CREATE','UPDATE','DELETE','LOGIN','LOGOUT'],
    ARRAY['Contact','Deal','Task','Note'],
    ARRAY[
        'Tạo mới contact với lead tiềm năng',
        'Cập nhật trạng thái deal sang Đang đàm phán',
        'Đánh dấu hoàn thành task gửi báo giá',
        'Thêm ghi chú cho deal căn hộ 2PN',
        'Xóa contact trùng lặp với lead cũ',
        'Đăng nhập hệ thống CRM',
        'Đăng xuất khỏi hệ thống',
        'Cập nhật priority task lên Cao'
    ]
)) AS a(actions, entity_types, action_descs);

-- =========================================================
-- 10. STAFF_ACTIVITY_LOGS (50) - Lịch sử hoạt động Staff
-- =========================================================
INSERT INTO staff_activity_logs (id_log, action, entity_type, entity_id,
                                  id_staff, ip_address, user_agent, timestamp)
SELECT
    gen_random_uuid(),
    activity_actions[1 + (i % 12)],
    CASE WHEN i % 4 = 0 THEN 'Deal' WHEN i % 4 = 1 THEN 'Contact'
         WHEN i % 4 = 2 THEN 'Task' ELSE 'Note' END,
    CASE
        WHEN i % 4 = 0 THEN (SELECT id_deal FROM deals ORDER BY id_deal LIMIT 1 OFFSET (LEAST(i - 1, 49)))
        WHEN i % 4 = 1 THEN (SELECT id_contact FROM contacts ORDER BY id_contact LIMIT 1 OFFSET (LEAST(i - 1, 49)))
        WHEN i % 4 = 2 THEN (SELECT id_task FROM tasks ORDER BY id_task LIMIT 1 OFFSET (LEAST(i - 1, 49)))
        ELSE (SELECT id_note FROM notes ORDER BY id_note LIMIT 1 OFFSET (LEAST(i - 1, 49)))
    END,
    s.id,
    '10.0.0.' || (10 + (i % 200))::text,
    'Mozilla/5.0 Chrome/120.0 Safari/537.36',
    NOW() - ((i * 0.55)::int || ' days')::interval - ((i % 22) || ' minutes')::interval
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1
CROSS JOIN (VALUES (
    ARRAY[
        'login',
        'logout',
        'view_deal',
        'create_contact',
        'update_task',
        'add_note',
        'send_email',
        'make_call',
        'upload_document',
        'export_report',
        'assign_lead',
        'close_deal'
    ]
)) AS al(activity_actions);

-- =========================================================
-- 11. TEAM_MEMBERS (50) - Phân công team cho entity
-- =========================================================
INSERT INTO team_members (id, entity_type, entity_id, id_staff, role,
                          assigned_at, assigned_by, can_edit, can_delete)
SELECT
    gen_random_uuid(),
    CASE
        WHEN i <= 16 THEN 'Lead'
        WHEN i <= 25 THEN 'Customer'
        ELSE 'Deal'
    END,
    CASE
        WHEN i <= 16 THEN (SELECT id FROM _leads ORDER BY id OFFSET ((i - 1) % (SELECT total FROM _leads LIMIT 1)) LIMIT 1)
        WHEN i <= 25 THEN (SELECT id FROM _customers ORDER BY id OFFSET ((i - 17) % (SELECT total FROM _customers LIMIT 1)) LIMIT 1)
        ELSE (SELECT id_deal FROM deals ORDER BY id_deal LIMIT 1 OFFSET (LEAST(i - 26, 49)))
    END,
    s.id,
    (i % 3),
    NOW() - ((i * 0.7)::int || ' days')::interval,
    'Nguyen Van A',
    (i % 3 != 2),
    (i % 5 = 0)
FROM generate_series(1, 50) AS i
JOIN _staff s ON s.rn = (i % (SELECT total FROM _staff LIMIT 1)) + 1;

COMMIT;

-- =========================================================
-- Verify counts
-- =========================================================
SELECT 'contacts' AS tbl, count(*) FROM contacts
UNION ALL SELECT 'deals', count(*) FROM deals
UNION ALL SELECT 'tasks', count(*) FROM tasks
UNION ALL SELECT 'notes', count(*) FROM notes
UNION ALL SELECT 'note_mentions', count(*) FROM note_mentions
UNION ALL SELECT 'notifications', count(*) FROM notifications
UNION ALL SELECT 'calendar_events', count(*) FROM calendar_events
UNION ALL SELECT 'event_participants', count(*) FROM event_participants
UNION ALL SELECT 'audit_logs', count(*) FROM audit_logs
UNION ALL SELECT 'staff_activity_logs', count(*) FROM staff_activity_logs
UNION ALL SELECT 'team_members', count(*) FROM team_members
ORDER BY tbl;