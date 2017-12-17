﻿//namespace BrockBot.Commands
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Threading.Tasks;

//    using DSharpPlus;
//    using DSharpPlus.Entities;

//    using BrockBot.Data;
//    using BrockBot.Data.Models;
//    using BrockBot.Extensions;

//    /**Example Usage:
//     * .add upland_rares,ontario_rares
//     * .add upland_100iv
//     */
//    [Command(
//        Categories.Notifications, 
//        "Include Pokemon from the specified channels to be notified of.", 
//        "\tExample: `.add channel1,channel2`\r\n" +
//        "\tExample: `.add channel1`", 
//        "add"
//    )]
//    public class AddCommand : ICustomCommand
//    {
//        #region Properties

//        public bool AdminCommand => false;

//        public DiscordClient Client { get; }

//        public IDatabase Db { get; }

//        #endregion

//        #region Constructor

//        public AddCommand(DiscordClient client, IDatabase db)
//        {
//            Client = client;
//            Db = db;
//        }

//        #endregion

//        public async Task Execute(DiscordMessage message, Command command)
//        {
//            if (!command.HasArgs) return;
//            if (command.Args.Count != 1) return;

//            await message.IsDirectMessageSupported();

//            var author = message.Author.Id;
//            foreach (var chlName in command.Args[0].Split(','))
//            {
//                var channelName = chlName;
//                if (channelName[0] == '#') channelName = channelName.Remove(0, 1);

//                var channel = Client.GetChannelByName(channelName);
//                if (channel == null)
//                {
//                    await message.RespondAsync($"Channel name {channelName} is not a valid channel.");
//                    continue;
//                }

//                var server = Db[message.Channel.GuildId];
//                if (server == null) return;

//                if (!server.SubscriptionExists(author))
//                {
//                    server.Subscriptions.Add(new Subscription<Pokemon>(author, new List<Pokemon>(), new List<ulong> { channel.Id }));
//                    await message.RespondAsync($"You have successfully subscribed to {channel.Mention} notifications!");
//                    continue;
//                }

//                //User has already subscribed before, check if their new requested sub already exists.
//                if (!server[author].ChannelIds.Contains(channel.Id))
//                {
//                    server[author].ChannelIds.Add(channel.Id);
//                    await message.RespondAsync($"You have successfully subscribed to {channel.Mention} notifications!");
//                }
//                else
//                {
//                    await message.RespondAsync($"You are already subscribed to {channel.Mention} notifications.");
//                }
//            }
//        }
//    }
//}