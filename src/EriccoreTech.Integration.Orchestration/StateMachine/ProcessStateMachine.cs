namespace EriccoreTech.Integration.Orchestration {
    public sealed class ProcessStateMachine {

        private static readonly IReadOnlyDictionary<ProcessStatus, ProcessStatus[]> Allowed =
            new Dictionary<ProcessStatus, ProcessStatus[]> {
                [ProcessStatus.Created] = [ProcessStatus.Running, ProcessStatus.Cancelled],
                [ProcessStatus.Running] = [ProcessStatus.Waiting, ProcessStatus.Completed, ProcessStatus.Failed, ProcessStatus.Cancelled],
                [ProcessStatus.Waiting] = [ProcessStatus.Running, ProcessStatus.Cancelled],
                [ProcessStatus.Failed] = [ProcessStatus.Running],
                [ProcessStatus.Completed] = [],
                [ProcessStatus.Cancelled] = []
            };

        public static ProcessStatus Transition(ProcessStatus current, ProcessStatus next) {
            if (!Allowed[current].Contains(next)) {
                throw new InvalidOperationException($"Invalid transition: {current} -> {next}");
            }

            return next;
        }

    }
}
