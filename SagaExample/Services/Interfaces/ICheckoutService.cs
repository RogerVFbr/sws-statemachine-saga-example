using SagaExample.Models;

namespace SagaExample.Services.Interfaces
{
    public interface ICheckoutService
    {
        CheckoutState ExecuteCheckout();
    }
}