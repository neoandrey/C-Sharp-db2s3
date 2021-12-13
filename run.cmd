@echo off 
echo "Do you want to commit the current  version of the application? (type  'y' or  'n'): "
@echo off 
set /p commit_response=  

if  %commit_response%==y (
  @echo off 
  echo 'Please type commit  message: 
  set /p commit_message=  
  git add  .
  git commit -m  "%commit_message%"
) 

cd %cd%
%cd%\bin\ArbiterCopy.exe

pause