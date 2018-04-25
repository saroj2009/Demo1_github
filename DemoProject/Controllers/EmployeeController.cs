using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DemoProject.Models;
using DemoProject.Repository;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;

namespace DemoProject.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee/GetAllEmpDetails
        public ActionResult GetAllEmpDetails()
        {
            EmpRepository EmpRepo = new EmpRepository();
            return View(EmpRepo.GetAllEmployees());
        }
        // GET: Employee/AddEmployee
        public ActionResult AddEmployee()
        {
            EmployeeModel Emp = new EmployeeModel();
            EmpRepository erobj = new EmpRepository();
            Emp.Id = erobj.RandomNumber(1, 1000);
            return View(Emp);
        }
        // POST: Employee/AddEmployee
        [HttpPost]
        public ActionResult AddEmployee(EmployeeModel Emp, FormCollection image)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    HttpPostedFileBase fileNameglobal = null;
                    foreach (string item in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;
                        fileNameglobal = file;
                        if (file.ContentLength == 0)
                            continue;

                        if (file.ContentLength > 0)
                        {
                            string[] values = file.FileName.Split('\\');
                            CloudBlobContainer blobContainer = _blobServices.GetCloudBlobContainer();
                            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(values[values.Length-1]);
                            blob.UploadFromStream(file.InputStream);
                        }
                    }

                    var fileName = Path.GetFileName(fileNameglobal.FileName);
                    EmpRepository EmpRepo = new EmpRepository();
                    Emp.Image = fileName;
                    EmpRepo.AddEmployee(Emp);
                    ViewBag.Message = "Records added successfully.";
                }

                return RedirectToAction("GetAllEmpDetails");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        // GET: Delete  Employee details by id
        public ActionResult DeleteEmp(int id, string filename)
        {
            try
            {
                DeleteBlob(filename);
                EmpRepository EmpRepo = new EmpRepository();
                if (EmpRepo.DeleteEmployee(id))
                {
                    ViewBag.AlertMsg = "Employee details deleted successfully";
                }
                return RedirectToAction("GetAllEmpDetails");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GetAllEmpDetails");
            }
        }

        [NonAction]
        public void DeleteBlob(string fileName)
        {
            EmpRepository erObj = new EmpRepository();
            int count = erObj.GetImageCount(fileName);
            if (count == 1)
            {
                EmployeeModel obj = new EmployeeModel();

                string path = obj.Url + fileName;


                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("sarojcontainer");
                CloudBlockBlob blob = container.GetBlockBlobReference(fileName);//Changed path to fileName
                blob.Delete();
            }
        }
        BlobServices _blobServices = new BlobServices();
        public ActionResult Upload()
        {
            CloudBlobContainer blobContainer = _blobServices.GetCloudBlobContainer();
            List<string> blobs = new List<string>();
            foreach (var blobItem in blobContainer.ListBlobs())
            {
                blobs.Add(blobItem.Uri.ToString());

            }
            return View(blobs);
        }

        // GET: Employee/GetAllEmpList
        public ActionResult GetAllEmpList()
        {
            EmpRepository EmpRepo = new EmpRepository();
            return View(EmpRepo.GetAllEmpList());
        }
        // GET: Update Employee details by id
        public ActionResult UpdateEmpStatus(int id, string Location, String ActionParam)
        {
            try
            {
                
                EmpRepository EmpRepo = new EmpRepository();
                if (EmpRepo.UpdateEmployeeStatus(id, Location, ActionParam))
                {
                    ViewBag.AlertMsg = "Your status updated successfully";
                }
                return RedirectToAction("GetAllEmpList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GetAllEmpList");
            }
        }
        
        // GET: Employee/GetAllEmpList
        public ActionResult StatusDetailsById(int Id, String Name)
        {
            ViewData["Name"] = Name;
            EmpRepository EmpRepo = new EmpRepository();
            return View(EmpRepo.GetEmpStatusDetailsByID(Id));
        }
    }
}