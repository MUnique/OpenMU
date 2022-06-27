# All-in-one deployment

The all in one deployment is recommended, if you want to host on a small machine with a low amount of players.
In this case, all kinds of OpenMU subsystems (ConnectServer, GameServer, LoginServer, AdminPanel, ...) are running in one process.

## Deployment with docker-compose

### Install GIT

See https://github.com/git-guides/install-git

### Clone the repository

> git clone https://github.com/MUnique/OpenMU.git

### Navigate to the docker-compose files

Navigate to the folder deploy/all-in-one

### Option A - for local testing

> docker-compose up -d

And that's it ;-)

However, if you want to make it available through the internet, you should choose Option B:

### Option B - with HTTPS

If you want to share your server with the world, it's recommended to set up HTTPS for nginx.
Otherwise, traffic from and to the admin panel is not encrypted.

#### Adapt the config

In the nginx.prod.conf, change "example.org" to your domain name.

#### Run it

> docker-compose up -f docker-compose.yml docker-compose.prod.yml -d

#### Run certbot explicitly

Hint: replace "example.org" with your domain.

> docker compose run --rm  certbot certonly --webroot --webroot-path /var/www/certbot/ -d example.org

#### Set up certificate renewal
Because your certificates expire after 3 months, it's recommended to renew them regularly.
To renew it, run this command:

> docker compose run --rm certbot renew

Of course, it would make sense to add a cron job (e.g. once a week) on your host machine for that.

## What's next

Now, when you have deployed OpenMU, it's time to discover the AdminPanel.
If your containers run on docker at your local machine, you can simply go to http://localhost/

There you'll find a setup in the navigation menu, where you can select your desired game version, number of game servers (just the data of it), and if test accounts should be created.

Click on 'Install', wait a bit until the database is set up and filled with the data and voila, OpenMU is ready to use.
