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
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.Reflection;

namespace db2s3{

            public class   S3UploadLibrary{

               public  const int PENDING = 0;
               public  const int SUCCESSFUL = 1;

               public  const int FAILED = 2;
                public  static System.IO.StreamWriter   	fs;
                public  static string 					logFile		           = AppDomain.CurrentDomain.BaseDirectory+"..\\log\\db2s3_"+DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")+".log";
               public static string                         directoryOfUploadfiles;
               public static List<S3Gateway>                s3Gateways;     
               public static string                         bucketName;
               public static string                         serverName;
               public static string                         serverIPAddress;
               public static string                         additionalServerInfo;
               public static string                         sqliteDatabaseFile;
               public static string                         accessID;
               public static string                         accessKey;
               public static string                         profilePath;
               public static bool                           useProfile;
               public static string                         centralInventoryServer;
               public static string                         centralInventoryUser;
               public static string                         centralInventoryPassword;
               public static string                         centralInventoryDBName;
               public static bool                           runOnSchedule;
               public static string                         scheduleName;
               public  static string 				    	fromAddress   	    		 = "db2s3@interswitchgroup.com";
               public  static string 				     toAddress                 = "";
               public  static string 				    	bccAddress    	   	      = "";
               public  static string 					ccAddress     	   	      = "";
               public  static string 					smtpServer    			 = "172.16.10.223";
               public  static int 				     smtpPort     	    		 = 25;
               public  static string 					sender             		 = "db2s3@interswitchgroup.com";
               public  static string 				     senderPassword 	      = "";
               public  static bool 				     isSSLEnabled  		      = false;
			public  static S3UploadConfig               uploadConfig   	      = new  S3UploadConfig();
			public  static string 					configFileName            = AppDomain.CurrentDomain.BaseDirectory+"..\\conf\\db2s3.json";
               public  static bool                         sendNotification          =  false;
               public  static string                       alternateRowColour        =  ""; 
               public  static string                       emailFontFamily           =  "";
               public  static string                       emailFontSize             =  "";
               public  static string                       colour                    =  ""; 
               public  static string                       borderColour              =  ""; 
               public   static object                      fileLocker                =  new object();
               public  static SQLiter                     sqliteHelper;
               public static  StringBuilder                 emailError                =   new StringBuilder();
               public static string                         sqliteDBName;
               public static string                         region;    
               public static string                         profileName;  

               public static string                         sqliteConfigFile;  

               public  static  int                          s3ReadWriteTimeOut;

               public static   int                          s3ConnectionTimeOut;      

               public static  string                        emailSeparator;

