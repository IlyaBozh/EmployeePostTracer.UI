﻿
namespace EmployeePostTracer.Http.Models;

public class LetterAllInfoResponse : LetterMainInfoResponse
{
    public string Recipient { get; set; }
    public string Content { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
}
