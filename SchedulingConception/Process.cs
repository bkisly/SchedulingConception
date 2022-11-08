namespace SchedulingConception
{
    internal class Process
    {
        public int Pid { get; }
        public string Name { get; }
        public ProcessPriority Priority { get; }

        public delegate void ProcessExecutedEventHandler(Process process);
        public event ProcessExecutedEventHandler? ProcessExecuted;

        public Process(int pid, string name, ProcessPriority priority)
        {
            Pid = pid;
            Name = name;
            Priority = priority;
        }

        public void Execute()
        {
            Console.WriteLine($"{Name} is executed! (PID: {Pid})");
            ProcessExecuted?.Invoke(this);
        }
    }

    internal enum ProcessPriority
    {
        Io = 1,
        Computing = 2,
        Inter = 4
    }

    internal record struct ProcessCountInfo(uint Inter, uint Computing, uint Io);
}
