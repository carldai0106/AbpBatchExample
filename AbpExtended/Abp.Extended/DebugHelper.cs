namespace Abp
{
    public class DebugHelper
    {
        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#endif
                return false;
                
            }
        }
    }
}
