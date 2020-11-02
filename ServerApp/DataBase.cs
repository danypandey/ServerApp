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
                    latestVersion = reader.GetFloat(0);
                    mandatoryUpdate = reader.GetBoolean(2);
                }
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
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
                    CurrentStableVersion = latestVersion.ToString(),
                    MandatoryUpdate = mandatoryUpdate
                };
            }
        }

        internal async Task<Result> fetchMsiLink(string versionNumber)
        {
            float latestVersion;
            bool num = float.TryParse(versionNumber, out latestVersion);

            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT * FROM \"OStoreVersions\" WHERE \"VersionNumber\" = '" + latestVersion + "' ";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    linkMSI = reader.GetString(1);
                }
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }
            return new Result
            {
                latestVersionLink = linkMSI
            };
        }

        internal async Task<Result> notifyAllClients()
        {
            throw new NotImplementedException();
        }
    }
}