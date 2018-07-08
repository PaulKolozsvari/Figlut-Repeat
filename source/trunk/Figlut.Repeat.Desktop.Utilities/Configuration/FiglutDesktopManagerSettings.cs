namespace Figlut.Repeat.Desktop.Utilities.Configuration
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Server.Toolkit.Utilities.SettingsFile;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Serialization;
    using Figlut.Server.Toolkit.Utilities.Logging;

    #endregion //Using Directives

    public class FiglutDesktopManagerSettings : Settings
    {
        #region Properties

        #region UI

        public int FormBarHeight { get; set; }

        public bool StartInFullScreen { get; set; }

        #endregion //UI

        #region Web Service

        public string WebServiceBaseUrl { get; set; }

        public int WebServiceRequestTimeout { get; set; }

        public TextEncodingType WebServiceTextResponseEncoding { get; set; }

        public SerializerType WebServiceMessagingFormat { get; set; }

        #endregion //Web Service

        #region Logging

        public bool LogToFile { get; set; }

        public bool LogToWindowsEventLog { get; set; }

        public bool LogToConsole { get; set; }

        public string LogFileName { get; set; }

        public string EventSourceName { get; set; }

        public string EventLogName { get; set; }

        public LoggingLevel LoggingLevel { get; set; }

        #endregion //Logging

        #endregion //Properties

        #region Methods

        public static FiglutDesktopManagerSettings CreateDefaultSettings()
        {
            return new FiglutDesktopManagerSettings()
            {
                ShowMessageBoxOnException = true,
                FormBarHeight = 21,
                StartInFullScreen = true,
                WebServiceBaseUrl = "http://127.0.0.1:2983/FiglutRepeat",
                WebServiceRequestTimeout = 30000,
                WebServiceTextResponseEncoding = TextEncodingType.UTF8,
                WebServiceMessagingFormat = SerializerType.JSON,
                LogToFile = false,
                LogToWindowsEventLog = true,
                LogToConsole = false,
                LogFileName = "FRepeat.Log.txt",
                EventSourceName = "FRepeat.Source",
                EventLogName = "FRepeat.Log",
                LoggingLevel = LoggingLevel.Normal
            };
        }

        #endregion //Methods
    }
}
