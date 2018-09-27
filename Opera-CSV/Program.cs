using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Opera_CSV
{
    class Program
    {
        static void Main(string[] args)
        {
            string localFileName = "SYMON.TXT";
            string newlocalFileName = "NewSYMNON.TXT";
            UpdateDelimeter(localFileName, newlocalFileName);
            PushToAzureStorage(newlocalFileName).GetAwaiter().GetResult();
            Console.ReadLine();
        }

        private static void UpdateDelimeter(string localFileName, string newLocalFileName)
        {
            string line;
       
            using (StreamReader file = new StreamReader(Environment.CurrentDirectory + @"\" + localFileName))
            {
                while ((line = file.ReadLine()) != null)
                {

                    string target = "";
                    char[] delimiters = new char[] { '\t' };
                    string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    string targetWithoutTab = "";

                    for (int i = 0; i < parts.Length; i++)
                    {

                        if (targetWithoutTab == "")
                        {
                            targetWithoutTab = parts[i];
                        }
                        else
                        {
                            targetWithoutTab += "," + parts[i];
                        }

                    }
                    string[] targetWihtoutSpace = targetWithoutTab.Split(' ');
                    foreach (var word in targetWihtoutSpace)
                    {
                        target += word;
                    }
                    Console.WriteLine(target);
                    localFileName = "NewSYMNON.TXT";
                    using (System.IO.StreamWriter targetFile =
                        new System.IO.StreamWriter(Environment.CurrentDirectory + @"\" + localFileName, true))
                    {
                        targetFile.WriteLine(target);
                    }

                }
                file.Close();
            }

        }
        private static async Task PushToAzureStorage(string fileName)
        {
            CloudStorageAccount storageAccount = null;
            //CloudBlobContainer cloudBlobContainer = null;
            CloudBlobContainer cloudBlobContainerHadi = null;
            //string sourceFile = null;
            //string destinationFile = null;

            //https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=windows#configure-your-storage-connection-string
            /*Configure your storage connection string
            To run the application, you must provide the connection string for your storage account.The sample application
            reads the connection string from an environment variable and uses it to authorize requests to Azure Storage.
            After you have copied your connection string, write it to a new environment variable on the local machine
            running the application.To set the environment variable, open a console window, and follow the instructions for 
            your operating system.Replace < yourconnectionstring > with your actual connection string:

            // setx storageconnectionstring "<yourconnectionstring>"
            // reboot the windows
            */
            string storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

          
                    cloudBlobContainerHadi = cloudBlobClient.GetContainerReference("rmgdxb-container");
                    
                    string sourceFile = "";
                    sourceFile = Path.Combine(Environment.CurrentDirectory, fileName);

                    CloudBlockBlob cloudBlockBlobHadi = cloudBlobContainerHadi.GetBlockBlobReference(fileName);
                    cloudBlockBlobHadi.Properties.ContentType = "text/plain";


                    await cloudBlockBlobHadi.UploadFromFileAsync(sourceFile);
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }

            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }
     
    }
}
