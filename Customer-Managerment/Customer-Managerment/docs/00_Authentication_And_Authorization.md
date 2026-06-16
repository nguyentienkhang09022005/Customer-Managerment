# NHÓM 0: XÁC THỰC & PHÂN QUYỀN (Authentication & Authorization)

## Mục tiêu
Quản lý đăng nhập, refresh token, quên mật khẩu (OTP email) và phân quyền truy cập cho 3 role chính của hệ thống.

---

## 1. Phân quyền 3 Role chính

| Role    | Quyền hạn chính                                                                                              |
|---------|--------------------------------------------------------------------------------------------------------------|
| ADMIN   | Toàn quyền: quản lý Staff, xem toàn bộ Deal/Lead/Customer, nhận thông báo khi Task hoàn thành, xem Audit Log. |
| MANAGER | Quản lý nhóm, xem dashboard tổng quan, phân công Task, theo dõi hiệu suất team.                              |
| STAFF   | Nhân viên kinh doanh: tạo Deal/Contact/Note cho bản thân, được gán Task, dùng Chat AI, xem dữ liệu của mình. |

> **Lưu ý kỹ thuật:** Enum `StaffRole` trong `CustomerManagement.Api/Input/Type/Enums/StaffRoleType.cs` hiện chỉ có 2 giá trị `ADMIN` và `STAFF`. Role `MANAGER` được tham chiếu trong các tài liệu nghiệp vụ nhưng chưa được mở rộng trong enum. Khi cần bổ sung Manager cần: thêm giá trị vào enum, mở rộng validator trong `StaffHandler`, cập nhật logic check role tại các `Mutation/Query`.

### Phân quyền theo GraphQL (cơ chế hiện tại)
- `DealQuery.GetDeals()`: chỉ ADMIN mới lấy được toàn bộ; STAFF dùng `getMyDeals` (`DealQuery.cs:32-38`).
- `DealQuery.GetDealById()`: ADMIN lấy mọi deal; STAFF chỉ xem deal mình là OWNER hoặc là MEMBER trong `team_members` (`DealQuery.cs:71-87`).
- `DealMutation.CreateDealAsync()`: STAFF bị ép `IdStaff = currentUserId` (không thể gán deal cho người khác); ADMIN giữ nguyên `IdStaff` từ request (`DealMutation.cs:28-31`).
- `TaskHandler.UpdateTaskStatusAsync()`: khi task chuyển sang `COMPLETED` sẽ gửi notification cho tất cả `Role = "ADMIN"` (`TaskHandler.cs:195-211`).
- `ReportHandler.GetTopPerformingStaffAsync()`: lọc staff theo `Role = "Staff"` để xếp hạng (`ReportHandler.cs:159`).

### Claim trong JWT
`TokenService.GenerateAccessToken()` sinh các claim:
- `sub` = `Id` (Guid staff)
- `email` = email
- `ClaimTypes.Name` = fullname
- `ClaimTypes.Role` = role string
- `jti` = unique token id

---

## 2. Cấu trúc file

### Domain Layer
- `Domain/Entities/Person.cs` — discriminator `PersonType { Staff, Lead, Customer }`, các trường staff: `Username`, `PasswordHash`, `Role`, `Salary`, `Status`, `LastActiveAt`.

### Application Layer
- `Application/Handlers/Auth/AuthenticationHandler.cs`
- `Application/Handlers/Auth/ForgotPasswordHandler.cs`
- `Application/Handlers/Auth/ForgotPasswordCacheData.cs`
- `Application/Interfaces/ITokenService.cs`, `IRefreshTokenService.cs`

### Infrastructure Layer
- `Infrastructure/Services/TokenService.cs` — sinh JWT access + refresh.
- `Infrastructure/Services/RefreshTokenService.cs` — lưu/đọc/xoá refresh token trên Redis với key `refresh_token:{idUser}`.

### API Layer
- `Api/Mutation/AuthenticationMutation.cs`
- `Api/Mutation/ForgotPasswordMutation.cs`
- `Api/MiddleWare/GraphQLExceptionFilter.cs` — map `DomainException` -> status code, `AUTH_NOT_AUTHENTICATED` -> 401, `AUTH_NOT_AUTHORIZED` -> 403.

---

## 3. Luồng nghiệp vụ

### 3.1. Login
**Trigger:** `mutation login(authenticationRequest)`.
1. `AuthenticationHandler.LoginHandleAsync()` lookup staff theo `username` (`StaffRepository.GetStaffByUsernameAsync`).
2. So khớp `BCrypt.Verify(password, PasswordHash)`. Throw `DomainException("Tên đăng nhập không đúng!", 409)` / `("Mật khẩu không đúng!", 409)` nếu sai.
3. `TokenService.GenerateAccessToken()` tạo access JWT (claim `sub`, `email`, `role`...).
4. `TokenService.GenerateRefreshToken()` tạo refresh JWT.
5. `RefreshTokenService.SaveRefreshTokenAsync()` lưu refresh token vào Redis key `refresh_token:{staffId}` với TTL = `JwtSettings:RefreshTokenExpirationDays`.
6. Set cookie `refreshToken` (`HttpOnly`, `Secure`, `SameSite=None`, `Path=/`).
7. Trả về `AuthenticationResponse { Token, InfStaff }`.

