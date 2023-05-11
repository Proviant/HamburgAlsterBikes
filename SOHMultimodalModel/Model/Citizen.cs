using System;
using Mars.Common.Core.Random;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using SOHDomain.Common;
using SOHMultimodalModel.Commons;
using SOHMultimodalModel.Layers;
using SOHMultimodalModel.Planning;

namespace SOHMultimodalModel.Model
{
    /// <summary>
    ///     The <code>Citizen</code> is proceeding his/her dayplan by moving to different POIs within a day.
    /// </summary>
    public class Citizen : MultiCapableAgent<CitizenLayer>
    {
        #region constructor

        public override void Init(CitizenLayer layer)
        {
            _mediatorLayer = layer.MediatorLayer;

            Position homePosition;
            if (StartPosition == null)
            {
                homePosition = _mediatorLayer.VectorBuildingsLayer.RandomPosition();
                StartPosition = homePosition;
            }
            else
            {
                homePosition = _mediatorLayer.GetNextPoiOfType(StartPosition,
                    OsmFeatureCodes.Buildings);
            }

            base.Init(layer);

            Home = new PointOfInterest(TripReason.HomeTime, homePosition);

            // WalkingSteeringHandle.Position = StartPosition ?? homePosition;

            if (Worker)
            {
                // var workPosition = _mediatorLayer.GetPoiWithOneOutOfManyTypes(Position, new List<int>
                //     {OsmFeatureCodes.Industrial, OsmFeatureCodes.Commercial}, false);
                var workPosition = _mediatorLayer.VectorLandUseLayer.RandomPosition();
                Work = new PointOfInterest(TripReason.Work, workPosition);
            }

            Tour = new Tour(layer.Context, Worker, PartTimeWorker);

            EnvironmentLayer = layer.SpatialGraphMediatorLayer;
        }

        #endregion

        #region fields and constants

        private const double FreeTimeAtHomeProbability = 0.5;
        private const double EatAtHomeProbability = 0.8;
        private const double EatAtHomeDistance = 0.5;

        private MediatorLayer _mediatorLayer;

        private bool _partTimeWorker;

        #endregion

        #region properties

        /// <summary>
        ///     Gets or sets the flag, indicating that his citizen is a full worker
        /// </summary>
        [PropertyDescription(Ignore = true, Name = "worker")]
        public bool Worker { get; set; }

        /// <summary>
        ///     Gets or sets a flag indicating that this citizen is a part-time worker and thus a <see cref="Worker" /> as well.
        /// </summary>
        [PropertyDescription(Ignore = true, Name = "partTimeWorker")]
        public bool PartTimeWorker
        {
            get => _partTimeWorker;
            set
            {
                if (value)
                    Worker = _partTimeWorker = true;
                else
                    _partTimeWorker = false;
            }
        }

        /// <summary>
        ///     Describes how much percent of the population has a car to their personal disposal
        /// </summary>
        [PropertyDescription(Ignore = true)]
        public double CapabilityDrivingWithProbability
        {
            set => CapabilityDrivingOwnCar = RandomHelper.SmallerThan(value);
        }

        /// <summary>
        ///     Gets the associated day plan for this agent.
        /// </summary>
        public Tour Tour { get; set; }

        /// <summary>
        ///     Gets or sets the central <c>Home</c> POI of this agent.
        /// </summary>
        [PropertyDescription(Ignore = true)]
        public PointOfInterest Home { get; private set; }

        /// <summary>
        ///     Gets or sets the main <c>Work</c> POI of this agent.
        /// </summary>
        [PropertyDescription(Ignore = true)]
        public PointOfInterest Work { get; private set; }

        public double Height { get; set; }
        public double Width { get; set; }

        #endregion

        #region citizen methods

        public override void Tick()
        {
            if (Tour.MoveNext())
            {
                var goalPosition = FindPositionForTrip(Tour.Current);
                MultimodalRoute = MultimodalLayer.Search(this, Position, goalPosition, Capabilities);
            }

            base.Move();
        }

        public void ChangeWork(Position position)
        {
            Work = new PointOfInterest(TripReason.Work, position);
        }

        public void ChangeHome(Position position)
        {
            Home = new PointOfInterest(TripReason.HomeTime, position);
        }

        private Position FindPositionForTrip(Trip trip)
        {
            return trip.TripReason switch
            {
                TripReason.Work => Work.Position,
                TripReason.HomeTime => Home.Position,
                TripReason.Eat => RandomHelper.SmallerThan(EatAtHomeProbability) &&
                                  Position.DistanceInKmTo(Home.Position) < EatAtHomeDistance
                    ? Home.Position
                    : _mediatorLayer.FindNextNearestLocationForAnyTarget(Position, OsmGroups.Eat),
                TripReason.FreeTime => RandomHelper.SmallerThan(FreeTimeAtHomeProbability)
                    ? Home.Position
                    : _mediatorLayer.FindNextNearestLocationForAnyTarget(Position, OsmGroups.FreeTime),
                TripReason.Errands => _mediatorLayer.FindNextNearestLocationForAnyTarget(Position, OsmGroups.Errand),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #endregion
    }
}