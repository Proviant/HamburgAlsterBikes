using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Annotations;
using NetTopologySuite.GeometriesGraph;

namespace SOHTravellingBox.model.Trafic
{
    public class TrafficLightHandler
    {
        public static IDictionary<ISpatialEdge, TrafficLight> AllSignals { get; set; }
    }
}