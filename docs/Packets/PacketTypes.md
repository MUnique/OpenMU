# Packet types

Depending on the first byte of a packet, the packet has different attributes.

| First byte | Length of the packet | Encrypted Server -> Client | Encrypted Client -> Server |
|------------|----------------------|-----------|---------|
| 0xC1 | Specified by the second byte | No | Yes (XOR32) |
| 0xC2 | Specified by the second and third byte | No | Yes (XOR32) |
| 0xC3 | Specified by the second byte | Yes (Simple modulus) | Yes (Simple modulus + XOR32) |
| 0xC4 | Specified by the second and third byte | Yes (Simple modulus) | Yes (Simple modulus + XOR32) |