namespace Sortec.Domain.Entities
{
    public class Response<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public bool Deleted { get; set; }

        public Response()
        {
            Status = true;
            Deleted = true;
            Message = string.Empty;
        }

        public Response(T? data, string message = "", bool status = true, bool deleted = true)
        {
            Data = data;
            Message = message;
            Status = status;
            Deleted = deleted;
        }
    }

    public class Response : Response<object>
    {
        public Response(string message = "", bool status = true, bool deleted = true)
            : base(null, message, status, deleted) { }
    }
}