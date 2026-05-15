# CRM Backend Refactoring Documentation

## Overview
This document describes the comprehensive refactoring of the CRM backend system from a Database-First approach to a Code-First approach with proper entity architecture, soft delete, and audit fields.

---

## 1. Entity Architecture Changes

### 1.1 New Entity Hierarchy (TPH - Table Per Hierarchy)

**Before:**
- `Person` entity (standalone)
- `Staff` entity (separate table, duplicate fields with Person)
- `Lead` entity (shares ID with Person via FK)
- `Customer` entity (shares ID with Person via FK)

**After:**
```
BaseEntity (Id, CreatedAt, UpdatedAt, IsDeleted, DeletedAt)
└── Person (Fullname, Email, Phone, Location, Discriminator)
    ├── Staff (Username, PasswordHash, Role, Salary) - Discriminator = 0
    ├── Lead (Resource) - Discriminator = 1
    └── Customer - Discriminator = 2
```

### 1.2 Role System (2 Roles)

| Role | Value | Description |
|------|-------|-------------|
| ADMIN | 0 | Full system access, can create staff accounts |
| STAFF | 1 | Standard user access, managed by Admin |

**Note:** Registration feature has been removed. Admin creates staff accounts manually via `createStaff` mutation.

### 1.3 Lead to Customer Conversion

When Contact status is updated to `SUCCESS`:
1. The associated Lead's `Discriminator` is changed from `Lead` (1) to `Customer` (2)
2. Lead record is updated in place, not deleted and recreated
3. Email uniqueness is checked against Staff only (Lead/Customer share table)

---

## 2. Status Enums & Constants

### 2.1 ContactStatus Enum
```csharp
public enum ContactStatus
{
    NEW,
    IN_PROGRESS,
    SUCCESS,
    FAILED,
    CLOSED,
    CANCELLED
}
```

### 2.2 StatusContactConstant
```csharp
ContactNew = "NEW"
ContactInProgress = "IN_PROGRESS"
ContactSuccess = "SUCCESS"
ContactFailed = "FAILED"
ContactClosed = "CLOSED"
ContactCanceled = "CANCELED"
```

### 2.3 DealStatus Enum
```csharp
public enum DealStatus
{
    OPEN,
    NEGOTIATING,
    WON,
    LOST
}
```

### 2.4 StatuDealConstant
```csharp
DealOpen = "OPEN"
DealNegotiating = "NEGOTIATING"
DealWon = "WON"
DealLost = "LOST"
```

---

## 3. GraphQL Schema Changes

### 3.1 Input Types

Created in `CustomerManagement.Api/Input.Type/`:

| File | Description |
|------|-------------|
| `StaffInputType.cs` | `StaffInput`, `StaffUpdateInput` |
| `PersonInputType.cs` | `PersonInput`, `PersonUpdateInput` |
| `LeadInputType.cs` | `LeadInput`, `LeadUpdateInput` |
| `CustomerInputType.cs` | `CustomerInput`, `CustomerUpdateInput` |
| `ContactInputType.cs` | `ContactInput`, `ContactUpdateInput` |
| `DealInputType.cs` | `DealInput`, `DealUpdateInput` |

### 3.2 Enum Types

| File | Values |
|------|--------|
| `StaffRoleType.cs` | ADMIN, STAFF |
| `ContactStatusType.cs` | NEW, IN_PROGRESS, SUCCESS, FAILED, CLOSED, CANCELLED |
| `DealStatusType.cs` | OPEN, NEGOTIATING, WON, LOST |

### 3.3 Naming Standardization

| Old Name | New Name |
|----------|----------|
| `infLeadResponse` | `lead` |
| `infStaffResponse` | `staff` |
| `infCustomerResponse` | `customer` |
| `personResponse` | `person` |

---

## 4. Statistics & Chart Queries

### 4.1 GetStatistics Response
```csharp
QuantityStatisticsResponse {
    TotalProfit: decimal
    QuantityCustomers: int
    QuantityLeads: int
    QuantityContacts: int
    QuantityDeals: int
    quantityStatisticsDetailContactResponse: {
        QuantityContactsPending (NEW status)
        QuantityContactsInProgress
        QuantityContactsDone (SUCCESS status)
        QuantityContactsCancel
        QuantityContactsFailed
    }
    quantityStatisticsDetailDealResponse: {
        QuantityDealsPending (OPEN status)
        QuantityDealsWon
        QuantityDealsLost
    }
}
```

### 4.2 GetChartDeal Response
```csharp
ChartDealResponse {
    SuccessfullDealValue: decimal (sum of WON deals)
    FailedDealValue: decimal (sum of LOST deals)
    ListSuccessfullDeal: List<ListSuccessfullDealResponse>
    ListFailedDeal: List<ListFailedDealResponse>
}

ListSuccessfullDealResponse {
    IdDeal: Guid
    Price: decimal?
    Status: string?
    CreatedAt: DateTime?
}
```

