using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    public class PassCodeProviderFactory
    {
        /// <summary>
        /// 平台类型
        /// </summary>
        public static int PlatformType { get; set; }

        public static IPassCodeProvider GetPlatform()
        {
            IPassCodeProvider p = null;
            switch (PlatformType)
            {
                case 1: p = new YPYPassCodeProvider(); break;
                case 2: p = new AMPassCodeProvider(); break;
                case 3: p = new YMPassCodeProvider(); break;
                case 4: p = new YunMaPassCodeProvider(); break;
                case 5: p = new ZMPassCodeProvider(); break;
                case 6: p = new KMPassCodeProvider(); break;
                case 7: p = new FQPassCodeProvider(); break;
                case 8: p = new FMPassCodeProvider(); break;
            }
            return p;
        }
    }
}
