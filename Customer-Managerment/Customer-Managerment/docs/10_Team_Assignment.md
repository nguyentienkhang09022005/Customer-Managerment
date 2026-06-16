# NHÓM 10: PHÂN CÔNG NHÓM (Team Assignment)

## Mục tiêu
Quản lý đội ngũ nhân viên cùng phụ trách một entity nghiệp vụ (Lead hoặc Deal). Hỗ trợ phân quyền chi tiết (OWNER/MEMBER/VIEWER + CanEdit/CanDelete), chuyển quyền sở hữu, ngăn xoá OWNER cuối cùng.

---

## 1. Database Schema

```csharp
TeamMember:
  Id (PK, Guid)
  EntityType   (NVARCHAR)  // "Lead" | "Deal"
  EntityId     (Guid)
  IdStaff      (FK -> Person Staff)
  Role         (INT)       // 0=OWNER, 1=MEMBER, 2=VIEWER
  AssignedAt   (DATETIME)
  AssignedBy   (NVARCHAR, nullable)  // Tên người gán
  CanEdit      (BOOL)
  CanDelete    (BOOL)
```

---

## 2. Constants

### `TeamEntityTypeConstant`
- `Lead`
- `Deal`

### `TeamRoleConstant`
| Value | Constant | Quyền mặc định |
|-------|----------|-----------------|
| 0 | OWNER | Toàn quyền |
| 1 | MEMBER | Xem + sửa (tuỳ CanEdit) |
| 2 | VIEWER | Chỉ xem |

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/TeamAssignmentHandler.cs`
- `Application/Interfaces/ITeamMemberRepository.cs`

### API Layer
- `Api/Mutation/TeamAssignmentMutation.cs`
- `Api/Query/TeamAssignmentQuery.cs`
- `Api/Input/Type/TeamMemberInputType.cs`, `Enums/TeamRoleType.cs`

### Tự động
- `DealHandler.CreateDealAsync` tự thêm OWNER khi tạo deal.

---

## 4. GraphQL Endpoints

### Mutations
- `addTeamMember(input: AddTeamMemberInput!): TeamMemberResponse!`
- `updateTeamMember(input: UpdateTeamMemberInput!, idTeamMember: UUID!): TeamMemberResponse!`
- `removeTeamMember(idTeamMember: UUID!): Boolean!`
- `transferOwnership(entityType: String!, entityId: UUID!, newOwnerId: UUID!): TeamMemberResponse!`

### Queries
- `getTeamMembers(entityType: String!, entityId: UUID!): [TeamMemberResponse!]!`
- `getMyTeams(idStaff: UUID!): [TeamMemberResponse!]!`
- `getTeamMemberPermissions(idTeamMember: UUID!): TeamMemberResponse`

---

## 5. Luồng nghiệp vụ

### 5.1. Thêm Team Member
1. `ValidateAddRequest`:
   - `EntityType` ∈ {Lead, Deal}.
   - `Role` ∈ {OWNER, MEMBER, VIEWER}.
2. `StaffRepository.GetStaffByIdAsync(IdStaff)` — throw `StaffNotFoundException`.
3. `ValidateEntityExistsAsync(EntityType, EntityId)`:
   - Nếu "Lead" -> check `LeadRepository.GetLeadByIdAsync`.
   - Nếu "Deal" -> check `DealRepository.GetDealByIdAsync`.
   - Throw `LeadNotFoundException` / `DealNotFoundException`.
4. Check staff đã là member chưa — throw `ConflictException`.
5. Tạo `TeamMember`:
   - `Role = int mapping từ string` (OWNER=0, MEMBER=1, VIEWER=2).
   - `AssignedBy = currentUserName` (lấy từ `ClaimTypes.Name`).
   - `CanEdit`, `CanDelete` lấy từ request.
   - `AssignedAt = now`.
6. `AddAsync`. Trả về `TeamMemberResponse`.

### 5.2. Cập nhật Team Member
1. Lấy member. Throw `TeamMemberNotFoundException`.
2. Update `Role` (nếu có), `CanEdit` (nếu có), `CanDelete` (nếu có).
3. Lưu. Trả về response.

### 5.3. Xoá Team Member
1. Lấy member.
2. **Nếu là OWNER (Role = 0):** đếm tổng member trong team. Nếu chỉ còn 1 -> throw `BusinessRuleException("Không thể xoá OWNER cuối cùng!")`.
3. `RemoveAsync`. Trả về true.

### 5.4. Chuyển quyền sở hữu
1. Validate entity tồn tại.
2. Lấy staff mới. Throw `StaffNotFoundException`.
3. Lấy member mới trong team — nếu không có throw `NotFoundException("OWNER mới phải là thành viên của nhóm!")`.
4. Với mọi member hiện tại:
   - Nếu `Role = 0` (OWNER) -> downgrade thành `MEMBER` (1).
5. Set member mới:
   - `Role = 0` (OWNER).
   - `CanEdit = true`, `CanDelete = true`.
6. Lưu tất cả. Trả về response của OWNER mới.

> **Lưu ý:** Nếu chuyển OWNER cho 1 người CHƯA là member thì throw lỗi. Phải addTeamMember trước rồi mới transferOwnership.

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Auto OWNER khi tạo Deal | DealHandler tự tạo TeamMember { Role=OWNER, CanEdit=true, CanDelete=true } cho người tạo. |
| Bảo vệ OWNER cuối | Không thể xoá member duy nhất có Role=OWNER. |
| Chuyển OWNER | OWNER mới phải là member hiện tại. Mọi OWNER cũ thành MEMBER. |
| EntityType giới hạn | Chỉ hỗ trợ Lead và Deal. KHÔNG có cho Customer, Task, Note, Calendar. |
| Phân quyền CanEdit/CanDelete | Cho phép tuỳ biến — OWNER mặc định true, MEMBER mặc định false. Hiện chưa có GraphQL check quyền dựa trên flag này. |

---

## 7. Tích hợp ngang

- **Deal**: auto OWNER khi tạo; deal dùng team để phân quyền xem (`getMyDeals` lọc theo team).
- **Lead**: có thể thêm nhiều staff cùng phụ trách.
- **Cascade delete**: xoá Lead/Deal sẽ xoá toàn bộ `TeamMember` của entity đó (qua `RemoveByEntityAsync`).
- **Audit**: KHÔNG tự động ghi audit log cho team assignment.
