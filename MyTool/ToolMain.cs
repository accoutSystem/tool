using Maticsoft.Model;
using PD.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MyEntiry;
using MyDB;
using MyTool.Statistics;
using MyTool.Valid;
using System.Diagnostics;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains.WFDataItem;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

using MyTool.Export;
using MyTool.Log;
using MyTool.Common;
using MyTool.Free;
using MyTool.Passenger;
using MyTool.Valid.PW;

namespace MyTool
{
    public partial class ToolMain : Form
    {
        public static ToolMain Current { get; set; }

        static List<T_Business> businessCollection = new List<T_Business>();

        public static List<T_Business> BusinessCollection
        {
            get
            {
                return businessCollection;
            }
            set
            {
                businessCollection = value;
            }
        }

        public ToolMain()
        {
            InitializeComponent();
            Current = this;
            Load += new EventHandler(ToolMain_Load);
        }

        void ToolMain_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadBusiness();
        }

        public void LoadBusiness() 
        {
            Thread th = new Thread(new ThreadStart(() =>
            {
                try
                {
                    var db = DataTransaction.Create();

                    var data = db.Query("SELECT * FROM  t_business").Tables[0];

                    foreach (DataRow row in data.Rows)
                    {
                        businessCollection.Add(new T_Business
                        {
                            Id = row["businessId"] + "",
                            Name = row["businessName"] + "",
                            Remark = row["address"] + "",
                        });
                    }

                    this.Invoke(new Action(() =>
                    {
                        lbBusiness.DisplayMember = "Name";
                        lbBusiness.ValueMember = "Id";
                        lbBusiness.DataSource = businessCollection;
                    }));
                }
                catch { }

                LoadResourceNumber();
            }));
            th.Start();
        }

