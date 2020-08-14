using System;
using System.ServiceProcess;

namespace Scrapper_Windows_Service
{
    public partial class Service1 : ServiceBase
    {
        Gary_Matter_Scrapper_Library.Classes.LogClass LogClass;
        Gary_Matter_Scrapper_Library.Classes.MainClass MainClass;
        public Service1()
        {
            InitializeComponent();
            LogClass = new Gary_Matter_Scrapper_Library.Classes.LogClass();
            MainClass = new Gary_Matter_Scrapper_Library.Classes.MainClass();
        }

        protected override void OnStart(string[] args)
        {
            LogClass.LogWriter("Started");
            try
            {
                LogClass.LogWriter(GetInterval().ToString());
                MainClass.SahibindenScrapper();
                System.Timers.Timer aTimer = new System.Timers.Timer();
                aTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
                aTimer.Interval = GetInterval();
                aTimer.Enabled = true;
                aTimer.Start();
            }
            catch (Exception e)
            {
                LogClass.LogWriter("Error:" + e.Message);
            }
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            LogClass.LogWriter("TickStart");
            MainClass.SahibindenScrapper();
            LogClass.LogWriter("TickEnd");
        }

        protected override void OnStop()
        {
            LogClass.LogWriter("Finished");
        }

        internal double GetInterval()
        {
            return Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["Interval"]);
        }
    }
}
