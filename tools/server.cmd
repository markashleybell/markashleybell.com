@ECHO OFF
REM This will give you a simple, non-HTTPS server
REM cd public
REM python -m http.server
cd tools
start python -m https-dev-server
cd ../
