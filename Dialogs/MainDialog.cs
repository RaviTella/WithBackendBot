using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WithBackendBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        public MainDialog(QueryDialog queryDialog)
            : base(nameof(MainDialog))
        {

            var waterfallSteps = new WaterfallStep[]
            {
                NameStepAsync,
                IntentStepAsync,
                BranchingStepAsync,
                ThanksStepAsync
            };
            AddDialog(queryDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("I am here to assist you ! What is your name?")
                }, cancellationToken);
        }


        private static async Task<DialogTurnResult> IntentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string name = (string)stepContext.Result;
            stepContext.Values["name"] = name;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Nice to meet you " + name), cancellationToken);
            await Task.Delay(2000);
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Are you looking for informtaion about flowcal repositories?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> BranchingStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = ((FoundChoice)stepContext.Result).Value;
            string name = (string)stepContext.Values["name"];
            switch (choice)
            {
                case "Yes":
                    return await stepContext.BeginDialogAsync(nameof(QueryDialog), "", cancellationToken);
                default:
                    return await stepContext.NextAsync(null, cancellationToken);
            }
        }


        private static async Task<DialogTurnResult> ThanksStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string name = (string)stepContext.Values["name"];
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you " + name), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
