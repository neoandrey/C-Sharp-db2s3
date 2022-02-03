## db2s3: Upload Files to Simple Storage Service (S3) Platforms

This tool was built to help copy files from local storage on Windows machines to platforms that run the Amazon Web Services'(AWS) Simple Storage Service (S3) protocol such as  AWS S3 itself and other related platforms that use this protocol. It is designed to be as easy to use as possible.  

The tool is highly customizable as it has a bunch of options that can be changed to meet the requirements of any environment. Ranging from using AWS profiles for connections to using IDs and keys from the command line. 

### Configuration File

The default configuration file is the dbs23 JSON file located in the conf folder of this repository. It contains the following parameters which are used by the application when it runs:
-  **directoryOfUploadfiles**:  This parameter specifies the folder on the local machine running the application that contains the files to be uploaded.
-  **filterFileExtension**: This is used to filter the folder specified above for specific file types.
-  **logFileName**: The parameter override the default name for the log file, db2s3_log.
-  **bucketName**: This specifies the name of the bucket on the S3 platform to which the files are uploaded.
-  **serverName**: This specifies the name of the machine that run this application.
-  **serverIPAddress**: This specifies the IP Address of the machine that run this application.
-  **additionalServerInfo**: This saves additional information about the server that runs the application just for reference purposes.
-  **sqliteDatabaseName**: This is used to determine the name of the local SQLite database used to manage the application data. The default file name is db2s3.
-  **sqliteDatabaseFile**: This is used to determine the file name of the local SQLite database.
-  **sqliteConfigFile**: This parameter specifies the configuration file that contains the SQL commands used to manage the internal SQLite database
-  **accessID**:  This parameter is used to specify the Access ID for accessing the S3 platform. It can be used in place of an AWS profile but should be used with care because of security reasons
-  **accessKey**: This specifes the Access Key used with the Access ID specified above to connect to the S3 platform.
-  **profilePath**: This specifies the path to the AWS profile file if the file is not saved in its default location.
-  **profileName**: The name of the aws profile to use as an AWS profile file might contain several profiles
-  **useProfileFile**: A boolean value that specifies whether to use the AWS profile or not.
-  **region**: The AWS region running the S3 endpoint
-  **s3Gateways**: The is used to specify a list of S3 Service URLs that the application should connect to. Each list should contain 3 of the following:

      1. *address:* An address (hostname or IP) of the S3 endpoint that is accessible from the host running the application.
      2. *port:* The port that the S3 endpoint works with.
      3. *protocol:* The protocol used bt the application to connect to the S3 endpoint
      4. 
-  **centralInventoryServer**: This is optional. An address (hostname or IP) of a server that stores information from several instances of this application running on different machines (In progress).
-  **centralInventoryUser**: This is optional. A valid user that can be used to connect to the central inventory server (In progress).
-  **centralInventoryPassword**: This is optional. A valid password that can be used to connect to the central inventory server (In progress).
-  **centralInventoryDBName**:  This is optional.The name of the database on the central server that stores information from all other machines.
-  **runOnSchedule**:  This parameter is supposed to make the application run on a schedule (In progress).
-  **scheduleName**: This is the name of the schedule to be used to run the application
-  **sendNotification**: This parameter determine if an email notification should be sent after an upload session.
-  **toAddress**: The address where the email alert should be sent to
-  **fromAddress**: The sender of the email alert
-  **bccAddress**: Blind Carbon-copy
-  **ccAddress**: Carbon copy
-  **smtpServer**: The SMTP server for sending email alert
-  **smtpPort**:The SMTP port for sending email alert
-  **sender**: The username of the account used to access the email server
-  **senderPassword**: The password used to access the email server
-  **emailSeparator**: The character used to sepatate email addresses
-  **isSSLEnabled**: This specifies if SSL should be used while sending the email
-  **alternateRowColour**: This specifies the alternate row color of the table within the email notification
-  **emailFontFamily**: This determines the font-family of the email
-  **emailFontSize**:This determines the font-sizw of the email
-  **colour**: This determines the body colour of the email
-  **borderColour**: This determines the border colour of the email
-  **s3ConnectionTimeOut**: This determines the connection time out for connecting to the s3 gateway
-  **s3ReadWriteTimeOut**: This determines the read time out for connecting to the s3 gateway
-  **borderWidth**: This determines width of the border of the table in the email alert
-  **headerBgColor**:This determines the border colour of the header of the email
-  **urlValidityDays**: This determines the number of days that the URL used to access the files uploaded to the s3Bucket will be valid.

## Parameter Override

Command line parameters can be used to override default configurations specified in the configuraion file.

          -h: This parameter is used to print this help message.		
          -c: This parameter is used to specify the configuration file to be used.
          -d: This parameter is used to specify the directory or folder to scan for upload items.
          -b: The  S3 bucket to upload items to.
          -s: The name of the server from which items are uploaded.
          -s: The IP address of the server from which items are uploaded.
          -a: The Access ID used to connect to the S3 gateway
          -k: The Access key used to connect to the S3 gateway
          -x: The file extension type to filter from the directory specified by the -d option.
          -f: A comma-separated list of files to be downloaded from the S3 bucket specified with the -b option.
          -o: The output folder where the files downloaded are saved. 

### CSV Exports

After every upload session that uploads data into a bucket, a CSV report is generated and saved to the .cav folder of the application

### Log

A log file is created for every run.


