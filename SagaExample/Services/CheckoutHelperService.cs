using Microsoft.Extensions.Logging;
using SagaExample.Models;
using SagaExample.Repositories.Interfaces;
using SWS.StateMachine.Models;

namespace SagaExample.Services
{
    public class CheckoutHelperService
    {
        private readonly ILogger<CheckoutHelperService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public CheckoutHelperService(
            ILogger<CheckoutHelperService> logger,
            IOrderRepository orderRepository, 
            IPaymentRepository paymentRepository, 
            IInventoryRepository inventoryRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _inventoryRepository = inventoryRepository;
        }
        
        private void SetProgressByStatus(ExecutionStatus executionStatus, StepProgress progress)
        {
            if (executionStatus != ExecutionStatus.Success)
                progress.Failed();
            else
                progress.Succeeded();
        }

        private void LogCompletion(string prefix, ExecutionStatus executionStatus, long elapsed)
        {
            var message = $"'{prefix}' completed (Status: {executionStatus} | Elapsed: {elapsed}ms)";
            
            if (executionStatus == ExecutionStatus.Success)
                _logger.LogInformation(message);
            else
                _logger.LogError(message);
        }
    }
}