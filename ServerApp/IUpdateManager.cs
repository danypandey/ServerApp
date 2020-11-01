using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using UserCommonApp;

namespace UserCommonApp
{
    [ServiceContract]
    public interface IUpdateManager
    {
        /*
         * Get 
         */
        [OperationContract]
        [WebGet(UriTemplate = "/updateservice/{clientVersion}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<Result> ValidateClientVersion(string clientVersion);

        /*
         * Get 
         */
        [OperationContract]
        [WebGet(UriTemplate = "/updateservice/download/{VersionNumber}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<Result> DownloadBinaries(string VersionNumber);

        /*
         * Post 
         */
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/updateservice/notify", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<Result> NotifyAllClients();
    }
}

