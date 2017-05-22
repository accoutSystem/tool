using ChangePassWord.Entiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChangePassWord.Base
{
    public interface IWebRequest
    {
          string Request(ChangePassWordSession session);
    }
}
