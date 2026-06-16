# NHÓM 1: QUẢN LÝ NHÂN VIÊN (Staff Management)

## Mục tiêu
Quản lý hồ sơ nhân viên (Staff) trong hệ thống CRM: tạo, cập nhật, xoá mềm, khôi phục, lấy danh sách. Staff là tài khoản đăng nhập vào hệ thống và thuộc 1 trong 3 role: `ADMIN`, `MANAGER`, `STAFF`.

---

## 1. Database Schema

Dùng bảng `persons` đã có với discriminator `PersonType.Staff`:
- `Id` (PK, Guid)
- `Fullname` (NVARCHAR, required)
- `Email` (NVARCHAR, required, unique trong phạm vi Staff)
- `Phone` (NVARCHAR, nullable)
- `Location` (NVARCHAR, nullable)
- `Username` (NVARCHAR, required, unique)
- `PasswordHash` (NVARCHAR, BCrypt hash)
- `Role` (NVARCHAR) — `ADMIN` | `MANAGER` | `STAFF`
- `Salary` (DECIMAL, nullable)
- `Status` (INT) — `0=OFFLINE | 1=ONLINE | 2=BUSY | 3=AWAY`
- `LastActiveAt` (DATETIME, nullable)
- `Discriminator` = `Staff` (TPH inheritance)
- Soft delete: `IsDeleted`, `DeletedAt`
- Audit: `CreatedAt`, `UpdatedAt`

---

## 2. Cấu trúc file

### Application Layer
- `Application/UseCases/StaffHandler.cs` — toàn bộ validate + điều phối CRUD.
- `Application/Interfaces/IStaffRepository.cs`

### Infrastructure Layer
- `Infrastructure/Repositories/StaffRepository.cs` — pool DbContext, BCrypt hash.

### API Layer
- `Api/Mutation/StaffMutation.cs` — `createStaff`, `updateStaff`, `deleteStaff`, `restoreStaff`.
- `Api/Query/StaffQuery.cs` — `getStaffs`, `getStaffById`.
- `Api/Input/Type/StaffInputType.cs`, `PersonInputType.cs` — input DTOs.
- `Api/Input/Type/Enums/StaffRoleType.cs` — enum input.

---

## 3. GraphQL Endpoints

### Mutations
- `createStaff(input: StaffInput!): StaffResponse!`
- `updateStaff(input: StaffUpdateInput!, idStaff: UUID!): StaffResponse!`
- `deleteStaff(idStaff: UUID!): Boolean!` — soft delete.
- `restoreStaff(idStaff: UUID!): StaffResponse!`

### Queries
- `getStaffs(filter, sort): [StaffResponse!]!`
- `getStaffById(idStaff: UUID!): StaffResponse`

---

## 4. Luồng nghiệp vụ

### 4.1. Tạo Staff
1. `StaffHandler.CreateStaffAsync(request)` gọi `ValidateStaffCreation`:
   - `Fullname`, `Email`, `Username`, `Password` bắt buộc.
   - `Email` phải match regex `^[^@\s]+@[^@\s]+\.[^@\s]+$`.
   - `Password.Length >= 6`.
   - Nếu có `Role` thì phải ∈ {`ADMIN`, `STAFF`}.
2. Check trùng `Email` (`GetStaffByEmailAsync`).
3. Check trùng `Username` (`GetStaffByUsernameAsync`).
4. `AutoMapper` map `StaffCreationRequest` -> `Person` (discriminator = Staff).
5. `StaffRepository.AddStaffAsync()`:
   - `Id = Guid.NewGuid()`
   - `IsDeleted = false`, `CreatedAt = now`
   - Nếu có `PasswordHash` thì BCrypt re-hash.
6. Trả về `StaffResponse`.

### 4.2. Cập nhật Staff
1. Validate input giống Create (password không bắt buộc).
2. `GetStaffByIdAsync(idStaff)`. Throw `StaffNotFoundException` nếu rỗng.
3. Check email mới không trùng với staff khác (trừ chính staff hiện tại).
4. Cập nhật `Fullname`, `Email`, `Phone`, `Location`, `Role`, `Salary`, `UpdatedAt`.
5. Lưu DB.

### 4.3. Xoá mềm Staff
1. `StaffRepository.SoftDeleteStaffAsync()` set `IsDeleted = true`, `DeletedAt = now`.
2. Throw `StaffNotFoundException` nếu staff không tồn tại.
3. Trả về `"Xóa nhân viên thành công!"`.

### 4.4. Khôi phục Staff
1. `RestoreStaffAsync()` set `IsDeleted = false`, `DeletedAt = null`.
2. Lấy lại staff và trả về `StaffResponse`.

---

## 5. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Email duy nhất | 1 email chỉ map với 1 staff (kể cả staff đã xoá mềm — `IgnoreQueryFilters`). |
| Username duy nhất | Tương tự email. |
| Role hợp lệ | Hiện tại chỉ chấp nhận `ADMIN`, `STAFF`. Cần mở rộng enum nếu muốn `MANAGER`. |
| Soft delete | Không xoá cứng — phục vụ audit và hoàn tác. |
| Không tự xoá | Chưa có rule chặn staff tự xoá chính mình — cần bổ sung nếu yêu cầu. |

---

## 6. Tích hợp ngang

- **Authentication**: `Person.Username/PasswordHash/Role` được dùng bởi `AuthenticationHandler`.
- **Staff Presence**: `Status`, `LastActiveAt` được cập nhật bởi `StaffPresenceHandler`.
- **Deal/Task/Contact/Note**: `IdStaff` FK trỏ tới `Person.Id` (Discriminator=Staff) — staff là người phụ trách.
- **TeamMember**: staff có thể được gán vào `team_members` của Lead/Deal.
- **Audit Log**: mọi thay đổi trên Staff được log thông qua `AuditLogHandler.LogAsync` (xem Nhóm 11).
