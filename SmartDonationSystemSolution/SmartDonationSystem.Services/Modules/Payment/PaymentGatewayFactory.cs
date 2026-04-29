using SmartDonationSystem.Core.Modules.Payment.Interfaces;

namespace SmartDonationSystem.Services.Modules.Payment
{
    public class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;

        public PaymentGatewayFactory(IEnumerable<IPaymentGateway> gateways)
        {
            _gateways = gateways;
        }

        public IPaymentGateway Get(string gatewayName)
        {
            var gateway = _gateways
                .FirstOrDefault(g => g.Name.Equals(gatewayName, StringComparison.OrdinalIgnoreCase));

            if (gateway == null)
                throw new Exception("Unsupported payment gateway");

            return gateway;
        }
    }
}
