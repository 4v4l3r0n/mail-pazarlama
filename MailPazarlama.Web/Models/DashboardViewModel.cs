namespace MailPazarlama.Web.Models;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public int SubscriberCount { get; set; }
    public int TemplateCount { get; set; }
    public int SentMailCount { get; set; }
}
