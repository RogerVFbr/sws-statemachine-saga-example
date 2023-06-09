using System;
using Newtonsoft.Json;
using SagaExample.Services.Interfaces;

namespace SagaExample.Controllers
{
    public class CheckoutController : ICheckoutController
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        public void ExecuteCheckout()
        {
            var result = _checkoutService.ExecuteCheckout();
            Console.Write(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}