# PROMPT: Xây dựng UI React TypeScript cho Exam Service

## Tổng quan
Xây dựng một ứng dụng web React TypeScript hoàn chỉnh để quản lý hệ thống thi trắc nghiệm với các chức năng:
- Quản lý Ngân hàng câu hỏi (Question Banks)
- Quản lý Câu hỏi (Questions) - tạo thủ công hoặc import từ Excel
- Quản lý Bài thi (Exams)
- Làm bài thi (Take Exam)
- Xem kết quả thi (View Results)

## Lưu ý quan trọng về ID
**Tất cả ID trong hệ thống đều sử dụng GUID (Globally Unique Identifier) dưới dạng string trong TypeScript:**
- Tất cả `id` trong TypeScript interfaces đều là `string` (không phải `number`)
- GUID được biểu diễn dưới dạng string, ví dụ: `"550e8400-e29b-41d4-a716-446655440000"`
- Khi gọi API, truyền GUID dưới dạng string trong URL hoặc request body
- Khi hiển thị, có thể format GUID nếu cần (nhưng thường không cần thiết)

## Base URL
```
http://localhost:5000/api (hoặc URL của backend khi deploy)
```

---

## 1. TYPES & INTERFACES (TypeScript)

### 1.1 API Response Types

```typescript
// enums.ts
export enum ResponseCode {
  Success = 200,
  BadRequest = 400,
  Unauthorized = 401,
  NotFound = 404,
  InternalServerError = 500,
  ValidationFailed = 1001,
  BusinessRuleViolation = 1002
}

export enum ResponseMessage {
  RequestSuccessful = "RequestSuccessful",
  InvalidRequest = "InvalidRequest",
  UnauthorizedAccess = "UnauthorizedAccess",
  ResourceNotFound = "ResourceNotFound",
  UnexpectedError = "UnexpectedError",
  ValidationError = "ValidationError",
  BusinessError = "BusinessError"
}

// types.ts
export interface ApiResponse<T> {
  code: ResponseCode;
  status: "success" | "error";
  message?: string;
  data?: T;
  errors?: string | string[] | object;
}

// Question Bank Types
export interface QuestionBankSummary {
  id: string; // GUID as string
  name: string;
}

export interface QuestionBank {
  id: string; // GUID as string
  name: string;
  description: string;
}

// Question Types
export type QuestionType = "MultipleChoice" | "ShortAnswer";

export interface Question {
  id: string; // GUID as string
  questionText: string;
  questionType: QuestionType;
  questionBankId: string; // GUID as string
  audioUrl?: string;
  // Note: CorrectAnswer không được trả về trong response để bảo mật
}

// Exam Types
export interface ExamSummary {
  id: string; // GUID as string
  title: string;
  durationInMinutes: number;
}

export interface ExamDetail {
  id: string; // GUID as string
  title: string;
  durationInMinutes: number;
  questions: Question[];
}

// Submission Types
export interface SubmissionResult {
  id: string; // GUID as string
  examId: string; // GUID as string
  studentId: string; // GUID as string
  startedAt: string; // ISO 8601 datetime
  submittedAt?: string; // ISO 8601 datetime
  score?: number; // Điểm từ 0-10
}

// Request Types
export interface CreateQuestionBankRequest {
  name: string;
  description?: string;
}

export interface UpdateQuestionBankRequest {
  name: string;
  description?: string;
}

export interface CreateQuestionRequest {
  questionText: string;
  questionType: QuestionType;
  questionBankId: string; // GUID as string
  correctAnswer: string;
  audioUrl?: string;
}

export interface UpdateQuestionRequest {
  questionText: string;
  questionType: QuestionType;
  correctAnswer: string;
  audioUrl?: string;
}

export interface CreateExamRequest {
  title: string;
  durationInMinutes: number;
  questionIds: string[]; // Array of GUIDs as strings
}

export interface UpdateExamRequest {
  title: string;
  durationInMinutes: number;
  questionIds: string[]; // Array of GUIDs as strings
}

export interface StartSubmissionRequest {
  studentId: string; // GUID as string
  examId: string; // GUID as string
}

export interface SubmitAnswerRequest {
  questionId: string; // GUID as string
  selectedAnswer: string;
}

export interface SubmitExamRequest {
  answers: SubmitAnswerRequest[];
}

// Exam Set Types
export interface ExamSet {
  id: string; // GUID as string
  name: string;
  flashcardId: string; // GUID as string
}

export interface CreateExamSetRequest {
  name: string;
  flashcardId: string; // GUID as string
}
```

---

## 2. API ENDPOINTS & JSON RESPONSES

### 2.1 Question Banks API

