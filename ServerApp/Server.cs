using System;
using System.Threading.Tasks;
using UserCommonApp;
using System.ServiceModel;


namespace ServerApp
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Server : IUpdateManager
    {
        public Server()
        {
            database.con.Open();
        }

        ~Server()
        {
            database.con.Close();
        }

        DataBase database = new DataBase("Host = localhost; User Id = postgres; Password = Dany@100; Database = UpdateService");

        

        public async Task<Result> ValidateClientVersion(string clientVersion)
        {
            Result validateVersion = null;
            try
            {
                validateVersion = await database.validateClientVersion(clientVersion);
            }
            catch (Exception msg)
            {
            }
            return validateVersion;
        }

        public async Task<Result> DownloadBinaries(string VersionNumber)
        {
            Result binariesUpdatedMSI = null;
            try
            {
                binariesUpdatedMSI = await database.downloadBinaries(VersionNumber);
            }
            catch (Exception msg)
            {
            }
            return binariesUpdatedMSI;
        }

        public async Task<Result> NotifyAllClients()
        {
            Result clientNotify = null;
            try
            {
                clientNotify = await database.notifyAllClients();
            }
            catch (Exception msg)
            {
            }
            return clientNotify;
        }
    }
}
