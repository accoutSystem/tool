using IdentificationTool.Properties;
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

namespace IdentificationTool
{
    public partial class IdentificationPage : Form
    {
        private int excuteCount;

        public int ExcuteCount
        {
            get { return excuteCount; }
            set
            {
                excuteCount = value;
                this.Invoke(new Action(() =>
                {
                    lbcount.Text = value.ToString();
                }));
            }
        }
        private int successCount;

        public int SuccessCount
        {
            get { return successCount; }
            set
            {
                successCount = value; this.Invoke(new Action(() =>
                {
                    lbAddCount.Text = value.ToString();
                }));
            }
        }
        private int errorCount;

        public int RepateCount
        {
            get { return errorCount; }
            set
            {
                errorCount = value; this.Invoke(new Action(() =>
                {
                    lbBadCount.Text = value.ToString();
                }));
            }
        }
        public IdentificationPage()
        {
            InitializeComponent();
            Load += IdentificationPage_Load;
        }

        void IdentificationPage_Load(object sender, EventArgs e)
        {
            Ref();
        }

        void Ref()
        {
            Thread th = new Thread(new ThreadStart(() =>
            {
                try
                {
                    var data = DataTransaction.Create();

                    var source = data.DoGetDataTable(@"  select sum(case state when 0 then 1 else 0 end) noUser,
sum(case state when 1 then 1 else 0 end) user,
sum(case state when 3 then 1 else 0 end) addPa,
sum(case state when 2 then 1 else 0 end) usering from t_passenger   ");

                    var count = Convert.ToInt32(data.DoGetDataTable("select count(1) from t_userpassenger").Rows[0][0]);

                    Invoke(new Action(() =>
                    {
                        lbSum.Text = "总数:未使用" + source.Rows[0]["noUser"] + " 已使用" + (count + Convert.ToInt32(source.Rows[0]["user"])) + " 正在使用" + source.Rows[0]["usering"] + " 已添加联系人" + source.Rows[0]["addPa"];

                    }));
                }
                catch { }
            }));
            th.Start();

        }

        private void AddGoodPassengerClick(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                this.btnAddID.Enabled = false;

                this.btnAddID.Text = "正在导入...";

                Thread th = new Thread(new ThreadStart(() =>
                {
                    openFileDialog1.FileNames.ToList().ForEach(fileName =>
                    {
                        var goodPassengers = File.ReadAllLines(fileName, Encoding.UTF8).ToList();

                        ExcuteCount = goodPassengers.Count;
                        SuccessCount = 0;
                        RepateCount = 0;
                        var data = DataTransaction.Create();

                        int rowCount = 0;

                        var sqls = new List<string>();

                        goodPassengers.ForEach(passengerItem =>
                        {
                            if (!string.IsNullOrEmpty(passengerItem.Trim()))
                            {
                                try
                                {
                                    var passengers = passengerItem.Split(' ');
                                    if (passengers.Length == 2)
                                    {
                                        var passengerName = passengers[0];

                                        var passenerId = passengers[1];

                                        WriteMessage("开始搜索" + passengerName);

                                        string sql = "select count(*) from t_passenger where idNo='" + passenerId + "'";

                                        var count = Convert.ToInt32(data.DoGetDataTable(sql).Rows[0][0]);

                                        if (count <= 0)
                                        {
                                            sql = "select count(*) from t_userpassenger where idNo='" + passenerId + "'";
                                            count = Convert.ToInt32(data.DoGetDataTable(sql).Rows[0][0]);
                                        }

                                        if (count <= 0)
                                        {
                                            rowCount++;

                                            sql = string.Format("insert into t_passenger(passengerId,name,idNo,state) values('{0}','{1}','{2}',0)", Guid.NewGuid().ToString(), passengerName, passenerId);

                                            sqls.Add(sql);

                                            if (rowCount > 100)
                                            {
                                                WriteMessage("开始插入" + rowCount + "行已用数据");

                                                data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());

                                                WriteMessage("成功插入" + rowCount + "行已用数据");

                                                rowCount = 0;

                                                sqls.Clear();
                                                SuccessCount += 100;
                                            }
                                        }
                                        else
                                        {
                                            RepateCount++;
                                            WriteMessage(passengerName + "已存在");
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                        });
                        if (sqls.Count > 0)
                        {
                            data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());
                        }

                        this.Invoke(new Action(() =>
                        {
                            this.btnAddID.Text = "导入身份证";

                            this.btnAddID.Enabled = true;
                        }));
                    });
                }));
                th.IsBackground = true; th.Start();

            }
        }

        private void AddBadPassengerClick(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnAddBadID.Enabled = false;

                this.btnAddBadID.Text = "正在导入...";

                Thread th = new Thread(new ThreadStart(() =>
                {
                    var badPassengers = File.ReadAllLines(openFileDialog1.FileName, Encoding.UTF8).ToList();

                    ExcuteCount = badPassengers.Count;

                    SuccessCount = 0;

                    RepateCount = 0;

                    var data = DataTransaction.Create();

                    var sqls = new List<string>();

                    int rowCount = 0;

                    badPassengers.ForEach(passengerItem =>
                    {
                        try
                        {
                            var passengers = passengerItem.Split(' ');

                            var passengerName = passengers[0];

                            var passenerId = passengers[1];

                            WriteMessage("开始搜索" + passengerName);

                            string sql = "select passengerId from t_passenger where idNo='" + passenerId + "'";

                            var passenger = data.DoGetDataTable(sql);

                            if (passenger.Rows.Count <= 0)
                            {
                                sql = "select idt_userPassenger from t_userpassenger where idNo='" + passenerId + "'";

                                passenger = data.DoGetDataTable(sql);

                                if (passenger.Rows.Count <= 0)
                                {
                                    rowCount++;

                                    sql = string.Format("insert into t_passenger(passengerId,name,idNo,state) values('{0}','{1}','{2}',1)", Guid.NewGuid().ToString(), passengerName, passenerId);

                                    sqls.Add(sql);
                                }
                            }
                            else
                            {
                                rowCount++;

                                sql = string.Format("update t_passenger set state=1 where passengerid='{0}'", passenger.Rows[0]["passengerId"] + string.Empty);
                            }

                            if (rowCount > 100)
                            {
                                SuccessCount += 100;

                                WriteMessage("开始插入" + rowCount + "行已用数据");

                                data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());

                                WriteMessage("成功插入" + rowCount + "行已用数据");

                                rowCount = 0;

                                sqls.Clear();
                            }
                            else
                            {
                                WriteMessage(passengerName + "已存在");
                            }
                        }
                        catch
                        {
                            RepateCount++;
                        }
                    });

                    if (sqls.Count > 0)
                    {
                        data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());
                    }

                    this.Invoke(new Action(() =>
                    {
                        this.btnAddBadID.Text = "导入错误身份证";
                        this.btnAddBadID.Enabled = true;
                    }));
                }));
                th.IsBackground = false; th.Start();

            }
        }

