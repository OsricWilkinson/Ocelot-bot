using EchoBot1.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Ocelot;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Dialogs
{
    public class OcelotDialog : ComponentDialog
    {
        private const string DoneKey = "value-done";
        private const string StateKey = "value-state";
        private readonly Ocelot.Process proc;

        public OcelotDialog() : base(nameof(OcelotDialog))
        {
            proc = Ocelot.Storage.LoadProcess();

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                ChoiceStepAsync,
                LoopStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private PromptOptions BuildOptions(WaterfallStepContext stepContext, ChatState state)
        {
            Stanza current = null;
            string text = "";

            while (true)
            {
                current = proc.GetStanza(state.CurrentStanzaID);
                if (text.Length > 0)
                {
                    text += "\n";
                }

                text += proc.GetPhrase(((InstructionStanza)current).Text).Internal;

                if (!current.HasNext || current.StanzaType == "Question" || current.Next[0] == "end")
                {
                    break;
                }
                state.CurrentStanzaID = current.Next[0];
            }

            if (current.StanzaType == "Question")
            {
                QuestionStanza qs = (QuestionStanza)current;

                IList<Choice> choices = new List<Choice>();

                for (int i =0; i < qs.Answers.Length; i +=1 )
                {
                    var choice = new Choice();
                    choice.Value = proc.GetPhrase(qs.Answers[i]).Internal;
                    if (choice.Value.ToLower().StartsWith("yes"))
                    {
                        choice.Synonyms = new List<string>{ "yes", "yup", "y" };
                    } else if (choice.Value.ToLower().StartsWith("no"))
                    {
                        choice.Synonyms = new List<string> { "no", "nope", "n" };
                    }
                    choices.Add(choice);
                }

                return new PromptOptions { Prompt = MessageFactory.Text(text), Choices = choices.ToArray() };
            }
            if (!current.HasNext)
            {
                stepContext.Values[DoneKey] = true;
            }


            return new PromptOptions { Prompt = MessageFactory.Text(text)};
        }

        private async Task<DialogTurnResult> ChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ChatState state = stepContext.Options as ChatState ?? new ChatState();
            stepContext.Values[StateKey] = state;
            if (state.CurrentStanzaID == null)
            {
                state.CurrentStanzaID = "start";
            }

            return await stepContext.PromptAsync(nameof(ChoicePrompt), BuildOptions(stepContext, state), cancellationToken);
        }

        private async Task<DialogTurnResult> LoopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = (FoundChoice)stepContext.Result;
            var done = stepContext.Values.ContainsKey(DoneKey);
            var state = stepContext.Values[StateKey] as ChatState;

            if (!done)
            {
                QuestionStanza qs = proc.GetStanza(state.CurrentStanzaID) as QuestionStanza;
                for (int i =0; i < qs.Answers.Length; i += 1)
                {
                    if (choice.Value == proc.GetPhrase(qs.Answers[i]).Internal)
                    {
                        state.  CurrentStanzaID = qs.Next[i];
                        break;
                    }
                }
                return await stepContext.ReplaceDialogAsync(nameof(OcelotDialog), state, cancellationToken);
            }
            else
            {
                stepContext.Values[StateKey] = null; 
                return await stepContext.EndDialogAsync(state, cancellationToken);
            }
        }
    }
}
