﻿using glib.Email;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToolWebManager.Base;

namespace ToolWebManager.Email
{
    public partial class CXEmaiList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                Query();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void Query() 
        {
            string sql = "select messageid,messagefilename,messagefrom,messagecreatetime,'' title from email.hm_messages where   messageaccountid='" + LoginUserInfo.Current.AccountId + "' order by messagecreatetime desc";
         
            DataTransaction data = EmailHelper.GetConnection(LoginUserInfo.Current.EmailAddress);

            bool isOtherEmail = EmailHelper.IsOtherEmail(LoginUserInfo.Current.EmailAddress);

            var source = data.DoGetDataTable(sql);

            foreach (DataRow row in source.Rows) 
            {
                try
                {
                    var file =  row["messagefilename"] + string.Empty;

                    var email = EmailHelper.GetEmaiLStream(LoginUserInfo.Current.EmailAddress, file, isOtherEmail);

                    row["title"] = DecodeEncodedWordValue(email.Subject);
                }
                catch (Exception ex)
                {
                    row["title"] = ex.Message;
                }
            }
            GridView1.DataSource = source;

            GridView1.DataBind();
        }

       
        public static string DecodeEncodedWordValue(string mimeString)
        {
            var regex = new Regex(@"=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<value>.*?)\?=");
            var encodedString = mimeString;
            var decodedString = string.Empty;

            while (encodedString.Length > 0)
            {
                var match = regex.Match(encodedString);
                if (match.Success)
                {
                    // If the match isn't at the start of the string, copy the initial few chars to the output
                    decodedString += encodedString.Substring(0, match.Index);

                    var charset = match.Groups["charset"].Value;
                    var encoding = match.Groups["encoding"].Value.ToUpper();
                    var value = match.Groups["value"].Value;

                    if (encoding.Equals("B"))
                    {
                        // Encoded value is Base-64
                        var bytes = Convert.FromBase64String(value);
                        decodedString += Encoding.GetEncoding(charset).GetString(bytes);
                    }
                    else if (encoding.Equals("Q"))
                    {
                        // Encoded value is Quoted-Printable
                        // Parse looking for =XX where XX is hexadecimal
                        var regx = new Regex("(\\=([0-9A-F][0-9A-F]))", RegexOptions.IgnoreCase);
                        decodedString += regx.Replace(value, new MatchEvaluator(delegate(Match m)
                        {
                            var hex = m.Groups[2].Value;
                            var iHex = Convert.ToInt32(hex, 16);

                            // Return the string in the charset defined
                            var bytes = new byte[1];
                            bytes[0] = Convert.ToByte(iHex);
                            return Encoding.GetEncoding(charset).GetString(bytes);
                        }));
                        decodedString = decodedString.Replace('_', ' ');
                    }
                    else
                    {
                        // Encoded value not known, return original string
                        // (Match should not be successful in this case, so this code may never get hit)
                        decodedString += encodedString;
                        break;
                    }

                    // Trim off up to and including the match, then we'll loop and try matching again.
                    encodedString = encodedString.Substring(match.Index + match.Length);
                }
                else
                {
                    // No match, not encoded, return original string
                    decodedString += encodedString;
                    break;
                }
            }
            return decodedString;
        }
    }
}