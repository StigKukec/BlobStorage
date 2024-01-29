using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobApp.Dal
{
    static class Repository
    {
        public const string ForwardSlash = "/";
        public const string ContainerName = "blobcontainer";
        private static readonly string cs = ConfigurationManager.ConnectionStrings["cs"].ConnectionString; 

        private static readonly Lazy<BlobContainerClient> container = new(()=> new BlobServiceClient(cs).GetBlobContainerClient(ContainerName));
        public static BlobContainerClient Container => container.Value;
    }
}
