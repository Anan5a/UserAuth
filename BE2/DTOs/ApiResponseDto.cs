
namespace BE.DTOs
{
    public class ApiResponseDto
    {
        public int Code { get; set; }

        public string Message { get; set; }
        public User? User { get; set; }
    }
}
