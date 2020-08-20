using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmiteBot.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns pong")]
        public async Task Pong(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }
        [Command("add")]
        [Description("Adds two numbers together")]
        public async Task Add(CommandContext ctx, int num1, int num2)
        {
            await ctx.Channel.SendMessageAsync($"{num1 + num2}").ConfigureAwait(false);
        }
        [Command("Response")]
        public async Task Response(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }

        public List<string> GetVoiceChannelUsers(CommandContext ctx)
        {
            // Gets the voicechannel the user is in.
            DiscordChannel currentChannel = ctx.Member.VoiceState.Channel;
            if (currentChannel == null)
            {
                // Return error when no channel is found.
                ctx.Channel.SendMessageAsync("User not active in voice channel.");
                return null;
            }

            // Create a dictionary with every channel from the server and fill it.
            Dictionary<ulong, DiscordChannel> channelList = new Dictionary<ulong, DiscordChannel>();
            List<string> usersInChannel = new List<string>();

            foreach (var channel in ctx.Guild.Channels)
            {
                channelList.Add(channel.Key, channel.Value);
            }

            // Check if one of the servers has the same key as the voicechannel the user is in.
            // Adds all user in voicechannel to list when match is found.
            foreach (var ch in channelList)
            {
                if (ch.Key == currentChannel.Id)
                {
                    foreach (var user in ch.Value.Users)
                    {
                        usersInChannel.Add(user.DisplayName);
                    }
                    return usersInChannel;
                }
            }
            return null;
        }

        private static Random rng = new Random();

        public static void ShuffleList<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        [Command("shuffle")]
        [Description("Shuffles people in voice channel into 2 teams.")]
        public async Task Shuffle(CommandContext ctx)
        {
            List<string> usersInChannel = GetVoiceChannelUsers(ctx);
            int userAmount = usersInChannel.Count;
            string team1string = null;
            string team2string = null;
            ShuffleList(usersInChannel);

            // Checks if amount of people to split into 2 teams is even.
            if (userAmount%2 == 0 && usersInChannel.Count > 0)
            {
                while (userAmount != 0)
                {
                    foreach (var user in usersInChannel)
                    {
                        if (userAmount > (usersInChannel.Count / 2))
                        {
                            team1string += "\n" + user;
                            userAmount--;
                        }

                        else
                        {
                            team2string += ("\n" + user);
                            userAmount--;
                        }
                    }
                }

                DiscordEmbedBuilder team1Embed = new DiscordEmbedBuilder
                {
                    Title = "Team 1",
                    Description = $"{team1string}",
                    Color = DiscordColor.Green,
                };

                DiscordEmbedBuilder team2Embed = new DiscordEmbedBuilder
                {
                    Title = "Team 2",
                    Description = $"{team2string}",
                    Color = DiscordColor.Green,
                };

                await ctx.Channel.SendMessageAsync(embed: team1Embed).ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(embed: team2Embed).ConfigureAwait(false);
            }

            else if (usersInChannel.Count == 0)
            {
                await ctx.Channel.SendMessageAsync("No users in In-Houses channel.");
            }

            else if (userAmount%2 != 0)
            {
                await ctx.Channel.SendMessageAsync("Uneven number of users in channel.");
            }
        }

        [Command("poll")]
        public async Task Poll(CommandContext ctx, string pollname, TimeSpan duration, params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var pollEnbed = new DiscordEmbedBuilder
            {
                Title = pollname,
                Description = string.Join(" ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEnbed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
            var distinctRestult = result.Distinct();

            var results = distinctRestult.Select(x => $"{x.Emoji}: {x.Total}");

            await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
        }

        [Command("dialogue")]
        public async Task Dialogue(CommandContext command)
        {
            string input = string.Empty;
        }
    }
}