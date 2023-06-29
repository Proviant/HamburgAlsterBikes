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
using Mars.Interfaces.Annotations;

namespace SOHDomain.Model
{
    public class TrafficSchedulingLayer : AgentSchedulerLayer<HumanTraveler, HumanTravelerLayer>
    {
    }
}