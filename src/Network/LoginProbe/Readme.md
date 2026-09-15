# Login Probe

A console program which logs an account into a running server the way a game
client does — ask the connect server for a game server, connect to it, send the
encrypted login packet — and then holds the session open for a while.

It exists for tests and diagnostics which need a *real* client connection
rather than a simulated player: checking that an account is refused a second
login, that the connect server hands out the right endpoint, or that the login
path still works after a change to the encryption or the packet definitions.
Because it speaks the protocol through
[Network](../Readme.md) and [Packets](../Packets), it stays correct when those
change.

## Usage

```text
MUnique.OpenMU.Network.LoginProbe --account <name> [--password <pw>]
                                 [--host 127.0.0.1] [--connect-port 44406]
                                 [--server 0] [--hold <seconds>]
```

The password defaults to the account name, which is what the shipped test
accounts use. One JSON object per step is written to standard output:

```json
{"step":"server_selected","host":"127.127.127.127","port":55902,"server":0}
{"ok":true,"step":"login","account":"test4","result":"Okay"}
{"ok":true,"step":"holding","seconds":30}
{"ok":true,"step":"disconnected","account":"test4"}
```

The exit code is 0 when the login succeeded, 1 when the server refused it or
the connection failed, and 2 for a usage error.

## Contents

* `Program` - argument parsing, the connect server conversation
  (`ServerListRequest` → `ConnectionInfoRequest` → `ConnectionInfo`), and the
  game server login: a `Connection` with the client side of
  `OpenSourceClientNetworkEncryptionFactoryPlugIn`, credentials encrypted with
  `Xor3Encryptor`, and a wait for `LoginResponse`.
