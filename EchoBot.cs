using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Core.Extensions;
using System.Text.RegularExpressions;

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
                        await context.SendActivity($"Welcome {memberAdded.Name}. My purpose is to count how many times you repeated words in your messages.\nGo Ahead and send me some text! 🤖");
                }
            }
            // Handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                string finalAnswer = "I can't process things that aren't text. Try sending me some words!";
                string text = context.Activity.Text;
                if (!string.IsNullOrEmpty(text)) {
                    List<CountState> state = context.GetConversationState<List<CountState>>();
                    // Text to lowercase, trimmed start and end spaces and replace some characters that make the spliting wrong
                    string message = context.Activity.Text.ToLowerInvariant();
                    // Echo back to the user the count of whatever were typed.
                    string resultCount = countWordsOfText(message, state);
                    if (!string.IsNullOrEmpty(resultCount))
                    {
                        finalAnswer = resultCount;
                    }
                }
                await context.SendActivity(finalAnswer);
            }
        }
        public string countWordsOfText(string text, List<CountState> currentState)
        {
            string multipleSpacesPattern = "\\s\\s+";
            string notWordPattern = "\\W";
            Regex rgx = new Regex(notWordPattern);
            text = rgx.Replace(text, " ");
            //Since the replacement fills the string with multiple spaces, we run another replacing the spaces with a single one
            //Could be solved with a more complex regex and a single Replace
            rgx = new Regex(multipleSpacesPattern);
            text = rgx.Replace(text, " ");
            List<string> wordList = new List<string>(text.Split(" "));
            //Lets count duplicated words
            var result =    from x in wordList
                            group x by x into g
                            let count = g.Count()
                            orderby count descending
                            select new { Word = g.Key, Count = count };
            string finalString = "";
            foreach (var item in result)
            {   
                //if found its in list, add the count to the existing element. Otherwise add a new one
                int index = currentState.FindIndex(st => st.Word == item.Word);
                if (item.Word.Length > 0)
                    if (index > -1) //Word isn't empty
                    {
                        currentState[index].Count += item.Count;
                    }
                    else
                    {
                        currentState.Add(new CountState());
                        currentState.Last().Word = item.Word;
                        currentState.Last().Count += item.Count;
                    }
            }
            currentState = currentState.OrderByDescending(x=>x.Count).ToList();
            foreach(CountState cs in currentState)
            {
                finalString += $"\n{cs.Word} - {cs.Count}";
            }
            return finalString;
        }
    }
}
