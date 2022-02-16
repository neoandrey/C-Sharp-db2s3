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
                public static string todaysDate    = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss"); 
                public  static string 					logFile		           = AppDomain.CurrentDomain.BaseDirectory+"..\\log\\db2s3_"+todaysDate+".log";
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
               public  static SQLiter                      sqliteHelper;
               public static  StringBuilder                 emailError                =   new StringBuilder();
               public static string                         sqliteDBName;
               public static string                         region;    
               public static string                         profileName;  

               public static string                         sqliteConfigFile;  

               public  static  int                          s3ReadWriteTimeOut;

               public static   int                          s3ConnectionTimeOut;      

               public static  string                        emailSeparator;

               public static  StringBuilder                 emailBody  =   new StringBuilder();

               public static string borderWidth;

               public static string headerBgColor;

               public static string filterFileExtension;

               public static int urlValidityDays;

               public static bool scanSubfolders = true;

               public static long uploadPartionSize = 0;

               public static  int  waitInterval    =  0;

               public static  int   threadCount     =   1;

               public static int    concurrentServiceRequests  =36;
               

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

                              Console.WriteLine("Error reading configuration file "+cfgFile);
                              log("Error reading configuration file "+cfgFile);
                              Console.WriteLine( getErrorMessage(e));
                              log(getErrorMessage(e));
                              emailError.AppendLine("<div style=\"color:red\">  " + getErrorMessage(e)+"</div>"); 

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
                        string  propertyString                   = File.ReadAllText(configFileName);
                        uploadConfig                             = Newtonsoft.Json.JsonConvert.DeserializeObject<S3UploadConfig>(propertyString); 
                        directoryOfUploadfiles                   = uploadConfig.directoryOfUploadfiles;
                        s3Gateways	                              = uploadConfig.s3Gateways;
                        logFile    	                         = string.IsNullOrEmpty(uploadConfig.logFileName)?logFile:uploadConfig.logFileName;
                        bucketName 	                         = !string.IsNullOrEmpty(uploadConfig.bucketName) ? uploadConfig.bucketName.Replace("_","-") :"db2s3";
                        serverName	                              = uploadConfig.serverName  ;
                        serverIPAddress 	                    = uploadConfig.serverIPAddress;
                        additionalServerInfo                     = uploadConfig.additionalServerInfo ;
                        sqliteDBName                             = uploadConfig.sqliteDatabaseName;
                        sqliteDatabaseFile                       = uploadConfig.sqliteDatabaseFile ;   
                        accessID 	                              = uploadConfig.accessID ;
                        accessKey	                              = uploadConfig.accessKey ;
                        profilePath	                         = uploadConfig.profilePath ;
                        useProfile	                              = uploadConfig.useProfileFile ;
                         profileName                             = uploadConfig.profileName;
                        centralInventoryServer                   = uploadConfig.centralInventoryServer ;
                        centralInventoryUser                     = uploadConfig.centralInventoryUser ;
                        centralInventoryPassword                 = uploadConfig.centralInventoryPassword ;
                        centralInventoryDBName                   = uploadConfig.centralInventoryDatabaseName ;
                        runOnSchedule	 	                    = uploadConfig.runOnSchedule;
                        scheduleName	 	                    = uploadConfig.scheduleName ;
                        sendNotification 	                    = uploadConfig.sendNotification ;
                        toAddress 	 	                         = uploadConfig.toAddress  ;
                        fromAddress		                    = string.IsNullOrEmpty(uploadConfig.fromAddress)?fromAddress:uploadConfig.fromAddress;
                        bccAddress		                         = uploadConfig.bccAddress  ;
                        ccAddress		                         = uploadConfig.ccAddress  ;
                        smtpServer	                              = uploadConfig.smtpServer  ;
                        smtpPort		                         = !string.IsNullOrEmpty(uploadConfig.smtpPort) && uploadConfig.smtpPort.Length>0 ?Convert.ToInt32(uploadConfig.smtpPort):0  ;
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
                        headerBgColor                            = uploadConfig.headerBgColor;
                        borderWidth                              = uploadConfig.borderWidth;
                        filterFileExtension                      = uploadConfig.filterFileExtension;
                        urlValidityDays                          = uploadConfig.urlValidityDays;
                        scanSubfolders                           = uploadConfig.scanSubfolders;  
                        uploadPartionSize                        = uploadConfig.uploadPartionSize; 
                        waitInterval                             = uploadConfig.waitInterval; 
                        threadCount                              = uploadConfig.concurrency > 0 ?uploadConfig.concurrency: threadCount;  
                        concurrentServiceRequests                = uploadConfig.concurrentServiceRequests > 0 ?uploadConfig.concurrentServiceRequests: concurrentServiceRequests;         
                    }catch(Exception e){

                         Console.WriteLine("Error reading configuration file: "+e.Message);
                         writeToLog("Error reading configuration file: "+e.Message);
                         Console.WriteLine( S3UploadLibrary.getErrorMessage(e));
                         writeToLog(getErrorMessage(e));
                         emailError.AppendLine("<div style=\"color:red\">  " + getErrorMessage(e)+"</div>");             

                    }
                    writeToLog("Configuration successfully loaded. ");
                    Console.WriteLine("Configuration successfully loaded.");
                    
               }

     public  static void sendMailNotification(Dictionary<string, DataTable> dTableMap){

            try {
	               Console.WriteLine("Sending Notification... ");
				S3UploadLibrary.writeToLog("Sending Notification... ");
                    emailBody  = new StringBuilder();
				emailBody.AppendLine("<div style=\"color:black\">Hi, All.</div>");
				emailBody.AppendLine("<div style=\"color:black\">\n</div>");
				emailBody.AppendLine("<div style=\"color:black\">Trust this meets you well</div>");
				emailBody.AppendLine("<div style=\"color:black\">\n</div>");
				emailBody.AppendLine("<div style=\"color:black\">Please see details for the file upload session for the contents of "+S3UploadLibrary.directoryOfUploadfiles+" folder below: </div>");
                    MailMessage message = new MailMessage();
	
				if ( !string.IsNullOrEmpty(S3UploadLibrary.toAddress)){

					foreach (var address in S3UploadLibrary.toAddress.Split(new [] {S3UploadLibrary.emailSeparator}, StringSplitOptions.RemoveEmptyEntries)){
							if(!string.IsNullOrWhiteSpace(address)){
										message.To.Add(address);   	
							}
					}
				}
				if ( !string.IsNullOrEmpty(S3UploadLibrary.ccAddress)){
					foreach (var address in S3UploadLibrary.ccAddress.Split(new [] {S3UploadLibrary.emailSeparator}, StringSplitOptions.RemoveEmptyEntries)){
						if(!string.IsNullOrWhiteSpace(address)){
							message.CC.Add(address);   	
						}
					}
				}
				if ( !string.IsNullOrEmpty(S3UploadLibrary.bccAddress)){
					foreach (var address in S3UploadLibrary.bccAddress.Split(new [] {S3UploadLibrary.emailSeparator}, StringSplitOptions.RemoveEmptyEntries)){
								if(!string.IsNullOrWhiteSpace(address)){
											message.Bcc.Add(address);   	
								}
					}
				}
			  if ( !string.IsNullOrEmpty(S3UploadLibrary.smtpServer)){
                         if ( !string.IsNullOrEmpty(S3UploadLibrary.toAddress)){
                              Console.WriteLine("Sending Notification... ");
                              S3UploadLibrary.writeToLog("Sending Notification... ");
                              message.From = new MailAddress(S3UploadLibrary.fromAddress);				
                              message.Subject = "S3 Upload Session Report for "+S3UploadLibrary.directoryOfUploadfiles+" at  "+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                              message.IsBodyHtml = true;
                    
                              emailBody.AppendLine("<style type=\"text/css\">");
                              emailBody.AppendLine("table.gridtable {");
                              emailBody.AppendLine("	font-family:"+S3UploadLibrary.emailFontFamily+";");
                              emailBody.AppendLine("	font-size:"+S3UploadLibrary.emailFontSize+";");
                              emailBody.AppendLine("	color:"+S3UploadLibrary.borderColour+";");
                              emailBody.AppendLine("	border-width:"+S3UploadLibrary.borderWidth+";");
                              emailBody.AppendLine("	border-color: "+S3UploadLibrary.borderColour+";");
                              emailBody.AppendLine("	border-collapse: collapse;");
                              emailBody.AppendLine("}");
                              emailBody.AppendLine("table.gridtable th {");
                              emailBody.AppendLine("	border-width: "+S3UploadLibrary.borderWidth+";");
                              emailBody.AppendLine("	padding: 8px;");
                              emailBody.AppendLine("	border-style: solid;");
                              emailBody.AppendLine("	border-color:"+S3UploadLibrary.borderColour+";");
                              emailBody.AppendLine("	background-color:"+S3UploadLibrary.headerBgColor+";");
                              emailBody.AppendLine("}");
                              emailBody.AppendLine("table.gridtable td {");
                              emailBody.AppendLine("	border-width:"+S3UploadLibrary.borderWidth+";");
                              emailBody.AppendLine("	padding: 8px;");
                              emailBody.AppendLine("	border-style: solid;");
                              emailBody.AppendLine("	border-color: "+S3UploadLibrary.borderColour+";");
                              emailBody.AppendLine("}");
                              emailBody.AppendLine("</style>");
                              
                              foreach(KeyValuePair<string, DataTable>  tabMap in dTableMap){
                                   emailBody.AppendLine("<div>\n</div>");
                              
                                   emailBody.AppendLine("<div><hr/></div>");
                                   emailBody.AppendLine("<div justify=\"left\"><table class=\"gridtable\">");
                                   emailBody.AppendLine("<thead bgcolor="+S3UploadLibrary.headerBgColor+">");
                                   emailBody.AppendLine("<caption style=\"color:gray\" justify=\"left\">"+tabMap.Key+"</caption>");
                                   foreach (DataColumn col in tabMap.Value.Columns){
                                        emailBody.AppendLine("<th>"+col.ColumnName+"</th>");
                                   }
                                   
                                   emailBody.AppendLine("</thead>");
                                   emailBody.AppendLine("<tbody>");
                                   
                                   int k = 0;
                              foreach (DataRow row in tabMap.Value.Rows) {
                              if(k%2!=0){
                                                  emailBody.AppendLine("<tr style=\"background-color:#ffffff\"> ");   // <td>"+row["INDEX_NO"]+"</td><td>"+row["PARAMETER"]+"</td><td>"+row["VALUE"]+"</td></tr>");
                                   } else{
                                                  emailBody.AppendLine("<tr style=\"background-color:"+S3UploadLibrary.alternateRowColour+"\">"); //<td>"+row["INDEX_NO"]+"</td><td>"+row["PARAMETER"]+"</td><td>"+row["VALUE"]+"</td></tr>");
                                   }
                                   
                                   foreach(DataColumn dCol in  tabMap.Value.Columns){
                                        if(dCol.ToString()=="no."){
                                                       emailBody.AppendLine("<td>"+(int.Parse(row[dCol.ToString()].ToString()) +1)+"</td>");

                                        }else  {

                                                  emailBody.AppendLine("<td>"+ row[dCol.ToString()].ToString()+"</td>");
                                        }						   
                                   }
                                   emailBody.AppendLine("</tr>");
                                   ++k;
                              }
                                   emailBody.AppendLine("</tbody>");
                              emailBody.AppendLine("</table></div>");
                              }
                         
                    emailBody.AppendLine("<div><hr/></div>");
                         if(!string.IsNullOrWhiteSpace(emailError.ToString())){
                              emailBody.AppendLine("<div><h3><ul> Error List </h3></ul></div>");
                              emailBody.AppendLine("<div>\n</div>");
                              emailBody.AppendLine(emailError.ToString());
                         }
                         emailBody.AppendLine("<div>\n</div>");
                              emailBody.AppendLine("<div>\n</div>");
                         emailBody.AppendLine("Thank you.");
                         
                    message.Body = emailBody.ToString();
                         SmtpClient smtpClient = new SmtpClient();
                         smtpClient.UseDefaultCredentials = true;

                         smtpClient.Host = S3UploadLibrary.smtpServer;
                         smtpClient.Port = Int32.Parse(S3UploadLibrary.smtpPort.ToString());
                         smtpClient.EnableSsl = S3UploadLibrary.isSSLEnabled;
                         smtpClient.Credentials = new System.Net.NetworkCredential(S3UploadLibrary.sender, S3UploadLibrary.senderPassword);
                         smtpClient.Send(message);
                    }else{
                              Console.WriteLine("Email could not be sent as there is no recepient in the toAddress field. ");
                              writeToLog("Email could not be sent as there is no recepient in the toAddress field.");
                    }
            }else{
                 	Console.WriteLine("Email could not be sent as there is no SMTP server specified: "+S3UploadLibrary.smtpServer);
				writeToLog("Email could not be sent as there is no SMTP server specified: "+S3UploadLibrary.smtpServer);

            }
		} catch(Exception  e){
			
				Console.WriteLine("Error sending email notification ");
				writeToLog("Error sending email notification");
				Console.WriteLine( S3UploadLibrary.getErrorMessage(e));
				S3UploadLibrary.writeToLog(S3UploadLibrary.getErrorMessage(e));   	
		}

	}

               public static void  writeToLog(string logMessage){

                    Thread writeThread =  new  Thread(()=>{   
                         try{
                              lock(fileLocker){
                                   fs.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"=>"+logMessage);
                                   fs.Flush();
                              }
                         }catch(Exception  e){

                         Console.WriteLine( getErrorMessage(e));
                         log(getErrorMessage(e));
                         emailError.AppendLine("<div style=\"color:red\">  " + getErrorMessage(e)+"</div>"); 

                         }
                    });

                    writeThread.Start();
                    writeThread.Join();
                    
               }

                public static Dictionary<string, bool>convertToBoolMap(Dictionary<string,string> rawMap){

                    Dictionary<string, bool> tempDico = new  Dictionary<string, bool>();

                    if(rawMap!=null)
                   	foreach(KeyValuePair<string, string> parameter in rawMap){
						
							tempDico.Add(parameter.Key, bool.Parse(parameter.Value));

						}
                  return tempDico;
            }
          public static DataTable getDataTable (List<Dictionary<string,object>> items)
          {
               DataTable dataTable = new DataTable();
               if (items.Count >0){
                    
                    foreach (string column in items[0].Keys)
                    {
                         dataTable.Columns.Add(column);
                        
                    }
               foreach(Dictionary<string,object> item in items){
                     var record = new object[item.Count];
                     int i = 0;
                    foreach(object value in item.Values)
                         {

                              record[i]=value;
                              ++i;
                         }
                          dataTable.Rows.Add(record);
               }
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
                         Console.WriteLine(value);
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
                         Console.WriteLine( getErrorMessage(e));
                         writeToLog(getErrorMessage(e));
                         emailError.AppendLine("<div style=\"color:red\">  " + getErrorMessage(e)+"</div>"); 
 
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
          var st = new StackTrace(e);
          var frames = st.GetFrames();
          var frame  = st.GetFrame(0);
          foreach(var tempFrame in  frames){
             if(string.IsNullOrEmpty(tempFrame.GetFileName())){
                  frame = tempFrame;
                  Console.WriteLine("\nFile Name: "+tempFrame.GetFileName()+"\n");
                   Console.WriteLine("\nLine Name: "+tempFrame.GetFileLineNumber()+"\n");
                    Console.WriteLine("\nLine Name: "+tempFrame.GetMethod()+"\n");
                  break;
             }

          }
          var line                     = frame.GetFileLineNumber();
          var fileName                 = frame.GetFileName();
           var column                  = frame.GetFileColumnNumber();
          StringBuilder errorBuilder   =  new StringBuilder();
          errorBuilder.Append("\nError Message: "+e.Message+"\n");
          errorBuilder.Append("\nError Source: "+e.Source+"\n");
           errorBuilder.Append("\nFile Name: "+fileName+"\n");
          errorBuilder.Append("\nLine number: "+line+"\n");
          errorBuilder.Append("\nColumn: "+column+"\n");
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