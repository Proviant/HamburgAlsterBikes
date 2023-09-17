using Internal;
using System.Collections.Generic;
using Mars.Interfaces.Agents;
using SOHTravellingBox.model.Data;
using System;
using Mars.Interfaces.Environments;
using SOHDomain.Model;
using Mars.Interfaces.Annotations;
using SOHDomain.TrafficLights.layers;
using SOHDomain.TrafficLights;
using System.Collections;
using Mars.Numerics;
using ServiceStack;

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
        int LengthPhaseRed { get; set; }
        // The allowed-to-move time in seconds, when the light signal is yellow / passable.
        int LengthPhaseYellow { get; set; }
        // The allowed-to-move time in seconds, when the light signal is green / passable.
        int LengthPhaseGreen { get; set; }
        // The already waited time in the current phase.
        int CurrTime { get; set; }
        // The current phase of the light signal
        CarLightSignalPhase CurrPhase { get; set; }

        // The currently waiting agents in front of the signal.
        Queue<IAgent> WaitingRoadUsers { get; set; }

        public TrafficLight()
        {
            CurrPhase = CarLightSignalPhase.GREEN;
            WaitingRoadUsers = new();
        }

        public void Init(TrafficLightLayer layer)
        {
            Random r = new();
            CurrPhase = CarLightSignalPhase.GREEN;
            WaitingRoadUsers = new();
            TrafficLightLayer = layer;

            LengthPhaseGreen = 70;
            LengthPhaseYellow = 1;
            LengthPhaseRed = 20;

            CurrTime = r.Next(0, LengthPhaseGreen + LengthPhaseYellow + LengthPhaseRed);
        }

        ///
        /// <summary>
        ///  Advances the clock inside the light signal and allows it to switch to green or red over time.
        /// </summary>
        public void Tick()
        {
            UpdateTime();
            CheckQueue();
        }

        ///
        /// <summary>
        ///  Method which is called to advance the traffic light clock.
        /// </summary>
        private void UpdateTime()
        {
            CurrTime++;

            // If the current time exceeds the green phase time limit, then switch to yellow
            if (LengthPhaseYellow > 0 && (CurrTime > LengthPhaseGreen || CurrTime > LengthPhaseGreen + LengthPhaseYellow + LengthPhaseRed))
            {
                CurrPhase = CarLightSignalPhase.YELLOW;
            }

            // If the current time exceeds the green + yellow phase time limit, then switch to red
            if (LengthPhaseRed > 0 && CurrTime > LengthPhaseGreen + LengthPhaseYellow)
            {
                CurrPhase = CarLightSignalPhase.RED;
            }

            // But if the red phase time limit is also exceeded, turn time back to start (0).
            if (LengthPhaseGreen > 0 && CurrTime >= LengthPhaseGreen + LengthPhaseRed + 2 * LengthPhaseYellow)
            {
                CurrTime = 0;
                CurrPhase = CarLightSignalPhase.GREEN;
            }
        }

        ///
        /// <summary>
        ///  Updates the 
        /// </summary>
        public void CheckQueue()
        {
            // Retrieve IAgent at the front of the queue
            IAgent IAgent = null;
            if (WaitingRoadUsers.Count > 0)
            {
                IAgent = WaitingRoadUsers.Peek();
            }

            // If the leading IAgent is not on this road anymore, dequeue.
            if (IsQueued(IAgent) && CanPass(IAgent))
            {
                RemoveFirstFromQueue();
            }
        }

        ///
        /// <summary>
        ///  Enters the LightSignal queue if possible - Meaning a red light, too many cars or not in queue already. Returns true if possible, false otherwise.
        /// </summary>
        public Boolean Enter(IAgent IAgent)
        {
            if (IsQueued(IAgent) || CanPass(IAgent))
            {
                return false;
            }

            // If the agent itself isn't already queued, there is a queue in front of the light signal OR there is a non-passable signal being displayed, then queue the agent.
            AddLastToQueue(IAgent);
            return true;
        }

        ///
        /// <summary>
        ///  Checks, if the IAgent can pass this light signal. Yes if: The light signal is on the opposite side of the street or the current phase is green and
        ///  the IAgent is the only one in queue OR at the head of the queue.
        /// </summary>
        public Boolean CanPass(IAgent IAgent)
        {
            if (IAgent == null) return true;
            try
            {
                return (this.CurrPhase.Equals(CarLightSignalPhase.GREEN) || this.CurrPhase.Equals(CarLightSignalPhase.YELLOW))
                && (WaitingRoadUsers.Count == 0 || WaitingRoadUsers.Peek().Equals(IAgent));
            }
            catch (AggregateException)
            {
                // Sollte aufgrund von parrallelen Tasks die Queue plötzlich verändert werden, so wird eine AggregateException geworfen.
                return WaitingRoadUsers.Count == 0;
            }
        }

        ///
        /// <summary>
        ///  Checks, if the IAgent is already waiting.
        /// </summary>
        public Boolean IsQueued(IAgent IAgent)
        {
            if (IAgent == null) return false;
            return WaitingRoadUsers.Contains(IAgent);
        }

        public int GetWaitingRoadUsers()
        {
            return WaitingRoadUsers.Count;
        }

        private void AddLastToQueue(IAgent agent)
        {
            WaitingRoadUsers.Enqueue(agent);
        }

        private void RemoveFirstFromQueue()
        {
            WaitingRoadUsers.Dequeue();
        }

        public String getName()
        {
            return "lat:" + Latidute + "-lon:" + Longitude;
        }
    }
}