            public  S3UploadLibrary(){ 
                    if (!File.Exists(logFile))  {
                          fs = File.CreateText(logFile);
                    }else{
                          fs = File.AppendText(logFile);
                    } 
				initS3UploadLibrary();
			 }
      		public  S3UploadLibrary(string  cfgFile){
                    if (!File.Exists(logFile))  {
                          fs = File.CreateText(logFile);
                    }else{
                          fs = File.AppendText(logFile);
                    } 
				initS3UploadLibrary();
					  
                    if(!string.IsNullOrEmpty(cfgFile) ){


                         configFileName           =  cfgFile.Contains("\\\\")? cfgFile:cfgFile.Replace("\\", "\\\\");
                                             
                    } 

                    try{
                              if(File.Exists(configFileName)){


                              initS3UploadLibrary();

                              }
                         }catch(Exception e){

                              Console.WriteLine("Error reading configuration file "+cfgFile+": "+e.Message+"\n"+e.ToString());
                              Console.WriteLine(e.StackTrace);
                              log("Error reading configuration file "+cfgFile+": "+e.Message+"\n"+e.ToString());
                              log(e.StackTrace);

                         }
                    	
		       	         		
				}
               public  void  initS3UploadLibrary(){

                    Console.WriteLine("Reading configuration file: "+configFileName);
                   	log("===========================Started Database to StorageGRID Upload Session at "+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"==============================");	 
                    
                    readConfigFile();

		

                }
               public  static  void readConfigFile(){

                    writeToLog("Reading contents of configuration file: "+configFileName);
                    Console.WriteLine("Reading contents of configuration file: "+configFileName);
                    try{
                        string  propertyString                  = File.ReadAllText(configFileName);
                        uploadConfig                            = Newtonsoft.Json.JsonConvert.DeserializeObject<S3UploadConfig>(propertyString); 
                        directoryOfUploadfiles                  = uploadConfig.directoryOfUploadfiles;
                        s3Gateways	                             = uploadConfig.s3Gateways;
                        logFile    	                        = uploadConfig.logFileName;
                        bucketName 	                        = uploadConfig.bucketName !=null ? uploadConfig.bucketName.Replace("_","-") :"db2s3";
                        serverName	                             = uploadConfig.serverName  ;
                        serverIPAddress 	                   = uploadConfig.serverIPAddress  ;
                        additionalServerInfo                    = uploadConfig.additionalServerInfo ;
                        sqliteDBName                            = uploadConfig.sqliteDatabaseName;
                        sqliteDatabaseFile                      = uploadConfig.sqliteDatabaseFile ;   
                        accessID 	                             = uploadConfig.accessID ;
                        accessKey	                             = uploadConfig.accessKey ;
                        profilePath	                         = uploadConfig.profilePath ;
                        useProfile	                             = uploadConfig.useProfileFile ;
                         profileName                              = uploadConfig.profileName;
                        centralInventoryServer                  = uploadConfig.centralInventoryServer ;
                        centralInventoryUser                    = uploadConfig.centralInventoryUser ;
                        centralInventoryPassword                = uploadConfig.centralInventoryPassword ;
                        centralInventoryDBName                  = uploadConfig.centralInventoryDatabaseName ;
                        runOnSchedule	 	                    = uploadConfig.runOnSchedule;
                        scheduleName	 	                    = uploadConfig.scheduleName ;
                        sendNotification 	                    = uploadConfig.sendNotification ;
                        toAddress 	 	                    = uploadConfig.toAddress  ;
                        fromAddress		                    = uploadConfig.fromAddress ;
                        bccAddress		                    = uploadConfig.bccAddress  ;
                        ccAddress		                         = uploadConfig.ccAddress  ;
                        smtpServer	                              = uploadConfig.smtpServer  ;
                        smtpPort		                         = uploadConfig.smtpPort != null && uploadConfig.smtpPort.Length>0 ?Convert.ToInt32(uploadConfig.smtpPort):0  ;
                        sender			                    = uploadConfig.sender     ;
                        senderPassword		                    = uploadConfig.senderPassword  ;
                        isSSLEnabled		                    = uploadConfig.isSSLEnabled  ;
                        alternateRowColour	                    = uploadConfig.alternateRowColour;
                        emailFontFamily		                    = uploadConfig.emailFontFamily;
                        emailFontSize		                    = uploadConfig.emailFontSize;
                        colour			                    = uploadConfig.colour;
                        borderColour		                    = uploadConfig.borderColour; 
                        region                                   = uploadConfig.region;
                        sqliteConfigFile                         = uploadConfig.sqliteConfigFile;
                        sqliteHelper                             = new SQLiter(sqliteDBName, sqliteConfigFile);  
                        s3ConnectionTimeOut                      = uploadConfig.s3ConnectionTimeOut;
                        s3ReadWriteTimeOut                       = uploadConfig.s3ReadWriteTimeOut;
                        emailSeparator                           = uploadConfig.emailSeparator;
                                    
                   
                    }catch(Exception e){

                    Console.WriteLine("Error reading configuration file: "+e.Message);
                    Console.WriteLine(e.StackTrace);

                    }
                    writeToLog("Configuration successfully loaded. ");
                    Console.WriteLine("Configuration successfully loaded.");
                    
               }

               public static void  writeToLog(string logMessage){

                    Thread writeThread =  new  Thread(()=>{   
                         try{
                              lock(fileLocker){
                                   fs.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"=>"+logMessage);
                                   fs.Flush();
                              }
                         }catch(Exception  e){

                              Console.WriteLine("Error writing data to log file: "+e.Message);
                              Console.WriteLine(e.StackTrace);

                         }
                    });