#### GET /api/QuestionBanks
**Lấy danh sách tất cả ngân hàng câu hỏi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "name": "Ngân hàng câu hỏi Toán học"
    },
    {
      "id": "550e8400-e29b-41d4-a716-446655440002",
      "name": "Ngân hàng câu hỏi Tiếng Anh"
    }
  ]
}
```

#### GET /api/QuestionBanks/{id}
**Lấy chi tiết ngân hàng câu hỏi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "name": "Ngân hàng câu hỏi Toán học",
    "description": "Các câu hỏi về toán học cơ bản"
  }
}
```

**Response 404:**
```json
{
  "code": 404,
  "status": "error",
  "message": "Không tìm thấy tài nguyên",
  "errors": "Không tìm thấy Ngân hàng câu hỏi ID=999"
}
```

#### POST /api/QuestionBanks
**Tạo ngân hàng câu hỏi mới**

**Request Body:**
```json
{
  "name": "Ngân hàng câu hỏi Vật Lý",
  "description": "Các câu hỏi về vật lý cơ bản"
}
```

**Response 201:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440003",
    "name": "Ngân hàng câu hỏi Vật Lý",
    "description": "Các câu hỏi về vật lý cơ bản"
  }
}
```

**Response 400 (Validation Error):**
```json
{
  "code": 1001,
  "status": "error",
  "message": "Dữ liệu không hợp lệ",
  "errors": [
    "The Name field is required."
  ]
}
```

#### PUT /api/QuestionBanks/{id}
**Cập nhật ngân hàng câu hỏi**

**Request Body:**
```json
{
  "name": "Ngân hàng câu hỏi Vật Lý - Nâng cao",
  "description": "Các câu hỏi về vật lý nâng cao"
}
```

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": null
}
```

#### DELETE /api/QuestionBanks/{id}
**Xóa ngân hàng câu hỏi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": null
}
```

#### GET /api/QuestionBanks/{bankId}/questions
**Lấy danh sách câu hỏi trong ngân hàng**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "questionText": "2 + 2 = ?",
      "questionType": "MultipleChoice",
      "questionBankId": "550e8400-e29b-41d4-a716-446655440001",
      "audioUrl": null
    },
    {
      "id": "660e8400-e29b-41d4-a716-446655440002",
      "questionText": "Thủ đô của Việt Nam là gì?",
      "questionType": "ShortAnswer",
      "questionBankId": "550e8400-e29b-41d4-a716-446655440001",
      "audioUrl": "https://example.com/audio.mp3"
    }
  ]
}
```

#### POST /api/QuestionBanks/questions
**Tạo câu hỏi mới**

**Request Body:**
```json
{
  "questionText": "5 x 5 = ?",
  "questionType": "MultipleChoice",
  "questionBankId": 1,
  "correctAnswer": "25",
  "audioUrl": null
}
```

**Response 201:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440003",
    "questionText": "5 x 5 = ?",
    "questionType": "MultipleChoice",
    "questionBankId": "550e8400-e29b-41d4-a716-446655440001",
    "audioUrl": null
  }
}
```

**Response 400 (Business Rule Violation):**
```json
{
  "code": 1002,
  "status": "error",
  "message": "Vi phạm quy tắc nghiệp vụ",
  "errors": "Không tìm thấy Ngân hàng câu hỏi ID=999 để thêm câu hỏi."
}
```

#### GET /api/QuestionBanks/questions/{id}
**Lấy chi tiết câu hỏi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440001",
    "questionText": "2 + 2 = ?",
    "questionType": "MultipleChoice",
    "questionBankId": "550e8400-e29b-41d4-a716-446655440001",
    "audioUrl": null
  }
}
```

#### POST /api/QuestionBanks/{bankId}/import-questions
**Import câu hỏi từ file Excel**

**Request:** FormData với key `file`
- Content-Type: `multipart/form-data`
- File: Excel (.xlsx hoặc .xls)
- Format Excel:
  - Row 1: Header (QuestionText, QuestionType, CorrectAnswer, AudioUrl)
  - Row 2+: Data
  - Cột 1: QuestionText (bắt buộc)
  - Cột 2: QuestionType (bắt buộc) - "MultipleChoice" hoặc "ShortAnswer"
  - Cột 3: CorrectAnswer (bắt buộc)
  - Cột 4: AudioUrl (tùy chọn)

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": null
}
```

**Response 400 (Validation Error):**
```json
{
  "code": 1002,
  "status": "error",
  "message": "Vi phạm quy tắc nghiệp vụ",
  "errors": "Chỉ chấp nhận file Excel (.xlsx hoặc .xls)."
}
```

**Response 400 (Excel Validation Error):**
```json
{
  "code": 1002,
  "status": "error",
  "message": "Vi phạm quy tắc nghiệp vụ",
  "errors": "Có lỗi trong file Excel:\nDòng 3: Thiếu nội dung câu hỏi. Dòng 5: Loại câu hỏi không hợp lệ. Chỉ chấp nhận: MultipleChoice, ShortAnswer."
}
```

---

### 2.2 Exams API

#### GET /api/Exams
**Lấy danh sách tất cả bài thi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": [
    {
      "id": "770e8400-e29b-41d4-a716-446655440001",
      "title": "Bài thi Toán học cơ bản",
      "durationInMinutes": 60
    },
    {
      "id": "770e8400-e29b-41d4-a716-446655440002",
      "title": "Bài thi Tiếng Anh",
      "durationInMinutes": 90
    }
  ]
}
```

