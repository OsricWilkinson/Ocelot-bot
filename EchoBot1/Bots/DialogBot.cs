using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Bots
{
    public class DialogBot<T> : ActivityHandler where T: Dialog
    {
        private readonly BotState _conversationState;
        private readonly Dialog _dialog;

        public DialogBot(ConversationState conversationState, T dialog)
        {
            this._dialog = dialog;
            this._conversationState = conversationState;
        }
        private Task<Item> GetState<Item>(ITurnContext turnContext, BotState botState) where Item : new()
        {
            var accessor = botState.CreateProperty<Item>(nameof(Item));
            return accessor.GetAsync(turnContext, () => new Item());
        }




        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected async override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await _dialog.Run(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = MessageFactory.Text("Hello");
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }
    }
}
