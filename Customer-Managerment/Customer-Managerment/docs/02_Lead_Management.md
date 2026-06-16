# NHÓM 2: QUẢN LÝ KHÁCH HÀNG TIỀM NĂNG (Lead Management)

## Mục tiêu
Quản lý vòng đời của Lead — khách hàng tiềm năng trước khi trở thành Customer chính thức. Hỗ trợ CRUD, import Excel hàng loạt, gán team phụ trách, theo dõi trạng thái chuyển đổi.

---

## 1. Database Schema

Dùng bảng `persons` với `Discriminator = PersonType.Lead`:
- `Id` (PK, Guid)
- `Fullname` (required)
- `Email` (required, unique trong toàn bộ `persons`)
- `Phone`, `Location`, `Resource` (nullable) — `Resource` = nguồn lead (Facebook, Web, Referral...).
- `Status` (INT) — xem `LeadStatusConstant`
- Soft delete + audit.

`team_members` ánh xạ lead với staff phụ trách (EntityType = "Lead").

---

## 2. Status constants

```csharp
LeadStatusConstant:
  0 = NEW          // Mới tạo
  1 = CONTACTED    // Đã liên hệ
  2 = QUALIFIED    // Đủ điều kiện
  3 = CONVERTED    // Đã chuyển thành Customer
  4 = LOST         // Mất
```

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/LeadHandler.cs`
- `Application/Interfaces/ILeadRepository.cs`

### API Layer
- `Api/Mutation/LeadMutation.cs`
- `Api/Query/LeadQuery.cs`
- `Api/Input/Type/LeadInputType.cs`
- `Api/Controllers/FileUploadController.cs` — endpoint REST `/api/fileupload/lead` để import Excel.

---

## 4. GraphQL + REST Endpoints

### Mutations
- `createLead(request: LeadCreationRequest!): LeadResponse!`
- `updateLead(request: LeadUpdateRequest!, idLead: UUID!): LeadResponse!`
- `deleteLead(idLead: UUID!): String!` — soft delete + xoá `team_members` liên quan.

### Queries
- `getLeads(filter, sort): [LeadResponse!]!`
- `getLeadById(idLead: UUID!): [LeadResponse!]!`

### REST
- `POST /api/fileupload/lead` — multipart file `.xlsx` import hàng loạt.

---

## 5. Luồng nghiệp vụ

### 5.1. Tạo Lead thủ công
1. `LeadHandler.CreateLeadAsync(request)` gọi `ValidateLeadCreation`:
   - `Fullname`, `Email` bắt buộc.
   - `Email` match regex.
2. `LeadRepository.CheckPersonByEmailAsync(email)` — chặn trùng email toàn cục (Lead + Customer + Staff đều check).
3. AutoMapper map -> `Person` (Discriminator = Lead).
4. `AddLeadAsync` insert DB. Trả về `LeadResponse`.

### 5.2. Cập nhật Lead
1. Validate giống Create.
2. Lấy lead theo Id. Throw `LeadNotFoundException` nếu rỗng.
3. Check trùng email nếu đổi email.
4. Cập nhật `Fullname`, `Email`, `Phone`, `Location`, `Resource`. Set `UpdatedAt`.
5. Lưu và trả về.

### 5.3. Xoá Lead
1. `SoftDeleteLeadAsync` set `IsDeleted = true`.
2. **Quan trọng:** `TeamMemberRepository.RemoveByEntityAsync(EntityType=Lead, idLead)` xoá mọi thành viên trong team của lead này — tránh orphan rows.
3. Trả về `"Xóa khách hàng tiềm năng thành công!"`.

### 5.4. Import Lead từ Excel
**Endpoint:** `POST /api/fileupload/lead` (multipart `IFormFile`).

Cấu trúc file Excel (sheet 1, bắt đầu từ row 2):
| Cột 1 | Cột 2 | Cột 3 | Cột 4 | Cột 5 |
|-------|-------|-------|-------|-------|
| Email | Fullname | Phone | Location | Resource |

Luồng:
1. `LeadHandler.ImportLeadExcelAsync(file)`:
   - Throw 400 nếu file rỗng.
   - Mở bằng EPPlus (`ExcelPackage`).
   - Nếu `rowCount < 2` throw "File Excel không có dữ liệu!".
2. Với mỗi row từ 2 đến N:
   - Bỏ qua nếu thiếu email/fullname.
   - Bỏ qua nếu email không hợp lệ.
   - Bỏ qua nếu email đã tồn tại (`CheckPersonByEmailAsync`).
   - Insert `Person { Discriminator = Lead, ... }`.
   - Tăng biến `importedCount`.
3. Trả về `"Đã import thành công {N} leads!"`.

> **Đặc điểm:** Import KHÔNG dùng transaction. Nếu lỗi giữa chừng, các row trước đó vẫn được lưu. Nên cân nhắc wrap trong `DbContext.Database.BeginTransactionAsync()` nếu cần atomic.

### 5.5. Khôi phục Lead
- `RestoreLeadAsync` set `IsDeleted = false`, `DeletedAt = null`. Trả về lead.

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Email toàn cục unique | 1 email chỉ tồn tại ở 1 `Person` (Lead/Customer/Staff). |
| Soft delete | Lead đã xoá mềm ẩn khỏi query mặc định (Global Query Filter). |
| Cascade team_members | Xoá lead -> xoá mọi team member liên quan (Lead, Deal). |
| Conversion | Lead -> Customer thông qua `Contact.Status = SUCCESS` (xem Nhóm 4). |

---

## 7. Tích hợp ngang

- **Contact**: tạo contact cho lead (nhân viên gọi điện, gặp mặt...). Khi status = SUCCESS, lead tự động được convert thành Customer.
- **TeamMember**: có thể thêm nhiều staff cùng phụ trách 1 lead.
- **Task/Note/Calendar**: có thể gắn `LinkedEntityType="Lead"`, `LinkedEntityId=lead.Id`.
- **ReportHandler**: thống kê conversion rate, lead theo nguồn.
- **Chat AI**: CRMie có thể truy vấn lead list để hỗ trợ nhân viên.
