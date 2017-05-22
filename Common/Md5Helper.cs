#region source code header

// solution:Workstation
// created:2015-02-13
// modify:2015-02-27
// copyright fangbian.com 2015

#endregion

#region

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace CX.Common
{
    public class Md5Helper
    {
        public static string Md5(string str)
        {
            if (str == null) return "";
            var bs = Encoding.ASCII.GetBytes(str);
            bs = new MD5CryptoServiceProvider().ComputeHash(bs);
            var result = "";
            for (var i = 0; i < bs.Length; i++)
                result += bs[i].ToString("x").PadLeft(2, '0');
            return result;
        }

        public static string Md5(string str, Encoding encoding)
        {
            if (str == null) return "";
            var bs = encoding.GetBytes(str);
            bs = new MD5CryptoServiceProvider().ComputeHash(bs);
            var result = "";
            for (var i = 0; i < bs.Length; i++)
                result += bs[i].ToString("x").PadLeft(2, '0');
            return result;
        }

        public static string GetMD5HashFromByte(byte[] Data)
        {
            try
            {
                if (Data == null)
                    return null;
                MD5 md5 = new MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(Data);

                var sb = new StringBuilder();
                for (var i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                MD5 md5 = new MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(file);
                file.Close();

                var sb = new StringBuilder();
                for (var i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}