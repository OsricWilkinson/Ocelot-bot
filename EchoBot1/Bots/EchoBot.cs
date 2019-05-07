// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EchoBot1.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace EchoBot1.Bots
{
    public class EchoBot : ActivityHandler
    {
        private BotState _converstationState;
        private BotState _userState;

        public EchoBot(ConversationState conversationState, UserState userState)
        {
            this._converstationState = conversationState;
            this._userState = userState;
        }

        private Task<T> GetState<T>(ITurnContext turnContext, BotState botState) where T : new()
        {
            var accessor = botState.CreateProperty<T>(nameof(T));
            return accessor.GetAsync(turnContext, () => new T());
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await _converstationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            ConversationData cd = await GetState<ConversationData>(turnContext, _converstationState);
            ChatState cs = await GetState<ChatState>(turnContext, _userState);

            if (string.IsNullOrEmpty(cs.Name))
            {
                if (cd.AskedName)
                {
                    cs.Name = turnContext.Activity.Text?.Trim();
                    await turnContext.SendActivityAsync($"Hello {cs.Name}");
                    cd.AskedName = false;
                }
                else
                {
                    await turnContext.SendActivityAsync("What is your name?");
                    cd.AskedName = true;
                }
            } else
            {
                await turnContext.SendActivityAsync($"{cs.Name} said '{turnContext.Activity.Text}'");
            }

            Console.WriteLine("Got request: ", turnContext.Activity.Text);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and Welcome!"), cancellationToken);
                }
            }
        }
    }
}