---

## 5. Handler Changes

### 5.1 ContactHandler - Lead to Customer Conversion

When `updateContact` changes status to `SUCCESS`:
```csharp
// Check if email already exists in Staff
var staffWithEmail = await _staffRepository.GetStaffByEmailAsync(leadToConvert.Email);
if (staffWithEmail != null)
{
    throw new DomainException("Email đã tồn tại trong hệ thống!", 409);
}

// Update Lead to Customer (change Discriminator)
leadToConvert.Discriminator = PersonType.Customer;
leadToConvert.UpdatedAt = DateTime.UtcNow;

await _leadRepository.UpdateLeadAsync(leadToConvert);
```

### 5.2 Method Signature Changes

| Handler | Method | Change |
|---------|--------|--------|
| StaffHandler | DeleteStaffAsync | Added `deletedBy` parameter |
| StaffHandler | RestoreStaffAsync | NEW method |
| LeadHandler | DeleteLeadAsync | No `deletedBy` parameter |
| LeadHandler | RestoreLeadAsync | NEW method |
| CustomerHandler | DeleteCustomerAsync | No `deletedBy` parameter |
| CustomerHandler | RestoreCustomerAsync | NEW method |
| ContactHandler | DeleteContactAsync | No `deletedBy` parameter |
| DealHandler | DeleteDealAsync | No `deletedBy` parameter |
| ContactHandler | UpdateContactAsync | Added Lead→Customer conversion on SUCCESS |

---

## 6. Repository Updates

### 6.1 LeadRepository.UpdateLeadAsync
Now updates `Discriminator` field:
```csharp
existingLead.Discriminator = lead.Discriminator;
existingLead.UpdatedAt = DateTime.UtcNow;
```

### 6.2 CustomerRepository
Added `GetCustomerByEmailAsync(string email)` method.

### 6.3 ContactRepository.AddContactAsync
Default status changed to `StatusContactConstant.ContactNew` ("NEW")

### 6.4 DealRepository.AddDealAsync
Default status changed to `StatuDealConstant.DealOpen` ("OPEN")

---

## 7. Database Schema

```
persons (TPH table)
├── id (PK, UUID)
├── fullname (NVARCHAR200)
├── email (NVARCHAR255, UNIQUE)
├── phone (NVARCHAR20)
├── location (NVARCHAR500)
├── discriminator (INT) -- 0=Staff, 1=Lead, 2=Customer
├── username (NVARCHAR100, UNIQUE) -- Staff only
├── password_hash (NVARCHAR500) -- Staff only
├── role (NVARCHAR50) -- Staff only (ADMIN, STAFF)
├── salary (DECIMAL15,2) -- Staff only
├── resource (NVARCHAR500) -- Lead only
├── created_at (TIMESTAMP)
├── updated_at (TIMESTAMP)
├── is_deleted (BOOLEAN)
└── deleted_at (TIMESTAMP)

contacts
├── id_contact (PK, UUID)
├── type (NVARCHAR50)
├── title (NVARCHAR100)
├── content (TEXT)
├── status (NVARCHAR50) -- NEW, IN_PROGRESS, SUCCESS, FAILED, CLOSED, CANCELLED
├── id_staff (FK -> persons.id)
├── id_lead (FK -> persons.id)
├── created_at (TIMESTAMP)
├── updated_at (TIMESTAMP)
├── is_deleted (BOOLEAN)
└── deleted_at (TIMESTAMP)

deals
├── id_deal (PK, UUID)
├── title (NVARCHAR100)
├── content (TEXT)
├── price (DECIMAL15,2)
├── status (NVARCHAR50) -- OPEN, NEGOTIATING, WON, LOST
├── id_staff (FK -> persons.id)
├── id_customer (FK -> persons.id)
├── created_at (TIMESTAMP)
├── updated_at (TIMESTAMP)
├── is_deleted (BOOLEAN)
└── deleted_at (TIMESTAMP)
```

---

## 8. Removed Features

### Registration Flow - REMOVED
- Registration feature completely removed
- Admin creates staff accounts via `createStaff` mutation
- No OTP verification needed for staff creation

### Removed Files
- `CustomerManagement.Api/Mutation/RegisterMutation.cs`
- `CustomerManagement.Application/Handlers/Auth/RegisterHandler.cs`
- `CustomerManagement.Application/Handlers/Auth/RegisterCacheData.cs`
- `CustomerManagement.Application/DTOs/Auth/RegisterRequest.cs`

---

## 9. Files Modified Summary

### Domain Layer
- `CustomerManagement.Domain/Entities/BaseEntity.cs` - NEW
- `CustomerManagement.Domain/Entities/Person.cs` - NEW
- `CustomerManagement.Domain/Entities/Contact.cs` - Moved from Infrastructure
- `CustomerManagement.Domain/Entities/Deal.cs` - Moved from Infrastructure

