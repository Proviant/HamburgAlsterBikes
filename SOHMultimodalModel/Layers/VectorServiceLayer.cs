using System.Linq;
using Mars.Common;
using Mars.Common.Core.Random;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace SOHMultimodalModel.Layers
{
    public class VectorServiceLayer : VectorLayer 
    {
        public Position RandomPosition()
        {
            if (Features.Count <= 0)
                return null;

            var feature = Features.ElementAt(RandomHelper.Random.NextInteger(0, Features.Count));
            return feature.VectorStructured.Geometry.RandomPositionFromGeometry();
        }
    
    }    
}

