# NHÓM 3: QUẢN LÝ KHÁCH HÀNG (Customer Management)

## Mục tiêu
Quản lý khách hàng chính thức (Customer) — đã được chuyển đổi từ Lead hoặc tạo trực tiếp. Customer là nguồn doanh thu, gắn liền với Deal.

---

## 1. Database Schema

Dùng bảng `persons` với `Discriminator = PersonType.Customer`:
- `Id` (PK, Guid)
- `Fullname`, `Email` (required, unique)
- `Phone`, `Location`
- Soft delete + audit.
- **Không có `Status` riêng** cho Customer — trạng thái thể hiện qua Deal (OPEN/WON/LOST) hoặc Contact gần nhất.

> **Khác biệt với Lead:** Customer không có trường `Status` và `Resource` trong entity. Mọi logic tương tác dùng qua Deal/Contact.

---

## 2. Cấu trúc file

### Application Layer
- `Application/UseCases/CustomerHandler.cs`
- `Application/Interfaces/ICustomerRepository.cs`

### API Layer
- `Api/Mutation/CustomerMutation.cs`
- `Api/Query/CustomerQuery.cs`
- `Api/Input/Type/CustomerInputType.cs`
- `Api/Controllers/FileUploadController.cs` — `POST /api/fileupload/customer`.

---

## 3. GraphQL + REST Endpoints

### Mutations
- `createCustomer(request: CustomerCreationRequest!): CustomerResponse!`
- `updateCustomer(request: CustomerUpdateRequest!, idCustomer: UUID!): CustomerResponse!`
- `deleteCustomer(idCustomer: UUID!): Boolean!`
- `restoreCustomer(idCustomer: UUID!): CustomerResponse!`

### Queries
- `getCustomers(filter, sort): [CustomerResponse!]!`
- `getCustomerById(idCustomer: UUID!): [CustomerResponse!]!`

### REST
- `POST /api/fileupload/customer` — multipart import Excel.

---

## 4. Luồng nghiệp vụ

### 4.1. Tạo Customer trực tiếp
1. `CustomerHandler.CreateCustomerAsync`:
   - Validate `Fullname`, `Email` required + email regex.
   - `LeadRepository.CheckPersonByEmailAsync` (dùng chung) chặn trùng email toàn cục.
2. AutoMapper -> `Person` (Discriminator=Customer).
3. `AddCustomerAsync` insert. Trả về `CustomerResponse`.

### 4.2. Cập nhật Customer
- Tương tự Lead: validate, check trùng email, cập nhật field, set `UpdatedAt`.

### 4.3. Xoá Customer (soft)
- `SoftDeleteCustomerAsync` set `IsDeleted = true`.
- Lưu ý: KHÔNG tự động xoá Deal liên quan. Nếu Customer còn Deal active (chưa WON/LOST) thì nên cân nhắc chặn xoá — hiện tại code chưa có rule này.

### 4.4. Import Customer từ Excel
**Endpoint:** `POST /api/fileupload/customer`.

Cấu trúc Excel (sheet 1, từ row 2):
| Cột 1 | Cột 2 | Cột 3 | Cột 4 |
|-------|-------|-------|-------|
| Email | Fullname | Phone | Location |

Luồng giống `ImportLeadExcelAsync`:
1. Bỏ qua nếu thiếu email/fullname hoặc email không hợp lệ hoặc trùng.
2. Insert từng `Person { Discriminator=Customer }`.
3. Trả về `"Đã import thành công {N} customers!"`.

> Cũng không dùng transaction — rủi ro tương tự import Lead.

### 4.5. Khôi phục
- Set `IsDeleted=false`. Trả về `CustomerResponse`.

---

## 5. Conversion Lead -> Customer (luồng liên kết)
Conversion KHÔNG xảy ra trong CustomerHandler. Nó xảy ra tự động trong `ContactHandler.UpdateContactAsync` khi cập nhật `Contact.Status = "SUCCESS"`:
1. Tìm Lead theo `Contact.IdLead`.
2. Check trùng email với Staff (nếu trùng throw `EmailAlreadyExistsException`).
3. `lead.Discriminator = PersonType.Customer` (chuyển discriminator, cùng bảng `persons`).
4. `UpdateLeadAsync` lưu — bản ghi Lead "biến mất" (đã là Customer cùng Id).
5. Khi load lại Contact, navigation `IdLeadNavigation` sẽ trỏ thành Customer.

> **Lưu ý:** Vì cùng `Id`, các reference cũ (Note/Task gắn `LinkedEntityId`) tự động trỏ sang Customer mà không cần update. Đây là trick TPH inheritance rất hay.

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Email unique toàn cục | 1 email = 1 Person (Staff/Lead/Customer). |
| Soft delete | Mặc định ẩn khỏi query. Có thể restore. |
| Không cascade Deal | Xoá customer không tự động huỷ deal — cần xử lý thủ công. |
| Conversion một chiều | Lead -> Customer (qua Contact SUCCESS) là một chiều. Không có API revert. |

---

## 7. Tích hợp ngang

- **Deal**: `Deal.IdCustomer` FK -> Customer.Id. Mỗi customer có thể có nhiều deal.
- **Contact**: contact được tạo cho Lead; khi SUCCESS, lead tự thành customer.
- **Note/Task/Calendar**: gắn `LinkedEntityType="Customer"`, `LinkedEntityId=Id`.
- **Report**: `DashboardResponse`, `LeadConversionResponse`, `ExportCustomersReportAsync`.
- **Chat AI**: CRMie tham chiếu customer list để gợi ý nhân viên.