### Domain Layer - Exception
- `CustomerManagement.Domain/Exception/DomainException.cs` - NEW
- `CustomerManagement.Domain/Exception/Exceptions.cs` - NEW (all custom exceptions)

### Domain Layer - Constants
- `CustomerManagement.Domain/Constant/StatusContactConstant.cs` - Updated (SUCCESS, FAILED, CLOSED)
- `CustomerManagement.Domain/Constant/StatuDealConstant.cs` - Updated (OPEN, NEGOTIATING)

### Infrastructure Layer - Repositories
- `CustomerManagement.Infrastructure/Repositories/StaffRepository.cs`
- `CustomerManagement.Infrastructure/Repositories/LeadRepository.cs` - Updated Discriminator handling
- `CustomerManagement.Infrastructure/Repositories/CustomerRepository.cs` - Added GetCustomerByEmailAsync
- `CustomerManagement.Infrastructure/Repositories/ContactRepository.cs`
- `CustomerManagement.Infrastructure/Repositories/DealRepository.cs`

### Application Layer - Handlers
- `CustomerManagement.Application/UseCases/StaffHandler.cs`
- `CustomerManagement.Application/UseCases/LeadHandler.cs`
- `CustomerManagement.Application/UseCases/CustomerHandler.cs`
- `CustomerManagement.Application/UseCases/ContactHandler.cs` - Lead→Customer conversion on SUCCESS
- `CustomerManagement.Application/UseCases/DealHandler.cs`
- `CustomerManagement.Application/UseCases/StatisticsHandler.cs` - Includes detail statistics
- `CustomerManagement.Application/UseCases/ChartDealHandler.cs`

### Application Layer - Interfaces
- `CustomerManagement.Application/Interfaces/ICustomerRepository.cs` - Added GetCustomerByEmailAsync

### API Layer
- `CustomerManagement.Api/Input.Type.Enums/StaffRoleType.cs`
- `CustomerManagement.Api/Input.Type.Enums/ContactStatusType.cs`
- `CustomerManagement.Api/Input.Type.Enums/DealStatusType.cs`
- All Input Types (Staff, Lead, Customer, Contact, Deal, Person)

---

## 10. Domain Exceptions

All exceptions are located in `CustomerManagement.Domain/Exception/`.

### 10.1 Exception Hierarchy

```
DomainException (base)
├── ValidationException (400)
│   ├── RequiredFieldException
│   ├── InvalidEmailException
│   ├── InvalidFormatException
│   ├── InvalidGuidException
│   ├── InvalidLengthException
│   └── InvalidPasswordException
├── NotFoundException (404)
│   ├── StaffNotFoundException
│   ├── LeadNotFoundException
│   ├── CustomerNotFoundException
│   ├── ContactNotFoundException
│   └── DealNotFoundException
├── ConflictException (409)
│   ├── EmailAlreadyExistsException
│   ├── UsernameAlreadyExistsException
│   └── DuplicateEntryException
├── UnauthorizedException (401)
│   └── InvalidCredentialsException
└── BusinessRuleException (422)
    ├── InvalidStatusTransitionException
    └── CannotConvertLeadException
```

### 10.2 Input Validation Rules

**StaffHandler.CreateStaffAsync:**
- Fullname: required, not empty
- Email: required, valid email format
- Username: required, not empty
- Password: required, min 6 chars
- Role: must be "ADMIN" or "STAFF"

**StaffHandler.UpdateStaffAsync:**
- Fullname: required, not empty
- Email: required, valid email format
- Role: if provided, must be "ADMIN" or "STAFF"

**LeadHandler.CreateLeadAsync:**
- Fullname: required, not empty
- Email: required, valid email format

**CustomerHandler.CreateCustomerAsync:**
- Fullname: required, not empty
- Email: required, valid email format

**ContactHandler.CreateContactAsync:**
- IdStaff: required, valid Guid
- IdLead: required, valid Guid
- Type: optional, max 50 chars
- Title: optional, max 100 chars

**ContactHandler.UpdateContactAsync:**
- Status: if provided, must be valid ContactStatus enum value

**DealHandler.CreateDealAsync:**
- IdStaff: required, valid Guid
- IdCustomer: required, valid Guid
- Title: required, not empty, max 100 chars
- Price: if provided, must be >= 0

**DealHandler.UpdateDealAsync:**
- Title: if provided, not empty, max 100 chars
- Price: if provided, must be >= 0
- Status: if provided, must be valid DealStatus enum value

---

## 11. Build Status

**Build:** SUCCESS (0 errors, 0 warnings)

---

## 12. New Collaborative Features (2026-05-13)

### NHÓM 1: QUẢN LÝ CÔNG VIỆC (Task Management)

