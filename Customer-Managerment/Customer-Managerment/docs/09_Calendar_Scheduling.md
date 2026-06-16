# NHÓM 9: QUẢN LÝ LỊCH HẸN (Calendar & Scheduling)

## Mục tiêu
Quản lý sự kiện lịch (meeting, call, deadline, follow-up), mời người tham gia, theo dõi phản hồi, gửi reminder tự động trước khi sự kiện bắt đầu.

---

## 1. Database Schema

```csharp
CalendarEvent:
  IdEvent (PK, Guid)
  Title        (NVARCHAR 200, required)
  Description  (NVARCHAR, nullable)
  EventType    (INT)   // 0=MEETING, 1=CALL, 2=TASK_DEADLINE, 3=FOLLOW_UP
  StartTime, EndTime (DATETIME, EndTime >= StartTime)
  Location     (NVARCHAR, nullable)
  IsAllDay     (BOOL)
  ReminderMinutes (INT?, nullable) -- phút trước StartTime
  Status       (INT)   // 0=SCHEDULED, 1=IN_PROGRESS, 2=COMPLETED, 3=CANCELLED
  CreatedAt, UpdatedAt
  IsDeleted, DeletedAt
  IdStaff (FK -> Person Staff, người tạo/organizer)
  RelatedEntityType (NVARCHAR, nullable)
  RelatedEntityId   (Guid, nullable)

EventParticipant:
  Id (PK, Guid)
  IdEvent (FK -> CalendarEvent)
  IdStaff (FK -> Person Staff)
  Status     (INT)  // 0=PENDING, 1=ACCEPTED, 2=DECLINED, 3=TENTATIVE
  RespondedAt (DATETIME?, nullable)
```

---

## 2. Constants

### `CalendarEventTypeConstant`
- `MEETING`, `CALL`, `TASK_DEADLINE`, `FOLLOW_UP` (0-3)

### `CalendarEventStatusConstant`
- `SCHEDULED`, `IN_PROGRESS`, `COMPLETED`, `CANCELLED` (0-3)

