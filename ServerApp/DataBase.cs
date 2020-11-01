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
        private float latestVersion;
        private bool mandatoryUpdate;
        private string linkMSI;
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

            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT * FROM \"OStoreVersions\" LIMIT 1;";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    latestVersion = float.Parse(reader.GetString(0));
                    mandatoryUpdate = bool.Parse(reader.GetString(1));
                }
            }
            catch (Exception msg)
            {
            }

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
                    MandatoryUpdate = mandatoryUpdate
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