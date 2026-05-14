# NHÓM 4: TRẠNG THÁI HOẠT ĐỘNG (Staff Presence)

## Mục tiêu
Theo dõi trạng thái online/offline của Staff để team biết ai đang available.

---

## 1. Database Schema

### Staff Status (mở rộng Person)

Thêm vào Person entity (Staff):
```
- Status (INT) -- 0=OFFLINE, 1=ONLINE, 2=BUSY, 3=AWAY
- LastActiveAt (DATETIME)
```

### StaffActivityLog Entity (lịch sử hoạt động)
```
StaffActivityLog:
- IdLog (PK, Guid)
- IdStaff (FK -> persons.id)
- Action (NVARCHAR100) -- LOGIN, LOGOUT, VIEW_LEAD, VIEW_DEAL, UPDATE_CONTACT
- EntityType (NVARCHAR50, NULLABLE)
- EntityId (GUID, NULLABLE)
- Timestamp (DATETIME)
- IpAddress (NVARCHAR50, NULLABLE)
- UserAgent (NVARCHAR500, NULLABLE)
```

---

## 2. Files cần tạo mới

### Domain Layer
- `Domain/Constant/StaffStatusConstant.cs`
- `Domain/Exception/InvalidStatusException.cs`

### Application Layer
- `DTOs/Response/StaffStatusResponse.cs`
- `DTOs/Response/StaffActivityLogResponse.cs`
- `Interfaces/IStaffActivityLogRepository.cs`
- `UseCases/StaffPresenceHandler.cs`

### Infrastructure Layer
- `Repositories/StaffActivityLogRepository.cs`
- `Mapping/StaffPresenceMapper.cs`

### API Layer
- `Input/Type/Enums/StaffStatusType.cs`
- `Query/StaffPresenceQuery.cs`
- `Mutation/StaffPresenceMutation.cs`

---

## 3. API Endpoints

### Mutations
- `updateMyStatus(status)` - Staff cập nhật trạng thái của mình
- `refreshLastActive()` - Staff gọi để cập nhật LastActiveAt

### Queries
- `getStaffStatuses()` - Lấy trạng thái tất cả staff (Admin)
- `getOnlineStaffs()` - Lấy danh sách staff đang online
- `getStaffActivityLogs(idStaff, fromDate, toDate)` - Lấy lịch sử hoạt động

---

## 4. Status Values

| Value | Constant | Mô tả |
|-------|----------|-------|
| 0 | OFFLINE | Không hoạt động |
| 1 | ONLINE | Đang online |
| 2 | BUSY | Đang bận (meeting/call) |
| 3 | AWAY | Vắng mặt (đi ra ngoài) |

---

## 5. Business Rules

### Auto Status Changes
- Khi Staff login thành công -> Status = ONLINE, LastActiveAt = now
- Khi Staff logout -> Status = OFFLINE
- Khi LastActiveAt > 15 phút không có action -> Status = OFFLINE (background job)

### Manual Status
- Staff có thể tự đặt BUSY hoặc AWAY
- Staff không thể tự đặt OFFLINE (phải logout)

### Activity Logging
- Ghi log khi: LOGIN, LOGOUT, CREATE, UPDATE, DELETE trên bất kỳ entity nào
- Log entity access: VIEW, UPDATE
- Batch insert log (insert nhiều logs cùng lúc)

---

## 6. Implementation Order

1. Thêm fields vào Person entity (Status, LastActiveAt)
2. StaffActivityLog entity + Repository
3. Handler
4. GraphQL types + Query + Mutation
5. Cập nhật AuthenticationHandler để set status khi login/logout
6. Tạo migration
7. Background job cho auto-offline

---

## 7. Lưu ý

- Cần cập nhật `Person` entity để thêm Status và LastActiveAt
- Tạo migration để thêm columns vào persons table
- Backend job (hangfire/quartz) để update OFFLINE tự động