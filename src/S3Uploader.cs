
using System; 
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Data;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

// To interact with Amazon S3.
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace db2s3{

    class S3Uploader{
        public AmazonS3Client  s3Client;

        public AmazonS3Config s3Config = new AmazonS3Config();
        public string uploadDirectory;
        //public  List<S3UploadSession> uploadSession;
        public  List<S3Gateway> s3Gateways;
        public S3UploadSession uploadSession;
        public  List<S3Gateway> attemptedGateways   =  new List<S3Gateway>();



        public  List<S3UploadEntity> entitiesToBeUploaded =  new List<S3UploadEntity> ();
        public S3Uploader(){
             try{
                new S3UploadLibrary();
                this.runS3Upload();
             }catch(Exception e){
                Console.WriteLine("An error has occurred");
                Console.WriteLine("Error: "+e.StackTrace);
                S3UploadLibrary.writeToLog("An error has occurred");
                S3UploadLibrary.writeToLog("Error: "+e.StackTrace);

             }

        }
        public S3Uploader(string configFilePath){
              try{
                new S3UploadLibrary(configFilePath);
                this.runS3Upload();
             }catch(Exception e){
                Console.WriteLine("An error has occurred");
                S3UploadLibrary.writeToLog("An error has occurred");
                Console.WriteLine( S3UploadLibrary.getErrorMessage(e));
                S3UploadLibrary.writeToLog(S3UploadLibrary.getErrorMessage(e));
             }
        }

        public void setUploadDirectory(string folder){
             this.uploadDirectory = folder;
        }  

        public string getUploadDirectory(){
             return this.uploadDirectory;
        }

        public List<S3Gateway> getS3Gateways(){
             return this.s3Gateways;
        } 
        public  void setS3Gateways(List<S3Gateway> gateways){
             this.s3Gateways = gateways;
        }
  
        public  void runS3Upload(){
            S3UploadLibrary.writeToLog(String.Format("Scanning path {0} for files to upload...",S3UploadLibrary.directoryOfUploadfiles));
            Console.WriteLine(String.Format("Scanning path {0} for files to upload...",S3UploadLibrary.directoryOfUploadfiles));
            this.setUploadDirectory(S3UploadLibrary.directoryOfUploadfiles);
            this.setS3Gateways(S3UploadLibrary.s3Gateways);
            List<S3UploadEntity> allS3EntitiesInPath  = this.getUploadEntitiesFromPath(this.getUploadDirectory(), false);
            List<S3UploadItem> previouslyUploadedItems                       = getPreviouslyUploadedItems();
            List<S3UploadEntity> entitiesToBeUploaded =  this.getEntitiesToBeUploaded(allS3EntitiesInPath, previouslyUploadedItems);
            this.setUploadSession(createUploadSession());
            List<S3UploadItem> itemsUploaded =  startEntityUploadSession(entitiesToBeUploaded);
            itemsUploaded.ForEach(x=>x.save());
           // this.getUploadSession().setGateway(String.Join(",", attemptedGateways.Select(x => x.getUrl()).ToArray()));
            this.getUploadSession().setUploadCount(itemsUploaded.Count);
            this.getUploadSession().setEndTime(DateTime.Now);
            this.getUploadSession().setStatus(S3UploadLibrary.SUCCESSFUL);
            Console.WriteLine(this.getUploadSession().getStartTime().ToString());
            Console.WriteLine(this.getUploadSession().getEndTime().ToString());
            this.getUploadSession().save();
        }
 
        public S3UploadSession createUploadSession(){
              S3UploadSession session   =  new S3UploadSession();
              session.setSessionID(session.getLastS3UploadSessionID()+1);
              session.setServerName(S3UploadLibrary.serverName);
              session.setServerIP(S3UploadLibrary.serverIPAddress);
              session.setBucketName(S3UploadLibrary.bucketName); 
              session.setUploadPath(S3UploadLibrary.directoryOfUploadfiles);
              session.setUploadCount(0);
              session.setStatus(S3UploadLibrary.PENDING);
              return session;

        }

        public void checkCertificate(){
                                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (
                        object sender,
                        X509Certificate certificate,
                        X509Chain chain,
                        SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };
        }

        public bool initS3Client(string accessKey, string secretKey, string gateway){
              bool isConnected = false;
              try{
                    s3Config.ServiceURL  = gateway;
                    s3Config.Timeout = TimeSpan.FromSeconds(1);
                    s3Config.ForcePathStyle = true;
                    //s3Config.ReadWriteTimeout = TimeSpan.FromSeconds(10);
                    //s3Config. RetryMode = RequestRetryMode.Standard;
                    //s3Config.MaxErrorRetry = 3;
                    BasicAWSCredentials basicCredentials = new BasicAWSCredentials(accessKey, secretKey);
                    this.checkCertificate();
                    this.getUploadSession().setGateway(gateway);
                    S3UploadLibrary.writeToLog("Attempting to connect to gateway at: "+gateway);
                    Console.WriteLine("Attempting to connect to gateway at: "+gateway);
                    if(S3UploadLibrary.useProfile && File.Exists(S3UploadLibrary.profilePath)){
                        CredentialProfileStoreChain chain = new CredentialProfileStoreChain(S3UploadLibrary.profilePath);
                        AWSCredentials  awsCredentials;

                        if (chain.TryGetAWSCredentials(S3UploadLibrary.profileName, out awsCredentials))
                        {
                            this.s3Client =  new AmazonS3Client(awsCredentials); //, basicProfile.Region)
                           
                        }
                    } else{

                         this.s3Client = new AmazonS3Client(basicCredentials,s3Config);
          
                    } 

                    ListBucketsResponse buckets = s3Client.ListBuckets();
                    if(buckets != null){
                            isConnected = true;
                    }
                    
                    S3UploadLibrary.writeToLog("Successfully connected to gateway at: "+gateway);
                    Console.WriteLine("Successfully connected to gateway at: "+gateway);

            
              }catch(Exception e ){

                   Console.WriteLine("Unable to connect to S3 via gateway: "+gateway);
                  Console.WriteLine( S3UploadLibrary.getErrorMessage(e));

                S3UploadLibrary.writeToLog("Unable to connect to S3 via gateway: "+gateway);
                S3UploadLibrary.writeToLog( S3UploadLibrary.getErrorMessage(e));

              } 
               return isConnected;
        }

         public List<S3UploadItem> startEntityUploadSession(List<S3UploadEntity> entities){
              List<S3UploadItem> uploadedItems = new List<S3UploadItem>();

              this.getUploadSession().setStartTime(DateTime.Now);
              string accessKey = S3UploadLibrary.accessID;
              string secretKey = S3UploadLibrary.accessKey;
              foreach(S3Gateway gateway in s3Gateways){
                   attemptedGateways.Add(gateway);
                   if (initS3Client(accessKey, secretKey, gateway.getUrl())){
                        break;
                   }
              }
              if(this.s3Client!= null){
                  Console.WriteLine("Starting bucket upload");
                  S3UploadLibrary.writeToLog("Starting bucket upload");
                  string currentFile = "None";
                  try{
                    
                    ListBucketsResponse response = this.s3Client.ListBuckets();  
                    bool found = false;  
                    foreach(S3Bucket bucket in response.Buckets) {  
                        if (bucket.BucketName == S3UploadLibrary.bucketName) {  
                            found = true;  
                            break;  
                        }  
                    }  
                  if (!found ) {  

                        Console.WriteLine(String.Format("Bucket {0} not found. Creating bucket now...", S3UploadLibrary.bucketName));
                        S3UploadLibrary.writeToLog(String.Format("Bucket {0} not found. Creating bucket now...", S3UploadLibrary.bucketName));
                        Console.WriteLine("Bucket Name: "+S3UploadLibrary.bucketName);
                        Console.WriteLine("Bucket Region: "+S3UploadLibrary.region);
                        PutBucketResponse buckResponse = this.s3Client.PutBucket(new PutBucketRequest(){BucketName = S3UploadLibrary.bucketName, BucketRegion  = S3UploadLibrary.region }); 
        
                        Console.WriteLine("Bucket Response"+buckResponse.ToString());
                    }
                    long lastUploadedItemID  = S3UploadLibrary.getLastID( "upload_items", "item_id", "max_item_id");
                    this.checkCertificate();
                    foreach( S3UploadEntity entity in entities){
                        
                        Console.WriteLine(String.Format("Uploading file {0} to bucket {1}...",entity.getName(),S3UploadLibrary.bucketName));
                        S3UploadLibrary.writeToLog(String.Format("Uploading file {0} to bucket {1}...",entity.getName(),S3UploadLibrary.bucketName));
                        Dictionary<string,string> metadata = new Dictionary<string,string>(); 
                        currentFile = entity.fullName; 
                        metadata.Add("x-amz-meta-filepath",currentFile); 
                        PutObjectRequest request = new PutObjectRequest(){
                             BucketName = S3UploadLibrary.bucketName
                            ,Key = entity.getName()
                            ,FilePath = entity.getFullName()
                            ,Timeout               = TimeSpan.FromSeconds(100)
                            ,ReadWriteTimeout      = TimeSpan.FromSeconds(100)  
                          
                        }; 
                        DateTime startTime = DateTime.Now;                 
                        this.s3Client.PutObject(request); 
                        DateTime endTime   = DateTime.Now;
                        
                        GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest(){
                               BucketName = S3UploadLibrary.bucketName
                            ,Key = entity.getName()
                            ,Expires = DateTime.Now.Add(new TimeSpan(1000, 0, 0, 0))
                        };  
                        
                        string url = this.s3Client.GetPreSignedURL(urlRequest);
                        ++lastUploadedItemID;
                        uploadedItems.Add(new S3UploadItem()  
                            {  
                            itemID              =   lastUploadedItemID, 
                            sessionID           =   this.getUploadSession().sessionID, 
                            parentFolder        = this.getUploadDirectory(),  
                            fileName            = entity.filePath,  
                            bucketName          = S3UploadLibrary.bucketName,
                            creationTime        = DateTime.Now,
                            fileSize            = entity.getSize(),
                            uploadStartTime     = startTime,
                            uploadEndTime       = endTime,
                            fileUrl             = url,   
                            uploadStatus        = S3UploadLibrary.SUCCESSFUL
                            }
                 );
                    }

                  

                }catch(Exception e){
                   Console.WriteLine("Error uploading file: "+currentFile);
                   S3UploadLibrary.writeToLog("Error uploading file: "+currentFile);
                  Console.WriteLine( S3UploadLibrary.getErrorMessage(e));
                  S3UploadLibrary.writeToLog(S3UploadLibrary.getErrorMessage(e));

                }


              }else{
                   string[] gways = S3UploadLibrary.s3Gateways.Select(x =>x.getUrl()).ToArray();
                   Console.WriteLine("Error connecting to gateways provided in configuration:"+String.Join(";",gways));
                   S3UploadLibrary.writeToLog("Error connecting to gateways provided in configuration:"+String.Join(";",gways));


              }
  return uploadedItems;
         }
        
        public List<S3UploadEntity> getEntitiesToBeUploaded( List<S3UploadEntity> allEntitiesInFolder,  List<S3UploadItem> alreadyUploadedItems){

                List<S3UploadEntity> uploadableEntities =  new List<S3UploadEntity>();
                if(alreadyUploadedItems.Count == 0){
                    uploadableEntities = allEntitiesInFolder;
                }else{ 
                     List<string>  successfullyUploadedFileNames = alreadyUploadedItems.Where(x =>x.getUploadStatus() ==S3UploadLibrary.SUCCESSFUL).Select(x => x.getFileName()).ToList();
                foreach(S3UploadEntity entity in allEntitiesInFolder){
                          string fileName =  entity.getFullName();
                          if(!successfullyUploadedFileNames.Contains(fileName)){
                                uploadableEntities.Add(entity);
                          }
                }
                 }
                    return uploadableEntities;
                }

        public List<S3UploadItem> getPreviouslyUploadedItems(){
            List<S3UploadItem> uploadEntities   =  new List<S3UploadItem>();
            string  rawSelect   = S3UploadLibrary.sqliteHelper.getQueryTemplate("select");
            string  tableName    = "upload_items";
            string  columnNames  =  "*";
            DataTable uploadedItemTable = S3UploadLibrary.getData(tableName,columnNames, null);
            
            uploadEntities = (from DataRow dr in uploadedItemTable.Rows  
                select new S3UploadItem()  
                {  
                itemID              =  Convert.ToInt64(dr["item_id"]), 
                sessionID           =  Convert.ToInt64(dr["session_id"]), 
                parentFolder        = dr["parent_folder"].ToString(),  
                fileName            = dr["file_name"].ToString(),  
                bucketName          = dr["bucket_name"].ToString(),
                creationTime        = DateTime.Parse(dr["creation_time"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                fileSize            = Convert.ToInt64(dr["file_size"]),
                uploadStartTime     = DateTime.Parse(dr["upload_start_time"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                uploadEndTime       = DateTime.Parse(dr["upload_end_time"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                fileUrl             = dr["file_url"].ToString(),   
                uploadStatus        = Convert.ToInt32(dr["upload_status"].ToString())
                 }).ToList();  

                return uploadEntities;


        }
        public List<S3UploadEntity> getUploadEntitiesFromPath(string targetDirectory, bool scanSubfolders){
		   List<S3UploadEntity> uploadEntities   =  new List<S3UploadEntity>();
		   if(Directory.Exists(targetDirectory)){
             
				
				string [] fileEntries = Directory.GetFiles(targetDirectory);

				foreach(string fileName in fileEntries){
						uploadEntities.Add( new S3UploadEntity(fileName));
				}
					
			if(scanSubfolders){
				 
				string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
				foreach(string subdirectory in subdirectoryEntries){
					getUploadEntitiesFromPath(subdirectory,scanSubfolders);
				}
				 
			 }

			}
			
			return uploadEntities;
			
	}

     public  void setUploadSession(S3UploadSession session){
           this.uploadSession = session;
     }

     
     public  S3UploadSession getUploadSession( ){
           return this.uploadSession;
     }
        public static void Main (string[] args){
                string configFile 		= ""; 
            try {	
                for(int i =0; i< args.Length; i++){
                    
                    if (args[0].ToLower()=="-h" ||args[0].ToLower()=="help" || args[0].ToLower()=="/?" || args[0].ToLower()=="?" ){
                        
                        Console.WriteLine(" This application uploads files from a specified location to a specified S3 bucket");
                        Console.WriteLine(" Usage: ");	
                        Console.WriteLine(" -c: This parameter is used to specify the configuration file to be used.");
                        Console.WriteLine(" -h: This parameter is used to print this help message.");	
                                                        
                    } else if  ((i+1)< args.Length ) {
                        if(args[i].ToLower()=="-c" && (args[(i+1)] != null && args[(i+1)].Length!=0)){
                        configFile =  args[(i+1)];	
                    }
                }
            }	
                if(string.IsNullOrEmpty(configFile)){
                    
                    new  S3Uploader();
                    
                }else {					
                    new  S3Uploader(configFile);
                }
            }catch(Exception e) {
                
                    Console.WriteLine( e.Message);
                    Console.WriteLine(e.StackTrace);

            
            }

        }

    }
    
    
    }