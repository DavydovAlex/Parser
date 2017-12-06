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
    #endregion


        /// <summary>
        /// Read local file 
        /// </summary>
        /// <param name="fileName"> Path to Html file</param>
        /// <param name="encoding"> Encoding </param>
        /// <returns></returns>
        private string ReadFile(string path,Encoding encoding)
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


        /// <summary>
        /// Get Html frol local file or via Net
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
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
        /// <param name="URL">URI</param>
        /// <param name="encoding"> Encoding </param>
        /// <returns></returns>
        private string GetHtmlViaInternet(string URI,Encoding encoding)
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
        private string GetHtmlViaInternet(Uri URI, Encoding encoding)
        {
            string result = GetHtmlViaInternet(URI.AbsoluteUri, encoding);
            return result;
        }


        /// <summary>
        /// Резбивает исходный Html файл на список строк от тега до тега
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        /// <example>dvvfefe</example>>
        public List<string> Split(string html)
        {
            string pattern = P_HTML_FROM_TAG_TO_TAG;
            List<string> result = new List<string>();
            int currentPosition = 0;
            string matchedString = "";
            while (currentPosition <= html.Length)
            {
                matchedString = GetMatch(html, pattern, currentPosition);
                if (matchedString == "")
                {
                    result.Add(html.Substring(currentPosition));
                    break;
                }
                    
                currentPosition += matchedString.Length;
                result.Add(matchedString);

            }
            return result;
        }

        public List<string> RemoveAtributes(List<string> splitedHtml)
        {
            List<string> result = new List<string>();
            string pattern = @"(?<=<[\s]*?[a-zA-Z0-9]*?\s)[^>]*?(?=(>|\>))";
            foreach(string elem in splitedHtml)
            {
                string buf = Regex.Replace(elem, pattern, "");
                result.Add(buf);
            }
            return result;
        }

        /// <summary>
        /// Ищет совпадение по заданному паттерну
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



        public List<Tag> GetTagList(string html)
        {
            List<string> splitedHtml = Split(html);
            List<Tag> splitedToTags = new List<Tag>();
            int tagPos = 0;
            Tag bufTag = new Tag();
            foreach (string elem in splitedHtml)
            {
                bufTag = FillTagAttributes(elem);
                bufTag.Position = tagPos;
                splitedToTags.Add(bufTag);
                tagPos++;
            }
            return splitedToTags;
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
                Value = GetMatch(tagToTag, P_VALUE)
            };
            return tag;
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
            else if (Regex.IsMatch(tagToTag, P_DOCTYPE))
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
        public List<Tag> FindSingleTagsT(string html)
        {
            List<string> splitedHtml = Split(html);
            Stack<Tag> stack = new Stack<Tag>();
            List<Tag> tags = new List<Tag>();//Список одиночных тегов
            Tag tag = new Tag();
            int pos = 0;
            foreach (string str in splitedHtml)
            {
                
                tag = FillTagAttributes(str);
                tag.Position = pos;


                if (tag.Status == 2 || tag.Status == 3)
                {
                    pos++;
                    continue;
                }
                    

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
                else
                    stack.Push(tag);

                pos++;
            }
            return tags;
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

        static MatchCollection GetMatches(string html,string pattern,int startpos=0)
        {
            Regex regex = new Regex(pattern);
            MatchCollection result = regex.Matches(html, startpos);
            return result;
        }


        /// <summary>
        /// Rewrite
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public Encoding GetEncoding(string html)
        {
            string head = GetMatch(html, GetPattern_PAIRED_TAG("head"));/*Знаем что Head только один берем первый элемент и сразу забираем значение*/
            string encoding=GetMatch(head, P_META);
            Encoding result;
            if (encoding.Length == 0)
                result = Encoding.UTF8;
            else
                result= Encoding.GetEncoding(encoding);
            return result;

        }


        public static string GetPattern_PAIRED_TAG(string tagName)
        {
            string pattern = @"<\s*?" + tagName + @"[^>]*?>[\s\S]*?</\s*?" + tagName + @"\s*?>"; 
            return pattern;
        }



        /// <summary>
        /// Переписать без использования splitedHtml (подумать) т.к. этот список создается не в одной функции(наверно замедлит работу)
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public  List<Tag> FindSingleTags(string html)
        {
            List<string> splitedHtml = Split(html);
            Stack<Tag> stack = new Stack<Tag>();         
            List<Tag> tags = new List<Tag>();//Список одиночных тегов
            Tag tag = new Tag();
            int pos = 0;
            foreach (string str in splitedHtml)
            {
                tag.Name = GetMatch(str, P_TAG_NAME);
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

        public  string  ComplementSingleTags(string html)
        {
            List<string> SplitedHtml = Split(html);
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

        public  void CheckAttributes(string html)
        {
            string attributes;
            if (!Regex.IsMatch(html, @"</[^>]>"))
            {
                string subStr = GetMatch(html, @"(?<=<\s*?[a-zA-Z0-9]*?\s)[^>]*?(?=>)");
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
        public string Value;

    }


}
