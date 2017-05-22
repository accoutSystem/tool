using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTool.Common
{
    public class RegisterMethod
    {
        public static string GetChineseSpellCode(string unicodeString)
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

        public static string GetRandom(int length)
        {

            string newCode = "";

            for (int i = 0; i < length; i++)
            {
                newCode += e.Next(10);
            }

            return newCode;
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
}
