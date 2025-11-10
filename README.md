# exam-service

## Tổng quan
Hệ thống quản lý thi trắc nghiệm với các chức năng:
- Quản lý Ngân hàng câu hỏi (Question Banks)
- Quản lý Câu hỏi (Questions) - tạo thủ công hoặc import từ Excel
- Quản lý Bài thi (Exams)
- Làm bài thi và nộp bài (Submissions)
- Quản lý Bộ đề thi (Exam Sets)

## Lưu ý quan trọng
**Tất cả ID trong hệ thống đều sử dụng GUID (Globally Unique Identifier) thay vì int để:**
- Đảm bảo tính duy nhất toàn cục
- Tránh xung đột khi merge dữ liệu từ nhiều nguồn
- Bảo mật tốt hơn (không thể đoán ID)
- Dễ dàng tích hợp với các hệ thống phân tán

**Các Entity sử dụng Guid:**
- QuestionBank.Id
- Question.Id, Question.QuestionBankId
- Exam.Id
- ExamQuestion.ExamId, ExamQuestion.QuestionId
- Submission.Id, Submission.ExamId, Submission.StudentId
- SubmissionAnswer.Id, SubmissionAnswer.SubmissionId, SubmissionAnswer.QuestionId
- ExamSet.Id, ExamSet.FlashcardId

## 1. Các quy tắc đặt tên phổ biến
-  **Pascal Case:** Chữ cái đầu tiên trong từ định danh và chữ cái đầu tiên của mỗi từ nối theo sau phải được viết hoa. Sử dụng Pascal Case để đặt tên cho một tên có từ 3 ký tự trở lên.
    - Ví dụ: `CodingConvention`

- **Camel Case:** Chữ cái đầu tiên trong từ định danh là chữ thường và chữ cái đầu tiên của mối từ nối theo sau phải được viết hoa.
    - Ví dụ: `codingConvention`

- **Upper Case:** Tất cả các ký tự trong từ định danh phải được viết hoa. Sử dụng quy tắc này đối với tên định danh có từ 2 ký tự trở xuống
    - Ví dụ: `CODINGCONVENTION`

## 2. Tóm tắt cách sử dụng các quy tắc 

- **Tên biến:** Camel Case (Danh từ)
    - Ví dụ: `firstName`

- **Hằng số:** Upper Case (Có gạch chân giữa các từ)
    - Ví dụ: `DEVELOP_HOST`

- **Tên class, Enum**: Pascal Case (Danh từ)
    - Ví dụ: `CreateUser`

- **Tham số**: Camel Case (Danh từ)
    - Ví dụ: `displayTime`

- **Thuộc tính**: Pascal Case (Danh từ)
    - Ví dụ: `BackgroundColor`

- **Phương thức**: Pascal Case (Động từ)
    - Ví dụ: `GetAge()`

- **Sự kiện**: Pascal Case (Có hậu tố EventHandler)
    - Ví dụ: `SelectedIndexChanged`

- **Interface**: Pascal Case (Có tiền tố I)
    - Ví dụ: `IUserService`

### [Tài liệu tham khảo đầy đủ](https://viblo.asia/p/coding-conventions-trong-c-eW65Gg1j5DO)
