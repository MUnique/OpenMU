# Network Analyzer

This project contains the platform independent part of the network analysis:
the data model of a captured connection and the analysis of its packets, based
on the packet definitions (xml files) of the
[Packets](../Packets) project.

It's used by the
[WinForms analyzer tool](../Analyzer.WinForms/Readme.md), which captures the
traffic by acting as a proxy between game client and server.

## Contents

* `Packet` - a single captured data packet, with its timestamp, direction and
  raw data.

* `PacketAnalyzer` - determines the packet definition of a packet and extracts
  its field values, based on the packet definitions and the client version.

* `ICapturedConnection` - a connection with its captured packets. Implemented
  by the live connections of the analyzer tool, and by `SavedConnection`, which
  loads a previously saved capture.

* `CapturedConnectionExtensions` - saves the captured packets of a connection
  to a file (*.mucap) and loads them again.
