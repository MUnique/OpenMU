# All-in-one-traefik deployment

The all in one (with traefik) deployment is recommended, if you want to host
on a small machine with a low amount of players and want to host your
MuOnline Website on the same machine.

Once Traefik works as a Reverse Proxy, you can handle miltiple website
without change the default port to HTTP/HTTPS connections.

Addin a few labels to your container, you will tell Traefik how to
handle incoming requests and he will redirect to the correct website.

``` yml
version: '3'

services:
  admin-panel:
    ...
    labels:
      - "traefik.enable=true"
      - "traefik.docker.network=proxy"
      - "traefik.http.routers.adm.entrypoints=websecure"
      - "traefik.http.routers.adm.rule=Host(`admin.domain.com`)"
      - "traefik.http.routers.adm.middlewares=auth"
      - "traefik.http.middlewares.auth.basicauth.usersfile=.htpasswd"
      
  muonline-website:
    ...
    labels:
      - "traefik.enable=true"
      - "traefik.docker.network=proxy"
      - "traefik.http.routers.muonline.entrypoints=websecure"
      - "traefik.http.routers.muonline.rule=Host(`muonline.domain.com`)"
```

You can even add multiple domains and/or subdomains to your host label

``` yml
- "traefik.http.routers.muonline.rule=Host(`www.domain1.com`,`domain1.com`,`sub.domain1.com`)"
```

In this case, all kinds of OpenMU subsystems (ConnectServer, GameServer, LoginServer,
AdminPanel, ...) are running in one process, but you can run
AdminPanel and other websites separately.

## Deployment with docker-compose

### Install GIT

See [https://github.com/git-guides/install-git](https://github.com/git-guides/install-git).

### Clone the repository

``` bash
git clone https://github.com/MUnique/OpenMU.git
```

It will create a new folder OpenMU with the repository contents inside.

### Navigate to the docker-compose files

Navigate to the folder `deploy/all-in-one-traefik`

``` bash
cd deploy/all-in-one-traefik
```

### Create a new docker network

You need to have communication between containers from different docker compose files

``` bash
docker network create proxy
```

### Option A - for local testing

To run the official docker image on your local traefik server, just run:

``` bash
docker compose -f docker-compose.yml up -d
```

And that's it. It's then available on your local computer through a loopback
ip. Your AdminPanel URL is `http://admin.docker.localhost`. You can change it
in the `docker-compose.yml` file.

However, if you want to make it available through the internet, you should choose
Option B:

### Option B - with HTTPS

If you want to share your server with the world, it's recommended to set up HTTPS
and Traefik can handle it for you. Otherwise, traffic from and to the admin
panel is not encrypted.

#### Set your domain name as environment variable

Make a copy of `.env.example` called `.env` and edit the `.env` file with your
domain/subdomain to the AdminPanel URL.

``` bash
cp .env.example .env
```

Make a copy of `data-traefik/acme.example.json` called `data-traefik/acme.json`
and apply permission 600 to `acme.json`

``` bash
cp data-traefik/acme.example.json data-traefik/acme.json
chmod 600 data-traefik/acme.json
```

#### Run it

``` bash
docker compose -f docker-compose.prod.yml up -d
```

#### Important

Avoid editing the .htpasswd manually. Instead, access the admin panel
and add a new user. If you are using the _all-in-one-traefik_ you
need to restart Traefik after add a new user to it takes effect.

## What's next

The server is automatically started and initialized for Season 6. You can start
playing, if you want :-)

Additionally, take a look at the AdminPanel.

If your containers run on docker at your local machine, you can simply go to `http://admin.docker.localhost/`.
The default username is 'admin', the password 'openmu'. You should change that later.

If you want to run another game version, you can go to the setup page through
the navigation menu, where you can select your desired game version,
number of game servers (just the data of it), and if test accounts
should be created.

If you click on 'Install', wait a bit until the database is set up and
filled with the data and voila, OpenMU is ready to use.
