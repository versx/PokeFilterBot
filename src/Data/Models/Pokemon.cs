﻿namespace BrockBot.Data.Models
{
    using Newtonsoft.Json;

    [JsonObject("pokemon")]
    public class Pokemon
    {
        [JsonProperty("pokemonId")]
        public uint PokemonId { get; set; }

        [JsonProperty("minimumCP")]
        public int MinimumCP { get; set; }

        [JsonProperty("minimumIV")]
        public int MinimumIV { get; set; }

        [JsonIgnore]
        public int MinimumLevel { get; set; }
    }
}