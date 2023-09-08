using System;
using System.Collections.Generic;
using System.Windows;
using Mars.Components.Layers;
using SOHBicycleModel.Model;
using SOHMultimodalModel.Multimodal;

namespace SOHMultimodalModel.Model
{
    /// <summary>
    ///     This layer implements the <see cref="IMultimodalLayer" /> to provide a variety of multi-modal routing capabilities.
    /// </summary>
    public class BicycleLeaderLayer : AgentSchedulerLayer<BicycleLeader, HumanTravelerLayer>
    {
    }
}