using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mars.Interfaces.Environments;
using SOHCarModel.Model;

namespace SOHMultimodalModel.Model
{
    /// <summary>
    ///     This <see cref="Traveler{TLayer}" /> entity uses the <c>car</c> and <c>walking</c> modality to reach their
    ///     goal.
    /// </summary>
    public class CarTraveler : Traveler<CarTravelerLayer>
    {

        private CarTravelerLayer _carTravelerLayer;

        /// <summary>
        ///     Indicates if this agent possesses a car.
        /// </summary>
        public bool HasCar { get; set; }

        public override void Init(CarTravelerLayer layer)
        {
            base.Init(layer);

            _carTravelerLayer = layer;
            EnvironmentLayer = layer.SpatialGraphMediatorLayer;
            CarRentalLayer = layer.CarRentalLayer;
            Gender = (GenderType)new Random().Next(2);
            OvertakingActivated = true;
            EnableCapability(ModalChoice.CarRentalDriving);
            EnableCapability(ModalChoice.CarDriving);

            if (HasCar)
            {
                Car = _carTravelerLayer.EntityManager.Create<RentalCar>("type", "city");
            }
        }

        protected override MultimodalRoute FindMultimodalRoute()
        {
            if (_carTravelerLayer.GatewayLayer != null)
            {
                var (start, goal) = _carTravelerLayer.GatewayLayer.Validate(Position, GoalPosition);
                return SearchMultimodalRoute(start, goal);
            }

            return SearchMultimodalRoute(Position, GoalPosition);
        }

        private MultimodalRoute SearchMultimodalRoute(Position start, Position goal)
        {
            if (HasCar)
            {
                var street = EnvironmentLayer.Environment;
                var route = street.FindShortestRoute(street.NearestNode(start), street.NearestNode(goal),
                    edge => edge.Modalities.Contains(SpatialModalityType.CarDriving));
                return new MultimodalRoute(route, ModalChoice.CarDriving);
            }

            return MultimodalLayer.Search(this, start, goal, ModalChoice.CarRentalDriving);
        }

        protected override bool EnterModalType(ModalChoice modalChoice, Route route)
        {
            var success = base.EnterModalType(modalChoice, route);
            if (success && modalChoice == ModalChoice.CarRentalDriving) _carTravelerLayer.RentalCount++;
            return success;
        }
    }
}