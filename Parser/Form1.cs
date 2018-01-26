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
using System.Text.RegularExpressions;



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
               // Uri e = new Uri(@"rt[FULLNAME]=1");//@"D:\1.txt"   @""   @"D:\chm\1234.htm" D:\Prikaz.html

                //string str = HTMLWorker.GetHtml(e, Encoding.UTF8);
                //HTMLWorker.SaveFile(@"D:\1.html", str);
                string str = HTMLWorker.GetHtml(@"D:\1.html", Encoding.UTF8);
                wrk.RemoveBlock(ref str, HTMLWorker.P_SCRIPT);
                wrk.RemoveBlock(ref str, HTMLWorker.GetPattern_PAIRED_TAG("style"));
                wrk.RemoveBlock(ref str, HTMLWorker.GetPattern_PAIRED_TAG("head"));
                wrk.RemoveBlock(ref str, HTMLWorker.P_COMMENT);

                
                List<Tag> tags = wrk.Split(str);
                foreach (Tag row in tags)
                {
                    string tagToTag = row.Value;
                    
                    Dictionary<string, string> ssss = wrk.GetAttributes(tagToTag);

                }

                //string table = wrk.GetMatches(str, HTMLWorker.GetPattern_PAIRED_TAG("table"))[0].Value;
                //List<string> dev = wrk.Split(str);
                //List<string> classes = new List<string>();

                ////foreach(string row in dev)
                ////{
                ////    string buf = wrk.GetClass(row);
                ////    classes.Add(buf);

                ////}

                //List<string> atttr = new List<string>();
                //foreach (string row in dev)
                //{
                //    string buf = wrk.LeadAttributesToXML(row);
                //    atttr.Add(buf);

                //}

                //List<string> id = new List<string>();
                ////foreach (string row in dev)
                ////{
                ////    string buf = wrk.GetId(row);
                ////    id.Add(buf);
                ////}
                ////List<string> dewf = wrk.RemoveAtributes(dev);
                //List<Classes.Tag> rfrf = wrk.FindSingleTags(str);
                ////List<Tag> dcv = wrk.GetTagList(str);
                //// string res = wrk.ComplementSingleTags(table);

            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            
        }
    }
}
