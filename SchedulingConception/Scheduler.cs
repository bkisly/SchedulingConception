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

        public void Run(double simulationTime = 7, double deltaTime = .01)
        {
            // Preparation

            _deltaTime = deltaTime;

            var cycles = (int)(simulationTime / deltaTime);
            var executedProcesses = new List<Process>();
            var pidToTimeLeft = GetExecTimeDictionary(simulationTime);

            // Scheduling loop

            for (var i = 0; i < cycles; i++)
            {
                var currentProcess = _processes.Dequeue();

                if (pidToTimeLeft[currentProcess.Pid] > 0)
                {
                    currentProcess.Execute();
                    pidToTimeLeft[currentProcess.Pid] -= deltaTime;
                    Thread.Sleep(TimeSpan.FromSeconds(_deltaTime));
                }

                if (pidToTimeLeft[currentProcess.Pid] <= 0)
                {
                    executedProcesses.Add(currentProcess);

                    if (_processes.Count == 0 && executedProcesses.Count > 0)
                    {
                        foreach (var process in executedProcesses)
                            _processes.Enqueue(process);

                        executedProcesses.Clear();
                    }
                }
                else _processes.Enqueue(currentProcess);
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

        private Dictionary<int, double> GetExecTimeDictionary(double schedulingTime)
        {
            var total = 0;
            if (_processCountInfo.InterProcCount > 0) total += 4;
            if (_processCountInfo.ComputingProcCount > 0) total += 2;
            if (_processCountInfo.IoProcCount > 0) total++;

            return _processes.ToDictionary(process => process.Pid, process => process.Priority switch
            {
                ProcessPriority.Inter => (4d / total) * schedulingTime / _processCountInfo.InterProcCount,
                ProcessPriority.Computing => (2d / total) * schedulingTime / _processCountInfo.ComputingProcCount,
                _ => (1d / total) * schedulingTime / _processCountInfo.IoProcCount
            });
        }

        private static IEnumerable<Process> GenerateProcesses(ProcessCountInfo processCountInfo)
        {
            var id = 0;

            for (var i = 0; i < processCountInfo.InterProcCount; i++)
            {
                yield return new Process(id, $"Inter process no. {i}", ProcessPriority.Inter);
                id++;
            }

            for (var i = 0; i < processCountInfo.ComputingProcCount; i++)
            {
                yield return new Process(id, $"Computing process no. {i}", ProcessPriority.Computing);
                id++;
            }

            for (var i = 0; i < processCountInfo.IoProcCount; i++)
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
