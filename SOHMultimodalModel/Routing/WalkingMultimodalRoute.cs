using Mars.Interfaces.Environments;
using SOHDomain.Graph;

namespace SOHMultimodalModel.Routing
{
    /// <summary>
    ///     Complies the MultimodalRoute class, but only holds a walking route from start to goal
    /// </summary>
    public class WalkingMultimodalRoute : MultimodalRoute
    {
        public WalkingMultimodalRoute(SpatialGraphMediatorLayer environment, Position start, Position goal)
        {
            var walk = environment.Environment;
            var startNode = walk.NearestNode(start, null, SpatialModalityType.Walking);
            var goalNode = walk.NearestNode(goal, SpatialModalityType.Walking);

            var route = walk.FindShortestRoute(startNode, goalNode,
                edge => edge.Modalities.Contains(SpatialModalityType.Walking));
            Add(route, ModalChoice.Walking);
        }
    }
}