#### GET /api/Exams/{id}
**Lấy chi tiết bài thi (bao gồm danh sách câu hỏi)**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "770e8400-e29b-41d4-a716-446655440001",
    "title": "Bài thi Toán học cơ bản",
    "durationInMinutes": 60,
    "questions": [
      {
        "id": "660e8400-e29b-41d4-a716-446655440001",
        "questionText": "2 + 2 = ?",
        "questionType": "MultipleChoice",
        "questionBankId": "550e8400-e29b-41d4-a716-446655440001",
        "audioUrl": null
      },
      {
        "id": "660e8400-e29b-41d4-a716-446655440002",
        "questionText": "5 x 5 = ?",
        "questionType": "MultipleChoice",
        "questionBankId": "550e8400-e29b-41d4-a716-446655440001",
        "audioUrl": null
      }
    ]
  }
}
```

#### POST /api/Exams
**Tạo bài thi mới**

**Request Body:**
```json
{
  "title": "Bài thi Toán học cơ bản",
  "durationInMinutes": 60,
  "questionIds": ["660e8400-e29b-41d4-a716-446655440001", "660e8400-e29b-41d4-a716-446655440002", "660e8400-e29b-41d4-a716-446655440003", "660e8400-e29b-41d4-a716-446655440004", "660e8400-e29b-41d4-a716-446655440005"]
}
```

**Response 201:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "770e8400-e29b-41d4-a716-446655440001",
    "title": "Bài thi Toán học cơ bản",
    "durationInMinutes": 60,
    "questions": [...]
  }
}
```

**Response 400 (Business Rule Violation):**
```json
{
  "code": 1002,
  "status": "error",
  "message": "Vi phạm quy tắc nghiệp vụ",
  "errors": "Bài thi phải có ít nhất 1 câu hỏi."
}
```

#### PUT /api/Exams/{id}
**Cập nhật bài thi**

**Request Body:**
```json
{
  "title": "Bài thi Toán học cơ bản - Cập nhật",
  "durationInMinutes": 90,
  "questionIds": [1, 2, 3, 4, 5, 6]
}
```

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": null
}
```

#### DELETE /api/Exams/{id}
**Xóa bài thi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": null
}
```

---

### 2.3 Submissions API

#### POST /api/Submissions/start
**Bắt đầu làm bài thi**

**Request Body:**
```json
{
  "studentId": "550e8400-e29b-41d4-a716-446655440000",
  "examId": "770e8400-e29b-41d4-a716-446655440001"
}
```

**Response 201:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "880e8400-e29b-41d4-a716-446655440001",
    "examId": "770e8400-e29b-41d4-a716-446655440001",
    "studentId": "550e8400-e29b-41d4-a716-446655440000",
    "startedAt": "2024-01-15T10:00:00Z",
    "submittedAt": null,
    "score": null
  }
}
```

#### POST /api/Submissions/{submissionId}/submit
**Nộp bài thi**

**Request Body:**
```json
{
  "answers": [
    {
      "questionId": "660e8400-e29b-41d4-a716-446655440001",
      "selectedAnswer": "4"
    },
    {
      "questionId": "660e8400-e29b-41d4-a716-446655440002",
      "selectedAnswer": "25"
    },
    {
      "questionId": "660e8400-e29b-41d4-a716-446655440003",
      "selectedAnswer": "Hà Nội"
    }
  ]
}
```

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "880e8400-e29b-41d4-a716-446655440001",
    "examId": "770e8400-e29b-41d4-a716-446655440001",
    "studentId": "550e8400-e29b-41d4-a716-446655440000",
    "startedAt": "2024-01-15T10:00:00Z",
    "submittedAt": "2024-01-15T10:45:00Z",
    "score": 8.5
  }
}
```

**Response 400 (Business Rule Violation):**
```json
{
  "code": 1002,
  "status": "error",
  "message": "Vi phạm quy tắc nghiệp vụ",
  "errors": "Bài thi này đã được nộp trước đó."
}
```

