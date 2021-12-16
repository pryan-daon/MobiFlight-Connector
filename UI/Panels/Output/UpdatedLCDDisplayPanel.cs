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
    public partial class UpdatedLCDDisplayPanel : UserControl
    {
        SimpleLCDDisplayContentPanel simpleLCDDisplayContentPanel = new SimpleLCDDisplayContentPanel();

        List<UserControl> displayPanels = new List<UserControl>();

        int Cols = 16;
        int Lines = 2;

        public UpdatedLCDDisplayPanel()
        {
            InitializeComponent();

            displayPanels.Add(simpleLCDDisplayContentPanel);
        }

        private void lcdDisplayContentTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = lcdDisplayContentTypeComboBox.SelectedIndex;

            foreach (UserControl p in displayPanels)
            {
                p.Enabled = false;
                p.AutoSize = false;
                p.Height = 0;
            }

            switch (selectedIndex)
            {
                case 0:
                    simpleLCDDisplayContentPanel.Enabled = true;
                    simpleLCDDisplayContentPanel.AutoSize = true;
                    //simpleLCDDisplayContentPanel.Height = 200;
                    break;
                default: 
                    Log.Instance.log("Unsupported LCD content type index: " + selectedIndex, LogSeverity.Error);
                    break;
            }
        }

        private void DisplayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cols = int.Parse(((sender as ComboBox).SelectedValue.ToString()).Split(',').ElementAt(1));
            Lines = int.Parse(((sender as ComboBox).SelectedValue.ToString()).Split(',').ElementAt(2));
            simpleLCDDisplayContentPanel.lcdDisplayTextBox.Width = 4 + (Cols * 8);
            simpleLCDDisplayContentPanel.lcdDisplayTextBox.Height = Lines * 16;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            // check if this is currently selected and properly initialized
            if (DisplayComboBox.SelectedValue == null) return config;

            config.LcdDisplay.Address = DisplayComboBox.SelectedValue.ToString().Split(',').ElementAt(0);

            config.LcdDisplay.Lines.Clear();
            foreach (String line in simpleLCDDisplayContentPanel.lcdDisplayTextBox.Lines)
            {
                config.LcdDisplay.Lines.Add(line);
            }
            return config;
        }

        public void SetAddresses(List<ListItem> ports)
        {
            DisplayComboBox.DataSource = new List<ListItem>(ports);
            DisplayComboBox.DisplayMember = "Label";
            DisplayComboBox.ValueMember = "Value";
            if (ports.Count > 0)
                DisplayComboBox.SelectedIndex = 0;

            DisplayComboBox.Enabled = ports.Count > 0;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // preselect display stuff
            if (config.LcdDisplay.Address != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(DisplayComboBox, config.LcdDisplay.Address.ToString()))
                {
                    // TODO: provide error message
                    Log.Instance.log("_syncConfigToForm : Exception on selecting item in LCD Address ComboBox", LogSeverity.Debug);
                }
            }
            if (config.LcdDisplay.Lines.Count > 0)
                simpleLCDDisplayContentPanel.lcdDisplayTextBox.Lines = config.LcdDisplay.Lines.ToArray();
        }
    }
}
