using System;

namespace AstraDMLDemo.Models
{
    /// <summary>
    /// Model view for handling and display error messages
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
