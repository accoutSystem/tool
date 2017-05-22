using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool.Common
{
    public class BaseForm : Form
    {
        public void BeginLoading() {
            this.Enabled = false;
        }

        public void EndLoading() {
            this.Enabled = true;
        }
    }
}
