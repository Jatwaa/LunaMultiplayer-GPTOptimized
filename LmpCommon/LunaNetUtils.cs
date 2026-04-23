using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

#pragma warning disable SYSLIB0014

namespace LmpCommon
{
    public static class LunaNetUtils
    {
        private static IPAddress cachedExternalIpAddress = null;
        private static DateTime cachedExternalIpTime = DateTime.MinValue;
        private static readonly TimeSpan ExternalIpCacheDuration = TimeSpan.FromMinutes(5);
        public static bool IsTcpPortInUse(int port)
        {
            try
            {
                return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(e => e.Port == port);
            }
            catch (Exception)
            {
                //This fails on macOS High Sierra
                return false;
            }
        }

        public static bool IsUdpPortInUse(int port)
        {
            try
            {
                return IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().Any(e => e.Port == port);
            }
            catch (Exception)
            {
                //This fails on macOS High Sierra
                return false;
            }
        }

        public static IPAddress GetOwnInternalIPv4Address()
        {
            // Step 1: OS routing-table lookup — no packets are sent, just a route query.
            // This reliably picks the correct source IP on multi-homed machines (e.g. a machine
            // with both Wi-Fi and Ethernet).
            IPAddress routeIp = null;
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.Connect("8.8.8.8", 80);
                    var a = ((IPEndPoint)socket.LocalEndPoint).Address;
                    if (a != null && !a.Equals(IPAddress.Loopback))
                        routeIp = a;
                }
            }
            catch { /* fall through */ }

