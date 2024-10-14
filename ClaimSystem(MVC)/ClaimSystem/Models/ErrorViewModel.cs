namespace ClaimSystem.Models
{
    public class ErrorViewModel
    {
        // Stores request ID to manage issues
        public string? RequestId { get; set; }

        // True if valid Request ID is present
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
