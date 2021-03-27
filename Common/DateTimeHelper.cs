using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WSY.Common
{
    public class DateTimeHelper
    {
        #region 判断
        /// <summary>
        /// 是否为日期型字符串
        /// </summary>
        /// <param name="StrSource">日期字符串(2008-05-08)</param>
        /// <returns></returns>
        public static bool IsDate(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|
                        (((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|
                        (((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }

        /// <summary>
        /// 是否为时间型字符串
        /// </summary>
        /// <param name="source">时间字符串(15:00:00)</param>
        /// <returns></returns>
        public static bool IsTime(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        /// 是否为日期+时间型字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateTime(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$ ");
        }

        #endregion

        /// <summary> 
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime time)
        {
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString();
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public static string GetToday()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static string GetToday(string format)
        {
            return DateTime.Now.ToString(format);
        }

        public static string GetDate(int i)
        {
            return DateTime.Now.AddDays((double)i).ToString("yyyy-MM-dd");
        }

        public static string GetNumberWeekDay(DateTime dt)
        {
            int y = dt.Year;
            int i = dt.Month;
            int d = dt.Day;
            if (i < 3)
            {
                i += 12;
                y--;
            }
            if (y % 400 == 0 || (y % 100 != 0 && y % 4 == 0))
            {
                d--;
            }
            else
            {
                d++;
            }
            return ((d + 2 * i + 3 * (i + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7).ToString();
        }

        public string GetChineseWeekDay(int y, int m, int d)
        {
            string[] weekstr = new string[]
			{
				"日",
				"一",
				"二",
				"三",
				"四",
				"五",
				"六"
			};
            if (m < 3)
            {
                m += 12;
                y--;
            }
            if (y % 400 == 0 || (y % 100 != 0 && y % 4 == 0))
            {
                d--;
            }
            else
            {
                d++;
            }
            return "星期" + weekstr[(d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7];
        }

        public static int GetDaysOfYear(int iYear)
        {
            return DateTimeHelper.IsRuYear(iYear) ? 366 : 365;
        }

        public static int GetDaysOfYear(DateTime dt)
        {
            return IsRuYear(dt.Year) ? 366 : 365;
        }

        public static int GetDaysOfMonth(int iYear, int Month)
        {
            int days = 0;
            switch (Month)
            {
                case 1:
                    days = 31;
                    break;

                case 2:
                    days = (IsRuYear(iYear) ? 29 : 28);
                    break;

                case 3:
                    days = 31;
                    break;

                case 4:
                    days = 30;
                    break;

                case 5:
                    days = 31;
                    break;

                case 6:
                    days = 30;
                    break;

                case 7:
                    days = 31;
                    break;

                case 8:
                    days = 31;
                    break;

                case 9:
                    days = 30;
                    break;

                case 10:
                    days = 31;
                    break;

                case 11:
                    days = 30;
                    break;

                case 12:
                    days = 31;
                    break;
            }
            return days;
        }

        public static int GetDaysOfMonth(DateTime dt)
        {
            int days = 0;
            int year = dt.Year;
            switch (dt.Month)
            {
                case 1:
                    days = 31;
                    break;

                case 2:
                    days = (IsRuYear(year) ? 29 : 28);
                    break;

                case 3:
                    days = 31;
                    break;

                case 4:
                    days = 30;
                    break;

                case 5:
                    days = 31;
                    break;

                case 6:
                    days = 30;
                    break;

                case 7:
                    days = 31;
                    break;

                case 8:
                    days = 31;
                    break;

                case 9:
                    days = 30;
                    break;

                case 10:
                    days = 31;
                    break;

                case 11:
                    days = 30;
                    break;

                case 12:
                    days = 31;
                    break;
            }
            return days;
        }

        public static string GetWeekNameOfDay(DateTime dt)
        {
            string week = string.Empty;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    week = "星期日";
                    break;

                case DayOfWeek.Monday:
                    week = "星期一";
                    break;

                case DayOfWeek.Tuesday:
                    week = "星期二";
                    break;

                case DayOfWeek.Wednesday:
                    week = "星期三";
                    break;

                case DayOfWeek.Thursday:
                    week = "星期四";
                    break;

                case DayOfWeek.Friday:
                    week = "星期五";
                    break;

                case DayOfWeek.Saturday:
                    week = "星期六";
                    break;
            }
            return week;
        }

        public static int GetWeekNumberOfDay(DateTime dt)
        {
            int week = 0;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    week = 7;
                    break;

                case DayOfWeek.Monday:
                    week = 1;
                    break;

                case DayOfWeek.Tuesday:
                    week = 2;
                    break;

                case DayOfWeek.Wednesday:
                    week = 3;
                    break;

                case DayOfWeek.Thursday:
                    week = 4;
                    break;

                case DayOfWeek.Friday:
                    week = 5;
                    break;

                case DayOfWeek.Saturday:
                    week = 6;
                    break;
            }
            return week;
        }

        public static int GetWeekAmount(int year)
        {
            DateTime end = new DateTime(year, 12, 31);
            GregorianCalendar gc = new GregorianCalendar();
            return gc.GetWeekOfYear(end, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static int GetWeekOfYear(DateTime dt)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static void WeekRange(int year, int weekOrder, ref DateTime firstDate, ref DateTime lastDate)
        {
            DateTime firstDay = new DateTime(year, 1, 1);
            int firstOfWeek = Convert.ToInt32(firstDay.DayOfWeek);
            int dayDiff = -1 * firstOfWeek + 1;
            int dayAdd = 7 - firstOfWeek;
            firstDate = firstDay.AddDays((double)dayDiff).Date;
            lastDate = firstDay.AddDays((double)dayAdd).Date;
            if (weekOrder != 1)
            {
                int addDays = (weekOrder - 1) * 7;
                firstDate = firstDate.AddDays((double)addDays);
                lastDate = lastDate.AddDays((double)addDays);
            }
        }

        public static int DiffDays(DateTime dtfrm, DateTime dtto)
        {
            if (dtto.ToString("yyyy-MM-dd") == "1900-01-01" || dtfrm.ToString("yyyy-MM-dd") == "1900-01-01")
            { return 0; }
            else
            {
                return (dtto.Date - dtfrm.Date).Days;
            }
        }

        private static bool IsRuYear(int iYear)
        {
            return iYear % 400 == 0 || (iYear % 4 == 0 && iYear % 100 != 0);
        }

        public static DateTime ToDate(string strInput)
        {
            DateTime oDateTime;
            try
            {
                oDateTime = DateTime.Parse(strInput);
            }
            catch (Exception)
            {
                oDateTime = DateTime.Today;
            }
            return oDateTime;
        }

        public static string ToString(DateTime oDateTime, string strFormat)
        {
            string strDate;
            try
            {
                string text = strFormat.ToUpper();
                if (text != null)
                {
                    if (text == "SHORTDATE")
                    {
                        strDate = oDateTime.ToShortDateString();
                        goto IL_47;
                    }
                    if (text == "LONGDATE")
                    {
                        strDate = oDateTime.ToLongDateString();
                        goto IL_47;
                    }
                }
                strDate = oDateTime.ToString(strFormat);
            IL_47: ;
            }
            catch (Exception)
            {
                strDate = oDateTime.ToShortDateString();
            }
            return strDate;
        }

        /// <summary>
        /// 默认长时间替换
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="defValue">缺省值</param>
        public static string ReplaceToDateTime(DateTime obj, string defValue)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj).Replace("1900-01-01 00:00:00", defValue);
        }

        /// <summary>
        /// 默认短时间替换
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="defValue">缺省值</param>
        public static string ReplaceToShortDateTime(DateTime obj, string defValue)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm}", obj).Replace("1900-01-01 00:00", defValue);
        }

        #region 时间转文本
        public static string DateTimeToRead(string date)
        {
            string result = null;
            if (date!="")
            {
                result = DateTimeToRead(Convert.ToDateTime(date) as DateTime?);
            } 
            return result;
        }

        public static string DateTimeToRead(DateTime src)
        {
            return DateTimeToRead(src as DateTime?);
        }
        public static string DateTimeToRead(DateTime? src)
        {
            string result = null;
            if (src.HasValue)
            {
                result = DateTimeToStr2(src.Value);
            }
            return result;
        }

        private static string DateTimeToStr1(DateTime src)
        {
            string result = null;
            TimeSpan timeSpan = DateTime.Now - src;
            if (((int)timeSpan.TotalDays) > 365) //大于一年的  
            {
                int years = (int)timeSpan.TotalDays / 365;
                //if (timeSpan.TotalDays % 365 > 0) years++;  
                result = string.Format("{0}年之前", years);
            }
            else if (((int)timeSpan.TotalDays) > 30) //大于一个月的  
            {
                int months = (int)timeSpan.TotalDays / 30;
                //if (timeSpan.TotalDays % 30 > 0) months++;  
                result = string.Format("{0}个月前", months);
            }
            else if (((int)timeSpan.TotalDays) > 7) //大于一周的  
            {
                int weeks = (int)timeSpan.TotalDays / 7;
                //if (timeSpan.TotalDays % 7 > 0) weeks++;  
                result = string.Format("{0}周前", weeks);
            }
            else if (((int)timeSpan.TotalDays) > 0) //大于 0 天的  
            {
                result = string.Format("{0}天前", (int)timeSpan.TotalDays);
            }
            else if (((int)timeSpan.TotalHours) > 0) //一小时以上的  
            {
                result = string.Format("{0}小时", (int)timeSpan.TotalHours);
            }
            else if (((int)timeSpan.TotalMinutes) > 0) //一分钟以上的  
            {
                result = string.Format("{0}分钟前", (int)timeSpan.TotalMinutes);
            }
            else if (((int)timeSpan.TotalSeconds) >= 0 && ((int)timeSpan.TotalSeconds) <= 60) //一分钟内  
            {
                result = "刚刚";
            }
            else
            {
                result = src.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return result;
        }

        private static string DateTimeToStr2(DateTime src)
        {
            string result = null;

            long currentSecond = (long)(DateTime.Now - src).TotalSeconds;

            long minSecond = 60;                //60s = 1min  
            long hourSecond = minSecond * 60;   //60*60s = 1 hour  
            long daySecond = hourSecond * 24;   //60*60*24s = 1 day  
            long weekSecond = daySecond * 7;    //60*60*24*7s = 1 week  
            long monthSecond = daySecond * 30;  //60*60*24*30s = 1 month  
            long yearSecond = daySecond * 365;  //60*60*24*365s = 1 year  

            if (currentSecond >= yearSecond)
            {
                int year = (int)(currentSecond / yearSecond);
                result = string.Format("{0}年前", year);
            }
            else if (currentSecond < yearSecond && currentSecond >= monthSecond)
            {
                int month = (int)(currentSecond / monthSecond);
                result = string.Format("{0}个月前", month);
            }
            else if (currentSecond < monthSecond && currentSecond >= weekSecond)
            {
                int week = (int)(currentSecond / weekSecond);
                result = string.Format("{0}周前", week);
            }
            else if (currentSecond < weekSecond && currentSecond >= daySecond)
            {
                int day = (int)(currentSecond / daySecond);
                result = string.Format("{0}天前", day);
            }
            else if (currentSecond < daySecond && currentSecond >= hourSecond)
            {
                int hour = (int)(currentSecond / hourSecond);
                result = string.Format("{0}小时前", hour);
            }
            else if (currentSecond < hourSecond && currentSecond >= minSecond)
            {
                int min = (int)(currentSecond / minSecond);
                result = string.Format("{0}分钟前", min);
            }
            else if (currentSecond < minSecond && currentSecond >= 0)
            {
                result = "刚刚";
            }
            else
            {
                result = src.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 声明期间类型枚举
        /// </summary>
        public enum Period { Day, Week, Month, Year };
        /// <summary>
        /// 获取指定期间的起止日期
        /// </summary>
        /// <param name="period">期间类型</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        public static void GetPeriod(Period period, out DateTime beginDate, out DateTime endDate)
        {
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;
            switch (period)
            {
                case Period.Year: //年
                    beginDate = new DateTime(year, 1, 1);
                    endDate = new DateTime(year, 12, 31);
                    break;
                case Period.Month: //月
                    beginDate = new DateTime(year, month, 1);
                    endDate = beginDate.AddMonths(1).AddDays(-1);
                    break;
                case Period.Week: //周
                    int week = (int)DateTime.Today.DayOfWeek;
                    if (week == 0) week = 7; //周日
                    beginDate = DateTime.Today.AddDays(-(week - 1));
                    endDate = beginDate.AddDays(6);
                    break;
                default: //日
                    beginDate = DateTime.Today;
                    endDate = DateTime.Today;
                    break;
            }
        }
    }
}
