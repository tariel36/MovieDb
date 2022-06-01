# MovieDB
MySQL database and phpMyAdmin panel.

Be aware that this package is designed for at home use, because root is used as main DB user with unsafe password. If your resolve thsoe issues, you're good to go.

# Details
## MySQL
* URL: `http://localhost:3306`;
* User: `root` by default, depends on `.env` file;
* Password: `root` by default, depends on `.env` file;

## phpMyAdmin
* URL: `http://localhost:8081`;

# Installation
1. Set environmental variables in `.env` file;
2. [Optional] Implement safetly measures within `install.bat` script - get rid of `root` etc;
2. Run `install.bat`;

## What `ping -n 15 127.0.0.1 > nul` does?
On Windows, it's hard to implemnet reliable `sleep` command to wait for certain amount of time. What this command does, is perform `n` pings - in the example it's 15 - one every each second and does not print the output. So in essence, it's equal to `wait n seconds`. It is done to let all the docekr images and services time to boot up.

# External
* [MySQL - Docker image](https://hub.docker.com/_/mysql);
* [phpMyAdmin - Docker image](https://hub.docker.com/r/phpmyadmin/phpmyadmin/);

