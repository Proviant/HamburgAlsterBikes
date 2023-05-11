using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mars.Interfaces.Environments;
using NetTopologySuite.GeometriesGraph;

namespace SOHTravellingBox.model.Trafic
{
    public class TrafficLightHandler
    {
        public IDictionary<ISpatialEdge, LightSignal> AllSignals { get; set; }
    }
}