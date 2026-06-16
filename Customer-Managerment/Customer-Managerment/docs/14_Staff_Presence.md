# NHÓM 14: TRẠNG THÁI HOẠT ĐỘNG NHÂN VIÊN (Staff Presence)

## Mục tiêu
Theo dõi trạng thái online/offline/busy/away của từng nhân viên để team biết ai đang available, ai đang bận. Ghi log hoạt động (LOGIN, STATUS_CHANGE) để admin giám sát.

---

## 1. Database Schema

Mở rộng bảng `persons` (Staff):
- `Status` (INT) — `0=OFFLINE | 1=ONLINE | 2=BUSY | 3=AWAY`
- `LastActiveAt` (DATETIME, nullable)

Bảng riêng `StaffActivityLog`:
```csharp
StaffActivityLog:
  IdLog (PK, Guid)
  IdStaff (FK -> persons.id)
  Action      (NVARCHAR)  // LOGIN | STATUS_CHANGE | ...
  EntityType  (NVARCHAR, nullable)
  EntityId    (Guid, nullable)
  Timestamp   (DATETIME)
  IpAddress   (NVARCHAR, nullable)
  UserAgent   (NVARCHAR, nullable)
```

---

## 2. Constants

### `StaffStatusConstant`
| Value | Constant | Mô tả |
|-------|----------|-------|
| 0 | OFFLINE | Không hoạt động |
| 1 | ONLINE | Đang online |
| 2 | BUSY | Đang bận (meeting/call) |
| 3 | AWAY | Vắng mặt (đi ra ngoài) |

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/StaffPresenceHandler.cs`
- `Application/Interfaces/IStaffActivityLogRepository.cs`

### API Layer
- `Api/Mutation/StaffPresenceMutation.cs`
- `Api/Query/StaffPresenceQuery.cs`

---

## 4. GraphQL Endpoints

### Mutations
- `updateMyStatus(status: Int!): StaffStatusResponse!` — staff tự đổi status.
- `refreshLastActive(): StaffStatusResponse!` — heartbeat từ client (cập nhật LastActiveAt).

### Queries
- `getStaffStatuses(filter, sort): [StaffStatusResponse!]!` — tất cả staff.
- `getOnlineStaffs(filter, sort): [StaffStatusResponse!]!` — chỉ staff có Status = 1.
- `getStaffActivityLogs(idStaff: UUID!, fromDate?, toDate?): [StaffActivityLogResponse!]!`

---

## 5. Luồng nghiệp vụ

### 5.1. Cập nhật status (`updateMyStatus`)
1. Lấy staff theo `idStaff` từ JWT. Throw `StaffNotFoundException`.
2. Validate `status ∈ [0, 3]`.
3. **Nếu `status == 0` (OFFLINE):** throw `ValidationException("Staff không thể tự đặt OFFLINE. Vui lòng đăng xuất!")` — bắt buộc dùng logout mutation.
4. `StaffRepository.UpdateStaffStatusAsync` set `Status = status`.
5. `StaffRepository.UpdateLastActiveAsync` set `LastActiveAt = now`.
6. **Log activity:**
   - Nếu `status == 1` (ONLINE) -> Action = `"LOGIN"`.
   - Ngược lại -> Action = `"STATUS_CHANGE"`.
   - Lưu `StaffActivityLog` với `IpAddress` (từ `Connection.RemoteIpAddress`) và `UserAgent` (từ header).
7. Trả về `StaffStatusResponse`.

### 5.2. Refresh Last Active (`refreshLastActive`)
1. Lấy staff. Throw nếu rỗng.
2. `UpdateLastActiveAsync` set `LastActiveAt = now`. **Không** ghi activity log (chỉ update silent).
3. Trả về `StaffStatusResponse`.

### 5.3. Lấy danh sách status
- `getStaffStatuses()`: tất cả staff, map sang `StaffStatusResponse { IdStaff, Fullname, Email, Status, StatusName, LastActiveAt }`.
- `getOnlineStaffs()`: chỉ staff có `Status = 1` (qua `StaffRepository.GetOnlineStaffsAsync`).

### 5.4. Lấy activity log
- Validate staff tồn tại.
- Nếu có `fromDate` + `toDate` -> query theo range; ngược lại lấy tất cả.
- Order by `Timestamp DESC`, take 100 log mới nhất.
- Map sang `StaffActivityLogResponse`.

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Không tự OFFLINE | STAFF phải logout qua mutation riêng, không đặt Status=0. |
| Activity log khi chuyển status | Mỗi lần `updateMyStatus` đều ghi 1 log (LOGIN hoặc STATUS_CHANGE). |
| Silent refresh | `refreshLastActive` không ghi log, chỉ update LastActiveAt. |
| LastActiveAt | Client nên gọi heartbeat mỗi vài phút để giữ trạng thái "fresh". |
| Auto OFFLINE | **Chưa có background job** tự động chuyển OFFLINE khi LastActiveAt > 15 phút. Cần bổ sung. |
| Auth check | Hiện KHÔNG kiểm tra role khi xem `getStaffStatuses` — bất kỳ staff nào cũng xem được toàn bộ. |

---

## 7. Tích hợp ngang

- **Authentication**: sau khi login thành công, client nên gọi `updateMyStatus(1)` để set ONLINE. Sau logout, `Status` vẫn giữ nguyên giá trị trước đó (cần xử lý thêm khi logout).
- **Dashboard**: lấy `getOnlineStaffs` để hiển thị team online.
- **Notification**: có thể push realtime khi staff chuyển status (chưa implement).
- **Audit Log**: action `LOGIN` có thể được mirror sang `AuditLog` (chưa implement).
