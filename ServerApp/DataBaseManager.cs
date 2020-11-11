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
            if(string.Equals(clientConfiguration.clientPlatform, "Windows"))
            {
                if(clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT \"Upgrade_Reference_Id\", \"Binary_Version_Number\", \"Mandatory_Update\", \"Minimum_Version_Supported\", \"Release_Date\" FROM \"OStorVersions\" WHERE \"Os_Version\" = 'Windows' AND \"Platform\" = 'x64'  ORDER BY \"Release_Date\" DESC LIMIT 1";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            clientConfiguration.UpgradeReferenceId = reader.GetString(0);
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
                        cmd.CommandText = "SELECT \"Upgrade_Reference_Id\", \"Binary_Version_Number\", \"Mandatory_Update\", \"Minimum_Version_Supported\", \"Release_Date\" FROM \"OStorVersions\" WHERE \"Os_Version\" = 'Windows' AND \"Platform\" = 'x32'  ORDER BY \"Release_Date\" DESC LIMIT 1";
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
                        cmd.CommandText = "SELECT \"Upgrade_Reference_Id\", \"Binary_Version_Number\", \"Mandatory_Update\", \"Minimum_Version_Supported\", \"Release_Date\" FROM \"OStorVersions\" WHERE \"Os_Version\" = 'Linux' AND \"Platform\" = 'x64'  ORDER BY \"Release_Date\" DESC LIMIT 1";
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
                        cmd.CommandText = "SELECT \"Upgrade_Reference_Id\", \"Binary_Version_Number\", \"Mandatory_Update\", \"Minimum_Version_Supported\", \"Release_Date\" FROM \"OStorVersions\" WHERE \"Os_Version\" = 'Linux' AND \"Platform\" = 'x32'  ORDER BY \"Release_Date\" DESC LIMIT 1";
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
            else if(string.Equals(clientConfiguration.clientPlatform, "MAC"))
            {
                if (clientConfiguration.is64Bit)
                {
                    try
                    {
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT \"Upgrade_Reference_Id\", \"Binary_Version_Number\", \"Mandatory_Update\", \"Minimum_Version_Supported\", \"Release_Date\" FROM \"OStorVersions\" WHERE \"Os_Version\" = 'MAC' AND \"Platform\" = 'x64'  ORDER BY \"Release_Date\" DESC LIMIT 1";
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
                        cmd.CommandText = "SELECT \"Upgrade_Reference_Id\", \"Binary_Version_Number\", \"Mandatory_Update\", \"Minimum_Version_Supported\", \"Release_Date\" FROM \"OStorVersions\" WHERE \"Os_Version\" = 'MAC' AND \"Platform\" = 'x32'  ORDER BY \"Release_Date\" DESC LIMIT 1";
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

            if(currentClientVersion >= minimumRequiredVersion)
            {
                if(mandatoryUpdate)
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