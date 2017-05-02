using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace SysTimeUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemTime sysTime = new SystemTime();
            while (true)
            {

                SetSystemDateTime.GetLocalTime(sysTime);
                string time = sysTime.vYear.ToString() + "-" + sysTime.vMonth.ToString() + "-" + sysTime.vDay.ToString()
                    + " " + sysTime.vHour.ToString() + ":" + sysTime.vMinute.ToString() + ":" + sysTime.vSecond.ToString();
                Console.WriteLine(time);
                Thread.Sleep(1000);
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey().KeyChar == 'y')
                    {
                        sysTime.vMinute -= 10;
                        SetSystemDateTime.SetLocalTime(sysTime);
                    }
                    else if(Console.ReadKey().KeyChar == 'n')
                    {
                        sysTime.vMinute += 10;
                        SetSystemDateTime.SetLocalTime(sysTime);
                    }
                }
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

    }
}
