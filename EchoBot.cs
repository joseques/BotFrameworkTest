using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Linq;

namespace BotBuilderTest
{
    public class EchoBot : IBot
    {
        public async Task OnTurn(ITurnContext context)
        {
            //Handling Conversation updates
            if (context.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                //Hey, a new member got added (And not removed)
                if (context.Activity.MembersAdded != null && context.Activity.MembersAdded.Any())
                {
                    //Running a captcha to see if it isn't a bot 🤖
                    ChannelAccount memberAdded = context.Activity.MembersAdded.ToList().Find(newMember => newMember.Id != context.Activity.Recipient.Id);
                    if(memberAdded != null)
                        //Greet the user and explain the bots purpose
                        await context.SendActivity($"Welcome {memberAdded.Name}. My purpose is to count how many times you repeat a word in your message.\nGo Ahead and send me some text!");
                }
            }
            // Handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Echo back to the user the count of whatever were typed.
                await context.SendActivity(countWordsOfText(context.Activity.Text));
            }
        }
        public string countWordsOfText(string text)
        {
            List<string> wordList = new List<string>(text.Replace("\n"," ").Split(" "));
            //Lets count duplicated words
            var result =    from x in wordList
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
