using System;
using System.Threading.Tasks;
using UserCommonApp;
using Newtonsoft.Json;


namespace ServerApp
{
    class UpgradeServer : IUpdateManager
    {
        DataBaseManager databasemanager = new DataBaseManager("Host = localhost; User Id = postgres; Password = Dany@100; Database = UpdateService");
        /*public UpgradeServer()
        {
            database.con.Open();
        }

        ~UpgradeServer()
        {
            database.con.Close();
        }*/
        
        public async Task<ValidationResponse> ValidateClientVersion(string clientConfiguration)
        {
            ValidationResponse clientConfigObject = JsonConvert.DeserializeObject<ValidationResponse>(clientConfiguration);
            ValidationResponse validateVersion = null;
            databasemanager.con.Open();
            try
            {
                validateVersion = await databasemanager.validateClientVersion(clientConfigObject);
            }
            catch (Exception msg)
            {
            }
            databasemanager.con.Close();
            return validateVersion;
        }

        public async Task<byte[]> FetchMSI(string VersionNumber)
        {
            byte[] latestMSI = null;
            databasemanager.con.Open();
            try
            {
                latestMSI = await databasemanager.fetchMSI(VersionNumber);
            }
            catch (Exception msg)
            {
            }
            databasemanager.con.Close();
            return latestMSI;
        }

        public async Task<ValidationResponse> NotifyAllClients()
        {
            ValidationResponse clientNotify = null;
            databasemanager.con.Open();
            try
            {
                clientNotify = await databasemanager.notifyAllClients();
            }
            catch (Exception msg)
            {
            }
            databasemanager.con.Close();
            return clientNotify;
        }
    }
}
