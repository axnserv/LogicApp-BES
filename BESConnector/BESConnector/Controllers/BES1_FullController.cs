using BESConnector.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TRex.Metadata;

namespace BESConnector.Controllers
{
    public class BES1_FullController : ApiController
    {

        [Metadata("With Input and Output", "Experiment has web service input and output modules")]
        [HttpPost, Route("api/Full")]
        //public async Task<BatchScoreStatus> Put([FromBody]
        //                                [Metadata("Input Blob Info", "Input Blob and Global Parameters Keys")]
        //                                    BES_Full Obj)
        public async Task<BatchScoreStatus> Post(
             [Metadata("API POST URL", "Web Service Request URI")] string _API_URL,
             [Metadata("API Key", "Web Service API Key")] string _API_Key,
             [Metadata("Storage Account Name (Input)", "Azure Storage Account Name")] string _Input_AccountName,
             [Metadata("Storage Account Key (Input)", "Azure Storage Account Key")] string _Input_AccountKey,
             [Metadata("Storage Container Name (Input)", "Azure Storage Container Name")] string _Input_Container,
             [Metadata("Blob Name (Input)", "Azure Storage Blob Name")] string _Input_Blob,
             [Metadata("Storage Account Name (Output)", "Azure Storage Account Name. Leave blank if same with Input")] string _Output_AccountName = "",
             [Metadata("Storage Account Key (Output)", "Azure Storage Account Key. Leave blank if same with Input")] string _Output_AccountKey = "",
             [Metadata("Storage Container Name (Output)", "Azure Storage Container Name. Leave blank if same with Input")] string _Output_Container = "",
             [Metadata("Blob Name (Output)", "Azure Storage Blob Name. Include file extention. Leaving blank will set it default name")] string _Output_Blob = "",
             [Metadata(FriendlyName = "Global Parameters Keys", Description = "Comma separated list of parameters", Visibility = VisibilityType.Advanced)] string _GlobalKeys = "",
             [Metadata(FriendlyName = "Global Parameters Values", Description = "Comma separated list of values", Visibility = VisibilityType.Advanced)] string _GlobalValues = ""
            
            )
        {
            BES_Full Obj = new BES_Full
            {
                API_URL = _API_URL,
                API_Key = _API_Key,
                Input_AccountName = _Input_AccountName,
                Input_AccountKey = _Input_AccountKey,
                Input_Container = _Input_Container,
                Input_Blob = _Input_Blob,
                Output_AccountName = _Output_AccountName,
                Output_AccountKey = _Output_AccountKey,
                Output_Container = _Output_Container,
                Output_Blob = _Output_Blob,
                GlobalKeys = _GlobalKeys,
                GlobalValues = _GlobalValues
            };

            if (string.IsNullOrEmpty(Obj.Output_AccountName))
                Obj.Output_AccountName = Obj.Input_AccountName;
            if (string.IsNullOrEmpty(Obj.Output_AccountKey))
                Obj.Output_AccountKey = Obj.Input_AccountKey;
            if (string.IsNullOrEmpty(Obj.Output_Container))
                Obj.Output_Container = Obj.Input_Container;
            if (string.IsNullOrEmpty(Obj.Output_Blob))
                Obj.Output_Blob = "output_" + Obj.Input_Blob;

            BatchScoreStatus result = await BatchExecutionService.InvokeBatchExecutionService(Obj);
            return result;
        }
    }
}
