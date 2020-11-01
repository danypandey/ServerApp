using System;
using Ziroh.Misc.Common;
using UserCommonApp;
using System.ServiceModel;

namespace ServerApp
{
    class Service
    {
        internal void Start(Uri baseAddress)
        {
            GenericHttpService<Server, IUserService> service = new GenericHttpService<Server, IUserService>(baseAddress.ToString(), false);
            try
            {
                service.StartHost();
                Console.WriteLine("started");
                Console.ReadKey();
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
