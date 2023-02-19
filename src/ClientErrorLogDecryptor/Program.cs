// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;

var logPath = args.FirstOrDefault();
if (string.IsNullOrWhiteSpace(logPath))
{
    Console.WriteLine("No path to the error log specified as first parameter. Press any key to exit...");
    Console.ReadKey();
    return;
}

var decryptionKey = new byte[] { 0x7C, 0xBD, 0x81, 0x9F, 0x3D, 0x93, 0xE2, 0x56, 0x2A, 0x73, 0xD2, 0x3E, 0xF2, 0x83, 0x95, 0xBF };
try
{
    var fileBytes = await File.ReadAllBytesAsync(logPath).ConfigureAwait(false);

    var resultBuilder = new StringBuilder();
    var i = 0;
    foreach (var character in fileBytes)
    {
        
        var result = (char)(character ^ decryptionKey[i % decryptionKey.Length]);
        resultBuilder.Append(result);
        i++;
    }

    var resultPath = logPath + ".decrypted.txt";
    File.WriteAllText(resultPath, resultBuilder.ToString());
    Console.WriteLine($"Decrypted file has been written to {resultPath}. Press any key to exit...");
    
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error during decrypting the file '{logPath}':");
    Console.WriteLine(ex);

    Console.WriteLine($"Press any key to exit...");
}

Console.ReadKey();