        int messageCount = 0;

        bool isShowMessage = true;
        private void WriteMessage(string message)
        {
            if (isShowMessage)
            {
                this.Invoke(new Action(() =>
                {
                    messageCount++;

                    if (messageCount == 100)
                    {
                        messageCount = 0;
                        textBox1.Text = string.Empty;
                    }

                    textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "->" + message;

                    textBox1.SelectionStart = this.textBox1.Text.Length;

                    textBox1.ScrollToCaret();
                }));
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            if (isShowMessage)
            {
                btnMessage.Image = Resources.start;
                btnMessage.Text = "开启消息";
                isShowMessage = false;
            }
            else
            {
                isShowMessage = true;
                btnMessage.Image = Resources.stop;
                btnMessage.Text = "关闭消息";
            }
        }

        private void lbSum_Click(object sender, EventArgs e)
        {
            Ref();
        }

        bool isMove = false;
        bool isTick = false;
        bool isTickMove = false;
        public static bool IsProvideServer()
        {
            if (DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:00:01")) ||
                  DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 07:01:00")))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (isMove)
            {
                btnConvertPassenger.Text = "开始迁移数据";
                btnConvertPassenger.Image = Resources.start;
                isMove = false;
                return;
            }
            else
            {
                btnConvertPassenger.Text = "停止迁移数据";
                btnConvertPassenger.Image = Resources.stop;
                if (MessageBox.Show("是否启动定时迁移，定时11点后", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
                {
                    isTick = true;
                }
                else
                {
                    isTick = false;
                }
                isMove = true;
            }

            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = DataTransaction.Create();

                int i = 0;

                while (isMove)
                {
                    if (isTick && IsProvideServer() == false)
                    {
                        WriteMessage("已监控定时任务");
                        Thread.Sleep(30000);
                        continue;
                    }
                    try
                    {
                        WriteMessage("开始读取联系人");

                        string sql = "select * from t_passenger where state=1 or state=3 limit 0,100 ";

                        List<string> sqls = new List<string>();

                        DataTable source = data.Query(sql).Tables[0];

                        if (source.Rows.Count <= 0)
                        {
                            Invoke(new Action(() =>
                            {
                                btnConvertPassenger.Text = "开始迁移数据";
                                btnConvertPassenger.Image = Resources.start;
                            }));

                            isMove = false;

                            break;
                        }
                        foreach (DataRow row in source.Rows)
                        {
                            sqls.Add("delete from t_passenger where passengerId='" + row["passengerId"] + "'");

                            var state = (row["state"] + "").Equals("3") ? "3" : "0";

                            var current = data.DoGetDataTable("select idNo from t_userpassenger where idNo='" + row["idNo"] + "'");

                            if (current.Rows.Count <= 0)
                            {
                                sqls.Add("insert into t_userpassenger(name,idNo,state) values('" + row["name"] + "','" + row["idNo"] + "','" + state + "')");
                            }
                        }

                        WriteMessage("开始迁移联系人");

                        data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());

                        WriteMessage("迁移联系人成功");

                        i += 100;

                        Invoke(new Action(() =>
                        {
                            btnConvertPassenger.Text = "停止迁移数据" + i;
                        }));
                    }
                    catch (Exception ex)
                    {
                        WriteMessage("迁移联系人失败" + ex.Message);
                    }
                }

            }));

            th.Start();
        }
        bool isQC = false;
        private void toolStripButton1_Click_2(object sender, EventArgs e)
        {
            if (isQC)
            {
                btnAC.Text = "开始去重";
                btnAC.Image = Resources.start;
                isQC = false;
                return;
            }
            else
            {
                btnAC.Text = "停止去重";
                btnAC.Image = Resources.stop;
                if (MessageBox.Show("是否启动定时迁移，定时11点后", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
                {
                    isTickMove = true;
                }
                else
                {
                    isTickMove = false;
                }
                isQC = true;
            }

            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = DataTransaction.Create();

                int i = 0;

                while (isQC)
                {
                    if (isTickMove && IsProvideServer() == false)
                    {
                        WriteMessage("已监控移动数据定时任务");
                        Thread.Sleep(30000);
                        continue;
                    }
                    try
                    {
                        WriteMessage("开始读取去重联系人");

                        string sql = "select count(*)  a,idno from   t_userpassenger group by idno HAVING count(idno)>1 limit 0,200 ";

                        List<string> sqls = new List<string>();

                        DataTable source = data.Query(sql).Tables[0];

                        if (source.Rows.Count <= 0)
                        {
                            Invoke(new Action(() =>
                            {
                                btnAC.Text = "开始去重";
                                btnAC.Image = Resources.start;
                                isQC = false;
                            }));

                            break;
                        }
                        StringBuilder idNos = new StringBuilder();
                        foreach (DataRow row in source.Rows)
                        {
                            idNos.Append("'" + row["idno"] + "',");

                        }
                        idNos.Remove(idNos.Length - 1, 1);
                        var current = data.DoGetDataTable("select * from t_userpassenger where idno in(" + idNos.ToString() + ")");
                        foreach (DataRow row in source.Rows)
                        {
                            var currentRows = current.Select("idno='" + row["idno"] + "'");

                            if (currentRows.Length > 1)
                            {
                                sqls.Add("delete from t_userpassenger where idt_userPassenger=" + currentRows[0]["idt_userPassenger"]);
                            }
                        }
                        WriteMessage("开始迁移联系人");

                        data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());

                        WriteMessage("迁移联系人成功");

                        i += 200;

                        Invoke(new Action(() =>
                        {
                            btnAC.Text = "停止去重" + i;
                        }));
                    }
                    catch (Exception ex)
                    {
                        WriteMessage("迁移联系人失败" + ex.Message);
                    }
                }

            }));

            th.Start();
            //select count(*)  a,idno from   t_userpassenger group by idno HAVING count(idno)>1 limit 0,1000
        }
        bool isAnysic = false;
        private void toolStripButton1_Click_3(object sender, EventArgs e)
        {
            if (isAnysic)
            {
                btnAnysic.Text = "开始分析身份证";
                btnAnysic.Image = Resources.start;
                isAnysic = false;
                return;
            }
            else
            {
                btnAnysic.Text = "停止分析身份证";
                btnAnysic.Image = Resources.stop;
                isAnysic = true;
            }
            int count = 0;
            Thread th = new Thread(new ThreadStart(() =>
           {
               var data = DataTransaction.Create();
               //0 未使用 3已使用 4 40后身份证 5 50后身份证 6 60后身份证 7 70后身份证  8 80后身份证 9 90后身份证  10 00后
               while (isAnysic)
               {
                   WriteMessage("开始读取联系人");

                   var passengers = data.DoGetDataTable("select idt_userPassenger,idNo from  t_userpassenger where state=0 limit 0,100");
                   if (passengers.Rows.Count <= 0) {
                       Invoke(new Action(() => {
                           btnAnysic.Text = "开始分析身份证";
                           btnAnysic.Image = Resources.start;
                       }));
                     
                       isAnysic = false;
                       continue;
                   }
                   WriteMessage("读取联系人完毕");

                   WriteMessage("开始分析身份证");

                   List<string> sqls = new List<string>();

                   foreach (DataRow row in passengers.Rows)
                   {
                       string no = row["idNo"] + string.Empty;

                       var type = GetType(GetYear(no));

                       if (type != IdNoType.None)
                       {
                           var typeNumber = Convert.ToInt32(type);
                           sqls.Add("update t_userpassenger set state=" + typeNumber + " where idt_userPassenger='" + row["idt_userPassenger"] + "'");
                       }
                   }
                   count += sqls.Count; 
                   Invoke(new Action(() =>
                   {
                       btnAnysic.Text = "停止分析身份证" + count;
                   }));
                   WriteMessage("分析身份证完毕");
                   WriteMessage("开始存储分析完成身份证");
                   data.ExecuteMultiSql(sqls.ToArray());
                   WriteMessage("存储分析完成身份证完毕");
               }
           }));
            th.Start();
        }
        private IdNoType GetType(string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                return IdNoType.None;
            }

            int number = Convert.ToInt32(year);

            if (number >= 1940 && number <= 1949)
            {
                return IdNoType.身份证后40;
            }
            if (number >= 1950 && number <= 1959)
            {
                return IdNoType.身份证后50;
            }
            if (number >= 1960 && number <= 1969)
            {
                return IdNoType.身份证后60;
            }
            if (number >= 1970 && number <= 1979)
            {
                return IdNoType.身份证后70;
            }
            if (number >= 1980 && number <= 1989)
            {
                return IdNoType.身份证后80;
            }
            if (number >= 1990 && number <= 1999)
            {
                return IdNoType.身份证后90;
            }
            if (number >= 2000 && number <= 2010)
            {
                return IdNoType.身份证00后;
            }
            if (number >= 1920 && number <= 1929)
            {
                return IdNoType.身份证后20;
            }
            if (number >= 1930 && number <= 1939)
            {
                return IdNoType.身份证后30;
            } if (number >= 2010 && number <= 2019)
            {
                return IdNoType.身份证后01;
            }

            return IdNoType.None;
        }
        /// <summary>
        /// 获取身份证年份
        /// </summary>
        /// <param name="identityCard"></param>
        /// <returns></returns>
        private string GetYear(string identityCard)
        {
            if (identityCard.Length == 18)
            {
                return identityCard.Substring(6, 4);
            }
            //处理15位的身份证号码从号码中得到生日和性别代码
            if (identityCard.Length == 15)
            {
                return "19" + identityCard.Substring(6, 2);
            }
            return string.Empty;
        }
    }
    public enum IdNoType
    {
        None=0,
        身份证后40 = 4,
        身份证后50 = 5,
        身份证后60 = 6,
        身份证后70 = 7,
        身份证后80 = 8,
        身份证后90 = 9,
        身份证00后 = 10,
        身份证后20 = 11,
        身份证后30 = 12,
        身份证后01 = 13,
    }
}
