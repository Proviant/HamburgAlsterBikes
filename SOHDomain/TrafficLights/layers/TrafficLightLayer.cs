using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mars.Common.Collections.KNNGraph;
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
using SOHTravellingBox.model;

namespace SOHDomain.TrafficLights
{
    public class TrafficLightLayer : AbstractLayer, ISpatialGraphLayer
    {
        public ISpatialGraphEnvironment Environment { get; set; }

        /// <summary>
        ///     All traffic light entities of this layer sorted by GUID
        /// </summary>
        public IDictionary<Guid, TrafficLight> TrafficLightsByGUID { get; private set; }
        public IDictionary<ISpatialNode, TrafficLight> TrafficLightsByNode { get; private set; }

        public TrafficLightLayer()
        {
        }

        bool ILayer.InitLayer(
            LayerInitData layerInitData,
            RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgent)
        {
            base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);

            // Initialize the SpatialGraphEnvironment
            string start = Directory.GetCurrentDirectory();
            char sep = Path.DirectorySeparatorChar;
            Environment = new SpatialGraphEnvironment($"{start}{sep}resources{sep}trafficlightslayer.geojson");

            // Retrieve AgentManager
            IAgentManager agentManager = layerInitData.Container.Resolve<IAgentManager>();
            // Spawn in all TrafficLights onto the trafficlight-layer as a list
            List<TrafficLight> lights = agentManager.Spawn<TrafficLight, TrafficLightLayer>().ToList();

            // Iterate over all elements in the config and add them to the dictionary.
            TrafficLightsByGUID = new Dictionary<Guid, TrafficLight>();
            TrafficLightsByNode = new Dictionary<ISpatialNode, TrafficLight>();
            foreach (TrafficLight light in lights)
            {
                TrafficLightsByGUID.Add(Guid.NewGuid(), light);

                Position trafficLightPos = Position.CreateGeoPosition(light.Longitude, light.Latidute);
                ISpatialNode nearestNode = Environment.NearestNode(trafficLightPos);

                // If there is no nearest node found, skip it since this traffic light might then be out of the simulation area
                if (nearestNode == null)
                {
                    continue;
                }

                TrafficLightsByNode.Add(nearestNode, light);
            }

            // Display if all 1260 traffic lights have been added
            // Console.WriteLine("Anzahl Ampeln nach GUIDs:  " + TrafficLightsByGUID.Count);
            // Console.WriteLine("Anzahl Ampeln nach Knoten: " + TrafficLightsByNode.Count);

            return true;
        }

        public void Tick()
        {
            // Advance each traffic light by one second
            foreach (TrafficLight light in TrafficLightsByGUID.Values)
            {
                light.Tick();
            }
        }
    }
}