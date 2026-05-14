# NHÓM 6: NHẬT KÝ HỆ THỐNG (Audit Log)

## Mục tiêu
Ghi lại tất cả thay đổi trong hệ thống để phục vụ compliance, troubleshooting và theo dõi hoạt động.

---

## 1. Database Schema

### AuditLog Entity
```
AuditLog:
- IdLog (PK, Guid)
- Action (NVARCHAR50) -- CREATE, UPDATE, DELETE, RESTORE, ASSIGN, LOGIN, LOGOUT
- EntityType (NVARCHAR50) -- Staff, Lead, Customer, Contact, Deal, Task, Note
- EntityId (GUID) -- Id của entity thay đổi
- OldValues (JSONB, NULLABLE) -- Giá trị trước thay đổi
- NewValues (JSONB, NULLABLE) -- Giá trị sau thay đổi
- IdStaff (FK -> persons.id, NULLABLE) -- Người thực hiện (NULL nếu là system)
- StaffName (NVARCHAR200) -- Tên người thực hiện (denormalized)
- IpAddress (NVARCHAR50, NULLABLE)
- UserAgent (NVARCHAR500, NULLABLE)
- Timestamp (DATETIME)
- Description (NVARCHAR500) -- Mô tả ngắn: "Staff 'A' updated Lead 'B'"
```

---

## 2. Files cần tạo mới

### Domain Layer
- `Domain/Entities/AuditLog.cs`
- `Domain/Constant/AuditActionConstant.cs`
- `Domain/Constant/AuditEntityTypeConstant.cs`
- `Domain/Exception/AuditLogNotFoundException.cs`

### Application Layer
- `DTOs/Response/AuditLogResponse.cs`
- `DTOs/Response/AuditLogListResponse.cs`
- `DTOs/Response/AuditStatisticsResponse.cs`
- `Interfaces/IAuditLogRepository.cs`
- `UseCases/AuditLogHandler.cs`

### Infrastructure Layer
- `Repositories/AuditLogRepository.cs`
- `Mapping/AuditLogMapper.cs`

### API Layer
- `Input/Type/Enums/AuditActionType.cs`
- `Input/Type/Enums/AuditEntityType.cs`
- `Query/AuditLogQuery.cs`
- `Mutation/AuditLogMutation.cs` -- Internal only, không expose cho client

---

## 3. API Endpoints

### Mutations (Internal - không expose)
- `createAuditLog(...)` -- Internal method called by handlers

### Queries
- `getAuditLogs(entityType, entityId, fromDate, toDate)` - Lịch sử thay đổi
- `getAuditLogsByStaff(idStaff, fromDate, toDate)` - Thay đổi của một staff
- `getAuditLogsByAction(action, fromDate, toDate)` - Lọc theo action
- `getEntityHistory(entityType, entityId)` - Toàn bộ lịch sử entity
- `getAuditStatistics(fromDate, toDate)` - Thống kê audit

---

## 4. Tracked Actions

| Action | Entity Types | Khi nào |
|--------|--------------|---------|
| CREATE | Tất cả | Tạo mới entity |
| UPDATE | Tất cả | Cập nhật fields |
| DELETE | Tất cả | Xóa (soft delete) |
| RESTORE | Tất cả | Khôi phục |
| ASSIGN | Task, TeamMember | Gán task, thêm team |
| LOGIN | Staff | Staff đăng nhập |
| LOGOUT | Staff | Staff đăng xuất |

---

## 5. Implementation Approach

### AOP Pattern (Aspect Oriented Programming)
Tạo extension method hoặc decorator để auto-log:

```csharp
// Example: Audit helper method
public async Task UpdateWithAuditAsync<T>(T entity, string action, Guid staffId)
    where T : BaseEntity
{
    var oldValues = GetOldValues(entity);
    // Update entity
    await _repository.UpdateAsync(entity);
    var newValues = GetNewValues(entity);
    await _auditLogRepository.LogAsync(action, entityType, entityId, oldValues, newValues, staffId);
}
```

### Manual Integration
- Thêm audit log calls trong handlers sau khi thay đổi thành công
- Ví dụ: `await _auditLogRepository.LogAsync("UPDATE", "Lead", idLead, oldLead, updatedLead, staffId);`

---

## 6. Data Retention

- Lưu trữ audit logs vĩnh viễn (không xóa)
- Có thể partition theo năm để query hiệu quả
- Export archive sau 3 năm

---

## 7. Business Rules

### JSONB Storage
- OldValues/NewValues lưu dạng JSONB
- Dễ dàng query nested fields
- PostgreSQL hỗ trợ JSONB operators (@>, ? , etc.)

### Performance
- Index trên: EntityType, EntityId, Timestamp, IdStaff
- Partition theo tháng/năm nếu volume lớn

### Privacy
- Không log: password, token, email (nếu cần masking)
- Sanitize sensitive data trước khi log

---

## 8. Implementation Order

1. Entity + Constants + Exceptions
2. Repository + Mapper
3. Handler + Query
4. Thêm audit log vào existing handlers (StaffHandler, LeadHandler, etc.)
5. Tạo migration
6. Không cần Mutation vì audit log chỉ được tạo nội bộ