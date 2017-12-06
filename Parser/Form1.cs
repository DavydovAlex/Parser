using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Classes;




namespace Parser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            HTMLWorker wrk = new HTMLWorker();
            try
            {
                Uri e = new Uri(@"https://www.google.com");//@"D:\1.txt"
                string str = wrk.GetHtml(@"D:\chm\1234.htm", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            
        }
    }
}
