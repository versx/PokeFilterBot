﻿namespace BrockBot.Commands
{
    using System;
    using System.Threading.Tasks;

    using DSharpPlus;
    using DSharpPlus.Entities;

    using BrockBot.Data;
    using BrockBot.Extensions;

    [Command(
        Categories.Notifications,
        "Enables or disables all of your Pokemon and Raid notification subscriptions at once.",
        "\tExample: `.enable` (Enables all of your Pokemon and Raid notifications.)\r\n" +
        "\tExample: `.disable` (Disables all of your Pokemon and Raid notifications.)",
        "enable", "disable"
    )]
    public class EnableDisableCommand : ICustomCommand
    {
        #region Properties

        public CommandPermissionLevel PermissionLevel => CommandPermissionLevel.User;

        public DiscordClient Client { get; }

        public IDatabase Db { get; }

        #endregion

        #region Constructor

        public EnableDisableCommand(DiscordClient client, IDatabase db)
        {
            Client = client;
            Db = db;
        }

        #endregion

        public async Task Execute(DiscordMessage message, Command command)
        {
            await message.IsDirectMessageSupported();

            var author = message.Author.Id;
            if (!Db.SubscriptionExists(author))
            {
                await message.RespondAsync($"{message.Author.Mention} is not currently subscribed to any Pokemon or Raid notifications.");
                return;
            }

            var enabled = string.Compare(command.Name, "enable", true) == 0;
            Db[author].Enabled = enabled;
            Db.Save();

            await message.RespondAsync($"{message.Author.Mention} has **{command.Name}d** Pokemon and Raid notifications.");
        }
    }
}