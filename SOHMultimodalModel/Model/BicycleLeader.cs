using System.Collections.Generic;
using Mars.Common.Core.Random;
using Mars.Interfaces.Environments;
using SOHMultimodalModel.Model;

namespace SOHBicycleModel.Model
{
    public class BicycleLeader : HumanTraveler
    {
        private List<Position> GoalPositions { get; set; }

        public override void Init(HumanTravelerLayer layer)
        {
            base.Init(layer);

            Gender = (GenderType)RandomHelper.Random.Next(0, 2);
            OvertakingActivated = false;

            _choices = new ModalityChooser().Evaluate(this);
            _choices.Add(ModalChoice.Walking);
            _choices.Remove(ModalChoice.CoDriving);
            _choices.Remove(ModalChoice.CarDriving);
            _choices.Remove(ModalChoice.CarRentalDriving);

            GoalPositions = new List<Position>();

            // TODO Hier Koordinaten von Zielort rausfinden.
            StartPosition = Position.CreateGeoPosition(9.99175, 53.55397);
            GoalPosition = Position.CreateGeoPosition(9.99187, 53.5539);

            const int radiusInM = 100;
            if (_choices.Contains(ModalChoice.CyclingOwnBike) && BicycleParkingLayer != null)
                Bicycle = BicycleParkingLayer.CreateOwnBicycleNear(StartPosition, radiusInM, UsesBikeAndRide);
        }
    }
}