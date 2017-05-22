using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEntiry
{
    public class T_Passenger
    {
        public string PassengerId { get; set; }
        public string Name { get; set; }

        public string IdNo { get; set; }

        public int State { get; set; }

        private string move = string.Empty;

        public string Move
        {
            get
            {
                if (string.IsNullOrEmpty(move))
                {
                    return "0";
                }
                return move;
            }
            set { move = value; }
        }
    }
    public class T_Email
    {
        public string EmailId { get; set; }
        public string Email { get; set; }
        public int State { get; set; }
        public string PassWord { get; set; }
    }
}
