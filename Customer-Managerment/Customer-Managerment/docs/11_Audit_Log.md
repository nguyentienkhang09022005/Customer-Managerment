# NHÓM 11: NHẬT KÝ KIỂM TOÁN (Audit Log)

## Mục tiêu
Ghi lại toàn bộ thao tác quan trọng (Create/Update/Delete/Restore/Assign/Login/Logout) trên các entity, kèm old/new values, IP, user agent. Hỗ trợ truy vết lịch sử thay đổi, phục vụ compliance.

---

## 1. Database Schema

```csharp
AuditLog:
  IdLog (PK, Guid)
  Action      (NVARCHAR)  // CREATE | UPDATE | DELETE | RESTORE | ASSIGN | LOGIN | LOGOUT
  EntityType  (NVARCHAR)  // Staff | Lead | Customer | Contact | Deal | Task | Note | Notification | TeamMember
  EntityId    (Guid)
  OldValues   (NVARCHAR, nullable)  -- JSON serialize
  NewValues   (NVARCHAR, nullable)  -- JSON serialize
  IdStaff     (Guid?, nullable)     -- Staff thực hiện
  StaffName   (NVARCHAR, nullable)
  IpAddress   (NVARCHAR, nullable)
  UserAgent   (NVARCHAR, nullable)
  Timestamp   (DATETIME)
  Description (NVARCHAR, nullable)  -- Mô tả tự động: "UPDATE Lead 'xxx' bởi 'yyyy'"
```

---

## 2. Constants

### `AuditActionConstant`
- `CREATE`, `UPDATE`, `DELETE`, `RESTORE`, `ASSIGN`, `LOGIN`, `LOGOUT`

### `AuditEntityTypeConstant`
- `Staff`, `Lead`, `Customer`, `Contact`, `Deal`, `Task`, `Note`, `Notification`, `TeamMember`

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/AuditLogHandler.cs` — service ghi log + query.
- `Application/Interfaces/IAuditLogRepository.cs`

### API Layer
- `Api/Query/AuditLogQuery.cs`

> **Lưu ý:** Hiện KHÔNG có `AuditLogMutation` — chỉ có query đọc. Việc ghi log được thực hiện bên trong các handler nghiệp vụ (qua dependency injection `AuditLogHandler.LogAsync`) hoặc có thể được gọi thủ công từ mutation khác (chưa thấy sử dụng trong code hiện tại).

---

## 4. GraphQL Endpoints (chỉ Query)

- `getAuditLogs(entityType?, entityId?, fromDate?, toDate?, page=1, pageSize=50): [AuditLogResponse!]!`
- `getAuditLogsByStaff(idStaff: UUID!, fromDate?, toDate?, page, pageSize): [AuditLogResponse!]!`
- `getAuditLogsByAction(action: String!, fromDate?, toDate?, page, pageSize): [AuditLogResponse!]!`
- `getEntityHistory(entityType: String!, entityId: UUID!): [AuditLogResponse!]!` (lấy tối đa 100 log mới nhất)
- `getAuditStatistics(fromDate, toDate): AuditStatisticsResponse!`

---

## 5. Luồng nghiệp vụ

### 5.1. Ghi log (Programmatic)
`AuditLogHandler.LogAsync(action, entityType, entityId, oldValues, newValues, idStaff?, ipAddress?, userAgent?, description?)`:
1. Lấy `staffName` từ `StaffRepository.GetStaffByIdAsync` nếu có `idStaff`.
2. Tạo `AuditLog`:
   - `Action = action.ToUpper()`.
   - `OldValues` / `NewValues` = `JsonSerializer.Serialize(...)` nếu object != null.
   - `Description` = custom nếu có, ngược lại auto-generate: `"{action} {entityType} '{entityId}' bởi '{staffName}'"`.
3. Lưu DB.

### 5.2. Query logs
- `GetAuditLogsAsync(entityType?, entityId?, fromDate?, toDate?, page, pageSize)`:
  - Nếu cả `entityType` + `entityId` -> lọc theo entity.
  - Nếu cả `fromDate` + `toDate` -> lọc theo range.
  - Nếu không có filter -> lấy tất cả.
  - Order by `Timestamp DESC`. Skip/Take theo page.
  - Trả về `AuditLogListResponse { Logs, TotalCount }`.

- `GetByStaffAsync`, `GetByActionAsync` tương tự nhưng filter theo `IdStaff` / `Action`.

- `GetEntityHistoryAsync(entityType, entityId)`: lấy tối đa 100 log mới nhất cho 1 entity — phục vụ hiển thị "lịch sử thay đổi" trên UI.

- `GetStatisticsAsync(fromDate, toDate)`: trả về `AuditStatisticsResponse` (số log theo action/entity — do repository tự tính).

### 5.3. Phân trang
- Page mặc định = 1, pageSize = 50.
- Công thức: `Skip((page - 1) * pageSize).Take(pageSize)`.
- `GetAuditLogsAsync` cũng trả về `TotalCount` (qua `GetTotalCountAsync`).

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Soft delete | AuditLog KHÔNG bị xoá khi entity bị xoá mềm — giữ lịch sử. |
| Action uppercase | `Action` luôn được uppercase trước khi lưu. |
| JSON serialize | OldValues/NewValues lưu dưới dạng JSON string. |
| Không tự động ghi | Các handler nghiệp vụ (Lead, Customer, Deal...) **CHƯA** tự gọi `AuditLogHandler.LogAsync`. Cần bổ sung để đảm bảo compliance. |
| IP & UserAgent | Phải được truyền từ mutation (qua `IHttpContextAccessor`). |

---

## 7. Tích hợp ngang

- **Authentication**: có thể ghi `LOGIN` / `LOGOUT` action (cần tích hợp thêm).
- **CRUD các entity**: cần bổ sung call `AuditLogHandler.LogAsync` ở từng handler.
- **StaffPresence**: ghi `LOGIN` (khi online) và `STATUS_CHANGE` (qua `StaffActivityLog`, không phải `AuditLog`).
- **Reporting**: `AuditStatisticsResponse` dùng cho dashboard compliance.
