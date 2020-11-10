using System;
using System.Threading.Tasks;
using UserCommonApp;
using Npgsql;
using System.Net;

namespace ServerApp
{
    class DataBaseManager
    {
        private float latestVersion;
        private bool mandatoryUpdate;
        private string MSILink;
        private string databaseConnectionConfiguration;
        internal NpgsqlConnection con;

        public DataBaseManager(string databaseConfiguration)
        {
            databaseConnectionConfiguration = databaseConfiguration;
            con = new NpgsqlConnection(databaseConnectionConfiguration);
        }


        internal async Task<ValidationResponse> validateClientVersion(ValidationResponse clientConfiguration)
        {
            float currentClientVersion;
            bool num = float.TryParse(clientConfiguration.ClientVersionNumber, out currentClientVersion);

            if(string.Equals(clientConfiguration.clientPlatform, "Windows"))
            {
                if(clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM \"OStoreVersions\" WHERE \"Id\" = 'Win64' ORDER BY \"ReleaseDate\" DESC LIMIT 1";
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
                }
                else if (!clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM \"OStoreVersions\" WHERE \"Id\" = 'Win32' ORDER BY \"ReleaseDate\" DESC LIMIT 1";
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
                }
            }
            else if (string.Equals(clientConfiguration.clientPlatform, "Linux"))
            {
                if (clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM \"OStoreVersions\" WHERE \"Id\" = 'Lin64' ORDER BY \"ReleaseDate\" DESC LIMIT 1";
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
                }
                else if (!clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM \"OStoreVersions\" WHERE \"Id\" = 'Lin32' ORDER BY \"ReleaseDate\" DESC LIMIT 1";
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
                }
            }
            else if(string.Equals(clientConfiguration.clientPlatform, "OSX"))
            {
                if (clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM \"OStoreVersions\" WHERE \"Id\" = 'Mac64' ORDER BY \"ReleaseDate\" DESC LIMIT 1";
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
                }
                else if (!clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM \"OStoreVersions\" WHERE \"Id\" = 'Mac32' ORDER BY \"ReleaseDate\" DESC LIMIT 1";
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
                }
            }
            else
            {
                return new ValidationResponse
                {
                    error_code = 0,

                    // Need to handle errors here
                };
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

        internal async Task<byte[]> fetchMSI(ValidationResponse clientconfig)
        {
            float MSI_Version;
            bool num = float.TryParse(clientconfig.CurrentStableVersion, out MSI_Version);
            byte[] msiFile = null;
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT * FROM \"OStoreVersions\" WHERE \"VersionNumber\" = '" + MSI_Version + "'";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    MSILink = reader.GetString(1);
                }
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }

            if(!String.IsNullOrEmpty(MSILink))
            {
                msiFile = await DownloadBinaries(MSILink);
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