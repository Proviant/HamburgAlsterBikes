using System.Collections.Generic;
using Mars.Interfaces.Agents;
using SOHTravellingBox.model.Data;
using System;
using Mars.Interfaces.Environments;
using SOHDomain.Model;

namespace SOHTravellingBox.model
{
    public class LightSignal : IEntity
    {
        // The street this light signal is part of
        public ISpatialEdge StreetLocation { get; set; }
        Guid IEntity.ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // The waiting time in seconds, when the light signal is red / impassable
        int LengthPhaseRed { get; set; }
        // The allowed-to-move time in seconds, when the light signal is green / passable.
        int LengthPhaseGreen { get; set; }
        // The already waited time in the current phase.
        int CurrTime { get; set; }
        // The current phase of the light signal
        CarLightSignalPhase CurrPhase { get; set; }

        // The currently waiting agents in front of the signal.
        Queue<RoadUser> WaitingRoadUsers { get; set; }

        public LightSignal(ISpatialEdge Edge, int LengthPhaseRed, int LengthPhaseGreen)
        {
            this.LengthPhaseGreen = LengthPhaseGreen;
            this.LengthPhaseRed = LengthPhaseRed;
            this.CurrTime = 0;
            this.CurrPhase = CarLightSignalPhase.GREEN;
            this.WaitingRoadUsers = new Queue<RoadUser>();
        }

        public LightSignal() : this(null, 0, 0)
        {
        }

        ///
        /// <summary>
        ///  Advances the clock inside the light signal and allows it to switch to green or red over time.
        /// </summary>
        public void Tick()
        {
            CurrTime++;

            // If the current time exceeds the green phase time limit, then switch to red
            if (CurrTime > LengthPhaseGreen)
            {
                CurrPhase = CarLightSignalPhase.RED;

                // But if the red phase time limit is also exceeded, turn time back to start (0).
                if (CurrTime >= LengthPhaseGreen + LengthPhaseRed)
                {
                    CurrTime = 0;
                    CurrPhase = CarLightSignalPhase.GREEN;
                }
            }

            // If the leading RoadUser is not on this road anymore, dequeue.
            if (!IsOnSameRoad(WaitingRoadUsers.Peek()))
            {
                WaitingRoadUsers.Dequeue();
            }
        }

        ///
        /// <summary>
        ///  Enters the LightSignal queue if possible - Meaning a red light, too many cars or not in queue already. Returns true if possible, false otherwise.
        /// </summary>
        public Boolean Enter(RoadUser RoadUser)
        {
            // If there is a queue in front of the light signal OR there is a non-passable signal being displayed, then queue the agent.
            if (IsOnSameRoad(RoadUser) && !CanPass(RoadUser) && !WaitingRoadUsers.Contains(RoadUser))
            {
                this.WaitingRoadUsers.Enqueue(RoadUser);
                return true;
            }

            return false;
        }

        ///
        /// <summary>
        ///  Checks, if the RoadUser can pass this light signal. Yes if: The light signal is on the opposite side of the street or the current phase is green and
        ///  the RoadUser is the only one in queue OR at the head of the queue.
        /// </summary>
        public Boolean CanPass(RoadUser RoadUser)
        {
            return (Convert.ToBoolean(this.CurrPhase) && (WaitingRoadUsers.Count == 0 || WaitingRoadUsers.Peek().Equals(RoadUser)));
        }

        ///
        /// <summary>
        ///  Checks, if the RoadUser is on the same road as the lightsignal.
        /// </summary>
        public Boolean IsOnSameRoad(RoadUser RoadUser)
        {
            return RoadUser.CurrentEdge.Equals(StreetLocation);
        }
    }
}