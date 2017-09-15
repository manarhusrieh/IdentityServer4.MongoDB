# Connectionstring
* All integration tests are running using default connection string "mongodb://localhost/identityserver".
* If your local computer doesn't install mongodb, you can use below command to redirect traffic.
```
netsh interface portproxy add v4tov4 listenport=27017 listenaddress=0.0.0.0 connectport=27017 connectaddress=[target_database_server]
```
