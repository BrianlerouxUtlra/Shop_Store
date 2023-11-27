namespace Shop_store.Models
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public Exception Exception { get; set; }
    }
}
