using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ChangePassWord.PassCode
{
    public class AnalysePassCode
    {
        // Fields
        internal static int index = LoadLibFromFile(@"C:\code\12306kyfw.lib", "123");

        // Methods
        public static string GetCheckCode(string imagebase64)
        {
            if (string.IsNullOrEmpty(imagebase64))
            {
                return null;
            }
            StringBuilder code = new StringBuilder();
            byte[] fileBuffer = Convert.FromBase64String(imagebase64);
            int length = fileBuffer.Length;
            try
            {
                int num1 = !GetCodeFromBuffer(index, fileBuffer, length, code) ? 0 : 1;
                return code.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [DllImport(@"C:\code\Sunday.dll")]
        internal static extern bool GetCodeFromBuffer(int LibFileIndex, byte[] FileBuffer, int ImgBufLen, StringBuilder Code);
        [DllImport(@"C:\code\Sunday.dll")]
        internal static extern bool GetCodeFromFile(int LibFileIndex, string FilePath, StringBuilder Code);
        [DllImport(@"C:\code\Sunday.dll")]
        internal static extern int LoadLibFromFile(string LibFilePath, string nSecret);

    }
}
