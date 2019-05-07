using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1
{
    public class OcelotDialog : ComponentDialog
    {
            public OcelotDialog() : base(nameof(OcelotDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }


        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext context, CancellationToken token)
        {
            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Please enter your name") };

            return await context.PromptAsync(nameof(TextPrompt), promptOptions, token);

        }

    }
}
