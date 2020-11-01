using System;
using System.Threading.Tasks;
using UserCommonApp;
using System.ServiceModel;
using Ziroh.Misc.Logging;
using Npgsql;

namespace ServerApp
{
    class DataBase
    {
        private static float latestVersion = 1.0F;
        private bool mandatoryUpdate = false;
        private string linkMSI = "hmsidownload";
        string path;
        public NpgsqlConnection con;

        public DataBase(string databaseConfiguration)
        {
            path = databaseConfiguration;
            con = new NpgsqlConnection(path);
        }

        internal async Task<Result> validateClientVersion(string clientVersion)
        {
            float currentClientVersion;
            bool num = float.TryParse(clientVersion, out currentClientVersion);

            if (currentClientVersion == latestVersion)
            {
                return new Result
                {
                    Error_code = 0,
                    CurrentStableVersion = default(string)
                };
            }
            else
            {
                return new Result
                {
                    Error_code = 1,
                    MandatoryUpdate = mandatoryUpdate,
                    CurrentStableVersion = linkMSI
                };
            }
        }

        internal async Task<Result> downloadBinaries(string VersionNumber)
        {
            throw new NotImplementedException();
        }

        internal async Task<Result> notifyAllClients()
        {
            throw new NotImplementedException();
        }
    }
}
