using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using SmiteBot.Handlers.Dialogue;
using System.Threading.Tasks;
using Discord;
using DSharpPlus.Interactivity;

namespace SmiteBot.Handlers.Dialogue.Steps
{
    public class TextStep : DialogueStepBase
    {
        private IDialogueStep _nextStep;
        private readonly int? _minlength;
        private readonly int? _maxLength;

        public TextStep(string content, IDialogueStep nextStep, int? minLength = null,
            int? maxLength = null) : base(content)
        {
            _nextStep = nextStep;
            _minlength = minLength;
            _maxLength = maxLength;
        }

        public Action<string> OnValidResult { get; set; }

        public override IDialogueStep NextStep => _nextStep;

        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = $"Please respond below.",
                Description = $"{user.Mention}, {_content}",
            };

            embedBuilder.AddField("To stop the dialogue", "Use the `cancel command");

            if(_minlength.HasValue)
            {
                embedBuilder.AddField("Min length:", $"{_minlength.Value} characters");
            }
            if(_maxLength.HasValue)
            {
                embedBuilder.AddField("Max length:", $"{_maxLength.Value} characters.");
            }

            var interactivity = client.GetInteractivity();

            while (true)
            {
                var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);
                OnMessageAdded(embed);

                var messageResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == channel.Id && x.Author.Id == user.Id).ConfigureAwait(false);

                OnMessageAdded(messageResult.Result);

                if (messageResult.Result.Content.Equals("`cancel", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (_minlength.HasValue)
                {
                    if (messageResult.Result.Content.Length < _minlength.Value)
                    {
                        await TryAgain(channel, $"Your input is {_minlength.Value - messageResult.Result.Content.Length} too short.");
                    }
                }

                OnValidResult(messageResult.Result.Content);

                return false;
            }
        }
    }
}
