using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
namespace ConsoleApp1
{
    class Program
    {
        public static ReaderWriterLockSlim ReaderWriterLockSlim = new ReaderWriterLockSlim();
        static void Main(string[] args)
        {
            //string a = @"<?xml version='1.0' encoding='utf - 8'?> <ROOT> <WorkCode>W120320180927160000580653</WorkCode> <Status>0</Status> <Desc>工单执行成功，数据已下发至 10.211.112.16 Olt NA-NA-13-5 Pon口</Desc> <Rollback></Rollback> </ROOT> ";
            //string b = @" <? xml version = '1.0' encoding = 'UTF-8' ?>< ROOT >< Status > 0 </ Status >< Desc > 接收成功!</ Desc ></ ROOT > ";

            //Console.WriteLine(a);
            //Console.WriteLine(b);

            //Console.WriteLine(a.Length);
            //Console.WriteLine(b.Length);

            //string c = a.Remove(120);
            //string d = b.Remove(60);
            //Console.WriteLine(c);
            //Console.WriteLine(d);

            //MyClass.WriteSometing();
            Config config = new Config(new Config.delegateRun(Config_eventRun));
            ThreadPool.QueueUserWorkItem(config.UpdateStrings);
            //config.UpdateStrings();
            //config.EventRun += Config_eventRun;

            while(true)
            {
                Console.WriteLine("ExitReadlock...");
                ReaderWriterLockSlim.EnterReadLock();
                //Console.WriteLine("In main update");
                foreach (var item in MyClass.YD)
                {
                    Console.Write(item);
                }
                //Console.WriteLine();
                //foreach (var item in MyClass.LT)
                //{
                //    Console.Write(item);
                //}
                //Console.WriteLine();
                //foreach (var item in MyClass.DX)
                //{
                //    Console.Write(item);
                //}
                ReaderWriterLockSlim.ExitReadLock();
                Console.WriteLine("ExitReadlock...");
                Console.WriteLine();

                Thread.Sleep(1000);
            }
            
            Console.ReadKey();

        }

        private static void Config_eventRun()
        {
            MyClass.UpdateCollection();
        }
    }

    public static class MyClass
    {
        public static string[] YD = Config.ChinaMobile.Replace('，',',').Split(',').Select(n => n.Trim(' ')).ToArray();
        public static string[] LT = Config.ChinaTelecom.Replace('，', ',').Split(',').Select(n => n.Trim(' ')).ToArray();
        public static string[] DX = Config.ChinaUnicom.Replace('，', ',').Split(',').Select(n => n.Trim(' ')).ToArray();

        static readonly object m_lock = new object();
        

        public static void UpdateCollection()
        {
            Console.WriteLine("enterwritelock...");
            Program.ReaderWriterLockSlim.EnterWriteLock();
            {
                Console.WriteLine("update something");
                YD = Config.ChinaMobile.Replace('，', ',').Split(',').Select(n => n.Trim(' ')).ToArray();
                LT = Config.ChinaTelecom.Replace('，', ',').Split(',').Select(n => n.Trim(' ')).ToArray();
                DX = Config.ChinaUnicom.Replace('，', ',').Split(',').Select(n => n.Trim(' ')).ToArray();
            }
            Program.ReaderWriterLockSlim.ExitWriteLock();
            Console.WriteLine("ExitWritelock...");
        }
        public static void WriteSometing()
        {
            Console.WriteLine("hello");
        }
    }

    public class Config
    {
        //定义一个委托
     public delegate void delegateRun();
     //定义一个事件
     public event delegateRun EventRun;

        public Config(delegateRun run)
        {
            EventRun += run;
        }
        static string chinaMobile = ConfigurationManager.AppSettings["ChinaMobile"];
        static string chinaTelecom = ConfigurationManager.AppSettings["ChinaTelecom"];
        static string chinaUnicom = ConfigurationManager.AppSettings["ChinaUnicom"];

        //比较字符串是否变动
        //比较chinaMobile是否更新
        //用事件

        //一旦读了，就不再更新了，除非使用ConfigurationManager.RefreshSection("appSettings");
        public static string ChinaMobile=ConfigurationManager.AppSettings["ChinaMobile"];
        public static string ChinaTelecom= ConfigurationManager.AppSettings["ChinaTelecom"];
        public static string ChinaUnicom= ConfigurationManager.AppSettings["ChinaUnicom"];

        public void UpdateStrings(object obj)
        {

            while (true)
            { 
                ConfigurationManager.RefreshSection("appSettings");
                //Console.WriteLine(chinaMobile);
                //Console.WriteLine(ChinaMobile);
                //Console.WriteLine();
                if (ChinaMobile != ConfigurationManager.AppSettings["ChinaMobile"])
                {
                    ChinaMobile= ConfigurationManager.AppSettings["ChinaMobile"];
                    EventRun();
                }
                Thread.Sleep(1000);
            }
        }
    }

}
