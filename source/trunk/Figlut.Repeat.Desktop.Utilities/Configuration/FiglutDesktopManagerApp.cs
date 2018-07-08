namespace Figlut.Repeat.Desktop.Utilities.Configuration
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Utilities.Serialization;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Repeat.Desktop.Utilities.Web;
    using Figlut.Repeat.ORM;

    #endregion //Using Directives

    public class FiglutDesktopManagerApp
    {
        #region Singleton Setup

        private static FiglutDesktopManagerApp _instance;

        public static FiglutDesktopManagerApp Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FiglutDesktopManagerApp();
                }
                return _instance;
            }
        }

        #endregion //Singleton Setup

        #region Constructors

        private FiglutDesktopManagerApp()
        {
        }

        #endregion //Constructors

        #region Fields

        private FiglutDesktopManagerSettings _settings;
        private FiglutRepeatRestServiceClient _restClient;
        private User _currentUser;

        #endregion //Fields

        #region Properties

        public FiglutDesktopManagerSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GOC.Instance.GetSettings<FiglutDesktopManagerSettings>(true, true);
                }
                return _settings;
            }
        }

        public FiglutRepeatRestServiceClient RestClient
        {
            get
            {
                if (_restClient == null)
                {
                    _restClient = GetRestWebServiceClient(Settings, null, null, null);
                }
                return _restClient;
            }
        }

        public User CurrentUser
        {
            get { return _currentUser; }
        }

        #endregion //Properties

        #region Methods

        public void Initialize(bool wrapWebException, WaitProcess w)
        {
            _settings = Settings;
            GOC.Instance.ShowMessageBoxOnException = _settings.ShowMessageBoxOnException;
            w.ChangeStatus("Initializing logger ...");
            GOC.Instance.Logger = new Logger(
                _settings.LogToFile,
                _settings.LogToWindowsEventLog,
                _settings.LogToConsole,
                _settings.LoggingLevel,
                _settings.LogFileName,
                _settings.EventSourceName,
                _settings.EventLogName);
            _restClient = GetRestWebServiceClient(Settings, w, null, null);
        }

        public User Login(WaitProcess w, string userName, string password)
        {
            FiglutRepeatRestServiceClient restClient = GetRestWebServiceClient(Settings, w, userName, password);
            _currentUser = restClient.Login();
            _restClient = restClient;
            return _currentUser;
        }

        private FiglutRepeatRestServiceClient GetRestWebServiceClient(
            FiglutDesktopManagerSettings settings,
            WaitProcess w,
            string userName,
            string password)
        {
            if (w != null)
            {
                w.ChangeStatus("Setting web service client text response encoding ...");
            }
            GOC.Instance.SetEncoding(settings.WebServiceTextResponseEncoding);
            if (w != null)
            {
                w.ChangeStatus("Initializing web service client ...");
            }
            IMimeWebServiceClient webServiceClient = GetWebServiceClient(settings.WebServiceMessagingFormat, settings.WebServiceBaseUrl, userName, password);
            return new FiglutRepeatRestServiceClient(webServiceClient, settings.WebServiceRequestTimeout);
        }

        private IMimeWebServiceClient GetWebServiceClient(
            SerializerType serializerType,
            string webServiceBaseUrl,
            string userName,
            string password)
        {
            IMimeWebServiceClient result = null;
            WebServiceClient webServiceClient = null;
            switch (serializerType)
            {
                case SerializerType.XML:
                    webServiceClient = new XmlWebServiceClient(webServiceBaseUrl);
                    break;
                case SerializerType.JSON:
                    webServiceClient = new JsonWebServiceClient(webServiceBaseUrl);
                    break;
                case SerializerType.CSV:
                    webServiceClient = new CsvWebServiceClient(webServiceBaseUrl);
                    break;
                default:
                    throw new ArgumentException(string.Format(
                        "{0} not supported as a messaging format.",
                        serializerType.ToString()));
            }
            if (!string.IsNullOrEmpty(userName))
            {
                webServiceClient.NetworkCredential = new NetworkCredential(userName, password);
            }
            result = (IMimeWebServiceClient)webServiceClient;
            return result;
        }

        #endregion //Methods
    }
}