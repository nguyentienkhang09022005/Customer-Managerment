# TỔNG HỢP KIẾN TRÚC & TÀI LIỆU NGHIỆP VỤ BACKEND (Refactoring Documentation)

> **Mục đích:** File tổng hợp này cung cấp cái nhìn toàn cảnh về kiến trúc hệ thống, stack công nghệ, phân quyền 3 role, các quyết định thiết kế chính và hướng dẫn mở rộng. Mỗi nhóm chức năng có file chi tiết riêng (`00_..` đến `14_..`).

---

## 1. Tổng quan hệ thống

**Customer-Managerment** là hệ thống CRM (Customer Relationship Management) tập trung vào quản lý:
- Khách hàng tiềm năng (Lead)
- Khách hàng chính thức (Customer)
- Giao dịch (Deal)
- Hoạt động liên hệ (Contact)
- Công việc (Task) & Ghi chú (Note) với mention realtime
- Lịch hẹn (Calendar) với reminder tự động
- Phân quyền nhóm (Team Assignment)
- Nhật ký kiểm toán (Audit Log)
- Trợ lý AI (CRMie) tích hợp Groq
- Thống kê, báo cáo & xuất Excel

---

## 2. Tech Stack

| Layer | Công nghệ |
|-------|-----------|
| Runtime | .NET 9 (ASP.NET Core) |
| API | GraphQL (HotChocolate) + REST (Controller) cho File Upload |
| ORM | Entity Framework Core (Code-First) + Npgsql (PostgreSQL) |
| Cache | Redis (StackExchange.Redis + `IDistributedCache`) |
| Realtime | SignalR (`/hubs/notifications`, `/hubs/notes`) |
| Auth | JWT (HS256) + Cookie cho refresh token |
| Email | FluentEmail + SendGrid |
| AI | Groq API (Llama 4 Scout) |
| Excel | EPPlus (NonCommercial) |
| Object Mapping | AutoMapper |
| Password | BCrypt.Net |
| Validation | Custom `DomainException` |

---

## 3. Kiến trúc phân lớp (Clean Architecture)

```
┌─────────────────────────────────────────────────┐
│  CustomerManagement.Api                         │
│  - GraphQL Query / Mutation / Hub               │
│  - Input Types / Response Types                 │
│  - Middleware (GraphQLExceptionFilter)          │
└─────────────────────────────────────────────────┘
                       │ depends on
┌─────────────────────────────────────────────────┐
│  CustomerManagement.Application                 │
│  - UseCases / Handlers                          │
│  - DTOs (Requests / Responses)                  │
│  - Interfaces (repositories, services)          │
└─────────────────────────────────────────────────┘
                       │ depends on
┌─────────────────────────────────────────────────┐
│  CustomerManagement.Domain                      │
│  - Entities, Value Objects                      │
│  - Constants (Status, Role, Type)               │
│  - Exceptions (Domain, Validation, NotFound)    │
└─────────────────────────────────────────────────┘
                       │ implemented by
┌─────────────────────────────────────────────────┐
│  CustomerManagement.Infrastructure              │
│  - Repositories (EF Core)                       │
│  - Services (Token, RefreshToken, Chat)         │
│  - BackgroundService (CalendarReminder)         │
│  - DbContext + Migrations                       │
└─────────────────────────────────────────────────┘
```

---

## 4. Phân quyền 3 Role

Xem chi tiết tại `00_Authentication_And_Authorization.md`.

| Role | Code value | Quyền |
|------|-----------|-------|
| **ADMIN** | `"ADMIN"` | Toàn quyền hệ thống: CRUD staff, xem toàn bộ deal/lead/customer, nhận notif khi task complete, xem audit log & dashboard. |
| **MANAGER** | *(chưa trong enum)* | Quản lý nhóm, theo dõi hiệu suất, phân công task. (Hiện tại hệ thống CHƯA có enum value MANAGER — cần mở rộng.) |
| **STAFF** | `"STAFF"` | Nhân viên kinh doanh: tạo deal/contact/note cho bản thân, nhận task, dùng chat AI, xem dữ liệu của mình. |

