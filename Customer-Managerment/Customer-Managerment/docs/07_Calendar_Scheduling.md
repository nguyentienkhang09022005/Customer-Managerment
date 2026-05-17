
# NHÓM 7: LỊCH & HẸN (Calendar/Scheduling)

## Mục tiêu
Đặt lịch hẹn với Lead/Customer, nhắc nhở trước cuộc hẹn và quản lý lịch làm việc của team.

---

## 1. Database Schema

### CalendarEvent Entity
```
CalendarEvent:
- IdEvent (PK, Guid)
- Title (NVARCHAR200, NOT NULL)
- Description (TEXT, NULLABLE)
- EventType (INT) -- 0=MEETING, 1=CALL, 2=TASK_DEADLINE, 3=FOLLOW_UP
- StartTime (DATETIME, NOT NULL)
- EndTime (DATETIME, NOT NULL)
- Location (NVARCHAR500, NULLABLE)
- IsAllDay (BOOLEAN, default: false)
- ReminderMinutes (INT) -- Số phút trước khi nhắc (15, 30, 60)
- Status (INT) -- 0=SCHEDULED, 1=IN_PROGRESS, 2=COMPLETED, 3=CANCELLED
- CreatedAt, UpdatedAt (audit)
- IsDeleted, DeletedAt (soft delete)
- IdStaff (FK -> persons.id) -- Người tạo/organizer
- RelatedEntityType (NVARCHAR50, NULLABLE) -- "Lead", "Customer"
- RelatedEntityId (GUID, NULLABLE)
```

### EventParticipant Entity
```
EventParticipant:
- Id (PK, Guid)
- IdEvent (FK -> calendar_events.id)
- IdStaff (FK -> persons.id) -- Người tham gia
- Status (INT) -- 0=PENDING, 1=ACCEPTED, 2=DECLINED, 3=TENTATIVE
- RespondedAt (DATETIME, NULLABLE)
```

---

## 2. Files cần tạo mới

### Domain Layer
- `Domain/Entities/CalendarEvent.cs`
- `Domain/Entities/EventParticipant.cs`
- `Domain/Constant/CalendarEventTypeConstant.cs`
- `Domain/Constant/CalendarEventStatusConstant.cs`
- `Domain/Constant/ParticipantStatusConstant.cs`
- `Domain/Exception/CalendarEventNotFoundException.cs`

### Application Layer
- `DTOs/Requests/CalendarEventCreationRequest.cs`
- `DTOs/Requests/CalendarEventUpdateRequest.cs`
- `DTOs/Requests/EventParticipantRequest.cs`
- `DTOs/Response/CalendarEventResponse.cs`
- `DTOs/Response/EventParticipantResponse.cs`
- `DTOs/Response/CalendarScheduleResponse.cs`
- `Interfaces/ICalendarEventRepository.cs`
- `Interfaces/IEventParticipantRepository.cs`
- `UseCases/CalendarHandler.cs`

### Infrastructure Layer
- `Repositories/CalendarEventRepository.cs`
- `Repositories/EventParticipantRepository.cs`
- `Mapping/CalendarMapper.cs`

### API Layer
- `Input/Type/CalendarEventInputType.cs`
- `Input/Type/Enums/CalendarEventType.cs`
- `Input/Type/Enums/CalendarEventStatus.cs`
- `Input/Type/Enums/ParticipantStatus.cs`
- `Query/CalendarQuery.cs`
- `Mutation/CalendarMutation.cs`

---

## 3. API Endpoints

### Mutations
- `createCalendarEvent(CalendarEventInput)` - Tạo sự kiện
- `updateCalendarEvent(CalendarEventUpdateInput, idEvent)` - Cập nhật
- `deleteCalendarEvent(idEvent)` - Xóa
- `cancelCalendarEvent(idEvent)` - Hủy sự kiện
- `addParticipant(idEvent, idStaff)` - Thêm người tham gia
- `updateParticipantStatus(idParticipant, status)` - Phản hồi
- `removeParticipant(idParticipant)` - Xóa người tham gia

### Queries
- `getCalendarEvents(fromDate, toDate, idStaff)` - Lấy events trong khoảng
- `getCalendarEventById(idEvent)` - Chi tiết event
- `getMyEvents(idStaff, fromDate, toDate)` - Events của staff
- `getUpcomingEvents(idStaff, days)` - Events sắp tới
- `getEventsByEntity(entityType, entityId)` - Events của Lead/Customer
- `getEventParticipants(idEvent)` - Người tham gia event

---

## 4. Event Types

| Value | Constant | Mô tả |
|-------|----------|-------|
| 0 | MEETING | Cuộc họp trực tiếp |
| 1 | CALL | Cuộc gọi điện |
| 2 | TASK_DEADLINE | Deadline của task |
| 3 | FOLLOW_UP | Theo dõi khách hàng |

---

## 5. Business Rules

### Scheduling
- EndTime phải >= StartTime
- Không cho phép overlap đối với cùng 1 organizer
- Staff có thể xem lịch của team members (nếu là team)

### Reminders
- Gửi notification trước ReminderMinutes
- Background job check mỗi 5 phút
- Nếu ReminderMinutes = 0 hoặc null -> không nhắc

