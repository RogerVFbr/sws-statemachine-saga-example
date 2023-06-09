using System.Collections.Generic;
using SagaExample.Models.Dtos;

namespace SagaExample.Models
{
    public class StepProgress
    {
        public readonly List<TimedEntity<string>> Data = new() {new TimedEntity<string>("Created")};

        public void Started() => Data.Add(new TimedEntity<string>("Started"));
        public void Succeeded() => Data.Add(new TimedEntity<string>("Succeeded"));
        public void Failed() => Data.Add(new TimedEntity<string>("Failed"));
        
        public void Retry() => Data.Add(new TimedEntity<string>("Retry"));
        public void RollbackStarted() => Data.Add(new TimedEntity<string>("RollbackStarted"));
        
        public void RollbackSucceeded() => Data.Add(new TimedEntity<string>("RollbackSucceeded"));
        public void RollbackFailed() => Data.Add(new TimedEntity<string>("RollbackFailed"));
    }
}