**Entities:**
- `TaskEntity` - Quản lý công việc với Priority và Status

**Tables Created:**
- `tasks` - id_task, title, description, due_date, priority, status, id_staff_assigned, linked_entity_type, linked_entity_id

**Constants:**
- `TaskPriorityConstant` - LOW, MEDIUM, HIGH, URGENT
- `TaskStatusConstant` - PENDING, IN_PROGRESS, COMPLETED, CANCELLED

**API Endpoints:**
- Mutations: createTask, updateTask, deleteTask, restoreTask, assignTask, updateTaskStatus
- Queries: getTasks, getTasksByStaff, getTasksByStatus, getTaskById

**Business Rules:**
- ADMIN: Tạo, sửa, xóa, gán task cho bất kỳ staff nào
- STAFF: Chỉ cập nhật status của task được gán cho mình
- Khi gán task -> tạo Notification cho Staff
- Khi COMPLETED -> thông báo cho Admin

---

### NHÓM 2: BÌNH LUẬN & GHI CHÚ (Activity Notes)

**Entities:**
- `Note` - Bình luận trên Lead/Customer/Deal với @mention support
- `NoteMention` - Theo dõi @mention trong bình luận

**Tables Created:**
- `notes` - id_note, content, type, is_pinned, id_staff, linked_entity_type, linked_entity_id, parent_note_id
- `note_mentions` - id_mention, id_note, id_staff_mentioned

**Constants:**
- `NoteTypeConstant` - COMMENT, UPDATE, SYSTEM

**API Endpoints:**
- Mutations: createNote, updateNote, deleteNote, pinNote, unpinNote, replyNote
- Queries: getNotesByEntity, getNoteById, getPinnedNotes

**Business Rules:**
- COMMENT: Bình luận thường của staff
- UPDATE: Cập nhật trạng thái (tự động tạo khi Lead/Customer/Deal thay đổi)
- SYSTEM: Thông báo hệ thống
- @Mention: Khi content chứa `@username` -> tạo NoteMention + Notification

---

### NHÓM 3: THÔNG BÁO (Notifications)

**Entities:**
- `Notification` - Hệ thống thông báo real-time

**Tables Created:**
- `notifications` - id_notification, title, message, type, is_read, is_pinned, id_staff, related_entity_type, related_entity_id

**Constants:**
- `NotificationTypeConstant` - TASK_ASSIGNED, TASK_COMPLETED, DEAL_UPDATED, CONTACT_STATUS_CHANGED, MENTION, SYSTEM

**API Endpoints:**
- Mutations: markAsRead, markAllAsRead, pinNotification, deleteNotification
- Queries: getNotifications, getUnreadNotifications, getPinnedNotifications, getUnreadCount

**Trigger Events:**
- Task được gán -> TASK_ASSIGNED notification
- Task hoàn thành -> TASK_COMPLETED notification cho Admin
- @mention trong Note -> MENTION notification

---

## 13. Implemented Features (NHÓM 1-8)

### Completed Features

| # | Feature Group | Status |
|---|---------------|--------|
| 1 | Task Management (NHÓM 1) | ✅ COMPLETED |
| 2 | Activity Notes (NHÓM 2) | ✅ COMPLETED |
| 3 | Notifications (NHÓM 3) | ✅ COMPLETED |
| 4 | Staff Presence (NHÓM 4) | ✅ COMPLETED |
| 5 | Team Assignment (NHÓM 5) | ✅ COMPLETED |
| 6 | Audit Log (NHÓM 6) | ✅ COMPLETED |
| 7 | Calendar/Scheduling (NHÓM 7) | ✅ COMPLETED |
| 8 | Analytics Dashboard (NHÓM 8) | ✅ COMPLETED |

### Feature Summary

**NHÓM 1 - Task Management:**
- `TaskEntity` with Priority and Status
- CRUD operations with soft delete
- Linked to Lead/Customer/Deal

**NHÓM 2 - Activity Notes:**
- `Note` entity with @mention support
- Types: COMMENT, UPDATE, SYSTEM
- Linked to Lead/Customer/Deal/Task

**NHÓM 3 - Notifications:**
- Real-time notification system
- Types: TASK_ASSIGNED, TASK_COMPLETED, DEAL_UPDATED, MENTION, SYSTEM
- Triggered by task assignment and @mentions

**NHÓM 4 - Staff Presence:**
- Status tracking (OFFLINE, ONLINE, BUSY, AWAY)
- Activity logging
- LastActiveAt tracking

**NHÓM 5 - Team Assignment:**
- Teams with members
- Team lead functionality
- Shared entity access

**NHÓM 6 - Audit Log:**
- System-wide change tracking
- Entity history
- Admin visibility

---

## 14. Newly Implemented Features (NHÓM 4, 5, 6) - 2026-05-15