**Response 400 (Time Expired):**
```json
{
  "code": 1002,
  "status": "error",
  "message": "Vi phạm quy tắc nghiệp vụ",
  "errors": "Đã hết thời gian làm bài."
}
```

#### GET /api/Submissions/{id}
**Xem kết quả bài thi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "880e8400-e29b-41d4-a716-446655440001",
    "examId": "770e8400-e29b-41d4-a716-446655440001",
    "studentId": "550e8400-e29b-41d4-a716-446655440000",
    "startedAt": "2024-01-15T10:00:00Z",
    "submittedAt": "2024-01-15T10:45:00Z",
    "score": 8.5
  }
}
```

---

### 2.4 Exam Sets API

#### GET /api/ExamSets
**Lấy danh sách bộ đề thi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": [
    {
      "id": "990e8400-e29b-41d4-a716-446655440001",
      "name": "Bộ đề thi Toán học",
      "flashcardId": "660e8400-e29b-41d4-a716-446655440000"
    }
  ]
}
```

#### GET /api/ExamSets/{id}
**Lấy chi tiết bộ đề thi**

**Response 200:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "990e8400-e29b-41d4-a716-446655440001",
    "name": "Bộ đề thi Toán học",
    "flashcardId": "660e8400-e29b-41d4-a716-446655440000"
  }
}
```

#### POST /api/ExamSets
**Tạo bộ đề thi mới**

**Request Body:**
```json
{
  "name": "Bộ đề thi Toán học",
  "flashcardId": "660e8400-e29b-41d4-a716-446655440000"
}
```

**Response 201:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Yêu cầu được thực hiện thành công",
  "data": {
    "id": "990e8400-e29b-41d4-a716-446655440001",
    "name": "Bộ đề thi Toán học",
    "flashcardId": "660e8400-e29b-41d4-a716-446655440000"
  }
}
```

---

## 3. CÁC TRANG MÀN HÌNH CẦN XÂY DỰNG

### 3.1 Question Bank Management Pages

#### 3.1.1 Question Bank List Page
**Route:** `/question-banks`
**Chức năng:**
- Hiển thị danh sách tất cả ngân hàng câu hỏi
- Có nút "Tạo mới" để tạo ngân hàng câu hỏi
- Mỗi item có:
  - Tên ngân hàng
  - Nút "Xem chi tiết"
  - Nút "Sửa"
  - Nút "Xóa" (có confirm dialog)
  - Nút "Xem câu hỏi"

**UI Components:**
- Table hoặc Card list
- Search bar (tìm kiếm theo tên)
- Pagination (nếu cần)

#### 3.1.2 Create/Edit Question Bank Page
**Route:** `/question-banks/new` hoặc `/question-banks/:id/edit`
**Form Fields:**
- Name (required, max 100 chars)
- Description (optional, max 500 chars)

**Validation:**
- Name bắt buộc
- Hiển thị lỗi validation từ API

#### 3.1.3 Question Bank Detail Page
**Route:** `/question-banks/:id`
**Chức năng:**
- Hiển thị thông tin ngân hàng câu hỏi
- Tab/Danh sách câu hỏi trong ngân hàng
- Nút "Thêm câu hỏi" (modal hoặc navigate)
- Nút "Import từ Excel" (modal upload file)

#### 3.1.4 Question List in Bank Page
**Route:** `/question-banks/:id/questions`
**Chức năng:**
- Hiển thị danh sách câu hỏi trong ngân hàng
- Filter theo QuestionType (MultipleChoice/ShortAnswer)
- Mỗi item có:
  - QuestionText
  - QuestionType badge
  - Nút "Sửa"
  - Nút "Xóa"
- Nút "Thêm câu hỏi mới"
- Nút "Import từ Excel"

#### 3.1.5 Create/Edit Question Page
**Route:** `/question-banks/:bankId/questions/new` hoặc `/questions/:id/edit`
**Form Fields:**
- QuestionText (required, textarea)
- QuestionType (required, select: MultipleChoice | ShortAnswer)
- CorrectAnswer (required, text input)
- AudioUrl (optional, URL input)

**Validation:**
- Tất cả trường bắt buộc phải có giá trị
- QuestionType phải là một trong: MultipleChoice, ShortAnswer

#### 3.1.6 Import Questions Modal/Page
**Route:** Modal hoặc `/question-banks/:id/import`
**Chức năng:**
- File upload component (chỉ chấp nhận .xlsx, .xls)
- Hiển thị progress khi upload
- Hiển thị lỗi chi tiết nếu có (từng dòng lỗi)
- Download template Excel (optional)

**Excel Template Format:**
```
| QuestionText | QuestionType | CorrectAnswer | AudioUrl |
|--------------|-------------|---------------|----------|
| 2 + 2 = ?    | MultipleChoice | 4 | (optional) |
| Thủ đô VN?   | ShortAnswer | Hà Nội | (optional) |
```

