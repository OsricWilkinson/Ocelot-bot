using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Bots
{
    public class DialogBot : ActivityHandler
    {
        private BotState ConversationState;

        public DialogBot(ConversationState conversationState)
        {
            this.ConversationState = conversationState;
        }
 
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected async override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            // var dialog = new TextPrompt(nameof(TextPrompt));
            var dialog = new ChoicePrompt(nameof(ChoicePrompt));

            var options = new List<string>();
            options.AddRange(new string[] { "a", "b", "c" });

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Pick something"),  Choices = ChoiceFactory.ToChoices(options) };

            var accessor = ConversationState.CreateProperty<DialogState>(nameof(DialogState));
            var dialogSet = new DialogSet(accessor);
            dialogSet.Add(dialog);

            var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);
            var results = await dialogContext.ContinueDialogAsync(cancellationToken);
            if (results.Status == DialogTurnStatus.Empty)
            {
                await dialogContext.BeginDialogAsync(dialog.Id, promptOptions, cancellationToken);
            } 
        }
    }
}
