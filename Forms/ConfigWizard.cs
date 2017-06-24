﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;

namespace MobiFlight
{
    public partial class ConfigWizard : Form
    {
        public event EventHandler PreconditionTreeNodeChanged;

        static int lastTabActive = 0;

        ExecutionManager _execManager = null;
        int displayPanelHeight = -1;
        List<UserControl> displayPanels = new List<UserControl>();
        OutputConfigItem config = null;
        ErrorProvider errorProvider = new ErrorProvider();
        Dictionary<String, String> arcazeFirmware = new Dictionary<String, String>();
        DataSet _dataSetConfig = null;

        Dictionary<string, ArcazeModuleSettings> moduleSettings;

        Panels.DisplayPinPanel              displayPinPanel             = new Panels.DisplayPinPanel();
        Panels.DisplayBcdPanel              displayBcdPanel             = new Panels.DisplayBcdPanel();
        Panels.DisplayLedDisplayPanel       displayLedDisplayPanel      = new Panels.DisplayLedDisplayPanel();
        Panels.DisplayNothingSelectedPanel  displayNothingSelectedPanel = new Panels.DisplayNothingSelectedPanel();
        Panels.LCDDisplayPanel              displayLcdDisplayPanel      = new Panels.LCDDisplayPanel();
        Panels.ServoPanel                   servoPanel                  = new Panels.ServoPanel();
        Panels.StepperPanel                 stepperPanel                = new Panels.StepperPanel();
        

        public ConfigWizard(ExecutionManager mainForm, 
                             OutputConfigItem cfg, 
                             ArcazeCache arcazeCache, 
                             Dictionary<string, ArcazeModuleSettings> moduleSettings, 
                             DataSet dataSetConfig, 
                             String filterGuid)
        {
            this.moduleSettings = moduleSettings;
            Init(mainForm, cfg);            
            initWithArcazeCache(arcazeCache);
            preparePreconditionPanel(dataSetConfig, filterGuid);            
        }

        protected void Init(ExecutionManager mainForm, OutputConfigItem cfg)
        {
            this._execManager = mainForm;
            config = cfg;
            InitializeComponent();
            comparisonSettingsPanel.Enabled = false;
            
            // if one opens the dialog for a new config
            // ensure that always the first tab is shown
            if (cfg.FSUIPCOffset == OutputConfigItem.FSUIPCOffsetNull)
            {
                lastTabActive = 0;
            }
            tabControlFsuipc.SelectedIndex = lastTabActive;

            _initDisplayPanels();
            _initPreconditionPanel();
            fsuipcConfigPanel.setMode(true);
            fsuipcConfigPanel.syncFromConfig(cfg);
            
            // displayLedDisplayComboBox.Items.Clear(); 
        }

        private void _initPreconditionPanel()
        {
            preConditionTypeComboBox.Items.Clear();
            List<ListItem> preconTypes = new List<ListItem>() {
                new ListItem() { Value = "none",    Label = MainForm._tr("LabelPrecondition_None") },
                new ListItem() { Value = "config",  Label = MainForm._tr("LabelPrecondition_ConfigItem") },
                new ListItem() { Value = "pin",     Label = MainForm._tr("LabelPrecondition_ArcazePin") }
            };
            preConditionTypeComboBox.DataSource = preconTypes;
            preConditionTypeComboBox.DisplayMember = "Label";
            preConditionTypeComboBox.ValueMember = "Value";
            preConditionTypeComboBox.SelectedIndex = 0;

            preconditionConfigComboBox.SelectedIndex = 0;
            preconditionRefOperandComboBox.SelectedIndex = 0;

            // init the pin-type config panel
            List<ListItem> preconPinValues = new List<ListItem>() {
                new ListItem() { Value = "0", Label = "Off" },
                new ListItem() { Value = "1", Label = "On" },                
            };

            preconditionPinValueComboBox.DataSource = preconPinValues;
            preconditionPinValueComboBox.DisplayMember = "Label";
            preconditionPinValueComboBox.ValueMember = "Value";
            preconditionPinValueComboBox.SelectedIndex = 0;

            preconditionSettingsPanel.Enabled = false;
            preconditionApplyButton.Visible = false;
        }

