@ECHO OFF
start "C:\Windows\SysWOW64\wscript" "C:\Program Files (x86)\Git\Git Bash.vbs" %~dp0
start ..\scripts\activate
cd public
start python -m SimpleHTTPServer