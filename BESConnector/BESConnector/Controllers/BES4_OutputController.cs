﻿using BESConnector.Models;
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
    public class BES4_OutputController : ApiController
    {

        [Metadata("With only Output", "Experiment has no web serivce input module, but has a web service output module (e.g. uses a Reader module")]
        [HttpPost, Route("api/OnlyOutput")]
        public async Task<BatchScoreStatus> Post(
              [Metadata("API POST URL", "Web Service Request URI")] string _API_URL,
              [Metadata("API Key", "Web Service API Key")] string _API_Key,
              
              [Metadata("Storage Account Name (Output)", "Azure Storage Account Name. Leave blank if same with Input")] string _Output_AccountName,
              [Metadata("Storage Account Key (Output)", "Azure Storage Account Key. Leave blank if same with Input")] string _Output_AccountKey,
              [Metadata("Storage Container Name (Output)", "Azure Storage Container Name. Leave blank if same with Input")] string _Output_Container,
              [Metadata("Blob Name (Output)", "Azure Storage Blob Name. Include file extention. Leaving blank will set it default name")] string _Output_Blob,
              [Metadata(FriendlyName = "Global Parameters Keys", Description = "Comma separated list of parameters", Visibility = VisibilityType.Advanced)] string _GlobalKeys = "",
              [Metadata(FriendlyName = "Global Parameters Values", Description = "Comma separated list of values", Visibility = VisibilityType.Advanced)] string _GlobalValues = ""

             )
        {
            BES_Output Obj = new BES_Output
            {
                API_URL = _API_URL,
                API_Key = _API_Key,
                
                Output_AccountName = _Output_AccountName,
                Output_AccountKey = _Output_AccountKey,
                Output_Container = _Output_Container,
                Output_Blob = _Output_Blob,
                GlobalKeys = _GlobalKeys,
                GlobalValues = _GlobalValues
            };
            BatchScoreStatus result = await BatchExecutionService.InvokeBatchExecutionService(Obj);
            return result;
        }

    }
}