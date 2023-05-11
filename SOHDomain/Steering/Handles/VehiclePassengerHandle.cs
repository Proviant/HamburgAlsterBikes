using Mars.Interfaces.Environments;
using SOHDomain.Model;
using SOHDomain.Steering.Capables;
using SOHDomain.Steering.Common;

namespace SOHDomain.Steering.Handles
{
    /// <summary>
    ///     Standard implementation of the <c>IPassengerHandle</c> interface, that allows driver and passenger to leave
    ///     the vehicle if they are on it.
    /// </summary>
    /// <typeparam name="TSteeringCapable"></typeparam>
    /// <typeparam name="TPassengerCapable"></typeparam>
    /// <typeparam name="TSteeringHandle"></typeparam>
    /// <typeparam name="TPassengerHandle"></typeparam>
    public class VehiclePassengerHandle
        <TSteeringCapable, TPassengerCapable, TSteeringHandle, TPassengerHandle> : IPassengerHandle
        where TSteeringCapable : ISteeringCapable
        where TPassengerCapable : IPassengerCapable
        where TSteeringHandle : ISteeringHandle
        where TPassengerHandle : IPassengerHandle
    {
        protected readonly Vehicle<TSteeringCapable, TPassengerCapable, TSteeringHandle, TPassengerHandle> Vehicle;

        public VehiclePassengerHandle(
            Vehicle<TSteeringCapable, TPassengerCapable, TSteeringHandle, TPassengerHandle> vehicle)
        {
            Vehicle = vehicle;
        }

        public Position Position => Vehicle.Position;

        public bool LeaveVehicle(IPassengerCapable passengerCapable)
        {
            if (passengerCapable.Equals(Vehicle.Driver))
            {
                Vehicle.Driver = null;
                Vehicle.NotifyPassengers(PassengerMessage.NoDriver);
                return true;
            }

            if (Vehicle.Passengers.Contains(passengerCapable))
            {
                Vehicle.Passengers.Remove(passengerCapable);
                return true;
            }

            return false;
        }
    }
}