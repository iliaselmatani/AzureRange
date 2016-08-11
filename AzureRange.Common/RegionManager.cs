﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureRange.Website
{
    public class RegionManager
    {
        private List<string> _regionsLocations = new List<string> {
            "uscentral,Central US, Iowa,42.0747, -93.49997,US",
            "useast,East US, Virginia,37.51282, -78.69794,US",
            "useast2,East US 2, Virginia,37.51282, -78.69794,US",
            "usgov1,US Gov Iowa,Iowa,42.0747, -93.49997,US Government",
            "usgov2,US Gov Virginia,Virginia,37.51282, -78.69794,US Government",
            "usnorth,North Central US,Illinois,40.11394,-89.15877,US",
            "ussouth,South Central US,Texas,31.46273,-99.33304,US",
            "uswest,West US, California,37.2551,-119.6175,US",
            "uswest2,West US 2, West US 2,37.2551,-119.6175,US",
            "uswestcentral,West Central US,West Central US,37.2551,-119.6175,US",
            "europenorth,North Europe, Ireland,53.183,-8.199,Europe",
            "europewest,West Europe, Netherlands,52.34226,5.528157,Europe",
            "asiaeast,East Asia, Hong Kong,22.27981,114.1618,Asia",
            "asiasoutheast,Southeast Asia, Singapore,1.321996,103.8205,Asia",
            "asiaeast,Japan East, Tokyo,35.68993, 139.6918,Asia",
            "asiasoutheast,Japan West, Osaka,34.67752,135.5129,Asia",
            "brazilsouth,Brazil South, Sao Paulo State,-22.26351, -48.73421,South America",
            "australiaeast,Australia East, New South Wales,-32.16693, 147.0125,Autralia",
            "australiasoutheast,Australia Southeast, Victoria,-36.86425, 144.3104,Autralia",
            "indiacentral,Central India, Pune,18.52522, 73.84863,India",
            "indiasouth,South India, Chennai,13.07983,80.27008,India",
            "indiawest,West India, Mumbai,18.9488,72.83056,India",
            "chinaeast,China East, Shanghai,31.16961,121.5168,China",
            "chinanorth,China North, Beijing,39.91257,116.389,China",
            "canadacentral,Canada Central, Toronto,43.65317,-79.3827,Canada",
            "canadaeast,Canada East, Quebec City,46.81332,-71.20937,Canada"};

        public List<AzureRegion> GetRegions(List<string> list)
        {
            List<AzureRegion> regions = new List<AzureRegion>();
            foreach(var regionName in list)
            {
                var line = _regionsLocations.FirstOrDefault(r => r.StartsWith(regionName));
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var splittedLine = line.Split(',');
                    regions.Add(new AzureRegion
                    {
                        Id = regionName,
                        Name = splittedLine[1],
                        Location = splittedLine[2],
                        Latitude = double.Parse(splittedLine[3]),
                        Longitude = double.Parse(splittedLine[4]),
                        Geography = splittedLine[5]
                    });
                }
            }
            return regions;
        }
    }
}