                    writeThread.Start();
                    writeThread.Join();
                    
               }

			public static Dictionary<string,string>readJSONMap(ArrayList rawMap){

                    Dictionary<string, string> tempDico = new  Dictionary<string, string>();
                    string tempVal  ="";
                    if(rawMap!=null)
                    foreach(var keyVal in rawMap){
                                
                        tempVal = keyVal.ToString();
                        if(!string.IsNullOrEmpty(tempVal)){
                            tempVal = tempVal.Replace("{","").Replace("}","").Replace("\"","").Trim();
                           // Console.WriteLine("tempVal: "+tempVal);
                            if(tempVal.Split(':').Count() ==2)tempDico.Add(tempVal.Split(':')[0].Trim(),tempVal.Split(':')[1].Trim());
                            else if(tempVal.Split(':').Count() ==3  && "ABCDEFGHIJKLMNOPQRSTUVQXYZ".Contains(tempVal.Split(':')[1].Trim().ToUpper() )) {
                                tempDico.Add(tempVal.Split(':')[0].Trim(),tempVal.Split(':')[1].Trim()+":"+tempVal.Split(':')[2].Trim());
                            }  
                        }  

                    }
                  return tempDico;
            }
                public static Dictionary<string, bool>convertToBoolMap(Dictionary<string,string> rawMap){

                    Dictionary<string, bool> tempDico = new  Dictionary<string, bool>();

                    if(rawMap!=null)
                   	foreach(KeyValuePair<string, string> parameter in rawMap){
						
							tempDico.Add(parameter.Key, bool.Parse(parameter.Value));

						}
                  return tempDico;
            }
          public static DataTable getDataTable<T>(List<T> items)
          {
               DataTable dataTable = new DataTable(typeof(T).Name);
               PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
               foreach (PropertyInfo prop in Props)
               {
                    dataTable.Columns.Add(prop.Name);
               }
               foreach (T item in items)
               {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                         values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
               }
               
               return dataTable;
          }
          
          public static void exportCSV (DataTable dtDataTable, string strFilePath) { 

                 try{ 

                    StreamWriter sw = new StreamWriter(strFilePath, false); 

                    Console.WriteLine("Exporting data to file: "+strFilePath); 
                    S3UploadLibrary.writeToLog("Exporting data to file: "+strFilePath);    

                    for (int i = 0; i < dtDataTable.Columns.Count; i++) {  
                         sw.Write(dtDataTable.Columns[i]);  
                         if (i < dtDataTable.Columns.Count - 1) {  
                         sw.Write(",");  
                         }  
                    }  
                    sw.Write(sw.NewLine);  
                    foreach(DataRow dr in dtDataTable.Rows) {  
                    for (int i = 0; i < dtDataTable.Columns.Count; i++) {  
                    if (!Convert.IsDBNull(dr[i])) {  
                         string value = dr[i].ToString();  
                         if (value.Contains(',')) {  
                                   value = String.Format("\"{0}\"", value);  
                                   sw.Write(value);  
                         } else {  
                              sw.Write(dr[i].ToString());  
                         }  
                    }  
                    if (i < dtDataTable.Columns.Count - 1) {  
                         sw.Write(",");  
                    }  
                    }  
                    sw.Write(sw.NewLine);  
                    }  
                    sw.Close();  
                     }catch(Exception e){

                  Console.WriteLine("Error exporting data to file: "+strFilePath); 
                  S3UploadLibrary.writeToLog("Error exporting data to file: "+strFilePath);                                          
                  Console.WriteLine( S3UploadLibrary.getErrorMessage(e));
                  S3UploadLibrary.writeToLog(S3UploadLibrary.getErrorMessage(e));
 
            }
          } 
          public static long getLastID(string tableName, string columnName, string  columnAlias){
               long lastUploadSessionID  = 0;
               string  rawSelect        =  sqliteHelper.getQueryTemplate("max");
               string fetchMaxUploadSessionScript = removeWhereClause(rawSelect.Replace("table_name_placeholder",tableName).Replace("max_column_name_placeholder",columnAlias).Replace("column_name_placeholder",columnName).Trim());
               DataTable sessionUploadTable = sqliteHelper.getDataFromScript(fetchMaxUploadSessionScript);
               lastUploadSessionID      =  sessionUploadTable.Rows[0].Field<Int64>(columnAlias);
               return lastUploadSessionID;     
          }

