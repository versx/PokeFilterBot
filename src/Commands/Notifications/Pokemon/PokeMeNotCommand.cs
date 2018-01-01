﻿namespace BrockBot.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DSharpPlus;
    using DSharpPlus.Entities;

    using BrockBot.Data;
    using BrockBot.Extensions;

    [Command(
        Categories.Notifications,
        "Unsubscribe from a one or more or even all subscribed Pokemon notifications.",
        "\tExample: `.pokemenot 149`\r\n" +
        "\tExample: `.pokemenot 3,6,9,147,148,149`\r\n" +
        "\tExample: `.pokemenot` (Removes all subscribed Pokemon notifications.)",
        "pokemenot"
    )]
    public class PokeMeNotCommand : ICustomCommand
    {
        #region Properties

        public bool AdminCommand => false;

        public DiscordClient Client { get; }

        public IDatabase Db { get; }

        #endregion

        #region Constructor

        public PokeMeNotCommand(DiscordClient client, IDatabase db)
        {
            Client = client;
            Db = db;
        }

        #endregion

        public async Task Execute(DiscordMessage message, Command command)
        {
            await message.IsDirectMessageSupported();

            var server = Db[message.Channel.GuildId];
            if (server == null) return;

            var author = message.Author.Id;

            if (!server.SubscriptionExists(author))
            {
                await message.RespondAsync($"{message.Author.Username} is not subscribed to any Pokemon notifications.");
                return;
            }

            if (!(command.HasArgs && command.Args.Count == 1))
            {
                if (!server.RemoveAllPokemon(author))
                {
                    await message.RespondAsync($"Failed to remove all Pokemon notifications for {message.Author.Username}.");
                    return;
                }

                await message.RespondAsync($"{message.Author.Username} has unsubscribed from **all** Pokemon notifications.");
                return;
            }

            var notSubscribed = new List<string>();
            var unsubscribed = new List<string>();

            var cmd = command.Args[0];
            foreach (var arg in cmd.Split(','))
            {
                var index = Convert.ToUInt32(arg);
                if (!Db.Pokemon.ContainsKey(index.ToString()))
                {
                    await message.RespondAsync($"Pokedex number {index} is not a valid Pokemon id.");
                    continue;
                }

                var pokemon = Db.Pokemon[index.ToString()];
                var unsubscribePokemon = server[author].Pokemon.Find(x => x.PokemonId == index);
                if (unsubscribePokemon != null)
                {
                    if (server[author].Pokemon.Remove(unsubscribePokemon))
                    {
                        //msg += $"**{pokemon.Name}**";
                        unsubscribed.Add(pokemon.Name);
                    }
                }
                else
                {
                    notSubscribed.Add(pokemon.Name);
                    //await message.RespondAsync($"{message.Author.Username} is not subscribed to **{pokemon.Name}** notifications.");
                }
            }

            await message.RespondAsync
            (
                (unsubscribed.Count > 0
                    ? $"{message.Author.Username} has unsubscribed from **{string.Join("**, **", unsubscribed)}** notifications."
                    : string.Empty) +
                (notSubscribed.Count > 0 
                    ? $" {message.Author.Username} is not subscribed to {string.Join(", ", notSubscribed)} notifications." 
                    : string.Empty)
            );
        }
    }
}