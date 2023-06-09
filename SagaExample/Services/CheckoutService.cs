using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SagaExample.Exceptions;
using SagaExample.Models;
using SagaExample.Repositories;
using SagaExample.Repositories.Interfaces;
using SagaExample.Services.Interfaces;
using SWS.StateMachine.Models;
using SWS.StateMachine.States.Task;

namespace SagaExample.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ILogger<CheckoutService> _logger;
        
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInventoryRepository _inventoryRepository;
        
        private readonly ITask<CheckoutState, CheckoutState> _executeOrderTask;
        private readonly ITask<CheckoutState, CheckoutState> _executePaymentTask;
        private readonly ITask<CheckoutState, CheckoutState> _updateInventoryTask;
        
        private readonly ITask<CheckoutState, CheckoutState> _rollbackOrderTask;
        private readonly ITask<CheckoutState, CheckoutState> _rollbackPaymentTask;
        private readonly ITask<CheckoutState, CheckoutState> _rollbackInventoryTask;
        
        private readonly ITask<CheckoutState, CheckoutState> _rollbackFinishedTask;

        public CheckoutService(
            ILogger<CheckoutService> logger,
            IOrderRepository orderRepository, 
            IPaymentRepository paymentRepository, 
            IInventoryRepository inventoryRepository, 
            ITask<CheckoutState, CheckoutState> executeOrderTask, 
            ITask<CheckoutState, CheckoutState> executePaymentTask, 
            ITask<CheckoutState, CheckoutState> updateInventoryTask,
            ITask<CheckoutState, CheckoutState> rollbackOrderTask, 
            ITask<CheckoutState, CheckoutState> rollbackPaymentTask, 
            ITask<CheckoutState, CheckoutState> rollbackInventoryTask,
            ITask<CheckoutState, CheckoutState> rollbackFinishedTask
            )
        {
            _logger = logger;
            
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _inventoryRepository = inventoryRepository;
            
            _executeOrderTask = executeOrderTask;
            _executePaymentTask = executePaymentTask;
            _updateInventoryTask = updateInventoryTask;
            
            _rollbackOrderTask = rollbackOrderTask;
            _rollbackPaymentTask = rollbackPaymentTask;
            _rollbackInventoryTask = rollbackInventoryTask;
            _rollbackFinishedTask = rollbackFinishedTask;

            SetupStateMachineCheckout();
            SetupStateMachineRollback();
            SetupStateMachineDefaults();
        }

        public CheckoutState ExecuteCheckout()
        {
            var checkoutState = new CheckoutState();
            _executeOrderTask.Execute(checkoutState);
            return checkoutState;
        }

        private void SetupStateMachineCheckout()
        {
            _executeOrderTask
                .Name("Create Order")
                .AddPreHook(task => task.InputPayload.Order.Progress.Started())
                .AddSuccessHook(task => task.InputPayload.Order.Progress.Succeeded())
                .AddFailureHook(task => task.InputPayload.Order.Progress.Failed())
                .AddRetryHook(task => task.InputPayload.Order.Progress.Retry())
                .NextState(_executePaymentTask)
                .AddCatchPolicy(_rollbackFinishedTask)
                .Resource(checkoutState =>
                {
                    var response = _orderRepository.CreateOrder();
                    checkoutState.Order.SetExecutePayload(response);
                    return checkoutState;
                });
            
            _executePaymentTask
                .Name("Execute Payment")
                .AddPreHook(task => task.InputPayload.Payment.Progress.Started())
                .AddSuccessHook(task => task.InputPayload.Payment.Progress.Succeeded())
                .AddFailureHook(task => task.InputPayload.Payment.Progress.Failed())
                .AddRetryHook(task => task.InputPayload.Payment.Progress.Retry())
                .NextState(_updateInventoryTask)
                .AddCatchPolicy(_rollbackOrderTask)
                .Resource(checkoutState =>
                {
                    var response = _paymentRepository.ExecutePayment();
                    checkoutState.Payment.SetExecutePayload(response);
                    return checkoutState;
                });
            
            _updateInventoryTask
                .Name("Update Inventory")
                .AddPreHook(task => task.InputPayload.Inventory.Progress.Started())
                .AddSuccessHook(task => task.InputPayload.Inventory.Progress.Succeeded())
                .AddFailureHook(task => task.InputPayload.Inventory.Progress.Failed())
                .AddRetryHook(task => task.InputPayload.Inventory.Progress.Retry())
                .AddCatchPolicy(_rollbackPaymentTask)
                .Resource(checkoutState =>
                {
                    var response = _inventoryRepository.UpdateInventory();
                    checkoutState.Inventory.SetExecutePayload(response);
                    return checkoutState;
                });
        }
        
        private void SetupStateMachineRollback()
        {
            _rollbackInventoryTask
                .Name("Inventory Rollback")
                .AddPreHook(task => task.InputPayload.Inventory.Progress.RollbackStarted())
                .AddSuccessHook(task => task.InputPayload.Inventory.Progress.RollbackSucceeded())
                .AddFailureHook(task => task.InputPayload.Inventory.Progress.RollbackFailed())
                .AddRetryHook(task => task.InputPayload.Inventory.Progress.Retry())
                .NextState(_rollbackPaymentTask)
                .Resource(checkoutState =>
                {
                    var response = _inventoryRepository.RollbackInventoryUpdate();
                    checkoutState.Inventory.SetRollbackPayload(response);
                    return checkoutState;
                });
            
            _rollbackPaymentTask
                .Name("Payment Rollback")
                .AddPreHook(task => task.InputPayload.Payment.Progress.RollbackStarted())
                .AddSuccessHook(task => task.InputPayload.Payment.Progress.RollbackSucceeded())
                .AddFailureHook(task => task.InputPayload.Payment.Progress.RollbackFailed())
                .AddRetryHook(task => task.InputPayload.Payment.Progress.Retry())
                .NextState(_rollbackOrderTask)
                .Resource(checkoutState =>
                {
                    var response = _paymentRepository.RollbackPayment();
                    checkoutState.Payment.SetRollbackPayload(response);
                    return checkoutState;
                });
                                    
            _rollbackOrderTask
                .Name("Order Rollback")
                .AddPreHook(task => task.InputPayload.Order.Progress.RollbackStarted())
                .AddSuccessHook(task => task.InputPayload.Order.Progress.RollbackSucceeded())
                .AddFailureHook(task => task.InputPayload.Order.Progress.RollbackFailed())
                .AddRetryHook(task => task.InputPayload.Order.Progress.Retry())
                .NextState(_rollbackFinishedTask)
                .Resource(checkoutState =>
                {
                    var response = _orderRepository.RollbackOrderCreation();
                    checkoutState.Order.SetRollbackPayload(response);
                    return checkoutState;
                });
            
            _rollbackFinishedTask
                .AddPreHook(task => _logger.LogInformation("Rollback finished"));
        }

        private void SetupStateMachineDefaults()
        {
            new List<ITask<CheckoutState, CheckoutState>>
            {
                _executeOrderTask, _executePaymentTask, _updateInventoryTask, 
                _rollbackOrderTask, _rollbackPaymentTask, _rollbackInventoryTask
            }
                .ForEach(stateMachineTask => stateMachineTask
                    .AddRetryPolicy(1, 2, 2)
                    .AddPreHook(task => _logger.LogInformation("Requesting '{TaskName}' ...", task.Name))
                    .AddRetryHook(task => LogRetry(task.Name, task.RetryContext, task.Elapsed))
                    .AddSuccessHook(task => _logger.LogInformation("'{TaskName}' completed. Elapsed: {Elapsed}ms", task.Name, task.Elapsed))
                    .AddFailureHook(task => LogFailure(task.Name, task.Exception, task.Elapsed))
                );
        }
        
        private void LogFailure(string prefix, Exception exception, long elapsed) => 
            _logger.LogError("'{TaskName}' failed. Elapsed: {Elapsed}ms. Exception: {Type}. Message: {Message}",
                prefix, elapsed, exception.GetType().ToString(), exception.Message);
        
        private void LogRetry(string prefix, RetryContext retryContext, long elapsed) => 
            _logger.LogWarning(
                "'{TaskName}' failed. Elapsed: {Elapsed}ms. Exception: {Type}. Message: {Message}. Retrying " +
                "(Attempt {RetryCount}/{MaxRetries}, awaiting {}s)...",
                prefix, elapsed, retryContext.GetLatestException().GetType().ToString(), 
                retryContext.GetLatestException().Message, retryContext.AttemptCount, retryContext.MaxAttempts,
                retryContext.Delay);
    }
}