using System;
using System.Collections.Generic;

using System.Net;


namespace AzureMLConnector.Models
{
    public class ResponeObject
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Description { get; set; }
    }
}