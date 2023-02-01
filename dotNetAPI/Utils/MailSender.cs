using System.Net.Http;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace dotNetAPI.Utils
{
    public class MailSender
    {
        public async Task SendCarReturnMail(string receiver, string carId)
        {
            var apiKey = GetMailApiKey();
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("01151568@pw.edu.pl", "Rent Company (Our)");
            var subject = "Car Return";
            var to = new EmailAddress(receiver);
            var plainTextContent = "Your car (carId=" + carId +") has been returned";
            var htmlContent = "<strong>Your car (carId=" + carId + ") has been returned. Thank you for using our services.</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
        private string GetMailApiKey()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("mailApi.json")
                .Build();

            string mailApi = configuration.GetSection("Keys")["MailApiKey"];
            return mailApi;
        }
    
    }
}
