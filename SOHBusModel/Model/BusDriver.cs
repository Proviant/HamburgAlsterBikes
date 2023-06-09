using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Components.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using SOHBusModel.Route;
using SOHBusModel.Steering;
using SOHDomain.Steering.Capables;
using SOHDomain.Steering.Common;

namespace SOHBusModel.Model
{
    public class BusDriver : AbstractAgent, IBusSteeringCapable
    {
        private static int _stableId;

        private long _startTickForCurrentStation;

        private BusDriver(UnregisterAgent unregister, BusLayer layer)
        {
            ID = Guid.NewGuid();
            _unregister = unregister;
            Layer = layer;
        }

        public BusDriver(BusLayer layer, UnregisterAgent unregister) : this(unregister, layer)
        {
            InitializeBus();
        }

        public BusDriver(BusLayer layer, UnregisterAgent unregister, string busType) : this(unregister, layer)
        {
            InitializeBus(busType);
        }

        public bool Boarding => AmountOfTicksAtCurrentStation <= MinimumBoardingTimeInSeconds || !DepartureTickArrived;
        public bool DepartureTickArrived => _departureTick <= Layer.Context.CurrentTick;

        private long AmountOfTicksAtCurrentStation => Layer.Context.CurrentTick - _startTickForCurrentStation;

        public BusRoute.BusRouteEnumerator BusRouteEnumerator =>
            (BusRoute.BusRouteEnumerator)(_busRouteEnumerator ??= BusRoute.GetEnumerator());

        public IEnumerable<BusRouteEntry> RemainingStations => BusRoute.Skip(BusRouteEnumerator.CurrentIndex);

        public BusRouteEntry CurrentBusRouteEntry => BusRouteEnumerator.Current;

        public int StationStops => BusRoute.Entries.IndexOf(BusRouteEnumerator.Current);

        [PropertyDescription(Name = "line")]
        public string Line { get; set; }

        [PropertyDescription(Name = "waitingInSeconds")]
        public int MinimumBoardingTimeInSeconds { get; set; }

        [PropertyDescription]
        public BusLayer Layer { get; }

        private Mars.Interfaces.Environments.Route Route
        {
            get => _steeringHandle.Route;
            set => _steeringHandle.Route = value;
        }

        public bool GoalReached => _steeringHandle?.GoalReached ?? false;
        public Bus Bus { get; set; }
        public int StableId { get; } = _stableId++;

        public BusRoute BusRoute { get; set; }

        public Position Position
        {
            get => Bus.Position;
            set => Bus.Position = value;
        }

        public void Notify(PassengerMessage passengerMessage)
        {
            //do nothing, I am the driver
        }

        public bool OvertakingActivated => false;
        public bool BrakingActivated => false;

        private void InitializeBus(string type = "HHA-Typ-DT5")
        {
            Bus = Layer.EntityManager.Create<Bus>("type", type);
            Bus.Layer = Layer;
            Bus.TryEnterDriver(this, out _steeringHandle);
        }

        public override void Tick()
        {
            if (BusRoute == null)
            {
                FindTrainRouteAndStartCommuting();
                _departureTick = Layer.Context.CurrentTick;
            }

            if (!Boarding)
            {
                Bus.BusStation?.Leave(Bus);

                _steeringHandle.Move();

                if (GoalReached)
                {
                    Environment.Remove(Bus);
                    BusRouteEnumerator.Current?.To.Enter(Bus);

                    var currentMinutes = BusRouteEnumerator.Current?.Minutes ?? 0;
                    _departureTick += currentMinutes * 60;

                    var notAtTerminalStation = FindNextRouteSection();
                    if (notAtTerminalStation)
                    {
                        Bus.NotifyPassengers(PassengerMessage.GoalReached);
                    }
                    else
                    {
                        Bus.NotifyPassengers(PassengerMessage.TerminalStation);
                        _unregister(Layer, this);
                    }
                }
            }
        }


        private void FindTrainRouteAndStartCommuting()
        {
            if (Layer.BusRouteLayer.TryGetRoute(Line, out var schedule))
                BusRoute = schedule;
            else
                throw new ArgumentException($"No train route provided by {nameof(BusRouteLayer)}");

            if (BusRoute.Count() < 2)
                throw new ArgumentException("Train route requires at least two stops");

            FindNextRouteSection();

            var trainStation = BusRouteEnumerator.Current?.From;
            if (!trainStation?.Enter(Bus) ?? true)
                throw new ArgumentException("Train could not dock the first station");
        }

        private bool FindNextRouteSection()
        {
            if (!BusRouteEnumerator.MoveNext()) return false;

            var source = Environment.NearestNode(BusRouteEnumerator.Current?.From.Position, SpatialModalityType.TrainDriving);
            var target = Environment.NearestNode(BusRouteEnumerator.Current?.To.Position, SpatialModalityType.TrainDriving);

            Route = Environment.FindShortestRoute(source, target, edge => edge.Modalities.Contains(SpatialModalityType.TrainDriving));

            if (Route == null || Route.Count == 0)
                throw new ApplicationException(
                    $"{nameof(BusDriver)} cannot find route from '{source.Position}' " +
                    $"to '{target.Position}' but: {Environment.Nodes.Count}");

            _startTickForCurrentStation = Layer.Context.CurrentTick;
            Environment.Insert(Bus, Route.First().Edge.From);
            return true;
        }


        #region fields

        private BusSteeringHandle _steeringHandle;
        private readonly UnregisterAgent _unregister;
        private long _departureTick;
        private ISpatialGraphEnvironment Environment => Layer.GraphEnvironment;

        bool ISteeringCapable.BrakingActivated
        {
            get { return Bus.Driver.BrakingActivated; }
            set { Bus.Driver.BrakingActivated = value; }
        }

        private IEnumerator<BusRouteEntry> _busRouteEnumerator;

        #endregion
    }
}