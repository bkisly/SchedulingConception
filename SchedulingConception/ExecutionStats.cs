namespace SchedulingConception
{
    internal class ExecutionStats
    {
        private readonly uint _compProcCount, _interProcCount, _ioProcCount;

        public uint ComputingExecutions { get; set; }
        public uint InterExecutions { get; set; }
        public uint IoExecutions { get; set; }

        public double ComputingTime { get; set; }
        public double InterTime { get; set; }
        public double IoTime { get; set; }      

        public uint TotalExecutions => ComputingExecutions + InterExecutions + IoExecutions;
        public double TotalTime => ComputingTime + InterTime + IoTime;

        public ExecutionStats(ProcessCountInfo processCountInfo)
        {
            processCountInfo.Deconstruct(out _interProcCount, out _compProcCount, out _ioProcCount);
        }

        public void PrintReport()
        {
            Console.WriteLine("\n============= EXECUTION STATS =============\n");
            Console.WriteLine("~ Input data ~");
            Console.WriteLine($"{_interProcCount} interactive processes, {_compProcCount} computing processes, {_ioProcCount} I/O processes.\n");

            Console.WriteLine("~ Results ~");
            Console.WriteLine($"{InterExecutions} executions of interactive processes");
            Console.WriteLine($"{ComputingExecutions} executions of computing processes");
            Console.WriteLine($"{IoExecutions} executions of I/O processes");
            Console.WriteLine($"{TotalExecutions} total executions\n");

            Console.WriteLine($"{InterExecutions * 100 / TotalExecutions:F2}% of interactive executions");
            Console.WriteLine($"{ComputingExecutions * 100 / TotalExecutions:F2}% of computing executions");
            Console.WriteLine($"{IoExecutions * 100 / TotalExecutions:F2}% of I/O executions\n");

            Console.WriteLine($"{InterTime} s spent on interactive processes");
            Console.WriteLine($"{ComputingTime} s spent on computing processes");
            Console.WriteLine($"{IoTime} s spent on I/O processes");
            Console.WriteLine($"{TotalTime} s in total");
        }
    }
}
