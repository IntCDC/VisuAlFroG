using System;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using Core.Utilities;
using Core.Abstracts;



/*
 * Web API Manager
 * 
 */
/* TEST
Execute:

    bool received_response = false;
    var response = _client.GetAsync(_base_address + "api/request").Result;
    if (response != null)
    {
        Log.Default.Msg(Log.Level.Debug, response.ToString());
        Log.Default.Msg(Log.Level.Debug, response.Content.ReadAsStringAsync().Result);
        received_response = true;
    }
*/
namespace Visualizations
{
    namespace WebAPI
    {
        public class WebAPIService : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();

                // Generate random port number
                /// TODO Is there a reason why to choose random port? Break sometimes (when port is already used?)
                var generator = new Random();
                var port = generator.Next(49215, 65535).ToString();
                //port = "55555";

                // Create base address
                _base_address = "http://localhost:" + port + "/";
                Log.Default.Msg(Log.Level.Debug, "Web API Base Address: " + _base_address);

                // Actually start web API and create new client
                WebApp.Start<StartUp>(url: _base_address);
                _client = new HttpClient();

                _timer.Stop();
                _initilized = true;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initilized;
            }


            public override bool Terminate()
            {
                if (_initilized)
                {
                    _client.Dispose();
                    _client = null;

                    _initilized = false;
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private HttpClient _client = null;
            private string _base_address = null;
        }
    }
}
