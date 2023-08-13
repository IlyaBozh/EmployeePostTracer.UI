
namespace EmployeePostTracer.Http.Models;

public class AddLetterRequest
{
    public string Header { get; set; }
    public string Sender { get; set; }
    public string Recipient { get; set; }
    public string Content { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
}