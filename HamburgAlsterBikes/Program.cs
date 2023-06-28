using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Mars.Common.Core.Logging;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Components.Starter;
using Mars.Core.Simulation;
using Mars.Interfaces;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using SOHBicycleModel.Model;
using SOHBicycleModel.Rental;
using SOHCarModel.Model;
using SOHDomain.Graph;
using SOHDomain.Model;
using SOHDomain.TrafficLights;
using SOHMultimodalModel.Model;
using SOHTravellingBox.model;

namespace SOHTravellingBox
{
    /// <summary>
    ///     This pre-defined starter program runs the travelling scenario with  outside passed arguments or
    ///     a default simulation inputConfiguration as CSV output and trips.
    /// </summary>
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("EN-US");
            LoggerFactory.SetLogLevel(LogLevel.Warning);

            var description = new ModelDescription();

            // All environments where the agent move on resolve routes.
            // description.AddLayer<SidewalkLayer>();
            description.AddLayer<StreetLayer>(new[] { typeof(ISpatialGraphLayer) });
            description.AddLayer<SpatialGraphMediatorLayer>(new[] { typeof(ISpatialGraphLayer) });

            // All data layers and interacting entities.
            //description.AddLayer<BicycleParkingLayer>(new []{typeof(IBicycleParkingLayer)});
            //description.AddLayer<BicycleRentalLayer>(new []{typeof(IBicycleRentalLayer)});
            //description.AddLayer<CarParkingLayer>(new []{typeof(ICarParkingLayer)});
            //description.AddLayer<CarRentalLayer>(new []{typeof(ICarRentalLayer)});

            // All agent layers
            description.AddLayer<HumanTravelerLayer>();
            description.AddLayer<CarLayer>();
            description.AddLayer<TrafficLightLayer>("TrafficLightLayer");
            description.AddLayer<TrafficSchedulingLayer>("TrafficSchedulingLayer");

            description.AddAgent<HumanTraveler, TrafficSchedulingLayer>();
            description.AddAgent<TrafficLight, TrafficLightLayer>();
            description.AddEntity<Bicycle>();
            description.AddEntity<RentalBicycle>();
            description.AddEntity<Car>();
            description.AddEntity<RentalCar>();

            for (int i = 1; i <= 1; i++)
            {
                ISimulationContainer application;
                if (args != null && args.Any())
                {
                    var container = CommandParser.ParseAndEvaluateArguments(description, args);

                    var config = container.SimulationConfig;
                    config.SimulationRunIteration = i;

                    if (i == 0)
                    {
                        config.Globals.PostgresSqlOptions.OverrideByConflict = false;
                    }
                    application = SimulationStarter.BuildApplication(description, config);
                }
                else
                {
                    var file = File.ReadAllText("config.json");
                    var simConfig = SimulationConfig.Deserialize(file);
                    application = SimulationStarter.BuildApplication(description, simConfig);
                }

                var simulation = application.Resolve<ISimulation>();

                var watch = Stopwatch.StartNew();
                var state = simulation.StartSimulation();

                watch.Stop();

                Console.WriteLine($"Executed iterations {state.Iterations} lasted {watch.Elapsed}");
                application.Dispose();
            }
        }
    }
}