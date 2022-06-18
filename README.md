# MovieDB

Selfhosted movie database prepared for hosting in docker.

## Docker

Docker compose file contains of 4 services:
* MySQL database - [MySQL - Docker image](https://hub.docker.com/_/mysql);
* phpMyAdmin panel - [phpMyAdmin - Docker image](https://hub.docker.com/r/phpmyadmin/phpmyadmin/);
* Frontend service;
* Backend service;

Be aware that this package is designed for at home use, because root is used as main DB user with unsafe password. If your resolve those issues, you're good to go.

## Building & Deploying

How to build and deploy the application.

### Backend

1. Copy `appsettings.template.json` and rename it to `appsettings.json`;
2. Fill in missing configuration values, modify those you want:

    * Modify connection string if needed;
    * `ApiKeys:OpenMovieDb` - provide your OpenMovieDb api key;
    * `ApiKeys:MyAnimeList` - provide your MyAnimeList api key;
    * For day od 2022-06-16, the `ApiKeys:Anilist` is not needed;
    * `Email:Host` - provide your email SMTP host;
    * `Email:From` - provide your email addres;
    * `Email:User` - provide your email address;
    * `Email:Password` - provide your email password;
    * `ConnectionString` - Key to selected connection string, for example `ConnectionStrings:DockerConnectionString`;
    * `IsDeveloper` - `false`

3. Compile your app;
4. Run migrations if needed:

    1. Install tools - `dotnet tool install --global dotnet-ef`'
    2. Update db / create db - `dotnet ef database update`;

5. Build docker image, if needed coerce paths to dockerfile and solution file, you can do it through VS (preferable) or by command line:

```
docker build -f "Dockerfile" --force-rm -t moviedb/backend  --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=MovieDbApi.Core" ".../MovieDb/project/backend/MovieDbApi"
```

### Frontend

1. If you did not change any URLs, paths etc, you're good to go;
2. Build docker image - `docker build -t moviedb/frontend:latest .`;

### MySql

Nothing to do here.

### phpMyAdmin

Nothing to do here.

### Deploying to Docker

1. Copy `.env.template` and change name to `.env`;
2. Set all keys, for example:

    * `MYSQL_ROOT_USER=root`;
    * `MYSQL_ROOT_PASSWORD=root`;
    * `SHARED_ROOT_DIR_USER=user`;
    * `SHARED_ROOT_DIR_PASS=pass`;
    * `SHARED_ROOT_DIR_ROOT=//192.168.0.1/path`;

You can run `docker_install.bat` - remember to change `root` credentials if you did in previous steps.

Alternatively, you can run all the commands one by one. Of course, remember to wait between commands so image restarts, etc.

```
docker compose --env-file ./.env up -d

docker exec -i moviedb-mysql mysql -uroot -proot -e "update mysql.user set host = '%' where user='root';"

docker container restart moviedb-mysql

docker exec -i moviedb-mysql mysql -uroot -proot -e "ALTER USER 'root'@'' IDENTIFIED WITH mysql_native_password BY 'root';"
```

## Access

### MySQL

* URL: `http://<host>:3307`;
* User: `root` by default, depends on `.env` file;
* Password: `root` by default, depends on `.env` file;

### phpMyAdmin

* URL: `http://<host>:3308`;

### Frontend

* URL: `http://<host>:3309`;

### Backend

* URL: `http://<host>:5137`;
* API: `http://<host>:5137/api`;
* SWAGGER: `http://<host>:5137/swagger/index.html`;

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