        protected void _initDisplayPanels () {
            // make all panels small and store the common height
            groupBoxDisplaySettings.Controls.Add(displayPinPanel);
            displayPinPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayBcdPanel);
            displayBcdPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayLedDisplayPanel);
            displayLedDisplayPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayNothingSelectedPanel);
            displayNothingSelectedPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(servoPanel);
            servoPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(stepperPanel);
            stepperPanel.Dock = DockStyle.Top;
            stepperPanel.OnManualCalibrationTriggered += new EventHandler<Panels.ManualCalibrationTriggeredEventArgs>(stepperPanel_OnManualCalibrationTriggered);
            stepperPanel.OnSetZeroTriggered += new EventHandler(stepperPanel_OnSetZeroTriggered);


            groupBoxDisplaySettings.Controls.Add(displayLcdDisplayPanel);
            displayLcdDisplayPanel.AutoSize = false;
            displayLcdDisplayPanel.Height = 0;
            displayLcdDisplayPanel.Dock = DockStyle.Top;

            displayPanels.Clear();
            displayPanelHeight = 0;
            displayPanels.Add(displayPinPanel);
            displayPanels.Add(displayBcdPanel);
            displayPanels.Add(displayLedDisplayPanel);
            displayPanels.Add(displayNothingSelectedPanel);
            displayPanels.Add(servoPanel);
            displayPanels.Add(stepperPanel);
            displayPanels.Add(displayLcdDisplayPanel);

            foreach (UserControl p in displayPanels)
            {
                if (p.Height > 0 && (p.Height > displayPanelHeight)) displayPanelHeight = p.Height;                
                p.Height = 0;
            } //foreach

            displayNothingSelectedPanel.Height = displayPanelHeight;
            displayNothingSelectedPanel.Enabled = true;
        }

        void stepperPanel_OnSetZeroTriggered(object sender, EventArgs e)
        {
            _syncFormToConfig();
            String serial = config.DisplaySerial;
            if (serial.Contains('/'))
            {
                serial = serial.Split('/')[1].Trim();
            }
            _execManager.getMobiFlightModuleCache().resetStepper(serial, config.StepperAddress);
        }

        void stepperPanel_OnManualCalibrationTriggered(object sender, Panels.ManualCalibrationTriggeredEventArgs e)
        {
            _syncFormToConfig();
            int steps = e.Steps;
            
            String serial = config.DisplaySerial;
            if (serial == null) return;

            if (serial.Contains('/'))
            {
                serial = serial.Split('/')[1].Trim();
            }

            MobiFlightStepper stepper = _execManager.getMobiFlightModuleCache()
                                                    .GetModuleBySerial(serial)
                                                    .GetStepper(config.StepperAddress);

            int CurrentValue = stepper.Position();
            int NextValue = (CurrentValue + e.Steps);

            _execManager.getMobiFlightModuleCache().setStepper(
                serial,
                config.StepperAddress,
                (NextValue).ToString(),
                Int16.Parse(config.StepperOutputRev),
                Int16.Parse(config.StepperOutputRev),
                config.StepperCompassMode
            );
            
        }        

        private void preparePreconditionPanel(DataSet dataSetConfig, String filterGuid)
        {
            _dataSetConfig = dataSetConfig;
            DataRow[] rows = dataSetConfig.Tables["config"].Select("guid <> '" + filterGuid +"'");         
   
            // this filters the current config
            DataView dv = new DataView (dataSetConfig.Tables["config"]);
            dv.RowFilter = "guid <> '" + filterGuid + "'";
            preconditionConfigComboBox.DataSource = dv;
            preconditionConfigComboBox.ValueMember = "guid";
            preconditionConfigComboBox.DisplayMember = "description";

            if (preconditionConfigComboBox.Items.Count == 0)
            {
                List<ListItem> preconTypes = new List<ListItem>() {
                new ListItem() { Value = "none",    Label = MainForm._tr("LabelPrecondition_None") },
                new ListItem() { Value = "pin",     Label = MainForm._tr("LabelPrecondition_ArcazePin") }
                };
                preConditionTypeComboBox.DataSource = preconTypes;
                preConditionTypeComboBox.DisplayMember = "Label";
                preConditionTypeComboBox.ValueMember = "Value";
                preConditionTypeComboBox.SelectedIndex = 0;
            }

            displayLcdDisplayPanel.SetConfigRefsDataView(dv, filterGuid);
        }

        /// <summary>
        /// sync the config wizard with the provided settings from arcaze cache such as available modules, ports, etc.
        /// </summary>
        /// <param name="arcazeCache"></param>
        public void initWithArcazeCache (ArcazeCache arcazeCache)
        {
            
            // update the display box with
            // modules
            displayModuleNameComboBox.Items.Clear();
            preconditionPinSerialComboBox.Items.Clear();
            displayModuleNameComboBox.Items.Add("-");
            preconditionPinSerialComboBox.Items.Add("-");

            foreach (IModuleInfo module in arcazeCache.getModuleInfo())
            {
                arcazeFirmware[module.Serial] = module.Version;
                displayModuleNameComboBox.Items.Add(module.Name + "/ " + module.Serial);
                preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }
#if MOBIFLIGHT
            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().getModuleInfo())
            {
                displayModuleNameComboBox.Items.Add(module.Name + "/ " + module.Serial);

                // Not yet supported for pins
                // preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }
#endif
            displayModuleNameComboBox.SelectedIndex = 0;
            preconditionPinSerialComboBox.SelectedIndex = 0;            
        }

        /// <summary>
        /// sync the values from config with the config wizard form
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected bool _syncConfigToForm(OutputConfigItem config)
        {
            string serial = null;
            if (config == null) throw new Exception(MainForm._tr("uiException_ConfigItemNotFound"));

            _syncFsuipcTabFromConfig(config);

            _syncComparisonTabFromConfig(config);
            
            // third tab
            if (!ComboBoxHelper.SetSelectedItem(displayTypeComboBox, config.DisplayType))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Display Type ComboBox", LogSeverity.Debug);
            }

            if (config.DisplaySerial != null && config.DisplaySerial != "")
            {
                serial = config.DisplaySerial;
                if (serial.Contains('/'))
                {
                    serial = serial.Split('/')[1].Trim();
                }
                if (!ComboBoxHelper.SetSelectedItemByPart(displayModuleNameComboBox, serial))
                {
                    // TODO: provide error message
                }
            }


            displayPinPanel.syncFromConfig(config);

            displayBcdPanel.syncFromConfig(config);

            displayLedDisplayPanel.syncFromConfig(config);

            servoPanel.syncFromConfig(config);

            stepperPanel.syncFromConfig(config);

            displayLcdDisplayPanel.syncFromConfig(config);
            
            preconditionListTreeView.Nodes.Clear();

            foreach (Precondition p in config.Preconditions)
            {
                TreeNode tmpNode = new TreeNode();
                tmpNode.Text = p.ToString();
                tmpNode.Tag = p;
                tmpNode.Checked = p.PreconditionActive;
                try
                {
                    _updateNodeWithPrecondition(tmpNode, p);
                    preconditionListTreeView.Nodes.Add(tmpNode);
                }
                catch (IndexOutOfRangeException e)
                {
                    Log.Instance.log("An orphaned precondition has been found", LogSeverity.Error);
                    continue;
                }                
            }

            overridePreconditionCheckBox.Checked = config.Preconditions.ExecuteOnFalse;
            overridePreconditionTextBox.Text = config.Preconditions.FalseCaseValue;

            if (preconditionListTreeView.Nodes.Count == 0)
            {
                _addEmptyNodeToTreeView();
            }

            return true;
        }

        private void _syncComparisonTabFromConfig(OutputConfigItem config)
        {
            // second tab
            comparisonActiveCheckBox.Checked = config.ComparisonActive;
            comparisonValueTextBox.Text = config.ComparisonValue;

            if (!ComboBoxHelper.SetSelectedItem(comparisonOperandComboBox, config.ComparisonOperand))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Comparison ComboBox", LogSeverity.Debug);
            }
            comparisonIfValueTextBox.Text = config.ComparisonIfValue;
            comparisonElseValueTextBox.Text = config.ComparisonElseValue;

            interpolationCheckBox.Checked = config.Interpolation.Active;
            interpolationPanel1.syncFromConfig(config.Interpolation);
        }

        private void _syncFsuipcTabFromConfig(OutputConfigItem config)
        {
            fsuipcConfigPanel.syncFromConfig(config);
        }

        private void _addEmptyNodeToTreeView()
        {
            TreeNode tmpNode = new TreeNode();
            Precondition p = new Precondition();

            tmpNode.Text = p.ToString();
            tmpNode.Tag = p;
            tmpNode.Checked = p.PreconditionActive;            
            _updateNodeWithPrecondition(tmpNode, p);
            config.Preconditions.Add(p);
            preconditionListTreeView.Nodes.Add(tmpNode);
        }
        
        /// <summary>
        /// sync current status of form values to config
        /// </summary>
        /// <returns></returns>
        protected bool _syncFormToConfig()
        {
            fsuipcConfigPanel.syncToConfig(config);

            // comparison panel
            config.ComparisonActive = comparisonActiveCheckBox.Checked;
            config.ComparisonValue = comparisonValueTextBox.Text;
            config.ComparisonOperand = comparisonOperandComboBox.Text;
            config.ComparisonIfValue = comparisonIfValueTextBox.Text;
            config.ComparisonElseValue = comparisonElseValueTextBox.Text;

            config.Interpolation.Active = interpolationCheckBox.Checked;
            interpolationPanel1.syncToConfig(config.Interpolation);

            config.DisplayType = displayTypeComboBox.Text;
            config.DisplayTrigger = "normal";
            config.DisplaySerial = displayModuleNameComboBox.Text;

            // sync the two properties that are not part of the preconditions list

            config.Preconditions.ExecuteOnFalse = overridePreconditionCheckBox.Checked;
            config.Preconditions.FalseCaseValue = overridePreconditionTextBox.Text;

            // sync panels
            displayPinPanel.syncToConfig(config);
            
            displayLedDisplayPanel.syncToConfig(config);

            displayBcdPanel.syncToConfig(config);

            servoPanel.syncToConfig(config);
            
            stepperPanel.syncToConfig(config);

            displayLcdDisplayPanel.syncToConfig(config);
            
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _testModeStop();
            try {
                if (!ValidateChildren())
                {
                    return;
                }
            } catch (System.InvalidOperationException eOp)
            {
                Log.Instance.log("ConfigWizard:button1_Click: " + eOp.Message, LogSeverity.Debug);
            }
            _syncFormToConfig();
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _testModeStop();
            DialogResult = DialogResult.Cancel;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void displaySerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            Log.Instance.log("displaySerialComboBox_SelectedIndexChanged: called.", LogSeverity.Debug);
#endif
            // check which extension type is available to current serial
            ComboBox cb = (sender as ComboBox);

            try
            {
                // disable test button
                // in case that no display is selected                
                String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());

                displayTypeComboBox.Enabled = groupBoxDisplaySettings.Enabled = testSettingsGroupBox.Enabled = (serial != "");
                // serial is empty if no module is selected (on init of form)
                //if (serial == "") return;                

                // update the available types depending on the 
                // type of module
                if (serial.IndexOf("SN") != 0)
                {
                    displayTypeComboBox.Items.Clear();
                    displayTypeComboBox.Items.Add("Pin");
                    displayTypeComboBox.Items.Add(ArcazeLedDigit.TYPE);
                    //displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                }
                else
                {
                    displayTypeComboBox.Items.Clear();
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);
                    foreach (DeviceType devType in module.GetConnectedOutputDeviceTypes())
                    {
#if DEBUG
                        Log.Instance.log("displaySerialComboBox_SelectedIndexChanged: Adding Device Type: " + devType.ToString(), LogSeverity.Debug);
#endif
                        switch (devType)
                        {
                            case DeviceType.LedModule:
                                displayTypeComboBox.Items.Add(ArcazeLedDigit.TYPE);
                                break;

                            case DeviceType.Output:
                                displayTypeComboBox.Items.Add("Pin");
                                //displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                                break;

                            case DeviceType.Servo:
                                displayTypeComboBox.Items.Add(DeviceType.Servo.ToString("F"));
                                break;

                            case DeviceType.Stepper:
                                displayTypeComboBox.Items.Add(DeviceType.Stepper.ToString("F"));
                                break;

                            case DeviceType.LcdDisplay:
                                displayTypeComboBox.Items.Add(DeviceType.LcdDisplay.ToString("F"));
                                break;
                        }
                    }
                }

                // third tab
                if (!ComboBoxHelper.SetSelectedItem(displayTypeComboBox, config.DisplayType))
                {
                    // TODO: provide error message
                    Log.Instance.log("displayArcazeSerialComboBox_SelectedIndexChanged : Problem setting Display Type ComboBox", LogSeverity.Debug);
                }

            }
            catch (Exception ex)
            {
                displayPinPanel.displayPinBrightnessPanel.Visible = false;
                displayPinPanel.displayPinBrightnessPanel.Enabled = false;
                Log.Instance.log("displayArcazeSerialComboBox_SelectedIndexChanged : Some Exception occurred" + ex.Message, LogSeverity.Debug);
            }
        }

        private void displayTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Log.Instance.log("displayTypeComboBox_SelectedIndexChanged: called.", LogSeverity.Debug);

            foreach (UserControl p in displayPanels)
            {
                p.Enabled = false;
                p.AutoSize = false;
                p.Height = 0;
            } //foreach

            try
            {
                bool panelEnabled = true;
                // get the deviceinfo for the current arcaze
                ComboBox cb = displayModuleNameComboBox;                
                String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());

                // we remove the callback method to ensure, that it is not added more than once
                displayLedDisplayPanel.displayLedAddressComboBox.SelectedIndexChanged -= displayLedAddressComboBox_SelectedIndexChanged;

                if (arcazeFirmware.ContainsKey(serial))
                {

                    switch ((sender as ComboBox).SelectedItem.ToString())
                    {
                        case "DisplayDriver":
                            panelEnabled = ushort.Parse(arcazeFirmware[serial]) > 0x529;
                            break;

                        case "LedDriver2":
                            panelEnabled = ushort.Parse(arcazeFirmware[serial]) > 0x554;
                            break;

                        case "LedDriver3":
                            panelEnabled = ushort.Parse(arcazeFirmware[serial]) > 0x550;
                            break;
                    }

                    displayPinPanel.displayPinBrightnessPanel.Visible = (moduleSettings[serial].type == SimpleSolutions.Usb.ArcazeCommand.ExtModuleType.LedDriver3);
                    displayPinPanel.displayPinBrightnessPanel.Enabled = (displayPinPanel.displayPinBrightnessPanel.Visible && (cb.SelectedIndex > 1));
                    
                    //preconditionPortComboBox.Items.Clear();
                    //preconditionPinComboBox.Items.Clear();

                    List<ListItem> ports = new List<ListItem>();                    

                    foreach (String v in ArcazeModule.getPorts())
                    {
                        ports.Add(new ListItem() { Label = v, Value = v });
                        if (v == "B" || v == "E" || v == "H" || v == "K")
                        {
                            ports.Add(new ListItem() { Label = "-----", Value = "-----" });
                        }

                        if (v == "A" || v == "B")
                        {
                            //preconditionPortComboBox.Items.Add(v);
                        }
                    }

                    displayPinPanel.SetPorts(ports);
                    displayBcdPanel.SetPorts(ports);

                    List<ListItem> pins = new List<ListItem>();
                    foreach (String v in ArcazeModule.getPins())
                    {
                        pins.Add(new ListItem() { Label = v, Value = v });
                        //preconditionPinComboBox.Items.Add(v);
                    }

                    displayPinPanel.SetPins(pins);
                    displayBcdPanel.SetPins(pins);
                    displayPinPanel.WideStyle = false;

                    List<ListItem> addr = new List<ListItem>();
                    List<ListItem> connectors = new List<ListItem>();
                    foreach (string v in ArcazeModule.getDisplayAddresses()) addr.Add(new ListItem() { Label = v, Value = v });
                    foreach (string v in ArcazeModule.getDisplayConnectors()) connectors.Add(new ListItem() { Label = v, Value = v });
                    displayLedDisplayPanel.WideStyle = false;
                    displayLedDisplayPanel.SetAddresses(addr);
                    displayLedDisplayPanel.SetConnectors(connectors);                    
                }
                else if (serial.IndexOf("SN") == 0)
                {
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);

                    displayPinPanel.displayPinBrightnessPanel.Visible = true;
                    displayPinPanel.displayPinBrightnessPanel.Enabled = (displayPinPanel.displayPinBrightnessPanel.Visible && (cb.SelectedIndex > 1));

                    List<ListItem> outputs = new List<ListItem>();
                    List<ListItem> ledSegments = new List<ListItem>();
                    List<ListItem> servos = new List<ListItem>();
                    List<ListItem> stepper = new List<ListItem>();
                    List<ListItem> lcdDisplays = new List<ListItem>();

                    foreach (IConnectedDevice device in module.GetConnectedDevices())
                    {
                        Log.Instance.log("displayTypeComboBox_SelectedIndexChanged: Adding connected device: " + device.Type.ToString() + ", " + device.Name, LogSeverity.Debug);
                        switch (device.Type)
                        {
                            case DeviceType.LedModule:
                                ledSegments.Add(new ListItem() { Value = device.Name, Label = device.Name });
                                break;

                            case DeviceType.Output:
                                outputs.Add(new ListItem() { Value = device.Name, Label = device.Name });
                                break;

                            case DeviceType.Servo:
                                servos.Add(new ListItem() { Value = device.Name, Label = device.Name });
                                break;

                            case DeviceType.Stepper:
                                stepper.Add(new ListItem() { Value = device.Name, Label = device.Name });
                                break;

                            case DeviceType.LcdDisplay:
                                int Cols = (device as MobiFlightLcdDisplay).Cols;
                                int Lines= (device as MobiFlightLcdDisplay).Lines;
                                lcdDisplays.Add(new ListItem() { Value = device.Name+","+ Cols+","+Lines, Label = device.Name });
                                break;
                        }                        
                    }
                    displayPinPanel.WideStyle = true;
                    displayPinPanel.SetPorts(new List<ListItem>());
                    displayPinPanel.SetPins(outputs);

                    displayLedDisplayPanel.WideStyle = true;
                    displayLedDisplayPanel.displayLedAddressComboBox.SelectedIndexChanged += new EventHandler(displayLedAddressComboBox_SelectedIndexChanged);
                    displayLedDisplayPanel.SetAddresses(ledSegments);

                    servoPanel.SetAdresses(servos);

                    stepperPanel.SetAdresses(stepper);

                    displayLcdDisplayPanel.SetAddresses(lcdDisplays);
                }
                if ((sender as ComboBox).Text == "Pin")
                {
                    displayPinPanel.Enabled = panelEnabled;
                    displayPinPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == ArcazeBcd4056.TYPE)
                {
                    displayBcdPanel.Enabled = panelEnabled;
                    displayBcdPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == ArcazeLedDigit.TYPE)
                {
                    displayLedDisplayPanel.Enabled = panelEnabled;
                    displayLedDisplayPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == DeviceType.Servo.ToString("F"))
                {
                    servoPanel.Enabled = panelEnabled;
                    servoPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == DeviceType.Stepper.ToString("F"))
                {
                    stepperPanel.Enabled = panelEnabled;
                    stepperPanel.Height = displayPanelHeight;
                }
                if ((sender as ComboBox).Text == DeviceType.LcdDisplay.ToString("F"))
                {
                    displayLcdDisplayPanel.Enabled = panelEnabled;
                    displayLcdDisplayPanel.AutoSize = true;
                    displayLcdDisplayPanel.Height = displayPanelHeight;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(MainForm._tr("uiMessageNotImplementedYet"), 
                                MainForm._tr("Hint"), 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
            }
        }

        void displayLedAddressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = displayModuleNameComboBox;                
            String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());
            MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);

            List<ListItem> connectors = new List<ListItem>();

            foreach (IConnectedDevice device in module.GetConnectedDevices())
            {
                if (device.Type != DeviceType.LedModule) continue;
                if (device.Name != ((sender as ComboBox).SelectedItem as ListItem).Value) continue;
                for (int i = 0; i< (device as MobiFlightLedModule).SubModules; i++) {
                    connectors.Add(new ListItem() { Label = (i + 1).ToString(), Value = (i + 1).ToString() });
                }
            }
            displayLedDisplayPanel.SetConnectors(connectors);
        }

        private void fsuipcOffsetTextBox_Validating(object sender, CancelEventArgs e)
        {
            _validatingHexFields(sender, e, 4);            
        }

        private void comparisonActiveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            comparisonSettingsPanel.Enabled = (sender as CheckBox).Checked;
        }

        private void ConfigWizard_Load(object sender, EventArgs e)
        {
            _syncConfigToForm(config);
        }
        
        private void fsuipcMultiplyTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                float.Parse((sender as TextBox).Text);
                removeError(sender as Control);
            }
            catch (Exception exc)
            {
                Log.Instance.log("fsuipcMultiplyTextBox_Validating : Parsing problem, " + exc.Message, LogSeverity.Debug);
                displayError(sender as Control, MainForm._tr("uiMessageFsuipcConfigPanelMultiplyWrongFormat"));
                e.Cancel = true;
            }
        }
        
        private void _validatingHexFields(object sender, CancelEventArgs e, int length)
        {
            try
            {
                string tmp = (sender as TextBox).Text.Replace("0x", "").ToUpper();
                (sender as TextBox).Text = "0x" + Int64.Parse(tmp, System.Globalization.NumberStyles.HexNumber).ToString("X" + length.ToString());
            }
            catch (Exception exc)
            {                
                e.Cancel = true;
                Log.Instance.log("_validatingHexFields : Parsing problem, " + exc.Message, LogSeverity.Debug);
                MessageBox.Show(MainForm._tr("uiMessageConfigWizard_ValidHexFormat"), MainForm._tr("Hint"));
            }
        }

        private void displayError(Control control, String message)
        {
            errorProvider.SetError(
                    control,
                    message);
            MessageBox.Show(message, MainForm._tr("Hint"));
        }

        private void removeError(Control control)
        {
            errorProvider.SetError(
                    control,
                    "");
        }

        private void displayArcazeSerialComboBox_Validating(object sender, CancelEventArgs e)
        {
            /* disabled this validation to permit configs even without module or
             * as precondition only
             
            if (displayArcazeSerialComboBox.Text.Trim() == "-")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = displayTabPage;
                displayArcazeSerialComboBox.Focus();
                displayError(displayArcazeSerialComboBox, MainForm._tr("uiMessageConfigWizard_SelectArcaze"));                
            }
            else
            {
               removeError(displayArcazeSerialComboBox);             
            }
             */
        }

        private void portComboBox_Validating(object sender, CancelEventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            if (!cb.Parent.Visible) return;
            if (null == cb.SelectedItem) return;
            if (cb.SelectedItem.ToString() == "-----")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = displayTabPage;
                cb.Focus();
                displayError(cb, MainForm._tr("Please_select_a_port"));
            }
            else
            {
                removeError(cb);
            }
        }

        private void displayLedDisplayComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (displayTypeComboBox.Text == ArcazeLedDigit.TYPE)                
            {                
                try
                {
                    int.Parse(displayLedDisplayPanel.displayLedAddressComboBox.Text);
                    removeError(displayLedDisplayPanel.displayLedAddressComboBox);
                }
                catch (Exception exc)
                {
                    Log.Instance.log("displayLedDisplayComboBox_Validating : Parsing problem, " + exc.Message, LogSeverity.Debug);
                    e.Cancel = true;
                    tabControlFsuipc.SelectedTab = displayTabPage;
                    displayLedDisplayPanel.displayLedAddressComboBox.Focus();
                    displayError(displayLedDisplayPanel.displayLedAddressComboBox, MainForm._tr("uiMessageConfigWizard_ProvideLedDisplayAddress"));
                }                
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value = Int16.Parse((sender as ComboBox).Text);
            for (int i = 0; i < 8; i++)
            {
                displayLedDisplayPanel.displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "CheckBox"].Visible = i < value;
                displayLedDisplayPanel.displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "CheckBox"].Visible = i < value;
                displayLedDisplayPanel.Controls["displayLedDisplayLabel" + i].Visible = i < value;

                // uncheck all invisible checkboxes to ensure correct mask
                if (i >= value)
                {
                    (displayLedDisplayPanel.displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "CheckBox"] as CheckBox).Checked = false;
                    (displayLedDisplayPanel.displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "CheckBox"] as CheckBox).Checked = false;
                }
            }
        }

        private void preConditionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = ((sender as ComboBox).SelectedItem as ListItem).Value;
            preconditionSettingsGroupBox.Visible = selected != "none";
            preconditionRuleConfigPanel.Visible = false;
            preconditionRuleConfigPanel.Visible = selected == "config";
            preconditionPinPanel.Visible = selected == "pin";
        }

        private void preconditionRuleConfigPanel_Validating(object sender, CancelEventArgs e)
        {            
        }
        
        private void preconditionRefValueTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionRuleConfigPanel).Visible)
            {
                removeError(preconditionRefValueTextBox);
                return;
            }

            if (preconditionRefValueTextBox.Text.Trim() == "")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                displayError(preconditionRefValueTextBox, MainForm._tr("uiMessageConfigWizard_SelectComparison"));
            }
            else
            {
                removeError(preconditionRefValueTextBox);
            }
        }

        private void preconditionPinSerialComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionRefValueTextBox);
                return;
            }

            if (preconditionPinSerialComboBox.Items.Count > 1 && preconditionPinSerialComboBox.Text.Trim() == "-")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                preconditionPinSerialComboBox.Focus();
                displayError(preconditionPinSerialComboBox, MainForm._tr("uiMessageConfigWizard_SelectArcaze"));
            }
            else
            {
                removeError(preconditionPinSerialComboBox);
            }

        }

        private void preconditionPinComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionPinComboBox);
                return;
            }

            if (preconditionPinSerialComboBox.SelectedIndex > 0 && preconditionPinComboBox.SelectedIndex == -1)
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                displayError(preconditionPinComboBox, MainForm._tr("Please_select_a_pin"));
            }
            else
            {
                removeError(preconditionPinComboBox);
            }
        }

        private void preconditionPortComboBox_Validating(object sender, CancelEventArgs e) {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionPortComboBox);
                return;
            }

            if (preconditionPinSerialComboBox.SelectedIndex > 0 && preconditionPortComboBox.SelectedIndex == -1)
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                displayError(preconditionPortComboBox, MainForm._tr("Please_select_a_port"));
            }
            else
            {
                removeError(preconditionPortComboBox);
            }
        }

        private void displayPinTestButton_Click(object sender, EventArgs e)
        {
            _testModeStart();
        }

        private void displayPinTestStopButton_Click(object sender, EventArgs e)
        {
            _testModeStop();
        }

        private void _testModeStart()
        {
            _syncFormToConfig();
            displayPinTestStopButton.Enabled = true;
            displayPinTestButton.Enabled = false;
            displayTypeGroupBox.Enabled = false;
            groupBoxDisplaySettings.Enabled = false;
            _execManager.executeTestOn(config);
        }

        private void _testModeStop()
        {
            // check if running in test mode otherwise simply return
            if (!displayPinTestStopButton.Enabled) return;

            displayPinTestStopButton.Enabled = false;
            displayPinTestButton.Enabled = true;
            displayTypeGroupBox.Enabled = true;
            groupBoxDisplaySettings.Enabled = true;
            _execManager.executeTestOff(config);
        }

        private void tabControlFsuipc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if running in test mode
            lastTabActive = (sender as TabControl).SelectedIndex;
            _testModeStop();
        }
        
        private void fsuipcOffsetTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //updateByteSizeComboBox();
        }

        /*
        private void updateByteSizeComboBox()
        {
            string selectedText = fsuipcSizeComboBox.Text;
            fsuipcSizeComboBox.Items.Clear();
            fsuipcSizeComboBox.Enabled = true;

            if (fsuipcOffsetTypeComboBox.SelectedValue.ToString() == FSUIPCOffsetType.Integer.ToString())
            {
                fsuipcSizeComboBox.Items.Add("1");
                fsuipcSizeComboBox.Items.Add("2");
                fsuipcSizeComboBox.Items.Add("4");
                fsuipcSizeComboBox.Items.Add("8");
                ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, selectedText);
            }
            else if (fsuipcOffsetTypeComboBox.SelectedValue.ToString() == FSUIPCOffsetType.Float.ToString())
            {                
                fsuipcSizeComboBox.Items.Add("4");
                fsuipcSizeComboBox.Items.Add("8");
                ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, selectedText);
            }
            else if (fsuipcOffsetTypeComboBox.SelectedValue.ToString() == FSUIPCOffsetType.String.ToString())
            {
                fsuipcSizeComboBox.Enabled = false;
            }
        }
        */
        
        private void preconditionListTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            preconditionListTreeView.SelectedNode = e.Node;
            //if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            Precondition config = (e.Node.Tag as Precondition);
            preConditionTypeComboBox.SelectedValue = config.PreconditionType;
            preconditionSettingsPanel.Enabled = true;
            preconditionApplyButton.Visible = true;
            config.PreconditionActive = e.Node.Checked;

            switch (config.PreconditionType)
            {
                case "config":
                    try
                    {                        
                        preconditionConfigComboBox.SelectedValue = config.PreconditionRef;
                    }
                    catch (Exception exc)
                    {
                        // precondition could not be loaded
                        Log.Instance.log("preconditionListTreeView_NodeMouseClick : Precondition could not be loaded, " + exc.Message, LogSeverity.Debug);
                    }

                    ComboBoxHelper.SetSelectedItem(preconditionRefOperandComboBox, config.PreconditionOperand);
                    preconditionRefValueTextBox.Text = config.PreconditionValue;
                    break;

                case "pin":
                    ArcazeIoBasic io = new ArcazeIoBasic(config.PreconditionPin);                    
                    ComboBoxHelper.SetSelectedItemByPart(preconditionPinSerialComboBox, config.PreconditionSerial);
                    preconditionPinValueComboBox.SelectedValue = config.PreconditionValue;
                    preconditionPortComboBox.SelectedIndex = io.Port;
                    preconditionPinComboBox.SelectedIndex = io.Pin;
                    break;
            }  

            aNDToolStripMenuItem.Checked = config.PreconditionLogic == "and";
            oRToolStripMenuItem.Checked = config.PreconditionLogic == "or";            
        }

        private void preconditionApplyButton_Click(object sender, EventArgs e)
        {
            // sync the selected node with the current settings from the panels
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            if (selectedNode == null) return;

            Precondition c = selectedNode.Tag as Precondition;
            
            c.PreconditionType = (preConditionTypeComboBox.SelectedItem as ListItem).Value;
            switch (c.PreconditionType)
            {
                case "config":
                    c.PreconditionRef = preconditionConfigComboBox.SelectedValue.ToString();
                    c.PreconditionOperand = preconditionRefOperandComboBox.Text;
                    c.PreconditionValue = preconditionRefValueTextBox.Text;
                    c.PreconditionActive = true;
                    break;
                
                case "pin":                    
                    c.PreconditionSerial = preconditionPinSerialComboBox.Text;
                    c.PreconditionValue = preconditionPinValueComboBox.SelectedValue.ToString();
                    c.PreconditionPin = preconditionPortComboBox.Text + preconditionPinComboBox.Text;
                    c.PreconditionActive = true;
                    break;                    
            }

            _updateNodeWithPrecondition(selectedNode, c);
        }    
    
        private void _updateNodeWithPrecondition (TreeNode node, Precondition p) 
        {
            String label = p.PreconditionLabel;
            if (p.PreconditionType == "config")
            {
                String replaceString = "[unknown]";
                if (_dataSetConfig != null)
                {
                    DataRow[] rows = _dataSetConfig.Tables["config"].Select("guid = '" + p.PreconditionRef + "'");
                    if (rows.Count() == 0) throw new IndexOutOfRangeException(); // an orphaned entry has been found
                    replaceString = rows[0]["description"] as String;
                }
                label = label.Replace("<Ref:" + p.PreconditionRef  + ">", replaceString);
            }
            else if (p.PreconditionType == "pin")
            {
                label = label.Replace("<Serial:" + p.PreconditionSerial + ">", p.PreconditionSerial.Split('/')[0]);
            }
            
            label = label.Replace("<Logic:and>", " (AND)").Replace("<Logic:or>", " (OR)");
            node.Checked = p.PreconditionActive;
            node.Tag = p;
            node.Text = label;
            aNDToolStripMenuItem.Checked = p.PreconditionLogic == "and";
            oRToolStripMenuItem.Checked = p.PreconditionLogic == "or";
        }

        private void addPreconditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Precondition p = new Precondition();
            TreeNode n = new TreeNode();
            n.Tag = p;
            config.Preconditions.Add(p);
            preconditionListTreeView.Nodes.Add(n);
            _updateNodeWithPrecondition(n, p);
        }

        private void andOrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            Precondition p = selectedNode.Tag as Precondition;
            if ((sender as ToolStripMenuItem).Text == "AND")
                p.PreconditionLogic = "and";
            else
                p.PreconditionLogic = "or";

            _updateNodeWithPrecondition(selectedNode, p);            
        }

        private void removePreconditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            Precondition p = selectedNode.Tag as Precondition;
            config.Preconditions.Remove(p);
            preconditionListTreeView.Nodes.Remove(selectedNode);
        }

        private void preconditionPinSerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the deviceinfo for the current arcaze
            ComboBox cb = preconditionPinSerialComboBox;
            String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());
            // if (serial == "" && config.DisplaySerial != null) serial = ArcazeModuleSettings.ExtractSerial(config.DisplaySerial);

            if (serial.IndexOf("SN") != 0)
            {
                preconditionPortComboBox.Items.Clear();
                preconditionPinComboBox.Items.Clear();

                List<ListItem> ports = new List<ListItem>();

                foreach (String v in ArcazeModule.getPorts())
                {
                    ports.Add(new ListItem() { Label = v, Value = v });
                    if (v == "B" || v == "E" || v == "H" || v == "K")
                    {
                        ports.Add(new ListItem() { Label = "-----", Value = "-----" });
                    }

                    if (v == "A" || v == "B")
                    {
                        preconditionPortComboBox.Items.Add(v);
                    }
                }

                List<ListItem> pins = new List<ListItem>();
                foreach (String v in ArcazeModule.getPins())
                {
                    pins.Add(new ListItem() { Label = v, Value = v });
                    preconditionPinComboBox.Items.Add(v);
                }
            }
        }

        private void interpolationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            interpolationPanel1.Enabled = (sender as CheckBox).Checked;
            if ((sender as CheckBox).Checked)
                interpolationPanel1.Save = true;
        }
    }
}
