using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SysTimeUpdate
{
    class Program
    {
        static void Main(string[] args)
        {

            //从日志文件中获取本地时间和服务器时间差值
          
            //设置日志文件位置
            string logPath = @".\oesA.log";

            TimeSpan gapTime = new TimeSpan();
            Match gapMatch = null;
            Match localTimeMatch = null;
            Console.WriteLine("Time sync begins... Press any key to interrupt.");

            //循环持续进行
            while (true)
            {
                //当有键盘输入时，跳出循环，程序退出
                if (Console.KeyAvailable)
                {
                    break;
                }

                //读取日志文件
                string[] fileLines = File.ReadAllLines(logPath);
                //在日志文件中从后向前遍历，获取gap值
                for (int linenum = fileLines.Length - 1; linenum >= 0; linenum--)
                {
                    if (Regex.IsMatch(fileLines[linenum], @"Time Synchronized, the gap is (\d)+/(\d)+"))
                    {

                        gapMatch = Regex.Match(fileLines[linenum], @"(?<=Time Synchronized, the gap is )(\d)+");
                        localTimeMatch = Regex.Match(fileLines[linenum], @"\d\d-\d\d-\d\d \d\d:\d\d:\d\d,\d{3}");
                        break;
                    }
                }

                gapTime = TimeSpan.FromMilliseconds(double.Parse(gapMatch.Value));
                //获取本地时间
                DateTime localTime = DateTime.Now;
                //使用gap值与本地时间相加，获取用来更新的时间
                DateTime updateTime = localTime + gapTime;

                //创建SystemTime类，并赋予与服务器时间校对过的新时间，用于更新本地系统时间
                SystemTime sysTime = new SystemTime();
                sysTime.vYear = (ushort)updateTime.Year;
                sysTime.vMonth = (ushort)updateTime.Month;
                sysTime.vDay = (ushort)updateTime.Day;
                sysTime.vHour = (ushort)updateTime.Hour;
                sysTime.vMinute = (ushort)updateTime.Minute;
                sysTime.vSecond = (ushort)updateTime.Second;
                sysTime.vMillisecond = (ushort)updateTime.Millisecond;
                //更新本地时间
                SetSystemDateTime.SetLocalTime(sysTime);

                //输出信息
                Console.WriteLine("#############################################################");
                Console.WriteLine("Local system time before sync,{0}", localTime.ToString("yyyyMMdd HH:mm:ss, fff"));
                Console.WriteLine("Time gap,{0}", gapTime.TotalMilliseconds);
                Console.WriteLine("Local system time after sync,{0}", updateTime.ToString("yyyyMMdd HH:mm:ss, fff"));
                Console.WriteLine("#############################################################");
                Console.WriteLine();
                
                //线程暂停，用于设定时间同步频率，单位为毫秒
                Thread.Sleep(5000);
            }

        }
    }

    public class SetSystemDateTime
    {

        [DllImportAttribute("Kernel32.dll")]

        public static extern void GetLocalTime(SystemTime st);

        [DllImportAttribute("Kernel32.dll")]

        public static extern void SetLocalTime(SystemTime st);

    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public class SystemTime

    {

        public ushort vYear;

        public ushort vMonth;

        public ushort vDayOfWeek;

        public ushort vDay;

        public ushort vHour;

        public ushort vMinute;

        public ushort vSecond;

        public ushort vMillisecond;

    }
}
