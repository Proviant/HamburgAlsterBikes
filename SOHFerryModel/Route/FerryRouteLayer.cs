using System.Collections.Generic;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using SOHFerryModel.Station;

namespace SOHFerryModel.Route
{
    public class FerryRouteLayer : AbstractLayer
    {
        public readonly FerryStationLayer StationLayer;

        public FerryRouteLayer(FerryStationLayer stationLayer)
        {
            StationLayer = stationLayer;
        }

        public Dictionary<int, FerryRoute> FerryRoutes { get; private set; }

        public override bool InitLayer(
            LayerInitData layerInitData,
            RegisterAgent registerAgentHandle = null,
            UnregisterAgent unregisterAgent = null)
        {
            base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);
            FerryRoutes = FerryRouteReader.Read(layerInitData.LayerInitConfig.File, StationLayer);

            return true;
        }
    }
}