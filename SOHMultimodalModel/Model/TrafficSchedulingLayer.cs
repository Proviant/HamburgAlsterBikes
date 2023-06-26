using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mars.Common;
using Mars.Components.Layers;
using NetTopologySuite.Geometries;
using SOHMultimodalModel.Model;

namespace SOHDomain.Model
{
    public class TrafficSchedulingLayer : SchedulerLayer
    {
        CycleTravelerLayer cycleLayer { get; set; }
        CarTravelerLayer carLayer { get; set; }

        public TrafficSchedulingLayer()
        {
            Console.WriteLine("Created Traffic Scheduling Layer!");
        }

        protected override void Schedule(SchedulerEntry dataRow)
        {
            var source = dataRow.SourceGeometry.RandomPositionFromGeometry();
            var target = dataRow.TargetGeometry.RandomPositionFromGeometry();

            CycleTraveler bicycleTraveler = new CycleTraveler();
            CarTraveler carTraveler = new CarTraveler();

            bicycleTraveler.Init(cycleLayer);
            carTraveler.Init(carLayer);

            RegisterAgent(cycleLayer, bicycleTraveler);
            RegisterAgent(carLayer, carTraveler);
        }
    }
}