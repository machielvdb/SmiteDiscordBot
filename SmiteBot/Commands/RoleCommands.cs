using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace SmiteBot.Commands
{
    public class RoleCommands : BaseCommandModule
    {
        [Command("welcome")]
        public async Task Welcome(CommandContext ctx)
        {   
            // Embed aanmaken en variabelen declareren.
            var welcomeEmbed = new DiscordEmbedBuilder
            {
                Title = "Select main and secondary role.",
                Color = DiscordColor.Green
            };

            var welcomeMessage = await ctx.Channel.SendMessageAsync(embed: welcomeEmbed).ConfigureAwait(false);

            var support = DiscordEmoji.FromName(ctx.Client, ":koala:");
            var adc = DiscordEmoji.FromName(ctx.Client, ":lion_face:");
            var mid = DiscordEmoji.FromName(ctx.Client, ":bear:");
            var jungle = DiscordEmoji.FromName(ctx.Client, ":tiger:");
            var solo = DiscordEmoji.FromName(ctx.Client, ":panda_face:");

            await welcomeMessage.CreateReactionAsync(support).ConfigureAwait(false);
            await welcomeMessage.CreateReactionAsync(adc).ConfigureAwait(false);
            await welcomeMessage.CreateReactionAsync(mid).ConfigureAwait(false);
            await welcomeMessage.CreateReactionAsync(jungle).ConfigureAwait(false);
            await welcomeMessage.CreateReactionAsync(solo).ConfigureAwait(false);

            var interactivity = ctx.Client.GetInteractivity();

            // Wachten tot er met emoji wordt gereageerd en parameters controleren.
            var reactionResult = await interactivity.WaitForReactionAsync(
                x => x.Message == welcomeMessage &&
                x.User == ctx.User &&
                (x.Emoji == support || x.Emoji == adc || x.Emoji == mid || x.Emoji == jungle || x.Emoji == solo)).ConfigureAwait(false);

            // Rollen toekennen op basis van gekozen emoji.
            if (reactionResult.Result.Emoji == support)
            {
                var role = ctx.Guild.GetRole(632872210750767106);
                await ctx.Member.GrantRoleAsync(role);
            }

            else if (reactionResult.Result.Emoji == adc)
            {
                var role = ctx.Guild.GetRole(632871942281625641);
                await ctx.Member.GrantRoleAsync(role);
            }

            else if (reactionResult.Result.Emoji == mid)
            {
                var role = ctx.Guild.GetRole(632871837272899635);
                await ctx.Member.GrantRoleAsync(role);
            }

            else if (reactionResult.Result.Emoji == jungle)
            {
                var role = ctx.Guild.GetRole(632872340195246081);
                await ctx.Member.GrantRoleAsync(role);
            }

            else if (reactionResult.Result.Emoji == solo)
            {
                var role = ctx.Guild.GetRole(632871489086947348);
                await ctx.Member.GrantRoleAsync(role);
            }

            await welcomeMessage.DeleteAsync().ConfigureAwait(false);
        }
    }
}