﻿using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Q42.HueApi.WinRT
{
  /// <summary>
  /// Find bridges through SSDP request for Windows 8 / WinRT
  /// </summary>
  public class SSDPBridgeLocator : IBridgeLocator
  {

    /// <summary>
    /// Returns list of bridge IPs
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<string>> LocateBridgesAsync(TimeSpan timeout)
    {
      if (timeout <= TimeSpan.Zero)
        throw new ArgumentException("Timeout value must be greater than zero.", nameof(timeout));

      var discoveredDevices = new List<string>();
      var multicastIP = new HostName("239.255.255.250");
      
      using (var socket = new DatagramSocket())
      {

        //Handle MessageReceived
        socket.MessageReceived += (sender, e) =>
        {
          var reader = e.GetDataReader();
          var bytesRemaining = reader.UnconsumedBufferLength;
          var receivedString = reader.ReadString(bytesRemaining);

          var location = receivedString.Substring(receivedString.ToLower().IndexOf("location:", System.StringComparison.Ordinal) + 9);
          receivedString = location.Substring(0, location.IndexOf("\r", System.StringComparison.Ordinal)).Trim();

          discoveredDevices.Add(receivedString);
        };

        await socket.BindEndpointAsync(null, string.Empty);
        socket.JoinMulticastGroup(multicastIP);

        var start = DateTime.Now;

        do
        {
          using (var stream = await socket.GetOutputStreamAsync(multicastIP, "1900"))
          using (var writer = new DataWriter(stream))
          {
            string request = "M-SEARCH * HTTP/1.1\r\n" +
                         "HOST:239.255.255.250:1900\r\n" +
                         //"ST:urn:schemas-upnp-org:device:Basic:1\r\n" + //Alternative
                         // "ST:upnp:rootdevice\r\n" +                    //Alternative
                         "ST:SsdpSearch:all\r\n" +
                         "MAN:\"ssdp:discover\"\r\n" +
                         "MX:3\r\n\r\n\r\n";

            writer.WriteString(request.ToString());
            await writer.StoreAsync();

          }

        }
        while (DateTime.Now.Subtract(start) < timeout); // try for thee seconds
      }

      return await FilterBridges(discoveredDevices);

    }

    private async Task<IEnumerable<string>> FilterBridges(List<string> discoveredDevices)
    {
      List<string> bridgeIps = new List<string>();


      var endpoints = discoveredDevices.Where(s => s.EndsWith("/description.xml")).ToList();

      foreach (var endpoint in endpoints)
      {
		  var ip = endpoint.Replace("http://", "").Replace("https://", "").Replace("/description.xml", "");

        //Not in the list yet?
        if (!bridgeIps.Contains(ip))
        {
          //Check if it is Hue Bridge
          if (await IsHue(endpoint))
          {
            //Add ip
            bridgeIps.Add(ip);
          }
        }
      }

      return bridgeIps;
    }

    // http://www.nerdblog.com/2012/10/a-day-with-philips-hue.html - description.xml retrieval
    private async Task<bool> IsHue(string discoveryUrl)
    {
      var http = new HttpClient { Timeout = TimeSpan.FromMilliseconds(2000) };
      var res = await http.GetStringAsync(discoveryUrl);
      if (!string.IsNullOrWhiteSpace(res))
      {
        if (res.ToLower().Contains("philips hue bridge"))
          return true;
      }
      return false;
    }

  }
}
