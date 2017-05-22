using ResourceBulidTool.PassCodeProvider;
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

        public static string GetName() {
            switch (PlatformType)
            {
                case 2:return "爱码";
                case 3:return "壹码";
                case 4: return "畅行短信平台";
                case 10: return "一片云新";
                case 1: return "一片云";
                case 5: return "飞码";
                case 11: return "呀呀呀";
            }
            return string.Empty;
        }

        public static IPassCodeProvider GetPlatform()
        {
            IPassCodeProvider p = null;
            switch (PlatformType)
            {
                case 1: p = new YPYPassCodeProvider(); break;
                case 2: p = new AMPassCodeProvider(); break;
                case 3: p = new YMPassCodeProvider(); break;
                case 4: p = new CXPasscodeProvider(); break;
                case 5: p = new FMPassCodeProvider(); break;
                case 10: p = new YPYNewPassCodeProvider(); break;
                case 11: p = new YYYPassCodeProvider(); break;
                    
            }
            Current = p;
            return p;
        }

        public static IPassCodeProvider Current { get; set; }
    }
}
