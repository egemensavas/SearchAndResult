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
        private void button1_Click(object sender, EventArgs e)
        {
            EventClass.ButtonClickEvent(sender, e);
        }
        #endregion
    }
}