---

### 3.2 Exam Management Pages

#### 3.2.1 Exam List Page
**Route:** `/exams`
**Chức năng:**
- Hiển thị danh sách tất cả bài thi
- Mỗi item hiển thị:
  - Title
  - Duration (phút)
  - Số lượng câu hỏi
  - Nút "Xem chi tiết"
  - Nút "Sửa"
  - Nút "Xóa"
- Nút "Tạo bài thi mới"
- Search bar

#### 3.2.2 Create/Edit Exam Page
**Route:** `/exams/new` hoặc `/exams/:id/edit`
**Form Fields:**
- Title (required, max 200 chars)
- DurationInMinutes (required, number input, min 1)
- Question Selection:
  - Hiển thị danh sách tất cả Question Banks
  - Cho phép chọn Question Bank để xem câu hỏi
  - Checkbox để chọn câu hỏi (multiple selection)
  - Hiển thị số lượng câu hỏi đã chọn
  - Validate: phải chọn ít nhất 1 câu hỏi

**UI Flow:**
1. User nhập Title và Duration
2. Chọn Question Bank từ dropdown
3. Hiển thị danh sách câu hỏi trong Question Bank đó
4. User check/uncheck các câu hỏi muốn thêm vào bài thi
5. Có thể chọn nhiều Question Banks và nhiều câu hỏi
6. Hiển thị tổng số câu hỏi đã chọn
7. Submit form

#### 3.2.3 Exam Detail Page
**Route:** `/exams/:id`
**Chức năng:**
- Hiển thị thông tin bài thi:
  - Title
  - Duration
  - Danh sách câu hỏi (không hiển thị đáp án đúng)
- Nút "Sửa"
- Nút "Xóa"
- Nút "Bắt đầu làm bài" (nếu là student)

---

### 3.3 Take Exam Pages

#### 3.3.1 Exam Start Page
**Route:** `/exams/:id/start`
**Chức năng:**
- Hiển thị thông tin bài thi:
  - Title
  - Duration
  - Số lượng câu hỏi
  - Hướng dẫn làm bài
- Input: StudentId (hoặc lấy từ context/auth)
- Nút "Bắt đầu làm bài"
- Khi click "Bắt đầu", gọi API `/api/Submissions/start`
- Navigate đến exam page với submissionId

#### 3.3.2 Exam Taking Page
**Route:** `/exams/:examId/take/:submissionId`
**Chức năng:**
- Timer countdown (hiển thị thời gian còn lại)
- Hiển thị từng câu hỏi (có thể pagination hoặc scroll)
- Mỗi câu hỏi có:
  - QuestionText
  - Audio player (nếu có AudioUrl)
  - Input field cho câu trả lời:
    - MultipleChoice: Radio buttons hoặc dropdown
    - ShortAnswer: Text input
- Navigation giữa các câu hỏi (Previous/Next buttons)
- Question list sidebar (hiển thị số câu, câu nào đã trả lời)
- Auto-save answers (optional, localStorage)
- Nút "Nộp bài" (có confirm dialog)
- Warning khi gần hết giờ
- Auto submit khi hết giờ

**State Management:**
- Lưu answers trong local state
- Khi submit, gọi API `/api/Submissions/{submissionId}/submit`
- Navigate đến result page

**Timer Logic:**
```typescript
// Calculate remaining time
const exam = examDetail;
const submission = submissionResult;
const startedAt = new Date(submission.startedAt);
const endTime = new Date(startedAt.getTime() + exam.durationInMinutes * 60000);
const remainingTime = endTime - new Date();

// Update every second
// Auto submit when time <= 0
```

#### 3.3.3 Exam Result Page
**Route:** `/submissions/:id/result`
**Chức năng:**
- Hiển thị kết quả:
  - Điểm số (Score) - hiển thị rõ ràng, có màu sắc
  - Thời gian bắt đầu
  - Thời gian nộp bài
  - Tổng số câu hỏi
  - Số câu đúng (tính từ score)
- Danh sách câu hỏi và đáp án:
  - QuestionText
  - Your Answer (đáp án của học sinh)
  - Correct Answer (đáp án đúng) - chỉ hiển thị sau khi nộp
  - Icon đúng/sai cho mỗi câu
- Nút "Xem lại bài thi"
- Nút "Quay về danh sách"

**UI Design:**
- Score display: Large, prominent, color-coded (green if >= 5, red if < 5)
- Question list: Card layout với màu sắc phân biệt đúng/sai

---

## 4. API SERVICE LAYER (React)

### 4.1 API Client Setup

```typescript
// api/client.ts
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5000/api';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Response interceptor để xử lý error
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Handle error globally
    return Promise.reject(error);
  }
);
```

