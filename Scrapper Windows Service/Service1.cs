using System;
using System.ServiceProcess;

namespace Scrapper_Windows_Service
{
    public partial class Service1 : ServiceBase
    {
        readonly HelperClass HelperClass;
        public Service1()
        {
            InitializeComponent();
            HelperClass = new HelperClass();
        }

        protected override void OnStart(string[] args)
        {
            HelperClass.LogWriter("Started");
            try
            {
                HelperClass.DoTheThing();
                System.Timers.Timer aTimer = new System.Timers.Timer();
                aTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
                aTimer.Interval = 180000;
                aTimer.Enabled = true;
                aTimer.Start();
            }
            catch (Exception e)
            {
                HelperClass.LogWriter("Error:" + e.Message);
            }
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            HelperClass.LogWriter("TickStart");
            HelperClass.DoTheThing();
            HelperClass.LogWriter("TickEnd");
        }

        protected override void OnStop()
        {
            HelperClass.LogWriter("Finished");
        }
    }
}
