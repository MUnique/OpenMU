# Distributed

## Deployment with docker-compose

Currently, we just have a docker-compose file for the deployment.
docker-compose has the limitation, that all runs on the same physical machine.

For an even more distributed environment, with more machines, kubernetes can be
used. However, we don't have a finished configuration for kubernetes, yet. If you
are familiar with kubernetes, all contributions are welcome for kubernetes
configuration files.

So, these are the steps, if you want to deploy it with docker-compose:

### Install GIT

See [https://github.com/git-guides/install-git](https://github.com/git-guides/install-git).

### Clone the repository

`git clone https://github.com/MUnique/OpenMU.git`

It will create a new folder OpenMU with the repository contents inside.

### Navigate to the docker-compose files

Navigate to the folder deploy/distributed

`cd deploy/distributed`

### Option A - for local testing

To use the official docker images, just run:

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

Now, when you have deployed OpenMU, it's time to discover the AdminPanel.

If your containers run on docker at your local machine, you can simply go to `http://localhost/admin`.
The default username is 'admin', the password 'openmu'. You should change that later.

There you'll find a setup in the navigation menu, where you can select your desired
game version, number of game servers (just the data of it), and if test accounts
should be created.

Click on 'Install', wait a bit until the database is set up and filled with the
data.
OpenMU should now be ready to use.

## Environment variables

The openmu images which are used in this docker-compose consider the following
environment variables:

### ASPNETCORE_ENVIRONMENT

It's usually specified correctly in the docker-compose files. It has effect on
the ip resolver, see below.

### RESOLVE_IP

Similar to the -resolveIp starting parameter of the all-in-one startup project,
you're able to control the ip resolving with this variable. The defaults usually
work fine here, so you should try not to set this variable.

| Value | Description         |
|-------|---------------------|
| local | Default value in a *Development* environment. Determines a local ip. If none is found, a loopback IP is used (127.127.127.127). |
| public | Default value in a *Production* environment. The public ip is automatically determined by an [external API](https://www.ipify.org/). |
| loopback | Returns *127.127.127.127*, usefully only if you run your server and client on the same machine. |
| [custom ip] | You can specify a custom ip, if needed. Example: *192.168.0.1* |

### GS_ID

It's usually specified correctly in the docker-compose files or each game server.
Specifies the id of a game server, and is used to retrieve the GameServerConfiguration
from the database.
