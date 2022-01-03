using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class ScriptedLCDDisplayContentPanel : UserControl
    {
        public ScriptedLCDDisplayContentPanel()
        {
            InitializeComponent();
        }

        public String GetScript()
        {
            return richTextBox1.Text;
        }

        public void SetScript(string script)
        {
            richTextBox1.Text = script;
        }
    }
}
