namespace Figlut.Repeat.Receiver.Web.Service.REST
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
    public interface IRepeatReceiverRestService : IRestService
    {
        #region Methods

        [OperationContract]
        [WebGet(UriTemplate = "/Login")]
        Stream Login();

        [OperationContract]
        //[WebInvoke(UriTemplate = "/receive-sms-zoomconnect/&from={fromValue}&messageId={messageIdValue}&message={messageValue}&date={dateValue}&campaign={campaignValue}&dataField={dataFieldValue}&nonce={nonceValue}&nonce-date={noncedateValue}&checksum={checksumValue}", Method = "POST")]
        [WebInvoke(UriTemplate = "/receive-sms-zoomconnect?from={fromValue}&messageId={messageIdValue}&message={messageValue}&date={dateValue}&campaign={campaignValue}&dataField={dataFieldValue}&nonce={nonceValue}&nonce-date={noncedateValue}&checksum={checksumValue}", Method = "POST")]
        //[WebInvoke(UriTemplate = "/receive-sms-zoomconnect/&from=+27725551234&messageId=124567&message=REPLY&date=201404282055&campaign=shoe campaign&dataField=custom&nonce=89430dc5-cac1-4795-9dd0-46c9b8233e52&nonce-date=20140428210027&checksum=180f27d80dd05886e81da0c2ccebf830bc85c7ae", Method = "POST")]
        Stream ReceiveSmsZoomConnect(string fromValue, string messageIdValue, string messageValue, string dateValue, string campaignValue, string dataFieldValue, string nonceValue, string noncedateValue, string checksumValue);

        [OperationContract]
        //[WebInvoke(UriTemplate = "/receive-sms-zoomconnect/&from={fromValue}&messageId={messageIdValue}&message={messageValue}&date={dateValue}&campaign={campaignValue}&dataField={dataFieldValue}&nonce={nonceValue}&nonce-date={noncedateValue}&checksum={checksumValue}", Method = "POST")]
        [WebInvoke(UriTemplate = "/delivery-report-sms-zoomconnect?from={fromValue}&messageId={messageIdValue}&message={messageValue}&date={dateValue}&campaign={campaignValue}&dataField={dataFieldValue}&nonce={nonceValue}&nonce-date={noncedateValue}&checksum={checksumValue}", Method = "POST")]
        //[WebInvoke(UriTemplate = "/receive-sms-zoomconnect/&from=+27725551234&messageId=124567&message=REPLY&date=201404282055&campaign=shoe campaign&dataField=custom&nonce=89430dc5-cac1-4795-9dd0-46c9b8233e52&nonce-date=20140428210027&checksum=180f27d80dd05886e81da0c2ccebf830bc85c7ae", Method = "POST")]
        Stream DeliveryReportSmsZoomConnect(string fromValue, string messageIdValue, string messageValue, string dateValue, string campaignValue, string dataFieldValue, string nonceValue, string noncedateValue, string checksumValue);

        #endregion //Methods
    }
}