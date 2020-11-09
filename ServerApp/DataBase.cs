using System;
using System.Threading.Tasks;
using UserCommonApp;
using Npgsql;
using System.Net;

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


        internal async Task<ValidationResponse> validateClientVersion(string clientVersion)
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
                return new ValidationResponse
                {
                    error_code = 0,
                    CurrentStableVersion = default(string)
                };
            }
            else
            {
                return new ValidationResponse
                {
                    error_code = 1,
                    CurrentStableVersion = latestVersion.ToString(),
                    MandatoryUpdate = mandatoryUpdate
                };
            }
        }

        internal async Task<byte[]> fetchMSI(string versionNumber)
        {
            float latestVersion;
            bool num = float.TryParse(versionNumber, out latestVersion);
            byte[] msiFile = null;
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

            if(!String.IsNullOrEmpty(linkMSI))
            {
                msiFile = await DownloadBinaries(linkMSI);
            }

            return msiFile;
        }

        private async Task<byte[]> DownloadBinaries(string url)
        {
            byte[] fileData = null;
            if (!String.IsNullOrEmpty(url))
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        fileData = client.DownloadData(url);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return fileData;
        }

        internal async Task<ValidationResponse> notifyAllClients()
        {
            throw new NotImplementedException();
        }
    }
}