using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public  class InternetConnection
{
    public static DateTime GetNistTime()
    {
        const string ntpServer = "time.android.com";

        var ntpData = new byte[48];
        ntpData[0] = 0x1B;

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;

        // Prend la première adresse dispo, IPv4 ou IPv6 (tu peux aussi préférer IPv4 si tu veux)
        var address = addresses.FirstOrDefault(ip =>
            ip.AddressFamily == AddressFamily.InterNetwork ||
            ip.AddressFamily == AddressFamily.InterNetworkV6);

        if (address == null)
            throw new Exception("Aucune adresse IPv4/IPv6 valide trouvée pour le serveur NTP.");

        var ipEndPoint = new IPEndPoint(address, 123);

        // Le socket doit être créé avec la MÊME famille que l'adresse résolue
        using (var socket = new Socket(address.AddressFamily, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.Connect(ipEndPoint);
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();
        }

        const byte serverReplyTime = 40;

        ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
        ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

        var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .AddMilliseconds((long)milliseconds);

        return networkDateTime.ToLocalTime();
    }

    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }
}
