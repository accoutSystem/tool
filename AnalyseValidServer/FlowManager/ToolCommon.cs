using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTool.FlowManager
{
   public  class ToolCommon
    {
        public static string Path = Environment.CurrentDirectory + @"\Flow\";
        public static bool IsProvideServer()
        {
            if (DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 22:55:00")) ||
                  DateTime.Now < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 07:01:00")))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
