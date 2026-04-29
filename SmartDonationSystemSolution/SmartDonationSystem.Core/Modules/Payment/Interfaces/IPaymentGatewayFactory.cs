namespace SmartDonationSystem.Core.Modules.Payment.Interfaces
{
    public interface IPaymentGatewayFactory
    {
        IPaymentGateway Get(string gatewayName);
    }
}