### `ParticipantStatusConstant`
- `PENDING`, `ACCEPTED`, `DECLINED`, `TENTATIVE` (0-3)

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/CalendarHandler.cs`
- `Application/Interfaces/ICalendarEventRepository.cs`, `IEventParticipantRepository.cs`

### API Layer
- `Api/Mutation/CalendarMutation.cs` — emit realtime notification.
- `Api/Query/CalendarQuery.cs`
- `Api/Input/Type/CalendarEventInputType.cs`

### Realtime
- `Api/Services/RealtimeNotificationService.cs` — gửi notif khi add participant.

### Background
- `Infrastructure/Services/CalendarReminderService.cs` — chạy mỗi 5 phút, tạo notification `REMINDER` cho organizer + participants khi sự kiện sắp tới trong 5-10 phút.

---

## 4. GraphQL Endpoints

### Mutations
- `createCalendarEvent(input: CalendarEventInput!): CalendarEventResponse!`
- `updateCalendarEvent(input: CalendarEventUpdateInput!, idEvent: UUID!): CalendarEventResponse!`
- `deleteCalendarEvent(idEvent: UUID!): Boolean!`
- `cancelCalendarEvent(idEvent: UUID!): Boolean!`
- `addParticipant(input: EventParticipantInput!): CalendarEventResponse!` — tạo participant + notification + realtime push.
- `updateParticipantStatus(idParticipant: UUID!, status: Int!): EventParticipantResponse!`
- `removeParticipant(idParticipant: UUID!): Boolean!`

### Queries
- `getCalendarEvents(fromDate, toDate, idStaff?): [CalendarEventResponse!]!`
- `getCalendarEventById(idEvent: UUID!): CalendarEventResponse`
- `getMyEvents(idStaff: UUID!, fromDate, toDate): [CalendarEventResponse!]!`
- `getUpcomingEvents(idStaff: UUID!, days: Int!): [CalendarEventResponse!]!` (top 20).
- `getEventParticipants(idEvent: UUID!): [EventParticipantResponse!]!`
- `getEventsByEntity(entityType: String!, entityId: UUID!): [CalendarEventResponse!]!`

---

## 5. Luồng nghiệp vụ

### 5.1. Tạo Event
1. `ValidateCreation`:
   - `Title` required, max 200.
   - `EndTime >= StartTime`.
   - `EventType` ∈ [0, 3].
2. `StaffRepository.GetStaffByIdAsync(IdStaff)` — throw `StaffNotFoundException`.
3. Map -> `CalendarEvent`. Set `Status = SCHEDULED` (0). Set `CreatedAt`.
4. `AddAsync`. Trả về response (KHÔNG tự động thêm organizer vào `EventParticipant` — chỉ `IdStaff` của event).

### 5.2. Cập nhật Event
- Cập nhật từng field non-null. Validate `EndTime >= StartTime` nếu cả 2 cùng được gửi.
- Validate `EventType` và `Status` ∈ [0, 3] nếu có.

### 5.3. Xoá Event
- Soft delete.

### 5.4. Cancel Event
- Set `Status = CANCELLED` (3). Không xoá participants (giữ audit).

### 5.5. Thêm Participant
1. Lấy event. Throw `CalendarEventNotFoundException`.
2. Lấy staff. Throw `StaffNotFoundException`.
3. Check participant đã tồn tại chưa — nếu có throw `ConflictException`.
4. Tạo `EventParticipant { Status = PENDING }`.
5. **Tạo Notification** trong DB:
   - Title: "Bạn được thêm vào sự kiện mới"
   - Message: "Bạn được thêm vào sự kiện: {title}"
   - Type: `SYSTEM`
   - Related: `CalendarEvent`.
6. **Realtime push** qua `IRealtimeNotificationService.SendNotificationToStaffAsync(idStaff, notification)`.
7. Trả về `CalendarEventResponse` (kèm participant mới).

### 5.6. Cập nhật trạng thái Participant
1. Lấy participant. Throw `EventParticipantNotFoundException`.
2. Validate `status ∈ [0, 3]`.
3. Set `Status`, `RespondedAt = now`.
4. Lưu. Trả về `EventParticipantResponse`.

### 5.7. Xoá Participant
- `RemoveAsync`. Throw `EventParticipantNotFoundException` nếu không tồn tại.

### 5.8. Calendar Reminder (background)
1. Mỗi 5 phút, query event:
   - `IsDeleted = false`
   - `Status = SCHEDULED`
   - `ReminderMinutes > 0`
   - `StartTime ∈ [now + 5min, now + 10min]`.
2. Với mỗi event, check đã có `Notification` với `Type=REMINDER`, `RelatedEntityId=IdEvent`, `CreatedAt > now - 10min` chưa. Nếu rồi -> bỏ qua (dedup).
3. Tạo notification:
   - Cho organizer (`IdStaff` của event): Title "Nhắc nhở: {title}".
   - Cho từng participant: Title "Nhắc nhở tham gia: {title}".
   - Message: "Sự kiện của bạn sẽ bắt đầu trong {ReminderMinutes} phút. Loại: {EventType}. Thời gian: {HH:mm}".
4. Lưu DB. **Không** gọi realtime push.

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| EndTime > StartTime | Validate cả khi tạo và khi update (nếu cả 2 cùng có). |
| ReminderMinutes | Phải > 0 mới trigger reminder. |
| Reminder window | 5-10 phút trước StartTime. |
| Reminder dedup | Mỗi event chỉ nhận 1 reminder/10 phút. |
| Realtime duy nhất | Chỉ `addParticipant` mới trigger realtime notification. Các thay đổi khác (create/update event, change status) chỉ lưu DB. |
| Liên kết entity | Có thể gắn `RelatedEntityType` (Lead/Customer/Deal) để filter sau. |
| Timezone | Tất cả DateTime dùng UTC. Client tự convert theo timezone. |

---

## 7. Tích hợp ngang

- **Notification**: SYSTEM (khi add participant), REMINDER (background).
- **Realtime**: SignalR qua NotificationHub.
- **LinkedEntity**: tham chiếu Lead/Customer/Deal.
- **Audit**: KHÔNG ghi audit log cho calendar event.
