using OpenAI.Chat;
using System.Reflection.Metadata;
using umbraco14test_2.Models;
using umbraco14test_2.Utils;

namespace umbraco14test_2.Services
{
    public class ChatGptService: IChatGptService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        public ChatGptService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ChatClient> CreateClient() => new ChatClient(model: LocalConstants.GPT_40_MINI, GetApiKey());

        public IEnumerable<ChatMessage> ParseMessages(MessagesModel messageList)
        {
            List<ChatMessage> messagesParsed = new();

            //messagesParsed.Add(new SystemChatMessage("Please avoid all types of special characteres, at the beggining of all your messages --message-- and at the end --/message-- "));

            var rootPrompt = "If you are completely sure that the user is requesting a page please add inside this tags '<pagerequested></pagerequested>' only the needed properties '<pagetitle></pagetitle>' and '<pagecontent></pagecontent>' with a value you suggest after the user tells you what is the page about, ensure that '<pagecontent></pagecontent>' has 2 paragraphs  e.g, if a user ask you a page about nature you could say '<pagerequested><pagetitle>Nature is beautiful</pagetitle><pagecontent>lorem ipsum</pagecontent></pagerequested>'";

            foreach (var chatItem in messageList.Messages)
            {
                

                if (chatItem.Role == "user")
                {
                    messagesParsed.Add( new UserChatMessage(chatItem.Content + rootPrompt));
                }
                else if (chatItem.Role == "bot")
                {
                    messagesParsed.Add(new AssistantChatMessage(chatItem.Content));
                }
            }
            return messagesParsed;
        }
        private string GetApiKey() => _configuration.GetValue<string>("OPENAI_API_KEY") ?? "";
    }
}