### NHÓM 4: Staff Presence

**Entities:**
- `StaffActivityLog` - Activity logging
- Person entity extended with `Status` and `LastActiveAt` fields

**Tables Created:**
- `staff_activity_logs` - id_log, id_staff, action, entity_type, entity_id, timestamp, ip_address, user_agent

**Constants:**
- `StaffStatusConstant` - OFFLINE (0), ONLINE (1), BUSY (2), AWAY (3)

**API Endpoints:**
- Queries: `getStaffStatuses`, `getOnlineStaffs`, `getStaffActivityLogs`
- Mutations: `updateMyStatus`, `refreshLastActive`

**Business Rules:**
- Staff cannot manually set OFFLINE (must logout)
- Auto LOGIN/STATUS_CHANGE activity logging

---

### NHÓM 5: Team Assignment

**Entities:**
- `TeamMember` - Team membership with roles

**Tables Created:**
- `team_members` - id, entity_type, entity_id, id_staff, role, assigned_at, assigned_by, can_edit, can_delete

**Constants:**
- `TeamRoleConstant` - OWNER (0), MEMBER (1), VIEWER (2)
- `TeamEntityTypeConstant` - Lead, Deal

**API Endpoints:**
- Queries: `getTeamMembers`, `getMyTeams`, `getTeamMemberPermissions`
- Mutations: `addTeamMember`, `updateTeamMember`, `removeTeamMember`, `transferOwnership`

**Business Rules:**
- OWNER: full permissions (add/remove members, edit, delete entity)
- MEMBER: view + update (if CanEdit=true)
- VIEWER: view only
- Cannot delete last OWNER

---

### NHÓM 6: Audit Log

**Entities:**
- `AuditLog` - System-wide change tracking

**Tables Created:**
- `audit_logs` - id_log, action, entity_type, entity_id, old_values (jsonb), new_values (jsonb), id_staff, staff_name, ip_address, user_agent, timestamp, description

**Constants:**
- `AuditActionConstant` - CREATE, UPDATE, DELETE, RESTORE, ASSIGN, LOGIN, LOGOUT
- `AuditEntityTypeConstant` - Staff, Lead, Customer, Contact, Deal, Task, Note, Notification, TeamMember

**API Endpoints:**
- Queries: `getAuditLogs`, `getAuditLogsByStaff`, `getAuditLogsByAction`, `getEntityHistory`, `getAuditStatistics`
- Internal: `LogAsync` method for creating audit entries

**Business Rules:**
- Logs are immutable (no update/delete)
- JSONB storage for OldValues/NewValues
- Index on EntityType, EntityId, Timestamp, IdStaff

---

## 15. Migration History

| Migration | Date | Description |
|-----------|------|-------------|
| 20260513165737 | 2026-05-13 | AddTaskNoteNotificationEntities |
| 20260514185433 | 2026-05-14 | AddStaffPresence |
| 20260514190935 | 2026-05-14 | AddTeamAssignment |
| 20260514191148 | 2026-05-14 | AddAuditLog |
| 20260514192211 | 2026-05-14 | AddCalendarScheduling |
| 20260514192716 | 2026-05-14 | AddAnalyticsDashboard |

---

## 16. NHÓM 7: Calendar/Scheduling (2026-05-15)

### Entities
- `CalendarEvent` - Lịch sự kiện với participants
- `EventParticipant` - Người tham gia sự kiện

### Tables Created
- `calendar_events` - id_event, title, description, event_type, start_time, end_time, location, is_all_day, reminder_minutes, status, id_staff, related_entity_type, related_entity_id
- `event_participants` - id, id_event, id_staff, status, responded_at

### Constants
- `CalendarEventTypeConstant` - MEETING (0), CALL (1), TASK_DEADLINE (2), FOLLOW_UP (3)
- `CalendarEventStatusConstant` - SCHEDULED (0), IN_PROGRESS (1), COMPLETED (2), CANCELLED (3)
- `ParticipantStatusConstant` - PENDING (0), ACCEPTED (1), DECLINED (2), TENTATIVE (3)

### API Endpoints
- Queries: getCalendarEvents, getCalendarEventById, getMyEvents, getUpcomingEvents, getEventParticipants
- Mutations: createCalendarEvent, updateCalendarEvent, deleteCalendarEvent, cancelCalendarEvent, addParticipant, updateParticipantStatus, removeParticipant

---

## 17. NHÓM 8: Analytics Dashboard (2026-05-15)

### Response DTOs
- `DashboardResponse` - Tổng quan dashboard
- `RevenueChartResponse` - Biểu đồ doanh thu theo ngày/tháng
- `PipelineFunnelResponse` - Pipeline funnel (OPEN → NEGOTIATING → WON)
- `StaffPerformanceResponse` - Hiệu suất nhân viên
- `LeadConversionResponse` - Tỷ lệ chuyển đổi Lead → Customer
- `ExportReportResponse` - Download URL cho export

