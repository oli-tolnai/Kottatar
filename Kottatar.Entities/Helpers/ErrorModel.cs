using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kottatar.Entities.Helpers
{
    public class ErrorModel
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
        public string Path { get; set; }

        public ErrorModel(string message, int statusCode = 500, string path = null)
        {
            Message = message;
            StatusCode = statusCode;
            Timestamp = DateTime.UtcNow;
            Path = path;
        }

        public ErrorModel()
        {
            Message = "An error occurred.";
            StatusCode = 500;
            Timestamp = DateTime.UtcNow;
        }
    }
}
