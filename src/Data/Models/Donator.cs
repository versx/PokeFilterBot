﻿namespace BrockBot.Data.Models
{
    using System;

    using Newtonsoft.Json;

    [JsonObject("donator")]
    public class Donator
    {
        [JsonProperty("userId")]
        public ulong UserId { get; set; }

        [JsonProperty("daysAvailable")]
        public long DaysAvailable { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("dateDonated")]
        public DateTime DateDonated { get; set; }
    }
}