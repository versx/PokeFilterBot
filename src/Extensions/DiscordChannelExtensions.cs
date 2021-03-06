﻿namespace BrockBot.Extensions
{
    using System;
    using System.Threading.Tasks;

    using DSharpPlus;
    using DSharpPlus.Entities;

    using BrockBot.Utilities;

    public static class DiscordChannelExtensions
    {
        public static async Task IsDirectMessageSupported(this DiscordMessage message)
        {
            if (message.Channel.Guild == null)
            {
                await message.RespondAsync("DM is not supported for this command yet.");
                return;
            }
        }

        public static DiscordChannel GetChannelByName(this DiscordClient client, string channelName, bool isTextChannel = true)
        {
            foreach (var guild in client.Guilds)
            {
                foreach (var channel in guild.Value.Channels)
                {
                    if (string.Compare(channel.Name, channelName, true) == 0 && (channel.IsCategory && !isTextChannel || !channel.IsCategory && isTextChannel))
                    {
                        return channel;
                    }
                }
            }

            return null;
        }

        public static async Task<DiscordChannel> GetChannel(this DiscordClient client, ulong channelId)
        {
            try
            {
                return await client.GetChannelAsync(channelId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task LockChannel(this DiscordChannel channel, DiscordRole role)
        {
            try
            {
                await channel.GrantPermissions(role, Permissions.ReadMessageHistory | Permissions.AccessChannels, Permissions.SendMessages);
            }
            catch (Exception ex)
            {
                Utils.LogError(ex);
            }
        }

        public static async Task UnlockChannel(this DiscordChannel channel, DiscordRole role)
        {
            try
            {
                await channel.GrantPermissions(role, Permissions.ReadMessageHistory | Permissions.AccessChannels | Permissions.SendMessages, Permissions.None);
            }
            catch (Exception ex)
            {
                Utils.LogError(ex);
            }
        }
    }
}