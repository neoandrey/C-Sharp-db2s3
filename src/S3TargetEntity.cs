namespace db2s3 {
    public class S3TargetEntity{
        public string name;
        public string ipAddress;

        public string pathToBeUploaded;

        public bool scanSubfolders =false;

        public string additionalInfo ;


        public S3TargetEntity(string name, string ipAddress,string location,bool scanSub){
               this.setName(name);
               this.setIPAddress(ipAddress);
               this.setPathToBeUploaded(location);
               this.setScanSubfolders(scanSub);
        }

       public S3TargetEntity(string nameOrIP, bool scanSub, string location){
             string[] ipSegments = nameOrIP.Split('.');
               if(ipSegments.Length == 4){
                  this.setIPAddress(nameOrIP);
               }else{
                   this.setName(nameOrIP);
               }
               
               this.setScanSubfolders(scanSub);
               this.setPathToBeUploaded(location);
        }
        
       public S3TargetEntity(string nameOrIP, string location){
               string[] ipSegments = nameOrIP.Split('.');
               if(ipSegments.Length == 4){
                  this.setIPAddress(nameOrIP);
               }else{
                   this.setName(nameOrIP);
               }
               
               this.setPathToBeUploaded(location);
        }

        public void setName(string name){
            this.name = name;
        }

        public void setIPAddress(string ip){
            this.ipAddress = ip;

        }

        public void setPathToBeUploaded(string location){
            this.pathToBeUploaded = location;
        }
        
        public void setScanSubfolders(bool scf){
             this.scanSubfolders= scf;
        }

        public void setAdditionalInfo(string info){

             this.additionalInfo  = info;
        }
        public string getAdditionalInfo(){

            return this.additionalInfo;
        }

    }
    
 }