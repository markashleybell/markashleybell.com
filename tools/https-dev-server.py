from http.server import HTTPServer, SimpleHTTPRequestHandler
import os
import ssl

httpd = HTTPServer(('localhost', 4443), SimpleHTTPRequestHandler)

httpd.socket = ssl.wrap_socket (httpd.socket, 
        keyfile="certificates/localhost-key.pem", 
        certfile='certificates/localhost-cert.pem', server_side=True)

os.chdir('../public')

httpd.serve_forever()
