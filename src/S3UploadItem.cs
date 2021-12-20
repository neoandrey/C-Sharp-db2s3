using System; 
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Data;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;

namespace db2s3{

public class S3UploadItem{

    public   long itemID;       
      public   long           sessionID ;     
      public string      parentFolder;    
      public string         fileName;       
      public string          bucketName;     
      public  DateTime        creationTime;   
      public long          fileSize;      
      public  DateTime     uploadStartTime;
      public  DateTime    uploadEndTime;
      public string       fileUrl;    
      public int        uploadStatus ; 

      public long getItemID(){
          return this.itemID;
      }

      public long getSessionID(){
           return this.sessionID;
      }

      public string getParentFolder(){
          return this.parentFolder;
      }
     
      public string getFileName(){
          return this.fileName;
      }

      public string getBucketName(){ 
          return this.bucketName;
      }

      public DateTime getCreationTime(){
           return this.creationTime;
      }

      public long  getFileSize(){
           return this.fileSize;
      }

      public DateTime getUploadStartTime(){
           return this.uploadStartTime;
      }

      public DateTime getUploadEndTime(){
           return this.uploadEndTime;
      }
 
      public string getFileURL(){
          return this.fileUrl;
      }
    
    public int getUploadStatus(){
         return this.uploadStatus;
    }


      public void  setItemID(long id){
           this.itemID = id;
      }

      public void setSessionID(long id){
            this.sessionID = id;
      }

      public void setParentFolder(string parent){
           this.parentFolder = parent;
      }
     
      public void setFileName(string filename){
           this.fileName = filename;
      }

      public void setBucketName(string bucket){ 
           this.bucketName = bucket;
      }

      public  void setCreationTime(DateTime createTime){
            this.creationTime = createTime;
      }

      public void  setFileSize(long  size){
            this.fileSize =  size;
      }

      public void setUploadStartTime(DateTime startTime){
            this.uploadStartTime =  startTime;
      }

      public void setUploadEndTime(DateTime endTime){
            this.uploadEndTime = endTime;
      }

      public void setFileURL(string url){
          this.fileUrl = url;
      }
    
    public void setUploadStatus(int status){
          this.uploadStatus =  status;
    }
    public string getColumns(){
         return  String.Join(",",new string[] {"item_id"  ,"session_id","parent_folder","file_name","bucket_name","creation_time","file_size","upload_start_time","upload_end_time" ,"file_url" ,"upload_status" });
    }

     public List<S3UploadItem> get(Dictionary<string, object> colValMap){
              DataTable uploadTable =  S3UploadLibrary.getData("upload_items", "*", colValMap);
              List<S3UploadItem>  uploadItems = (from DataRow dr in uploadTable.Rows  
              select new S3UploadItem()  {  
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
           return uploadItems;
          }

     public bool save(){
          Dictionary<string, object> valueMap  = this.getDict();
          string tableName = "upload_items";
          return S3UploadLibrary.saveData(tableName, valueMap);   
     }

      public Dictionary<string,Object> convertToMap(){

               return  new Dictionary<string,Object>(){
                              {"itemID"              ,this.itemID },
                               {"sessionID"            , this.sessionID },
                               {"parentFolder"         , this.parentFolder },
                               {"fileName"             , this.fileName },
                               {"bucketName"           , this.bucketName },
                               {"creationTime"         , this.creationTime },
                               {"fileSize"             , this.fileSize },
                               {"uploadStartTime"     , this.uploadStartTime },
                               {"uploadEndTime"       , this.uploadEndTime },
                               {"fileUrl"              ,this.fileUrl },
                              {"uploadStatus"          , this.uploadStatus  }            
               };


}

          public Dictionary<string,Object> getDict(){

               return  new Dictionary<string,Object>(){
                              {"item_id"              ,this.itemID },
                               {"session_id"            , this.sessionID },
                               {"parent_folder"         , this.parentFolder },
                               {"file_name"             , this.fileName },
                               {"bucket_name"           , this.bucketName },
                               {"creation_time"         , this.creationTime },
                               {"file_size"             , this.fileSize },
                               {"upload_start_time"     , this.uploadStartTime },
                               {"upload_end_time"       , this.uploadEndTime },
                               {"file_url"              ,this.fileUrl },
                              {"upload_status"          , this.uploadStatus  }            
               };


}

	public string getTableColumnName(string column){
	
		if(column.ToLower().Equals("itemid")){
			 return  "item_id";
		}else if(column.ToLower().Equals("sessionid")){
			 return  "session_id";
		}else if(column.ToLower().Equals("parentfolder")){
			 return  "parent_folder";
		}else if(column.ToLower().Equals("filename")){
			 return  "file_name";
		}else if(column.ToLower().Equals("bucketname")){
			 return  "bucket_name";
		}else if(column.ToLower().Equals("creationtime")){
			 return  "creation_time";
		}else if(column.ToLower().Equals("filesize")){
			 return  "file_size";
		}else if(column.ToLower().Equals("uploadstarttime")){
			 return  "upload_start_time";
		}else if(column.ToLower().Equals("uploadEndTime")){
			 return  "upload_end_time";
		}else if(column.ToLower().Equals("fileurl")){
			 return  "file_url";
		}else if(column.ToLower().Equals("status")){
			 return  "status";
		}
		   return "";
	}

}

}