using BESConnector.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TRex.Metadata;

namespace BESConnector.Models
{
    public class BES_None : BES_Obj
    {        
        public override AzureBlobDataReference GenerateInput()
        {
            return new AzureBlobDataReference();
        }

        public override Dictionary<string, AzureBlobDataReference> GenerateOutputs()
        {
            return new Dictionary<string, AzureBlobDataReference>();
        }
    }
}