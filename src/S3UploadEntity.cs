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
                Console.WriteLine( S3UploadLibrary.getErrorMessage(e));
                S3UploadLibrary.writeToLog(S3UploadLibrary.getErrorMessage(e));
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

        public string getEntityKey(){
             //return this.fullName.Replace(this.getName(),"").Replace(S3UploadLibrary.directoryOfUploadfiles,"").Replace('\\','/');
             string filePath = this.fullName;
             Boolean isFolder   = false;
             FileAttributes attr = File.GetAttributes(String.Format(@"{0}",filePath));

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory){
                isFolder = true;
            }


             string key= this.fullName.Replace(S3UploadLibrary.directoryOfUploadfiles,"").Replace('\\','/').Replace(' ','_');
             key = key.Trim();

            if( isFolder && !string.IsNullOrEmpty(key) &&!key.EndsWith("/")){

                key = key+"/";
            }
             if(key.StartsWith("/")){
                 key= key.Substring(1);
             }
             key   = string.IsNullOrEmpty(key.Trim())?this.getName().Trim(): key.Trim();

             return key ;
        }

        public long getUploadSessionID(){

            return this.uploadSessionID;
        }
        

    }
    
    }