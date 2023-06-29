using System.Collections.Generic;
using Mars.Interfaces.Agents;
using SOHTravellingBox.model.Data;
using System;
using Mars.Interfaces.Environments;
using SOHDomain.Model;
using Mars.Interfaces.Annotations;
using SOHDomain.TrafficLights.layers;
using SOHDomain.TrafficLights;

namespace SOHTravellingBox.model
{
    public class TrafficLight : IAgent<TrafficLightLayer>
    {
        // The position of this traffic light
        [PropertyDescription(Name = "Longitude")]
        public Double Longitude { get; set; }
        [PropertyDescription(Name = "Latidute")]
        public Double Latidute { get; set; }
        // The environment this traffic signal is part of.
        [PropertyDescription]
        private TrafficLightLayer TrafficLightLayer { get; set; }

        Guid IEntity.ID { get; set; }

        // The waiting time in seconds, when the light signal is red / impassable
        [PropertyDescription(Name = "LengthPhaseRed")]
        int LengthPhaseRed { get; set; }
        // The allowed-to-move time in seconds, when the light signal is green / passable.
        [PropertyDescription(Name = "LengthPhaseGreen")]
        int LengthPhaseGreen { get; set; }
        // The already waited time in the current phase.
        int CurrTime { get; set; }
        // The current phase of the light signal
        CarLightSignalPhase CurrPhase { get; set; }

        // The currently waiting agents in front of the signal.
        Queue<RoadUser> WaitingRoadUsers { get; set; }

        public TrafficLight()
        {
        }

        public void Init(TrafficLightLayer layer)
        {
            this.CurrTime = 0;
            this.CurrPhase = CarLightSignalPhase.GREEN;
            this.WaitingRoadUsers = new Queue<RoadUser>();
            this.TrafficLightLayer = layer;
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

            // Retrieve RoadUser at the front of the queue
            RoadUser roadUser = null;
            if (WaitingRoadUsers.Count > 0)
            {
                roadUser = WaitingRoadUsers.Peek();
            }

            // If the leading RoadUser is not on this road anymore, dequeue.
            if (IsQueued(roadUser) && !IsOnSameRoad(roadUser))
            {
                if (WaitingRoadUsers.Count > 0)
                {
                    WaitingRoadUsers.Dequeue();
                }
            }
        }

        ///
        /// <summary>
        ///  Enters the LightSignal queue if possible - Meaning a red light, too many cars or not in queue already. Returns true if possible, false otherwise.
        /// </summary>
        public Boolean Enter(RoadUser RoadUser)
        {
            // If there is a queue in front of the light signal OR there is a non-passable signal being displayed, then queue the agent.
            if (IsOnSameRoad(RoadUser) && !CanPass(RoadUser) && !IsQueued(RoadUser))
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
            if (RoadUser == null)
            {
                return false;
            }

            ISpatialEdge EdgeRoadUser = RoadUser.CurrentEdge;
            ISpatialNode OwnNearestNode = TrafficLightLayer.Environment.NearestNode(new Position(Longitude, Latidute));

            // Iterate over all streets towards the node / traffic light. 
            foreach (KeyValuePair<int, ISpatialEdge> Pair in OwnNearestNode.IncomingEdges)
            {
                // If any incoming edge / street matches, then this traffic signal is on the same road.
                if (Pair.Value.Equals(EdgeRoadUser))
                {
                    return true;
                }
            }

            return false;
        }

        ///
        /// <summary>
        ///  Checks, if the RoadUser is already waiting.
        /// </summary>
        public Boolean IsQueued(RoadUser RoadUser)
        {
            return WaitingRoadUsers.Contains(RoadUser);
        }
    }
}