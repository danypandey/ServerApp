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
        private string upgradeReferenceId;
        private string databaseConnectionConfiguration;
        private string releaseDate;
        private string installerPreference;
        internal NpgsqlConnection con;

        public DataBaseManager(string databaseConfiguration)
        {
            databaseConnectionConfiguration = databaseConfiguration;
            con = new NpgsqlConnection(databaseConnectionConfiguration);
        }


        internal async Task<ValidationResponse> validateClientVersion(ClientResponse clientConfiguration)
        {
            string client_win_vers = clientConfiguration.clientPlatform;
            string client_platform = null;
            if (clientConfiguration.is64Bit)
            {
                client_platform = "x64";
            }
            else
            {
                client_platform = "x32";
            }

            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT \"Upgrade_Reference_Id\", \"Binary_Version_Number\", \"Mandatory_Update\", \"Minimum_Version_Supported\", \"Release_Date\", \"Installer_Preference\" FROM \"OStorVersions\" WHERE \"Os_Version\" = '" + client_win_vers + "' AND \"Platform\" = '" + client_platform + "'  ORDER BY \"Release_Date\" DESC LIMIT 1";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    upgradeReferenceId = reader.GetString(0);
                    latestVersion = reader.GetString(1);
                    mandatoryUpdate = reader.GetBoolean(2);
                    minimumSupportedVersion = reader.GetString(3);
                    releaseDate = reader.GetDate(4).ToString();
                    installerPreference = reader.GetString(5);
                }
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }

            /*float currentClientVersion;
            bool num = float.TryParse(clientConfiguration.ClientVersionNumber, out currentClientVersion);

            float minimumRequiredVersion;
            bool num1 = float.TryParse(minimumSupportedVersion, out minimumRequiredVersion);*/

            if (float.Parse(clientConfiguration.ClientVersionNumber) == float.Parse(latestVersion))
            {
                return new ValidationResponse
                {
                    error_code = 0,
                    isUpdateAvailable = false
                };
            }
            else if (float.Parse(clientConfiguration.ClientVersionNumber) >= float.Parse(minimumSupportedVersion))
            {
                
                if (mandatoryUpdate)
                {
                    return new ValidationResponse
                    {
                        error_code = 0,
                        isUpdateAvailable = true,
                        MandatoryUpdate = true,
                        ReleaseDate = releaseDate,
                        UpgradeReferenceId = upgradeReferenceId,
                        CurrentStableVersion = latestVersion
                    };
                }
                else
                {
                    return new ValidationResponse
                    {
                        error_code = 0,
                        isUpdateAvailable = true,
                        MandatoryUpdate = false,
                        ReleaseDate = releaseDate,
                        UpgradeReferenceId = upgradeReferenceId,
                        CurrentStableVersion = latestVersion
                    };
                }
            }
            else if (float.Parse(clientConfiguration.ClientVersionNumber) < float.Parse(minimumSupportedVersion))
            {
                if (installerPreference == "High")
                {
                    return new ValidationResponse
                    {
                        error_code = 0,
                        isUpdateAvailable = true,
                        MandatoryUpdate = true,
                        ReleaseDate = releaseDate,
                        UpgradeReferenceId = upgradeReferenceId,
                        CurrentStableVersion = latestVersion
                    };
                }
                else
                {
                    try
                    {
                        var cmd1 = new NpgsqlCommand();
                        cmd1.Connection = con;
                        cmd1.CommandText = "SELECT \"Upgrade_Reference_Id\", \"Binary_Version_Number\", \"Mandatory_Update\", \"Release_Date\" FROM \"OStorVersions\" WHERE \"Binary_Version_Number\" = '" + minimumSupportedVersion + "' AND \"Os_Version\" = '" + client_win_vers + "' AND \"Platform\" = '" + client_platform + "' ";
                        NpgsqlDataReader reader = cmd1.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            upgradeReferenceId = reader.GetString(0);
                            latestVersion = reader.GetString(1);
                            mandatoryUpdate = reader.GetBoolean(2);
                            releaseDate = reader.GetDate(3).ToString();
                        }
                    }
                    catch (Exception msg)
                    {
                        Console.WriteLine(msg);
                    }

                    return new ValidationResponse
                    {
                        error_code = 0,
                        isUpdateAvailable = true,
                        MandatoryUpdate = true,
                        ReleaseDate = releaseDate,
                        UpgradeReferenceId = upgradeReferenceId,
                        CurrentStableVersion = latestVersion
                    };
                }
            }
            else
            {
                return new ValidationResponse
                {
                    error_code = 1
                };
            }
        }

        internal async Task<byte[]> fetchMSI(string UpgradeReferenceId)
        {
            byte[] msiFile = null;
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT \"MSI_Name\" FROM \"OStorVersions\" WHERE \"Upgrade_Reference_Id\" = '" + UpgradeReferenceId + "' ";
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    MSIName = reader.GetString(0);
                }
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }

            if(!String.IsNullOrEmpty(MSIName))
            {
                //string ConstructedUrl = baseUrl + Domain + Region + MSIName;                
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