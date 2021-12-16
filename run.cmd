 @echo off
 set proxy_filepath=%cd%\proxy.txt
 echo "Checking proxy path: %proxy_filepath%"
 IF EXIST "%proxy_filepath%" (
   SET /p proxy_url=<"%proxy_filepath%"
 )  ELSE (
   echo "Please type proxy URL:"
      set /p proxy_url=    
 )
rem echo  %proxy_url%>%cd%\proxy.txt
rem echo "setting proxy as:%proxy_url%"
rem SET  HTTP_PROXY=%proxy_url%
rem SET  HTTPS_PROXY=%proxy_url%
powershell  -File  %cd%\run.ps1
pause