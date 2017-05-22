using Fangbian.Log;
using Maticsoft.Model;
using MyEntiry;
using MyTool.Common;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PD.Business;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool
{
    public partial class ExportResourcePage : Form
    {
        private int count = 0;

        List<T_NewAccountEntity> accountCollection = new List<T_NewAccountEntity>();
        public ExportResourcePage()
        {
            InitializeComponent();
            Load += ExportResourcePage_Load;
        }

        void ExportResourcePage_Load(object sender, EventArgs e)
        {
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Id";
            comboBox1.DataSource = ToolMain.BusinessCollection;

            comboBox2.SelectedIndex = 0;
        }
     
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("请选择Business");
                return;
            }

            if (MessageBox.Show("确定导出资源给当前商户?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Cancel)
                return;
           
            var business = comboBox1.SelectedItem as T_Business;

            var goodResource = checkBox1.Checked;
            type = comboBox2.SelectedItem + string.Empty;
            var accountType = comboBox2.SelectedItem + string.Empty;

            switch (accountType)
            {
                case "普通号":
                    accountType = "0";
                    break;
                case "连号(不支持)":
                   accountType = "1";  

                   break;
                case "密码一致号(不支持)":
                   accountType = "3";  
                    break;
                case "客户王明宇":
                    accountType = "4";
                    break;
                case "翻新号(删除联系人)":
                    accountType = "5";
                    break;
            }

            accountCollection.Clear();
            var tempAccountType = accountType;
            if (accountType == "1")
            {
                accountType += " and t_newaccount.userName like '" + textBox1 .Text+ "%'";
            }

            if (!goodResource)
            {
               
                Export( string.Format(@" select a.*,t_useremail.passWord emailPassword from (select  * from t_newaccount
 where t_newaccount.state=2   and  t_newaccount.accountType={1} {2} limit 0,{0}) a
 left join t_useremail on
               a.email=t_useremail.email", tbnumber.Text,
                                         accountType, tempAccountType == "1" ? "order by t_newaccount.userName" : (checkBox2.Checked ? "" : "order by createTime")));
            }
            else
            {
                Export(string.Format(@"select a.*,t_useremail.passWord emailPassword from (select  t_newaccount.*  from t_newaccount
                                where t_newaccount.state=10 
and   t_newaccount.accountType={1} {2} limit 0,{0}) a
left join t_useremail on  a.email=t_useremail.email ", tbnumber.Text, accountType, tempAccountType == "1" ? "order by t_newaccount.userName" : (checkBox2.Checked ? "" : "order by createTime")));
            }
        }
     

        private void Export(string sql)
        {

            var accountType = comboBox2.SelectedItem + string.Empty;
            var business = comboBox1.SelectedItem as T_Business;
            var bulidNoEmail = checkBox4.Checked;
            Thread th = new Thread(new ThreadStart(() =>
            {
                var db = DataTransaction.Create();

                try
                {
                    if (checkBox3.Checked)
                    {
                        ExcuteData();
                    }
                    Console.WriteLine(sql);

                    System.Data.DataTable data = null;

                    var number = Convert.ToInt32(tbnumber.Text);
                    
                    data = db.Query(sql).Tables[0];
                    
                    if (data.Rows.Count < number)
                    {
                        MessageBox.Show("资源数据不足" + number + "个,实际" + data.Rows.Count+"个");
                        return;
                    }

                    foreach (DataRow row in data.Rows)
                    {
                        string guid = row["userguid"] + string.Empty;
                        if (accountCollection.Count(item => item.UserGuid.Equals(guid)) > 0)
                            continue;

                        accountCollection.Add(new T_NewAccountEntity
                        {
                            UserGuid = guid,
                            UserName = row["username"] + string.Empty,
                            PassWord = row["password"] + string.Empty,
                            CreateTime = Convert.ToDateTime(row["CreateTime"] + ""),
                            PwdQuestion = row["PwdQuestion"] + string.Empty,
                            PwdAnswer = row["PwdAnswer"] + string.Empty,
                            Phone = row["Phone"] + string.Empty,
                            PassengerName = row["PassengerName"] + string.Empty,
                            PassengerId = row["PassengerId"] + string.Empty,
                            LastTime = Convert.ToDateTime(row["LastTime"] + string.Empty),
                            businessId = row["businessId"] + string.Empty,
                            Email = row["Email"] + string.Empty,
                            IVR_passwd = row["IVR_passwd"] + string.Empty,
                            State = Convert.ToInt32(row["state"] + string.Empty),
                            EmailPassWord = row["emailPassword"] + string.Empty
                        });
                    }

                    //accountCollection.Sort(new FilesNameComparerClass());
                    string path = System.Environment.CurrentDirectory + @"\Data\" + business.Name + @"\"+DateTime.Now.ToString("yyyy-MM-dd")+@"\";

                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    string allPath = path + business.Name + DateTime.Now.ToString("yyyy-MM-dd HH mm") + "-" + tbnumber.Text + "行" + ".txt";
                    string userPath = path + business.Name + DateTime.Now.ToString("yyyy-MM-dd HH mm") + "-" + tbnumber.Text + "行user" + ".txt";
                    string emailPath = path + business.Name + DateTime.Now.ToString("yyyy-MM-dd HH mm") + "-" + tbnumber.Text + "行email" + ".txt";

                    var sheet = CreateExcel();

                    HSSFWorkbook noEmailPWSHeet = null;
                    if (bulidNoEmail)
                    {
                        noEmailPWSHeet = CreateExcel();
                    }

                    foreach (var user in accountCollection)
                    {
                        if (accountType == "翻新号(删除联系人)")
                        {
                            WriteRepayLog(user, business.Id);
                        }

                        user.PassWord = CXDataCipher.DecipheringUserPW(user.PassWord);

                        user.businessId = business.Id;

                        user.LastTime = DateTime.Now;

                        if (checkBox1.Checked)
                        {
                            user.State = 12;//特殊账号已出
                        }
                        else
                        {
                            user.State = 6;//已出且验证
                        }

                        user.BuyTime = DateTime.Now;

                        try
                        {
                           

                            UpdateAccount(user);

                            Storage(user, allPath);

                            StorageUser(user, userPath);

                            StorageEmail(user, emailPath);

                            WriteExcelRow(user, accountCollection.IndexOf(user), sheet,true);

                            if (bulidNoEmail)
                            {
                                WriteExcelRow(user, accountCollection.IndexOf(user), noEmailPWSHeet,false);
                            }
                            WriteMessage("已导出" + accountCollection.IndexOf(user) + "个");


                        }
                        catch(Exception ex)
                        {
                            Logger.Fatal("写入文件失败" + ex.Message+ ex.StackTrace);
                        }
                    }

                    for (int col = 0; col < 11; col++)
                    {
                        sheet.GetSheet("资源列表").AutoSizeColumn(col);
                    }

                    var ss = File.Create(allPath.Replace("txt", "xls"));
                    sheet.Write(ss);
                    ss.Close();
                    ss.Dispose();
                    if (bulidNoEmail)
                    {
                        string tempallPath = path + business.Name + DateTime.Now.ToString("yyyy-MM-dd HH mm") + "-" + tbnumber.Text + "行-E" + ".xls";
                        var noEmail = File.Create(tempallPath);
                        noEmailPWSHeet.Write(noEmail);
                        noEmail.Close();
                        noEmail.Dispose();
                    }
                    path = System.Environment.CurrentDirectory + @"\Data\" + business.Name + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\";
                    //打开文件夹
                    Process.Start("explorer.exe", path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("获取数据失败" + ex.Message);
                }
            }));

            th.Start();
        }

        private void WriteRepayLog(T_NewAccountEntity user,string newbusinessid) 
        {

            var db = PD.Business.DataTransaction.Create();

            db.ExecuteSql("insert into t_buyinrepeatlog(newBusinessId,oldBusinessId,userName,passWord) values(" + newbusinessid + ","+user.businessId+",'"+user.UserName+"','"+user.PassWord+"')");

        }

        private void WriteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                count++;
                if (count == 20)
                {
                    count = 0;
                    textBox2.Text = string.Empty;
                }
                textBox2.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;
            }));
        }
        string type = string.Empty;
        private void UpdateAccount(T_NewAccountEntity user)
        {
           
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            if (type.Equals("删除联系人号"))
            {
                strSql.Append("update t_hisnewaccount set buyTime=@buyTime, state=@State,LastTime=@LastTime,businessId=@businessId where userGuid=@userGuid");
            }
            else { 
            strSql.Append("update t_newaccount set buyTime=@buyTime, state=@State,LastTime=@LastTime,businessId=@businessId where userGuid=@userGuid");
            } ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@LastTime", DbType.DateTime,0),
					new ParameterInfo("@businessId", DbType.String,45),
					new ParameterInfo("@buyTime", DbType.DateTime,0),    };

            parameters[0].Value = user.UserGuid;
            if (type.Equals("删除联系人号"))
            {
                parameters[1].Value ="26";
            }
            else { 
            parameters[1].Value = user.State;
            }
            parameters[2].Value = user.LastTime;

            parameters[3].Value = user.businessId;
            parameters[4].Value = user.BuyTime;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();

            var sqls = new List<SqlParamterItem> { };

            sqls.Add(sql);

            string log = "insert into t_buylog(businessId,username,detail,lastTime) values('" + user.businessId + "','" + user.UserName + "','" + (user.State == 12 ? "手机核验" : "普通") + "新资源转售出','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            sqls.Add(new SqlParamterItem { Sql = log, ParamterCollection = new List<ParameterInfo> { } });

            DataTransaction.Create().ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls);
        }

      
        #region 写入Excel
        private HSSFWorkbook CreateExcel()
        {
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet("资源列表");
            int rowIndex = 0;
            IRow headerrow = sheet.CreateRow(rowIndex);
            headerrow.Height = 22 * 20;
            rowIndex++;
            ICellStyle style = book.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            List<string> headers = new List<string>() { "Guid", "用户名", "密码", "注册乘车人", "乘车人身份证", "注册时间", "邮件", "电话号码", "邮箱密码", "密保", "密保答案", "语音密码" };

            int i = 0;
            //表头
            foreach (var v in headers)
            {
                ICell cell = headerrow.CreateCell(i);
                cell.CellStyle = style;
                cell.SetCellValue(v);
                i++;
            }

            return book;
        }

        private void WriteExcelRow(T_NewAccountEntity item, int rowIndex, HSSFWorkbook book,bool wirteEmailPW)
        {
            ICellStyle contentStyleLeft = book.CreateCellStyle();
            contentStyleLeft.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            contentStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            contentStyleLeft.WrapText = true;
            rowIndex++;
            IRow contentRow = book.GetSheet("资源列表").CreateRow(rowIndex);
            contentRow.Height = 22 * 20;
            //"Guid",  
            ICell cell = contentRow.CreateCell(0);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.UserGuid);
            //"用户名",  
            cell = contentRow.CreateCell(1);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.UserName);
            //"密码",  
            cell = contentRow.CreateCell(2);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.PassWord);
            //"注册乘车人",  
            cell = contentRow.CreateCell(3);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.PassengerName);
            //"乘车人身份证",  
            cell = contentRow.CreateCell(4);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.PassengerId);
            //"注册时间",  
            cell = contentRow.CreateCell(5);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            //"邮件",  
            cell = contentRow.CreateCell(6);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.Email);
            //"电话号码",  
            cell = contentRow.CreateCell(7);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.Phone);
            //"邮箱密码",  
            cell = contentRow.CreateCell(8);
            cell.CellStyle = contentStyleLeft;
            if (wirteEmailPW)
            {
                cell.SetCellValue(item.EmailPassWord);
            }
            else {
                cell.SetCellValue("");
            
            }
            //"密保",  
            cell = contentRow.CreateCell(9);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.PwdAnswer);
            //"密保答案",  
            cell = contentRow.CreateCell(10);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.PwdQuestion);
            //"语音密码",  
            cell = contentRow.CreateCell(11);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.IVR_passwd);
        }

        #endregion

        #region 写入文件
        private void Storage(T_NewAccountEntity item, string path)
        {
            WriteMessage(item.UserName + "写入文件");

            try
            {

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);

                if (string.IsNullOrEmpty(item.EmailPassWord))
                {
                    item.EmailPassWord = "lichao";
                }

                sw.WriteLine(item.UserGuid + "," + item.UserName + "," + item.PassWord + "," + item.PassengerName + "," + item.PassengerId + "," + item.CreateTime + "," + item.Email + "," + item.Phone + "," + item.EmailPassWord + "," + item.PwdAnswer + "," + item.PwdQuestion + "," + item.IVR_passwd);

                sw.Close();

                sw.Dispose();

                WriteMessage(item.UserName + "写入文件成功");
            }
            catch (Exception ex)
            {
                WriteMessage(item.UserName + "写入文件失败" + ex.Message);
            }
        }

        private void StorageUser(T_NewAccountEntity item, string path)
        {
            WriteMessage(item.UserName + "写入文件");

            try
            {

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);
             
                sw.WriteLine(item.UserName + "," + item.PassWord);

                sw.Close();

                sw.Dispose();

                WriteMessage(item.UserName + "写入文件成功");
            }
            catch (Exception ex)
            {
                WriteMessage(item.UserName + "写入文件失败" + ex.Message);
            }
        }
        private void StorageEmail(T_NewAccountEntity item, string path)
        {
            WriteMessage(item.UserName + "写入文件");

            try
            {

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(item.Email + "," + item.PassWord);

                sw.Close();

                sw.Dispose();

                WriteMessage(item.UserName + "写入文件成功");
            }
            catch (Exception ex)
            {
                WriteMessage(item.UserName + "写入文件失败" + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 同步可能复位的数据
        /// </summary>
        private void ExcuteData()
        {
            WriteMessage("开始分析已经出货的错误数据");
            string sql = @"select userguid from (
select  t_newaccount.*  from t_newaccount
                                where t_newaccount.state=10 
and   t_newaccount.accountType=0 ) asa
inner join t_buylog on asa.username=t_buylog.username";
            var data = PD.Business.DataTransaction.Create();
            var source = data.Query(sql).Tables[0];
            foreach (DataRow row in source.Rows)
            {
                data.ExecuteSql("update t_newaccount set state =12 where  UserGuid='" + row["UserGuid"] + "'");
            }
            WriteMessage("分析已经出货的错误数据成功");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("请选择Business");
                return;
            }
            if (MessageBox.Show("确定导出资源给当前商户?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Cancel)
                return;

            var business = comboBox1.SelectedItem as T_Business;

            var goodResource = checkBox1.Checked;

            accountCollection.Clear();

            if (!goodResource)
            {
                Export(string.Format(@"   select a.*,t_useremail.passWord emailPassword from (select  * from t_newaccount
 where t_newaccount.state=2 and t_newaccount.isActive=1 and  t_newaccount.accountType=1 order by username  limit 0,{0}) a
 left join t_useremail on
               a.email=t_useremail.email", tbnumber.Text));
            }
            else
            {
                Export(string.Format(@"select a.*,t_useremail.passWord emailPassword from (select  t_newaccount.*  from t_newaccount
                                 where t_newaccount.state=10 
 and t_newaccount.isActive=1 and t_newaccount.accountType=1 order by username  limit 0,{0}) a
 left join t_useremail on   a.email=t_useremail.email ", tbnumber.Text));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("请选择Business");
                return;
            }
            if (MessageBox.Show("确定导出资源给当前商户?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Cancel)
                return;

            var business = comboBox1.SelectedItem as T_Business;

            var goodResource = checkBox1.Checked;

            accountCollection.Clear();

            if (!goodResource)
            {
                Export(string.Format(@"   select a.*,t_useremail.passWord emailPassword from (select  * from t_newaccount
 where t_newaccount.state=2 and t_newaccount.isActive=1 and  t_newaccount.accountType=3 order by username  limit 0,{0}) a
 left join t_useremail on
               a.email=t_useremail.email", tbnumber.Text));
            }
            else
            {
                Export(string.Format(@"select a.*,t_useremail.passWord emailPassword from (select  t_newaccount.*  from t_newaccount
                                 where t_newaccount.state=10 
 and t_newaccount.isActive=1 and t_newaccount.accountType=3 order by username  limit 0,{0}) a
 left join t_useremail on   a.email=t_useremail.email ", tbnumber.Text));
            }
        }
    }

    public class FilesNameComparerClass : IComparer<T_NewAccountEntity>
    {

         

        public int Compare(T_NewAccountEntity x, T_NewAccountEntity y)
        {
            if (x == null || y == null)
                throw new ArgumentException("Parameters can't be null");

            string fileA = x.UserName  ;
            string fileB = y.UserName  ;
            char[] arr1 = fileA.ToCharArray();
            char[] arr2 = fileB.ToCharArray();

            int i = 0, j = 0;
            while (i < arr1.Length && j < arr2.Length)
            {
                if (char.IsDigit(arr1[i]) && char.IsDigit(arr2[j]))
                {
                    string s1 = "", s2 = "";
                    while (i < arr1.Length && char.IsDigit(arr1[i]))
                    {
                        s1 += arr1[i];
                        i++;
                    }
                    while (j < arr2.Length && char.IsDigit(arr2[j]))
                    {
                        s2 += arr2[j];
                        j++;
                    }

                    if (int.Parse(s1) > int.Parse(s2))
                    {
                        return 1;
                    }

                    if (int.Parse(s1) < int.Parse(s2))
                    {
                        return -1;
                    }

                }
                else
                {
                    if (arr1[i] > arr2[j])
                    {
                        return 1;
                    }

                    if (arr1[i] < arr2[j])
                    {
                        return -1;
                    }
                    i++;
                    j++;

                }

            }

            if (arr1.Length == arr2.Length)
            {
                return 0;
            }
            else
            {
                return arr1.Length > arr2.Length ? 1 : -1;
            }
        }
    }
}