          public static DataTable getData(string tableName, string columnNames, Dictionary<string,Object> whereMap){
                 
                 string  rawSelect        =  sqliteHelper.getQueryTemplate("select");
                 string  whereClause      =  "";
                 string  selectScript     =  "";
                 if (whereMap != null){
                     whereClause =  getColumnValueMapForWhere(whereMap, "=");
                     rawSelect   = rawSelect.Replace("column_value_set_map_placeholder",whereClause);
                 }else {

                     rawSelect   = removeWhereClause(rawSelect);
                 } 
                 selectScript = rawSelect.Replace("column_name_placeholder",columnNames).Replace("table_name_placeholder",tableName);
                 DataTable selectTable = sqliteHelper.getDataFromScript(selectScript);
                 return selectTable;

        } 

        public static string getErrorMessage(Exception e){

             StringBuilder errorBuilder   =  new StringBuilder();
                  errorBuilder.Append("\nError Message: "+e.Message+"\n");
                  errorBuilder.Append("\nError Source: "+e.Source+"\n");
                  errorBuilder.Append("\nError Details: "+e.ToString()+"\n");

               return errorBuilder.ToString();
        }

          public static string getColumnValueMapForWhere(Dictionary<string,object> rawMap, string comparison){

                    StringBuilder  whereClause  =  new StringBuilder();
                    string  stringFormat = "{0}  {1} \'{2}\'";
                    string  intFormat    = "{0}  {1}  {2}";

                    if(rawMap!=null)
                   	foreach(KeyValuePair<string, object> parameter in rawMap){

                                  string  field    = parameter.Key;
                                  string  valueStr = "";
                                  object  value    = parameter.Value;
                                 if (value.GetType().ToString().ToLower() =="system.int16"|| value.GetType().ToString().ToLower()== "system.int32"  || value.GetType().ToString().ToLower()== "system.int64"){
                                      valueStr = String.Format(intFormat,  field,comparison, value);
                                 }else if(value.GetType().ToString().ToLower()== "system.collections.generic.list`1[system.object]"){
                                       StringBuilder listItems = new StringBuilder();
                                       listItems.Append(field);
                                       listItems.Append(comparison);
                                       listItems.Append("(");
                                       IList tempList = (IList)value;
                                       foreach(var item in tempList){
                                             if (item.GetType().ToString().ToLower() =="system.int16"|| item.GetType().ToString().ToLower()== "system.int32"  || item.GetType().ToString().ToLower()== "system.int64"){
                                               listItems.Append(String.Format("{0}",  item));
                                             }else{
                                                   listItems.Append(String.Format("'{0}'",  item));   
                                             }
                                             listItems.Append(",");
                                       }
                                       listItems = removeNLastChars(listItems,1);
                                       listItems.Append(")");
                                       valueStr = listItems.ToString();
                                      
                                 }else {
                                      valueStr = String.Format(stringFormat,  field,comparison, value);
                                 }
						
							whereClause.Append(valueStr).Append(',');
						}
                  return whereClause.ToString().EndsWith(",") ? removeNLastChars(whereClause,1).ToString():whereClause.ToString();
            }

          public  static int getCount(string tableName, string columnName, string columnAlias, Dictionary<string, Object> colValMap){
               int uploadCount          = 0;
               string  rawSelect        =  sqliteHelper.getQueryTemplate("count");
               string fetchMaxUploadSessionScript = "";
               if (colValMap == null){
                         fetchMaxUploadSessionScript =   removeWhereClause(rawSelect.Replace("column_name_placeholder",columnName).Replace("table_name_placeholder",tableName).Replace("max_column_name_placeholder",columnAlias).Trim());
               }else{

                    string  whereClause = getColumnValueMapForWhere(colValMap, "=");
                    fetchMaxUploadSessionScript = rawSelect.Replace("column_name_placeholder",columnName).Replace("table_name_placeholder",tableName).Replace("max_column_name_placeholder",columnAlias).Replace("column_value_set_map_placeholder",whereClause).Trim();

               }
               
               DataTable countTable = sqliteHelper.getDataFromScript(fetchMaxUploadSessionScript);
               uploadCount          =  countTable.Rows[0].Field<Int32>(columnAlias);
               return uploadCount;     
           }

