using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using WpfApp1.Common;
using Newtonsoft;
using System.Threading;
using System.Diagnostics;
using System.Security.Permissions;

namespace WpfApp1.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        Thread _thread;

        #region string const
        private const string strSuccessfully = "Successfully!";
        private const string strEmptyFile = "Empty file!";
        private const string strFileNoExist = "File no exist!";
        #endregion

        #region Constructor
        private MainViewModel()
        {
            
        }
        #endregion

        #region SingleTon
        private static readonly MainViewModel _instance = new MainViewModel();
        public static MainViewModel Instance
        {
            private set { }
            get { return _instance; }
        }
        #endregion

        #region Property
        private bool _isCCIConfigSuccessfully;
        public bool IsCCIConfigSuccessfully
        {
            set
            {
                _isCCIConfigSuccessfully = value;
                OnPropertyChanged("IsCCIConfigSuccessfully");
            }
            get { return _isCCIConfigSuccessfully; }
        }
        private bool _isAOIGUISetupSuccessfully;
        public bool IsAOIGUISetupSuccessfully
        {
            set
            {
                _isAOIGUISetupSuccessfully = value;
                OnPropertyChanged("IsAOIGUISetupSuccessfully");
            }
            get { return _isAOIGUISetupSuccessfully; }
        }
        private bool _isVisionConfigSuccessfully;
        public bool IsVisionConfigSuccessfully
        {
            set
            {
                _isVisionConfigSuccessfully = value;
                OnPropertyChanged("IsVisionConfigSuccessfully");
            }
            get { return _isVisionConfigSuccessfully; }
        }
        private string _readStatusFileCCIConfig = "...";
        public string ReadStatusFileCCIConfig
        {
            set
            {
                _readStatusFileCCIConfig = value;
                OnPropertyChanged("ReadStatusFileCCIConfig");
            }
            get { return _readStatusFileCCIConfig; }
        }
        private string _readStatusFileAOIGUISetup = "...";
        public string ReadStatusFileAOIGUISetup
        {
            set
            {
                _readStatusFileAOIGUISetup = value;
                OnPropertyChanged("ReadStatusFileAOIGUISetup");
            }
            get { return _readStatusFileAOIGUISetup; }
        }
        private string _readStatusFileVisionConfig = "...";
        public string ReadStatusFileVisionConfig
        {
            set
            {
                _readStatusFileVisionConfig = value;
                OnPropertyChanged("ReadStatusFileVisionConfig");
            }
            get { return _readStatusFileVisionConfig; }
        }

        private bool _isRunningGUI;
        public bool IsRunningGUI
        {
            set
            {
                _isRunningGUI = value;
                OnPropertyChanged(nameof(IsRunningGUI));
            }
            get { return _isRunningGUI; }
        }

        private bool _isRunningLogicConfig;
        public bool IsRunningLogicConfig
        {
            set
            {
                _isRunningLogicConfig = value;
                OnPropertyChanged(nameof(IsRunningLogicConfig));
            }
            get { return _isRunningLogicConfig; }
        }

        private bool _isRunningLogic;
        public bool IsRunningLogic
        {
            set
            {
                _isRunningLogic = value;
                OnPropertyChanged(nameof(IsRunningLogic));
            }
            get { return _isRunningLogic; }
        }
        #endregion

        #region Method
        public bool ReadXml()
        {
            if (File.Exists(FilePath.FilePath_CCIConfig))
            {
                try
                {
                    XmlTextReader xml = new XmlTextReader(FilePath.FilePath_CCIConfig);
                    while (xml.Read())
                    {
                        if (xml.IsEmptyElement)
                        {
                            IsCCIConfigSuccessfully = false;
                            return IsCCIConfigSuccessfully;
                        }
                    }

                    IsCCIConfigSuccessfully = true;
                    ReadStatusFileCCIConfig = strSuccessfully;

                    return IsCCIConfigSuccessfully;
                }
                catch (Exception ex)
                {
                    IsCCIConfigSuccessfully = false;
                    ReadStatusFileCCIConfig = $"{strEmptyFile} \"{ex.Message}\"";

                    return IsCCIConfigSuccessfully;
                }
            }
            else
            {
                IsCCIConfigSuccessfully = false;
                ReadStatusFileCCIConfig = strFileNoExist;

                return IsCCIConfigSuccessfully;
            }
        }

        public bool ReadIni()
        {
            if (File.Exists(FilePath.FilePath_AOIGUISetup))
            {
                List<Dictionary<string, string>> lstDic = IniFile.GetAllKeysWithAllSection();

                if (lstDic.Count > 0)
                {
                    IsAOIGUISetupSuccessfully = true;
                    ReadStatusFileAOIGUISetup = strSuccessfully;
                    return IsAOIGUISetupSuccessfully;
                }
                else
                {
                    IsAOIGUISetupSuccessfully = false;

                    ReadStatusFileAOIGUISetup = strEmptyFile;
                    return IsAOIGUISetupSuccessfully;
                }
            }
            else
            {
                IsAOIGUISetupSuccessfully = false;
                ReadStatusFileAOIGUISetup = strFileNoExist;
                return IsAOIGUISetupSuccessfully;
            }
        }

        public bool ReadJson()
        {
            if (File.Exists(FilePath.FilePath_VisionConfig))
            {
                StreamReader reader = new StreamReader(FilePath.FilePath_VisionConfig);
                string jsonString = reader.ReadToEnd();
                if (!String.IsNullOrEmpty(jsonString))
                {
                    IsVisionConfigSuccessfully = true;
                    ReadStatusFileVisionConfig = strSuccessfully;
                    return IsVisionConfigSuccessfully;
                }
                else
                {
                    IsVisionConfigSuccessfully = false;
                    ReadStatusFileVisionConfig = strEmptyFile;
                    return IsVisionConfigSuccessfully;
                }
            }
            else
            {
                IsVisionConfigSuccessfully = false;
                ReadStatusFileVisionConfig = strFileNoExist;
                return IsVisionConfigSuccessfully;
            }
        }

        public void CheckProcesses()
        {
            while (_thread.IsAlive)
            {
                try
                {
                    //KohyoungGUI, AOIConfig, AOIGUI

                    Process[] processes1 = Process.GetProcessesByName("KohyoungGUI");

                    Process[] processes2 = Process.GetProcessesByName("AOIConfig");

                    Process[] processes3 = Process.GetProcessesByName("AOIGUI");


                    if (processes1.Length > 0) IsRunningGUI = true;
                    if (processes2.Length > 0) IsRunningLogicConfig = true;
                    if (processes3.Length > 0) IsRunningLogic = true;


                    //1.
                    if (processes2.Length > 0)
                    {
                        foreach (var item in processes2)
                        {
                            if (item.ProcessName.Equals("AOIConfig"))
                            {
                                //MessageBox.Show("Logic Config is running! Cannot open");
                                IsRunningLogicConfig = true;
                            }
                        }
                    }

                    //2.
                    if (processes1.Length == 0 && processes3.Length == 0)
                    {
                        Process.Start(FilePath.StartLogic);
                        Thread.Sleep(2);
                    }


                    //3.
                    if (processes1.Length == 0)
                    {
                        if (processes3.Length == 0)
                        {
                            Process.Start(FilePath.StartLogic);
                            Thread.Sleep(2);
                        }
                    }


                    //4.
                    if (processes3.Length > 0 && processes1.Length == 0)
                    {
                        if (processes2.Length > 0)
                        {
                            foreach (var item in processes2)
                            {
                                if (item.ProcessName.Equals("AOIConfig"))
                                {
                                    item.Kill();
                                    Thread.Sleep(2);
                                }
                            }
                        }
                    }


                    Thread.Sleep(5);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public bool Run()
        {
            _thread = new Thread(new ThreadStart(CheckProcesses));
            _thread.IsBackground = true;
            _thread.Start();

            if (_thread.IsAlive)
                return true;
            else
                return false;

        }

        public void Stop()
        {
            if (_thread.IsAlive)
                _thread.Interrupt();
        }
        #endregion
    }
}
