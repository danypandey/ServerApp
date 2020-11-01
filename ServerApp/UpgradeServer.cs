using System;
using System.Threading.Tasks;
using UserCommonApp;
using System.ServiceModel;


namespace ServerApp
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class UpgradeServer : IUpdateManager
    {
        public UpgradeServer()
        {
            database.con.Open();
        }

        ~UpgradeServer()
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
            Result latestMSILink = null;
            try
            {
                latestMSILink = await database.downloadBinaries(VersionNumber);
            }
            catch (Exception msg)
            {
            }
            return latestMSILink;
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
