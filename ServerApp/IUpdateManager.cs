using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace UserCommonApp
{
    [ServiceContract]
    public interface IUpdateManager
    {
        /*
         * Get 
         */
        [OperationContract]
        [WebGet(UriTemplate = "/updateservice/{clientConfiguration}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<ValidationResponse> ValidateClientVersion(string clientConfiguration);

        /*
         * Get 
         */
        [OperationContract]
        [WebGet(UriTemplate = "/updateservice/download/{VersionNumber}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<byte[]> FetchMSI(string VersionNumber);

        /*
         * Post 
         */
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/updateservice/notify", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<ValidationResponse> NotifyAllClients();
    }
}

