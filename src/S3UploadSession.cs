using System; 
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Data;
using System.Globalization;
using System.Data.SQLite;

namespace db2s3{

            public class   S3UploadSession{
                
                public long  sessionID;
                public string  serverName;

                public string serverIP;
                public string bucketName;

                public string uploadPath;

                public long   uploadCount;

                public DateTime startTime;

                public DateTime endTime;

                public int  status;

                public string gateway;

                public S3UploadSession(){

                }


                public string getTableColumnName(string column){
                    if(column.ToLower().Equals("sessionid")){
                         return  "session_id";
                    }else if(column.ToLower().Equals("servername")){
                         return  "server_name";
                    }else if(column.ToLower().Equals("serverip")){
                         return  "server_ip";
                    }else if(column.ToLower().Equals("bucketname")){
                         return  "bucket_name";
                    }else if(column.ToLower().Equals("uploadpath")){
                         return  "upload_path";
                    }else if(column.ToLower().Equals("starttime")){
                         return  "start_time";
                    }else if(column.ToLower().Equals("endtime")){
                         return  "end_time";
                    }else if(column.ToLower().Equals("status")){
                         return  "status";
                    }else if(column.ToLower().Equals("gateway")){
                         return  "gateway";
                    }
                       return "";
                }

          public long getSessionID(){
          return this.sessionID;
          }


          public string getServerName(){
          return this.serverName;
          }

          public string getServerIP(){
          return this.serverIP;
          }


          public string getBucketName(){
          return this.bucketName;
          }

          public string getUploadPath(){
          return this.uploadPath;
          }

          public long getUploadCount(){
          return this.uploadCount;
          }

          public DateTime getStartTime(){
          return this.startTime;
          }

          public DateTime getEndTime(){
          return this.endTime;
          }


          public int getStatus(){
          return this.status;
          }


          public string  getGateway(){
          return this.gateway;
          }

          public void  setSessionID(long id){
               this.sessionID = id;
          }


          public void  setServerName(string name){
               this.serverName = name;
          }

          public  void setServerIP(string ip){
               this.serverIP  = ip;
          }


          public void setBucketName(string bucket){
               this.bucketName =  bucket;
          }

          public void setUploadPath(string path){
               this.uploadPath =  path;
          }

          public void  setUploadCount(long count){
               this.uploadCount =  count;
          }

          public void setStartTime(DateTime start){
               this.startTime = start;
          }

          public void setEndTime(DateTime end){
               this.endTime = end;
          }


          public void setStatus(int status){
               this.status = status;
          }


          public void setGateway(string gateway){
               this.gateway = gateway;
          }
     
          public long getLastS3UploadSessionID(){
               return S3UploadLibrary.getLastID( "upload_sessions", "session_id", "max_session");     
          }

          
          public static int getS3UploadSessionCount(Dictionary<string, object> colValMap){
                    return S3UploadLibrary.getCount("upload_session", "session_id", "session_count", null);
          }

          public List<S3UploadSession> get(Dictionary<string, object> colValMap){
              DataTable uploadTable =  S3UploadLibrary.getData("upload_session", "*", colValMap);
              List<S3UploadSession>  uploadSessions = (from DataRow dr in uploadTable.Rows  
              select new S3UploadSession()  {  
                sessionID     =  Convert.ToInt64(dr["session_id"]),  
                serverName    = dr["server_name"].ToString(),  
                serverIP      = dr["server_ip"].ToString(),  
                bucketName    = dr["bucket_name"].ToString(),
                uploadPath    = dr["upload_path"].ToString(),
                uploadCount   = Convert.ToInt64(dr["upload_count"]),
                startTime     = DateTime.Parse(dr["start_time"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                endTime       = DateTime.Parse(dr["end_time"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                status        = Convert.ToInt32(dr["status"]),   
                gateway    = dr["gateway"].ToString()
              }).ToList(); 
           return uploadSessions;
          }

     public bool save(){
          Dictionary<string, object> valueMap  = this.getDict();
          string tableName = "upload_sessions";
          return S3UploadLibrary.saveData(tableName, valueMap);   
     }

   public string getColumns(){
         return  String.Join(",",new string[] {"session_id","server_name","server_ip","bucket_name","upload_path","upload_count","start_time","end_time" ,"file_url" ,"status","gateway" });
    }

          public Dictionary<string,object> getDict(){

               return  new Dictionary<string,object>(){
                              {"session_id" , this.sessionID},
                               {"server_name" , this.serverName},
                               {"server_ip"   , this.serverIP},
                               {"bucket_name"  , this.bucketName},
                               {"upload_path"  , this.uploadPath},
                               {"upload_count" , this.uploadCount},
                              { "start_time"   , this.startTime},
                              { "end_time"     , this.endTime},
                              { "status"       , this.status},
                              { "gateway"      , this.gateway }            
               };

          }
}


}