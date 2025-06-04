namespace Remake.WebApi.Entities.DataTransferObjects
{
    public class ServerMessage
    {
        public string Message { get; set; }

        public ServerMessage(string message)
        {
            Message = message;
        }
    }
}
