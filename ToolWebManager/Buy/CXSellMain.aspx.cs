using Maticsoft.Model;
using MyTool.Common;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToolWebManager.Base;

namespace ToolWebManager.Buy
{
    public partial class CXSellMain : SellBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) 
            {
                LoadUser();
                ReLoadData();
            }
        }

        private void LoadUser() 
        {
            var dara = PD.Business.DataTransaction.Create();
            var source = dara.Query("select * from t_business ").Tables[0];
            DropDownList1.DataValueField = "businessId";
            DropDownList1.DataTextField = "businessName";
            DropDownList1.DataSource = source;
            DropDownList1.DataBind();
        }

        protected void btnexport_Click(object sender, EventArgs e)
        {
            //DownLoad("李超2015-08-09 21 50-12行.csv");
            //return;
            var number = ConvertValue(this.TextBox1.Text);
            if (number <= 0)
            {
                Message("请输入大于0的资源数量");
            }
            else 
            {
                var data = PD.Business.DataTransaction.Create();
                
                string sellSql=@"SELECT * FROM cx_sell.t_businessuser a inner join cx_sell.t_sellprice  b on
a.idt_businessUser=b.businessId
where businessid= "+SellBusiness.Current.SellBusinessId;
                
                var businessInfo = data.Query(sellSql).Tables[0];

                if (businessInfo.Rows.Count <= 0)
                {
                    Message("导出失败:查询用户余额失败");
                    return;
                }
             
                var price = Convert.ToDecimal(CheckBox1.Checked ? businessInfo.Rows[0]["mobileprice"] : businessInfo.Rows[0]["plainprice"]);
             
                var sell = Convert.ToDecimal(businessInfo.Rows[0]["sell"]);
             
                var money = price * number;

                if (money > sell)
                {
                    Message("导出失败:余额不足,导出需要" + money + "块,剩余" + sell);

                    return;
                }
                //Response.Write("开始执行导出");
                Export(number,money);
            }
        }

        private void Export(int number,decimal sellPrice)
        {
            lbDebug.Text = string.Empty;
            var goodResource = CheckBox1.Checked;

            var db = DataTransaction.Create();

            try
            {
                ExcuteData();

                System.Data.DataTable data = null;
                DebugInfo("开始提取数据");

                #region 提取数据
                if (!goodResource)
                {
                    data = db.Query(string.Format(@" select a.*,t_useremail.passWord emailPassword from (select  * from t_newaccount
 where t_newaccount.state=2   and  t_newaccount.accountType=0 {1} limit 0,{0}) a
 left join t_useremail on
               a.email=t_useremail.email", number, CheckBox2.Checked ? "" : "order by createTime")).Tables[0];
                }
                else
                {
                    data = db.Query(string.Format(@"select a.*,t_useremail.passWord emailPassword from (select  t_newaccount.*  from t_newaccount
                                where t_newaccount.state=10 
and   t_newaccount.accountType=0  {1} limit 0,{0}) a
left join t_useremail on  a.email=t_useremail.email", number, CheckBox2.Checked ? "" : "order by createTime")).Tables[0];
                }
                #endregion

                DebugInfo("提取数据完成");

                if (data.Rows.Count < number)
                {
                    Message("资源数据不足" + number + "个");
                    return;
                }

                #region 生成文件
                List<T_NewAccountEntity> accountCollection = new List<T_NewAccountEntity>();

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
                        State = Convert.ToInt32(row["state"] + string.Empty),
                        EmailPassWord = row["emailPassword"] + string.Empty
                    });
                }
                string baseDic = this.Request.PhysicalApplicationPath + @"Buy\DownLoad\";

                var type = CheckBox1.Checked ? "手机核验" : "普通";

                string fileName = DropDownList1.SelectedItem.Text + "-" + type + "-" + DateTime.Now.ToString("yyyy-MM-dd HH mm ss") + "-" + number + "行" + ".xls";
                string txtfileName = DropDownList1.SelectedItem.Text + "-" + type + "-" + DateTime.Now.ToString("yyyy-MM-dd HH mm ss") + "-" + number + "行" + ".txt";
              
                string path = this.Request.PhysicalApplicationPath + @"Buy\DownLoad\" + SellBusiness.Current.SellBusinessId + @"\";

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                List<SqlParamterItem> sqls = new List<SqlParamterItem>();

                var sheet = CreateExcel();

                foreach (var user in accountCollection)
                {
                    user.businessId = SellBusiness.Current.SellBusinessId;// business.Id;

                    user.LastTime = DateTime.Now;

                    if (goodResource)
                    {
                        user.State = 12;//特殊账号已出
                    }
                    else
                    {
                        user.State = 6;//已出且验证
                    }

                    user.BuyTime = DateTime.Now;

                    sqls.Add(UpdateNumber(user));

                    string log = "insert into t_buylog(businessId,username,detail,lastTime) values('" + user.businessId + "','" + user.UserName + "','" + (user.State == 12 ? "手机核验" : "普通") + "新资源转售出','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                    sqls.Add(new SqlParamterItem { Sql = log });

                    WriteExcelRow(user, accountCollection.IndexOf(user), sheet);

                    StorageUser(user, path + txtfileName);
                }
                DebugInfo("开始执行扣费和修改数据状态");
                #region 扣费处理
                //先更改商户费用
                var excutedata = DataTransaction.Create();

                string updateSell = string.Format("update cx_sell.t_businessuser set sell=sell-{0} where idt_businessUser='{1}'",
                    sellPrice, SellBusiness.Current.SellBusinessId);

                sqls.Add(new SqlParamterItem() { Sql = updateSell });

                string addSell = string.Format("insert into cx_sell.t_sell(businessid,createtime,sellNumber,sellType,sellMoney,storageaddress) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                  SellBusiness.Current.SellBusinessId,
                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                  TextBox1.Text,
                  CheckBox1.Checked ? 1 : 2,
                  sellPrice, fileName);
                sqls.Add(new SqlParamterItem() { Sql = addSell });

                excutedata.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls);
                DebugInfo("执行扣费和修改数据状态成功");

                #endregion

                #region 扣费完成生成文件
                DebugInfo("开始创建Excel文件");

                for (int col = 0; col < 11; col++)
                {
                    sheet.GetSheet("资源列表").AutoSizeColumn(col);
                }
                var ss = File.Create((path+fileName));
                sheet.Write(ss);
                ss.Close();
                ss.Dispose();
                DebugInfo("创建Excel文件成功");

                #endregion
              
                #endregion
              
                //Response.Write("扣费完成");
                
                ReLoadData();
                //Response.Write("重新加载数据完成");

                DownLoad(fileName);
                Response.Write("下载完成");

                //打开文件夹
            }
            catch (Exception ex)
            {
                Message("获取数据失败" + ex.Message);
            }
        }

        private void StorageUser(T_NewAccountEntity item, string path)
        {

            try
            {

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(item.UserName + "," + item.PassWord);

                sw.Close();

                sw.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 同步可能复位的数据
        /// </summary>
        private void ExcuteData()
        {
            DebugInfo("开始执行导出容错处理");

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

            DebugInfo("执行导出容错处理成功"+source.Rows.Count+"个被成功处理");
        }

        private void ReLoadData() 
        {

            string sellSql = @"SELECT * FROM cx_sell.t_businessuser a inner join cx_sell.t_sellprice  b on
a.idt_businessUser=b.businessId
where businessid= " + SellBusiness.Current.SellBusinessId;
            var data = DataTransaction.Create();
            var businessInfo = data.Query(sellSql).Tables[0];

            if (businessInfo.Rows.Count > 0)
            {
                var row = businessInfo.Rows[0];
                SellBusiness sell = new SellBusiness
                {
                    BusinessName = row["businessName"] + string.Empty,
                    PassWord = row["passWord"] + string.Empty,
                    SellBusinessId = row["idt_businessUser"] + string.Empty,
                    SellMoney = Convert.ToDecimal(row["sell"] + string.Empty),
                    UserName = row["userName"] + string.Empty,
                };
                sell.PlainPrice = Convert.ToDecimal(row["plainprice"]);

                sell.MobilePrice = Convert.ToDecimal(row["mobileprice"]);
                
                SellBusiness.Current = sell;

                lbUser.Text = SellBusiness.Current.BusinessName;

                lbSellMonry.Text = SellBusiness.Current.SellMoney.ToString();

                lbPlainPrice.Text = SellBusiness.Current.PlainPrice + string.Empty;

                lbMobilePrice.Text = SellBusiness.Current.MobilePrice + string.Empty;

                lbPTUser.Text = Math.Round(SellBusiness.Current.SellMoney / SellBusiness.Current.PlainPrice, 0) + string.Empty;

                lbMobileUser.Text = Math.Round(SellBusiness.Current.SellMoney / SellBusiness.Current.MobilePrice, 0) + string.Empty;
            }
        }

        private void Storage(T_NewAccountEntity item, string path)
        {
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
            }
            catch (Exception ex)
            {
                Response.Write("写入文件失败" + ex.Message);
            }
        }

        private SqlParamterItem UpdateNumber(T_NewAccountEntity user)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update t_newaccount set buyTime=@buyTime, state=@State,LastTime=@LastTime,businessId=@businessId where userGuid=@userGuid");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@LastTime", DbType.DateTime,0),
					new ParameterInfo("@businessId", DbType.String,45),
					new ParameterInfo("@buyTime", DbType.DateTime,0),  };

            parameters[0].Value = user.UserGuid;

            parameters[1].Value = user.State;

            parameters[2].Value = user.LastTime;

            parameters[3].Value = user.businessId;

            parameters[4].Value = user.BuyTime;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();

            return sql;
        }

        public void DownLoad(string fileName)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "sr", "window.open('DownLoad.aspx?name="+fileName+"');", true);
        }

        private int ConvertValue(string value)
        {
            int outValue = 0;
            Int32.TryParse(value, out outValue);
            return outValue;
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
            item.PassWord = CXDataCipher.DecipheringUserPW(item.PassWord);
            ICellStyle contentStyleLeft = book.CreateCellStyle();
            contentStyleLeft.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            contentStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            contentStyleLeft.WrapText = false;
            
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

        public void Message(string message) {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "info", "alert('" + message + "');", true);
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            SellBusiness.Current = null;
            Response.Redirect("CXSellLogin.aspx");
        }

        private void DebugInfo(string message) {

            lbDebug.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message+"</br>";
        }
    }
}