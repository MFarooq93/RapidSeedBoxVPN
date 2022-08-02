using System.Collections.Generic;

namespace PureVPN.Entity.Models.DTO
{
    public class LoginReply : BaseNetwork
    {
        public LoginReplyBody body { get; set; }
    }
    public class LoginReplyBody
    {
        public string uuid { get; set; }
        public string client_id { get; set; }
        public string token { get; set; }
        public List<VpnUsername> vpn_usernames { get; set; }
        public string secret { get; set; }
        public bool is_migrated { get; set; }
        public string method { get; set; }
        public string email { get; set; }
        /// <summary>
        /// Describes whether user should auto login to Members are
        /// </summary>
        public bool? shouldLoginWithMembersArea { get; set; }
    }


    public class VpnUsername
    {
        public string username { get; set; }
        public string expiry_date { get; set; }

        //private string _status;
        public string status { get; set; }
        public string billing_cycle { get; set; }
        public string payment_gateway { get; set; }
        public string hosting_id { get; set; }
        public string expiry_reason { get; set; }

        // These are switch user related properties
        public bool is_selected { get; set; }
        public string image
        {
            get
            {
                if (is_selected)
                    return $"pack://application:,,,/assets/checked.png";

                if (status.ToLower() == "disabled")
                    return $"pack://application:,,,/assets/disabled.png";

                if (status.ToLower() == "expired")
                    return $"pack://application:,,,/assets/expired.png";

                return $"pack://application:,,,/assets/empty.png";
            }
        }
        public string warning_message
        {
            get
            {
                if (status.ToLower() == "active")
                    return "";
                else
                    return "Account is " + status;
            }
        }

        public string UsernameAutomationID => $"Username_SelectAccount_{username}";
        public string UserStatusAutomationID => $"User_status_SelectAccount_{username}";
    }

}
