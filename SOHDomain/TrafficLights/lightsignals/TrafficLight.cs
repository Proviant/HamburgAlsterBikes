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
        Queue<IAgent> WaitingRoadUsers { get; set; }

        public TrafficLight()
        {
        }

        public void Init(TrafficLightLayer layer)
        {
            this.CurrTime = 0;
            this.CurrPhase = CarLightSignalPhase.GREEN;
            this.WaitingRoadUsers = new Queue<IAgent>();
            this.TrafficLightLayer = layer;
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
        }

        ///
        /// <summary>
        ///  Updates the 
        /// </summary>
        private void CheckQueue()
        {
            // Retrieve IAgent at the front of the queue
            IAgent IAgent = null;
            if (WaitingRoadUsers.Count > 0)
            {
                IAgent = WaitingRoadUsers.Peek();
            }

            // If the leading IAgent is not on this road anymore, dequeue.
            if (IsQueued(IAgent))
            {
                WaitingRoadUsers.Dequeue();
            }
        }

        ///
        /// <summary>
        ///  Enters the LightSignal queue if possible - Meaning a red light, too many cars or not in queue already. Returns true if possible, false otherwise.
        /// </summary>
        public Boolean Enter(IAgent IAgent)
        {
            // If there is a queue in front of the light signal OR there is a non-passable signal being displayed, then queue the agent.
            if (!CanPass(IAgent) && !IsQueued(IAgent))
            {
                this.WaitingRoadUsers.Enqueue(IAgent);
                return true;
            }

            return false;
        }

        ///
        /// <summary>
        ///  Checks, if the IAgent can pass this light signal. Yes if: The light signal is on the opposite side of the street or the current phase is green and
        ///  the IAgent is the only one in queue OR at the head of the queue.
        /// </summary>
        public Boolean CanPass(IAgent IAgent)
        {
            return ((this.CurrPhase.Equals(TrafficLightPhase.Green) || this.CurrPhase.Equals(TrafficLightPhase.Yellow))
            && (WaitingRoadUsers.Count == 0 || WaitingRoadUsers.Peek().Equals(IAgent)));
        }

        ///
        /// <summary>
        ///  Checks, if the IAgent is already waiting.
        /// </summary>
        public Boolean IsQueued(IAgent IAgent)
        {
            return WaitingRoadUsers.Contains(IAgent);
        }
    }
}