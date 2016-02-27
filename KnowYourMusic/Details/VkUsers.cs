using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowYourMusic.Details
{
    public class VkUsers
    {
        public UserResponse[] response { get; set; }
    }

    public class UserResponse
    {
        public string uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}
