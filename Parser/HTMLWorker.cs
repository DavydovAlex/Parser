using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;


namespace Classes
{
    class HTMLWorker
    {
        
        #region Constants
        /// <summary>
        /// Set option "RegexOptions.IgnoreCase" when use
        /// </summary>
        public const string P_DOCTYPE = @"<!\s*?[Dd][Oo][Cc][Tt][Yy][Pp][Ee][\s\S]*?>";

        public const string P_COMMENT= @"<!--(.|\n)*?-->";

        public const string P_SCRIPT = @"<(\s)*script.*>(.|\n)*?</(\s)*script(\s)*>";

        public const string P_META = @"(?<=<\s*meta.*?charset\s*=\s*""\s*).*(?=\s*""\s*>)";

        public const string P_SINGLE_TAG = @"<(.|n)*?>(.|n)*?<";

        public const string P_HTML_FROM_TAG_TO_TAG= @"<[\s\S]*?>[\s\S]*?(?=<[\s\S]*?>)";

        public const string P_TAG_NAME = @"(?<=<[\s/]*?)[a-zA-Z0-9]*?(?=(>| [\s\S]*?>))";

        public const string P_SINGLE_CLOSING_TAG = @"(?<=<[\s]*?)[a-zA-Z0-9]*?(?=(/>| [\s\S]*?/>))";

        public const string P_OPEN_TAG_NAME= @"(?<=<[\s]*?)[a-zA-Z0-9]*?(?=(>| [\s\S]*?>))";

        public const string P_CLOSING_TAG_NAME = @"(?<=<[\s]*?)/[a-zA-Z0-9]*?(?=(>|[\s]*?>))";

        //public static string P_VALID_ATTRIBUTE_CHECKING = @"<>";

        #endregion


        #region Constructors
        public HTMLWorker()
        {

        }
    #endregion

        /// <summary>
        /// Read file 
        /// </summary>
        /// <param name="fileName"> Path to Html file</param>
        /// <param name="encoding"> Encoding </param>
        /// <returns></returns>
        public string ReadFile(string path,Encoding encoding)
        {
            string result;
            using (StreamReader stream = new StreamReader(path, encoding))
            {
                result = stream.ReadToEnd();
            }
            return result;                    
        }
        /// <summary>
        /// Get Html frol local file or via Net
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string GetHtml(string URI, Encoding encoding)
        {
            Uri HtmlAddr = new Uri(URI);
            string result;
            if(HtmlAddr.Scheme==Uri.UriSchemeFile)
            {
                result = ReadFile(URI, encoding);
            }
            else
            {
                result = GetHtmlViaInternet(URI, encoding);
            }
            return result;
        }
        public string GetHtml(Uri URI, Encoding encoding)
        {
            string result;
            if (URI.Scheme == Uri.UriSchemeFile)
            {
                result = ReadFile(URI.AbsolutePath, encoding);
            }
            else
            {
                result = GetHtmlViaInternet(URI, encoding);
            }
            return result;
        }
        /// <summary>
        /// Get string containing Html code loaded from site
        /// </summary>
        /// <param name="URL">Source Html code URI</param>
        /// <param name="encoding"> Encoding </param>
        /// <returns></returns>
        public string GetHtmlViaInternet(string URI,Encoding encoding)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
            request.Referer = "https://www.google.ru/";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result;
            using (StreamReader stream = new StreamReader(response.GetResponseStream(), encoding))
            {
                result = stream.ReadToEnd();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string GetHtmlViaInternet(Uri URI, Encoding encoding)
        {
            string strURI = URI.AbsoluteUri;
            string result = GetHtmlViaInternet(strURI, encoding);
            return result;
        }

        public void ConvertHtmlToXml(string Html)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                 xml.LoadXml(Html);
            }
            catch(Exception e)
            {
                string err = e.Message;
            }
            
        }


        public static Match DOCKTYPE(string html)
        {
            string pattern = P_DOCTYPE;
            Regex reg = new Regex(pattern);
            Match result = reg.Match(html);
            return result;
        }
        public static void ChangeCase(ref string html,string pattern,bool Case)
        {
            //Regex reg = new Regex(pattern);
            //Match result = reg.Match(html);
           
            html=html.Replace("doctype", "DOCTYPE");

        }
        /// <summary>
        ///Rewrite or remove this function
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MatchCollection SeparateByTags(string html)
        {
            MatchCollection commnets = RemoveElement(ref html,P_COMMENT);
            MatchCollection scripts = RemoveElement(ref html, GetPattern_PAIRED_TAG("script"));
            MatchCollection styles = RemoveElement(ref html, GetPattern_PAIRED_TAG("style"));
            string pattern = @"<(.|\n)*?>(.)";
            Regex regex = new Regex(pattern);
            MatchCollection result = regex.Matches(html);
            return result;
        }
        public static MatchCollection RemoveElement(ref string changedHtml,string pattern)
        {
            MatchCollection result = GetElements(changedHtml, pattern);
            changedHtml = Regex.Replace(changedHtml, pattern,"");

            return result;
        }

        static MatchCollection GetElements(string html,string pattern,int startpos=0)
        {
            Regex regex = new Regex(pattern);

            MatchCollection result = regex.Matches(html, startpos);
            return result;
        }

