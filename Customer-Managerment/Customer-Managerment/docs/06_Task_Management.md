# NHÓM 6: QUẢN LÝ CÔNG VIỆC (Task Management)

## Mục tiêu
Giao việc cho nhân viên, theo dõi tiến độ, đính kèm với entity nghiệp vụ (Lead/Customer/Deal). Khi hoàn thành sẽ thông báo cho toàn bộ ADMIN.

---

## 1. Database Schema

```csharp
TaskEntity:
  IdTask (PK, Guid)
  Title       (NVARCHAR 200, required)
  Description (NVARCHAR, nullable)
  DueDate     (DATETIME, nullable)
  Priority    (INT)  // 0=LOW, 1=MEDIUM, 2=HIGH, 3=URGENT
  Status      (INT)  // 0=PENDING, 1=IN_PROGRESS, 2=COMPLETED, 3=CANCELLED
  CreatedAt, UpdatedAt
  IsDeleted, DeletedAt
  IdStaffAssigned (FK -> Person Staff)
  LinkedEntityType (NVARCHAR, nullable)  // "Lead" | "Customer" | "Deal"
  LinkedEntityId   (Guid, nullable)
```

---

## 2. Constants

### `TaskStatusConstant`
| Value | Constant |
|-------|----------|
| 0 | PENDING |
| 1 | IN_PROGRESS |
| 2 | COMPLETED |
| 3 | CANCELLED |

### `TaskPriorityConstant`
| Value | Constant |
|-------|----------|
| 0 | LOW |
| 1 | MEDIUM |
| 2 | HIGH |
| 3 | URGENT |

### `TaskLinkedEntityConstant`
- "Lead"
- "Customer"
- "Deal"

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/TaskHandler.cs` (kèm extension `TaskStatusConstantExtensions`).
- `Application/Interfaces/ITaskRepository.cs`

### API Layer
- `Api/Mutation/TaskMutation.cs`
- `Api/Query/TaskQuery.cs`
- `Api/Input/Type/TaskInputType.cs`

---

## 4. GraphQL Endpoints

### Mutations
- `createTask(input: TaskInput!): TaskResponse!`
- `updateTask(input: TaskUpdateInput!, idTask: UUID!): TaskResponse!`
- `deleteTask(idTask: UUID!): Boolean!`
- `restoreTask(idTask: UUID!): TaskResponse!`
- `assignTask(idTask: UUID!, idStaff: UUID!): TaskResponse!` — giao lại.
- `updateTaskStatus(idTask: UUID!, status: Int!): TaskResponse!` — cập nhật nhanh status.

### Queries
- `getTasks(filter, sort): [TaskResponse!]!`
- `getTaskById(idTask: UUID!): TaskResponse`
- `getTasksByStaff(idStaff: UUID!, filter, sort): [TaskResponse!]!`
- `getTasksByStatus(status: Int!, filter, sort): [TaskResponse!]!`

---

## 5. Luồng nghiệp vụ

### 5.1. Tạo Task
1. `TaskMutation.CreateTaskAsync(input)` parse `DueDate` từ string -> DateTime? (nếu parse được).
2. Map -> `TaskCreationRequest`.
3. `TaskHandler.CreateTaskAsync`:
   - `ValidateTaskCreation`:
     - `Title` required, max 200.
     - `DueDate` nếu có phải >= `DateTime.UtcNow` (so với thời điểm validate).
     - `Priority` ∈ [0, 3].
     - `LinkedEntityType` nếu có phải ∈ {Lead, Customer, Deal}.
   - `StaffRepository.GetStaffByIdAsync(IdStaffAssigned)` — throw `StaffNotFoundException`.
   - AutoMapper -> `TaskEntity`. **Set `Status = PENDING` (int 0).**
   - `AddTaskAsync`.
   - **Tạo Notification** cho staff được giao:
     - Title: "Bạn được giao công việc mới"
     - Message: "Bạn được giao công việc: {title}"
     - Type: `TASK_ASSIGNED`
     - `IdStaff = IdStaffAssigned`
     - `RelatedEntityType="Task"`, `RelatedEntityId=IdTask`.
   - Lưu notification vào DB.
4. Trả về `TaskResponse`.

### 5.2. Cập nhật Task
- Cập nhật các field non-null trong request. Tương tự validate như Create.
- Nếu đổi `IdStaffAssigned` thì KHÔNG tự động tạo notification mới (chỉ mutation `assignTask` riêng mới gửi notif).

### 5.3. Giao lại Task (`assignTask`)
1. Lấy task. Throw `TaskNotFoundException` nếu rỗng.
2. Kiểm tra staff mới tồn tại.
3. Cập nhật `IdStaffAssigned = idStaff`.
4. **Tạo Notification** cho staff mới (giống tạo mới).

### 5.4. Cập nhật nhanh Status (`updateTaskStatus`)
1. Lấy task.
2. Validate `status ∈ [0, 3]`.
3. Cập nhật `Status`, `UpdatedAt`.
4. **Nếu `Status == 2 (COMPLETED)`:** lấy tất cả staff có `Role = "ADMIN"`, tạo notification cho từng admin:
   - Title: "Công việc hoàn thành"
   - Message: "Công việc '{title}' đã được hoàn thành"
   - Type: `TASK_COMPLETED`.

### 5.5. Xoá & Khôi phục
- Soft delete / restore. Không cascade các entity liên quan.

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Auto status khi tạo | Task mới luôn ở `PENDING`. |
| DueDate validation | Phải >= thời điểm validate (lưu ý clock skew giữa client/server). |
| Notification khi giao | Mỗi lần `createTask` hoặc `assignTask` đều tạo notification. |
| Notification khi hoàn thành | Tất cả ADMIN đều nhận được. STAFF/MANAGER không nhận. |
| LinkedEntity | Chỉ chấp nhận Lead/Customer/Deal. Không link được Note/Task khác. |
| Không check permission | Code hiện KHÔNG check role khi tạo/sửa task — bất kỳ ai cũng có thể tạo task cho staff khác. Cần bổ sung. |

---

## 7. Tích hợp ngang

- **Notification**: Task tạo/giao/hoàn thành đều phát sinh notification.
- **Note/Deal/Lead/Customer**: có thể liên kết qua `LinkedEntityType/Id` (chỉ tham chiếu, không cascade).
- **Calendar**: Có thể tạo event loại `TASK_DEADLINE` tham chiếu task.
- **Report**: `StaffPerformance.TasksCompleted` hiện đang hardcode = 0 (chưa tính từ task).
