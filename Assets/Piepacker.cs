using System.Runtime.InteropServices;

namespace PiepackerSDK
{
    public static class Piepacker
    {
        [DllImport("__Internal")]
        public static extern string SessionID(); //use in connection call with param sessionId

        [DllImport("__Internal")]
        public static extern string UserSessionID(); //use in connection call with param userSessionId

        [DllImport("__Internal")]
        public static extern int PlayerIdx();
        
        [DllImport("__Internal")]
        public static extern string ClusterName();

        [DllImport("__Internal")]
        public static extern string ConnectionHost();

        [DllImport("__Internal")]
        public static extern string DisplayName();
    }
}
