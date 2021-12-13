using System.IO;
using System;

namespace db2s3 {
    public class S3UploadEntity{
        public string name;
        public string creationTime;
        public string  extension;
        public long size;
        public string filePath;  
        public string parentFolder;
        public string fullName;
        public int uploadStatus;
        public DateTime uploadTime;

        public long uploadSessionID;

        public S3UploadEntity(){
            
        }

        public S3UploadEntity(string name){

            try{
                if(File.Exists(name)){

                    FileInfo fileName = new FileInfo(name);
                    this.setName(fileName.Name);
                    this.setCreationTime(fileName.CreationTime.ToString());
                    this.setExtension(fileName.Extension);
                    this.setSize( fileName.Length);
                    this.setParentFolder( fileName.DirectoryName);
                    this.setFullName(fileName.FullName);

                }
            }catch(Exception e){

                Console.WriteLine("Error reading configuration file: "+e.Message+"\n"+e.ToString());
                Console.WriteLine(e.StackTrace);
                //writeToLog("Error reading configuration file: "+e.Message+"\n"+e.ToString());
                //writeToLog(e.StackTrace);

            }


        }

        public void setName(string name){
              this.name = name;
        }
        public void setCreationTime(string datetime){

            this.creationTime = datetime;
        }
      
        public void setExtension(string ext){
            this.extension = ext;
        }

        public void setSize(long size){
            this.size = size;
        }

        public void setParentFolder(string pFolder){
             this.parentFolder = pFolder;

        }

        public  void setFullName(string fullName){

             this.fullName = fullName;
        }
  
        public void setUploadStatus(int status){

            this.uploadStatus = status;
        }

        public void setUploadTime(DateTime dTime){

            this.uploadTime = dTime;

        }

        public void setUploadSessionID(long id){

            this.uploadSessionID = id;
        }

               public string getName(){
              return this.name;
        }
        public string getCreationTime(){

            return this.creationTime;
        }
      
        public string getExtension(){
             return this.extension;
        }

        public long getSize(){
            return  this.size;
        }

        public string getParentFolder(){
            return this.parentFolder;

        }

        public  string getFullName(){

             return this.fullName;
        }
  
        public int getUploadStatus(){

            return this.uploadStatus;
        }

        public DateTime getUploadTime(){

            return this.uploadTime;

        }

        public long getUploadSessionID(){

            return this.uploadSessionID;
        }
        

    }
    
    }