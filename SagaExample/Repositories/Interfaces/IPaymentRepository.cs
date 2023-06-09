using SagaExample.Models.Dtos;

namespace SagaExample.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        ExecutePaymentResponseDto ExecutePayment();
        RollbackPaymentResponseDto RollbackPayment();
    }
}