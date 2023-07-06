using System;
using System.Collections.Generic;
using Mars.Common.Core.Random;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using SOHBicycleModel.Parking;
using SOHMultimodalModel.Model;

namespace SOHBicycleModel.Model
{
    public class BicycleLeader : Traveler<HumanTravelerLayer>
    {
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


        protected ISet<ModalChoice> _choices;

        [PropertyDescription]
        public IBicycleParkingLayer BicycleParkingLayer { get; set; }

        private List<Position> GoalPositions { get; set; }

        public override void Init(HumanTravelerLayer layer)
        {
            base.Init(layer);

            Gender = (GenderType)RandomHelper.Random.Next(0, 2);
            GoalPositions = new List<Position>();
            OvertakingActivated = false;

            _choices = new ModalityChooser().Evaluate(this);
            _choices.Add(ModalChoice.Walking);

            // TODO Hier Koordinaten von Zielort rausfinden.
            StartPosition = Position.CreateGeoPosition(9.99175, 53.55397);
            GoalPosition = Position.CreateGeoPosition(9.99187, 53.5539);

            const int radiusInM = 100;
            if (_choices.Contains(ModalChoice.CyclingOwnBike) && BicycleParkingLayer != null)
                Bicycle = BicycleParkingLayer.CreateOwnBicycleNear(StartPosition, radiusInM, UsesBikeAndRide);

        }
    }

    public class ModalityChooser
    {
        public ISet<ModalChoice> Evaluate(BicycleLeader attributes)
        {
            if (RandomHelper.Random.NextDouble() < attributes.HasBike)
                return new HashSet<ModalChoice> { ModalChoice.CyclingOwnBike };

            if (RandomHelper.Random.NextDouble() < attributes.PrefersBike)
                return new HashSet<ModalChoice> { ModalChoice.CyclingRentalBike };

            return new HashSet<ModalChoice> { ModalChoice.Walking };
        }
    }
}