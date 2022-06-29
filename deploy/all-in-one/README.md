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

#### Adapt the config

In the nginx.prod.conf, change "example.org" to your domain name.

#### Run it

`docker-compose up -f docker-compose.yml docker-compose.prod.yml -d`

> Hint: Here, docker-compose is used with the **docker-compose** command instead
> of **docker compose**.
> Reason: The -f parameter is only available through **docker-compose**.


#### Run certbot explicitly

Hint: replace "example.org" with your domain.

`docker compose run --rm  certbot certonly --webroot --webroot-path /var/www/certbot/ -d example.org`

#### Set up certificate renewal

Because your certificates expire after 3 months, it's recommended to renew them regularly.
To renew it, run this command:

`docker compose run --rm certbot renew`

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
