# Intialization

This sub-project is about initializing the default data so the server is ready
to start. Currently, it also creates 10 accounts for testing.

The state of this sub-project is probably pretty messy and very incomplete, so
please don't wonder.

The initialization code is called by the a migration of entity framework core.
However, it's independent from it, so if we ever want to switch it's reuseable.

It's tested by an integration test in the corresponding Test-project. The test
runs the initialization on a real database instance and tries to load the game
configuration and one of the created accounts.
