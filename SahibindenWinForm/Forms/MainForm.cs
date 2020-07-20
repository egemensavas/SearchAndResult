using System;
using System.Windows.Forms;
using SahibindenWinForm.Classes;

namespace SahibindenWinForm
{
    public partial class MainForm : Form
    {
        #region Properties
        readonly EventClass EventClass;
        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            EventClass = new EventClass();
        }
        #endregion

        #region Event Methods
        private void Button1_Click(object sender, EventArgs e)
        {
            EventClass.ButtonClickEvent(sender, e);
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            EventClass.ButtonClickEvent(null, null);
            watch.Stop();
            MessageBox.Show("Done in " + (watch.ElapsedMilliseconds/1000).ToString().ToString() + " seconds.");
            Close();
        }
    }
}
