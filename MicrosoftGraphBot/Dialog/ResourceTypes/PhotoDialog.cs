using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftGraphBot.Dialog.ResourceTypes
{
    public class PhotoDialog : IDialog<bool>
    {
        /// <summary>
        /// Called to start a dialog
        /// </summary>
        /// <param name="context">IDialogContext</param>
        /// <returns></returns>
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        /// <summary>
        /// Processes messages received on new thread
        /// </summary>
        /// <param name="context">IDialogContext</param>
        /// <param name="item">Awaitable IMessageActivity</param>
        /// <returns>Task</returns>
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var token = await context.GetAccessToken();
            // Get the users profile photo from the Microsoft Graph
            //var bytes = await new HttpClient().GetStreamWithAuthAsync(token, "https://graph.microsoft.com/v1.0/me/photo/$value");
            byte[] bytes; 
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me/photo/$value")) 
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);

                    var pic = "data:image/png;base64," + Convert.ToBase64String(bytes);

                    // Output the picture as a base64 encoded string
                    var msg = context.MakeMessage();
                    msg.Text = "Check it out...I got your picture using the Microsoft Graph!";
                    msg.Attachments.Add(new Attachment("image/png", pic));
                    await context.PostAsync(msg);
                }
                
            }

           
        }
    }
}
