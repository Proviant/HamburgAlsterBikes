using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using Mars.Common;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using SOHMultimodalModel.Model;

namespace SOHDomain.Model
{
    public class TrafficSchedulingLayer : AgentSchedulerLayer<HumanTraveler, HumanTravelerLayer>
    {

        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgent = null)
        {
            bool val = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);
            return val;
        }

        protected override void Schedule(SchedulerEntry dataRow)
        {
            Console.WriteLine("Kam rein!!!");
        }
    }
}