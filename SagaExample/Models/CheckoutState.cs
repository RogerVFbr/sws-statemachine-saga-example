using SagaExample.Models.Dtos;

namespace SagaExample.Models
{
    public class CheckoutState
    {
        public StepState<CreateOrderResponseDto, RollbackOrderCreationResponseDto> Order { get; set; }
        public StepState<UpdateInventoryResponseDto, RollbackInventoryResponseDto> Inventory { get; set; }
        public StepState<ExecutePaymentResponseDto, RollbackPaymentResponseDto> Payment { get; set; }

        public CheckoutState()
        {
            Order = new StepState<CreateOrderResponseDto, RollbackOrderCreationResponseDto>();
            Inventory = new StepState<UpdateInventoryResponseDto, RollbackInventoryResponseDto>();
            Payment = new StepState<ExecutePaymentResponseDto, RollbackPaymentResponseDto>();
        }
    }
}