 @echo off
 set proxy_filepath=%cd%\proxy.txt
 echo "Checking proxy path: %proxy_filepath%"
 IF EXIST "%proxy_filepath%" (
   SET /p proxy_url=<"%proxy_filepath%"
 )  ELSE (
   echo "Please type proxy URL:"
      set /p proxy_url=    
 )
echo  "%proxy_url%">%cd%\proxy.txt
 echo "setting proxy as:%proxy_url%"
 SET  HTTP_PROXY=%proxy_url%
 SET  HTTPS_PROXY=%proxy_url%
powershell  -File  %cd%\run.ps1
pause