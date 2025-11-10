
using ChatClient.Data;
using Shared;
namespace ChatClient.Data;

public class TESTAMESSAGE
{
    MessageDTO message = new MessageDTO();
    UserAccount user = new UserAccount();
    
    
    public TESTAMESSAGE()
    {
        message.Sender = "Ducklord";
        message.Content = "Hej från klienten!";
        message.Timestamp = DateTime.Now; 
    }
    public MessageDTO GetMessage()
    {
        return message;
    }
  
    
    
}