@echo off
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe   /out:tests\bin\S3Test.exe   tests\src\S3Test.cs
.\tests\bin\S3Test.exe
pause



