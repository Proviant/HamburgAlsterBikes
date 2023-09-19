using System;
using System.Collections.Generic;
using Mars.Common.Core.Random;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using SOHBicycleModel.Parking;
using SOHBicycleModel.Rental;
using SOHDomain.TrafficLights;
using SOHMultimodalModel.Layers.TrafficLight;

namespace SOHMultimodalModel.Model
{
    /// <summary>
    ///     This <see cref="Traveler{HumanTravelerLayer}" /> entity uses a variety of modalities to reach its goal.
    /// </summary>
    public class HumanTraveler : Traveler<HumanTravelerLayer>
    {
        private ISet<ModalChoice> _choices;

        [PropertyDescription]
        public IBicycleParkingLayer BicycleParkingLayer { get; set; }

        public override void Init(HumanTravelerLayer layer)
        {
            base.Init(layer);

            Gender = (GenderType)RandomHelper.Random.Next(0, 2);
            OvertakingActivated = false;

            _choices = new ModalityChooser().Evaluate(this);
            _choices.Add(ModalChoice.Walking);

            const int radiusInM = 100;
            Bicycle = BicycleParkingLayer.CreateOwnBicycleNear(StartPosition, radiusInM, UsesBikeAndRide);
            Car = CarParkingLayer.CreateOwnCarNear(StartPosition, radiusInM);
        }

        protected override IEnumerable<ModalChoice> ModalChoices()
        {
            return _choices;
        }

        public override void Tick()
        {
            if (IsWaitingAtTrafficLight())
            {
                BrakingActivated = true;

                if (MultimodalRoute != null)
                {
                    switch (MultimodalRoute.CurrentModalChoice)
                    {
                        case ModalChoice.Walking:
                            BrakingActivated = true;
                            break;
                        case ModalChoice.CarDriving:
                        case ModalChoice.CarRentalDriving:
                            if (Car.Driver == null) break;
                            Car.Driver.BrakingActivated = true;
                            break;
                        case ModalChoice.CyclingOwnBike:
                            if (Bicycle.Driver == null) break;
                            Bicycle.Driver.BrakingActivated = true;
                            break;
                        case ModalChoice.CyclingRentalBike:
                            if (RentalBicycle.Driver == null) break;
                            RentalBicycle.Driver.BrakingActivated = true;
                            break;
                        case ModalChoice.Ferry:
                        case ModalChoice.Train:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (GoalReached)
            {
                // Checks, if the goal was actually reached with the current position
                // Console.WriteLine(MultimodalRoute.Goal.DistanceInMTo(Position) < 10);
            }

            base.Tick();
        }

        #region input

        [PropertyDescription(Name = "hasBike")]
        public double HasBike { get; set; }

        [PropertyDescription(Name = "hasCar")] public double HasCar { get; set; }

        [PropertyDescription(Name = "prefersBike")]
        public double PrefersBike { get; set; }

        [PropertyDescription(Name = "prefersCar")]
        public double PrefersCar { get; set; }

        [PropertyDescription(Name = "usesBikeAndRide")]
        public double UsesBikeAndRide { get; set; }

        [PropertyDescription(Name = "usesOwnBikeOutside")]
        public double UsesOwnBikeOutside { get; set; }

        [PropertyDescription(Name = "usesOwnCar")]
        public double UsesOwnCar { get; set; }

        #endregion
    }

    public class ModalityChooser
    {
        public ISet<ModalChoice> Evaluate(HumanTraveler attributes)
        {
            if (RandomHelper.Random.NextDouble() < attributes.HasCar)
                return new HashSet<ModalChoice> { ModalChoice.CarDriving };

            if (RandomHelper.Random.NextDouble() < attributes.HasBike)
                return new HashSet<ModalChoice> { ModalChoice.CyclingOwnBike };

            if (RandomHelper.Random.NextDouble() < attributes.PrefersCar)
                return new HashSet<ModalChoice> { ModalChoice.CarRentalDriving };

            if (RandomHelper.Random.NextDouble() < attributes.PrefersBike)
                return new HashSet<ModalChoice> { ModalChoice.CyclingRentalBike };

            return new HashSet<ModalChoice> { ModalChoice.Walking };
        }
    }
}