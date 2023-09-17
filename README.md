# Hamburg Alster Bikes

This repository contains an agent-based model written in [MARS](https://www.mars-group.org/) designed to analyze the conduciveness of traffic light patterns to bicycle travelers around the Binnen- and Außenalster of Hamburg, Germany.

## Software Requirements

-   [JetBrains Rider](https://www.jetbrains.com/rider/)
-   [.NET SDK](https://dotnet.microsoft.com/download/dotnet-core/)

## Getting Started

1. Open a terminal, that can run commands
2. Change directory to code-repo: `C:\Users\User\Desktop\Vorlesungen\#Bachelorarbeit\Code\HamburgAlsterBikes`
3. Execute the following command: `dotnet run -sm config.json -project HamburgAlsterBikes.csproj`

## Simulation Area

See the PNG files named `hamburg_alster_*.png` in the README_images directory for screenshots of different sections of the simulation area. The graph files are already integrated into the model via the `config.json`, whereas the file `hamburg_alster_pois.geojson` is not integrated.

## SOH modeling introduction

The SOH model provides urban mobility functionality for agents. Agents can therefore use different modalities (transportation devices) to reach their goals in the city. Movement is executed on a graph structure that represents roads, sidewalks, or railways.

### Agent types

The SOH model provides two main types of [agents](https://www.mars-group.org/docs/tutorial/soh/agents.html) that have a mobility desire (beside pure driver agents that fulfill the role of public transport).

-   [`Traveller` agents](https://www.mars-group.org/docs/tutorial/soh/agents/traveler.html) have a start and a goal and they try to reach their goal by using available transportation devices, which we call their movement `capabilities`. They can be spawned randomly by an `AgentSchedulerLayer` within a given spawn area and find random goals within a given target area. They can be used to generate mobility demand.
-   [`Citizen` agents](https://www.mars-group.org/docs/tutorial/soh/agents/citizen.html) have a daily schedule that cause their mobility demand. The schedule is defined in terms on their employment status. They can also choose between the modalities that are generally provided in the respective scenario and that are available or reasonable for the particular agent and its current location.

### Modalities

The SOH model provides a variety of modalities that can be used. We call them `ModalChoice`s.

-   `Walking` is the main modality and always available.
-   `CarDriving` requires an own car for the agent (co-driving is not yet implemented) that has to be parked on a parking place. The agent moves to the car, drives to a parking place near by the goal, and then concludes the rest of the way by foot.
-   `CyclingOwnBike` is quite similar to walking, because the bike can either be parked at the node or in a bike station. Because it can be parked quite everywhere, agents can move from start to goal with the bike (if the bike is available at the start node).
-   `CyclingRentalBike` is using a rental bike. The agent walks to a near by rental station that has remaining bikes, takes a bike that needs to be returned at another rental station and then finishes the remaining route by foot.
-   `Train` can be used to drive as a passenger. Therefore the agents searches a reasonable train station near by and exits the train station near the goal. A transfer between lines is possible at stations that provide different lines.
-   `Ferry` is quite similar to using the train just with ships moving over water.

### Environment

Although there are different modal choices, some of these share the same environment, for instance bikes might also use the streets like cars. We therefore have the `SpatialModalityType` discriminator that describes which lanes can be used by which transportation devices.

For movement, we need a [graph](https://www.mars-group.org/docs/tutorial/soh/layers/vector_layer.html#modality-networks) because all transportation devices require it. The graph is stored in the `SpatialGraphEnvironment` (SGE) that provides route searching capabilities and supervises movement concerning validity constraints like collision detection.

The environment is initialized by graphs that can be imported in either `graphml` or `geojson` format. For multimodal route searching, we require to integrate all relevant graphs in one SGE. So use the `inputs` configuration in the simulation config and add an import configuration to define that edges (later transformed to lanes) of this file can be used by a set of modalities (spatial modality types). See the `config.json` file in `HamburgAlsterBikes` for an example.

### Handle concept

The usage of transportation devices follows a [handle concept](https://www.mars-group.org/docs/tutorial/soh/steering) that is a contract between agent and vehicle. If the agent provides the required capabilities, then a vehicle can provide a handle for usage.

![contract](README_images/contract_schema.png)

Every vehicle type defines a steering handle that is provided by the respective vehicle on entrance. The handle takes care about the concrete movement logic and so capsulates the movement behavior by following traffic rules (like driving a car without actively thinking how to do it). The handle requires the agent to have certain capabilities that are required to use the vehicle. These are defined in the respective `ISteeringCapabable`. After leaving a vehicle the handle is invalidated and exchanged with the default `WalkingSteeringHandle`.

![car_steering_handle_concept](README_images/uml_car_steering.png)

## MARS Documentation

For more information on the above and other topics related to modelling and simulating with MARS, please see the [MARS documentation](https://www.mars-group.org/docs/tutorial/intro).
