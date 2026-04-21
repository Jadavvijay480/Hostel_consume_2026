using Hostel_Consume_2026.Models;

namespace Hostel_Consume_2026.Models
{

    public class ApiResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public UserModel data { get; set; }
    }

}
