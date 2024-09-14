using OpenAI.Chat;
using umbraco14test_2.Models;

namespace umbraco14test_2.Services
{
    public interface IChatGptService
    {
        Task<ChatClient> CreateClient();
        IEnumerable<ChatMessage> ParseMessages(MessagesModel messageList);
    }
}
