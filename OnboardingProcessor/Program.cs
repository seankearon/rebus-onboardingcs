﻿using Serilog;
using Topper;

namespace OnboardingProcessor
{
    static class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();

            var configuration = new ServiceConfiguration()
               .Add("OurBackendBus", () => new Backend());

            ServiceHost.Run(configuration);
        }
    }
}