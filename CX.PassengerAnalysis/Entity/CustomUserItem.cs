using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResourceBulidTool.Entity
{

    public class CustomUserCollection : List<CustomUserItem>
    {
        private static CustomUserCollection current = null;

        public static CustomUserCollection Current
        {
            get
            {

                if (current == null)
                {
                    current = new CustomUserCollection();
                }
                return CustomUserCollection.current;
            }
        }

        public CustomUserItem Get()
        {
            var userCollection = this.Where(item => item.State == 0).ToList();

            if (userCollection.Count > 0)
            {
                return userCollection[0];
            }
            return null;
        }

        public bool Completed()
        {
            return this.Count(item => item.State == 2) == 0 && this.Count(item => item.State ==0) == 0;
        }
    }
    /// <summary>
    /// 自定义用户项目
    /// </summary>
   public class CustomUserItem
    {
       public string UserName { get; set; }

       public string PassWord { get; set; }

       /// <summary>
       /// 0未使用 1使用成功 2使用中 3失败 4用户已经使用  
       /// </summary>
       public int State { get; set; }
    }
}
