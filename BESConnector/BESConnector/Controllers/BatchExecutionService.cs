using BESConnector.Models;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TRex.Metadata;

namespace BESConnector.Controllers
{
    public static class BatchExecutionService
    {
        #region Functions

       
        public static async Task<BatchScoreStatus> InvokeBatchExecutionService(BES_Obj besobj)
        {
            

            // set a time out for polling status
            const int TimeOutInMilliseconds = 120 * 10000; // Set a timeout of 20 minutes
            string BaseUrl = besobj.GetAPIURL();
            BaseUrl = BaseUrl.Substring(0, BaseUrl.LastIndexOf("/jobs") + 5);   // Correct BaseUrl don't have api-version
            string apiKey = besobj.GetAPIKey();

            BatchScoreStatus status = new BatchScoreStatus();
            using (HttpClient client = new HttpClient())
            {
                var request = new BatchExecutionRequest()
                {
                    Input = besobj.GenerateInput(),
                    Outputs = besobj.GenerateOutputs(),
                    GlobalParameters = besobj.GenerateGlobalParameters()                    
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);


                //Submitting the job...

                // submit the job
                var response = await client.PostAsJsonAsync(BaseUrl + "?api-version=2.0", request);
                if (!response.IsSuccessStatusCode)
                {
                    status.StatusCode = 0;
                    status.Details = response.ReasonPhrase + ". Job submitting gets Error. Please check input.";
                    status.SetAdditionInformation();
                    return status;
                }

                string jobId = await response.Content.ReadAsAsync<string>();
                Console.WriteLine(string.Format("Job ID: {0}", jobId));


                // start the job
                //Starting the job...
                response = await client.PostAsync(BaseUrl + "/" + jobId + "/start?api-version=2.0", null);
                if (!response.IsSuccessStatusCode)
                {
                    status.StatusCode = 0;
                    status.Details = response.ReasonPhrase + ". Cannot get status of jobId " + jobId;
                    status.SetAdditionInformation();
                    return null;
                }

                string jobLocation = BaseUrl + "/" + jobId + "?api-version=2.0";
                Stopwatch watch = Stopwatch.StartNew();
                bool done = false;                
                while (!done)
                {
                    //Checking the job status...
                    response = await client.GetAsync(jobLocation);
                    if (!response.IsSuccessStatusCode)
                    {   
                        
                        return null;
                    }

                    status = await response.Content.ReadAsAsync<BatchScoreStatus>();
                    if (watch.ElapsedMilliseconds > TimeOutInMilliseconds)
                    {
                        done = true;
                        await client.DeleteAsync(jobLocation);
                    }
                    switch (status.StatusCode)
                    {
                        case BatchScoreStatusCode.NotStarted:
                            break;
                        case BatchScoreStatusCode.Running:
                            break;
                        case BatchScoreStatusCode.Failed:
                            done = true;
                            break;
                        case BatchScoreStatusCode.Cancelled:
                            done = true;
                            break;
                        case BatchScoreStatusCode.Finished:
                            done = true;
                            break;
                    }

                    if (!done)
                    {
                        Thread.Sleep(1000); // Wait one second
                    }
                }

                status.SetAdditionInformation();
                return status;
            }

        }
        #endregion
    }
    #region PrivateClass
    public class AzureBlobDataReference
    {
        // Storage connection string used for regular blobs. It has the following format:
        // DefaultEndpointsProtocol=https;AccountName=ACCOUNT_NAME;AccountKey=ACCOUNT_KEY
        // It's not used for shared access signature blobs.
        public string ConnectionString { get; set; }

        // Relative uri for the blob, used for regular blobs as well as shared access 
        // signature blobs.
        public string RelativeLocation { get; set; }

        // Base url, only used for shared access signature blobs.
        public string BaseLocation { get; set; }

        // Shared access signature, only used for shared access signature blobs.
        public string SasBlobToken { get; set; }

        // Show full link of output blob
        public string FullURL { get; set; }

        /// <summary>
        /// Create full link for Blob output 
        /// </summary>
        public void SetFullURL()
        {
            FullURL = BaseLocation + RelativeLocation + SasBlobToken;
        }
    }

    public enum BatchScoreStatusCode
    {
        NotStarted,
        Running,
        Failed,
        Cancelled,
        Finished
    }

    public class BatchScoreStatus
    {
        [Metadata("Status Code", "0.NotStarted 1.Running 2.Failed 3.Cancelled 4.Finished")]
        // Status code for the batch scoring job
        public BatchScoreStatusCode StatusCode { get; set; }

        [Metadata("Status Description", "Show the meaning of Status Code.")]
        // Status Description
        public string StatusDescription { get; set; }

        [Metadata("Results", "List of output. Each output has: ConnectionString, RelativeLocation, BaseLocation, SasBlobToken, FullURL.")]
        // Locations for the potential multiple batch scoring outputs
        public IDictionary<string, AzureBlobDataReference> Results { get; set; }

        [Metadata("Error Details", "If the job's status is Failed, details will be showed here.")]
        // Error details, if any
        public string Details { get; set; }

        /// <summary>
        /// Set two more Informations
        /// 1. Status Descriton
        /// 2. Set full link for each output.
        /// </summary>
        public void SetAdditionInformation()
        {
            StatusDescription = Enum.GetName(typeof(BatchScoreStatusCode), this.StatusCode);
            if (Results != null)
                foreach( var outputObj in Results.Values)
                {
                    outputObj.SetFullURL();
                }
        }
    }

    public class BatchExecutionRequest
    {
        public AzureBlobDataReference Input { get; set; }
        public IDictionary<string, string> GlobalParameters { get; set; }

        // Locations for the potential multiple batch scoring outputs
        public IDictionary<string, AzureBlobDataReference> Outputs { get; set; }
    }
    #endregion

}