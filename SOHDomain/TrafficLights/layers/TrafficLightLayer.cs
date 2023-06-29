using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mars.Common.Core.Collections;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Core.Data;
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
        public IDictionary<Guid, TrafficLight> TrafficLightsByGUID { get; private set; }
        public IDictionary<Position, TrafficLight> TrafficLightsByPos { get; private set; }

        public TrafficLightLayer()
        {
        }

        bool ILayer.InitLayer(
            LayerInitData layerInitData,
            RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgent)
        {
            base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);
            Environment = new SpatialGraphEnvironment(layerInitData.LayerInitConfig.File);

            // Retrieve AgentManager
            IAgentManager agentManager = layerInitData.Container.Resolve<IAgentManager>();
            // Spawn in all TrafficLights onto the trafficlight-layer as a list
            List<TrafficLight> lights = agentManager.Spawn<TrafficLight, TrafficLightLayer>().ToList();

            // Iterate over all elements in the config and add them to the dictionary.
            TrafficLightsByGUID = new Dictionary<Guid, TrafficLight>();
            TrafficLightsByPos = new Dictionary<Position, TrafficLight>();
            foreach (TrafficLight light in lights)
            {
                TrafficLightsByGUID.Add(Guid.NewGuid(), light);
                TrafficLightsByPos.Add(new Position(light.Longitude, light.Latidute), light);
            }
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