using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Abp.Text.RegularExpressions
{
    public class RegexUtils
    {
        //常用正则表达式常量 

        #region 匹配整型字符正则表达式

        /// <summary>
        ///     匹配整型字符正则表达式
        /// </summary>
        public const string IntegerPattern = @"((\\+)\\d)?\\d*";

        #endregion

        #region 匹配字符串为A-Z、0-9及下划线以内的字符正则表达式

        /// <summary>
        ///     匹配字符串为A-Z、0-9及下划线以内的字符正则表达式
        /// </summary>
        public const string LetterOrNumberPattern = @"[a-zA-Z0-9_]";

        #endregion

        #region 匹配双字节字符(包括汉字在内)或匹配中文字符的正则表达式

        /// <summary>
        ///     匹配双字节字符(包括汉字在内)/匹配中文字符的正则表达式
        ///     可以用来计算字符串的长度(一个双字节字符长度计2，ASCII字符计1)
        /// </summary>
        public const string UnicodeCharacterPattern = @"[\u4e00-\u9fa5]";

        #endregion

        #region 匹配空白行的正则表达式

        /// <summary>
        ///     匹配空白行的正则表达式
        ///     可以用来删除空白行
        /// </summary>
        public const string BlankLinePattern = @"\n\s*\r";

        #endregion

        #region 匹配HTML标记的正则表达式

        /// <summary>
        ///     匹配HTML标记的正则表达式
        ///     也仅仅能匹配部分，对于复杂的嵌套标记依旧无能为力
        /// </summary>
        public const string HtmlTagPattern = @"<(\S*?)[^>]*>.*?</\1>|<.*?/>";

        #endregion

        #region 匹配首尾空白字符的正则表达式

        /// <summary>
        ///     匹配首尾空白字符的正则表达式
        ///     可以用来删除行首行尾的空白字符(包括空格、制表符、换页符等等)，非常有用的表达式
        /// </summary>
        public const string BeginEndBlankPattern = @"^\s*|\s*$";

        #endregion

        #region 匹配网址URL的正则表达式

        /// <summary>
        ///     匹配网址URL的正则表达式
        ///     相对很完美的版本 它验证的情况包括:协议、IP、域名、二级域名，域名中的文件、端口
        /// </summary>
        public const string UrlPattern =
            @"((https|http|ftp|rtsp|mms)?://)?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?(([0-9]{1,3}\.){3}[0-9]{1,3}|([0-9a-z_!~*'()-]+\.)*([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\.[a-z]{2,6})(:[0-9]{1,4})?((/?)|(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)";

        #endregion

        #region 匹配Email地址的正则表达式

        /// <summary>
        ///     匹配Email地址的正则表达式
        /// </summary>
        public const string EmailPattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

        #endregion

        #region 匹配帐号是否合法(字母开头，允许5-16字节，允许字母数字下划线)

        /// <summary>
        ///     匹配帐号是否合法(字母开头，允许5-16字节，允许字母数字下划线)
        /// </summary>
        public const string AccountPattern = @"^[a-zA-Z][a-zA-Z0-9_]{4,15}$";

        #endregion

        #region 匹配IP地址

        /// <summary>
        ///     匹配IP地址
        /// </summary>
        public const string IpPattern =
            @"(((25[0-5]|2[0-4][0-9]|19[0-1]|19[3-9]|18[0-9]|17[0-1]|17[3-9]|1[0-6][0-9]|1[1-9]|[2-9][0-9]|[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9]))|(192\.(25[0-5]|2[0-4][0-9]|16[0-7]|169|1[0-5][0-9]|1[7-9][0-9]|[1-9][0-9]|[0-9]))|(172\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|1[0-5]|3[2-9]|[4-9][0-9]|[0-9])))\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])";

        #endregion

        #region 匹配时间正则表达式

        /// <summary>
        ///     匹配时间正则表达式，时间格式：15:00:00
        /// </summary>
        public const string TimePattern = @"((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)";

        #endregion

        #region 匹配日期正则表达式

        /// <summary>
        ///     匹配日期正则表达式
        ///     正则表达式拼接而成，瑞年二月判断、非闰年二月判断、其它月份判断，日期格式可以为YYYY-MM-DD、YYYY/MM/DD、YYYY_MM_DD、YYYY.MM.DD
        /// </summary>
        public const string DatePattern =
            @"((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9])$)|(^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)$))";

        #endregion

        #region 匹配日期加日期正则表达式

        /// <summary>
        ///     匹配日期加日期正则表达式
        ///     正则表达式拼接而成，瑞年二月判断、非闰年二月判断、其它月份判断，格式为YYYY-MM-DD 15:00:00
        /// </summary>
        public const string DateTimePattern =
            @"(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)";

        #endregion

        #region 匹配国内电话号码(包含区号)

        /// <summary>
        ///     匹配国内电话号码(包含区号)
        ///     匹配形式如="XXX-XXXXXXX"、"XXXX-XXXXXXXX"、"XXX-XXXXXXX"、"XXX-XXXXXXXX"、"XXXXXXX"和"XXXXXXXX"
        /// </summary>
        public const string ChineseTelephonePattern = @"(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}";

        #endregion

        #region 匹配腾讯QQ号

        /// <summary>
        ///     匹配腾讯QQ号
        ///     腾讯QQ号从10000开始
        /// </summary>
        public const string QqNoPattern = @"[1-9][0-9]{4,9}";

        #endregion

        #region 匹配中国邮政编码

        /// <summary>
        ///     匹配中国邮政编码
        ///     中国邮政编码为6位数字
        /// </summary>
        public const string ChineseZipcodePattern = @"[1-9]\d{5}(?!\d)";

        #endregion

        #region 匹配中国身份证

        /// <summary>
        ///     匹配中国身份证
        ///     中国的身份证为15位或18位
        /// </summary>
        public const string ChineseIdentityNoPattern = @"[1-9]\d{14}|[1-9]\d{17}";

        #endregion

        #region 匹配Base64字符串

        /// <summary>
        ///     base64字符串
        /// </summary>
        public const string Base64Pattern = @"[A-Za-z0-9\+\/\=]";

        #endregion

        #region 匹配Sql危险字符

        /// <summary>
        ///     匹配Sql危险字符
        /// </summary>
        public const string SafeSqlStrPattern = @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']";

        #endregion

        #region 匹配有危险的可能用于链接的字符串

        /// <summary>
        ///     匹配有危险的可能用于链接的字符串
        /// </summary>
        public const string SafeUserInfoStrPattern = @"/^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|$guestexp/is";

        #endregion

        private static readonly Regex RegNumber = new Regex("^[0-9]+$");
        private static readonly Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");
        private static readonly Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$");
        private static readonly Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //等价于^[+-]?\d+[.]?\d+$

        private static readonly Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");
            //w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 

        private static readonly Regex RegChzn = new Regex("[\u4e00-\u9fa5]");
        //常用方法

        #region 是否匹配整型字符正则表达式

        /// <summary>
        ///     是否匹配整型字符正则表达式
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsInteger(string input)
        {
            return Regex.IsMatch(input, "^" + IntegerPattern + "$");
        }

        #endregion

        #region 是否匹配字符串为A-Z、0-9及下划线以内的字符正则表达式

        /// <summary>
        ///     是否匹配字符串为A-Z、0-9及下划线以内的字符正则表达式
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsLetterOrNumber(string input)
        {
            return Regex.IsMatch(input, "^" + LetterOrNumberPattern + "$");
        }

        #endregion

        #region 是否匹配双字节字符(包括汉字在内)或匹配中文字符的正则表达式

        /// <summary>
        ///     是否匹配双字节字符(包括汉字在内)或匹配中文字符的正则表达式
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsUnicodeCharacter(string input)
        {
            return Regex.IsMatch(input, "^" + UnicodeCharacterPattern + "$");
        }

        #endregion

        #region 是否匹配网址URL的正则表达式

        /// <summary>
        ///     是否匹配网址URL的正则表达式
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsUrl(string input)
        {
            return Regex.IsMatch(input, "^" + UrlPattern + "$");
        }

        #endregion

        #region 是否匹配Email地址的正则表达式

        /// <summary>
        ///     是否匹配Email地址的正则表达式
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsEmail(string input)
        {
            return Regex.IsMatch(input, "^" + EmailPattern + "$");
        }

        #endregion

        #region 是否匹配IP地址

        /// <summary>
        ///     是否匹配IP地址
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIp(string input)
        {
            return Regex.IsMatch(input, "^" + IpPattern + "$");
        }

        #endregion

        #region 是否匹配时间

        /// <summary>
        ///     是否匹配时间
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsTime(string input)
        {
            return Regex.IsMatch(input, "^" + TimePattern + "$");
        }

        #endregion

        #region 是否匹配日期

        /// <summary>
        ///     是否匹配日期
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsDate(string input)
        {
            return Regex.IsMatch(input, "^" + DatePattern + "$");
        }

        #endregion

        #region 是否为日期加时间

        /// <summary>
        ///     是否为日期加时间
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateTime(string input)
        {
            return Regex.IsMatch(input, @"^" + DateTimePattern + "$");
        }

        #endregion

        #region 是否匹配国内电话号码(包含区号)

        /// <summary>
        ///     是否匹配国内电话号码(包含区号)
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsChineseTelephone(string input)
        {
            return Regex.IsMatch(input, "^" + ChineseTelephonePattern + "$");
        }

        #endregion

        #region 是否匹配腾讯QQ号

        /// <summary>
        ///     是否匹配腾讯QQ号
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsQQNo(string input)
        {
            return Regex.IsMatch(input, "^" + QqNoPattern + "$");
        }

        #endregion

        #region 是否匹配中国邮政编码

        /// <summary>
        ///     是否匹配中国邮政编码
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsChineseZipcode(string input)
        {
            return Regex.IsMatch(input, "^" + ChineseZipcodePattern + "$");
        }

        #endregion

        #region 是否匹配中国身份证

        /// <summary>
        ///     是否匹配中国身份证
        /// </summary>
        /// <param name="input">待匹配字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsChineseIdentityNo(string input)
        {
            return Regex.IsMatch(input, "^" + ChineseIdentityNoPattern + "$");
        }

        #endregion

        //通用方法

        #region 指示输入字符串是否和正则表达式匹配

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static bool IsMatch(string input, string pattern, RegexOptions option)
        {
            return Regex.IsMatch(input, pattern, option);
        }

        #endregion

        public static bool IsMatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        public static string Replace(string input, string pattern, MatchEvaluator matchEvaluator, RegexOptions option)
        {
            return Regex.Replace(input, pattern, matchEvaluator, option);
        }

        public static string Replace(string input, string pattern, string replacement, RegexOptions option)
        {
            return Regex.Replace(input, pattern, replacement, option);
        }

        public static string Replace(string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public static string Replace(string input, string pattern)
        {
            return Regex.Replace(input, pattern, "");
        }

        public static string[] Split(string input, string pattern, RegexOptions option)
        {
            return Regex.Split(input, pattern, option);
        }

        public static string[] Split(string input, string pattern)
        {
            return Regex.Split(input, pattern);
        }

        #region 判断文件流是否为UTF8字符集

        /// <summary>
        ///     判断文件流是否为UTF8字符集
        /// </summary>
        /// <param name="sbInputStream">文件流</param>
        /// <returns>判断结果</returns>
        public static bool IsUTF8(FileStream sbInputStream)
        {
            int i;
            byte cOctets; // octets to go in this UTF-8 encoded character 
            byte chr;
            var bAllAscii = true;
            var iLen = sbInputStream.Length;

            cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                chr = (byte) sbInputStream.ReadByte();

                if ((chr & 0x80) != 0) bAllAscii = false;

                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        } while ((chr & 0x80) != 0);

                        cOctets--;
                        if (cOctets == 0) return false;
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    cOctets--;
                }
            }

            if (cOctets > 0)
            {
                return false;
            }

            if (bAllAscii)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 判断是否为base64字符串

        /// <summary>
        ///     判断是否为base64字符串
        /// </summary>
        /// <param name="str">待验证字符串</param>
        /// <returns>验证结果</returns>
        public static bool IsBase64String(string str)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(str, Base64Pattern);
        }

        #endregion

        #region 检测是否有Sql危险字符

        /// <summary>
        ///     检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, SafeSqlStrPattern);
        }

        #endregion

        #region 检测是否有危险的可能用于链接的字符串

        /// <summary>
        ///     检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, SafeUserInfoStrPattern);
        }

        #endregion

        #region 判断是否系统类型

        /// <summary>
        ///     判断是否系统类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns></returns>
        public static bool IsSystemNormalType(string typeName)
        {
            if (typeName == TypeCode.Boolean.ToString() || typeName == TypeCode.Byte.ToString()
                || typeName == TypeCode.Char.ToString() || typeName == TypeCode.DateTime.ToString()
                || typeName == TypeCode.DBNull.ToString() || typeName == TypeCode.Decimal.ToString()
                || typeName == TypeCode.Double.ToString() || typeName == TypeCode.Empty.ToString()
                || typeName == TypeCode.Int16.ToString() || typeName == TypeCode.Int32.ToString()
                || typeName == TypeCode.Int64.ToString() || typeName == TypeCode.Object.ToString()
                || typeName == TypeCode.SByte.ToString() || typeName == TypeCode.Single.ToString()
                || typeName == TypeCode.String.ToString() || typeName == TypeCode.UInt16.ToString()
                || typeName == TypeCode.UInt32.ToString() || typeName == TypeCode.UInt64.ToString())
            {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        ///     是否数字字符串
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumber(string inputData)
        {
            var m = RegNumber.Match(inputData);
            return m.Success;
        }

        /// <summary>
        ///     Judge input string whether or not is Number
        /// </summary>
        /// <param name="strNumber">string</param>
        /// <returns></returns>
        public static bool ComplexIsNumber(string strNumber)
        {
            var objNotNumberPattern = new Regex("[^0-9.-]");
            var objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            var objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            var strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            var strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            var objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) && !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) && objNumberPattern.IsMatch(strNumber);
        }

        /// <summary>
        ///     是否数字字符串 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumberSign(string inputData)
        {
            var m = RegNumberSign.Match(inputData);
            return m.Success;
        }

        /// <summary>
        ///     是否是浮点数
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(string inputData)
        {
            var m = RegDecimal.Match(inputData);
            return m.Success;
        }

        /// <summary>
        ///     是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimalSign(string inputData)
        {
            var m = RegDecimalSign.Match(inputData);
            return m.Success;
        }

        #region 中文检测

        /// <summary>
        ///     检测是否有中文字符
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsHasCHZN(string inputData)
        {
            var m = RegChzn.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 邮件地址

        /// <summary>
        ///     是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsEmail2(string inputData)
        {
            var m = RegEmail.Match(inputData);
            return m.Success;
        }

        #endregion
    }
}