### Phân quyền theo GraphQL
- `DealQuery.getDeals()` — chỉ ADMIN.
- `DealQuery.getMyDeals()` — STAFF dùng thay thế.
- `DealMutation.createDeal` — STAFF bị ép `IdStaff = currentUserId`.
- `TaskHandler.UpdateTaskStatusAsync` — khi COMPLETED, gửi notif cho tất cả ADMIN.
- `ReportHandler.GetTopPerformingStaffAsync` — lọc staff theo `Role = "Staff"`.
- **Thiếu:** chưa có attribute `[Authorize(Roles = "ADMIN")]` ở GraphQL — phân quyền chủ yếu thực hiện trong code handler/mutation. Có thể bổ sung bằng HotChocolate filtering.

---

## 5. Cấu trúc Database (Tables)

| Table | Mục đích |
|-------|----------|
| `persons` | Bảng TPH (Table-per-Hierarchy) chứa Staff/Lead/Customer phân biệt bằng `Discriminator`. |
| `contacts` | Hoạt động liên hệ giữa Staff và Lead/Customer. |
| `deals` | Giao dịch bán hàng gắn với Customer. |
| `tasks` | Công việc được giao cho Staff. |
| `notes` | Bình luận gắn với entity nghiệp vụ. |
| `note_mentions` | Quan hệ Note ↔ Staff được mention. |
| `calendar_events` | Sự kiện lịch. |
| `event_participants` | Quan hệ Event ↔ Staff tham gia. |
| `notifications` | Thông báo cho từng Staff. |
| `team_members` | Quan hệ Staff ↔ Lead/Deal (OWNER/MEMBER/VIEWER). |
| `audit_logs` | Nhật ký kiểm toán. |
| `staff_activity_logs` | Lịch sử LOGIN / STATUS_CHANGE. |

---

## 6. Các module chức năng (chi tiết từng file)

| Nhóm | File | Mô tả |
|------|------|-------|
| 0 | `00_Authentication_And_Authorization.md` | Login, Refresh Token, Logout, Introspect, Forgot Password (OTP), phân quyền 3 role. |
| 1 | `01_Staff_Management.md` | CRUD nhân viên. |
| 2 | `02_Lead_Management.md` | CRUD Lead + Import Excel. |
| 3 | `03_Customer_Management.md` | CRUD Customer + Import Excel + Conversion từ Lead. |
| 4 | `04_Contact_Management.md` | Hoạt động liên hệ + auto conversion Lead→Customer khi Status=SUCCESS. |
| 5 | `05_Deal_Management.md` | CRUD Deal + phân quyền ADMIN/STAFF + auto OWNER. |
| 6 | `06_Task_Management.md` | CRUD Task + giao việc + đổi status + notif cho ADMIN khi complete. |
| 7 | `07_Note_Management.md` | CRUD Note + pin/unpin + reply + mention `@username` + realtime SignalR. |
| 8 | `08_Notification_Management.md` | Notification center + mark read + realtime push + Calendar reminder. |
| 9 | `09_Calendar_Scheduling.md` | CRUD Event + participants + status phản hồi + background reminder. |
| 10 | `10_Team_Assignment.md` | Quản lý team cho Lead/Deal + transfer OWNER + bảo vệ OWNER cuối. |
| 11 | `11_Audit_Log.md` | Query nhật ký kiểm toán (lọc theo entity/staff/action/range). |
| 12 | `12_Statistics_And_Reports.md` | Dashboard summary + Revenue chart + Pipeline funnel + Staff performance + Lead conversion + Export Excel. |
| 13 | `13_AI_Chat.md` | CRMie chatbot với Groq + lịch sử Redis + system prompt bảo mật. |
| 14 | `14_Staff_Presence.md` | Trạng thái online/offline/busy/away + activity log. |

---

## 7. Luồng nghiệp vụ xuyên suốt (Cross-cutting)

