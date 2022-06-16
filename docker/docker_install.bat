docker compose --env-file ./env up -d
ping -n 15 127.0.0.1 > nul
docker exec -i moviedb-mysql mysql -uroot -proot -e "update mysql.user set host = '%' where user='root';"
ping -n 15 127.0.0.1 > nul
docker container restart moviedb-mysql
ping -n 15 127.0.0.1 > nul
docker exec -i moviedb-mysql mysql -uroot -proot -e "ALTER USER 'root'@'' IDENTIFIED WITH mysql_native_password BY 'root';"
