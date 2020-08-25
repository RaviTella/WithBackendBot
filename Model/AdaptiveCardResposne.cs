using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WithBackendBot.Cosmos;

namespace WithBackendBot.Model
{
    public class AdaptiveCardResposne
    {
        public static Attachment getCard(IEnumerable<Repository> reposataries)
        {
            var paths = new[] { ".", "Resources", "RepoAdaptiveCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));
            var template = new AdaptiveCards.Templating.AdaptiveCardTemplate(adaptiveCardJson);

            // "Expand" the template - this generates the final Adaptive Card payload
            var value = new Value();
            value.reposataries = reposataries;
            string cardJson = template.Expand(value);
            var results = AdaptiveCard.FromJson(cardJson).Card;

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = results,
            };
            return adaptiveCardAttachment;
        }

    }
}
