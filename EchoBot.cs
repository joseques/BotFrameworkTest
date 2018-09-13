using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BotBuilderTest
{
    public class EchoBot : IBot
    {
        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Echo back to the user whatever they typed.
                await context.SendActivity(countWordsOfText(context.Activity.Text));
            }
        }
        public string countWordsOfText(string text)
        {
            List<string> wordList = new List<string>(text.Replace("\n"," ").Split(" "));                
            var result = from x in wordList
                            group x by x into g
                            let count = g.Count()
                            orderby count descending
                            select new { Value = g.Key, Count = count };
            string finalString = "";
            foreach (var item in result)
            {
                finalString += $"\n{item.Value} - {item.Count}";
            }
            return finalString;
        }
    }
}
