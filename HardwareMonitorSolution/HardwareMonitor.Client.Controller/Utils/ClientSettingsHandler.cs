﻿using HardwareMonitor.Domain.Utils;
using Microsoft.Win32;
using System.Windows.Forms;

namespace HardwareMonitor.Client.Controller.Utils
{
    class ClientSettingsHandler
    {
        // The path to the key where Windows looks for startup applications
        private const string _STARTUP_APPLICATIONS_KEY = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string _STARTUP_APPLICATION_NAME = "HardwareMonitorCLient";
        private const string _DEFAULT_CLIENT_SETTINGS_KEY = "hardwaremonitorclientsettings";

        private const string _RUN_AT_STARTUP_KEY = "runatstartup";
        private const bool _DEFAULT_RUN_AT_STARTUP = false;

        private const string _STARTUP_NOTIFICATION_KEY = "startupnotification";
        private const bool _DEFAULT_STARTUP_NOTIFICATION = true;

        private bool _runAtStartup;
        public bool RunAtStartup
        {
            get
            {
                return _runAtStartup;
            }
            set
            {
                _runAtStartup = value;
                _settings.Set(_RUN_AT_STARTUP_KEY, _runAtStartup);
            }
        }

        private bool _startupNotification;
        public bool StartupNotification
        {
            get
            {
                return _startupNotification;
            }
            set
            {
                _startupNotification = value;
                _settings.Set(_STARTUP_NOTIFICATION_KEY, _startupNotification);
                SetAutorun(_startupNotification);
            }
        }

        private SettingsStorage _settings;

        public ClientSettingsHandler(string key = _DEFAULT_CLIENT_SETTINGS_KEY)
        {
            _settings = new SettingsStorage(key);
            Update();
        }

        public void Update()
        {
            string value;
            if (!bool.TryParse(value = _settings.Get(_RUN_AT_STARTUP_KEY).ToString(), out _runAtStartup))
                _runAtStartup = _DEFAULT_RUN_AT_STARTUP;

            if (!bool.TryParse(value = _settings.Get(_STARTUP_NOTIFICATION_KEY).ToString(), out _startupNotification))
                _startupNotification = _DEFAULT_STARTUP_NOTIFICATION;
        }

        private void SetAutorun(bool autorun)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(_STARTUP_APPLICATIONS_KEY, true))
            {
                if (autorun) key.SetValue(_STARTUP_APPLICATION_NAME, "\"" + Application.ExecutablePath + "\"");
                else if (key.GetValue(_STARTUP_APPLICATIONS_KEY) != null) key.DeleteValue(_STARTUP_APPLICATION_NAME);
            }
        }
    }
}
