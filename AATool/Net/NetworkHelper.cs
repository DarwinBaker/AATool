using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;

namespace AATool.Net
{
    public static class NetworkHelper
    {
        public static bool TryGetLocalIPAddress(out IPAddress ip)
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress address in host.AddressList)
            {
                if (address.AddressFamily is AddressFamily.InterNetwork)
                {
                    ip = address;
                    return true;
                }
            }
            ip = IPAddress.None;
            return false;
        }

        public static byte[] CompressString(string text) =>
            CompressBytes(Encoding.ASCII.GetBytes(text ?? string.Empty));

        public static bool TryDecompressString(byte[] compressed, out string text)
        {
            text = null;
            if (TryDecompressBytes(compressed, out byte[] decompressed))
            {
                try
                {
                    text = Encoding.ASCII.GetString(decompressed);
                    return true;
                }
                catch { }
            }
            return false;
        }
            

        public static byte[] CompressBytes(byte[] decompressed)
        {
            //compress byte array using deflate algorithm
            var output = new MemoryStream();
            using (var deflate = new DeflateStream(output, CompressionLevel.Optimal))
                deflate.Write(decompressed, 0, decompressed.Length);
            return output.ToArray();
        }

        public static bool TryDecompressBytes(byte[] compressed, out byte[] decompressed)
        {
            decompressed = null;
            try
            {
                //decompress byte array using deflate algorithm
                var input = new MemoryStream(compressed);
                var output = new MemoryStream();
                using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
                    deflate.CopyTo(output);
                decompressed = output.ToArray();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