        public static Match GetElement(string html, string pattern,int startpos=0)
        {
            Regex regex = new Regex(pattern,RegexOptions.Singleline);
            Match result = regex.Match(html,startpos);
            return result;
        }


        public static Encoding GetEncoding(string html)
        {
            string head = GetElement(html, GetPattern_PAIRED_TAG("head")).Value;/*Знаем что Head только один берем первый элемент и сразу забираем значение*/
            Match encoding=GetElement(head, P_META);
            Encoding result;
            if (encoding.Length == 0)
                result = Encoding.UTF8;
            else
                result= Encoding.GetEncoding(encoding.Value);
            return result;

        }
        public static string GetPattern_PAIRED_TAG(string tagName)
        {
            string pattern = @"<\s*?" + tagName + @"[^>]*?>[\s\S]*?</\s*?" + tagName + @"\s*?>"; //@"<(\s)*" + tagName + @".*>(.|\n)*?</(\s)*" + tagName + @"(\s)*>";
            return pattern;
        }
        public static List<string> SplitHtml(string html,string pattern)
        {
            List<string> result = new List<string>();
            int currentPosition = 0;
            string buf = "";
            while (currentPosition<=html.Length)
            {
                buf = GetElement(html, pattern, currentPosition).Value;
                GroupCollection dfv= GetElement(html, pattern, currentPosition).Groups;

                if (buf == "")
                    break;
                currentPosition += buf.Length;
                //string show = html.Substring(currentPosition, 20);
                result.Add(buf);
                
            }

            return result;
        }

        /// <summary>
        /// Переписать без использования splitedHtml (подумать) т.к. этот список создается не в одной функции(наверно замедлит работу)
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<Tag> FindSingleTags(string html)
        {
            List<string> splitedHtml = SplitHtml(html, P_HTML_FROM_TAG_TO_TAG);
            Stack<Tag> stack = new Stack<Tag>();         
            List<Tag> tags = new List<Tag>();
            Tag tag = new Tag();
            int pos = 0;
            foreach (string str in splitedHtml)
            {
                tag.Name = GetElement(str, P_TAG_NAME).Value;
                tag.Position = pos;

                if (Regex.IsMatch(str, P_SINGLE_CLOSING_TAG))
                {
                    tag.Status = 2;
                    pos++;
                    continue;//если тег имеет формат <tag /> нет необходимости выполнять проверки
                }
                else if (Regex.IsMatch(str, P_OPEN_TAG_NAME))                
                    tag.Status = 1;              
                else if(Regex.IsMatch(str, P_CLOSING_TAG_NAME))
                    tag.Status = 0;

                if (stack.Count != 0)
                {
                    //Удалить элемент из верхушки если пришедший тег имеет такое же имя, но является закрывающим
                    if (stack.Peek().Name == tag.Name && stack.Peek().Status==1 && tag.Status==0)
                    {
                        stack.Pop();
                    }
                    //Если пришедший тег закрывающий и имеет другое имя, значит тег в стеке одиночный               
                    else if (stack.Peek().Name != tag.Name && tag.Status==0)
                    {
                        //Проверяем в цикле, так как может быть несколько вложенных тегов и они не удалятся
                        while (stack.Peek().Name != tag.Name)
                        {
                            tags.Add(stack.Peek());
                            stack.Pop();                        
                        } 
                        if (stack.Peek().Name == tag.Name)
                            stack.Pop();
                        else
                            stack.Push(tag);
                    }
                    else
                        stack.Push(tag);
                }
                else
                    stack.Push(tag);
       
                pos++;
            }
            return tags;
        }

        public static string  ComplementSingleTags(string html)
        {
            List<string> SplitedHtml = SplitHtml(html,P_HTML_FROM_TAG_TO_TAG);
            List<Tag> singleTags = FindSingleTags(html);
            foreach(Tag tg in singleTags)
            {
                SplitedHtml[tg.Position] = SplitedHtml[tg.Position].Replace(">", "/>"); //"</" + tg.Name + ">";
            }
            string result = CompileHtml(SplitedHtml);
            return result;
        }
        public static string CompileHtml(List<string> splitedHtml)
        {
            string result = "";
            foreach(string str in splitedHtml)
            {
                result += str;
            }
            return result;
        }

        public static void CheckAttributes(string html)
        {
            string attributes;
            if (!Regex.IsMatch(html, @"</[^>]>"))
            {
                string subStr = GetElement(html, @"(?<=<\s*?[a-zA-Z0-9]*?\s)[^>]*?(?=>)").Value;
                int initiallyLength = subStr.Length;
                int strLen = 0;
                
                while (subStr!="")
                {
                    subStr = Regex.Match(subStr, @"(?<=\s*?[a-zA-Z]*?\s*?=\s*?""[^""]*?"")[\s\S]*?").Value;
                }

            }


        }

        public static string Match(string input, string pattern, int startPos)
        {
            string substr = input.Substring(startPos);
            string result = Regex.Match(substr, pattern).Value;
            return result;           
        }
    }

    struct Tag
    {
        //string _name;
        public string Name;
        public int Position;
        public int Status;
        

    }

    //public static class ExtRegex
    //{
    //    public static bool IsMatch(this Regex regex,string input,string pattern,int startPos)
    //    {
    //        return false;
    //    }
    //}
}
