using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using skillsBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cors;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3.Transfer;

namespace skillsBackend.Controllers
{
    [Route("api/[controller]")]
    //[EnableCors("default")]
    //api/uploadfiles
    public class UploadFilesController : Controller
    {

        private readonly SkillsContext _context;
        IAmazonS3 S3Client { get; set; }

        public UploadFilesController(SkillsContext context, IAmazonS3 s3Client)
        {
            _context = context;
            S3Client = s3Client;
        }

        [HttpPost]
        public IActionResult Post()
        {
            // Prepare variables
            UploadedImages imagePaths = new UploadedImages();
            //long size = 0;

            // Retrieve attached files from the POST
            var files = Request.Form.Files; 
            foreach (var file in files)
            {
                // generates a full path for a file in temp location
                var filename = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');

                var extension = Path.GetExtension(filename);

                //size += file.Length;
                //var fullPath = Path.GetTempFileName();
                //fullPath = hostingEnv.WebRootPath + $@"\{filename}";
                string fileName = GetRandomNumber() + GetCurrentDate() + extension;
                string fullPath = @"D:\LetSkillsImages\Images\" + fileName;
                Console.WriteLine("File Path: " + fullPath);

                // Write file do disk
                using (FileStream fs = System.IO.File.Create(fullPath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }

                // Upload files to Amazon S3
                //GetBucketsFromS3();
                var r = UploadFileToS3(fullPath, "images.angular4.test1", "images", fileName);

                // Accumulate image Urls in the list property to return later to Angular
                string S3FullPath = "https://s3.eu-west-2.amazonaws.com/" + "images.angular4.test1/" + "images/" + fileName;
                imagePaths.images.Add(S3FullPath);
            }

            // returns JSON object in the format {"images":["Path1","Path2"]};
            return Json(imagePaths);
        }


        public static async Task<bool> UploadFileToS3(string localPath, string bucketName, string subDirectoryInBucket, string fileNameInS3)
        {
            try
            {
                // EU (London):	eu-west-2
                using (var client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest2))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = bucketName + @"/" + subDirectoryInBucket,
                        Key = fileNameInS3,
                        FilePath = localPath
                    };
                    var response = await client.PutObjectAsync(request); // Upload to S3
                    Console.WriteLine("S3 response: " + response.HttpStatusCode);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in UploadFileToS3 method:" + ex.Message);
                return false;
            }
        }
        
        // Gets available buckets linked to the IAM account configured in Setup
        private void GetBucketsFromS3()
        {
            try
            {
                var AWSresponse = S3Client.ListBucketsAsync();
                Console.WriteLine("Processing S3 response");

                List<S3Bucket> buckets = AWSresponse.Result.Buckets;
                foreach (S3Bucket bucket in buckets)
                {
                    Console.WriteLine("Found bucket name {0} created at {1}", bucket.BucketName, bucket.CreationDate);
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                Console.WriteLine(amazonS3Exception.Message);
            }
        }

        //Function to get random number
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static string GetRandomNumber()
        {
            lock (syncLock)
            { // synchronize
                string r = "";
                int i;
                for (i = 1; i <= 20; i++)
                {
                    r += getrandom.Next(0, 9).ToString();
                }
                return r;
            }
        }

        // Get date as a string
        public static string GetCurrentDate() {
            
            //return DateTime.Now.ToString("yyMMddhhmmss");
            return DateTime.Now.ToString("yyMMdd");
        }
        
    }

    public class UploadedImages
    {
        public List<string> images { get; set; }

        // List must be initialized in the constructor
        public UploadedImages() { 
            images = new List<string>();
        }
    }
}