            // Step 2: build a list of candidate IPs from real NICs — those that have a default
            // gateway configured.  Docker bridge adapters and most VPN tunnel interfaces do NOT
            // have a gateway address, so they are naturally excluded.
            var candidates = new System.Collections.Generic.List<IPAddress>();
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus != OperationalStatus.Up) continue;
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Unknown) continue;

                    var props = nic.GetIPProperties();
                    bool hasGateway = false;
                    foreach (var gw in props.GatewayAddresses)
                    {
                        if (gw.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !gw.Address.Equals(IPAddress.Any))
                        { hasGateway = true; break; }
                    }
                    if (!hasGateway) continue;

                    foreach (var uni in props.UnicastAddresses)
                    {
                        if (uni?.Address?.AddressFamily == AddressFamily.InterNetwork)
                            candidates.Add(uni.Address);
                    }
                }
            }
            catch { /* fall through */ }

            // Step 3: if the routing-trick result is on a gateway NIC, use it directly —
            // this is the most accurate answer on multi-homed machines.
            if (routeIp != null && candidates.Contains(routeIp))
                return routeIp;

            // Step 4: routing trick returned a virtual/VPN IP.  Prefer any gateway-bearing NIC.
            if (candidates.Count > 0)
                return candidates[0];

            // Step 5: nothing found via routing trick or gateway NICs — fall back to the
            // routing result anyway (handles unusual networks like genuine 172.x.x.x LANs).
            if (routeIp != null)
                return routeIp;

            // Step 6: last resort — original NIC enumeration (original behaviour).
            var ni = GetNetworkInterface(AddressFamily.InterNetwork);
            if (ni == null)
            {
                return IPAddress.Loopback;
            }

            var properties = ni.GetIPProperties();
            foreach (var unicastAddress in properties.UnicastAddresses)
            {
                if (unicastAddress?.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return unicastAddress.Address;
                }
            }

            return IPAddress.Loopback;
        }

        public static UnicastIPAddressInformation GetOwnInternalIPv6Network()
        {
            var ni = GetNetworkInterface(AddressFamily.InterNetworkV6);
            if (ni == null)
            {
                return null;
            }

            var properties = ni.GetIPProperties();
            foreach (var unicastAddress in properties.UnicastAddresses)
            {
                if (unicastAddress?.Address != null
                    && unicastAddress.Address.AddressFamily == AddressFamily.InterNetworkV6
                    && !unicastAddress.Address.IsIPv6UniqueLocal() && !unicastAddress.Address.IsIPv6LinkLocal
                    && !unicastAddress.Address.IsIPv6SiteLocal && !unicastAddress.Address.IsIPv6Teredo)
                {
                    return unicastAddress;
                }
            }

            return null;
        }

        public static IPAddress GetOwnInternalIPv6Address()
        {
            var info = GetOwnInternalIPv6Network();
            if (info == null)
                return IPAddress.IPv6Loopback;

            return info.Address;
        }

        /// <summary>
        /// Gets whether the address is an IPv6 Unique Local address.
        /// Backport of https://github.com/dotnet/runtime/pull/48853 landing in .NET 6
        /// </summary>
        public static bool IsIPv6UniqueLocal(this IPAddress address)
        {
            if (address.AddressFamily != AddressFamily.InterNetworkV6)
                return false;
            var bytes = address.GetAddressBytes();
            if (bytes.Length != 16)
                return false;
            return (bytes[0] & 0xFE) == 0xFC;
        }

        public static IPAddress GetOwnExternalIpAddress()
        {
            if (cachedExternalIpAddress != null && (DateTime.UtcNow - cachedExternalIpTime) < ExternalIpCacheDuration)
                return cachedExternalIpAddress;

            var currentIpAddress = TryGetIpAddress("https://ip.42.pl/raw");

            if (string.IsNullOrEmpty(currentIpAddress))
                currentIpAddress = TryGetIpAddress("https://api.ipify.org/");
            if (string.IsNullOrEmpty(currentIpAddress))
                currentIpAddress = TryGetIpAddress("https://httpbin.org/ip");
            if (string.IsNullOrEmpty(currentIpAddress))
                currentIpAddress = TryGetIpAddress("http://checkip.dyndns.org");

            if (IPAddress.TryParse(currentIpAddress, out var ipAddress))
            {
                cachedExternalIpAddress = ipAddress;
                cachedExternalIpTime = DateTime.UtcNow;
                return ipAddress;
            }

            return null;
        }

        // TODO IPv6: This does not return AAAA records of hostnames.
        // However it is only used to parse the dedicated and master server list,
        // and server<->master and client<->master connections are IPv4-only, so the master
        // server gets the public IPv4 address.
        public static IPEndPoint CreateEndpointFromString(string endpoint)
        {
            try
            {
                    // [2001:db8::1]:8800
                    // 192.0.2.1:8800
                    var indexOfPortSeparator = endpoint.LastIndexOf(":", StringComparison.Ordinal);
                    var ip = endpoint.Substring(0, indexOfPortSeparator);
                    var port = int.Parse(endpoint.Substring(indexOfPortSeparator + 1));
                    if (IPAddress.TryParse(ip, out var addr))
                        return new IPEndPoint(addr, port);

                    var dnsIp = Dns.GetHostAddresses(ip.Trim());
                    var ipv4Address = dnsIp.FirstOrDefault(d => d.AddressFamily == AddressFamily.InterNetwork);
                    return ipv4Address != null ? new IPEndPoint(ipv4Address, port) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IPAddress[] CreateAddressFromString(string address)
        {
            try
            {
                if (IPAddress.TryParse(address, out var ip))
                {
                    return new []{ ip };
                }
                return Dns.GetHostAddresses(address);
            }
            catch (Exception)
            {
                return new IPAddress[]{};
            }
        }

        #region Private

        private class TimeoutWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                if (request is System.Net.HttpWebRequest httpRequest)
                    httpRequest.Timeout = 3000;
                return request;
            }
        }

        private static string TryGetIpAddress(string url)
        {
            try
            {
                using (var client = new TimeoutWebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    using (var stream = client.OpenRead(url))
                    {
                        if (stream == null) return null;
                        using (var reader = new StreamReader(stream))
                        {
                            var ipRegEx = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                            var result = ipRegEx.Matches(reader.ReadToEnd());

                            if (result.Count > 0 && IPAddress.TryParse(result[0].Value, out var ip))
                                return ip.ToString();
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private static NetworkInterface GetNetworkInterface(AddressFamily addressFamily)
        {
            try
            {
                var nics = NetworkInterface.GetAllNetworkInterfaces();
                if (nics.Length < 1)
                    return null;

                NetworkInterface best = null;
                foreach (var adapter in nics)
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback || adapter.NetworkInterfaceType == NetworkInterfaceType.Unknown)
                        continue;
                    var ipVersion = addressFamily == AddressFamily.InterNetwork
                        ? NetworkInterfaceComponent.IPv4
                        : NetworkInterfaceComponent.IPv6;
                    if (!adapter.Supports(ipVersion))
                        continue;
                    if (best == null)
                        best = adapter;
                    if (adapter.OperationalStatus != OperationalStatus.Up)
                        continue;

                    // Make sure this adapter has an address of the specified family
                    var properties = adapter.GetIPProperties();
                    foreach (var unicastAddress in properties.UnicastAddresses)
                    {
                        if (unicastAddress?.Address != null && unicastAddress.Address.AddressFamily == addressFamily)
                        {
                            // Yes it does, return this network interface.
                            return adapter;
                        }
                    }
                }
                return best;
            }
            catch
            {
                // Linux with many virtual interfaces (Docker, WireGuard, systemd-networkd) can hang here
                return null;
            }
        }

        #endregion
    }
}
