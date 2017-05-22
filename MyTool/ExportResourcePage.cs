using Maticsoft.Model;
using Microsoft.Office.Interop.Excel;
using MyEntiry;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PD.Business;
using System;
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

            accountCollection.Clear();

            Thread th = new Thread(new ThreadStart(() =>
            {
                var db = DataTransaction.Create();

                try
                {
                    System.Data.DataTable data =null;
                    var number = Convert.ToInt32(tbnumber.Text);
                    if (!goodResource)
                    {
                        data = db.Query(string.Format(@"   select a.*,t_useremail.passWord emailPassword from (select  * from t_newaccount
 where t_newaccount.state=2 and t_newaccount.isActive=1 and  t_newaccount.accountType=0 limit 0,{0}) a
 left join t_useremail on
               a.email=t_useremail.email", number)).Tables[0];
                    }
                    else
                    {
                        data = db.Query(string.Format(@"select a.*,t_useremail.passWord emailPassword from (select  t_newaccount.*  from t_newaccount
                              inner join t_customaccount on t_newaccount.userguid=t_customaccount.userguid
                  
                                     where t_newaccount.state=10 and t_newaccount.isActive=1  and t_customaccount.accounttype=2 and t_newaccount.accountType=0  limit 0,{0}) a
left join t_useremail on
                                       a.email=t_useremail.email ", number)).Tables[0];
                    }
                    if (data.Rows.Count < number)
                    {
                        MessageBox.Show("资源数据不足" + number + "个");
                        return;
                    }
                    foreach (DataRow row in data.Rows)
                    {
                        accountCollection.Add(new T_NewAccountEntity
                        {
                            UserGuid = row["userguid"] + string.Empty,
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
                            State = Convert.ToInt32(row["state"] + string.Empty) ,
                            EmailPassWord = row["emailPassword"] + string.Empty 
                        });
                    }
                    string path = System.Environment.CurrentDirectory + @"\Data\" + business.Name + @"\";

                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    path = path + business.Name + DateTime.Now.ToString("yyyy-MM-dd HH mm") + "-" + tbnumber.Text + "行" + ".txt";

                    var sheet = CreateExcel();

                    foreach (var user in accountCollection) 
                    {
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
                            UpdateNumber(user);
                            Storage(user,path);
                            WriteExcelRow(user, accountCollection.IndexOf(user), sheet);
                            WriteMessage("已导出"+accountCollection.IndexOf(user) + "个");
                        }
                        catch 
                        { 
                          
                        }
                    }

                    for (int col = 0; col < 11; col++)
                    {
                        sheet.GetSheet("资源列表").AutoSizeColumn(col);
                    }

                    var ss = File.Create(path.Replace("txt", "xls"));
                    sheet.Write(ss);
                    ss.Close();
                    ss.Dispose();

                    path = System.Environment.CurrentDirectory + @"\Data\";
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

        private void WriteExcelRow(T_NewAccountEntity item, int rowIndex, HSSFWorkbook book)
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
            cell.SetCellValue(item.EmailPassWord);
            //"密保",  
            cell = contentRow.CreateCell(9);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.PwdAnswer );
            //"密保答案",  
            cell = contentRow.CreateCell(10);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.PwdQuestion);
            //"语音密码",  
            cell = contentRow.CreateCell(11);
            cell.CellStyle = contentStyleLeft;
            cell.SetCellValue(item.IVR_passwd);
        }
        private void Storage(T_NewAccountEntity item,string path)
        {
            WriteMessage(item.UserName +"写入文件");

            try
            {

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);

                if (string.IsNullOrEmpty(item.EmailPassWord))
                {
                    item.EmailPassWord = "lichao";
                }

                sw.WriteLine(item.UserGuid + "," + item.UserName + "," + item.PassWord + "," + item.PassengerName + "," + item.PassengerId + "," + item.CreateTime + "," + item.Email + "," + item.Phone+","+item.EmailPassWord+","+item.PwdAnswer+","+item.PwdQuestion+","+item.IVR_passwd);

                sw.Close();

                sw.Dispose();

                WriteMessage(item.UserName +"写入文件成功");
            }
            catch (Exception ex)
            {
                WriteMessage(item.UserName +"写入文件失败" + ex.Message);
            }
        }

        private int count = 0;

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

        private void UpdateNumber(T_NewAccountEntity user)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update t_newaccount set buyTime=@buyTime, state=@State,LastTime=@LastTime,businessId=@businessId where userGuid=@userGuid");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@LastTime", DbType.DateTime,0),
					new ParameterInfo("@businessId", DbType.String,45),
					new ParameterInfo("@buyTime", DbType.DateTime,0),
                                         
                                         };

            parameters[0].Value = user.UserGuid;

            parameters[1].Value = user.State;

            parameters[2].Value = user.LastTime;

            parameters[3].Value = user.businessId;
            parameters[4].Value = user.BuyTime;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();


            DataTransaction.Create().ExecuteSql(sql);
            string log = "insert into t_buylog(businessId,username,detail,lastTime) values('" + user.businessId + "','" + user.UserName + "','"+(user.State==12?"手机核验":"普通")+"新资源转售出','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            DataTransaction.Create().ExecuteSql(log);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
             
        } 
    }
}
