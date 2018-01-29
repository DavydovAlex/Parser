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
        string Html;
        public Uri URI;
        Encoding encoding;
        List<string> SavingArrtibutes;
        


        #region Constants
        /// <summary>
        /// Set option "RegexOptions.IgnoreCase" when use
        /// </summary>
        public const string P_DOCTYPE = @"<!\s*?[Dd][Oo][Cc][Tt][Yy][Pp][Ee][\s\S]*?>";

        public const string P_COMMENT= @"<!--(.|\n)*?-->";

        public const string P_SCRIPT = @"<[\s]*?script[^>]*?>[\s\S]*?</[\s]*?script[\s]*?>";//@"<[\s]*?script[^>]*?>[^<]*?</[\s]*?script[\s]*?>";  @"<(/|[\s]*?)script[^>]*?>"

        public const string P_META = @"(?<=<\s*meta.*?charset\s*=\s*""\s*).*(?=\s*""\s*>)";

        public const string P_SINGLE_TAG = @"<(.|n)*?>(.|n)*?<";

        /// <summary>
        /// Part of Html from tag to next open/close tag
        /// </summary>
        public const string P_HTML_FROM_TAG_TO_TAG= @"<[\s\S]*?>[\s\S]*?(?=<[\s\S]*?>)";


        public const string P_TAG_NAME = @"(?<=<[\s/!]*?)[a-zA-Z0-9]*?(?=(>| [\s\S]*?>))";

        /// <summary>
        /// Get tag with format &lt;tag /&gt;
        /// </summary>
        public const string P_SINGLE_CLOSING_TAG = @"(?<=<[\s]*?)[a-zA-Z0-9]*?(?=(/>| [\s\S]*?/>))";

        /// <summary>
        /// Get tag with format &lt;tag .....&gt;
        /// </summary>
        public const string P_OPEN_TAG_NAME= @"(?<=<[\s]*?)[a-zA-Z0-9]*?(?=(>| [\s\S]*?>))";

        /// <summary>
        /// Get tag with format &lt;/tag&gt;
        /// </summary>
        public const string P_CLOSING_TAG_NAME = @"(?<=<[\s]*?)/[a-zA-Z0-9]*?(?=(>|[\s]*?>))";

        public const string P_VALUE = @"(?<=<[\s\S]*?>)[\s\S]*";

        #endregion     

        #region Constructors

        public HTMLWorker()
        {
        }

        public HTMLWorker(string URI)
        {
            Html = GetHtml(URI, encoding);
            this.URI = new Uri(URI);
        }

        public HTMLWorker(Uri URI)
        {
            Html = GetHtml(URI, encoding);
            this.URI = URI;
        }
    
        public HTMLWorker(string URI, Encoding encoding):this(URI)
        {
            this.encoding = encoding;        
        }

        public HTMLWorker(Uri URI, Encoding encoding) : this(URI)
        {
            this.encoding = encoding;
        }


        #endregion

        #region Completed  functions

        /// <summary>
        /// Read local file 
        /// </summary>
        /// <param name="fileName"> Path to Html file</param>
        /// <param name="encoding"> Encoding </param>
        /// <returns></returns>
        private static string ReadFile(string path,Encoding encoding)
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
        public static string GetHtml(string URI, Encoding encoding)
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


        /// <summary>
        /// Get Html frol local file or via Net
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetHtml(Uri URI, Encoding encoding)
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
        /// <param name="URL">URI</param>
        /// <param name="encoding"> Encoding </param>
        /// <returns></returns>
        private static string GetHtmlViaInternet(string URI,Encoding encoding)
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
        /// Get string containing Html code loaded from site
        /// </summary>
        /// <param name="URL">URI</param>
        /// <param name="encoding"> Encoding </param>
        /// <returns></returns>
        private static string GetHtmlViaInternet(Uri URI, Encoding encoding)
        {
            string result = GetHtmlViaInternet(URI.AbsoluteUri, encoding);
            return result;
        }

        public static void SaveFile(string path,string html)
        {
            using (StreamWriter stream=new StreamWriter(path))
            {
                Encoding ee=stream.Encoding;
                stream.WriteLine(html);
            }
        }
        #endregion

        #region Unused functions

        public static string Match(string input, string pattern, int startPos)
        {
            string substr = input.Substring(startPos);
            string result = Regex.Match(substr, pattern).Value;
            return result;
        }

        public List<string> RemoveAtributes(List<string> splitedHtml)
        {
            List<string> result = new List<string>();
            string pattern = @"(?<=<[\s]*?[a-zA-Z0-9]*?\s)[^>]*?(?=(>|\>))";
            foreach (string elem in splitedHtml)
            {
                string buf = Regex.Replace(elem, pattern, "");
                result.Add(buf);
            }
            return result;
        }

        //public List<Tag> GetTagList(string html)
        //{
        //    List<string> splitedHtml = Split(html);
        //    List<Tag> splitedToTags = new List<Tag>();
        //    int tagPos = 0;
        //    Tag bufTag = new Tag();
        //    foreach (string elem in splitedHtml)
        //    {
        //        bufTag = FillTagAttributes(elem);
        //        bufTag.Position = tagPos;
        //        splitedToTags.Add(bufTag);
        //        tagPos++;
        //    }
        //    return splitedToTags;
        //}

        /// <summary>
        /// Rewrite
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public Encoding GetEncoding(string html)
        {
            string head = GetMatch(html, GetPattern_PAIRED_TAG("head"));/*Знаем что Head только один берем первый элемент и сразу забираем значение*/
            string encoding = GetMatch(head, P_META);
            Encoding result;
            if (encoding.Length == 0)
                result = Encoding.UTF8;
            else
                result = Encoding.GetEncoding(encoding);
            return result;

        }

        public void CheckAttributes(string html)
        {
            string attributes;
            if (!Regex.IsMatch(html, @"</[^>]>"))
            {
                string subStr = GetMatch(html, @"(?<=<\s*?[a-zA-Z0-9]*?\s)[^>]*?(?=>)");
                int initiallyLength = subStr.Length;
                int strLen = 0;

                while (subStr != "")
                {
                    subStr = Regex.Match(subStr, @"(?<=\s*?[a-zA-Z]*?\s*?=\s*?""[^""]*?"")[\s\S]*?").Value;
                }

            }
        }


        public static string GetPattern_PAIRED_TAG(string tagName)
        {
            string pattern = @"<\s*?" + tagName + @"[^>]*?>[\s\S]*?</\s*?" + tagName + @"\s*?>";
            return pattern;
        }


        /// <summary>
        /// Парсим строку, получаем: Name,Status, Value
        /// </summary>
        /// <param name="tagToTag"></param>
        /// <returns></returns>
        private Tag FillTagAttributes(string tagToTag)
        {
            Tag tag = new Tag
            {
                Name = GetMatch(tagToTag, P_TAG_NAME),
                Status = GetTagStatus(tagToTag),
                Value = tagToTag
            };
            return tag;
        }
        #endregion


        public void SetResolvedAttributes(List<string> attributes)
        {
            SavingArrtibutes = attributes;
        }

        public List<Tag> Split(string html)
        {
            string pattern = @"<[\s\S]*?>[\s\S]*?(?=<[\s\S]*?>)";//replace into constant P_HTML_FROM_TAG_TO_TAG
            List<Tag> result = new List<Tag>();
            int cursorPosition = 0;
            string TagToTag = "";
            int tagPosition = 0;
            while (cursorPosition < html.Length)
            {
                TagToTag = GetMatch(html, pattern, cursorPosition); 
                
                Tag buf = new Tag();
                if (TagToTag != "")
                {
                    buf.Value = TagToTag;
                }
                else
                {
                    buf.Value = html.Substring(cursorPosition);
                    cursorPosition += buf.Value.Length;
                }
                buf.Position = tagPosition;
                buf.Name = GetMatch(buf.Value, P_TAG_NAME);
                buf.Status = GetTagStatus(buf.Value);                
                buf.Attributes = GetAttributes(buf.Value);

                
                cursorPosition += TagToTag.Length;
                result.Add(buf);
                tagPosition++;
            }
            return result;
        }

        public string LeadAttributesToXML(string tagToTag)
        {
            string result = "";
            MatchCollection attrs = Regex.Matches(tagToTag, @"\b[a-zA-Z0-9]+?[\s]*?=[\s]*?[\s\S]+?(?=(([\s]+?[\S]+?[\s]*?=)|>|/>))");
            result = Regex.Match(tagToTag, @"<[\s]*?[a-zA-Z0-9]+").Value + " ";
            foreach(Match str in attrs)
            {

                string buf= str.Value;

                result += PasteQuotes(buf);

                //int quotes = new Regex(@"""").Matches(buf).Count;
                //if(quotes==0)
                //{
                //    buf = Regex.Replace(buf, "=", @"=""");
                //    buf+=@"""";
                //}
                //else if(quotes==1)
                //{
                //    if(buf.TrimEnd(' ').Last()=='"')
                //    {
                //        buf = Regex.Replace(buf, "=", @"=""");
                //    }
                //    else
                //    {
                //        buf += @""" ";
                //    }
                //}
                //else if(quotes == 2)
                //{
                //    buf = buf.Substring(0, buf.LastIndexOf('"') + 1);
                //}
                //result += buf+" ";
            }
            //result += attrs.Count.ToString();
            result+= Regex.Match(tagToTag, @"(\>|>[\s\S]+)").Value + " ";
            return result;
        }

        public string PasteQuotes(string attr)
        {
            string result = "";

            string buf = attr;
            buf=Regex.Replace(buf,@"'","");
            int quotes = new Regex(@"""").Matches(buf).Count;
            if (quotes == 0)
            {
                buf = Regex.Replace(buf, "=", @"=""");
                buf += @"""";
            }
            else if (quotes == 1)
            {
                if (buf.TrimEnd(' ').Last() == '"')
                {
                    buf=@""""+buf;
                }
                else
                {
                    buf += @""" ";
                }
            }
            else if (quotes == 2)
            {
                buf = buf.Substring(0, buf.LastIndexOf('"') + 1);
            }
            result += buf + " ";



            return result; 
        }


        public Dictionary<string,string> GetAttributes(string tagToTag)
        {
            Dictionary<string,string> result = new Dictionary<string, string>();

            MatchCollection attrs = Regex.Matches(tagToTag, @"\b[a-zA-Z0-9]+?[\s]*?=[\s]*?[\s\S]+?(?=(([\s]+?[\S]+?[\s]*?=)|>|/>))");
            foreach(Match attrM in attrs )
            {
                string attr = attrM.Value;
                string key= Regex.Match(attr, @"[^=]*").Value;
                string value  = Regex.Match(attr, @"(?<==)[\s\S]+").Value; 
                if (result.ContainsKey(key))
                {
                    result[key] = result[key].TrimEnd(' ').TrimEnd('"') + " " + value.TrimStart(' ').TrimStart('"');
                    result[key]= PasteQuotes(result[key]);
                }
                else
                {
                    value = PasteQuotes(value);
                    result.Add(key, value);
                }
                
                
            }
            return result;
        }
        

        /// <summary>
        /// Ищет совпадение по заданному паттерну с определенного места
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pattern"></param>
        /// <param name="startpos"></param>
        /// <returns></returns>
        private string GetMatch(string html, string pattern, int startpos = 0)
        {
            Regex regex = new Regex(pattern);//, RegexOptions.Singleline
            string result = regex.Match(html, startpos).Value;
            return result;
        }



     




        /// <summary>
        /// Получаем статус тега
        /// </summary>
        /// <remarks>
        /// Варианты статуса:
        /// 0 - закрывающий тег </tag>;
        /// 1 - открывающий тег <tag>;
        /// 2 - одиночный закрывающийся тег <tag />.
        /// 3 - <!doctype ....>
        /// </remarks>
        /// <param name="tagToTag"></param>
        /// <returns></returns>
        private int GetTagStatus(string tagToTag)
        {
            int result=-1;

            if (Regex.IsMatch(tagToTag, P_SINGLE_CLOSING_TAG))//формат <tag />
                result = 2;
            else if (Regex.IsMatch(tagToTag, P_OPEN_TAG_NAME))//формат <tag>
                result = 1;
            else if (Regex.IsMatch(tagToTag, P_CLOSING_TAG_NAME)) //формат </tag>
                result = 0;
            else if (Regex.IsMatch(tagToTag, P_DOCTYPE))//Doctype
                result = 3;
            if (result == -1)
                throw new Exception("Can't define tag status for:\n"+tagToTag);

            return result;
        }


        /// <summary>
        /// Переписать без использования splitedHtml (подумать) т.к. этот список создается не в одной функции(наверно замедлит работу)
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public List<Tag> FindSingleTags(string html)
        {
            List<Tag> splitedHtml = Split(html);
            List<Tag> tags = new List<Tag>();//Список одиночных тегов
            Tag err = new Tag();
            Stack<Tag> stack = new Stack<Tag>();

            int position = 0;
            
            while (position<splitedHtml.Count-1)
             {
                try
                {

                foreach (Tag tag in splitedHtml)
                {
                    err = tag;
                    if (stack.Count != 0)
                    {
                        //Удалить элемент из верхушки если пришедший тег имеет такое же имя, но является закрывающим
                        if (stack.Peek().Name == tag.Name && stack.Peek().Status == 1 && tag.Status == 0)
                        {
                            stack.Pop();
                        }
                        //Если пришедший тег закрывающий и имеет другое имя, значит тег в стеке одиночный               
                        else if (stack.Peek().Name != tag.Name && tag.Status == 0)
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
                    else if (tag.Status < 3)
                        stack.Push(tag);
                    position = tag.Position;
                    }

                
            }
                catch
                {
                    Tag replacementTag = new Tag();
                    replacementTag.Attributes = new Dictionary<string, string>();
                    replacementTag.Name = "br";
                    replacementTag.Status = 2;
                    replacementTag.Position = err.Position;
                    replacementTag.Value = @"<br />";
                    splitedHtml[err.Position] = replacementTag;
                    
                }

            }
            int p = 0;
            int y = 0;
            try
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    y = i;
                    int pos = tags[i].Position;
                    Tag buf = splitedHtml[pos];
                    buf.Status = 2; //"</" + tg.Name + ">";
                    splitedHtml[pos] = buf;
                    p = pos;
                    
                }

            }
            catch
            {
                int k =y;
            }

            
            return splitedHtml;
            
        }


        


        /// <summary>
        /// Removing code block corresponds to pattern
        /// </summary>
        /// <param name="changedHtml"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public MatchCollection RemoveBlock(ref string changedHtml,string pattern)
        {
            MatchCollection result = GetMatches(changedHtml, pattern);
            changedHtml = Regex.Replace(changedHtml, pattern,"");

            return result;
        }


        public MatchCollection GetMatches(string html,string pattern,int startpos=0)
        {
            Regex regex = new Regex(pattern);
            MatchCollection result = regex.Matches(html, startpos);
            return result;
        }







        //public  string  ComplementSingleTags(string html)
        //{
        //    List<string> SplitedHtml = Split(html);
        //    List<Tag> singleTags = FindSingleTags(html);

        //        foreach (Tag tg in singleTags)
        //        {
        //            SplitedHtml[tg.Position] = SplitedHtml[tg.Position].Replace(">", "/>"); //"</" + tg.Name + ">";
        //        }

        //    string result = CompileHtml(SplitedHtml);
        //    return result;
        //}


        public string CompileHtml(List<Tag> splitedHtml)
        {
            string result = "";
            //Tag doctype = new Tag();
            
            foreach(Tag tag in splitedHtml)
            {
                try
                {
                    string buf = CompileString(tag);
                    result += buf;
                }
                catch(Exception e)
                {
                   
                    int k = tag.Position;
                }
                
            }
            return result;
        }

        public string CompileString(Tag tag)
        {
            string result = "";
            switch (tag.Status)
            {
                case 0:
                    result = "</" + tag.Name + ">"+Regex.Match(tag.Value,P_VALUE).Value;                    
                    break;
                case 1:
                    result = "<" + tag.Name ;
                    foreach(KeyValuePair<string, string> attribute in tag.Attributes)
                    {

                        foreach(string attr in SavingArrtibutes)
                        {
                            if (attr == attribute.Key)
                            {
                                result += " " + attribute.Key + "=" + attribute.Value;
                                break;
                            }                               
                        }
                        
                    }
                    result += " >" + Regex.Match(tag.Value, P_VALUE).Value;
                    break;
                case 2:
                    result = "<" + tag.Name;
                    foreach (KeyValuePair<string, string> attribute in tag.Attributes)
                    {
                        foreach (string attr in SavingArrtibutes)
                        {
                            if (attr == attribute.Key)
                            {
                                result += " " + attribute.Key + "=" + attribute.Value;
                                break;
                            }
                        }
                    }
                    result += " />" + Regex.Match(tag.Value, P_VALUE).Value;
                    break;
                case 3:
                    result = "<!" + tag.Name.ToUpper()+Regex.Match(tag.Value,@"(?<=<![\s]*?"+tag.Name+@"[\s]*?)[\s\S]*") + Regex.Match(tag.Value, P_VALUE).Value;
                    break;

            }

            return result;
        }


    }

    struct Tag
    {
        public Dictionary<string,string> Attributes;
        public string Name;
        public int Position;
        public int Status;
        public string Value;//часть строки от тега до тега:<tag attr> VALUE
        public string TagToTag;
    }


}
