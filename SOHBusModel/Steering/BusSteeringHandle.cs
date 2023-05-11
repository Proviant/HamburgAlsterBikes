using System;
using System.Linq;
using Mars.Interfaces.Environments;
using SOHBusModel.Model;
using SOHDomain.Model;
using SOHDomain.Steering.Capables;
using SOHDomain.Steering.Handles;

namespace SOHBusModel.Steering
{
    public class BusSteeringHandle : VehicleSteeringHandle<IBusSteeringCapable, IPassengerCapable,
        BusSteeringHandle, BusPassengerHandle>
    {
        private readonly IBusSteeringCapable _driver;

        //TODO use car driving handle
        
        public BusSteeringHandle(ISpatialGraphEnvironment environment, IBusSteeringCapable driver,
            Vehicle<IBusSteeringCapable, IPassengerCapable, BusSteeringHandle,
                BusPassengerHandle> vehicle, double standardSpeedLimit = 6.1722222)
            : base(environment, vehicle, standardSpeedLimit)
        {
            _driver = driver;
        }

        protected override double HandleIntersectionAhead(SpatialGraphExploreResult exploreResult,
            double biggestDeceleration)
        {
            var exploreResults = exploreResult.EdgeExplores;

            if (exploreResults.Count == 1)
            {
                var edgeExploreResult = exploreResults.First();
                var distanceToStation = edgeExploreResult.IntersectionDistance;

                if (distanceToStation < UrbanSafetyDistanceInM)
                {
                    var speedChange = CalculateSpeedChange(Vehicle.Velocity, SpeedLimit,
                        distanceToStation, 0, 0);
                    return Math.Min(speedChange, biggestDeceleration);
                }
            }

            return biggestDeceleration;
        }

        protected override double CalculateDrivingDistance(double biggestDeceleration)
        {
            if (Route.RemainingRouteDistanceToGoal < 3) // Make last step to goal
                return Route.RemainingRouteDistanceToGoal;
            if (biggestDeceleration < MaximalDeceleration) // Was speed change set? then use it
                return Vehicle.Velocity + biggestDeceleration;

            if (_driver is BusDriver trainDriver)
            {
                var tripTime = trainDriver.CurrentBusRouteEntry.Minutes * 60d -
                               trainDriver.MinimumBoardingTimeInSeconds * 2;
                var distance = Route.RouteLength / tripTime;

                if (distance > Route.RemainingRouteDistanceToGoal) // Make last step to goal
                    return Route.RemainingRouteDistanceToGoal;
                return distance;
            }

            return base.CalculateDrivingDistance(biggestDeceleration);
        }
    }
}