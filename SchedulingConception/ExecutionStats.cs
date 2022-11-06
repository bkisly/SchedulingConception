namespace SchedulingConception
{
    internal class ExecutionStats
    {
        private readonly uint _compProcCount, _interProcCount, _ioProcCount;

        public uint ComputingExecutions { get; set; }
        public uint InterExecutions { get; set; }
        public uint IoExecutions { get; set; }

        public uint TotalExecutions => ComputingExecutions + InterExecutions + IoExecutions;

        public ExecutionStats(uint interProcCount, uint compProcCount, uint ioProcCount)
        {
            _compProcCount = compProcCount;
            _interProcCount = interProcCount;
            _ioProcCount = ioProcCount;
        }

        public void PrintReport()
        {
            Console.WriteLine("\n============= EXECUTION STATS =============\n");
            Console.WriteLine("~ Input data ~");
            Console.WriteLine($"{_compProcCount} computing processes, {_interProcCount} inter processes, {_ioProcCount} I/O processes.\n");

            Console.WriteLine("~ Results ~");
            Console.WriteLine($"{ComputingExecutions} executions of computing processes");
            Console.WriteLine($"{InterExecutions} executions of inter processes");
            Console.WriteLine($"{IoExecutions} executions of I/O processes");
            Console.WriteLine($"{TotalExecutions} total executions\n");

            Console.WriteLine($"{ComputingExecutions * 100 / TotalExecutions:F2}% of computing executions");
            Console.WriteLine($"{InterExecutions * 100 / TotalExecutions:F2}% of inter executions");
            Console.WriteLine($"{IoExecutions * 100 / TotalExecutions:F2}% of I/O executions");
        }
    }
}
