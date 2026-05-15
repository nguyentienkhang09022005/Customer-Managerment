# NHÓM 3: THÔNG BÁO (Notifications)

## Mục tiêu
Hệ thống thông báo real-time cho Staff khi có thay đổi liên quan đến công việc, @mention, hoặc cập nhật trên Lead/Customer/Deal.

---

## 1. Database Schema

### Notification Entity
```
Notification:
- IdNotification (PK, Guid)
- Title (NVARCHAR200, NOT NULL)
- Message (TEXT, NOT NULL)
- Type (NVARCHAR50) -- TASK_ASSIGNED, TASK_COMPLETED, DEAL_UPDATED,
                      CONTACT_STATUS_CHANGED, MENTION, SYSTEM
- IsRead (BOOLEAN, default: false)
- CreatedAt (DATETIME)
- IdStaff (FK -> persons.id) -- Người nhận thông báo
- RelatedEntityType (NVARCHAR50, NULLABLE) -- "Task", "Lead", "Customer", "Deal", "Note"
- RelatedEntityId (GUID, NULLABLE)
- IsPinned (BOOLEAN) -- Ghim thông báo quan trọng
```

### NotificationSettings Entity (tuỳ chọn)
```
NotificationSettings:
- IdSettings (PK, Guid)
- IdStaff (FK -> persons.id)
- EnableTaskNotification (BOOLEAN, default: true)
- EnableDealNotification (BOOLEAN, default: true)
- EnableMentionNotification (BOOLEAN, default: true)
- EnableEmailNotification (BOOLEAN, default: false)
```

---

## 2. Files cần tạo mới

### Domain Layer
- `Domain/Entities/Notification.cs`
- `Domain/Entities/NotificationSettings.cs`
- `Domain/Constant/NotificationTypeConstant.cs`
- `Domain/Exception/NotificationNotFoundException.cs`

### Application Layer
- `DTOs/Requests/NotificationCreationRequest.cs` (internal)
- `DTOs/Response/NotificationResponse.cs`
- `DTOs/Response/NotificationListResponse.cs`
- `Interfaces/INotificationRepository.cs`
- `UseCases/NotificationHandler.cs`

### Infrastructure Layer
- `Repositories/NotificationRepository.cs`
- `Mapping/NotificationMapper.cs`

### API Layer
- `Input/Type/Enums/NotificationType.cs`
- `Query/NotificationQuery.cs`
- `Mutation/NotificationMutation.cs`
- Middleware for real-time notification (SignalR - tương lai)

---

## 3. API Endpoints

### Mutations
- `createNotification(NotificationInput)` - Internal only (tự động gọi)
- `markAsRead(idNotification)` - Đánh dấu đã đọc
- `markAllAsRead(idStaff)` - Đánh dấu tất cả đã đọc
- `deleteNotification(idNotification)` - Xóa thông báo
- `pinNotification(idNotification)` - Ghim thông báo

### Queries
- `getNotifications(idStaff)` - Lấy thông báo của staff
- `getUnreadNotifications(idStaff)` - Lấy thông báo chưa đọc
- `getNotificationCount(idStaff)` - Đếm số thông báo chưa đọc
- `getPinnedNotifications(idStaff)` - Lấy thông báo được ghim

---

## 4. Trigger Events

### Tự động tạo Notification khi:

| Sự kiện | Type | Người nhận | Related Entity |
|---------|------|------------|----------------|
| Task được gán | TASK_ASSIGNED | Assigned Staff | Task |
| Task hoàn thành | TASK_COMPLETED | Admin | Task |
| Deal status đổi | DEAL_UPDATED | Admin | Deal |
| Contact status đổi | CONTACT_STATUS_CHANGED | Admin | Contact |
| @mention trong Note | MENTION | Mentioned Staff | Note |
| Lead được tạo | SYSTEM | Admin | Lead |
| Customer được tạo | SYSTEM | Admin | Customer |

---

## 5. Business Rules

### Notifications per Staff
- Lưu trữ tối đa 100 thông báo gần nhất
- Tự động xóa thông báo cũ (oldest unread first)

### Read Status
- Mặc định: chưa đọc
- Khi click vào notification -> mark as read
- Có thể cấu hình tự động đọc sau X ngày

### Real-time via SignalR

**SignalR Hub:** `NotificationHub` at `/hubs/notifications`

**Methods:**
- `JoinStaffGroup(Guid idStaff)` - Subscribe to personal notifications
- `LeaveStaffGroup(Guid idStaff)` - Unsubscribe

**Service:** `IRealtimeNotificationService` → `RealtimeNotificationService`
- `SendNotificationToStaffAsync(Guid idStaff, NotificationResponse notification)` - Send to specific staff
- `BroadcastNotificationAsync(NotificationResponse notification)` - Broadcast to all

**Integration:**
- NotificationMutation uses `IRealtimeNotificationService` to send real-time notifications
- CalendarMutation sends notification to participants when events are created/updated
- CalendarReminderService sends reminder notifications via SignalR

---

### Real-time (tương lai)
- Dùng SignalR cho real-time notification
- Hoặc polling mỗi 30 giây

---

## 6. Integration

### Với Task Management (Nhóm 1)
- TASK_ASSIGNED: Khi Admin gán task
- TASK_COMPLETED: Khi Staff hoàn thành task

### Với Activity Notes (Nhóm 2)
- MENTION: Khi Staff được @mention trong note

### Với Deal/Contact (hiện có)
- DEAL_UPDATED: Khi Deal status thay đổi
- CONTACT_STATUS_CHANGED: Khi Contact status thay đổi

---

## 7. Implementation Order

1. Entity + Constants + Exceptions
2. Repository + Mapper
3. Handler
4. GraphQL types + Query + Mutation
5. Integration vào existing handlers (ContactHandler, DealHandler, TaskHandler)
6. Đăng ký services + Migration