### API Endpoints
- Queries: getDashboardSummary, getRevenueChart, getPipelineFunnel, getTopPerformingStaff, getStaffPerformanceReport, getLeadConversionReport
- Export: exportDealsReport, exportLeadsReport, exportCustomersReport

---

## 18. File Upload Changes (2026-05-15)

### Issue
HotChocolate 15 does not support `IFormFile`/`IFile` as GraphQL schema types. GraphQL mutations using file upload parameters caused schema initialization to fail with:
```
HotChocolate.SchemaException: Unable to infer or resolve a schema type from the type reference `IFormFile (Input)`.
```

### Solution
File upload functionality was moved from GraphQL mutations to REST API endpoints.

**Removed from GraphQL:**
- `LeadMutation.importLeadExcelAsync(IFormFile file)` - REMOVED
- `CustomerMutation.importCustomerExcelAsync(IFormFile file)` - REMOVED

**New REST Endpoints:**
- `POST /api/fileupload/lead` - Import Lead từ Excel file
- `POST /api/fileupload/customer` - Import Customer từ Excel file

### New Files
- `CustomerManagement.Api/Controllers/FileUploadController.cs` - REST controller for file uploads

### Files Modified
- `CustomerManagement.Api/Mutation/LeadMutation.cs` - Removed file upload method
- `CustomerManagement.Api/Mutation/CustomerMutation.cs` - Removed file upload method
- `CustomerManagement.Application/UseCases/LeadHandler.cs` - Changed `IFile` to `IFormFile`
- `CustomerManagement.Application/UseCases/CustomerHandler.cs` - Changed `IFile` to `IFormFile`

---

---

## 19. SignalR Realtime Implementation (2026-05-15)

### Overview
Two separate SignalR hubs implemented for real-time communications.

### Hubs

#### NotificationHub (`/hubs/notifications`)
- `JoinStaffGroup(Guid idStaff)` - Subscribe to personal notifications
- `LeaveStaffGroup(Guid idStaff)` - Unsubscribe from personal notifications

#### NoteHub (`/hubs/notes`)
- `JoinEntityGroup(string entityType, Guid entityId)` - Subscribe to entity notes (Lead/Customer/Deal)
- `LeaveEntityGroup(string entityType, Guid entityId)` - Unsubscribe
- `JoinStaffGroup(Guid idStaff)` - Subscribe to personal notes

### Services

| Interface | Implementation | Purpose |
|-----------|----------------|---------|
| `IRealtimeNotificationService` | `RealtimeNotificationService` | Send notifications to staff |
| `IRealtimeNoteService` | `RealtimeNoteService` | Send notes to entities/staff |
| `IRealtimeNotificationSender` | `RealtimeNotificationSenderAdapter` | Application layer adapter |
| `IRealtimeNoteSender` | `RealtimeNoteSenderAdapter` | Application layer adapter |

### GraphQL Integration

**NotificationMutation:**
- `createNotificationAsync` - Creates notification + sends via SignalR
- All other methods (markAsRead, markAllAsRead, pin, delete) trigger notifications via `IRealtimeNotificationService`

**NoteMutation:**
- `CreateNoteAsync` - Creates note + sends realtime via `IRealtimeNoteService`
- `UpdateNoteAsync` - Updates note + sends realtime
- `ReplyNoteAsync` - Creates reply + sends realtime

**CalendarMutation:**
- Event creation/participant changes send notifications to participants via `IRealtimeNotificationService`

### Background Service

**CalendarReminderService** (`CustomerManagement.Infrastructure/Services/`)
- Runs every 5 minutes
- Finds events starting in 5-10 minutes
- Sends `NotificationReminder` type notifications to organizer and participants

---

## 20. CalendarReminderService Background Job

### Functionality
- Runs every 5 minutes (via `IHostedService`)
- Queries events with `StartTime` in next 5-10 minutes
- Creates notifications for organizer and all accepted participants
- Notification type: `NotificationReminder`

### Notification Content
- Title: "Event Reminder"
- Message: "{EventTitle} starts at {StartTime}"
- RelatedEntityType: "CalendarEvent"
- RelatedEntityId: Event ID

---

## 21. Elasticsearch Status

Elasticsearch code has been **commented out** (not deleted) for future implementation.

**Commented in Program.cs:**
- `builder.Configuration["Elasticsearch:Uri"]` line
- `IElasticsearchService` registration
- 5 Query type extensions (StaffElasticSearchQuery, CustomersElasticSearchQuery, etc.)

**Commented in Handler files:**
- `CustomerHandler.cs`, `LeadHandler.cs`, `ContactHandler.cs`, `DealHandler.cs`, `StaffHandler.cs`, `TaskHandler.cs`, `NoteHandler.cs`

