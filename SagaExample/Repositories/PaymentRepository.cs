using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using SagaExample.Exceptions;
using SagaExample.Models.Dtos;
using SagaExample.Repositories.Interfaces;

namespace SagaExample.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(ILogger<PaymentRepository> logger)
        {
            _logger = logger;
        }

        public ExecutePaymentResponseDto ExecutePayment()
        {
            _logger.LogDebug("Executing Payment ...");
            Thread.Sleep(1000);
            // throw new Exception("Payment went wrong.");
            return new ExecutePaymentResponseDto();
        }
        
        public RollbackPaymentResponseDto RollbackPayment()
        {
            _logger.LogDebug("Reverting Payment ...");
            Thread.Sleep(1000);
            // throw new Exception("Payment Rollback went wrong.");
            return new RollbackPaymentResponseDto();
        }
    }
}