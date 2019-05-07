using EchoBot1.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Ocelot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Bots
{
    public class DialogBot : ActivityHandler
    {
        private readonly BotState ConversationState;
        private Task<T> GetState<T>(ITurnContext turnContext, BotState botState) where T : new()
        {
            var accessor = botState.CreateProperty<T>(nameof(T));
            return accessor.GetAsync(turnContext, () => new T());
        }

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
            Process proc = Ocelot.Storage.LoadProcess();
            ChatState state = await GetState<ChatState>(turnContext, ConversationState);
            if (state.CurrentStanzaID == null)
            {
                state.CurrentStanzaID = "start";
            }
            var accessor = ConversationState.CreateProperty<DialogState>(nameof(DialogState));

            var dialogSet = new DialogSet(accessor);
            var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);
            var results = await dialogContext.ContinueDialogAsync(cancellationToken);

            if (results.Status == DialogTurnStatus.Complete)
            {
                QuestionStanza qs = (QuestionStanza)proc.GetStanza(state.CurrentStanzaID);
                var r = (FoundChoice)results.Result;

                for (int i = 0; i < qs.Answers.Length; i += 1)
                {
                    if (proc.GetPhrase(qs.Answers[i]).Internal == r.Value)
                    {
                        state.CurrentStanzaID = qs.Next[i];
                        await dialogContext.CancelAllDialogsAsync(cancellationToken);
                        break;
                    }
                }

            }
            Stanza current = null;
            string text = "";
            do
            {
                current = proc.GetStanza(state.CurrentStanzaID);
                if (text.Length > 0)
                {
                    text += "\n";
                }

                text += proc.GetPhrase(((InstructionStanza)current).Text).Internal;

            } while (current.HasNext && current.StanzaType != "Question");

            Dialog dialog = null;
            PromptOptions promptOptions = null;
            if (current.StanzaType == "Question")
            {
                QuestionStanza qs = (QuestionStanza)current;
                dialog = new ChoicePrompt("dialog");
                var options = new List<string>();
                options.AddRange(qs.Answers.Select(n => proc.GetPhrase(n).Internal.Split(' ')[0]).ToArray());

                promptOptions = new PromptOptions { Prompt = MessageFactory.Text(text), Choices = ChoiceFactory.ToChoices(options) };
            }
            else
            {
                dialog = new TextPrompt("dialog");
                promptOptions = new PromptOptions { Prompt = MessageFactory.Text(text) };
            }
            dialogSet.Add(dialog);

            await dialogContext.BeginDialogAsync(dialog.Id, promptOptions, cancellationToken);

        }
        /*

        protected async override Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            Process proc = Ocelot.Storage.LoadProcess();
            ChatState state = await GetState<ChatState>(turnContext, ConversationState);
            state.CurrentStanzaID = "start";

            InstructionStanza current = (InstructionStanza)proc.GetStanza(state.CurrentStanzaID);

            var dialog = new TextPrompt("stanza-" + current.ID);
            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(proc.GetPhrase(current.Text).Internal) };

            var accessor = ConversationState.CreateProperty<DialogState>(nameof(DialogState));
            var dialogSet = new DialogSet(accessor);
            dialogSet.Add(dialog);
            var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);
            await dialogContext.BeginDialogAsync(dialog.Id, promptOptions, cancellationToken);
        }
        */
    }
}
