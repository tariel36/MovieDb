# MovieDB

Selfhosted movie database prepared for docker run.

## Docker

Docker compose file contains of 4 services:
* MySQL database - [MySQL - Docker image](https://hub.docker.com/_/mysql);
* phpMyAdmin panel - [phpMyAdmin - Docker image](https://hub.docker.com/r/phpmyadmin/phpmyadmin/);
* Frontend service;
* Backend service;

Be aware that this package is designed for at home use, because root is used as main DB user with unsafe password. If your resolve thsoe issues, you're good to go.

```
TODO: Installation guide

1. Set environmental variables in `.env` file;
2. [Optional] Implement safetly measures within `install.bat` script - get rid of `root` etc;
2. Run `install.bat`;

## What `ping -n 15 127.0.0.1 > nul` does?
On Windows, it's hard to implemnet reliable `sleep` command to wait for certain amount of time. What this command does, is perform `n` pings - in the example it's 15 - one every each second and does not print the output. So in essence, it's equal to `wait n seconds`. It is done to let all the docekr images and services time to boot up.


```

### MySQL

* URL: `http://localhost:3306`;
* User: `root` by default, depends on `.env` file;
* Password: `root` by default, depends on `.env` file;

### phpMyAdmin

* URL: `http://localhost:8081`;

### Frontend

```
TODO
```

### Backend

```
TODO
```

## Development

Development instructions.

### Installing DB

1. Install tools

```
dotnet tool install --global dotnet-ef
```

2. Add migrations

```
dotnet ef migrations add Init
```

3. Update db / create db

```
dotnet ef database update
```

# Libraries and external resources

List of used technologies, libraries and external resources.

## Frontend

* [TypeScript](https://www.typescriptlang.org);
* [SASS / SCSS](https://sass-lang.com);
* [lipis/flag-icons](https://github.com/lipis/flag-icons);
* [Angular](https://angular.io);
* [Angular Material](https://material.angular.io);
* [themyth92/ngx-lightbox](https://github.com/themyth92/ngx-lightbox);
* [ngx-translate/core](https://github.com/ngx-translate/core);
* [ngx-translate/http-loader](https://github.com/ngx-translate/http-loader);
* [LIGHTBOX by Lokesh Dhakar](https://lokeshdhakar.com/projects/lightbox2/);

## Backend

* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/);
* [ASP.NET 6.0](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-6.0);
* [Entity Framework](https://docs.microsoft.com/en-us/ef/)
* [domaindrivendev/Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore);
* [PomeloFoundation/Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
* [Newtonsoft Json.NET](https://www.newtonsoft.com/json);

## Other

* [Swagger](https://swagger.io);