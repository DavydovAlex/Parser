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
                Uri e = new Uri(@"https://www.google.com");//@"D:\1.txt"   @"https://www.google.com"   @"D:\chm\1234.htm"
                string str = wrk.GetHtml(e, Encoding.UTF8);
                wrk.RemoveBlock(ref str, HTMLWorker.P_SCRIPT);
                wrk.RemoveBlock(ref str, HTMLWorker.GetPattern_PAIRED_TAG("style"));
                wrk.RemoveBlock(ref str, HTMLWorker.GetPattern_PAIRED_TAG("head"));
                List<string> dev = wrk.Split(str);
                List<Tag> dcv = wrk.GetTagList(str);
                
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            
        }
    }
}