### 7.1. Authentication flow
```
Client → mutation login → AuthenticationHandler
  → BCrypt verify → JWT access + refresh
  → Redis save refreshToken (key: refresh_token:{idStaff})
  → Set cookie refreshToken (HttpOnly, Secure, SameSite=None)
  → Return { token, infStaff }

Client → cookie tự động gửi kèm → mutation refreshToken
  → Verify JWT → check Redis match → sinh access token mới (giữ refresh cũ)

Client → mutation logout
  → Verify cookie → xoá Redis key → clear cookie
```

### 7.2. Realtime flow (Note + Notification)
```
Mutation tạo Note / addParticipant / sendChatMessage
  → Lưu DB
  → NotificationHandler.CreateNotification (nếu có)
  → IRealtimeNotificationService.SendNotificationToStaffAsync
    → SignalR Clients.Group("staff_{idStaff}").SendAsync("ReceiveNotification", payload)

Client subscribe hub /hubs/notifications
  → joinStaffGroup(idStaff)
  → Lắng nghe event ReceiveNotification
```

### 7.3. Lead → Customer conversion flow
```
ContactHandler.UpdateContactAsync (status = SUCCESS)
  → Lấy Lead theo Contact.IdLead
  → Check email không trùng Staff
  → lead.Discriminator = PersonType.Customer
  → UpdateLeadAsync (cùng bảng persons, cùng Id)
  → Response.navigation chuyển từ Lead → Customer
```

### 7.4. Deal ownership & visibility flow
```
DealMutation.CreateDealAsync
  → DealHandler.CreateDealAsync
    → Auto-add TeamMember { Role=OWNER, CanEdit=true, CanDelete=true } cho IdStaff

DealQuery.getDeals (ADMIN only)
  → trả toàn bộ

DealQuery.getMyDeals (STAFF)
  → lấy teamMemberships của currentStaff
  → lọc Deal WHERE IdStaff = currentUser OR IdDeal IN (team deal list)
```

---

## 8. Tích hợp & Background Jobs

### 8.1. Calendar Reminder
- `CalendarReminderService` chạy mỗi 5 phút.
- Quét event `StartTime ∈ [now+5min, now+10min]`, `ReminderMinutes > 0`, `Status = SCHEDULED`.
- Tạo Notification cho organizer + participants. Dedup 10 phút.

### 8.2. Elasticsearch (đã comment)
- Tất cả handler đều có comment `// await _elasticsearchService.IndexAsync(response, "...")`.
- Tính năng search bằng ES đang được tắt (xem `Program.cs` comment `// .AddTypeExtension<StaffElasticSearchQuery>()`).
- Khi bật lại cần: cấu hình `ES__URL`, register `IElasticsearchService`, chạy migration index.

### 8.3. SignalR Hubs
- `/hubs/notifications` — group `staff_{idStaff}`.
- `/hubs/notes` — group `{entityType}_{entityId}` hoặc `staff_{idStaff}`.

---

## 9. Cấu hình môi trường

Tất cả secret được load qua `DotNetEnv.Env.Load()` trong `Program.cs`. Các biến chính:

```bash
CONNECTIONSTRINGS__PostgreSQLConnection
APPSETTINGS__SECRETKEY          # JWT signing key
APPSETTINGS__ISSUER
APPSETTINGS__AUDIENCE
APPSETTINGS__ACCESSTOKENEXP     # Phút
APPSETTINGS__REFRESHTOKENEXP    # Ngày
REDISSETTINGS__HOST
REDISSETTINGS__PORT
REDISSETTINGS__PASSWORD
SENDER_APIKEY                   # SendGrid
SENDER_EMAIL
SENDER_NAME
GROQ__APIKEY
GROQ__APIURL
# ES__URL                       # Elasticsearch (đang tắt)
```

---

## 10. Quyết định thiết kế quan trọng

