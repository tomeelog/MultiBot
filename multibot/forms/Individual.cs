using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Threading.Tasks;



namespace multibot.forms
{
    [Serializable]
    public class MyAwaitableImage : AwaitableAttachment
    {
        // Mandatory: you should have this ctor as it is used by the recognizer
        public MyAwaitableImage(Microsoft.Bot.Connector.Attachment source) : base(source) { }

        // Mandatory: you should have this serialization ctor as well & call base
        protected MyAwaitableImage(SerializationInfo info, StreamingContext context) : base(info, context) { }

        // Optional: here you can check for content-type for ex 'image/png' or other..
        public override async Task<ValidateResult> ValidateAsync<T>(IField<T> field, T state)
        {
            var result = await base.ValidateAsync(field, state);

            if (result.IsValid)
            {
                var isValidForMe = this.Attachment.ContentType.ToLowerInvariant().Contains("image/png");

                if (!isValidForMe)
                {
                    result.IsValid = false;
                    result.Feedback = $"Hey, dude! Provide a proper 'image/png' attachment, not any file on your computer like '{this.Attachment.Name}'!";
                }
                else
                {
                    var url = this.Attachment.ContentUrl;

                    HttpClient httpClient = new HttpClient();
                    Stream filestrem = await httpClient.GetStreamAsync(url);
                    httpClient.Dispose();

                    byte[] ImageAsByteArray = null;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            byte[] buf = new byte[1024];
                            count = filestrem.Read(buf, 0, 1024);
                            ms.Write(buf, 0, count);
                        } while (filestrem.CanRead && count > 0);
                        ImageAsByteArray = ms.ToArray();
                    }

                    HttpClient client = new HttpClient();

                    // Request headers.
                    client.DefaultRequestHeaders.Add(
                        "Ocp-Apim-Subscription-Key", "fcb7a03c796c40a3954a249c9b87d79c");

                    // Request parameters. A third optional parameter is "details".
                    string requestParameters =
                    "visualFeatures=Categories,Description,Color";

                    // Assemble the URI for the REST API Call.
                    string uri = "https://southcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?" + requestParameters;
                    
                    HttpResponseMessage response;


                    using (ByteArrayContent content = new ByteArrayContent(ImageAsByteArray))
                    {
                        // This example uses content type "application/octet-stream".
                        // The other content types you can use are "application/json"
                        // and "multipart/form-data".
                        content.Headers.ContentType =
                            new MediaTypeHeaderValue("application/octet-stream");

                        // Make the REST API call.
                        response = await client.PostAsync(uri, content);
                    }

                    // Get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(contentString);
                    var rs = Newtonsoft.Json.Linq.JToken.Parse(contentString);

                    if (rs.HasValues)
                    {
                        string val = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)rs).First).First).First).First).Value).Value.ToString();

                        Result rsult = JsonConvert.DeserializeObject<Result>(contentString);

                        var f = rsult.description;
                        string r = f.captions[0].text.ToString();
                        Console.WriteLine(r);
                        if (!val.Contains("people_"))
                        {

                            result.IsValid = false;
                            result.Feedback = $"I can see {r}. please upload your lovely image ";
                        }
                    }
                }
            }
            return result;
        }
        // Optional: here you can provide additional or override custom help text completely..
        public override string ProvideHelp<T>(IField<T> field)
        {
            var help = base.ProvideHelp(field);

            help += $"{Environment.NewLine}- Only 'image/png' can be attached to this field.";

            return help;
        }

        // Optional: here you can define your custom logic to get the attachment data or add custom logic to check it, etc..
        protected override async Task<Stream> ResolveFromSourceAsync(Microsoft.Bot.Connector.Attachment source)
        {
            var result = await base.ResolveFromSourceAsync(source);
                        
            return result;
        }
    }

    [Serializable]
    public class Individual
    {
        [Prompt("What's your name?")]
        public string Name { get; set; }

        [Prompt("Upload your image")]
        public MyAwaitableImage Image;

        [Prompt("What's your Nationality?")]
        public string Country { get; set; }

        [Prompt("What's your e-mail address?")]
        public string Email { get; set; }

        public static IForm<Individual> BuildIndividualForm()
        {
            OnCompletionAsyncDelegate<Individual> wrapUpRequest = async (context, state) =>
            {
                string name = state.Name.ToString();
                string country = state.Country.ToString();

                string status = "pending";
                
                string shortUrl = System.Web.Security.Membership.GeneratePassword(11, 0);
                Console.WriteLine(shortUrl);
                var url = state.Image.Attachment.ContentUrl;
                var httpClient = new HttpClient();
                
                var f1 = await httpClient.GetByteArrayAsync(url);
                              
                String s = Convert.ToBase64String(f1);

                //string constr = ConfigurationManager.ConnectionStrings["ASPNETConnectionString"].ToString();
                //SqlConnection conn = new SqlConnection(constr);

                //string sql = "INSERT into dbo.Registration (name,nationality,image,status,guid) VALUES ('" + name + "','" + country + "','" + s + "','" + status + "','" + shortUrl + "');";
                //SqlCommand cmd = new SqlCommand(sql);
                //cmd.Connection = conn;
                //try
                //{
                //    conn.Open();
                //    cmd.ExecuteNonQuery();
                await context.PostAsync($"Your details has been received. Please copy the code below to always check your registration status in the next 24hrs.");
                await context.PostAsync($"{shortUrl}");

                //    //Using The SMS gateway

                //    //string username = "sandbox";
                //    //string apiKey = "9112fbe9589fbe898f1e70ee8d5103c2450abddf272bac2df389ed33c2a52727";

                //    //string recipients = "+2348022155349,+2348165071261";

                //    //string message = $"Please use the code {shortUrl} to check your registration status";

                //    //AfricasTalkingGateway gateway = new AfricasTalkingGateway(username, apiKey);

                //    //try
                //    //{
                //    //    dynamic results = gateway.sendMessage(recipients, message);
                //    //    foreach (dynamic result in results)
                //    //    {

                //    //        Console.Write((string)result["status"] + ",");
                //    //        Console.Write((string)result["number"] + ",");
                //    //    }
                //    //}
                //    //catch (AfricasTalkingGatewayException e)
                //    //{
                //    //    Console.WriteLine("Encountered an error: " + e.Message);
                //    //    Console.WriteLine("Encountered an error: " + e.Message);

                //    //}

                //    //MailMessage mail = new MailMessage("you@yourcompany.com", "tomeelog@gmail.com");
                //    //SmtpClient client = new SmtpClient();
                //    //client.Port = 25;
                //    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //    //client.UseDefaultCredentials = false;
                //    //client.Host = "smtp.gmail.com";
                //    //mail.Subject = "this is a test email.";
                //    //mail.Body = "this is my test email body";
                //    //client.Send(mail);
                //    //Console.WriteLine(mail);
                //}
                //catch (Exception ex)
                //{
                //    await context.PostAsync($"cannot connect to database");
                //    await context.PostAsync($"{ex}"); 
                //}
                //finally
                //{
                //    conn.Close();
                //}
            };

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");

            return new FormBuilder<Individual>().Message("Kindly Provide your details!").Field("Name").Field("Email")
                .AddRemainingFields().Confirm("Please double check. Have you provided the right information?\n{*}").OnCompletion(wrapUpRequest).Build();
        }

        private static async Task<long> RetrieveAttachmentSizeAsync(AwaitableAttachment attachment)
        {
            var stream = await attachment;
            return stream.Length;
        }

       
            
        
    }
}