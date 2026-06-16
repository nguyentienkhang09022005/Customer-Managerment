# NHÓM 4: QUẢN LÝ HOẠT ĐỘNG LIÊN HỆ (Contact Management)

## Mục tiêu
Ghi nhận mọi hoạt động tương tác giữa Staff và Lead (gọi điện, gặp mặt, gửi email). Trạng thái kết quả quyết định việc Lead có được chuyển đổi thành Customer hay không.

---

## 1. Database Schema

```csharp
Contact:
  IdContact (PK, Guid)
  Type     (NVARCHAR 50)  // Loại: CALL, MEETING, EMAIL...
  Title    (NVARCHAR 100) // Tiêu đề
  Content  (NVARCHAR)     // Nội dung
  Status   (NVARCHAR)     // NEW | IN_PROGRESS | SUCCESS | FAILED | CLOSED | CANCELED
  CreatedAt, UpdatedAt
  IsDeleted, DeletedAt
  IdStaff  (FK -> Person Staff)
  IdLead   (FK -> Person Lead)
```

---

## 2. Status constants (`StatusContactConstant`)

| Value | Constant | Mô tả |
|-------|----------|-------|
| `NEW` | Mới tạo, chưa thực hiện |
| `IN_PROGRESS` | Đang xử lý |
| `SUCCESS` | Thành công — **trigger conversion Lead -> Customer** |
| `FAILED` | Thất bại |
| `CLOSED` | Đóng (sau khi có kết quả) |
| `CANCELED` | Huỷ bỏ |

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/ContactHandler.cs`
- `Application/Interfaces/IContactRepository.cs`

### API Layer
- `Api/Mutation/ContactMutation.cs`
- `Api/Query/ContactQuery.cs`
- `Api/Input/Type/ContactInputType.cs`

---

## 4. GraphQL Endpoints

### Mutations
- `createContact(request: ContactCreationRequest!): ContactResponse!`
- `updateContact(request: ContactUpdateRequest!, idContact: UUID!): ContactResponse!`
- `deleteContact(idContact: UUID!): String!`

### Queries
- `getContacts(filter, sort): [ContactResponse!]!`
- `getContactById(idContact: UUID!): [ContactResponse!]!`

---

## 5. Luồng nghiệp vụ

### 5.1. Tạo Contact
1. `ContactHandler.CreateContactAsync`:
   - `ValidateContactCreation`:
     - `IdStaff`, `IdLead` phải là Guid hợp lệ (khác `Guid.Empty`).
     - `Type` max 50 ký tự.
     - `Title` max 100 ký tự.
2. `StaffRepository.GetStaffByIdAsync(IdStaff)` — throw `StaffNotFoundException` nếu rỗng.
3. `LeadRepository.GetLeadByIdAsync(IdLead)` — throw `LeadNotFoundException` nếu rỗng.
4. AutoMapper -> `Contact`. Gán `IdStaff`, `IdLead`.
5. `AddContactAsync`. Trả về `ContactResponse` (kèm `Lead` + `Staff` navigation).

### 5.2. Cập nhật Contact
1. `GetContactByIdAsync` — throw nếu rỗng.
2. Validate các trường update.
3. Cập nhật `Type`, `Title`, `Content` (nếu có).
4. Nếu client gửi `Status` mới:
   - Phải ∈ {NEW, IN_PROGRESS, SUCCESS, FAILED, CLOSED, CANCELED}.
   - Nếu `Status = "SUCCESS"`:
     - Lấy Lead theo `IdLead`. Nếu có:
       - Check email Lead có trùng email Staff không — nếu trùng throw `EmailAlreadyExistsException` (chặn trường hợp nhân viên tự convert).
       - `lead.Discriminator = PersonType.Customer` (chuyển đổi TPH).
       - `UpdatedAt = now`.
       - `UpdateLeadAsync` lưu — **Lead giờ thành Customer**.
   - `Status` được uppercase trước khi lưu.
5. Cập nhật `UpdatedAt`.
6. Lưu DB.
7. Nếu status mới = "SUCCESS" -> navigation trả về là Customer; ngược lại là Lead. Logic ở dòng `if (updatedContact.Status == "SUCCESS")` quyết định query bảng nào.

### 5.3. Xoá Contact
- `SoftDeleteContactAsync` set `IsDeleted = true`. Trả về `"Xóa hoạt động thành công!"`.

---

## 6. Business Rules

| Quy tập | Mô tả |
|---------|-------|
| Conversion tự động | Chỉ khi `Status = "SUCCESS"`. Các status khác không trigger. |
| Email trùng staff | Không cho phép convert lead thành customer nếu email trùng với 1 staff. |
| Status case-insensitive | Client gửi `success` hay `SUCCESS` đều được uppercase. |
| Navigation động | Response chứa `Lead` nếu chưa convert, `Lead` (nhưng là Customer TPH) nếu đã convert. |

---

## 7. Tích hợp ngang

- **Lead -> Customer**: chuyển đổi TPH tự động khi Contact SUCCESS.
- **Statistics**: `GetQuantityStatisticsResponseAsync` đếm tổng contacts + chi tiết theo status.
- **Report**: đếm `contactsCreated` trong `StaffPerformanceResponse`.
- **ExportHandler**: hiện chưa có export riêng cho Contact, nhưng số liệu nằm trong các báo cáo tổng.
- **Notification**: KHÔNG tự tạo notification khi SUCCESS — cần bổ sung nếu muốn thông báo cho team.
