using BESConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TRex.Metadata;

namespace BESConnector.Controllers
{
    public class BES3_InputController : ApiController
    {
        [Metadata("With only Input", "Experiment has a web serivce input module, but no web service output module (e.g. uses a Writer module")]
        [HttpPost, Route("api/OnlyInput")]
        public async Task<BatchScoreStatus> Post(
             [Metadata("API POST URL", "Web Service Request URI")] string _API_URL,
             [Metadata("API Key", "Web Service API Key")] string _API_Key,
             [Metadata("Storage Account Name (Input)", "Azure Storage Account Name")] string _Input_AccountName,
             [Metadata("Storage Account Key (Input)", "Azure Storage Account Key")] string _Input_AccountKey,
             [Metadata("Storage Container Name (Input)", "Azure Storage Container Name")] string _Input_Container,
             [Metadata("Blob Name (Input)", "Azure Storage Blob Name")] string _Input_Blob,
             
             [Metadata(FriendlyName = "Global Parameters Keys", Description = "Comma separated list of parameters", Visibility = VisibilityType.Advanced)] string _GlobalKeys = "",
             [Metadata(FriendlyName = "Global Parameters Values", Description = "Comma separated list of values", Visibility = VisibilityType.Advanced)] string _GlobalValues = ""

            )
        {
            BES_Input Obj = new BES_Input
            {
                API_URL = _API_URL,
                API_Key = _API_Key,
                Input_AccountName = _Input_AccountName,
                Input_AccountKey = _Input_AccountKey,
                Input_Container = _Input_Container,
                Input_Blob = _Input_Blob,
                
                GlobalKeys = _GlobalKeys,
                GlobalValues = _GlobalValues
            };

            BatchScoreStatus result = await BatchExecutionService.InvokeBatchExecutionService(Obj);
            return result;
        }

    }
}
