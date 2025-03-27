namespace Sortec.Infrastructure.ExeptionHandling
{
    public class CustomHttpException : Exception
    {
        public int StatusCode { get; set; }
        public new string Message { get; set; }

        public CustomHttpException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}