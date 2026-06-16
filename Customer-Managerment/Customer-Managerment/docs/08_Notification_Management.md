# NHÓM 8: QUẢN LÝ THÔNG BÁO (Notification Management)

## Mục tiêu
Trung tâm thông báo trong hệ thống: tiếp nhận các sự kiện từ Task, Note, Calendar, Contact, Deal, Mention... và phát realtime tới từng nhân viên qua SignalR.

---

## 1. Database Schema

```csharp
Notification:
  IdNotification (PK, Guid)
  Title     (NVARCHAR)
  Message   (NVARCHAR)
  Type      (NVARCHAR)  // Xem bảng bên dưới
  IsRead    (BOOL)
  IsPinned  (BOOL)
  CreatedAt
  IdStaff   (FK -> Person Staff, người nhận)
  RelatedEntityType (NVARCHAR, nullable)
  RelatedEntityId   (Guid, nullable)
```

---

## 2. Constants (`NotificationTypeConstant`)

| Constant | Trigger |
|----------|---------|
| `TASK_ASSIGNED` | Được giao task mới |
| `TASK_COMPLETED` | Task hoàn thành (gửi cho tất cả ADMIN) |
| `DEAL_UPDATED` | (Dự phòng) Deal được cập nhật |
| `CONTACT_STATUS_CHANGED` | (Dự phòng) Contact đổi status |
| `MENTION` | Được mention trong note |
| `SYSTEM` | Hệ thống (calendar participant, etc.) |
| `REMINDER` | Reminder calendar event |

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/NotificationHandler.cs`
- `Application/Interfaces/INotificationRepository.cs`

### API Layer
- `Api/Mutation/NotificationMutation.cs`
- `Api/Query/NotificationQuery.cs`

### Realtime
- `Api/Hubs/NotificationHub.cs` (SignalR) — group `staff_{idStaff}`.
- `Api/Services/RealtimeNotificationService.cs` — gửi `ReceiveNotification`.

### Background
- `Infrastructure/Services/CalendarReminderService.cs` — mỗi 5 phút quét event sắp tới và tạo notification `REMINDER`.

---

## 4. GraphQL Endpoints

### Mutations
- `markAsRead(idNotification: UUID!): Boolean!`
- `markAllAsRead(idStaff: UUID!): Boolean!`
- `pinNotification(idNotification: UUID!): Boolean!`
- `deleteNotification(idNotification: UUID!): Boolean!`
- `createNotification(idStaff, title, message, type, relatedEntityType?, relatedEntityId?): NotificationResponse!` — **nội bộ**, dùng từ các handler khác.

### Queries
- `getNotificationById(idNotification: UUID!): NotificationResponse`
- `getNotifications(idStaff: UUID!, filter, sort): [NotificationResponse!]!`
- `getUnreadNotifications(idStaff: UUID!, filter, sort): [NotificationResponse!]!`
- `getPinnedNotifications(idStaff: UUID!, filter, sort): [NotificationResponse!]!`
- `getUnreadCount(idStaff: UUID!): Int!`

### SignalR Hub (`/hubs/notifications`)
- `joinStaffGroup(idStaff)` — join group `staff_{idStaff}`.
- `leaveStaffGroup(idStaff)`.
- Server push: `ReceiveNotification` event với payload `NotificationResponse`.

---

## 5. Luồng nghiệp vụ

### 5.1. Luồng tạo Notification (cách các module khác trigger)
Có 2 cách:

**Cách A — Qua `NotificationHandler` (không realtime):**
- Trong `TaskHandler.CreateTaskAsync` / `AssignTaskAsync` / `UpdateTaskStatusAsync` -> tạo `Notification` entity -> `_notificationRepository.AddNotificationAsync`.
- Không gọi realtime (chỉ lưu DB).
- Client phải poll hoặc refresh.

**Cách B — Qua `NotificationMutation.CreateNotification` (có realtime):**
- Tạo `Notification` entity + lưu DB.
- **Gọi `IRealtimeNotificationService.SendNotificationToStaffAsync(idStaff, response)`** -> broadcast SignalR tới group `staff_{idStaff}`.
- Hiện chỉ `CalendarMutation.AddParticipantAsync` dùng cách này.

### 5.2. Calendar Reminder (background)
1. `CalendarReminderService` chạy mỗi 5 phút.
2. Tìm event:
   - `IsDeleted = false`
   - `Status = SCHEDULED`
   - `ReminderMinutes > 0`
   - `StartTime` nằm trong cửa sổ 5-10 phút tới.
3. Với mỗi event:
   - Check đã gửi reminder chưa (query Notification có `RelatedEntityId=IdEvent`, `Type=REMINDER`, `CreatedAt > now - 10p`).
   - Nếu chưa -> gửi notification cho organizer (IdStaff của event) và từng participant:
     - Title: "Nhắc nhở: {title}" (organizer) hoặc "Nhắc nhở tham gia: {title}" (participant).
     - Message: "Sự kiện của bạn sẽ bắt đầu trong {reminderMinutes} phút. Loại: {eventType}. Thời gian: {HH:mm}".
     - Type: `REMINDER`.
4. **Không gọi realtime push** (chỉ lưu DB).

### 5.3. Mark as Read
- `MarkAsRead(id)` -> set `IsRead = true`. Throw exception nếu repo trả false.
- `MarkAllAsRead(idStaff)` -> set tất cả notification của staff thành read.

### 5.4. Pin
- `PinNotification(id)` -> set `IsPinned = true`. Lưu ý: hiện KHÔNG có endpoint Unpin.

### 5.5. Delete
- Soft delete (`IsDeleted = true`).

### 5.6. Realtime broadcast
Khi client subscribe `NotificationHub` và join staff group:
- Mọi lần `SendNotificationToStaffAsync(idStaff, notification)` (từ `NotificationMutation.CreateNotification`) sẽ trigger event `ReceiveNotification`.

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| 1 staff = 1 group | Group `staff_{idStaff}` chỉ nhận notif của chính staff đó. |
| 2 cách tạo | Cách A (chỉ DB) cho Task/Note mention; Cách B (DB + realtime) cho Calendar. **Không nhất quán** — nên chuẩn hoá dùng Cách B cho mọi loại. |
| Reminder dedup | CalendarReminderService dùng query 10 phút gần nhất để tránh gửi trùng. |
| Hard delete | KHÔNG — chỉ soft delete. |
| Read tracking | Có `IsRead` flag, có thể query `getUnreadCount`. |

---

## 7. Tích hợp ngang

- **Task**: TASK_ASSIGNED, TASK_COMPLETED (gửi cho ADMIN khi complete).
- **Note**: MENTION (khi @username).
- **Calendar**: SYSTEM (khi add participant), REMINDER (background job).
- **Contact/Deal**: hiện KHÔNG tự tạo notification (các constant `DEAL_UPDATED`, `CONTACT_STATUS_CHANGED` đã định nghĩa nhưng chưa được dùng).
- **Realtime**: SignalR group theo staff.
