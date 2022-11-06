namespace SchedulingConception
{
    internal class Scheduler
    {
        private readonly Queue<Process> _processes;
        private readonly ExecutionStats _stats;

        public Scheduler(uint interProcCount, uint computingProcCount, uint ioProcCount)
        {
            _processes = new Queue<Process>(GenerateProcesses(interProcCount, computingProcCount, ioProcCount));
            _stats = new ExecutionStats(interProcCount, computingProcCount, ioProcCount);
            RegisterEvents();
        }

        public void Run(uint cycles = 100, double deltaTime = .3)
        {
            var currentProcess = _processes.Dequeue();
            int cyclesForCurrentProcess = (int)currentProcess.Priority;

            for (int i = 0; i < cycles; i++)
            {
                if (cyclesForCurrentProcess != 0)
                {
                    currentProcess.Execute();
                    cyclesForCurrentProcess--;
                    Thread.Sleep(TimeSpan.FromSeconds(deltaTime));
                }

                if (cyclesForCurrentProcess != 0) continue;
                _processes.Enqueue(currentProcess);
                currentProcess = _processes.Dequeue();
                cyclesForCurrentProcess = (int)currentProcess.Priority;
            }

            _stats.PrintReport();
        }

        private void UpdateStats(Process process)
        {
            switch (process.Priority)
            {
                case ProcessPriority.Computing:
                default:
                    _stats.ComputingExecutions++;
                    break;
                case ProcessPriority.Inter:
                    _stats.InterExecutions++;
                    break;
                case ProcessPriority.Io:
                    _stats.IoExecutions++;
                    break;
            }
        }

        private static IEnumerable<Process> GenerateProcesses(uint interProcCount, uint computingProcCount,
            uint ioProcCount)
        {
            int id = 0;

            for (int i = 0; i < interProcCount; i++)
            {
                yield return new Process(id, $"Inter process no. {i}", ProcessPriority.Inter);
                id++;
            }

            for (int i = 0; i < computingProcCount; i++)
            {
                yield return new Process(id, $"Computing process no. {i}", ProcessPriority.Computing);
                id++;
            }

            for (int i = 0; i < ioProcCount; i++)
            {
                yield return new Process(id, $"I/O process no. {i}", ProcessPriority.Io);
                id++;
            }
        }

        private void RegisterEvents()
        {
            foreach (var process in _processes)
                process.ProcessExecuted += UpdateStats;
        }
    }
}
