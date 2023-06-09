using System;
using System.Diagnostics.Tracing;

namespace SagaExample.Exceptions
{
    public class CheckoutRevertException : Exception
    {
        public CheckoutRevertException(string message) : base(message)
        {
            
        }
    }
}