using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mars.Common.Collections.KNNGraph;
using Mars.Common.Core.Collections;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using SOHDomain.Graph;
using SOHDomain.TrafficLights.layers;
using SOHTravellingBox.model;

namespace SOHDomain.TrafficLights
{
    public class TrafficLightLayer : VectorLayer, ITrafficLightLayer, ISpatialGraphLayer
    {
        public ISpatialGraphEnvironment Environment { get; set; }

        /// <summary>
        ///     All traffic light entities of this layer sorted by GUID
        /// </summary>
        public IDictionary<Guid, IAgent> TrafficLightsByGUID { get; private set; }

        public IDictionary<Position, TrafficLight> TrafficLightsByPos { get; private set; }

        public TrafficLightLayer(ISpatialGraphEnvironment environment = null)
        {
            Environment = environment;
        }

        public override bool InitLayer(
            LayerInitData layerInitData,
            RegisterAgent registerAgentHandle = null,
            UnregisterAgent unregisterAgent = null)
        {
            base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);

            if (LayerInitConfig.Value is ISpatialGraphEnvironment input)
                Environment = input;
            else if (!string.IsNullOrEmpty(LayerInitConfig.File))
                Environment = new SpatialGraphEnvironment(layerInitData.LayerInitConfig.File);

            // Iterate over all elements in the config and add them to the dictionary.
            TrafficLightsByGUID = new Dictionary<Guid, IAgent>();
            foreach (var config in layerInitData.AgentInitConfigs)
            {
                TrafficLightsByGUID.AddRange(AgentManager.SpawnAgents(config,
                    registerAgentHandle, unregisterAgent, new List<ILayer> { this },
                    new List<IEnvironment> { Environment }));
            }

            Console.WriteLine("Anzahl Ampeln: " + TrafficLightsByGUID.Count);

            return true;
        }

        ///
        /// <summary>
        ///    Retrieves the nearest traffic light at the given position.
        /// </summary>
        public TrafficLight GetNearestTrafficLight(Position position)
        {


            return null;
        }
    }
}