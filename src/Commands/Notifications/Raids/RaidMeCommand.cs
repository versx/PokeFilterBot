﻿namespace BrockBot.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DSharpPlus;
    using DSharpPlus.Entities;

    using BrockBot.Data;
    using BrockBot.Data.Models;
    using BrockBot.Extensions;

    [Command(
        Categories.Notifications,
        "Subscribe to Pokemon raid notifications.",
        "\tExample: `.raidme Absol` (Subscribe to Absol raid notifications.)\r\n" +
        "\tExample: `.raidme Tyranitar,Magikarp` (Subscribe to Tyranitar and Magikarp raid notifications.)",
        "raidme"
    )]
    public class RaidMeCommand: ICustomCommand
    {
        #region Properties

        public CommandPermissionLevel PermissionLevel => CommandPermissionLevel.User;

        public DiscordClient Client { get; }

        public IDatabase Db { get; }

        #endregion

        #region Constructor

        public RaidMeCommand(DiscordClient client, IDatabase db)
        {
            Client = client;
            Db = db;
        }

        #endregion

        public async Task Execute(DiscordMessage message, Command command)
        {
            //await message.IsDirectMessageSupported();

            if (!command.HasArgs) return;
            if (command.Args.Count != 1) return;

            var author = message.Author.Id;
            var cmd = command.Args[0];

            var alreadySubscribed = new List<string>();
            var subscribed = new List<string>();

            foreach (var arg in cmd.Split(','))
            {
                var pokeId = Helpers.PokemonIdFromName(Db, arg);
                if (pokeId == 0)
                {
                    await message.RespondAsync($"{message.Author.Mention}, failed to find raid Pokemon {arg}.");
                    return;
                }

                var pokemon = Db.Pokemon[pokeId.ToString()];
                if (!Db.SubscriptionExists(author))
                {
                    Db.Subscriptions.Add(new Subscription<Pokemon>(author, new List<Pokemon>(), new List<Pokemon> { new Pokemon { PokemonId = pokeId } }));
                    subscribed.Add(pokemon.Name);
                    continue;
                }

                //User has already subscribed before, check if their new requested sub already exists.
                var subs = Db[author];
                if (!subs.Raids.Exists(x => x.PokemonId == pokeId))
                {
                    subs.Raids.Add(new Pokemon { PokemonId = pokeId });
                    subscribed.Add(pokemon.Name);
                    continue;
                }

                alreadySubscribed.Add(pokemon.Name);
            }

            await message.RespondAsync
            (
                (subscribed.Count > 0
                    ? $"{message.Author.Mention} has subscribed to **{string.Join("**, **", subscribed)}** raid notifications."
                    : string.Empty) +
                (alreadySubscribed.Count > 0
                    ? $" {message.Author.Mention} is already subscribed to {string.Join(",", alreadySubscribed)} raid notifications."
                    : string.Empty)
            );
        }
    }
}