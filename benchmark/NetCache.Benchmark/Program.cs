using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace NetCache.Benchmark
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                .Run(args, ManualConfig
                    .Create(DefaultConfig.Instance)
#if NET46
                    .With(MemoryDiagnoser.Default)
                    .With(Job.InProcess)
                    .With(ExecutionValidator.FailOnError));
#else
                    .AddDiagnoser(MemoryDiagnoser.Default)
                    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core21),
                        Job.Default.WithRuntime(CoreRuntime.Core31),
                        Job.Default.WithRuntime(CoreRuntime.Core50),
                        Job.Default.WithRuntime(ClrRuntime.Net471)
                    )
                    .AddValidator(ExecutionValidator.FailOnError));
#endif
        }
    }
}
