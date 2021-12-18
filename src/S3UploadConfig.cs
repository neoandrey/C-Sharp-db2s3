
using  System;
using  System.Collections;
using  System.Collections.Generic;
using  System.Text;

namespace db2s3
{

    public class S3UploadConfig{
        public string       directoryOfUploadfiles  { set; get;}
        public  List<S3Gateway>    s3Gateways { set; get;}
        public string       logFileName { set; get;}
        public string       bucketName  { set; get;}
        public string       serverName  { set; get;}
        public string       serverIPAddress  { set; get;}
        public string       additionalServerInfo { set; get;}

        public string      sqliteDatabaseName {set; get;}
        public string      sqliteDatabaseFile { set; get;}   
        public string      sqliteConfigFile {set; get;}       
        public string      accessID { set; get;}
        public string      accessKey { set; get;}
        public string      profilePath { set; get;}
        public string      profileName {set; get;}
        public bool        useProfileFile { set; get;}
        public string      centralInventoryServer { set; get;}
        public string      centralInventoryUser { set; get;}
        public string      centralInventoryPassword { set; get;}
        public string      centralInventoryDatabaseName { set; get;}
        public bool        runOnSchedule { set; get;}
        public string      scheduleName { set; get;}
        public bool        sendNotification { set; get;}
        public string      toAddress  { set; get;}
        public string      fromAddress { set; get;}
        public string      bccAddress  { set; get;}
        public string      ccAddress  { set; get;}
        public string      smtpServer  { set; get;}
        public string      smtpPort   { set; get;}
        public string      sender     { set; get;}
        public string      senderPassword  { set; get;}
        public bool        isSSLEnabled  { set; get;}
        public string      alternateRowColour{set; get;}
        public string      emailFontFamily{set; get;}
        public string      emailFontSize{set; get;}
        public string      colour {set; get;}
        public string      borderColour{set; get;}
        public string      region {set; get;}    
        public int         s3ConnectionTimeOut {get; set;}  
        public int         s3ReadWriteTimeOut {get; set;}  

        public string      emailSeparator {get; set;}


    }
}