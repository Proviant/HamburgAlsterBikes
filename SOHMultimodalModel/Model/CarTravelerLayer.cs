using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SOHMultimodalModel.Multimodal;
using SOHMultimodalModel.Routing;

namespace SOHMultimodalModel.Model
{
    /// <summary>
    ///     This layer implements the <see cref="IMultimodalLayer" /> to provide walking and car driving mutli-modal
    ///     routing capabilities.
    /// </summary>
    public class CarTravelerLayer : AbstractMultimodalLayer
    {
        public int RentalCount { get; set; }

        /// <summary>
        ///     Provides the possibility to enter or leave the graph via gateway points.
        /// </summary>
        public GatewayLayer GatewayLayer { get; set; }

    }
}