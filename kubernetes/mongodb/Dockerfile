FROM mongo:latest

COPY path/to/initDB.js /initDB.js

CMD mongod --bind_ip_all & sleep 5 && mongosh < /initDB.js && tail -f /dev/null