# NHÓM 7: QUẢN LÝ GHI CHÚ / BÌNH LUẬN (Note Management)

## Mục tiêu
Ghi chú/bình luận realtime trên các entity nghiệp vụ (Lead, Customer, Deal, Task). Hỗ trợ reply threading, ghim, mention nhân viên bằng `@username` (tự động tạo notification).

---

## 1. Database Schema

```csharp
Note:
  IdNote (PK, Guid)
  Content         (NVARCHAR 5000, required)
  Type            (NVARCHAR)  // COMMENT | UPDATE | SYSTEM
  IsPinned        (BOOL)
  CreatedAt, UpdatedAt
  IsDeleted, DeletedAt
  IdStaff         (FK -> Person Staff, tác giả)
  LinkedEntityType (NVARCHAR)  // "Lead" | "Customer" | "Deal" | "Task"
  LinkedEntityId   (Guid)
  ParentNoteId    (Guid?, nullable)  // reply threading

NoteMention:
  IdMention (PK, Guid)
  IdNote (FK)
  IdStaffMentioned (FK -> Person Staff)
  CreatedAt
```

---

## 2. Constants

### `NoteTypeConstant`
- `COMMENT` (bình luận thường)
- `UPDATE` (cập nhật trạng thái)
- `SYSTEM` (hệ thống sinh)

---

## 3. Cấu trúc file

### Application Layer
- `Application/UseCases/NoteHandler.cs`
- `Application/Interfaces/INoteRepository.cs`, `INoteMentionRepository.cs`

### API Layer
- `Api/Mutation/NoteMutation.cs` — emit realtime qua `IRealtimeNoteService`.
- `Api/Query/NoteQuery.cs`
- `Api/Input/Type/NoteInputType.cs`

### Realtime
- `Api/Hubs/NoteHub.cs` (SignalR) — group theo `entityType_entityId` hoặc `staff_{idStaff}`.
- `Api/Services/RealtimeNoteService.cs` — gửi `ReceiveNote` event tới group.

---

## 4. GraphQL Endpoints

### Mutations
- `createNote(input: NoteInput!): NoteResponse!`
- `updateNote(input: NoteUpdateInput!, idNote: UUID!): NoteResponse!`
- `deleteNote(idNote: UUID!): Boolean!`
- `pinNote(idNote: UUID!): NoteResponse!`
- `unpinNote(idNote: UUID!): NoteResponse!`
- `replyNote(idNote: UUID!, parentId: UUID!): NoteResponse!`

### Queries
- `getNotesByEntity(entityType: String!, entityId: UUID!): [NoteResponse!]!`
- `getPinnedNotes(entityType: String!, entityId: UUID!): [NoteResponse!]!`
- `getNoteById(idNote: UUID!): NoteResponse`

### SignalR Hub (`/hubs/notes`)
- `joinEntityGroup(entityType, entityId)` — join group `{lowercaseEntityType}_{id}`.
- `leaveEntityGroup(entityType, entityId)`.
- `joinStaffGroup(idStaff)` — join group `staff_{idStaff}` (cho mention).
- Server push: `ReceiveNote` event với payload `NoteResponse`.

---

## 5. Luồng nghiệp vụ

### 5.1. Tạo Note
1. `NoteMutation.CreateNoteAsync(input)` map -> `NoteCreationRequest`.
2. `NoteHandler.CreateNoteAsync`:
   - `ValidateNoteCreation`:
     - `Content` required, max 5000.
     - `Type` ∈ {COMMENT, UPDATE, SYSTEM}.
     - `LinkedEntityType` ∈ {Lead, Customer, Deal} (dùng chung `TaskLinkedEntityConstant`).
   - `StaffRepository.GetStaffByIdAsync(IdStaff)` — throw `StaffNotFoundException`.
   - AutoMapper -> `Note`. `AddNoteAsync`.
   - `ProcessMentionsAsync(note, staff.Fullname)`:
     - Regex `@(\w+)` tìm các mention trong `Content`.
     - Với mỗi username khớp:
       - `StaffRepository.GetStaffByUsernameAsync(username)`.
       - Nếu tìm thấy:
         - Tạo `NoteMention { IdNote, IdStaffMentioned }`.
         - Tạo `Notification`:
           - Title: "Bạn được nhắn đến trong một bình luận"
           - Message: "{authorName} đã nhắn đến bạn trong một bình luận: {50 ký tự đầu của Content}..."
           - Type: `MENTION`
           - Related: `Note`.
3. **Realtime push:** `IRealtimeNoteService.SendNoteToEntityAsync(entityType, entityId, noteResponse)` — gửi `ReceiveNote` tới group `entityType_entityId`.

### 5.2. Cập nhật Note
- Cập nhật `Content` (nếu có) hoặc `IsPinned` (nếu có). KHÔNG tự động chạy lại ProcessMentions — chỉ khi tạo mới.
- Realtime push tới group entity.

### 5.3. Pin / Unpin
- Set `IsPinned = true/false`. Realtime push.

### 5.4. Reply threading
1. Lấy parent note. Throw `NoteNotFoundException("Parent note không tìm thấy!")` nếu rỗng.
2. Lấy note hiện tại (cần pre-create qua createNote trước).
3. Set `note.ParentNoteId = parentId`, `note.LinkedEntityType = parentNote.LinkedEntityType`, `note.LinkedEntityId = parentNote.LinkedEntityId`.
4. Cập nhật DB. Realtime push.

### 5.5. Xoá Note
- Soft delete. **Không** xoá `NoteMention` (giữ audit).

---

## 6. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Regex mention | Pattern `@(\w+)` — chỉ bắt được username không dấu, không space. |
| Mention cache | Nếu username không tồn tại trong hệ thống -> bỏ qua. |
| LinkedEntity | Chỉ cho phép Lead/Customer/Deal (validate lúc tạo). Sau khi tạo có thể reply với parent thuộc entity khác nhưng vẫn kế thừa entity của parent. |
| Realtime | Mọi create/update/pin/unpin/reply đều broadcast tới group entity tương ứng. |
| Pin multi-entity | Một note có thể ghim; query `getPinnedNotes` lọc theo (entityType, entityId). |
| Threading | Reply có thể chain nhiều cấp (hiện code không giới hạn depth). |

---

## 7. Tích hợp ngang

- **Notification**: mention -> notification cho staff được nhắc.
- **Realtime**: SignalR hub `NoteHub` broadcast mọi thay đổi.
- **LinkedEntity**: tham chiếu tới Lead/Customer/Deal/Task (nhưng validation hiện chỉ cho Lead/Customer/Deal).
- **Audit**: hiện CHƯA ghi audit log cho note (chỉ realtime + DB).