### 4.2 API Service Functions

```typescript
// api/questionBankService.ts
import { apiClient } from './client';
import { ApiResponse, QuestionBank, QuestionBankSummary, Question, CreateQuestionBankRequest, UpdateQuestionBankRequest, CreateQuestionRequest, UpdateQuestionRequest } from '../types';

export const questionBankService = {
  // Question Banks
  getAll: async (): Promise<QuestionBankSummary[]> => {
    const response = await apiClient.get<ApiResponse<QuestionBankSummary[]>>('/QuestionBanks');
    return response.data.data || [];
  },

  getById: async (id: string): Promise<QuestionBank> => {
    const response = await apiClient.get<ApiResponse<QuestionBank>>(`/QuestionBanks/${id}`);
    return response.data.data!;
  },

  create: async (data: CreateQuestionBankRequest): Promise<QuestionBank> => {
    const response = await apiClient.post<ApiResponse<QuestionBank>>('/QuestionBanks', data);
    return response.data.data!;
  },

  update: async (id: string, data: UpdateQuestionBankRequest): Promise<void> => {
    await apiClient.put(`/QuestionBanks/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/QuestionBanks/${id}`);
  },

  // Questions
  getQuestionsByBankId: async (bankId: string): Promise<Question[]> => {
    const response = await apiClient.get<ApiResponse<Question[]>>(`/QuestionBanks/${bankId}/questions`);
    return response.data.data || [];
  },

  createQuestion: async (data: CreateQuestionRequest): Promise<Question> => {
    const response = await apiClient.post<ApiResponse<Question>>('/QuestionBanks/questions', data);
    return response.data.data!;
  },

  getQuestionById: async (id: string): Promise<Question> => {
    const response = await apiClient.get<ApiResponse<Question>>(`/QuestionBanks/questions/${id}`);
    return response.data.data!;
  },

  importQuestions: async (bankId: string, file: File): Promise<void> => {
    const formData = new FormData();
    formData.append('file', file);
    await apiClient.post(`/QuestionBanks/${bankId}/import-questions`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },
};

// api/examService.ts
import { apiClient } from './client';
import { ApiResponse, ExamSummary, ExamDetail, CreateExamRequest, UpdateExamRequest } from '../types';

export const examService = {
  getAll: async (): Promise<ExamSummary[]> => {
    const response = await apiClient.get<ApiResponse<ExamSummary[]>>('/Exams');
    return response.data.data || [];
  },

  getById: async (id: string): Promise<ExamDetail> => {
    const response = await apiClient.get<ApiResponse<ExamDetail>>(`/Exams/${id}`);
    return response.data.data!;
  },

  create: async (data: CreateExamRequest): Promise<ExamDetail> => {
    const response = await apiClient.post<ApiResponse<ExamDetail>>('/Exams', data);
    return response.data.data!;
  },

  update: async (id: string, data: UpdateExamRequest): Promise<void> => {
    await apiClient.put(`/Exams/${id}`, data);
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/Exams/${id}`);
  },
};

// api/submissionService.ts
import { apiClient } from './client';
import { ApiResponse, SubmissionResult, StartSubmissionRequest, SubmitExamRequest } from '../types';

export const submissionService = {
  start: async (data: StartSubmissionRequest): Promise<SubmissionResult> => {
    const response = await apiClient.post<ApiResponse<SubmissionResult>>('/Submissions/start', data);
    return response.data.data!;
  },

  submit: async (submissionId: string, data: SubmitExamRequest): Promise<SubmissionResult> => {
    const response = await apiClient.post<ApiResponse<SubmissionResult>>(`/Submissions/${submissionId}/submit`, data);
    return response.data.data!;
  },

  getResult: async (id: string): Promise<SubmissionResult> => {
    const response = await apiClient.get<ApiResponse<SubmissionResult>>(`/Submissions/${id}`);
    return response.data.data!;
  },
};
```

---

## 5. ERROR HANDLING

### 5.1 Error Response Format

Tất cả error responses đều có format:
```json
{
  "code": 400 | 404 | 1001 | 1002 | 500,
  "status": "error",
  "message": "Error message",
  "errors": "string | string[] | object"
}
```

### 5.2 Error Handling Strategy

```typescript
// utils/errorHandler.ts
import { AxiosError } from 'axios';
import { ApiResponse, ResponseCode } from '../types';

export const handleApiError = (error: AxiosError<ApiResponse<any>>): string => {
  if (error.response) {
    const apiResponse = error.response.data;
    
    // Handle different error types
    if (apiResponse.errors) {
      if (Array.isArray(apiResponse.errors)) {
        return apiResponse.errors.join(', ');
      }
      if (typeof apiResponse.errors === 'string') {
        return apiResponse.errors;
      }
    }
    
    return apiResponse.message || 'Đã xảy ra lỗi';
  }
  
  if (error.request) {
    return 'Không thể kết nối đến server';
  }
  
  return 'Đã xảy ra lỗi không xác định';
};

