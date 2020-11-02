﻿using System;
using System.Threading.Tasks;
using UserCommonApp;
using System.ServiceModel;


namespace ServerApp
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
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
        
        public async Task<Result> ValidateClientVersion(string clientVersion)
        {
            Result validateVersion = null;
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

        public async Task<Result> fetchMSILink(string VersionNumber)
        {
            Result latestMSILink = null;
            database.con.Open();
            try
            {
                latestMSILink = await database.fetchMsiLink(VersionNumber);
            }
            catch (Exception msg)
            {
            }
            database.con.Close();
            return latestMSILink;
        }

        public async Task<Result> NotifyAllClients()
        {
            Result clientNotify = null;
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
