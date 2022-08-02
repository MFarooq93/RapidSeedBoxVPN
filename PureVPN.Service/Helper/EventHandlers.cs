using PureVPN.Entity.Models;
using PureVPN.SpeedTest;

namespace PureVPN.Service.Helper
{
    public class EventHandlers
    {
        public delegate void CustomStatusEvent(StatusModel status);
        public delegate void DisconnectStatusEvent(AfterConnectionModel model);
        public delegate void PacketTransmitEvent(StatusModel status);
        public delegate void InitializingStatusEvent(InitializingStatusModel status);
        public delegate void NotificationsUnreadCountChangedEvent(int unreadCount);
    }
}
