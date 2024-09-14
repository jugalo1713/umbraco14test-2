using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using umbraco14test_2.Models;
using umbraco14test_2.Services;


namespace umbraco14test_2.controllers
{
    [ApiController]
	[Route("[controller]/[action]")]
	public class ChatbotController : UmbracoApiController
	{
        private readonly IConfiguration _configuration;
        private readonly IChatGptService _gptService;
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;




		public ChatbotController(IConfiguration configuration, IChatGptService gptService, IContentService contentService, IContentTypeService contentTypeService)
        {
            _configuration = configuration;
            _gptService = gptService;
            _contentService = contentService;
            _contentTypeService = contentTypeService;
        }

        [HttpPost]
        public async Task<string> Chat([FromBody] MessagesModel messageList)
        {
            try
            {
                var client = await _gptService.CreateClient();

                List<ChatMessage> messagesParsed = new();
                
                messagesParsed.AddRange(_gptService.ParseMessages(messageList).ToList());

                ChatCompletion completion = client.CompleteChat(messagesParsed);
                var completinResponse = completion.ToString();


                string pattern = @"<pagerequested>.*<\/pagerequested>";
                Match match = Regex.Match(completinResponse, pattern);

                if (match.Success)
                {
					// Extracted XML portion
					string xmlString = match.Value;

                    // Now parse the extracted XML
                    XElement root = XElement.Parse(xmlString);
                    string pageTitle = root.Element("pagetitle")?.Value;
                    string pageContent = root.Element("pagecontent")?.Value;

					var contentType = _contentTypeService.Get("contentAi");
					var parentId = Guid.Parse("ca4249ed-2b23-4337-b522-63cabe5587d1");

                    // Create a new child item of type 'Product'
                    var newPage = _contentService.Create(pageTitle, parentId, contentType.Alias);

					// Set the value of the property with alias 'price'
					newPage.SetValue("contentTitle", pageTitle);
					newPage.SetValue("pageContent", pageContent);

					// Save and publish the child item
					var contentCreated = _contentService.Save(newPage);
                    //var urlCreated = contentCreated.Url();
					completinResponse = $"He creado la pag {pageTitle}";
				}


                return completinResponse;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
