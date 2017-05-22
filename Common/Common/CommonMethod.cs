using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyTool.Common
{
    public class ToolCommonMethod
    {
        public static bool IsProvideServer()
        {
            if (DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:58:00")) ||
                  DateTime.Now < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 06:01:00")))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsClearServer()
        {
            if (DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:02:00")) ||
                  DateTime.Now < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 07:01:00")))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsProvideServerValid()
        {
            if (DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 22:52:00")) ||
                  DateTime.Now < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 07:03:00")))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static string GetChineseSpellCode(string unicodeString)
        {
            return GetChineseSpellCodeIn2(unicodeString);
        }

        /// <summary>
        /// 已废弃分析中文的方法
        /// </summary>
        /// <param name="unicodeString"></param>
        /// <returns></returns>
        public static string GetChineSpellCodeInHis(string unicodeString)
        {
            int i = 0;
            ushort key = 0;
            string strResult = string.Empty;            //创建两个不同的encoding对象     
            Encoding unicode = Encoding.Unicode;
            //创建GBK码对象     
            Encoding gbk = Encoding.GetEncoding(936);
            //将unicode字符串转换为字节     
            byte[] unicodeBytes = unicode.GetBytes(unicodeString);
            //再转化为GBK码     
            byte[] gbkBytes = Encoding.Convert(unicode, gbk, unicodeBytes);
            while (i < gbkBytes.Length)
            {
                //如果为数字\字母\其他ASCII符号     
                if (gbkBytes[i] <= 127)
                {
                    strResult = strResult + (char)gbkBytes[i];
                    i++;
                }
                else
                {
                    key = (ushort)(gbkBytes[i] * 256 + gbkBytes[i + 1]);

                    if (key >= '\uB0A1' && key <= '\uB0C4')
                    {
                        strResult = strResult + "A";
                    }
                    else if (key >= '\uB0C5' && key <= '\uB2C0')
                    {
                        strResult = strResult + "B";
                    }
                    else if (key >= '\uB2C1' && key <= '\uB4ED')
                    {
                        strResult = strResult + "C";
                    }
                    else if (key >= '\uB4EE' && key <= '\uB6E9')
                    {
                        strResult = strResult + "D";
                    }
                    else if (key >= '\uB6EA' && key <= '\uB7A1')
                    {
                        strResult = strResult + "E";
                    }
                    else if (key >= '\uB7A2' && key <= '\uB8C0')
                    {
                        strResult = strResult + "F";
                    }
                    else if (key >= '\uB8C1' && key <= '\uB9FD')
                    {
                        strResult = strResult + "G";
                    }
                    else if (key >= '\uB9FE' && key <= '\uBBF6')
                    {
                        strResult = strResult + "H";
                    }
                    else if (key >= '\uBBF7' && key <= '\uBFA5')
                    {
                        strResult = strResult + "J";
                    }
                    else if (key >= '\uBFA6' && key <= '\uC0AB')
                    {
                        strResult = strResult + "K";
                    }
                    else if (key >= '\uC0AC' && key <= '\uC2E7')
                    {
                        strResult = strResult + "L";
                    }
                    else if (key >= '\uC2E8' && key <= '\uC4C2')
                    {
                        strResult = strResult + "M";
                    }
                    else if (key >= '\uC4C3' && key <= '\uC5B5')
                    {
                        strResult = strResult + "N";
                    }
                    else if (key >= '\uC5B6' && key <= '\uC5BD')
                    {
                        strResult = strResult + "O";
                    }
                    else if (key >= '\uC5BE' && key <= '\uC6D9')
                    {
                        strResult = strResult + "P";
                    }
                    else if (key >= '\uC6DA' && key <= '\uC8BA')
                    {
                        strResult = strResult + "Q";
                    }
                    else if (key >= '\uC8BB' && key <= '\uC8F5')
                    {
                        strResult = strResult + "R";
                    }
                    else if (key >= '\uC8F6' && key <= '\uCBF9')
                    {
                        strResult = strResult + "S";
                    }
                    else if (key >= '\uCBFA' && key <= '\uCDD9')
                    {
                        strResult = strResult + "T";
                    }
                    else if (key >= '\uCDDA' && key <= '\uCEF3')
                    {
                        strResult = strResult + "W";
                    }
                    else if (key >= '\uCEF4' && key <= '\uD188')
                    {
                        strResult = strResult + "X";
                    }
                    else if (key >= '\uD1B9' && key <= '\uD4D0')
                    {
                        strResult = strResult + "Y";
                    }
                    else if (key >= '\uD4D1' && key <= '\uD7F9')
                    {
                        strResult = strResult + "Z";
                    }
                    else
                    {
                        strResult = strResult + "1";
                    }
                    i = i + 2;
                }
            }

            return strResult;
        }

        private static string GetChineseSpellCodeIn2(string ss)
        {
            var cc = ss.ToArray().ToList();
            string fd = string.Empty;
            foreach (var sss in cc)
            {
                fd += GetSpell(sss.ToString());
            }
            return fd;
        }

        private static string GetSpell(string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };

                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25)
                    {
                        max = areacode[i + 1];
                    }
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(97 + i) }).ToUpper();
                    }
                }
                return "1";
            }
            else return cnChar;
        }  

        public static string GetBirthdayIDNo(string identityCard)
        {
            string birthday = "";

            //处理18位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 18)
            {
                birthday = identityCard.Substring(6, 4) + "" + identityCard.Substring(10, 2) + "" + identityCard.Substring(12, 2);
            }
            //处理15位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 15)
            {
                birthday = "19" + identityCard.Substring(6, 2) + "" + identityCard.Substring(8, 2) + "" + identityCard.Substring(10, 2);
            }

            return birthday;
        }

        public static string GetBirthdayIDNoTo(string identityCard)
        {
            string birthday = "";

            //处理18位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 18)
            {
                birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
            }
            //处理15位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 15)
            {
                birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
            }

            return birthday;
        }

        public static bool IsIdNo(string identityCard) {
            if (identityCard.Length == 18||identityCard.Length == 15)
            {
                return true;
            }
            return false;
        
        }

        public static string GetCharRandom(int length)
        {
            string newCode = "";

            for (int i = 0; i < length; i++)
            {
                newCode += Convert.ToChar(e.Next(97, 122));
            }

            return newCode;
        }

        public static string GetRandom(int length)
        {

            string newCode = "";

            for (int i = 0; i < length; i++)
            {
                newCode += e.Next(10);
            }

            return newCode;
        }
        private static List<string> phoneSegment = new List<string>();
        public static string GetPhoneSegment()
        {
            if (phoneSegment.Count <= 0)
            {
                string str = "134|135|136|137|138|139|147|150|151|152|157|158|159|178|182|183|184|187|188|130|131|132|155|156|185|186|145|176|133|153|177|180|181|189";
                phoneSegment.AddRange(str.Split('|').ToList());
            }

            Random r = new Random();
            return phoneSegment[r.Next(phoneSegment.Count)];
        }

         static Random e = new Random();
        public static string GetRandom(int length,int maxNumber)
        {
          

            string newCode = "";

            for (int i = 0; i < length; i++)
            {
                newCode += e.Next(maxNumber);
            }

            return newCode;
        }
    }

    /// <summary>
    /// 畅行数据加密
    /// </summary>
    public class CXDataCipher {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        private static string encryptKey = "lichao12";
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encryption(string encryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Deciphering(string decryptString)
        {
            Console.WriteLine("畅行加密系统开始解密"+decryptString);
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
                Console.WriteLine("畅行加密系统解密成功" );
            }
            catch(Exception ex)
            {
                Console.WriteLine("畅行加密系统解密失败" + ex.Message);
                return decryptString;
            }
        }
        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string EncryptionUserPW(string encryptString)
        {
            string value = Encryption(encryptString);

            if (value.Equals(encryptString))
            {
                return encryptString;
            }
            else
            {
                return "-|" + value;
            }

        }
        /// <summary>
        /// 解密密码
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string DecipheringUserPW(string decryptString)
        {
            if (decryptString.Contains("-|"))
            {
                decryptString = decryptString.Remove(0, 2);
                string value = Deciphering(decryptString);

                return value;
            }
            else
            {
                return decryptString;
            }
        }
    }
}