// Usage in components
try {
  await questionBankService.create(data);
} catch (error) {
  const errorMessage = handleApiError(error as AxiosError);
  // Show error to user (toast, alert, etc.)
}
```

---

## 6. VALIDATION RULES

### 6.1 Client-side Validation

```typescript
// utils/validation.ts

export const validateQuestionBank = (data: CreateQuestionBankRequest): string[] => {
  const errors: string[] = [];
  
  if (!data.name || data.name.trim().length === 0) {
    errors.push('Tên ngân hàng câu hỏi là bắt buộc');
  }
  
  if (data.name && data.name.length > 100) {
    errors.push('Tên không được vượt quá 100 ký tự');
  }
  
  if (data.description && data.description.length > 500) {
    errors.push('Mô tả không được vượt quá 500 ký tự');
  }
  
  return errors;
};

export const validateQuestion = (data: CreateQuestionRequest): string[] => {
  const errors: string[] = [];
  
  if (!data.questionText || data.questionText.trim().length === 0) {
    errors.push('Nội dung câu hỏi là bắt buộc');
  }
  
  if (!data.questionType) {
    errors.push('Loại câu hỏi là bắt buộc');
  } else if (!['MultipleChoice', 'ShortAnswer'].includes(data.questionType)) {
    errors.push('Loại câu hỏi không hợp lệ');
  }
  
  if (!data.correctAnswer || data.correctAnswer.trim().length === 0) {
    errors.push('Đáp án đúng là bắt buộc');
  }
  
  if (!data.questionBankId) {
    errors.push('Ngân hàng câu hỏi là bắt buộc');
  }
  
  return errors;
};

export const validateExam = (data: CreateExamRequest): string[] => {
  const errors: string[] = [];
  
  if (!data.title || data.title.trim().length === 0) {
    errors.push('Tiêu đề bài thi là bắt buộc');
  }
  
  if (data.title && data.title.length > 200) {
    errors.push('Tiêu đề không được vượt quá 200 ký tự');
  }
  
  if (!data.durationInMinutes || data.durationInMinutes < 1) {
    errors.push('Thời gian thi phải lớn hơn 0');
  }
  
  if (!data.questionIds || data.questionIds.length === 0) {
    errors.push('Bài thi phải có ít nhất 1 câu hỏi');
  }
  
  return errors;
};
```

---

## 7. UI/UX GUIDELINES

### 7.1 Color Scheme
- Primary: Blue (#007bff)
- Success: Green (#28a745)
- Danger: Red (#dc3545)
- Warning: Orange (#ffc107)
- Info: Cyan (#17a2b8)

### 7.2 Components Library Suggestions
- **Material-UI (MUI)** hoặc **Ant Design** hoặc **Chakra UI**
- **React Router** cho routing
- **React Hook Form** cho form handling
- **Axios** cho API calls
- **React Query** hoặc **SWR** cho data fetching và caching
- **Zustand** hoặc **Redux Toolkit** cho state management (nếu cần)

### 7.3 Loading States
- Hiển thị loading spinner khi đang fetch data
- Disable buttons khi đang submit
- Skeleton loaders cho danh sách

### 7.4 Success/Error Notifications
- Toast notifications cho success/error messages
- Sử dụng **react-toastify** hoặc **notistack**

### 7.5 Responsive Design
- Mobile-first approach
- Breakpoints: sm (640px), md (768px), lg (1024px), xl (1280px)

---

## 8. ROUTING STRUCTURE

```typescript
// App.tsx routes
<Routes>
  {/* Question Banks */}
  <Route path="/question-banks" element={<QuestionBankList />} />
  <Route path="/question-banks/new" element={<CreateQuestionBank />} />
  <Route path="/question-banks/:id" element={<QuestionBankDetail />} />
  <Route path="/question-banks/:id/edit" element={<EditQuestionBank />} />
  <Route path="/question-banks/:id/questions" element={<QuestionList />} />
  <Route path="/question-banks/:bankId/questions/new" element={<CreateQuestion />} />
  <Route path="/questions/:id/edit" element={<EditQuestion />} />
  
  {/* Exams */}
  <Route path="/exams" element={<ExamList />} />
  <Route path="/exams/new" element={<CreateExam />} />
  <Route path="/exams/:id" element={<ExamDetail />} />
  <Route path="/exams/:id/edit" element={<EditExam />} />
  
  {/* Take Exam */}
  <Route path="/exams/:id/start" element={<ExamStart />} />
  <Route path="/exams/:examId/take/:submissionId" element={<TakeExam />} />
  <Route path="/submissions/:id/result" element={<ExamResult />} />
  
  {/* Exam Sets */}
  <Route path="/exam-sets" element={<ExamSetList />} />
  <Route path="/exam-sets/new" element={<CreateExamSet />} />
