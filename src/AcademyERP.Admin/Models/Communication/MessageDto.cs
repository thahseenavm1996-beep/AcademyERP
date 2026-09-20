namespace AcademyERP.Admin.Models.Communication;

public class MessageDto
{
    public Guid Id {get;set;}

    public string SenderName {get;set;}="";

    public string MessageText {get;set;}="";

    public DateTime SentAt {get;set;}
}