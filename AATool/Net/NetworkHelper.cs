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
            CompressBytes(Encoding.ASCII.GetBytes(text));

        public static string DecompressString(byte[] bytes) => 
            Encoding.ASCII.GetString(DecompressBytes(bytes));

        public static byte[] CompressBytes(byte[] data)
        {
            //compress byte array using deflate algorithm
            MemoryStream output = new ();
            using (DeflateStream deflate = new(output, CompressionLevel.Optimal))
                deflate.Write(data, 0, data.Length);
            return output.ToArray();
        }

        public static byte[] DecompressBytes(byte[] data)
        {
            //decompress byte array using deflate algorithm
            MemoryStream input  = new (data);
            MemoryStream output = new ();
            using (DeflateStream deflate = new(input, CompressionMode.Decompress))
                deflate.CopyTo(output);
            return output.ToArray();
        }
    }
}
