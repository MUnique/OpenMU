# Network Analyzer

This little tool can be used to analyze the network traffic between a server and client, when they use the mu online protocol.
It acts as a proxy, which means it waits for incoming connections and connects to the actual server when a connection arrives. It then forwards all the traffic between server and client.
It also decrypts incoming and encrypts outgoing traffic to allow to take a closer look at the messages.
In the future it might also be possible to send custom data packets to server and client - to test their reactions.

By default the analyzer contains packet definitions for packets which are sent between game client and game server, defined by two separate XML files. However, it's not limited to that. For example, you could basically write other packet definitions and use it to analyze the communication between connect server and game client, too. You can also edit these xml files on the fly - as soon as they change, the analyzer reloads them automatically.

## Usage

### Setup
First, enter your local port, on which the tcp/ip listener should run at. For example, I run it at port 55900.

Next, enter the ip and port of the (game) server you want to connect. For example, I run a game server at 55901 on my local machine (127.x.x.x).

Next, change the MU version, if it's different from season 6. You can change this anytime you want, however it only has effect on new connections.
Selecting the correct MU Version is required to correctly de & encrypt the network traffic. Currently, there are 3 options for that.

Finally, click on 'Start Proxy' - then the application will listen on port 55900 and is waiting for client connections.

### Features

#### General
As soon as a client connects, it will be listed in the 'Connection' list and the data packets are shown in the grid. When you click on one packet, it extracts the included information based on the configured packet definition (xml files).
It supports multiple concurrent connections, so when you click on one, you'll see its traffic.

#### Context Menu

The connection list has the following context menu items:

  * *Disconnect*: Disconnects a connected client
  * *Save to file*: Saves the captured traffic into a file (*.mucap).
  * *Load from file*: Loads the captured traffic from the file, so you can continue your work.
  * *Packet Sender*: Opens a new window with a simple packet sender.

## Known limitations

### Complex packet structures
Currently it's not possible to define some more complex packet structures (e.g. arrays of sub-structures) by the xml configuration.
I didn't have the time yet, but I don't think this is hard to extend. If you want to work on it, feel free to create an Issue and send me a pull request.

### Incomplete packet definitions
As it's very time consuming to write the definition for every single packet structure,
I only did it for the most common ones between game server and game client and only for season 6.
Feel free to submit pull requests to make them complete :)

## Future Ideas
  * Usage of the packet definitions as reference for packet parsing/sending in the game server - instead of hard coding the packet construction and parsing.