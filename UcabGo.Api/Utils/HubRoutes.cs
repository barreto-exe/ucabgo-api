using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcabGo.Api.Utils
{
    public static class HubRoutes
    {
        public static string CHAT_HUB { get => "chat"; }
        public static string CHAT_RECEIVE_MESSAGE { get => "ReceiveMessage"; }

        public static string ACTIVE_RIDE_HUB { get => "activeride"; }
        public static string ACTIVE_RIDE_RECEIVE_UPDATE { get => "ReceiveUpdate"; }

        public static string RIDES_MATCHING_HUB { get => "ridesmatching"; }
        public static string RIDES_MATCHING_RECEIVE_UPDATE { get => "ReceiveUpdate"; }
    }
}
