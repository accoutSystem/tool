using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolWebManager.Base
{
    public class ResourceLock
    {
        public static bool lockResource=false;
        public static bool IsLock() {
            return lockResource;
        }

        public static void Lock() {
            lockResource = true;

        }

        public static void UnLock()
        {
            lockResource = false;
        }
    }
}