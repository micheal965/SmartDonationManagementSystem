namespace SmartDonationSystem.Core.Modules.Messaging.DTOs
{
    public class MessageParticipantsPayload
    {
        public string SenderName { get; set; }
        public string SenderImage { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverImage { get; set; }
    }
}
