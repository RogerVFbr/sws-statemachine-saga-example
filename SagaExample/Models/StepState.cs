using System;
using System.Collections.Generic;
using SagaExample.Models.Dtos;

namespace SagaExample.Models
{
    public class StepState<T, U>
    {
        public TimedEntity<T> ExecutePayload { get; set; }
        public TimedEntity<U> RollbackPayload { get; set; }
        public StepProgress Progress { get; set; } = new();

        public void SetExecutePayload(T payload) => ExecutePayload = new TimedEntity<T>(payload);
        public void SetRollbackPayload(U payload) => RollbackPayload = new TimedEntity<U>(payload);
    }

}