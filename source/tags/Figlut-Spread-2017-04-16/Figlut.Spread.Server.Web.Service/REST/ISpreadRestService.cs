namespace Figlut.Spread.Web.Service.REST
{
    #region Using Directives

    using Figlut.Server.Toolkit.Web.Service.REST;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    [ServiceContract]
    public interface ISpreadRestService : IRestService
    {
        #region Methods

        [OperationContract]
        [WebGet(UriTemplate = "/Login")]
        Stream Login();

        [OperationContract]
        [WebInvoke(UriTemplate = "/send-sms", Method = "POST")]
        Stream SendSms(Stream inputStream);

        #endregion //Methods
    }
}