namespace SchedulingConception
{
    internal class Scheduler
    {
        private readonly Queue<Process> _processes;
        private readonly ExecutionStats _stats;

        private double _deltaTime;
        private readonly ProcessCountInfo _processCountInfo;

        public Scheduler(ProcessCountInfo processCountInfo)
        {
            _processCountInfo = processCountInfo;
            
            _processes = new Queue<Process>(GenerateProcesses(_processCountInfo));
            _stats = new ExecutionStats(_processCountInfo);

            RegisterEvents();
        }

        public void Run(double simulationTime, double deltaTime)
        {
            // Preparation

            _deltaTime = deltaTime;

            var cycles = (int)simulationTime / deltaTime;
            var total = 0;
            var baseCredits = new ProcessCountInfo();

            if (_processCountInfo.Inter > 0)
            {
                total += 4;
                baseCredits.Inter = 4;
            }

            if (_processCountInfo.Computing > 0)
            {
                total += 2;
                baseCredits.Computing = 2;
            }

            if (_processCountInfo.Io > 0)
            {
                total++;
                baseCredits.Io = 1;
            }

            var currentCredits = baseCredits;

            // Scheduling loop

            for (var i = 0; i < cycles && total > 0; i++)
            {
                var currentProcess = _processes.Dequeue();

                while (CreditsSelector(currentProcess, currentCredits) == 0)
                {
                    _processes.Enqueue(currentProcess);
                    currentProcess = _processes.Dequeue();
                }

                currentProcess.Execute();
                Thread.Sleep(TimeSpan.FromSeconds(deltaTime));
                _processes.Enqueue(currentProcess);

                switch (currentProcess.Priority)
                {
                    case ProcessPriority.Io:
                    default:
                        currentCredits.Io--;
                        break;
                    case ProcessPriority.Computing:
                        currentCredits.Computing--;
                        break;
                    case ProcessPriority.Inter:
                        currentCredits.Inter--;
                        break;
                }

                if (currentCredits == new ProcessCountInfo())
                    currentCredits = baseCredits;
            }

            _stats.PrintReport();
        }

        private static uint CreditsSelector(Process process, ProcessCountInfo credits) => process.Priority switch
        {
            ProcessPriority.Inter => credits.Inter,
            ProcessPriority.Computing => credits.Computing,
            _ => credits.Io
        };

        private void UpdateStats(Process process)
        {
            switch (process.Priority)
            {
                case ProcessPriority.Computing:
                default:
                    _stats.ComputingExecutions++;
                    _stats.ComputingTime += _deltaTime;
                    break;
                case ProcessPriority.Inter:
                    _stats.InterExecutions++;
                    _stats.InterTime += _deltaTime;
                    break;
                case ProcessPriority.Io:
                    _stats.IoExecutions++;
                    _stats.IoTime += _deltaTime;
                    break;
            }
        }

        private static IEnumerable<Process> GenerateProcesses(ProcessCountInfo processCountInfo)
        {
            var id = 0;

            for (var i = 0; i < processCountInfo.Inter; i++)
            {
                yield return new Process(id, $"Inter process no. {i}", ProcessPriority.Inter);
                id++;
            }

            for (var i = 0; i < processCountInfo.Computing; i++)
            {
                yield return new Process(id, $"Computing process no. {i}", ProcessPriority.Computing);
                id++;
            }

            for (var i = 0; i < processCountInfo.Io; i++)
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
