﻿namespace BrockBot.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DSharpPlus;
    using DSharpPlus.Entities;

    using BrockBot.Configuration;
    using BrockBot.Data;
    using BrockBot.Data.Models;
    using BrockBot.Extensions;

    [Command(
        Categories.Notifications,
        "Subscribe to raid boss Pokemon notifications.",
        "\tExample: `.raidme Absol` (Subscribe to Absol raid notifications.)\r\n" +
        "\tExample: `.raidme Tyranitar,Magikarp` (Subscribe to Tyranitar and Magikarp raid notifications.)\r\n" +
        "\tExample: `.raidme all` (Subscribe to all raid boss notifications.)",
        "raidme"
    )]
    public class RaidMeCommand : ICustomCommand
    {
        public const int MaxRaidSubscriptions = 5;

        #region Variables

        private readonly DiscordClient _client;
        private readonly IDatabase _db;
        private readonly Config _config;

        #endregion

        #region Properties

        public CommandPermissionLevel PermissionLevel => CommandPermissionLevel.User;

        #endregion

        #region Constructor

        public RaidMeCommand(DiscordClient client, IDatabase db, Config config)
        {
            _client = client;
            _db = db;
            _config = config;
        }

        #endregion

        public async Task Execute(DiscordMessage message, Command command)
        {
            //await message.IsDirectMessageSupported();

            if (!command.HasArgs) return;
            if (command.Args.Count != 1)
            {
                await message.RespondAsync($"{message.Author.Mention} please provide correct values such as `{_config.CommandsPrefix}{command.Name} tyranitar` or `{_config.CommandsPrefix}{command.Name} Magikarp,Absol,Mawile`");
                return;
            }

            var author = message.Author.Id;
            var cmd = command.Args[0];

            if (string.Compare(cmd, "all", true) == 0)
            {
                var isSupporter = await _client.IsSupporterOrHigher(author, _config);
                if (!isSupporter)
                {
                    await message.RespondAsync($"{message.Author.Mention} non-supporter members have a limited raid boss notification amount of {MaxRaidSubscriptions}, thus you may not use the 'all' parameter. Please narrow down your raid boss notification subscriptions to be more specific and try again.");
                    return;
                }

                for (uint i = 1; i < 386; i++)
                {
                    if (!i.IsValidRaidBoss(_config.RaidBosses)) continue;

                    var pokemon = _db.Pokemon[i.ToString()];
                    if (!_db.Exists(author))
                    {
                        _db.Subscriptions.Add(new Subscription<Pokemon>(author, new List<Pokemon>(), new List<Pokemon> { new Pokemon { PokemonId = i } }));
                        continue;
                    }

                    //User has already subscribed before, check if their new requested sub already exists.
                    var subs = _db[author];
                    if (!subs.Raids.Exists(x => x.PokemonId == i))
                    {
                        subs.Raids.Add(new Pokemon { PokemonId = i });
                        continue;
                    }
                }

                await message.RespondAsync($"{message.Author.Mention} subscribed to **all** raid boss notifications.");
                return;
            }

            var alreadySubscribed = new List<string>();
            var subscribed = new List<string>();

            foreach (var arg in cmd.Split(','))
            {
                var pokeId = _db.PokemonIdFromName(arg);
                if (pokeId == 0)
                {
                    await message.RespondAsync($"{message.Author.Mention}, failed to find raid Pokemon {arg}.");
                    return;
                }

                var pokemon = _db.Pokemon[pokeId.ToString()];
                if (!pokeId.IsValidRaidBoss(_config.RaidBosses))
                {
                    await message.RespondAsync($"{pokemon.Name} ({pokeId}) is not a valid raid boss.");
                    return;
                }

                if (!_db.Exists(author))
                {
                    _db.Subscriptions.Add(new Subscription<Pokemon>(author, new List<Pokemon>(), new List<Pokemon> { new Pokemon { PokemonId = pokeId } }));
                    subscribed.Add(pokemon.Name);
                    continue;
                }

                //User has already subscribed before, check if their new requested sub already exists.
                var subs = _db[author];
                if (!subs.Raids.Exists(x => x.PokemonId == pokeId))
                {
                    var isSupporter = await _client.IsSupporterOrHigher(author, _config);
                    if (!isSupporter && _db[author].Raids.Count >= MaxRaidSubscriptions)
                    {
                        await message.RespondAsync($"{message.Author.Mention} non-supporter members have a limited notification amount of {MaxRaidSubscriptions} different raid bosses, please consider donating to lift this to every raid Pokemon. Otherwise you will need to remove some subscriptions in order to subscribe to new raid Pokemon.");
                        return;
                    }

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