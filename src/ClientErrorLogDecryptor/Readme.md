# Client Error Log Decryptor

This tool can be used to decrypt the MuError.log files which are created by the
game client on every start.

It might help to find/analyse errors which occured on the client side.

## Usage

This is a command line tool. The file name is given as first parameter. If it
contains spaces, wrap it by quotes.
Example: ```MUnique.OpenMU.ClientErrorLogDecryptor.exe "C:\MU-Season 6E3\MuError.log"```

On windows you can also just simply drag & drop your MuError.log file on the exe.

The resulting file is written out into a new file, where the file name is extended
by ".decrypted.txt".
