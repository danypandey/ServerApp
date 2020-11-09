using System;
using System.Threading.Tasks;
using UserCommonApp;


namespace ServerApp
{
    class UpgradeServer : IUpdateManager
    {
        DataBase database = new DataBase("Host = localhost; User Id = postgres; Password = Dany@100; Database = UpdateService");
        /*public UpgradeServer()
        {
            database.con.Open();
        }

        ~UpgradeServer()
        {
            database.con.Close();
        }*/
        
        public async Task<ValidationResponse> ValidateClientVersion(string clientVersion)
        {
            ValidationResponse validateVersion = null;
            database.con.Open();
            try
            {
                validateVersion = await database.validateClientVersion(clientVersion);
            }
            catch (Exception msg)
            {
            }
            database.con.Close();
            return validateVersion;
        }

        public async Task<byte[]> FetchMSI(string VersionNumber)
        {
            byte[] latestMSI = null;
            database.con.Open();
            try
            {
                latestMSI = await database.fetchMSI(VersionNumber);
            }
            catch (Exception msg)
            {
            }
            database.con.Close();
            return latestMSI;
        }

        public async Task<ValidationResponse> NotifyAllClients()
        {
            ValidationResponse clientNotify = null;
            database.con.Open();
            try
            {
                clientNotify = await database.notifyAllClients();
            }
            catch (Exception msg)
            {
            }
            database.con.Close();
            return clientNotify;
        }
    }
}
