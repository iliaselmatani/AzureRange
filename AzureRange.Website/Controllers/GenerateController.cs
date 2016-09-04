﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
//test

namespace AzureRange.Website.Controllers
{
    public class GenerateController : BaseController
    {

        #region const_definition
        private const string _ciscoIOSPrefix = @"! CODE FOR IOS - CHECK IN A LAB BEFORE EXECUTING IN PRODUCTION!
router bgp <YOUR ASN>
 bgp log-neighbor-changes
 neighbor <PRIVATE_PEERING_EXPRESSROUTE_IP> remote-as 12076 ! ASN for ExpressRoute = 12076
 !
 address-family ipv4
  redistribute static ! this is one way to inject the networks below
  neighbor <PRIVATE_PEERING_EXPRESSROUTE_IP> activate
  no auto-summary
  no synchronization
 exit-address-family
!";
        private const string _ciscoASAPrefix = @"! CODE FOR Cisco ASA - CHECK IN A LAB BEFORE EXECUTING IN PRODUCTION!
router bgp <YOUR ASN>
 bgp log-neighbor-changes
 !
 address-family ipv4 unicast
  neighbor <PRIVATE_PEERING_EXPRESSROUTE_IP> remote-as <YOUR ASN>
  neighbor <PRIVATE_PEERING_EXPRESSROUTE_IP> description ExpressRoutePrivatePeering
  neighbor <PRIVATE_PEERING_EXPRESSROUTE_IP> activate
  neighbor <PRIVATE_PEERING_EXPRESSROUTE_IP> route-map AZURE-OUT out
  redistribute static
  no auto-summary
  no synchronization
 exit-address-family
!
route-map AZURE-OUT permit 10
 match ip address prefix-list AZURE-OUT
!
";
        #endregion

        public object Index(string[] region, string outputformat, string command)
        {
            var resultString = string.Empty;

            if (region != null)
            {
                var webGen = new WebGenerator(CacheConnection);
                var result = webGen.Generate(region.ToList());

                // Cisco IOS/IOS-XR
                if (outputformat == "cisco-ios")
                    resultString = _ciscoIOSPrefix + Environment.NewLine +
                        string.Join(string.Empty,
                        result.Select(r => "ip route " + r.ToStringLongMask() + " null0" + Environment.NewLine
                        ).ToArray());
                // Cisco ASA
                if (outputformat == "cisco-asa")
                {
                    resultString = _ciscoASAPrefix + Environment.NewLine;
                    resultString = resultString + string.Join(string.Empty,
                        result.Select(r => "route <interface_name> " + r.ToStringLongMask() + " <interface_name_IP>" + Environment.NewLine
                        ).ToArray());
                    resultString = resultString 
                        + "!" + Environment.NewLine
                        + "! Prefix-List to filter outgoing update to be restricted to the list below" + Environment.NewLine 
                        + "!" + Environment.NewLine;
                    var prefixSeqNumber = 10;
                    resultString = resultString + string.Join(string.Empty, result.Select(r => "prefix-list AZURE-OUT seq "+ prefixSeqNumber++*10 +" permit "
                         + r.ReadableIP + "/" + r.Mask + Environment.NewLine).ToArray());
                }

                if (outputformat == "list-subnet-masks")
                    resultString = string.Join(string.Empty, result.Select(r => r.ToStringLongMask() + Environment.NewLine).ToArray());
                if (outputformat == "list-cidr")
                    resultString = string.Join(string.Empty, result.Select(r => r.ReadableIP + "/" + r.Mask + Environment.NewLine).ToArray());
                if (outputformat == "csv-subnet-masks")
                {
                    resultString = string.Join(string.Empty, result.Select(r => "\"" + r.ToStringLongMask() + "\",").ToArray());
                    // need to remove the last ","
                    resultString = resultString.Substring(0, resultString.Length - 1);
                }
                if (outputformat == "csv-cidr")
                {
                    resultString = string.Join(string.Empty, result.Select(r => "\"" + r.ReadableIP + "/" + r.Mask + "\",").ToArray());
                    // remove the last "\","
                    resultString = resultString.Substring(0, resultString.Length - 1);
                }
            }

            if (command == "Download Output")
            {
                if (string.IsNullOrEmpty(resultString))
                {
                    return File(Encoding.ASCII.GetBytes("No region selected."), System.Net.Mime.MediaTypeNames.Application.Octet, "Error.txt");
                }
                else
                {
                    return File(Encoding.ASCII.GetBytes(resultString), System.Net.Mime.MediaTypeNames.Application.Octet, "AzureRange.txt");
                }
            }
            else if (command == "generate")
            {
                return WebUtility.HtmlEncode(resultString);
            }
            else
            {
                return WebUtility.HtmlDecode("Problem...");
            }
        }
    }
}
