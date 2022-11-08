using SchedulingConception;

if (args.Length < 2)
{
    Console.WriteLine("Insufficient amount of input args.");
    return;
}

double deltaTime = 0;
uint interProcCount = 10, computingProcCount = 10, ioProcCount = 10;

var validArgs = double.TryParse(args[0], out var simulationTime) && double.TryParse(args[1], out deltaTime);

if (args.Length == 5)
{
    validArgs = validArgs 
                && uint.TryParse(args[2], out interProcCount)
                && uint.TryParse(args[3], out computingProcCount)
                && uint.TryParse(args[4], out ioProcCount);
}
else if (args.Length != 2 && args.Length != 5)
{
    Console.WriteLine("Invalid amount of input args.");
    return;
}

if (!validArgs)
{
    Console.WriteLine("Invalid input data.");
    return;
}

var scheduler = new Scheduler(new ProcessCountInfo(interProcCount, computingProcCount, ioProcCount));
scheduler.Run(simulationTime, deltaTime);
