using WithBackendBot.Cosmos;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WithBackendBot.Model;
using Microsoft.Azure.Cosmos;
using AdaptiveCards;

namespace WithBackendBot.Dialogs
{

    public class QueryDialog : ComponentDialog
    {
        private readonly ICosmosService _cosmosService;

        public QueryDialog(ICosmosService cosmosDbService)
    : base(nameof(QueryDialog))
        {
            _cosmosService = cosmosDbService;

            var waterfallSteps = new WaterfallStep[]
            {
                AppNameStepAsync,
                DisplayResultsAsync
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }
        private static async Task<DialogTurnResult> AppNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var appName = (string)stepContext.Options;

            if (String.IsNullOrEmpty(appName))
            {
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What is the name of the app you are intrested in ?")
                    }, cancellationToken);
            }

            return await stepContext.PromptAsync(nameof(TextPrompt),
                   new PromptOptions
                   {
                       Prompt = MessageFactory.Text("Please provide me another app name")
                   }, cancellationToken);

        }
        private async Task<DialogTurnResult> DisplayResultsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string appName = (string)stepContext.Result;

            IEnumerable<Repository> reposataries = await _cosmosService.GetRepositoriesAsync(getQueryDefinition(appName));

            if (reposataries.Count() == 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("There are no repositories assiciated with app name " + appName), cancellationToken);

                return await stepContext.ReplaceDialogAsync(nameof(QueryDialog), appName);
            }
            var reply = MessageFactory.Attachment(AdaptiveCardResposne.getCard(reposataries));
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private QueryDefinition getQueryDefinition(string appName)
        {
            QueryDefinition queryDefinition = new QueryDefinition(
            "select c.dbName, c.instanceName, c.status, c.RDMSType, c.version, c.hostName, c.owner from c where c.appName = @appName")
             .WithParameter("@appName", appName);
            return queryDefinition;
        }



    }
}
