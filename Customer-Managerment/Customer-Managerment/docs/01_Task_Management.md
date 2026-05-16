# NHÓM 1: QUẢN LÝ CÔNG VIỆC (Task Management)

## Mục tiêu
Xây dựng hệ thống quản lý công việc cho phép Admin tạo/gán task và Staff theo dõi tiến độ.

---

## 1. Database Schema

### Task Entity
```
Task:
- IdTask (PK, Guid)
- Title (NVARCHAR200, NOT NULL)
- Description (TEXT, NULLABLE)
- DueDate (DATETIME, NULLABLE)
- Priority (INT) -- 0=LOW, 1=MEDIUM, 2=HIGH, 3=URGENT
- Status (INT) -- 0=PENDING, 1=IN_PROGRESS, 2=COMPLETED, 3=CANCELLED
- CreatedAt, UpdatedAt (audit)
- IsDeleted, DeletedAt (soft delete)
- IdStaffAssigned (FK -> persons.id) -- Staff được gán
- LinkedEntityType (NVARCHAR50) -- "Lead", "Customer", "Deal", NULL
- LinkedEntityId (GUID, NULLABLE) -- Id của Lead/Customer/Deal
```

### Constants
```
TaskPriorityConstant: LOW, MEDIUM, HIGH, URGENT
TaskStatusConstant: PENDING, IN_PROGRESS, COMPLETED, CANCELLED
TaskLinkedEntityConstant: Lead, Customer, Deal
```

---

## 2. Files cần tạo mới

### Domain Layer
- `Domain/Entities/Task.cs` - Entity class
- `Domain/Constant/TaskPriorityConstant.cs`
- `Domain/Constant/TaskStatusConstant.cs`
- `Domain/Constant/TaskLinkedEntityConstant.cs`
- `Domain/Exception/TaskNotFoundException.cs`

### Application Layer
- `DTOs/Requests/TaskCreationRequest.cs`
- `DTOs/Requests/TaskUpdateRequest.cs`
- `DTOs/Response/TaskResponse.cs`
- `Interfaces/ITaskRepository.cs`
- `UseCases/TaskHandler.cs`

### Infrastructure Layer
- `Repositories/TaskRepository.cs`
- `Mapping/TaskMapper.cs`

### API Layer
- `Input/Type/TaskInputType.cs`
- `Input/Type/Enums/TaskPriorityType.cs`
- `Input/Type/Enums/TaskStatusType.cs`
- `Input/Type/Enums/TaskLinkedEntityType.cs`
- `Query/TaskQuery.cs`
- `Mutation/TaskMutation.cs`

---

## 3. API Endpoints

### Mutations
- `createTask(TaskInput)` - Admin tạo task mới
- `updateTask(TaskUpdateInput, idTask)` - Admin cập nhật task
- `deleteTask(idTask)` - Admin xóa task (soft delete)
- `restoreTask(idTask)` - Admin khôi phục task
- `assignTask(idTask, idStaff)` - Admin gán task cho staff
- `updateTaskStatus(idTask, status)` - Staff cập nhật tiến độ

### Queries
- `getTasks` - Admin xem tất cả task
- `getTasksByStaff(idStaff)` - Lấy task của một staff
- `getTasksByStatus(status)` - Lọc task theo status
- `getTaskById(idTask)` - Chi tiết một task

---

## 4. Business Rules

### Quyền hạn
- **ADMIN**: Tạo, sửa, xóa, gán task cho bất kỳ staff nào
- **STAFF**: Chỉ cập nhật status của task được gán cho mình

### Validation
- Title: required, max 200 chars
- DueDate: nếu có, phải >= hiện tại
- Priority: 0-3
- Status: 0-3
- IdStaffAssigned: phải là Staff (discriminator=0)

### Triggers
- Khi gán task -> tạo Notification cho Staff
- Khi cập nhật status COMPLETED -> thông báo cho Admin

---

## 5. Implementation Order

1. Tạo Entity + Constants + Exceptions
2. Tạo Repository Interface + Implementation
3. Tạo DTOs + Mapper
4. Tạo Handler
5. Tạo GraphQL Input Types + Enums
6. Tạo Query + Mutation
7. Đăng ký services trong Program.cs
8. Tạo migration

---

## 6. Priority Files để sửa

- `CustomerManagement.Api/Program.cs` - đăng ký TaskHandler, TaskRepository, GraphQL types
- `CustomerManagement.Infrastructure/Data/CustomerManagementDbContext.cs` - thêm DbSet<Task>

---

## 7. Bug Fixes

### 2026-05-16: DateTime Parsing Error in TaskInput

**Issue:** `updateTask` mutation threw `DateTime cannot parse the given literal of type StringValueNode` when `dueDate` was provided.

**Root Cause:** `TaskInputType.cs` defined `DueDate` as `DateTime?` which Hot Chocolate cannot parse from GraphQL string literal.

**Fix:** Changed `DueDate` from `DateTime?` to `string?` in both `TaskInput` and `TaskUpdateInput`, then parse in mutation handlers:

```csharp
// TaskInputType.cs
public string? DueDate { get; set; }

// TaskMutation.cs
DateTime? dueDate = null;
if (!string.IsNullOrEmpty(input.DueDate) && DateTime.TryParse(input.DueDate, out var parsed))
{
    dueDate = parsed;
}
```

**Files Modified:**
- `CustomerManagement.Api\Input\Type\TaskInputType.cs`
- `CustomerManagement.Api\Mutation\TaskMutation.cs`

---

### 2026-05-16: Enum Value Format Requirement

**Issue:** Client sent `priority: "0"` (string number) but Hot Chocolate enum expects enum name like `LOW`, `MEDIUM`, `HIGH`, `URGENT`.

**Root Cause:** GraphQL enum input requires exact enum member names, not integer values.

**Fix (Client-side):** Change the variable format from number string to enum name:

```json
// Incorrect - will cause error
{ "priority": "0", "status": "0" }

// Correct
{ "priority": "LOW", "status": "PENDING" }
```

**Valid enum values:**

| TaskPriority | TaskItemStatus |
|--------------|----------------|
| LOW | PENDING |
| MEDIUM | IN_PROGRESS |
| HIGH | COMPLETED |
| URGENT | CANCELLED |