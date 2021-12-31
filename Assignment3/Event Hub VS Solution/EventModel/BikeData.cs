using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace EventModel
{
    public class BikeData
    {
        /* {
        "Trip ID": "913460",
        "Duration": "765",
        "Start Date": "8/31/2015 23:26",
        "Start Station": "Harry Bridges Plaza (Ferry Building)",
        "Start Terminal": "50",
        "End Date": "8/31/2015 23:39",
        "End Station": "San Francisco Caltrain (Townsend at 4th)",
        "End Terminal": "70",
        "Bike #": "288",
        "Subscriber Type": "Subscriber",
        "Zip Code": "2139"
    }*/
        // Convert using https://json2csharp.com/

        [JsonProperty("Trip ID")]
        public string TRIP_ID { get; set; }
        [JsonProperty("Duration")]
       
        public string DURATION { get; set; }
        [JsonProperty("Start Date")]
        public string START_DATE { get; set; }
        [JsonProperty("Bike #")]
        
        public string BIKE_NO { get; set; }
        [JsonProperty("Subscriber Type")]
        public string SUBSCRIBER_TYPE { get; set; }
        [JsonProperty("Zip Code")]
        public string ZIP_CODE { get; set; }

        [JsonProperty("Process Time")]
        public DateTime ProcessTime { get; set; }
    }
}




