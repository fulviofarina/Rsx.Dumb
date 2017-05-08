REM echo cd %SYSTEMROOT%\SysWow64\ > "%temp%\batch1.bat"
SET ba1=%temp%\batch1.bat
SET ba2=%temp%\batch2.bat
SET vb=%temp%\vb.vbs
echo start "Container" /MIN /WAIT %SYSTEMROOT%\SYSNATIVE\dism /online /enable-feature /featurename:"MSMQ-Container" /NoRestart > %ba1%
echo start "Server" /MIN /WAIT %SYSTEMROOT%\SYSNATIVE\dism /online /enable-feature /featurename:"MSMQ-Server" /NoRestart > %ba2%
REM
echo Set UAC = CreateObject^("Shell.Application"^) > %vb%
set params = %*:"="
echo UAC.ShellExecute "cmd", "/q /c %temp%\batch1.bat", "", "runas", 1 >> %vb%
echo UAC.ShellExecute "cmd", "/q /c %temp%\batch2.bat", "", "runas", 1 >> %vb%
REM "%temp%\getadmin1.vbs" 
REM "%temp%\getadmin2.vbs" 
