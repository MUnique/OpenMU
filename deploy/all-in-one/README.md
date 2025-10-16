# All-in-one deployment

The all in one deployment is recommended, if you want to host on a small machine
with a low amount of players.

In this case, all kinds of OpenMU subsystems (ConnectServer, GameServer, LoginServer,
AdminPanel, ...) are running in one process.

## Deployment with docker-compose

### Install GIT

See [https://github.com/git-guides/install-git](https://github.com/git-guides/install-git).

### Clone the repository

`git clone https://github.com/MUnique/OpenMU.git`

It will create a new folder OpenMU with the repository contents inside.

### Navigate to the docker-compose files

Navigate to the folder deploy/all-in-one

`cd deploy/all-in-one`

### Option A - for local testing

To use the official docker image, just run:

`docker compose up -d --no-build`

And that's it. It's then available on your local computer through a loopback ip.

However, if you want to make it available through the internet, you should choose
Option B:

### Option B - with HTTPS

If you want to share your server with the world, it's recommended to set up HTTPS
for nginx. Otherwise, traffic from and to the admin panel is not encrypted.

#### Set your domain name as environment variable

Specify the environment variable ```DOMAIN_NAME``` for docker compose.
This can be done in [various ways](https://docs.docker.com/compose/environment-variables/set-environment-variables/),
e.g. by editing the ```docker-compose.prod.yml``` or
[setting it in your shell](https://phoenixnap.com/kb/linux-set-environment-variable).

This variable is replaced in the nginx template config files which get included
in the other configuration files.

#### Run it

`docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d`

#### Run certbot explicitly

Hint: replace "example.org" with your domain.

`docker compose -f docker-compose.yml -f docker-compose.prod.yml run --rm certbot certonly --webroot --webroot-path /var/www/certbot/ -d example.org`

#### Set up certificate renewal

Because your certificates expire after 3 months, it's recommended to renew them regularly.
To renew it, run this command:

`docker compose -f docker-compose.yml -f docker-compose.prod.yml run --rm certbot renew`

Of course, it would make sense to add a cron job (e.g. once a week) on your host
machine for that.

## What's next

The server is automatically started and initialized for Season 6. You can start
playing, if you want :-)

Additionally, take a look at the AdminPanel.

If your containers run on docker at your local machine, you can simply go to `http://localhost/`.
The default username is 'admin', the password 'openmu'. You should change that later.

If you want to run another game version, you can go to the setup page through
the navigation menu, where you can select your desired game version,
number of game servers (just the data of it), and if test accounts
should be created.

If you click on 'Install', wait a bit until the database is set up and filled with the
data and voila, OpenMU is ready to use.

## Presets for LAN vs Public

To simplify IP resolution for clients, additional compose overlays are provided:

- LAN testing (announce local IP):

  `docker compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.lan.yml up -d --build`

- Public internet (announce public IP):

  `docker compose -f docker-compose.yml -f docker-compose.prod.yml -f docker-compose.public.yml up -d`

You can switch between them by recreating the `openmu-startup` service with the respective overlays.

### When port 80 is in use (existing reverse proxy)

If your host already uses port 80/443 (e.g., Nginx Proxy Manager), you have two options:

1) Disable the bundled nginx and proxy via your reverse proxy:

   - Start without our nginx service: `docker compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.lan.yml up -d --build --scale nginx-80=0`
   - In your reverse proxy, forward to `openmu-startup:8080` (container name and port) with WebSockets enabled.

2) Map the admin panel to an alternate host port (e.g., 8082):

   - `docker compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.lan.yml -f docker-compose.admin-port.yml up -d --build --scale nginx-80=0`
   - Access the admin panel at `http://<host>:8082/`.

### Run without bundled nginx (use your existing reverse proxy)

If you prefer to use your own nginx / reverse proxy, use the `docker-compose.no-nginx.yml` file which excludes the bundled nginx service and exposes the admin panel on host port 8082.

- LAN:

  `docker compose -f docker-compose.no-nginx.yml -f docker-compose.override.yml -f docker-compose.lan.yml up -d --build`

- Public:

  `docker compose -f docker-compose.no-nginx.yml -f docker-compose.public.yml up -d`

Then, configure your reverse proxy to forward to `http://<host>:8082/` with WebSockets enabled.

### Attach to your existing Nginx Proxy (Docker network)

If your Nginx Proxy Manager (or nginx) runs on an external Docker network (e.g. `proxy_net`), attach `openmu-startup` to that network so the proxy can reach it by container name:

1) Ensure the network exists:

   `docker network ls | grep proxy_net || docker network create proxy_net`

2) Start OpenMU attached to `proxy_net`:

   `docker compose -f docker-compose.no-nginx.yml -f docker-compose.override.yml -f docker-compose.lan.yml -f docker-compose.npm-net.yml up -d --build`

3) In your proxy (NPM):

   - Forward Hostname/IP: `openmu-startup`
   - Forward Port: `8080`
   - Scheme: `http`
   - WebSockets: enabled

### Use a fixed public IP without committing it

If you prefer a fixed public IP instead of DNS but don't want to commit it to the repo:

1) Copy the example overlay:

   `cp docker-compose.public-ip.example.yml docker-compose.public-ip.yml`

2) Edit `docker-compose.public-ip.yml` and set your public IP in `RESOLVE_IP`.

3) Start with:

   `docker compose -f docker-compose.no-nginx.yml -f docker-compose.public-ip.yml -f docker-compose.npm-net.yml up -d --build --no-deps openmu-startup`

Note: `docker-compose.public-ip.yml` is git-ignored by default.
