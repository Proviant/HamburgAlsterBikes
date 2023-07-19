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

        [PropertyDescription(Name = "hasCar")]
        public double HasCar
        {
            get { return 0.0; }
            set { }
        }

        [PropertyDescription(Name = "prefersBike")]
        public double PrefersBike { get; set; }

        [PropertyDescription(Name = "prefersCar")]
        public double PrefersCar
        {
            get { return 0.0; }
            set { }
        }

        [PropertyDescription(Name = "usesBikeAndRide")]
        public double UsesBikeAndRide { get; set; }

        [PropertyDescription(Name = "usesOwnBikeOutside")]
        public double UsesOwnBikeOutside { get; set; }

        [PropertyDescription(Name = "usesOwnCar")]
        public double UsesOwnCar
        {
            get { return 0.0; }
            set { }
        }
        #endregion


        protected ISet<ModalChoice> _choices;
        private Queue<Position> stops { get; set; }

        [PropertyDescription]
        public IBicycleParkingLayer BicycleParkingLayer { get; set; }

        private List<Position> GoalPositions { get; set; }

        public override void Init(HumanTravelerLayer layer)
        {
            base.Init(layer);
            Console.WriteLine("Initialisiert zumindest.");

            Gender = (GenderType)RandomHelper.Random.Next(0, 2);
            GoalPositions = new List<Position>();
            OvertakingActivated = false;

            _choices = new ModalityChooser().Evaluate(this);
            _choices.Add(ModalChoice.Walking);

            stops = BicycleLeaderRoute.GetRoute();

            StartPosition = stops.Dequeue();
            GoalPosition = stops.Dequeue();

            const int radiusInM = 100;
            if (_choices.Contains(ModalChoice.CyclingOwnBike) && BicycleParkingLayer != null)
                Bicycle = BicycleParkingLayer.CreateOwnBicycleNear(StartPosition, radiusInM, UsesBikeAndRide);

        }

        public override void Tick()
        {
            MultimodalRoute ??= FindMultimodalRoute();

            base.Move();

            if (GoalReached)
            {
                if (stops.Count == 0)
                {
                    MultimodalLayer.UnregisterAgent(MultimodalLayer, this);
                }
                else
                {
                    StartPosition = GoalPosition;
                    GoalPosition = stops.Dequeue();
                    MultimodalRoute = FindMultimodalRoute();
                }
            }
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