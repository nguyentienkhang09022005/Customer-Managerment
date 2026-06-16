# NHÓM 5: QUẢN LÝ GIAO DỊCH (Deal Management)

## Mục tiêu
Quản lý Deal (cơ hội bán hàng) gắn với Customer. Hỗ trợ tạo deal theo nhân viên, theo dõi trạng thái pipeline, phân quyền xem deal theo role.

---

## 1. Database Schema

```csharp
Deal:
  IdDeal (PK, Guid)
  Title    (NVARCHAR 100, required)
  Content  (NVARCHAR, nullable)
  Price    (DECIMAL, >= 0)
  Status   (NVARCHAR)  // OPEN | NEGOTIATING | WON | LOST
  CreatedAt, UpdatedAt
  IsDeleted, DeletedAt
  IdStaff    (FK -> Person Staff, người phụ trách chính)
  IdCustomer (FK -> Person Customer)
```

`team_members` (EntityType = "Deal") quản lý nhiều nhân viên cùng tham gia 1 deal.

---

## 2. Status constants (`StatuDealConstant`)

| Value | Constant | Mô tả |
|-------|----------|-------|
| `OPEN` | Mở, chưa đàm phán |
| `NEGOTIATING` | Đang đàm phán |
| `WON` | Thắng (doanh thu thực) |
| `LOST` | Thua |

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/DealHandler.cs`
- `Application/Interfaces/IDealRepository.cs`

### API Layer
- `Api/Mutation/DealMutation.cs`
- `Api/Query/DealQuery.cs`
- `Api/Input/Type/DealInputType.cs`

---

## 4. GraphQL Endpoints

### Mutations
- `createDeal(request: DealCreationRequest!): DealResponse!` — có rule ép `IdStaff` cho STAFF.
- `updateDeal(request: DealUpdateRequest!, idDeal: UUID!): DealResponse!`
- `deleteDeal(idDeal: UUID!): String!`

### Queries
- `getDeals(filter, sort): [DealResponse!]!` — **chỉ ADMIN**.
- `getMyDeals(filter, sort): [DealResponse!]!` — deal của tôi (OWNER + MEMBER).
- `getDealById(idDeal: UUID!): [DealResponse!]!` — có check role + team.

---

## 5. Luồng nghiệp vụ

### 5.1. Tạo Deal
1. `DealMutation.CreateDealAsync(request)`:
   - Lấy `currentUserId` và `currentUserRole` từ JWT.
   - Nếu `currentUserRole == "STAFF"` -> ép `request.IdStaff = currentUserId` (STAFF không thể gán deal cho người khác).
   - Nếu `currentUserRole == "ADMIN"` -> giữ nguyên `IdStaff` từ request (có thể gán bất kỳ).
2. `DealHandler.CreateDealAsync`:
   - `ValidateDealCreation`:
     - `IdStaff`, `IdCustomer` != `Guid.Empty`.
     - `Title` required, max 100 ký tự.
     - `Price` (nếu có) phải >= 0.
   - `StaffRepository.GetStaffByIdAsync(IdStaff)` — throw `StaffNotFoundException`.
   - `CustomerRepository.GetCustomerByIdAsync(IdCustomer)` — throw `CustomerNotFoundException`.
3. AutoMapper -> `Deal`.
4. `AddDealAsync` insert.
5. **Tự động thêm người tạo vào `team_members`** với `Role = OWNER`, `CanEdit=true`, `CanDelete=true` (xem Nhóm 10).
6. Trả về `DealResponse` (kèm `Customer`, `Staff`).

### 5.2. Cập nhật Deal
1. Lấy deal theo Id. Throw `DealNotFoundException`.
2. `ValidateDealUpdate`:
   - `Title` nếu có -> max 100.
   - `Price` nếu có -> >= 0.
3. Cập nhật các field non-null.
4. Nếu `Status` mới:
   - Phải ∈ {OPEN, NEGOTIATING, WON, LOST}.
   - Uppercase trước khi lưu.
5. Set `UpdatedAt`. Lưu.

### 5.3. Xoá Deal
1. `SoftDeleteDealAsync` set `IsDeleted = true`.
2. **`TeamMemberRepository.RemoveByEntityAsync(EntityType=Deal, idDeal)`** xoá mọi team member liên quan.
3. Trả về `"Xóa deal thành công!"`.

### 5.4. Phân quyền xem Deal
`DealQuery`:
- `getDeals()`:
  - ADMIN -> trả về tất cả deal.
  - STAFF -> **throw `InvalidOperationException`** với message "STAFF nên dùng getMyDeals để lấy danh sách deals của mình".
- `getMyDeals()`:
  - Lấy `teamMemberships` của staff hiện tại (qua `TeamMemberRepository.GetByStaffAsync`).
  - Lọc deal có `IdStaff == currentUserId` HOẶC `IdDeal` nằm trong danh sách deal mà staff là MEMBER.
- `getDealById(id)`:
  - ADMIN -> trả về bất kỳ.
  - STAFF -> chỉ trả về nếu `IdStaff == currentUserId` hoặc là MEMBER trong team deal đó.

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Auto OWNER | Người tạo deal tự động là OWNER trong team_members, full quyền edit/delete. |
| STAFF không gán hộ | STAFF bị ép `IdStaff = currentUserId` khi tạo. |
| STAFF phải dùng getMyDeals | Gọi `getDeals` với role STAFF sẽ throw exception. |
| Cascade team | Xoá deal xoá luôn team_members. |
| Status pipeline | OPEN -> NEGOTIATING -> WON/LOST. Không validate thứ tự (có thể nhảy cóc). |

---

## 7. Tích hợp ngang

- **Customer**: mỗi deal gắn với 1 customer.
- **Team Assignment**: OWNER + MEMBER. Cho phép nhiều nhân viên cùng phụ trách.
- **Task/Note/Calendar**: gắn `LinkedEntityType="Deal"`, `LinkedEntityId=IdDeal`.
- **Report**: 
  - `DashboardResponse.TotalRevenue` = sum `Price` của deal WON.
  - `RevenueChart` theo ngày/tháng.
  - `PipelineFunnel` đếm deal theo từng trạng thái.
  - `StaffPerformance`: đếm `WonDeals`, `LostDeals`, `WinRate`, `TotalRevenue`.
- **Export**: `ExportDealsReportAsync` xuất Excel.
- **Chat AI**: CRMie truy vấn deal list để tóm tắt cho nhân viên.