**Commented Query files:**
- `CustomersElasticSearchQuery.cs`, `LeadsElasticSearchQuery.cs`, `ContactsElasticSearchQuery.cs`, `DealsElasticSearchQuery.cs`, `StaffElasticSearchQuery.cs`

---

## 22. Bug Fixes (2026-05-16)

### NoteHandler NullReferenceException

**Issue:** `CreateNoteAsync` threw `NullReferenceException` at line 41.

**Root Cause:** Constructor was commented out, `_staffRepository` was null.

**Fix:** Uncommented constructor in `NoteHandler.cs`.

---

### GetCalendarEvents 500 Error (NullReference)

**Issue:** `GetCalendarEvents` query returned 500 error.

**Root Cause:** `MapStaffToResponse` assigned database null values to non-nullable DTO properties (`Fullname`, `Email`).

**Fix:** Added null-coalescing operators in `CalendarQuery.cs` and `CalendarHandler.cs`:
```csharp
Fullname = staff.Fullname ?? "",
Email = staff.Email ?? "",
```

---

### GetCalendarEvents ObjectDisposedException

**Issue:** `GetCalendarEvents` threw `ObjectDisposedException: Cannot access a disposed context instance`.

**Root Cause:** Repository methods returned `IQueryable<T>` with `await using var context` pattern. The `IQueryable` was lazy-evaluated after the DbContext was disposed.

**Fix:** Changed repository methods to return `Task<List<T>>` with `.ToListAsync()` before context disposal. Updated all callers in `CalendarQuery.cs` and `CalendarHandler.cs`.

---

### TeamAssignmentHandler ObjectDisposedException

**Issue:** `GetMyTeamsAsync` query threw `ObjectDisposedException` at line 62.

**Root Cause:** `TeamMemberRepository.GetByEntityAsync` and `GetByStaffAsync` returned `IQueryable<TeamMember>` with `await using var context`. The `IQueryable` was lazy-evaluated in `foreach` loop after context was disposed.

**Fix:** Changed interface and implementation from `Task<IQueryable<TeamMember>>` to `Task<List<TeamMember>>` with `.ToListAsync()` before context disposal.

**Files Modified:**
- `CustomerManagement.Application\Interfaces\ITeamMemberRepository.cs` (lines 8-9)
- `CustomerManagement.Infrastructure\Repositories\TeamMemberRepository.cs` (lines 26-39)

---

### AutoMapper Missing TeamMember -> TeamMemberResponse Mapping

**Issue:** `UpdateTeamMemberAsync` and `TransferOwnershipAsync` threw `AutoMapper.AutoMapperMappingException: Missing type map configuration or unsupported mapping`.

**Root Cause:** `_mapper.Map<TeamMemberResponse>(updated)` was called but no mapping profile existed for `TeamMember -> TeamMemberResponse`.

