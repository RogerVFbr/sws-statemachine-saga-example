using System;

namespace SagaExample.Models
{
    public class TimedEntity<T>
    {
        public T Value { get; set; }
        public DateTime CreatedAt { get; set; }

        public TimedEntity(T value)
        {
            Value = value;
            CreatedAt = DateTime.Now;
        }
    }
}