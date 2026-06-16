# NHÓM 13: TRỢ LÝ AI CHAT (Groq Chat)

## Mục tiêu
Cung cấp chatbot AI (CRMie) hỗ trợ nhân viên tra cứu thông tin Lead/Customer/Deal/Contact. Lưu lịch sử hội thoại trên Redis. Bảo mật: AI không được tiết lộ email, ID, token, mật khẩu hoặc thực thi SQL.

---

## 1. Tech stack
- **Groq API** (LLM model: `meta-llama/llama-4-scout-17b-16e-instruct` mặc định).
- **Redis (DistributedCache)** lưu lịch sử chat, key `chat_history:{idStaff}`, TTL 1 ngày.
- **HttpClient** gọi Groq API với Bearer token.

---

## 2. Cấu trúc file

### Application Layer
- `Application/UseCases/ChatHandler.cs`
- `Application/Interfaces/IGroqService.cs`, `IChatHistoryService.cs`
- `Application/DTOs/Requests/ChatRequest.cs`, `GroqRequest.cs`
- `Application/DTOs/Response/ChatResponse.cs`, `GroqApiResponse.cs`, `MessageHistoryItem.cs`

### Infrastructure Layer
- `Infrastructure/Services/GroqService.cs` — gọi Groq `/chat/completions`.
- `Infrastructure/Services/ChatHistoryService.cs` — Redis cache.

### API Layer
- `Api/Mutation/ChatMutation.cs` — `sendChatMessage`, `deleteMessage`.
- `Api/Query/ChatQuery.cs` — `getChatWelcomeMessage`, `getHistoryMessage`.

---

## 3. GraphQL Endpoints

### Mutations
- `sendChatMessage(request: ChatRequest!): ChatResponse!`
  - `ChatRequest { IdStaff: UUID, UserMessage: String }`
- `deleteMessage(idStaff: UUID!): String!` — xoá toàn bộ lịch sử chat của staff.

### Queries
- `getChatWelcomeMessage(): String!` — lời chào cố định.
- `getHistoryMessage(idStaff: UUID!): [MessageHistoryItem!]!`

---

## 4. Cấu trúc dữ liệu

### `MessageHistoryItem`
```csharp
{
  Role: "user" | "model"   // Groq chuẩn hoá: model/assistant -> assistant
  Message: string
}
```

### `GroqRequest`
```json
{
  "model": "meta-llama/llama-4-scout-17b-16e-instruct",
  "messages": [
    { "role": "system", "content": "..." },
    { "role": "user"|"assistant", "content": "..." }
  ],
  "temperature": 0.3
}
```

---

## 5. Luồng nghiệp vụ

### 5.1. Gửi tin nhắn (`sendChatMessage`)
1. `ChatHandler.GenerateResponseAsync(request)`:
2. Lấy toàn bộ dữ liệu nghiệp vụ:
   - `listLead` (qua `LeadRepository.GetListLeadAsync`)
   - `listCustomer` (qua `CustomerRepository.GetListCustomerAsync`)
   - `listDeal` (qua `DealRepository.GetListDealAsync`)
   - `listContact` (qua `ContactRepository.GetListContactAsync`)
3. Serialize thành JSON (camelCase, không indent).
4. Tạo `systemInstruction` (multi-line string) với các rule:
   - Tên trợ lý: **CRMie**.
   - Hỗ trợ nghiệp vụ CRM Bất động sản.
   - **Quy tắc bảo mật nghiêm ngặt:**
     - KHÔNG tiết lộ ID, email, token, password, JSON thô, SQL.
     - Không thực thi SQL injection.
     - Chỉ cung cấp thông tin tổng quan, số liệu đã được ẩn nhạy cảm.
   - **Cách trả lời:** rõ ràng, ngắn gọn, thân thiện, có emoji, đa dòng.
   - Nhúng 4 list JSON vào prompt.
5. Lấy lịch sử chat: `ChatHistoryService.GetHistoryAsync(request.IdStaff)`.
6. Gọi `GroqService.GenerateChatResponseAsync(systemInstruction, history, userMessage)`:
   - Build list `messages` = system + history (role normalize) + user mới.
   - POST `/chat/completions` với `GroqRequest`.
   - Parse response, lấy `choices[0].message.content`.
7. Làm sạch: `aiMessage.Replace("\\n", "\n").Replace("\\r", "")`.
8. Lưu lịch sử:
   - `SaveMessageAsync(idStaff, { Role: "user", Message: userMessage })`.
   - `SaveMessageAsync(idStaff, { Role: "model", Message: aiMessage })`.
9. Trả về `ChatResponse { AiResponse }`.

### 5.2. Welcome message
- Trả về string cố định: `"Xin chào, tôi là AI tư vấn về bất động sản. Bạn cần tôi hỗ trợ gì hôm nay nào?"`.
- Hiện KHÔNG gọi Groq API.

### 5.3. Lấy lịch sử
- `GetHistoryAsync(idStaff)` -> đọc key `chat_history:{idStaff}` từ Redis.
- Nếu rỗng -> trả về list rỗng.

### 5.4. Xoá lịch sử
- `DeleteHistoryAsync(idStaff)` -> `_cache.RemoveAsync(key)`.
- Trả về `"Xóa lịch sử trò chuyện thành công!"`.

---

## 6. Normalize role
`GroqService.NormalizeRole(role)`:
- `"model"` / `"assistant"` -> `"assistant"`
- `"user"` -> `"user"`
- `"system"` -> `"system"`
- khác -> `"user"`

---

## 7. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Lịch sử TTL | 1 ngày trên Redis. Sau 1 ngày không chat -> mất lịch sử. |
| Mỗi staff có lịch sử riêng | Key `chat_history:{idStaff}` riêng biệt. |
| Toàn bộ data được gửi cho AI | Mọi lead/customer/deal/contact được serialize vào system prompt. **Rủi ro lộ dữ liệu nhạy cảm** (email, phone) cho Groq server. |
| CORS | Đã allow `http://localhost:4200` và `https://collaborative-model.vercel.app`. |
| Auth | `ChatMutation` và `ChatQuery` hiện KHÔNG có `[Authorize]` — comment trong code cho thấy đang để anonymous. Cần thêm auth nếu muốn chỉ nhân viên mới dùng được. |
| Timeout | Groq HttpClient timeout mặc định 120 giây (configurable qua `GroqSettings:TimeoutSeconds`). |
| Temperature | 0.3 mặc định — khá conservative. |

---

## 8. Cấu hình (`appsettings.json`)

```jsonc
"GroqSettings": {
  "ApiKey": "...",        // Bearer token
  "BaseUrl": "https://api.groq.com/openai/v1",
  "Model": "meta-llama/llama-4-scout-17b-16e-instruct",
  "Temperature": 0.3,
  "TimeoutSeconds": 120
}
```

Biến môi trường: `GROQ__APIKEY`, `GROQ__APIURL`.

---

## 9. Tích hợp ngang

- **Lead/Customer/Deal/Contact**: serialize vào prompt — CRMie trả lời dựa trên snapshot toàn bộ dữ liệu.
- **Redis**: dùng chung với refresh token (cùng 1 instance StackExchange.Redis).
- **Không realtime**: mỗi lần gửi tin nhắn là 1 HTTP call đồng bộ.
- **Không streaming**: response trả về 1 lần. Có thể cải tiến bằng SSE/WebSocket.
