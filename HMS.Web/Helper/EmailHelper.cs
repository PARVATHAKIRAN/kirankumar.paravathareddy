using SendGrid;
using SendGrid.Helpers.Mail;


namespace HMS.Web.Helper
{
    public static class EmailHelper
    {
        public static async Task<string> Sendmail(string emailid, string mailsubject, string TextContent)
        {
            try
            {
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("vahora226@gmail.com", "Zamirahmad vahora");
                var subject = mailsubject;
                var ToEmail = emailid;
                var to = new EmailAddress(ToEmail, "Example User");
                var plainTextContent = TextContent;
                var htmlContent = "<strong>" + TextContent + "</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    return "ok";
                else 
                    return "failed";

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
