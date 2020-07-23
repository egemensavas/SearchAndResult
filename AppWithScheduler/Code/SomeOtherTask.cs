using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AppWithScheduler.Code
{
    public class SomeOtherTask : IScheduledTask
    {
        public string Schedule => "2 * * * *";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(2000, cancellationToken);
            string URL = "https://sahibindenapp.azurewebsites.net/API/SendNotification?SearchMasterID=2";
            var request = (HttpWebRequest)WebRequest.Create(URL);

            request.Method = "GET";
            request.ContentType = "application/json";

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result_ = streamReader.ReadToEnd();
            }
        }
    }
}