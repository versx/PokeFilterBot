﻿namespace BrockBot.Commands
{
    using System;
    using System.Threading.Tasks;

    using DSharpPlus;
    using DSharpPlus.Entities;

    using BrockBot.Configuration;
    using BrockBot.Data;
    using BrockBot.Diagnostics;
    using BrockBot.Extensions;

    [Command(
        Categories.Administrative,
        "Says something to the specified channel.",
        "\tExample: .say general \"Hey how's it going everyone?\"\r\n" +
        "\tExample: .say announcements \"Today will be a sunny day!\"",
        "say"
    )]
    public class SayCommand : ICustomCommand
    {
        private readonly DiscordClient _client;
        private readonly IDatabase _db;
        private readonly Config _config;
        private readonly IEventLogger _logger;

        #region Properties

        public CommandPermissionLevel PermissionLevel => CommandPermissionLevel.Admin;

        #endregion

        #region Constructor

        public SayCommand(DiscordClient client, IDatabase db, Config config, IEventLogger logger)
        {
            _client = client;
            _db = db;
            _config = config;
            _logger = logger;
        }

        #endregion

        public async Task Execute(DiscordMessage message, Command command)
        {
            if (!command.HasArgs) return;
            if (command.Args.Count != 2) return;

            await message.IsDirectMessageSupported();

            var channelName = command.Args[0];
            var channel = _client.GetChannelByName(channelName);
            if (channel == null)
            {
                await message.RespondAsync($"Failed to lookup channel {channelName}.");
                return;
            }
            
            try
            {
                var msg = command.Args[1];
                await channel.SendMessageAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}