using System;
using System.Threading.Tasks;
using UserCommonApp;
using Npgsql;
using System.Net;

namespace ServerApp
{
    class DataBaseManager
    {
        private string latestVersion;
        private bool mandatoryUpdate;
        private string minimumSupportedVersion;
        private string MSIName;
        private string databaseConnectionConfiguration;
        internal NpgsqlConnection con;

        public DataBaseManager(string databaseConfiguration)
        {
            databaseConnectionConfiguration = databaseConfiguration;
            con = new NpgsqlConnection(databaseConnectionConfiguration);
        }


        internal async Task<ValidationResponse> validateClientVersion(ValidationResponse clientConfiguration)
        {
            /*float currentClientVersion;
            bool num = float.TryParse(clientConfiguration.ClientVersionNumber, out currentClientVersion);*/

            if(string.Equals(clientConfiguration.clientPlatform, "Windows"))
            {
                if(clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT \"VersionNumber\", \"MandatoryUpdate\", \"MinimumVersion\" FROM \"OStorVersions\" INNER JOIN \"OStorMinimumVersions\" ON \"Platform\" = \"SupportedPlatform\" WHERE \"Platform\" = 'Win64'  ORDER BY \"ReleaseDate\" DESC LIMIT 1";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            latestVersion = reader.GetString(0);
                            mandatoryUpdate = reader.GetBoolean(1);
                            minimumSupportedVersion = reader.GetString(2);
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
                        cmd.CommandText = "SELECT \"VersionNumber\", \"MandatoryUpdate\", \"MinimumVersion\" FROM \"OStorVersions\" INNER JOIN \"OStorMinimumVersions\" ON \"Platform\" = \"SupportedPlatform\" WHERE \"Platform\" = 'Win32'  ORDER BY \"ReleaseDate\" DESC LIMIT 1";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            latestVersion = reader.GetString(0);
                            mandatoryUpdate = reader.GetBoolean(1);
                            minimumSupportedVersion = reader.GetString(2);
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
                        cmd.CommandText = "SELECT \"VersionNumber\", \"MandatoryUpdate\", \"MinimumVersion\" FROM \"OStorVersions\" INNER JOIN \"OStorMinimumVersions\" ON \"Platform\" = \"SupportedPlatform\" WHERE \"Platform\" = 'Lin64'  ORDER BY \"ReleaseDate\" DESC LIMIT 1";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            latestVersion = reader.GetString(0);
                            mandatoryUpdate = reader.GetBoolean(1);
                            minimumSupportedVersion = reader.GetString(2);
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
                        cmd.CommandText = "SELECT \"VersionNumber\", \"MandatoryUpdate\", \"MinimumVersion\" FROM \"OStorVersions\" INNER JOIN \"OStorMinimumVersions\" ON \"Platform\" = \"SupportedPlatform\" WHERE \"Platform\" = 'Lin32'  ORDER BY \"ReleaseDate\" DESC LIMIT 1";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            latestVersion = reader.GetString(0);
                            mandatoryUpdate = reader.GetBoolean(1);
                            minimumSupportedVersion = reader.GetString(2);
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
                        cmd.CommandText = "SELECT \"VersionNumber\", \"MandatoryUpdate\", \"MinimumVersion\" FROM \"OStorVersions\" INNER JOIN \"OStorMinimumVersions\" ON \"Platform\" = \"SupportedPlatform\" WHERE \"Platform\" = 'Mac64'  ORDER BY \"ReleaseDate\" DESC LIMIT 1";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            latestVersion = reader.GetString(0);
                            mandatoryUpdate = reader.GetBoolean(1);
                            minimumSupportedVersion = reader.GetString(2);
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
                        cmd.CommandText = "SELECT \"VersionNumber\", \"MandatoryUpdate\", \"MinimumVersion\" FROM \"OStorVersions\" INNER JOIN \"OStorMinimumVersions\" ON \"Platform\" = \"SupportedPlatform\" WHERE \"Platform\" = 'Mac32'  ORDER BY \"ReleaseDate\" DESC LIMIT 1";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            latestVersion = reader.GetString(0);
                            mandatoryUpdate = reader.GetBoolean(1);
                            minimumSupportedVersion = reader.GetString(2);
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
                    error_code = 1
                };
            }

            float currentClientVersion;
            bool num = float.TryParse(clientConfiguration.ClientVersionNumber, out currentClientVersion);

            float minimumRequiredVersion;
            bool num1 = float.TryParse(minimumSupportedVersion, out minimumRequiredVersion);

            if(currentClientVersion == minimumRequiredVersion)
            {
                return new ValidationResponse
                {
                    error_code = 0,
                    isUpdateAvailable = true,
                    CurrentStableVersion = latestVersion,
                    MandatoryUpdate = false
                };
            }
            else if (currentClientVersion < minimumRequiredVersion)
            {
                return new ValidationResponse
                {
                    error_code = 0,
                    isUpdateAvailable = true,
                    CurrentStableVersion = latestVersion,
                    MandatoryUpdate = true
                };
            }
            else
            {
                return new ValidationResponse
                {
                    error_code = 0,
                    isUpdateAvailable = true,
                    CurrentStableVersion = latestVersion,
                    MandatoryUpdate = false
                };
            }


            /*if (string.Equals(clientConfiguration.ClientVersionNumber, latestVersion))
            {
                return new ValidationResponse
                {
                    error_code = 0,
                    isUpdateAvailable = false
                };
            }
            else
            {
                return new ValidationResponse
                {
                    error_code = 0,
                    isUpdateAvailable = true,
                    CurrentStableVersion = latestVersion.ToString(),
                    MandatoryUpdate = mandatoryUpdate
                };
            }*/
        }

        internal async Task<byte[]> fetchMSI(ValidationResponse clientconfig)
        {
            byte[] msiFile = null;
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT \"MSIName\" FROM \"OStorVersions\" WHERE \"VersionNumber\" = '" + clientconfig.CurrentStableVersion + "' AND \"Platform\" = '" + clientconfig.clientPlatform + "'";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    MSIName = reader.GetString(1);
                }
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }

            if(!String.IsNullOrEmpty(MSIName))
            {
                msiFile = await DownloadBinaries(MSIName);
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