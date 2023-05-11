using System;
using Mars.Interfaces.Data;
using SOHTrainModel.Model;
using SOHTrainModel.Route;
using SOHTrainModel.Station;

namespace SOHTests.Commons.Layer
{
    public class TrainRouteLayerFixture : IDisposable
    {
        public TrainRouteLayerFixture()
        {
            TrainStationLayer = new TrainStationLayerFixture().TrainStationLayer;
            var trainRouteLayer = new TrainRouteLayer(TrainStationLayer);
            trainRouteLayer.InitLayer(
                new LayerInitData
                {
                    LayerInitConfig = {File = ResourcesConstants.TrainU1LineCsv}
                }, (_, _) => { }, (_, _) => { });
            TrainRouteLayer = trainRouteLayer;
        }

        public TrainStationLayer TrainStationLayer { get; private set; }

        public ITrainRouteLayer TrainRouteLayer { get; private set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            TrainStationLayer.Dispose();
            TrainStationLayer = null;
            TrainRouteLayer = null;
        }

        private class TrainStationLayerFixture : IDisposable
        {
            /// <summary>
            ///     Fixture for the <see cref="TrainStationLayerFixture" /> with given input file.
            /// </summary>
            /// <param name="filePath">Defines the import data. If <code>null</code>, then use Hamburg ferry stations.</param>
            public TrainStationLayerFixture(string filePath = null)
            {
                TrainStationLayer = new TrainStationLayer();
                TrainStationLayer.InitLayer(
                    new LayerInitData
                    {
                        LayerInitConfig = {File = filePath ?? ResourcesConstants.TrainStationsU1}
                    });
            }

            public TrainStationLayer TrainStationLayer { get; }

            public void Dispose()
            {
                TrainStationLayer.Dispose();
            }
        }
    }
}