### Participants
- organizers có thể thêm/bỏ participants
- participants nhận notification khi được thêm
- participants có thể: ACCEPT, DECLINE, TENTATIVE

---

## 6. Integration

### Với Notification (Nhóm 3)
- Nhắc nhở trước event: REMINDER type
- Thông báo khi có participant mới
- Thông báo khi event bị hủy/thay đổi

### Với Team Assignment (Nhóm 5)
- Khi tạo event cho Lead/Deal, tự động thêm team members

### Với Task (Nhóm 1)
- TASK_DEADLINE event được tạo khi task có DueDate
- Cập nhật event khi task DueDate thay đổi

---

## 7. Background Jobs

### CalendarReminderService (IMPLEMENTED)
- Chạy mỗi 5 phút
- Tìm events có reminder trong vòng 5-10 phút tới
- Tạo notification cho organizer và participants
- Notification type: `NotificationReminder`
- Service: `IRealtimeNotificationService.SendNotificationToStaffAsync`

### Reminder Job
- Chạy mỗi 5 phút
- Tìm events có reminder trong vòng 5-10 phút tới
- Tạo notification cho organizer và participants

### Conflict Detection
- Chạy khi tạo/cập nhật event
- Kiểm tra overlap với events khác của organizer

---

## 8. Implementation Order

1. Entity + Constants + Exceptions
2. Repository + Mapper
3. Handler
4. GraphQL types + Query + Mutation
5. Background job cho reminders
6. Đăng ký services + Migration

---

## 9. Bug Fixes

### 2026-05-16: 500 Error in GetCalendarEvents Query

**Issue:** `GetCalendarEvents` query returned 500 error ("Lỗi hệ thống không xác định").

**Root Cause:** `MapStaffToResponse` method assigned database null values to non-nullable DTO properties (`Fullname`, `Email`), causing `NullReferenceException` during Hot Chocolate serialization.

**Fix:** Added null-coalescing operators in both `CalendarQuery.cs` and `CalendarHandler.cs`:

```csharp
// Before
Fullname = staff.Fullname,
Email = staff.Email,

// After
Fullname = staff.Fullname ?? "",
Email = staff.Email ?? "",
```

**Files Modified:**
- `CustomerManagement.Api\Query\CalendarQuery.cs` (lines 173-176)
- `CustomerManagement.Application\UseCases\CalendarHandler.cs` (lines 383-386)

---

### 2026-05-16: ObjectDisposedException in GetCalendarEvents Query

**Issue:** `GetCalendarEvents` threw `ObjectDisposedException: Cannot access a disposed context instance`.

**Root Cause:** Repository methods returned `IQueryable<CalendarEvent>` with `await using var context` pattern. The `IQueryable` was lazy-evaluated after the DbContext was disposed when the async method completed.

**Fix:** Changed repository methods to return `Task<List<CalendarEvent>>` with `.ToListAsync()` before the context is disposed:

```csharp
// Before
public async Task<IQueryable<CalendarEvent>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
{
    await using var context = _contextFactory.CreateDbContext();
    return context.CalendarEvents...
}

// After
public async Task<List<CalendarEvent>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
{
    await using var context = _contextFactory.CreateDbContext();
    return await context.CalendarEvents...
        .ToListAsync();
}
```

**Files Modified:**
- `CustomerManagement.Application\Interfaces\ICalendarEventRepository.cs`
- `CustomerManagement.Infrastructure\Repositories\CalendarEventRepository.cs`
- `CustomerManagement.Api\Query\CalendarQuery.cs`
- `CustomerManagement.Application\UseCases\CalendarHandler.cs`

---

### 2026-05-16: ObjectDisposedException in EventParticipantRepository

**Issue:** `GetCalendarEventById`, `GetMyEvents`, `GetUpcomingEvents` threw `ObjectDisposedException` because `EventParticipantRepository.GetByEventAsync` returned `IQueryable<EventParticipant>` with `await using var context` pattern.

**Root Cause:** Same IQueryable disposal pattern in `EventParticipantRepository.GetByEventAsync`.

**Fix:** Changed return type from `Task<IQueryable<EventParticipant>>` to `Task<List<EventParticipant>>` with `.ToListAsync()`.

**Files Modified:**
- `CustomerManagement.Application\Interfaces\IEventParticipantRepository.cs` (line 8)
- `CustomerManagement.Infrastructure\Repositories\EventParticipantRepository.cs` (lines 26-32)

---

### 2026-05-16: CalendarReminderService Query Exception

**Issue:** CalendarReminderService threw `ObjectDisposedException` when querying events due to same IQueryable disposal pattern.

**Root Cause:** `GetUpcomingEventsAsync` method returned `IQueryable<CalendarEvent>` that was evaluated after context disposal.

**Fix:** Changed `GetUpcomingEventsAsync` to return `Task<List<CalendarEvent>>` with `.ToListAsync()`.

**Files Modified:**
- `CustomerManagement.Application\Interfaces\ICalendarEventRepository.cs`
- `CustomerManagement.Infrastructure\Repositories\CalendarEventRepository.cs`