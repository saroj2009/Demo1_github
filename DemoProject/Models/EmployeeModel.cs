using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace DemoProject.Models
{
    public class EmployeeModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        public string Image { get; set; }
        public EmployeeModel()
        {
            Url = ConfigurationManager.AppSettings["fulldestContainer"]; 
            //Url = "https://sarojwebappstorage.blob.core.windows.net/sarojcontainer2/";
        }
        public string Url { get; set; }
    }

    public class BlobServices
    {
        public CloudBlobContainer GetCloudBlobContainer()
        {
            string StorageconnString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string destContainer = ConfigurationManager.AppSettings["destContainer"];
            //string connString = "DefaultEndpointsProtocol=https;AccountName=sarojwebappstorage;AccountKey=bcXBWqEdljs7PbmVM83w+AtYqYazQIhp2O+9gikYWwlC2a4fNTHVnvgc83ETZpLquQTYGTl+4CrupCK4zWnXDg==";
            //string destContainer = "sarojcontainer2";
            
            // Get a reference to the storage account  
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageconnString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(destContainer);
            if (blobContainer.CreateIfNotExists())
            {
                blobContainer.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            }
            return blobContainer;

        }
    }

    public class EmployeeDetailsModel
    {
        public int EmpId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
    }
    public class EmployeeDailyStatusModel
    {
        public int EmpId { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
    }
}