| Quyết định | Lý do |
|-----------|-------|
| GraphQL (HotChocolate) thay vì REST | Single endpoint, client chủ động chọn field, dễ thêm field mới. |
| TPH inheritance (Person) | Lead và Customer dùng chung bảng, chuyển đổi chỉ đổi Discriminator. |
| Soft delete toàn bộ | Audit, restore dễ dàng. |
| BCrypt cho password | Chuẩn industry, salt tự động. |
| Refresh token rotation KHÔNG rotate | Đơn giản, nhưng cần theo dõi để tránh token bị đánh cắp sử dụng lâu. |
| OTP lưu IMemoryCache | Nhanh nhưng mất khi restart và không share giữa nhiều instance. |
| Auto OWNER khi tạo Deal | Giảm friction, mặc định người tạo chịu trách nhiệm. |
| 2 cách tạo notification | Cách A (chỉ DB) và Cách B (DB + realtime) — chưa thống nhất. |
| Calendar Reminder chỉ lưu DB | Không push realtime — cần bổ sung. |
| `exportDealsReport` trả binary trong GraphQL | Có thể gây nặng response — nên upload S3/Blob. |
| EPPlus NonCommercial | OK cho dev, cần license cho production. |

---

## 11. Điểm cần cải thiện (Backlog)

1. **Hoàn thiện role MANAGER**: mở rộng `StaffRole` enum, cập nhật validator + role-check logic.
2. **Đồng nhất notification**: mọi nơi tạo notification nên dùng `NotificationMutation.CreateNotification` (có realtime) thay vì thao tác DB trực tiếp.
3. **Audit log tự động**: thêm `AuditLogHandler.LogAsync` ở tất cả CRUD handler.
4. **Auto OFFLINE staff**: bổ sung background job check `LastActiveAt > 15 phút` -> set OFFLINE.
5. **OTP chuyển sang Redis**: cho phép horizontal scaling.
6. **Refresh token rotation**: rotate refresh token mỗi lần refresh để tăng bảo mật.
7. **Streaming Groq response**: dùng SSE/WebSocket để user thấy AI trả lời từng phần.
8. **Mask sensitive data trước khi gửi AI**: ẩn email, phone trong prompt để bảo vệ PII.
9. **Rate limit**: chống brute-force login + OTP spam.
10. **Bật Elasticsearch**: hoàn thiện search customers/leads/deals.
11. **StaffPerformance.TasksCompleted**: tính từ `tasks` table thay vì hardcode 0.
12. **LeadConversion.ConversionBySources**: aggregate theo `Resource` của Lead.
13. **Thêm `[Authorize(Roles=...)]` ở GraphQL**: chuẩn hoá phân quyền thay vì check trong code.
14. **Export Excel upload lên storage**: trả URL thay vì binary trong GraphQL.

---

## 12. Cách chạy local

```bash
# 1. Cài PostgreSQL + Redis, set env vars (xem .env.example nếu có)
# 2. Apply migrations
dotnet ef database update --project CustomerManagement.Infrastructure

# 3. Chạy API
dotnet run --project CustomerManagement.Api

# GraphQL endpoint: https://localhost:5114/graphql
# Voyager: https://localhost:5114/voyager
# SignalR notifications: /hubs/notifications
# SignalR notes: /hubs/notes
```

---

## 13. Glossary

- **TPH (Table-per-Hierarchy)**: pattern EF Core gộp các class cùng hierarchy vào 1 bảng, phân biệt bằng cột Discriminator.
- **OWNER**: nhân viên chịu trách nhiệm chính cho Lead/Deal, có quyền xoá + sửa + chuyển nhượng.
- **MEMBER**: nhân viên hỗ trợ, có thể edit tuỳ `CanEdit`.
- **VIEWER**: chỉ xem.
- **CRMie**: tên trợ lý AI nội bộ.
- **Reminder**: nhắc nhở trước khi sự kiện bắt đầu (phút).
- **Mention**: gắn tag `@username` trong note để kéo staff khác vào cuộc.
- **Pipeline Funnel**: phễu bán hàng OPEN -> NEGOTIATING -> WON/LOST.
