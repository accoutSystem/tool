using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
namespace Maticsoft.Model{
	 	//t_businessfinance
		public class T_BusinessFinance
	{
   		     
      	/// <summary>
		/// businessfinance
        /// </summary>		
		private string _businessfinance;
        public string businessfinance
        {
            get{ return _businessfinance; }
            set{ _businessfinance = value; }
        }        
		/// <summary>
		/// business_id
        /// </summary>		
		private string _business_id;
        public string business_id
        {
            get{ return _business_id; }
            set{ _business_id = value; }
        }        
		/// <summary>
		/// amount
        /// </summary>		
		private decimal _amount;
        public decimal amount
        {
            get{ return _amount; }
            set{ _amount = value; }
        }        
		/// <summary>
		/// createTime
        /// </summary>		
		private DateTime _createtime;
        public DateTime createTime
        {
            get{ return _createtime; }
            set{ _createtime = value; }
        }        
		/// <summary>
		/// resourceNumber
        /// </summary>		
		private int _resourcenumber;
        public int resourceNumber
        {
            get{ return _resourcenumber; }
            set{ _resourcenumber = value; }
        }        
		   
	}
}

