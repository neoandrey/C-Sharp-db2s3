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

public class Queries{
    public string select {get; set;}

   public string insert {get; set;}
   public string update {get; set;}
   public string delete {get; set;}
   public string max {get; set;}
   public string min {get; set;}
   public string count {get; set;}

}
public class TableIndex{

         public string indexName {set; get;}
         public bool   isUnique  {set; get;}

         public ArrayList columns {set; get;}


}

 public class TableProperties{

        public string tableName  { set; get;}
        public string createScript  { set; get;}

        public List<TableIndex> indexes  { set; get;}

 }
    public class SQLiteConfig{
         public List<TableProperties> tables  { set; get;}
        public Queries   queries { set; get;}
    }

    public class SQLiter{

        public string sqliteDB;
        public string sqliteDBPathTemplate        = AppDomain.CurrentDomain.BaseDirectory + "\\..\\db\\{0}.sqlite";
        public string liteConnectionStringTemplate   = "Data Source={0};Version=3;";

        public static string liteConnectionString;

        public string sqliteConfigFile;

        public  SQLiteConfig  sqliteConfig;
        
        public string sqliteDBPath;


        public SQLiter(string db, string configFile){
            
               this.setConfigFile(configFile); 
               this.readConfigFile();
               this.setDatabaseName(db);
               this.setDBPath();
               this.setSqliteConnectionString();
               this.createDatabaseItems();

        }

        public void setDatabaseName(string db){
            this.sqliteDB = db;
        }


        public string getQueryTemplate(string queryType){

             string template  ="";

              if(queryType.ToLower().Equals("select")){

                   template = this.sqliteConfig.queries.select;
              }else if(queryType.ToLower().Equals("insert")){

                     template = this.sqliteConfig.queries.insert;
              }else if(queryType.ToLower().Equals("update")){

                     template = this.sqliteConfig.queries.update;
              }else if(queryType.ToLower().Equals("delete")){

                     template = this.sqliteConfig.queries.delete;
              }else if(queryType.ToLower().Equals("max")){

                     template = this.sqliteConfig.queries.max;
              }else if(queryType.ToLower().Equals("min")){

                     template = this.sqliteConfig.queries.min;
              }else if(queryType.ToLower().Equals("count")){

                     template = this.sqliteConfig.queries.max;
              }

          return   template;
        }
        public string getDatabaseName(){
            return this.sqliteDB;
        }

        public void setConfigFile(string config){
            this.sqliteConfigFile =config;
        }

        public string getConfigFile(){
            return this.sqliteConfigFile;
        }

       public void setConfig(SQLiteConfig config){
            this.sqliteConfig =config;
        } 

        public SQLiteConfig getConfig(){
            return this.sqliteConfig;
        }

        
        public  void readConfigFile(){
            string configFileName = this.getConfigFile().Contains(":")?this.getConfigFile():AppDomain.CurrentDomain.BaseDirectory+this.getConfigFile();
            S3UploadLibrary.writeToLog("Reading contents of SQLite configuration file: "+configFileName);
            Console.WriteLine("Reading contents of configuration SQLite file: "+configFileName);
            try{

             this.setConfig( Newtonsoft.Json.JsonConvert.DeserializeObject<SQLiteConfig>(File.ReadAllText(configFileName)));  

            }catch(Exception e){

                              Console.WriteLine("Error reading sqlite configuration file "+configFileName+": "+e.Message+"\n"+e.ToString());
                              Console.WriteLine(e.StackTrace);
                              S3UploadLibrary.writeToLog("Error reading sqlite configuration file "+configFileName+": "+e.Message+"\n"+e.ToString());
                              S3UploadLibrary.writeToLog(e.StackTrace);
 
            }
        }


                public void createDatabaseItems() {
                         List<TableProperties> tableProperties  = this.getConfig().tables;

                        if (!File.Exists(this.getDBPath())){
                             
                            SQLiteConnection.CreateFile(this.getDBPath());
                            foreach(var tableInfo in tableProperties){
                                string tableName = tableInfo.tableName;
                                if(!checkIfTableExists(tableName)){
                                    createTable(tableInfo.createScript);
                                    foreach(var indexInfo  in  tableInfo.indexes){
                                    createIndexOnColumn(tableName, String.Join( ",", indexInfo.columns),indexInfo.indexName,        indexInfo.isUnique);

                                 }


                               }

                            }
                
                    } else {

                        Console.WriteLine(String.Format("SQLite database {0} successfully initialised ", this.getDatabaseName()));
                        S3UploadLibrary.writeToLog(String.Format("SQLite database {0} successfully initialised ", this.getDatabaseName()));
                    }
                    }

               public string getDBPath(){
                    return this.sqliteDBPath;
                }

                public void setDBPath(){
                    this.sqliteDBPath   =  String.Format(sqliteDBPathTemplate, this.getDatabaseName());
                }
                 public void  setSqliteConnectionString(){

                      liteConnectionString =  String.Format(liteConnectionStringTemplate, this.getDBPath());

                }

                 public bool checkIfTableExists(string  tableName){ 
                        try{

                               using  (SQLiteConnection  liteConnect = new SQLiteConnection(liteConnectionString)){
                                    liteConnect.Open();
                                    string sql = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='"+tableName+"';";
                                    SQLiteCommand command = new SQLiteCommand(sql, liteConnect);
                                    Object result = command.ExecuteScalar();
                                    command.Dispose();
                                    if(result.ToString() == "1"){

                                    return true;
                                }
                               }
                        } catch(Exception e){
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                                S3UploadLibrary.emailError.AppendLine("<div style=\"color:red\">E " + e.Message); 
                                S3UploadLibrary.emailError.AppendLine("<div style=\"color:red\">"+e.StackTrace);
                        }
                        return false;
                        }

                        public  bool createTable(String createScript){
                           
                        try{
                               using  ( SQLiteConnection  liteConnect = new SQLiteConnection(liteConnectionString)){
                                        liteConnect.Open();
                                        string sql =createScript;
                                        
                                        SQLiteCommand command = new SQLiteCommand(sql, liteConnect);
                                        if(command.ExecuteNonQuery()>=0) return true;
                                       // command.Dispose();

                               }
                         } catch(Exception e){
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                                S3UploadLibrary.emailError.AppendLine("<div style=\"color:red\">" + e.Message); 
                                S3UploadLibrary.emailError.AppendLine("<div style=\"color:red\">"+e.StackTrace);
                        }
                        return false;
                        }

                    
                      public  bool createIndexOnColumn(string tableName, string  columnName, string indexName , bool  isUnique  ){                 
                        try{
                               using  ( SQLiteConnection  liteConnect = new SQLiteConnection(liteConnectionString)){
                                        liteConnect.Open();
                                        string sql =    isUnique? "CREATE UNIQUE INDEX  IF NOT EXISTS "+indexName+" ON  "+tableName+"("+columnName+")":"CREATE INDEX  IF NOT EXISTS "+indexName+" ON  "+tableName+"("+columnName+")" ;
                                        
                                        SQLiteCommand command = new SQLiteCommand(sql, liteConnect);
                                        if(command.ExecuteNonQuery()>=0) return true;
                                       // command.Dispose();

                               }
                         } catch(Exception e){
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                                S3UploadLibrary.emailError.AppendLine("<div style=\"color:red\">" + e.Message); 
                                S3UploadLibrary.emailError.AppendLine("<div style=\"color:red\">"+e.StackTrace);
                        }
                        return false;
                        }


                    public  bool executeScript(string sqlScript){

                        try{
                            
                           S3UploadLibrary.writeToLog("Running the  following on inventory database: "+sqlScript);
                           Console.WriteLine("Running the  following on inventory database: "+sqlScript);
                            using  (SQLiteConnection  liteConnect = new SQLiteConnection(liteConnectionString)){
                                    
                                    liteConnect.Open();
                                    SQLiteCommand command = new SQLiteCommand(sqlScript, liteConnect);
                                    command.CommandTimeout = -1;
                                    command.ExecuteNonQuery();
                                    Console.WriteLine("Query complete");
                                    S3UploadLibrary.writeToLog("Query complete");
                                    command.Dispose();
                                    return true;
                                }
                                    
                                    
                        } catch(Exception e){
                        
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                            S3UploadLibrary.writeToLog(e.Message);
                            S3UploadLibrary.writeToLog(e.StackTrace);
                            S3UploadLibrary.emailError.AppendLine("<div style=\"color:red\">  " + e.Message); 
                            S3UploadLibrary.emailError.AppendLine("<div style=\"color:red\">"+e.StackTrace);
                            return false;
                        }

                        
                    }
       
    
        public static  ArrayList getExistingTables(){
            ArrayList tableList           = new ArrayList();     
            System.Data.DataTable dt      = new DataTable();
            try
            {
 
                using (SQLiteConnection liteConnect = new SQLiteConnection(liteConnectionString))
                {
                    liteConnect.Open();
                    string sql = "SELECT name FROM sqlite_master WHERE type='table'";
                    SQLiteCommand cmd = new SQLiteCommand(sql, liteConnect);
                    cmd.CommandTimeout = 0;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    cmd.Dispose();


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

            }

            foreach(DataRow row in dt.Rows){
                tableList.Add(row["name"].ToString());    
            }

            return  tableList;

        }
        public  DataTable getDataFromScript(string theScript)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SQLiteConnection liteConnect = new SQLiteConnection(liteConnectionString))
                {
                    liteConnect.Open();
                    SQLiteCommand cmd = new SQLiteCommand(theScript, liteConnect);
                    Console.WriteLine("Executing SQLite script: " + theScript);
                    S3UploadLibrary.writeToLog("Executing  SQLite script: " + theScript);
                    cmd.CommandTimeout = -1;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    cmd.Dispose();
                    Console.WriteLine("Script executed successfully.");
                    S3UploadLibrary.writeToLog("Script executed successfully");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error running script: "+theScript+"=>"+e.Message);
                Console.WriteLine(e.StackTrace);
               S3UploadLibrary.writeToLog("Error running script: "+theScript+"=>"+e.Message);
               S3UploadLibrary.writeToLog(e.StackTrace);
            }
            return dt;
        } 
    }
}
