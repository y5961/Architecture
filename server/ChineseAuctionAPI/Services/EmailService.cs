using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService1
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _config.GetSection("EmailSettings");

        using var client = new SmtpClient(emailSettings["SmtpServer"])
        {
            Port = int.Parse(emailSettings["Port"]),
            Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true, 
        };

        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }

    public string CreateWinnerTemplate(string userName, string giftName)
    {
        return $@"
        <div dir='rtl' style='font-family: Arial, sans-serif; border: 2px solid #FFD700; padding: 30px; border-radius: 15px; max-width: 500px; margin: auto; background-color: #ffffff; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
            <div style='text-align: center;'>
                <h1 style='color: #d4af37; font-size: 28px; margin-bottom: 10px;'>🎉 איזה כיף! יש לנו זוכה! 🎉</h1>
                <p style='font-size: 18px; color: #333;'>שלום <strong>{userName}</strong>,</p>
                <p style='font-size: 16px; color: #555;'>אנו נרגשים לבשר לך ששמך עלה בגורל בהגרלה הסינית!</p>
               
                <div style='background-color: #fdf2e9; border: 1px dashed #e67e22; padding: 20px; margin: 20px 0; border-radius: 10px;'>
                    <p style='font-size: 14px; color: #7f8c8d; margin: 0;'>הפרס בו זכית:</p>
                    <h2 style='color: #e67e22; margin: 10px 0;'>{giftName}</h2>
                </div>

                <p style='font-size: 16px; color: #333;'>נציג מטעמנו ייצור איתך קשר בהקדם לתיאום קבלת הפרס.</p>
                <br>
                <p style='font-size: 14px; color: #95a5a6;'>תודה שהשתתפת ובהצלחה בהגרלות הבאות!</p>
                <div style='margin-top: 25px; font-weight: bold; color: #d4af37;'>צוות המכירה הסינית</div>
            </div>
        </div>";
    }

}