### 3.2. Refresh Token
**Trigger:** `mutation refreshToken()` (đánh dấu `[AllowAnonymous]`).
1. Đọc cookie `refreshToken`. Throw 400 nếu rỗng.
2. `VerifyToken()` validate signature/expiry/audience/issuer.
3. Lấy `sub` claim -> staffId.
4. So sánh refresh token trong cookie với refresh token lưu trong Redis. Throw 400 nếu không khớp/không tồn tại.
5. `StaffRepository.GetStaffByIdAsync()` lấy staff hiện tại.
6. Sinh access token mới, trả về `AuthenticationResponse`. **Không** rotate refresh token (giữ nguyên token cũ).

### 3.3. Logout
**Trigger:** `mutation logout()`.
1. Đọc cookie `refreshToken`. Throw 400 nếu rỗng.
2. Verify token, lấy `sub` claim.
3. `RefreshTokenService.DeleteRefreshTokenAsync()` xoá key Redis.
4. `Response.Cookies.Delete("refreshToken")` xoá cookie.
5. Trả về `"Đăng xuất thành công!"`.

### 3.4. Introspect Token
**Trigger:** `mutation introspect(token)`.
- Parse JWT, trả về `{ Valid: bool, IdUser: Guid }`. Dùng để client kiểm tra token còn hạn mà không cần GraphQL auth header.

### 3.5. Forgot Password — Gửi OTP
**Trigger:** `mutation sendOTPForgotPassword({ email })`.
1. `ForgotPasswordHandler.SendOTPForForgotPasswordHandleAsync()`.
2. Tìm staff theo email. Throw `404 Email không tồn tại!` nếu không có.
3. Sinh OTP 6 số ngẫu nhiên (100000-999999).
4. Lưu `ForgotPasswordCacheData { Otp, Email }` vào `IMemoryCache` với key `OTP_ForgotPassword_{email}` và TTL **2 phút**.
5. Gửi email HTML qua FluentEmail + SendGrid với subject `"Mã OTP hỗ trợ quên mật khẩu"`, body chứa OTP.
6. Nếu gửi thất bại throw 500; thành công trả về `"OTP đã được gửi đến bạn!"`.

### 3.6. Forgot Password — Xác nhận OTP & đổi mật khẩu
**Trigger:** `mutation confirmOTPForgotPassword({ email, otp, newPassword, confirmPassword })`.
1. Lấy cache `OTP_ForgotPassword_{email}`. Throw 500 nếu không có (hết hạn hoặc chưa gửi).
2. So sánh `cacheData.Otp` với OTP client gửi lên. Throw 400 nếu sai.
3. So sánh `newPassword == confirmPassword`. Throw 400 nếu lệch.
4. Tìm staff theo email, hash mật khẩu mới bằng BCrypt, `UpdateStaffAsync` lưu DB.
5. `MemoryCache.Remove(cacheKey)` xoá OTP.
6. Trả về `"Đổi mật khẩu thành công!"`.

---

## 4. Cấu hình liên quan

```jsonc
// appsettings.json
"JwtSettings": {
  "SecretKey": "...",            // HS256 signing key
  "Issuer": "...",
  "Audience": "...",
  "AccessTokenExpirationMinutes": 60,
  "RefreshTokenExpirationDays": 7
}
```

- Tất cả secret được load từ biến môi trường qua `DotNetEnv.Env.Load()` trong `Program.cs`.
- Cookie `refreshToken` yêu cầu HTTPS (`Secure=true`).

---

## 5. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| BCrypt hash | Mật khẩu được hash bằng BCrypt trước khi lưu (`StaffRepository.AddStaffAsync`). |
| OTP TTL | 2 phút, lưu trong IMemoryCache (process-local, mất khi restart). |
| Refresh token rotation | KHÔNG rotate — cookie giữ nguyên đến khi logout hoặc hết hạn Redis. |
| Role trong JWT | Claim `ClaimTypes.Role` lưu đúng giá trị `ADMIN`/`STAFF` (uppercase). |
| DomainException | Mọi lỗi nghiệp vụ throw `DomainException(message, statusCode)` -> GraphQL filter trả về `extensions.status`. |
| Auth filter | Mọi GraphQL endpoint yêu cầu `[Authorize]` trừ `login`, `refreshToken`, `sendOTPForgotPassword`, `confirmOTPForgotPassword`, `introspect` (mặc định toàn bộ `[AddAuthorization]`). |

---

## 6. Lưu ý bảo mật

- Cần đảm bảo `JwtSettings:SecretKey` đủ mạnh (>= 256 bit) trong production.
- `ClockSkew = TimeSpan.Zero` trong `Program.cs` — token hết hạn chính xác tới phút.
- OTP chỉ lưu IMemoryCache nên horizontal scaling có thể khiến OTP không truy cập được giữa các instance — cân nhắc chuyển sang Redis nếu chạy multi-pod.
- Nên thêm rate-limit cho `login` và `sendOTPForgotPassword` để chống brute-force.
