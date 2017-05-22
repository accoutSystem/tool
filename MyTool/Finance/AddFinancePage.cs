using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyEntiry;
using Maticsoft.Model;
using PD.Business;

namespace MyTool.Finance
{
    public partial class AddFinancePage : Form
    {
        public AddFinancePage()
        {
            InitializeComponent();
            Load += new EventHandler(AddFinancePage_Load);
        }

        void AddFinancePage_Load(object sender, EventArgs e)
        {
            cmbBusiness.DisplayMember = "Name";
            cmbBusiness.ValueMember = "Id";
            cmbBusiness.DataSource = ToolMain.BusinessCollection;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var time = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            var amount = Convert.ToDecimal(tbAmount.Text);

            var number = Convert.ToInt32(tbNumber.Text);

            var businssId = (cmbBusiness.SelectedItem as T_Business) != null ? (cmbBusiness.SelectedItem as T_Business).Id : string.Empty;

            if (number <= 0)
            {
                MessageBox.Show("请输入数量");
                return;
            }

            if (amount < 0)
            {
                MessageBox.Show("请输入金额");
                return;
            }

            if (string.IsNullOrEmpty(businssId))
            {
                MessageBox.Show("请选择商户");
                return;
            }

            DataTransaction.Create().ExecuteSql(Add(new T_BusinessFinance
            {
                amount = amount,
                business_id = businssId,
                businessfinance = Guid.NewGuid().ToString(),
                createTime = dateTimePicker1.Value,
                resourceNumber = number
            }));

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public SqlParamterItem Add(T_BusinessFinance model)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into t_businessfinance(");
            strSql.Append("businessfinance,business_id,amount,createTime,resourceNumber");
            strSql.Append(") values (");
            strSql.Append("@businessfinance,@business_id,@amount,@createTime,@resourceNumber");
            strSql.Append(") ");

            ParameterInfo[] parameters = {
			            new ParameterInfo("@businessfinance", DbType.String,36) ,            
                        new ParameterInfo("@business_id", DbType.String,45) ,            
                        new ParameterInfo("@amount", DbType.Decimal,10) ,            
                        new ParameterInfo("@createTime", DbType.DateTime,0) ,            
                        new ParameterInfo("@resourceNumber", DbType.Int32,11)             
              
            };

            parameters[0].Value = model.businessfinance;
            parameters[1].Value = model.business_id;
            parameters[2].Value = model.amount;
            parameters[3].Value = model.createTime;
            parameters[4].Value = model.resourceNumber;

            sql.Sql = strSql.ToString();
            sql.ParamterCollection = parameters.ToList();
            return sql;
        }

    }
}