           public static bool updateData(string tableName, Dictionary<string, Object> colValMap,Dictionary<string, Object> whereClauseMap ){
               bool isUpdated                  = false;
               string  rawUpdate               = sqliteHelper.getQueryTemplate("update");
               string  updateScript            = "";
               string  updateClause            = getColumnValueMapForWhere(colValMap,"=");
               string  whereClause             = getColumnValueMapForWhere(whereClauseMap,"=");

               updateScript                    = updateScript.Replace("table_name_placeholder",tableName).Replace("column_value_set_map_placeholder",updateClause).Replace("column_value_set_map_placeholder",whereClause).Trim();      
               isUpdated                       = sqliteHelper.executeScript(updateScript);
               return isUpdated;     
           }

           public static bool removeData(string tableName, Dictionary<string, Object> colValMap){
               bool isDeleted                  = false;
               string  rawUpdate               = sqliteHelper.getQueryTemplate("delete");
               string  deleteScript            = "";
               string  deleteClause            = getColumnValueMapForWhere(colValMap,"=");
               deleteScript                    = deleteScript.Replace("table_name_placeholder",tableName).Replace("column_value_set_map_placeholder",deleteClause.ToString()).Trim();      
               isDeleted                       = sqliteHelper.executeScript(deleteScript);
               return isDeleted;     
           }
           public static bool saveData(string tableName, Dictionary<string, Object> colValMap){
               bool isSaved                    = false;
               string  rawInsert               = sqliteHelper.getQueryTemplate("insert");
               string  insertScript            = "";
               StringBuilder  fields           = new StringBuilder();
               StringBuilder  values           = new StringBuilder();

               string stringFormat             = "'{0}'";
               string intFormat                = "{0}";

               foreach(KeyValuePair<string, Object>  fieldVal in  colValMap){
                      string field    = fieldVal.Key;
                      object value    = fieldVal.Value!=null?fieldVal.Value:"";
                      string valueStr = "";
                      if (value.GetType().ToString().ToLower() =="system.int16"|| value.GetType().ToString().ToLower()== "system.int32"  || value.GetType().ToString().ToLower()== "system.int64"){
                                      valueStr = String.Format(intFormat, value);
                         }else {
                              valueStr = String.Format(stringFormat, value);
                         }
                                 fields.Append(field).Append(",");
                                 values.Append(valueStr).Append(",");
               }

               string columns      = removeNLastChars(fields,1).ToString();
               string insertValues = removeNLastChars(values,1).ToString();
               insertScript        = rawInsert.Replace("table_name_placeholder",tableName).Replace("column_names_placeholder",columns).Replace("values_placeholder",insertValues).Trim();      
               isSaved             = sqliteHelper.executeScript(insertScript);
               return isSaved;     
           }
			public static  void log(string logMessage){
				fs.WriteLine(logMessage);
				fs.Flush();	
			}
            
               public  static  StringBuilder removeNLastChars(StringBuilder builder,  int  numOfChars ){
                    StringBuilder temp  = builder;
                    for (int i=0; i<numOfChars;  i++  ){
                         temp.Length--;
                    }
                    return temp;
               }

               public  static string checkUNCFilePath(string filePath){
                         string  newFilePath="";
                         if(filePath.Contains("\\\\")){
                              newFilePath = filePath;
                         }else{
                             newFilePath=  String.Format("@'{0}'", filePath);
                         }

                         return newFilePath;

               }
            
               public static  string removeWhereClause(string clause){
                     return clause.Replace("WHERE","").Replace("column_value_set_map_placeholder","");
               }
            
     }
} 