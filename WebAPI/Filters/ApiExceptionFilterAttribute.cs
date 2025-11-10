using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Exceptions;
using WebAPI.CustomResponse;

namespace WebAPI.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            ResponseCode code = ResponseCode.InternalServerError;
            ResponseMessage msg = ResponseMessage.UnexpectedError;
            object? errors = exception.Message;
            int httpStatusCode = 500;

            switch (exception)
            {
                // BẮT LỖI NGHIỆP VỤ (1002)
                case BusinessRuleException:
                    code = ResponseCode.BusinessRuleViolation; // Mã 1002
                    msg = ResponseMessage.BusinessError;
                    errors = exception.Message;
                    httpStatusCode = (int)ResponseCode.BadRequest; // Lỗi 400
                    break;

                // BẮT LỖI KHÔNG TÌM THẤY (404)
                case NotFoundException:
                    code = ResponseCode.NotFound; // Mã 404
                    msg = ResponseMessage.ResourceNotFound;
                    errors = exception.Message;
                    httpStatusCode = (int)ResponseCode.NotFound; // Lỗi 404
                    break;

                // Lỗi 500 (Mặc định)
                default:
                    code = ResponseCode.InternalServerError; // Mã 500
                    msg = ResponseMessage.UnexpectedError;
                    errors = exception.Message;
                    httpStatusCode = (int)ResponseCode.InternalServerError; // Lỗi 500
                    break;
            }

            var apiResponse = ApiResponse<object>.Fail(code, msg, errors);

            context.Result = new ObjectResult(apiResponse)
            {
                StatusCode = httpStatusCode
            };

            context.ExceptionHandled = true;
        }
    }

    /// <summary>
    /// BỘ LỌC XỬ LÝ LỖI VALIDATION (1001)
    /// Bắt lỗi khi Request DTO đầu vào không hợp lệ (ví dụ: [Required] bị thiếu)
    /// </summary>
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var code = ResponseCode.ValidationFailed; // Mã 1001
                var msg = ResponseMessage.ValidationError;

                var apiResponse = ApiResponse<object>.Fail(code, msg, errors);

                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = (int)ResponseCode.BadRequest // Lỗi 400
                };
            }
        }
    }
}