</Routes>
```

---

## 9. STATE MANAGEMENT (Optional)

Nếu cần global state management:

```typescript
// stores/examStore.ts (Zustand example)
import create from 'zustand';

interface ExamStore {
  currentSubmission: SubmissionResult | null;
  answers: Record<number, string>; // questionId -> answer
  setSubmission: (submission: SubmissionResult) => void;
  setAnswer: (questionId: number, answer: string) => void;
  clearSubmission: () => void;
}

export const useExamStore = create<ExamStore>((set) => ({
  currentSubmission: null,
  answers: {},
  setSubmission: (submission) => set({ currentSubmission: submission }),
  setAnswer: (questionId, answer) => set((state) => ({
    answers: { ...state.answers, [questionId]: answer },
  })),
  clearSubmission: () => set({ currentSubmission: null, answers: {} }),
}));
```

---

## 10. TESTING CHECKLIST

### Functionality Tests
- [ ] Tạo/sửa/xóa Question Bank
- [ ] Tạo/sửa/xóa Question
- [ ] Import questions từ Excel (success case)
- [ ] Import questions từ Excel (error cases: invalid file, validation errors)
- [ ] Tạo/sửa/xóa Exam
- [ ] Bắt đầu làm bài thi
- [ ] Làm bài thi với timer
- [ ] Nộp bài thi
- [ ] Xem kết quả bài thi
- [ ] Auto submit khi hết giờ

### UI/UX Tests
- [ ] Loading states hiển thị đúng
- [ ] Error messages hiển thị rõ ràng
- [ ] Success notifications hiển thị
- [ ] Form validation hoạt động
- [ ] Confirm dialogs cho delete actions
- [ ] Responsive trên mobile

### Edge Cases
- [ ] Empty states (no data)
- [ ] Network errors
- [ ] Timeout errors
- [ ] Large file uploads
- [ ] Long question texts

---

## 11. ENVIRONMENT VARIABLES

```env
REACT_APP_API_BASE_URL=http://localhost:5000/api
```

---

## 12. PACKAGE.JSON DEPENDENCIES

```json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.20.0",
    "axios": "^1.6.0",
    "@tanstack/react-query": "^5.0.0",
    "react-hook-form": "^7.48.0",
    "react-toastify": "^9.1.0",
    "@mui/material": "^5.14.0",
    "@mui/icons-material": "^5.14.0",
    "date-fns": "^2.30.0"
  },
  "devDependencies": {
    "@types/react": "^18.2.0",
    "@types/react-dom": "^18.2.0",
    "typescript": "^5.2.0"
  }
}
```

---

## 13. SAMPLE COMPONENT STRUCTURE

```
src/
├── components/
│   ├── common/
│   │   ├── Button.tsx
│   │   ├── Input.tsx
│   │   ├── Modal.tsx
│   │   ├── LoadingSpinner.tsx
│   │   └── ErrorMessage.tsx
│   ├── questionBank/
│   │   ├── QuestionBankList.tsx
│   │   ├── QuestionBankForm.tsx
│   │   └── ImportQuestionsModal.tsx
│   ├── exam/
│   │   ├── ExamList.tsx
│   │   ├── ExamForm.tsx
│   │   └── QuestionSelector.tsx
│   └── submission/
│       ├── ExamTimer.tsx
│       ├── QuestionCard.tsx
│       └── ResultDisplay.tsx
├── pages/
│   ├── QuestionBankPages/
│   ├── ExamPages/
│   └── SubmissionPages/
├── api/
│   ├── client.ts
│   ├── questionBankService.ts
│   ├── examService.ts
│   └── submissionService.ts
├── types/
│   ├── index.ts
│   └── enums.ts
├── utils/
│   ├── validation.ts
│   ├── errorHandler.ts
│   └── dateUtils.ts
├── hooks/
│   ├── useQuestionBanks.ts
│   ├── useExams.ts
│   └── useSubmission.ts
└── App.tsx
```

---

## KẾT LUẬN

Prompt này bao gồm đầy đủ thông tin để xây dựng UI React TypeScript hoàn chỉnh cho Exam Service:
- Tất cả API endpoints với request/response examples
- TypeScript types và interfaces
- Các trang màn hình cần thiết
- Validation rules
- Error handling
- UI/UX guidelines
- Routing structure
- Component structure

Sử dụng prompt này để tạo UI hoàn chỉnh, hiện đại và user-friendly cho hệ thống thi trắc nghiệm.

