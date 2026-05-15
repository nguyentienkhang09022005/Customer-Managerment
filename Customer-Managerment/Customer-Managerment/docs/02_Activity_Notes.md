# NHÓM 2: BÌNH LUẬN & GHI CHÚ (Activity Notes)

## Mục tiêu
Cho phép Staff bình luận trên Lead/Customer/Deal, theo dõi lịch sử cập nhật và tag @mention nhân viên khác.

---

## 1. Database Schema

### Note Entity
```
Note:
- IdNote (PK, Guid)
- Content (TEXT, NOT NULL)
- Type (NVARCHAR50) -- COMMENT, UPDATE, SYSTEM
- CreatedAt, UpdatedAt (audit)
- IsDeleted, DeletedAt (soft delete)
- IdStaff (FK -> persons.id) -- Người tạo
- LinkedEntityType (NVARCHAR50) -- "Lead", "Customer", "Deal", "Task"
- LinkedEntityId (GUID, NOT NULL) -- Id của entity được gắn
- IsPinned (BOOLEAN) -- Ghim bình luận quan trọng
- ParentNoteId (FK -> notes.id, NULLABLE) -- Reply comment
```

### NoteMention Entity (để track @mention)
```
NoteMention:
- IdMention (PK, Guid)
- IdNote (FK -> notes.id)
- IdStaffMentioned (FK -> persons.id) -- Staff được tag
- CreatedAt
```

---

## 2. Files cần tạo mới

### Domain Layer
- `Domain/Entities/Note.cs`
- `Domain/Entities/NoteMention.cs`
- `Domain/Constant/NoteTypeConstant.cs`
- `Domain/Exception/NoteNotFoundException.cs`

### Application Layer
- `DTOs/Requests/NoteCreationRequest.cs`
- `DTOs/Requests/NoteUpdateRequest.cs`
- `DTOs/Response/NoteResponse.cs`
- `Interfaces/INoteRepository.cs`
- `UseCases/NoteHandler.cs`

### Infrastructure Layer
- `Repositories/NoteRepository.cs`
- `Repositories/NoteMentionRepository.cs`
- `Mapping/NoteMapper.cs`

### API Layer
- `Input/Type/NoteInputType.cs`
- `Input/Type/Enums/NoteTypeType.cs`
- `Query/NoteQuery.cs`
- `Mutation/NoteMutation.cs`

---

## 3. API Endpoints

### Mutations
- `createNote(NoteInput)` - Tạo bình luận mới
- `updateNote(NoteUpdateInput, idNote)` - Sửa bình luận
- `deleteNote(idNote)` - Xóa bình luận
- `pinNote(idNote)` - Ghim bình luận
- `unpinNote(idNote)` - Bỏ ghim
- `replyNote(idNote, parentId)` - Reply bình luận

### Queries
- `getNotesByEntity(entityType, entityId)` - Lấy notes của một entity
- `getNoteById(idNote)` - Chi tiết note
- `getPinnedNotes(entityType, entityId)` - Lấy notes được ghim

---

## 4. Business Rules

### Comment Types
- **COMMENT**: Bình luận thường của staff
- **UPDATE**: Cập nhật trạng thái (tự động tạo khi Lead/Customer/Deal thay đổi)
- **SYSTEM**: Thông báo hệ thống

### @Mention Logic
- Khi content chứa `@username` hoặc `@staffname` -> tạo NoteMention
- Tạo Notification cho staff được tag
- Notification type: `MENTION`

### Permissions
- **ADMIN**: Xóa/sửa bất kỳ note nào
- **STAFF**: Chỉ xóa/sửa note của mình

### Validation
- Content: required, max 5000 chars
- LinkedEntityType: "Lead", "Customer", "Deal", "Task"
- LinkedEntityId: phải tồn tại

---

## 5. Integration với các features khác

### Tự động tạo NOTE khi:
- Contact status thay đổi -> tạo UPDATE note trên Lead
- Deal status thay đổi -> tạo UPDATE note trên Deal
- Task được gán -> tạo SYSTEM note trên Task

### Reply Threading
- Notes có thể lồng nhau qua ParentNoteId
- Hiển thị theo tree structure

---

### Real-time via SignalR

**SignalR Hub:** `NoteHub` at `/hubs/notes`

**Methods:**
- `JoinEntityGroup(string entityType, Guid entityId)` - Subscribe to entity notes (Lead/Customer/Deal)
- `LeaveEntityGroup(string entityType, Guid entityId)` - Unsubscribe
- `JoinStaffGroup(Guid idStaff)` - Subscribe to personal notes

**Service:** `IRealtimeNoteService` → `RealtimeNoteService`
- `SendNoteToEntityAsync(string entityType, Guid entityId, NoteResponse note)` - Send to entity group
- `SendNoteToStaffAsync(Guid idStaff, NoteResponse note)` - Send to staff
- `BroadcastNoteUpdateAsync(NoteResponse note)` - Broadcast to all

**Integration:**
- NoteMutation uses `IRealtimeNoteService` to send real-time notes on create/update/reply
- All notes linked to Lead/Customer/Deal are broadcast to subscribed clients

---

## 6. Bug Fixes

### 2026-05-16: NullReferenceException in CreateNoteAsync

**Issue:** `CreateNoteAsync` threw `NullReferenceException` at line 41 when calling `_staffRepository.GetStaffByIdAsync()`.

**Root Cause:** Constructor of `NoteHandler` was commented out, causing `_staffRepository` to be null.

**Fix:** Uncommented constructor to enable proper dependency injection:

```csharp
public NoteHandler(
    INoteRepository noteRepository,
    INoteMentionRepository noteMentionRepository,
    IStaffRepository staffRepository,
    INotificationRepository notificationRepository,
    IMapper mapper)
{
    _noteRepository = noteRepository;
    _noteMentionRepository = noteMentionRepository;
    _staffRepository = staffRepository;
    _notificationRepository = notificationRepository;
    _mapper = mapper;
}
```

**File:** `CustomerManagement.Application\UseCases\NoteHandler.cs`

---

## 7. Implementation Order

1. Entity + Constants + Exceptions
2. Repository Interface + Implementation
3. DTOs + Mapper
4. Handler
5. GraphQL Input Types
6. Query + Mutation
7. Đăng ký services + Migration