# Interfaces

This project contains the interfaces which are used to communicate between the
different sub-systems ("servers") of the whole system.

The game logic and game server should only use these interfaces instead of the
actual implementations.
The goal is to make them exchangeable. This can be helpful for tests, and also
if we want to move specific systems to other machines or external processes
(scale-out).
Then there could be an implementation of an interface which forwards the calls
over the network (etc.) to another process.
