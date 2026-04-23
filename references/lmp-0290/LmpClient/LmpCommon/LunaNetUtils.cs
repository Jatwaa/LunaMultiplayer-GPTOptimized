// Decompiled with JetBrains decompiler
// Type: LmpCommon.LunaNetUtils
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace LmpCommon
{
  public static class LunaNetUtils
  {
    public static bool IsTcpPortInUse(int port)
    {
      try
      {
        return ((IEnumerable<IPEndPoint>) IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()).Any<IPEndPoint>((Func<IPEndPoint, bool>) (e => e.Port == port));
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool IsUdpPortInUse(int port)
    {
      try
      {
        return ((IEnumerable<IPEndPoint>) IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners()).Any<IPEndPoint>((Func<IPEndPoint, bool>) (e => e.Port == port));
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static IPAddress GetOwnSubnetMask()
    {
      NetworkInterface networkInterface = LunaNetUtils.GetNetworkInterface();
      if (networkInterface == null)
        return IPAddress.Any;
      foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
      {
        if (unicastAddress?.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
          return unicastAddress.IPv4Mask;
      }
      return IPAddress.Any;
    }

    public static IPAddress GetOwnInternalIPv4Address()
    {
      NetworkInterface networkInterface = LunaNetUtils.GetNetworkInterface();
      if (networkInterface == null)
        return IPAddress.Loopback;
      foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
      {
        if (unicastAddress?.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
          return unicastAddress.Address;
      }
      return IPAddress.Loopback;
    }

    public static UnicastIPAddressInformation GetOwnInternalIPv6Network()
    {
      NetworkInterface networkInterface = LunaNetUtils.GetNetworkInterface();
      if (networkInterface == null)
        return (UnicastIPAddressInformation) null;
      foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
      {
        if (unicastAddress?.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetworkV6 && !unicastAddress.Address.IsIPv6UniqueLocal() && !unicastAddress.Address.IsIPv6LinkLocal && !unicastAddress.Address.IsIPv6SiteLocal && !unicastAddress.Address.IsIPv6Teredo)
          return unicastAddress;
      }
      return (UnicastIPAddressInformation) null;
    }

    public static IPAddress GetOwnInternalIPv6Address()
    {
      UnicastIPAddressInformation internalIpv6Network = LunaNetUtils.GetOwnInternalIPv6Network();
      return internalIpv6Network == null ? IPAddress.IPv6Loopback : internalIpv6Network.Address;
    }

    private static bool IsIPv6UniqueLocal(this IPAddress address)
    {
      ushort addressByte = (ushort) address.GetAddressBytes()[0];
      return address.AddressFamily == AddressFamily.InterNetworkV6 && ((int) addressByte & 65024) == 64512;
    }

    public static IPAddress GetOwnExternalIpAddress()
    {
      string ipAddress = LunaNetUtils.TryGetIpAddress("https://ip.42.pl/raw");
      if (string.IsNullOrEmpty(ipAddress))
        ipAddress = LunaNetUtils.TryGetIpAddress("https://api.ipify.org/");
      if (string.IsNullOrEmpty(ipAddress))
        ipAddress = LunaNetUtils.TryGetIpAddress("https://httpbin.org/ip");
      if (string.IsNullOrEmpty(ipAddress))
        ipAddress = LunaNetUtils.TryGetIpAddress("http://checkip.dyndns.org");
      IPAddress address;
      return IPAddress.TryParse(ipAddress, out address) ? address : (IPAddress) null;
    }

    public static IPEndPoint CreateEndpointFromString(string endpoint)
    {
      try
      {
        int length = endpoint.LastIndexOf(":", StringComparison.Ordinal);
        string ipString = endpoint.Substring(0, length);
        int port = int.Parse(endpoint.Substring(length + 1));
        IPAddress address1;
        if (IPAddress.TryParse(ipString, out address1))
          return new IPEndPoint(address1, port);
        IPAddress address2 = ((IEnumerable<IPAddress>) Dns.GetHostAddresses(ipString.Trim())).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (d => d.AddressFamily == AddressFamily.InterNetwork));
        return address2 != null ? new IPEndPoint(address2, port) : (IPEndPoint) null;
      }
      catch (Exception ex)
      {
        return (IPEndPoint) null;
      }
    }

    public static IPAddress[] CreateAddressFromString(string address)
    {
      try
      {
        IPAddress address1;
        if (!IPAddress.TryParse(address, out address1))
          return Dns.GetHostAddresses(address);
        return new IPAddress[1]{ address1 };
      }
      catch (Exception ex)
      {
        return new IPAddress[0];
      }
    }

    private static string TryGetIpAddress(string url)
    {
      try
      {
        using (WebClient webClient = new WebClient())
        {
          using (Stream stream = webClient.OpenRead(url))
          {
            if (stream == null)
              return (string) null;
            using (StreamReader streamReader = new StreamReader(stream))
            {
              IPAddress address;
              if (IPAddress.TryParse(new Regex("\\b\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\b").Matches(streamReader.ReadToEnd())[0].Value, out address))
                return address.ToString();
            }
          }
        }
      }
      catch
      {
      }
      return (string) null;
    }

    private static NetworkInterface GetNetworkInterface()
    {
      NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
      if (networkInterfaces.Length < 1)
        return (NetworkInterface) null;
      NetworkInterface networkInterface1 = (NetworkInterface) null;
      foreach (NetworkInterface networkInterface2 in networkInterfaces)
      {
        if (networkInterface2.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface2.NetworkInterfaceType != NetworkInterfaceType.Unknown && networkInterface2.Supports(NetworkInterfaceComponent.IPv4))
        {
          if (networkInterface1 == null)
            networkInterface1 = networkInterface2;
          if (networkInterface2.OperationalStatus == OperationalStatus.Up)
          {
            foreach (UnicastIPAddressInformation unicastAddress in networkInterface2.GetIPProperties().UnicastAddresses)
            {
              if (unicastAddress?.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                return networkInterface2;
            }
          }
        }
      }
      return networkInterface1;
    }
  }
}
