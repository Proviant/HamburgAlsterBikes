using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common.Core.Collections;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Model;
using SOHDomain.Graph;
using SOHDomain.TrafficLights.layers;
using SOHTravellingBox.model;

namespace SOHDomain.TrafficLights
{
    public class TrafficLightLayer : AbstractLayer, ISpatialGraphLayer, ITrafficLightLayer
    {
        [PropertyDescription]
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

        bool ILayer.InitLayer(
            LayerInitData layerInitData,
            RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgent)
        {
            base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);
            Environment = new SpatialGraphEnvironment(layerInitData.LayerInitConfig.File);

            // Iterate over all elements in the config and add them to the dictionary.
            TrafficLightsByGUID = new Dictionary<Guid, IAgent>();
            foreach (AgentMapping config in layerInitData.AgentInitConfigs)
            {
                Console.WriteLine(config.File.ToString());

                IDictionary<Guid, TrafficLight> spawnedAgents = AgentManager.SpawnAgents<TrafficLight>(config,
                    registerAgentHandle, unregisterAgent, new List<ILayer> { this },
                    new List<IEnvironment> { Environment });

                // Each agent has to be added individually, since there is no AddRange implemented in the IDictionary
                foreach (KeyValuePair<Guid, TrafficLight> pair in spawnedAgents)
                {
                    TrafficLightsByGUID.Add(pair.Key, pair.Value);
                }
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

        public void Tick()
        {
            // Advance each traffic light by one second
            foreach (TrafficLight light in TrafficLightsByGUID.Values)
            {
                light.Tick();
            }
        }

        public void PreTick()
        {
            // Nothing
        }

        public void PostTick()
        {
            // Nothing
        }
    }
}