**Fix:** Created `TeamMemberMapper.cs` in `CustomerManagement.Infrastructure\Mapping\`.

**Files Created:**
- `CustomerManagement.Infrastructure\Mapping\TeamMemberMapper.cs`

---

### AuditLogRepository ObjectDisposedException

**Issue:** Multiple AuditLog queries threw `ObjectDisposedException` in `GetAuditLogsAsync`, `GetAuditLogsByStaffAsync`, `GetAuditLogsByActionAsync`, `GetEntityHistoryAsync`.

**Root Cause:** `IAuditLogRepository` methods (`GetAllAsync`, `GetByEntityAsync`, `GetByStaffAsync`, `GetByActionAsync`, `GetByDateRangeAsync`, `GetEntityHistoryAsync`) all returned `Task<IQueryable<AuditLog>>` with `await using var context` pattern.

**Fix:** Changed all 6 methods from `Task<IQueryable<AuditLog>>` to `Task<List<AuditLog>>` with `.ToListAsync()` before context disposal. Updated callers in `AuditLogQuery.cs` and `AuditLogHandler.cs` to use `List<AuditLog>` instead of `IQueryable<AuditLog>`.

**Files Modified:**
- `CustomerManagement.Application\Interfaces\IAuditLogRepository.cs`
- `CustomerManagement.Infrastructure\Repositories\AuditLogRepository.cs`
- `CustomerManagement.Api\Query\AuditLogQuery.cs`
- `CustomerManagement.Application\UseCases\AuditLogHandler.cs`

---

### AuditStatisticsResponse Missing Fields

**Issue:** `getAuditStatistics` query returned GraphQL error: "The field `totalActions` does not exist on the type `AuditStatisticsResponse`", etc.

**Root Cause:** `AuditStatisticsResponse` DTO was missing fields that client query required: `totalActions`, `totalEntities`, `topActions`, `topEntities`.

**Fix:** Added computed properties and new DTO classes (`TopActionItem`, `TopEntityItem`) to `AuditStatisticsResponse.cs`.

**File Modified:**
- `CustomerManagement.Application\DTOs\Response\AuditStatisticsResponse.cs`

---

## 23. AI Chat Feature - Commented Out for Future Development (2026-05-16)

### Overview
AI Chat feature (Google Gemini integration) has been **commented out** but **NOT deleted** for future development. All code is preserved and can be re-enabled by uncommenting.

### Files Commented Out

**Application Layer:**
- `CustomerManagement.Application\UseCases\ChatHandler.cs`
- `CustomerManagement.Application\Interfaces\IGeminiService.cs`
- `CustomerManagement.Application\Interfaces\IChatHistoryService.cs`
- `CustomerManagement.Application\DTOs\Chat\ChatRequest.cs`
- `CustomerManagement.Application\DTOs\Chat\ChatResponse.cs`
- `CustomerManagement.Application\DTOs\Chat\GeminiRequest.cs`
- `CustomerManagement.Application\DTOs\Chat\GeminiApiResponse.cs`
- `CustomerManagement.Application\DTOs\Response\MessageHistoryItem.cs`

**Infrastructure Layer:**
- `CustomerManagement.Infrastructure\Services\GeminiService.cs`
- `CustomerManagement.Infrastructure\Services\ChatHistoryService.cs`

**API Layer:**
- `CustomerManagement.Api\Query\ChatQuery.cs`
- `CustomerManagement.Api\Mutation\ChatMutation.cs`

**Program.cs Changes:**
- ChatHandler registration: commented out
- ChatQuery/ChatMutation GraphQL extensions: commented out
- IGeminiService HttpClient registration: commented out
- IChatHistoryService registration: commented out

### Re-enabling AI Chat
To re-enable AI Chat feature, uncomment the following in `Program.cs`:
1. `builder.Services.AddHttpClient<IGeminiService, GeminiService>()`
2. `builder.Services.AddScoped<IChatHistoryService, ChatHistoryService>()`
3. `builder.Services.AddScoped<ChatHandler>()`
4. `.AddTypeExtension<ChatQuery>()`
5. `.AddTypeExtension<ChatMutation>()`

---

## 24. Elasticsearch Feature - Commented Out for Future Development (2026-05-16)

### Overview
Elasticsearch search feature has been **commented out** but **NOT deleted** for future development. All code is preserved and can be re-enabled by uncommenting.

### Files Commented Out

**Application Layer:**
- `CustomerManagement.Application\Interfaces\IElasticsearchService.cs`

**Infrastructure Layer:**
- `CustomerManagement.Infrastructure\Services\ElasticsearchService.cs`

**API Layer (GraphQL Query Extensions):**
- `CustomerManagement.Api\Query\StaffElasticSearchQuery.cs`
- `CustomerManagement.Api\Query\CustomersElasticSearchQuery.cs`
- `CustomerManagement.Api\Query\LeadsElasticSearchQuery.cs`
- `CustomerManagement.Api\Query\ContactsElasticSearchQuery.cs`
- `CustomerManagement.Api\Query\DealsElasticSearchQuery.cs`

**Handler Files (Already commented out in constructors):**
- `CustomerManagement.Application\UseCases\LeadHandler.cs`
- `CustomerManagement.Application\UseCases\CustomerHandler.cs`
- `CustomerManagement.Application\UseCases\ContactHandler.cs`
- `CustomerManagement.Application\UseCases\DealHandler.cs`
- `CustomerManagement.Application\UseCases\StaffHandler.cs`
- `CustomerManagement.Application\UseCases\TaskHandler.cs`
- `CustomerManagement.Application\UseCases\NoteHandler.cs`

**Program.cs Changes:**
- Elasticsearch service registration: commented out
- Elasticsearch Query type extensions: commented out

### Re-enabling Elasticsearch
To re-enable Elasticsearch feature, uncomment the following in `Program.cs`:
1. `builder.Configuration["Elasticsearch:Uri"]` line
2. `builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>()`
3. All 5 `.AddTypeExtension<*ElasticSearchQuery>()` lines

Then uncomment constructor parameters and service usage in handler files.

---

*Document updated: 2026-05-16*
*All Features Status: NHÓM 1-8 COMPLETED - Build passing, all migrations applied*
*SignalR: 2 hubs (NotificationHub, NoteHub) with realtime services*
*File Upload: REST API endpoints implemented (not GraphQL)*
*Bug Fixes: NoteHandler constructor fixed, CalendarQuery null handling added, CalendarEventRepository disposed context fixed, TeamMemberRepository disposed context fixed, TeamMemberMapper created, AuditLogRepository disposed context fixed, StaffActivityLogRepository disposed context fixed, AuditStatisticsResponse fields added*
*AI Chat & Elasticsearch: Commented out for future development (2026-05-16)*