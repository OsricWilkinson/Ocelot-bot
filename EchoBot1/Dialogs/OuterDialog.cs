using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Dialogs
{
    public class OuterDialog : ComponentDialog
    {
        public OuterDialog() : base(nameof(OuterDialog))
        {
            AddDialog(new OcelotDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                StartAsync,
                EndAsync,
            }));
        }

        private async Task<DialogTurnResult> StartAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(OcelotDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> EndAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Thanks");
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
