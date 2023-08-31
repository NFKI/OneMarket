﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AlbionMarketCurrentPrices
{
    public class MarketCurrentPrices : IMarketCurrentPrices
    {
        // Current Prices property list
        public string item_id { get; set; }
        public string city { get; set; }
        public int quality { get; set; }
        public int sell_price_min { get; set; }
        public DateTime sell_price_min_date { get; set; }
        public int sell_price_max { get; set; }
        public DateTime sell_price_max_date { get; set; }
        public int buy_price_min { get; set; }
        public DateTime buy_price_min_date { get; set; }
        public int buy_price_max { get; set; }
        public DateTime buy_price_max_date { get; set; }

        public void ParseMarketCurrentPrices(string ResponseBody, string RequestId)
        {
            // Declare a list to store multiple instances of MarketCurrentPrices class
            // Number of instances is defined by the size of given JSON array []
            // Console.WriteLine(ResponseBody);
            List<MarketCurrentPrices> MarketCurrentPircesJsonList = JsonConvert.DeserializeObject<List<MarketCurrentPrices>>(ResponseBody);
            // Print out every instance of the class stored in the created list
            foreach (MarketCurrentPrices RequestedObjectDetails in MarketCurrentPircesJsonList)
            {
                /* 
                 * Required request parameters (Prices):
                 * itemList -> define desired items ID
                 * locations -> define desired market location
                 * qualities -> define game item quality
                */
                Console.WriteLine($"Item ID: {RequestedObjectDetails.item_id}\n" +
                    $"City: {RequestedObjectDetails.city}\n" +
                    $"Quality: {RequestedObjectDetails.quality}\n" +
                    $"Sell price (min): {RequestedObjectDetails.sell_price_min}\n" +
                    $"Transaction date: {RequestedObjectDetails.sell_price_min_date}\n" +
                    $"Sell price (max): {RequestedObjectDetails.sell_price_max}\n" +
                    $"Transaction date: {RequestedObjectDetails.sell_price_max_date}\n" +
                    $"Buy price (min): {RequestedObjectDetails.buy_price_min}\n" +
                    $"Transaction date: {RequestedObjectDetails.buy_price_min_date}\n" +
                    $"Buy price (max): {RequestedObjectDetails.buy_price_max}\n" +
                    $"Transaction date: {RequestedObjectDetails.buy_price_max_date}");
            }
        }
    }
}