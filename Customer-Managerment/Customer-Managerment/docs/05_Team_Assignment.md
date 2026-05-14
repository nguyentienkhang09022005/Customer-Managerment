hay# NHÓM 5: PHÂN CÔNG NHÓM (Team Assignment)

## Mục tiêu
Cho phép gán nhiều Staff vào một Lead hoặc Deal, chia sẻ thông tin và công việc trong nhóm.

---

## 1. Database Schema

### TeamMember Entity
```
TeamMember:
- Id (PK, Guid)
- EntityType (NVARCHAR50) -- "Lead", "Deal"
- EntityId (GUID) -- Id của Lead hoặc Deal
- IdStaff (FK -> persons.id) -- Staff trong nhóm
- Role (NVARCHAR50) -- "OWNER", "MEMBER", "VIEWER"
- AssignedAt (DATETIME)
- AssignedBy (NVARCHAR100) -- Staff gán
- CanEdit (BOOLEAN) -- Quyền chỉnh sửa
- CanDelete (BOOLEAN) -- Quyền xóa
```

### Note: Thay đổi FK trong Lead/Deal

Cần thêm:
- Deal: IdStaff chính (OWNER) - đã có
- Deal: Có thể có nhiều team members - qua TeamMember

---

## 2. Files cần tạo mới

### Domain Layer
- `Domain/Entities/TeamMember.cs`
- `Domain/Constant/TeamRoleConstant.cs`
- `Domain/Constant/TeamEntityTypeConstant.cs`
- `Domain/Exception/TeamMemberNotFoundException.cs`

### Application Layer
- `DTOs/Requests/AddTeamMemberRequest.cs`
- `DTOs/Requests/UpdateTeamMemberRequest.cs`
- `DTOs/Response/TeamMemberResponse.cs`
- `DTOs/Response/TeamResponse.cs`
- `Interfaces/ITeamMemberRepository.cs`
- `UseCases/TeamAssignmentHandler.cs`

### Infrastructure Layer
- `Repositories/TeamMemberRepository.cs`
- `Mapping/TeamMemberMapper.cs`

### API Layer
- `Input/Type/TeamMemberInputType.cs`
- `Input/Type/Enums/TeamRoleType.cs`
- `Input/Type/Enums/TeamEntityType.cs`
- `Query/TeamAssignmentQuery.cs`
- `Mutation/TeamAssignmentMutation.cs`

---

## 3. API Endpoints

### Mutations
- `addTeamMember(entityType, entityId, idStaff, role)` - Thêm staff vào nhóm
- `updateTeamMember(idTeamMember, role, canEdit, canDelete)` - Cập nhật quyền
- `removeTeamMember(idTeamMember)` - Xóa khỏi nhóm
- `transferOwnership(entityType, entityId, newOwnerId)` - Chuyển quyền sở hữu

### Queries
- `getTeamMembers(entityType, entityId)` - Lấy danh sách team
- `getMyTeams(idStaff)` - Lấy tất cả teams của một staff
- `getTeamMemberPermissions(idTeamMember)` - Lấy chi tiết quyền

---

## 4. Business Rules

### Roles
- **OWNER**: Chủ sở hữu - có full permissions
- **MEMBER**: Thành viên - có thể cập nhật
- **VIEWER**: Người xem - chỉ đọc

### Permissions
- OWNER có thể: thêm/xóa member, chỉnh sửa, xóa entity
- MEMBER có thể: xem, cập nhật (nếu CanEdit=true)
- VIEWER: chỉ xem

### Constraints
- Mỗi entity (Lead/Deal) phải có ít nhất 1 OWNER
- OWNER đầu tiên là người tạo entity
- Không thể xóa OWNER cuối cùng nếu không có MEMBER thay thế

---

## 5. Integration với existing entities

### Lead/Deal - có IdStaff (OWNER)
- Khi tạo Lead/Deal: tự động thêm creator làm OWNER
- Có thể query: `GetLeadsByTeamMember(idStaff)` - leads mà staff là thành viên

### Task - có IdStaffAssigned
- Tasks vẫn assign cho 1 người, nhưng team members có thể xem

### Permission Check
- Khi update Lead/Deal: kiểm tra có phải OWNER/MEMBER với CanEdit=true
- Khi delete: kiểm tra OWNER hoặc CanDelete=true

---

## 6. Implementation Order

1. Entity + Constants + Exceptions
2. Repository + Mapper
3. Handler
4. GraphQL types + Query + Mutation
5. Cập nhật LeadHandler/DealHandler để kiểm tra permissions
6. Đăng ký services + Migration