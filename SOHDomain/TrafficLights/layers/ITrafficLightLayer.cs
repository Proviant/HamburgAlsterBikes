using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using SOHDomain.Model;

namespace SOHDomain.TrafficLights.layers
{
    public interface ITrafficLightLayer : ILayer
    {
        /// <summary>
        ///     Holds the environment that can be used for traffic lights to advance and find.
        /// </summary>
        ISpatialGraphEnvironment Environment { get; }

    }
}