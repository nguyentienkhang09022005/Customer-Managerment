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

---

## 9. Bug Fixes

### 2026-05-16: ObjectDisposedException in AuditLog Queries

**Issue:** Multiple AuditLog queries threw `ObjectDisposedException: Cannot access a disposed context instance` in `GetAuditLogsAsync`, `GetAuditLogsByStaffAsync`, `GetAuditLogsByActionAsync`, `GetEntityHistoryAsync`.

**Root Cause:** `IAuditLogRepository` methods (`GetAllAsync`, `GetByEntityAsync`, `GetByStaffAsync`, `GetByActionAsync`, `GetByDateRangeAsync`, `GetEntityHistoryAsync`) all returned `Task<IQueryable<AuditLog>>` with `await using var context` pattern. The `IQueryable` was lazy-evaluated after the DbContext was disposed.

**Fix:** Changed all 6 methods from `Task<IQueryable<AuditLog>>` to `Task<List<AuditLog>>` with `.ToListAsync()` before context disposal. Updated callers in `AuditLogQuery.cs` and `AuditLogHandler.cs` to use `List<AuditLog>` instead of `IQueryable<AuditLog>`.

```csharp
// Before
public async Task<IQueryable<AuditLog>> GetAllAsync()
{
    await using var context = _contextFactory.CreateDbContext();
    return context.AuditLogs.OrderByDescending(a => a.Timestamp).AsNoTracking();
}

// After
public async Task<List<AuditLog>> GetAllAsync()
{
    await using var context = _contextFactory.CreateDbContext();
    return await context.AuditLogs
        .OrderByDescending(a => a.Timestamp)
        .AsNoTracking()
        .ToListAsync();
}
```

**Files Modified:**
- `CustomerManagement.Application\Interfaces\IAuditLogRepository.cs`
- `CustomerManagement.Infrastructure\Repositories\AuditLogRepository.cs`
- `CustomerManagement.Api\Query\AuditLogQuery.cs`
- `CustomerManagement.Application\UseCases\AuditLogHandler.cs`

---

### 2026-05-16: Missing Fields in AuditStatisticsResponse

**Issue:** `getAuditStatistics` query returned GraphQL error: "The field `totalActions` does not exist on the type `AuditStatisticsResponse`", etc.

**Root Cause:** `AuditStatisticsResponse` DTO was missing fields that client query required: `totalActions`, `totalEntities`, `topActions`, `topEntities`.

**Fix:** Added computed properties and new DTO classes to `AuditStatisticsResponse.cs`:

```csharp
public int totalActions => ActionCounts.Count;
public int totalEntities => EntityTypeCounts.Count;
public List<TopActionItem> topActions => ActionCounts
    .OrderByDescending(x => x.Value)
    .Take(5)
    .Select(x => new TopActionItem { action = x.Key, count = x.Value })
    .ToList();
public List<TopEntityItem> topEntities => EntityTypeCounts
    .OrderByDescending(x => x.Value)
    .Take(5)
    .Select(x => new TopEntityItem { entityType = x.Key, count = x.Value })
    .ToList();

public class TopActionItem
{
    public string action { get; set; } = "";
    public int count { get; set; }
}

public class TopEntityItem
{
    public string entityType { get; set; } = "";
    public int count { get; set; }
}
```

**File Modified:**
- `CustomerManagement.Application\DTOs\Response\AuditStatisticsResponse.cs`