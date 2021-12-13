using System;

namespace db2s3 {

 public class S3Gateway {

     public string  address;
     public int  port;
     public string  protocol="https";

     public S3Gateway(string address,  int port ){
            this.setAddress(address);
            this.setPort(port);
     }

      public S3Gateway(string address,  int port, string protocol ){
            this.setAddress(address);
            this.setPort(port);
            this.setProtocol(protocol);
      }

      public void setAddress(string address){
            this.address = address;
      }
      public void setPort(int port){
            this.port = port;
      }
            public void setProtocol(string protocol){
            this.protocol = protocol;
      }
   
     public  string getAddress(){
            return this.address;
     }
     
     public  int getPort(){
            return this.port;
     }

    public  string getProtocol(){
            return this.protocol;
     }

     public string getUrl(){

          return String.Format( "{0}://{1}:{2}/",this.getProtocol(), this.getAddress(), this.getPort());

     }





 }


}