        private void 原始数据导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var db = DataTransaction.Create();
                    openFileDialog1.FileNames.ToList().ForEach(file =>
                    {
                        File.ReadAllLines(file).ToList().ForEach(item =>
                       {

                           var account = Newtonsoft.Json.JsonConvert.DeserializeObject<T_NewAccountEntity>(item);
                         

                           var userName = account.UserName;

                           WriteMessage("开始核验" + userName);

                           System.Data.DataTable data = null;

                           try
                           {
                               data = db.Query("select count(*) from t_newaccount where username='" + userName + "'").Tables[0];
                           }
                           catch (Exception ex)
                           {
                               WriteMessage("核验" + userName + "失败" + ex.Message);
                           }

                           if (data != null)
                           {
                               if (Convert.ToInt32(data.Rows[0][0]) <= 0)
                               {
                                   WriteMessage("核验" + userName + "成功,开始插入");

                                   var sql = Add(account );

                                   try
                                   {
                                       db.ExecuteSql(sql);

                                       WriteMessage("成功插入" + userName);
                                   }
                                   catch (Exception ex)
                                   {
                                       WriteMessage("插入" + userName + "失败," + ex.Message);
                                   }
                               }
                           }

                       });
                    });
                }));

                th.Start();
            }
        }

        private void WriteMessage(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message);

            //this.Invoke(new Action(() =>
            //{
            //    count++;
            //    if (count == 20)
            //    {
            //        count = 0;
            //       // textBox1.Text = string.Empty;
            //    }
            //    //textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;
            //}));
        }

        public SqlParamterItem Add(T_NewAccountEntity model)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into t_newaccount(");
            strSql.Append("UserGuid,UserName,PassWord,PassengerName,PassengerId,Email,Phone,State,CreateTime,LastTime,PwdQuestion,PwdAnswer,IVR_passwd,businessId)");
            strSql.Append(" values (");
            strSql.Append("@UserGuid,@UserName,@PassWord,@PassengerName,@PassengerId,@Email,@Phone,@State,@CreateTime,@LastTime,@PwdQuestion,@PwdAnswer,@IVR_passwd,@businessId)");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@UserName", DbType.String,40),
					new ParameterInfo("@PassWord", DbType.String,30),
					new ParameterInfo("@PassengerName", DbType.String,20),
					new ParameterInfo("@PassengerId", DbType.String,39),
					new ParameterInfo("@Email", DbType.String,45),
					new ParameterInfo("@Phone", DbType.String,20),
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@CreateTime",DbType.DateTime,0),
					new ParameterInfo("@LastTime", DbType.DateTime,0),
					new ParameterInfo("@PwdQuestion", DbType.String,45),
					new ParameterInfo("@PwdAnswer", DbType.String,45),
					new ParameterInfo("@IVR_passwd", DbType.String,45),
					new ParameterInfo("@businessId", DbType.String,45)};
            parameters[0].Value = model.UserGuid;
            parameters[1].Value = model.UserName;
            parameters[2].Value = model.PassWord;
            parameters[3].Value = model.PassengerName;
            parameters[4].Value = model.PassengerId;
            parameters[5].Value = model.Email;
            parameters[6].Value = model.Phone;
            parameters[7].Value = model.State;
            parameters[8].Value = model.CreateTime;
            parameters[9].Value = model.LastTime;
            parameters[10].Value = model.PwdQuestion;
            parameters[11].Value = model.PwdAnswer;
            parameters[12].Value = model.IVR_passwd;
            parameters[13].Value = model.businessId;
            sql.Sql = strSql.ToString();
            sql.ParamterCollection = parameters.ToList();
            return sql;
        }

        public SqlParamterItem Update(T_NewAccountEntity model)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update t_newaccount set state=3,buyTime=@buyTime,LastTime=@LastTime,businessId=@businessId where userGuid=@userGuid");
            ParameterInfo[] parameters = { new ParameterInfo("@UserGuid", DbType.String ,36),
                                         new ParameterInfo("@buyTime", DbType.DateTime ,0),
                                         new ParameterInfo("@LastTime", DbType.DateTime ,0),
                                         new ParameterInfo("@businessId", DbType.String ,36)};
            parameters[0].Value = model.UserGuid;
            parameters[1].Value = model.BuyTime;
            parameters[2].Value = model.LastTime;
            parameters[3].Value = model.businessId;
            sql.Sql = strSql.ToString();
            sql.ParamterCollection = parameters.ToList();
            return sql;
        }


        private void yM数据导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectBusinessPage page = new SelectBusinessPage();

            if (page.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("请选择一个商户");
                return;
            }
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var db = DataTransaction.Create();

                    openFileDialog1.FileNames.ToList().ForEach(file =>
                    {
                        File.ReadAllLines(file).ToList().ForEach(item =>
                       {
                           var userInfo = item.Split(',');

                           if (userInfo.Count() <= 8)
                           {
                               MessageBox.Show("格式不正确文件");

                               return;
                           }

                           var userName = userInfo[1];

                           WriteMessage("开始核验" + userName);

                           System.Data.DataTable data = null;

                           try
                           {
                               data = db.Query("select UserGuid from t_newaccount where username='" + userName + "'  ").Tables[0];
                           }
                           catch (Exception ex)
                           {
                               WriteMessage("核验" + userName + "失败" + ex.Message);
                           }

                           if (data != null)
                           {
                               if (data.Rows.Count > 0)
                               {
                                   WriteMessage("发现" + userName + "数据,开始修改");

                                   var sql = Update(new T_NewAccountEntity
                                   {
                                       UserGuid = userInfo[0],
                                       BuyTime = DateTime.Now,
                                       LastTime = DateTime.Now,
                                       businessId = page.CurrentBusiness.Id
                                   });

                                   try
                                   {
                                       db.ExecuteSql(sql);

                                       WriteMessage("修改" + userName + "成功");
                                   }
                                   catch (Exception ex)
                                   {
                                       WriteMessage("修改" + userName + "失败," + ex.Message);
                                   }
                               }
                           }
                       });
                    });
                    MessageBox.Show("导入成功");
                }));

                th.Start();
            }
        }

        private void 自动核验ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 数据导出工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 导出可用资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportResourcePage page = new ExportResourcePage();
            page.Show();
        }

        private void 数据互通服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataLinkPage page = new DataLinkPage();
            page.Show();
        }

        private void toolStripStatusLabel8_Click(object sender, EventArgs e)
        {
            LoadResourceNumber();
        }

        /// <summary>
        /// 加载资源数量 //0 新注册 1通过邮箱核验 2 已通过验证 3已出 4已出未核验 5未核验 6已出且验证
        /// </summary>
        private void LoadResourceNumber()
        {
            try
            {
                var data = DataTransaction.Create();

                var source = data.Query(@"select    SUM(1) allCount ,
           SUM(CASE State WHEN 2 THEN 1 ELSE 0 END) valid ,
   SUM(CASE State WHEN 10 THEN 1 ELSE 0 END) phone ,
SUM(CASE State WHEN 3  THEN 1 when 4 then 1 when 6 then 1 when 12 then 1 ELSE 0 END) buy ,
SUM(CASE State WHEN 5 THEN 1 ELSE 0 END) novalid ,
SUM(CASE State WHEN 1 THEN 1 ELSE 0 END) validEmail   
   from t_newaccount   ").Tables[0];


                var hissource = data.Query(@"select    SUM(1) allCount ,
           SUM(CASE State WHEN 2 THEN 1 ELSE 0 END) valid ,
   SUM(CASE State WHEN 10 THEN 1 ELSE 0 END) phone ,
SUM(CASE State WHEN 3  THEN 1 when 4 then 1 when 6 then 1 when 12 then 1 ELSE 0 END) buy ,
SUM(CASE State WHEN 5 THEN 1 ELSE 0 END) novalid ,
SUM(CASE State WHEN 1 THEN 1 ELSE 0 END) validEmail   
   from t_hisnewaccount   ").Tables[0];
                this.Invoke(new Action(() =>
                {
                    lbCount.Text = Convert.ToInt32(source.Rows[0]["allCount"] + string.Empty) + Convert.ToInt32(hissource.Rows[0]["allCount"] + string.Empty) + string.Empty;
                    lbUserPhone.Text = Convert.ToInt32(source.Rows[0]["phone"] + string.Empty) + Convert.ToInt32(hissource.Rows[0]["phone"] + string.Empty) + string.Empty;
                    lbuse1.Text = Convert.ToInt32(source.Rows[0]["valid"] + string.Empty) + Convert.ToInt32(hissource.Rows[0]["valid"] + string.Empty) + string.Empty;
                    lbBuy.Text = Convert.ToInt32(source.Rows[0]["buy"] + string.Empty) + Convert.ToInt32(hissource.Rows[0]["buy"] + string.Empty) + string.Empty;
                    lbNoValid.Text = Convert.ToInt32(source.Rows[0]["novalid"] + string.Empty) + Convert.ToInt32(hissource.Rows[0]["novalid"] + string.Empty) + string.Empty;
                    lbEmailVaild.Text = Convert.ToInt32(source.Rows[0]["validEmail"] + string.Empty) + Convert.ToInt32(hissource.Rows[0]["validEmail"] + string.Empty) + string.Empty;
                }));
            }
            catch { }

        }

        #region 根据商户选择已出数据
        private void lbBusiness_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCount();
            Query();
        }

        private int currentPage = 0;

        public int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;
                this.Invoke(new Action(() =>
                {
                    lbCurrentPage.Text = string.Format("当前第{0}页", value);

                }));
            }
        }

        int pageSize = 20;

        int pageCount = 0;

        /// <summary>
        /// 加载已出总数据
        /// </summary>
        private void LoadCount()
        {
            var currentBusiness = lbBusiness.SelectedItem as T_Business;

            string start = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            string end = dateTimePicker2.Value.ToString("yyyy-MM-dd");

            string userName = tbUserName.Text;

            Thread th = new Thread(new ThreadStart(() =>
            {

                if (currentBusiness == null)
                {
                    return;
                }
                try
                {
                    var source = DataTransaction.Create().Query(string.Format("select count(*) from t_hisnewaccount where businessid='{0}' and buyTime>='{1} 00:00:01' and buyTime<='{2} 23:59:59' {3}",
                          currentBusiness.Id,
                          start,
                          end,
                           string.IsNullOrEmpty(userName) ? "" : (" and UserName like '%" + userName + "%'")
                           )).Tables[0];

                    int count = Convert.ToInt32(source.Rows[0][0].ToString());

                    pageCount = Convert.ToInt32(count / pageSize) + (count % pageSize > 0 ? 1 : 0);

                    this.Invoke(new Action(() =>
                    {
                        lbUserCount.Text = string.Format("共{0}行数据,一页{1}行", count, pageSize);
                        lbPageCount.Text = string.Format("共{0}页", pageCount);
                    }));

                    CurrentPage = 1;
                    this.Invoke(new Action(() =>
                    {
                        QueryCurrentData();

                    }));
                }
                catch { }
            }));
            th.Start();
        }

        private void btnUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CurrentPage--;
            if (CurrentPage <= 0)
                CurrentPage = 1;
            QueryCurrentData();
        }

        private void btnDown_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CurrentPage++;
            if (CurrentPage > pageCount)
                CurrentPage = pageCount;
            QueryCurrentData();
        }

        private void QueryCurrentData()
        {
            string start = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string end = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            string userName = tbUserName.Text;
            var currentBusiness = lbBusiness.SelectedItem as T_Business;
            Thread th = new Thread(new ThreadStart(() =>
            {
                try
                {
                    string sql = string.Format("select * from t_newaccount where businessid='{0}' and buyTime>='{2} 00:00:01' and buyTime<='{3} 23:59:59' {4} order by createtime desc limit {1},20",
                        currentBusiness.Id, (CurrentPage - 1) * pageSize,
                       start,
                      end,
                          string.IsNullOrEmpty(userName) ? string.Empty : (" and UserName like '%" + userName + "%'"));

                    var source = DataTransaction.Create().Query(sql).Tables[0];
                    source.Columns.Add("stateName");
                    foreach (DataRow row in source.Rows)
                    {

                        if (row["state"].ToString().Equals("3"))
                        {
                            row["stateName"] = "已出";
                        }

                        if (row["state"].ToString().Equals("6"))
                        {
                            row["stateName"] = "已出且验证";
                        }
                        if (row["state"].ToString().Equals("12"))
                        {
                            row["stateName"] = "优质资源已出（手机）";
                        }
                    }
                    this.Invoke(new Action(() =>
                    {
                        dataGridView1.DataSource = source;
                    }));
                }
                catch { }
            }));

            th.Start();
        }

        private void QueryBuyInfoClick(object sender, EventArgs e)
        {
            LoadCount();
        }
        #endregion

        private void 锁定软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LockPage.Current == null)
            {
                MessageBox.Show("未登录");
                return;
            }
            LockPage.Current.Visible = true;
            this.Visible = false;
        }

        private void 数据生成工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //将模拟数据添加到数据库 并且修改状态为5
            BulidUserPage page = new BulidUserPage();
            page.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void Query()
        {
            var currentBusiness = lbBusiness.SelectedItem as T_Business;

            if (currentBusiness == null)
            {
                return;
            }

            dataGridView2.AutoGenerateColumns = false;
            Thread th = new Thread(new ThreadStart(() =>
            {
                try
                {
                    string sql = "select date_format(buytime,'%Y-%c-%d ')  date ,count(1) count from t_hisnewaccount where businessid='" + currentBusiness.Id + "' group by date_format(buytime,'%Y-%c-%d ')  order by buyTime";
                    var source = DataTransaction.Create().DoGetDataTable(sql);
                    this.Invoke(new Action(() =>
                    {
                        dataGridView2.DataSource = source;

                    }));
                }
                catch {
                    MessageBox.Show("查询商户统计数据失败");
                }
            }));
            th.Start();


        }

        #region 生成某天的Excel数据
        private void BulidExcelClick(object sender, EventArgs e)
        {
            var currentBusiness = lbBusiness.SelectedItem as T_Business;

            Thread th = new Thread(new ThreadStart(() =>
            {
                if (currentBusiness == null)
                {

                    return;
                }
                string sql = string.Format(@" select  t_hisnewaccount.*,t_useremail.password emailPassword from t_hisnewaccount left join t_useremail on
                                           t_hisnewaccount.email=t_useremail.email   
 where businessid='{0}' and buyTime>='{1} 00:00:01' and buyTime<='{1} 23:59:59'    "
                    , currentBusiness.Id, dpExportTime.Value.ToString("yyyy-MM-dd"));
                var excelTable = DataTransaction.Create().DoGetDataTable(sql);
                if (excelTable.Rows.Count <= 0)
                {
                    MessageBox.Show("没有数据");
                    return;
                }
                this.Invoke(new Action(() =>
                {
                    btnExport.Text = "正在导出...";
                }));
                var sheet = CreateExcel();
                int index = 0;
                List<T_NewAccountEntity> accountCollection = new List<T_NewAccountEntity>();
                foreach (DataRow row in excelTable.Rows)
                { 
                    accountCollection.Add(new T_NewAccountEntity
                    {
                        UserGuid = row["UserGuid"] + string.Empty,
                        UserName = row["UserName"] + string.Empty,
                        PassWord = CXDataCipher.DecipheringUserPW(  row["PassWord"] + string.Empty),
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
                    this.Invoke(new Action(() =>
                    {
                        btnExport.Text = "已导出" + index + ",共" + excelTable.Rows.Count + "...";
                    }));
                   
                    index++;
                }
                foreach (var user in accountCollection)
                {
                    WriteExcelRow(user, accountCollection.IndexOf(user), sheet);
                }

                string path = System.Environment.CurrentDirectory + @"\Data\"+currentBusiness.Name+@"\" + DateTime.Now.ToString("yyyy-MM-dd") + "-" + currentBusiness.Name + ".xls";
                var ss = File.Create(path);
                sheet.Write(ss);
                ss.Close();
                ss.Dispose();
                this.Invoke(new Action(() =>
                {
                    btnExport.Text = "重新导出Excel";
                }));

                MessageBox.Show("导出成功");
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

        private void 财务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FinancePage page = new FinancePage();
            page.Show();
        }

        private void ToolMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            LockPage.Current.Visible = true;
            this.Visible = false;
            e.Cancel = true;
        }

        private void 已注册身份证导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PassengerManagerPage page = new PassengerManagerPage();
            //page.Show();
        }

        private void toolStripStatusLabel11_Click(object sender, EventArgs e)
        {
            ResourceStatisticsPage page = new ResourceStatisticsPage();
            page.Show();
        }
        private void 数据资源验证ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VailMainPage page = new VailMainPage();
            page.Show();
        }

        private void 账号登陆验证ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginValidPage page = new LoginValidPage();
            page.Show();
        }

        #region 导出TXT
        private void button3_Click(object sender, EventArgs e)
        {
            var currentBusiness = lbBusiness.SelectedItem as T_Business;
            var businessName = currentBusiness.Name;

            Thread th = new Thread(new ThreadStart(() =>
            {
                if (currentBusiness == null)
                {
                    return;
                }
                string sql = string.Format(@" select  t_hisnewaccount.*,T_email.password emailPassword from t_hisnewaccount left join t_email on
                                           t_hisnewaccount.email=t_email.email  where businessid='{0}' and buyTime>='{1} 00:00:01' and buyTime<='{1} 23:59:59'   "
                    , currentBusiness.Id, dpExportTime.Value.ToString("yyyy-MM-dd"));

                var excelTable = DataTransaction.Create().DoGetDataTable(sql);

                if (excelTable.Rows.Count <= 0)
                {
                    MessageBox.Show("没有数据");
                    return;
                }
                this.Invoke(new Action(() =>
                {
                    btnExportTxt.Text = "正在导出...";
                }));
                List<T_NewAccountEntity> accountCollection = new List<T_NewAccountEntity>();
                foreach (DataRow row in excelTable.Rows)
                {
                    accountCollection.Add(new T_NewAccountEntity
                    {
                        UserGuid = row["userguid"] + string.Empty,
                        UserName = row["username"] + string.Empty,
                        PassWord = CXDataCipher.DecipheringUserPW(row["PassWord"] + string.Empty),
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

                foreach (var user in accountCollection)
                {
                    try
                    {
                        Storage(user, accountCollection.Count, businessName);
                    }
                    catch
                    {

                    }
                }
                string path = System.Environment.CurrentDirectory + @"\Data\";
                //打开文件夹
                Process.Start("explorer.exe", path);

                this.Invoke(new Action(() =>
                {
                    btnExportTxt.Text = "重新导出Excel";
                }));

                MessageBox.Show("导出成功");
            }));
            th.Start();

        }

        private void Storage(T_NewAccountEntity item, int number, string name)
        {
            WriteMessage(item.UserName + "写入文件");

            try
            {
                string path = System.Environment.CurrentDirectory + @"\Data\重新导出(" + name + "" + DateTime.Now.ToString("yyyy-MM-dd HH mm") + "-" + number + "行)" + ".txt";

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
        #endregion

        private void esToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountActivation ss = new AccountActivation();
            ss.Activation(new Account12306Item { UserName = "QYL1984011421", PassWord = "QYL613980" });
        }

        private void 错误导出资源返库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
              openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    File.ReadAllLines(openFileDialog1.FileName).ToList().ForEach(line =>
                    {

                        var lines = line.Split(',');
                        Console.WriteLine("开始修改" + lines[1] + "");
                        UpdateNumber(new T_NewAccountEntity
                        {
                            LastTime = DateTime.Now,
                            UserName=lines[1],
                            UserGuid = lines[0],
                            businessId = string.Empty,
                            State = 2
                        });
                        Console.WriteLine("修改" + lines[1] + "完毕");

                    });
                }));
                th.Start();
            }
        }
        private void UpdateNumber(T_NewAccountEntity user)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
          
            if (user.State.Equals(10))
            {
                strSql.Append("update t_newaccount set  isActive=1,accountType=0, state=@State,LastTime=@LastTime,businessId=@businessId where userGuid=@userGuid");
            }
            if (user.State.Equals(2))
            {
                strSql.Append("update t_newaccount set  state=@State,LastTime=@LastTime,businessId=@businessId where userGuid=@userGuid");
            }
            if (user.State.Equals(12))
            {
                strSql.Append("update t_newaccount set  state=@State,LastTime=@LastTime, businessId=@businessId   where userGuid=@userGuid");
            }
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@LastTime", DbType.DateTime,0),
					new ParameterInfo("@businessId", DbType.String,45) 
                                      };
            parameters[0].Value = user.UserGuid;

            parameters[1].Value = user.State;

            parameters[2].Value = user.LastTime;

            parameters[3].Value = user.businessId;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();

            DataTransaction.Create().ExecuteSql(sql);

            string log = "delete from t_buylog where username='" + user.UserName + "'";

            DataTransaction.Create().ExecuteSql(log);
        }

        private void 优质资源返库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    File.ReadAllLines(openFileDialog1.FileName).ToList().ForEach(line =>
                    { 
                        var lines = line.Split(',');
                        Console.WriteLine("开始修改优质资源" + lines[1] + "");
                        UpdateNumber(new T_NewAccountEntity
                        {
                            LastTime = DateTime.Now,
                            UserGuid = lines[0],
                            UserName = lines[1],
                            businessId = string.Empty,
                            State = 10
                        });
                        Console.WriteLine("修改优质资源" + lines[1] + "完毕");
                    });
                    MessageBox.Show("修改完成");
                }));
                th.Start();
            }
        }



        private void 资源日志查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
             ResourceLogPage page = new ResourceLogPage();
             page.Show();
        }

        private void 导出连号资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportConsecutiveResourcePage data = new ExportConsecutiveResourcePage();
            data.Show();
        }

        private void 账号修改用户信息工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateUserPage page = new UpdateUserPage();
            page.Show();
        }

        private void 发送邮件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendEmailValidPage page = new SendEmailValidPage();
            page.Show();
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var data = DataTransaction.Create();

                    File.ReadAllLines(openFileDialog1.FileName,Encoding.Default).ToList().ForEach(line =>
                    {

                        T_NewAccountEntity user = Newtonsoft.Json.JsonConvert.DeserializeObject<T_NewAccountEntity>(line);

                        Console.WriteLine("开始添加外来资源" + user.UserName + "");

                        if (data.Query("select * from t_newaccount where UserName='" + user.UserName + "'").Tables[0].Rows.Count <= 0)
                        {
                            
                            data.ExecuteSql(CreateNewAccountScript(user));
                            Console.WriteLine("添加外来资源" + user.UserName + "完毕");
                        }
                        else 
                        {
                            Console.WriteLine("外来资源" + user.UserName + "已存在");
                        }
                    });
                    MessageBox.Show("修改完成");
                }));
                th.Start();
            }
        }

        public SqlParamterItem CreateNewAccountScript(T_NewAccountEntity model)
        {
            
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into t_newaccount(");
            strSql.Append("UserGuid,UserName,PassWord,PassengerName,PassengerId,Email,Phone,State,CreateTime,LastTime,PwdQuestion,PwdAnswer,IVR_passwd,businessId,accountType,move)");
            strSql.Append(" values (");
            strSql.Append("@UserGuid,@UserName,@PassWord,@PassengerName,@PassengerId,@Email,@Phone,@State,@CreateTime,@LastTime,@PwdQuestion,@PwdAnswer,@IVR_passwd,@businessId,@accountType,@move)");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@UserName", DbType.String,40),
					new ParameterInfo("@PassWord", DbType.String,39),
					new ParameterInfo("@PassengerName", DbType.String,20),
					new ParameterInfo("@PassengerId", DbType.String,39),
					new ParameterInfo("@Email", DbType.String,45),
					new ParameterInfo("@Phone", DbType.String,20),
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@CreateTime",DbType.DateTime,0),
					new ParameterInfo("@LastTime", DbType.DateTime,0),
					new ParameterInfo("@PwdQuestion", DbType.String,45),
					new ParameterInfo("@PwdAnswer", DbType.String,45),
					new ParameterInfo("@IVR_passwd", DbType.String,45),
					new ParameterInfo("@businessId", DbType.String,45),
                    new ParameterInfo("@accountType", DbType.Int32,11),
                     new ParameterInfo("@move", DbType.Int32,11)};
            parameters[0].Value = model.UserGuid;
            parameters[1].Value = model.UserName;
            parameters[2].Value = model.PassWord;
            parameters[3].Value = model.PassengerName;
            parameters[4].Value = model.PassengerId;
            parameters[5].Value = model.Email;
            parameters[6].Value = model.Phone;
            parameters[7].Value = model.State;
            parameters[8].Value = model.CreateTime;
            parameters[9].Value = model.LastTime;
            parameters[10].Value = model.PwdQuestion;
            parameters[11].Value = model.PwdAnswer;
            parameters[12].Value = model.IVR_passwd;
            parameters[13].Value = model.businessId;

            parameters[14].Value = 0;
            parameters[15].Value = model.Move;
           
            sql.Sql = strSql.ToString();
            sql.ParamterCollection = parameters.ToList();
            return sql;
        }


        private void UpdateAccount(T_NewAccountEntity user)
        {
            var sql = new SqlParamterItem();

            StringBuilder strSql = new StringBuilder();

            string accountType = string.IsNullOrEmpty(user.AccountType) ? "" : (", accountType="+user.AccountType);
            if (!string.IsNullOrEmpty(user.UserGuid))
            {
                strSql.AppendFormat("update t_newaccount set buyTime='{0}', state=@State,LastTime=@LastTime,businessId='{1}' {3} where userGuid='{2}'",
                user.BuyTime.ToString("yyyy-MM-dd HH:mm:ss"), user.businessId, user.UserGuid, accountType);
            }
            else
            {
                strSql.AppendFormat("update t_newaccount set  state=@State,LastTime=@LastTime {1}  where userName='{0}'", user.UserName, accountType);
            }


            ParameterInfo[] parameters =
            { 
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@LastTime", DbType.DateTime,0)  };

            parameters[0].Value = user.State;

            parameters[1].Value = user.LastTime;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();

            var sqls = new List<SqlParamterItem> { };

            sqls.Add(sql);

            if (user.State != 8)
            {
                string message = "";

                if (user.State == 20)
                {
                    message = "资源转变为核验乘车人资源";
                }
                else
                {
                    message = (user.State == 12 ? "手机核验" : "普通") + "新资源转售出";
                }
                string log = "insert into t_buylog(businessId,username,detail,lastTime) values('" + (string.IsNullOrEmpty(user.businessId) ? "0" : user.businessId) + "','" + user.UserName + "','" + message + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                sqls.Add(new SqlParamterItem { Sql = log, ParamterCollection = new List<ParameterInfo> { } });
            }
            DataTransaction.Create().ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls);
        }

        private void 身份证提取工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadPassengerPage page = new ReadPassengerPage();
            page.Show();
        }

        private void 变为检查乘车人资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    File.ReadAllLines(openFileDialog1.FileName).ToList().ForEach(line =>
                    {
                        var lines = line.Split(',');
                        Console.WriteLine("开始修改账号为检查乘车人账号->" + lines[1] + "");
                        UpdateAccount(new T_NewAccountEntity
                        {
                            LastTime = DateTime.Now,
                            UserGuid = string.Empty,
                            UserName = lines[1],
                            State = 20
                        });
                        Console.WriteLine("修改账号为检查乘车人账号完毕->" + lines[1] + "");
                    });
                    MessageBox.Show("修改完成");
                }));
                th.Start();
            }
        }

        private void 删除乘车ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeletePassenger passenger = new DeletePassenger();
            passenger.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CX.Config.SetConfig config = new CX.Config.SetConfig();
            config.ShowDialog();
        }

        private void 提取100个注册使用的账号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定提取", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {

                    var data = DataTransaction.Create();
                    string sql = "select *  from t_newaccount where state=2  limit 0,100";

                    var source = data.Query(sql).Tables[0];

                    foreach (DataRow row in source.Rows) 
                    {
                        Console.WriteLine("开始修改账号为检查乘车人账号->" + row["UserName"] + "");

                        UpdateAccount(new T_NewAccountEntity
                        {
                            LastTime = DateTime.Now,
                            UserGuid = row["UserGuid"] + string.Empty,
                            UserName = row["UserName"] + string.Empty,
                            State = 20,
                            AccountType = "0"
                        });
                        Console.WriteLine("修改账号为检查乘车人账号完毕->" + row["UserName"] + "");
                    }


                }));
                th.Start();
            }
        }

        private void 未核验外来资源转损坏资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定提取", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {

                    var data = DataTransaction.Create();
                    while (true)
                    {
                        string sql = "select *  from t_newaccount where state=5 and  passengername ='外来资源' limit 0,100";

                        var source = data.Query(sql).Tables[0];

                        if (source.Rows.Count <= 0)
                        {
                            break;
                        }
                        foreach (DataRow row in source.Rows)
                        {
                            Console.WriteLine("开始变为损坏账号->" + row["UserName"] + "");

                            UpdateAccount(new T_NewAccountEntity
                            {
                                LastTime = DateTime.Now,
                                UserGuid = row["UserGuid"] + string.Empty,
                                UserName = row["UserName"] + string.Empty,
                                State = 8
                            });
                            Console.WriteLine(row["UserName"] + "成功转变为损坏账号");
                        }
                    }
                }));
                th.Start();
            }
        }

        private void 提取100个资源作为客户王明宇测试数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定提取", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var data = DataTransaction.Create();

                    string sql = "select *  from t_newaccount where state=2 and  accounttype=0 limit 0,50";

                    var source = data.Query(sql).Tables[0];
                 
                    foreach (DataRow row in source.Rows)
                    {
                        Console.WriteLine("开始变为王明宇测试账号->" + row["UserName"] + "");

                        data.ExecuteSql("update t_newaccount set accountType=4,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where UserGuid='" + row["UserGuid"] + "'");

                        Console.WriteLine(row["UserName"] + "成功转变为王明宇测试账号");
                    }
                }));
                th.Start();
            }
        }

        private void 变售出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    File.ReadAllLines(openFileDialog1.FileName).ToList().ForEach(line =>
                    {

                        var lines = line.Split(',');
                        Console.WriteLine("开始修改优质资源" + lines[1] + "");
                        UpdateNumber(new T_NewAccountEntity
                        {
                            LastTime = DateTime.Now,
                            UserGuid = lines[0],
                            UserName = lines[1],
                            businessId = "124",
                            State = 12
                        });
                        Console.WriteLine("修改优质资源" + lines[1] + "完毕");
                    });
                    MessageBox.Show("修改完成");
                }));
                th.Start();
            }
        }

        private void 密码解密ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PWJMPage page = new PWJMPage();
            page.Show();
        }

        private void 删除文件中的数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var data = PD.Business.DataTransaction.Create();
                Thread th = new Thread(new ThreadStart(() =>
                {
                    File.ReadAllLines(openFileDialog1.FileName).ToList().ForEach(line =>
                    {
                        var lines = line.Split(',');
                        Console.WriteLine("开始删除资源" + lines[1] + "");
                        data.ExecuteSql("delete from t_newaccount where username='" + lines[1]+ "'");
                        Console.WriteLine("删除资源" + lines[1] + "完毕");
                    });
                    MessageBox.Show("删除完成");
                }));
                th.Start();
            }
        }

        private void 福利管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FreePage page = new FreePage();
            page.Show();
        }

        private void 用户使用情况抽查ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserValidPage page = new UserValidPage();
            page.Show();
        }

        private void 提取身份证ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportPassenger p = new ExportPassenger();
            p.Show();
        }

        private void 删除身份证ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeletePassengerInPC p = new DeletePassengerInPC();
            p.Show();
        }

        private void 加密密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EncryptionPWPage p = new EncryptionPWPage();
            p.Show();
        }

        private void 转新资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertNewAccountPage p = new ConvertNewAccountPage();
            p.Show();
        }

        private void 分析身份证12306操作ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalysisPassengerInPC pc = new AnalysisPassengerInPC();
            pc.Show();
        }
    }
}
