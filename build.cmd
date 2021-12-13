@echo off
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /r:"lib\System.Data.SQLite.dll","lib\Newtonsoft.Json.dll"  /t:library  /out:lib\s3upload_resources.dll src\S3Gateway.cs  src\S3TargetEntity.cs src\S3UploadConfig.cs src\S3UploadEntity.cs src\S3UploadLibrary.cs  src\SQLiter.cs src\S3UploadSession.cs src\S3UploadItem.cs
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /r:"lib\System.Data.SQLite.dll","lib\Newtonsoft.Json.dll","lib\s3upload_resources.dll","lib\AWSSDK.S3.dll","lib\AWSSDK.S3Control.dll","lib\AWSSDK.S3Outposts.dll","lib\AWSSDK.Core.dll" /out:bin\db2s3.exe src\S3Uploader.cs
pause
xcopy /e /f /y %cd%\lib\s3upload_resources.dll %cd%\bin\

if not exist .git\ (
   git init 
   git config  user.email  "neoandrey@yahoo.com"
   git config  user.name   "neoandrey@yahoo.com"
   echo "log" >.gitignore
   git add .  
   git commit -m "Initialize db2s3 Application " 
)

