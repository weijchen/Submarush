using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
public class AlignmentBehavior : FilteredFlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // if no neighbors, maintain current alignment
        if (context.Count == 0 || (filter.Filter(agent, context).Count == 0))
            return agent.transform.up;
        
        // add all points together and average
        Vector3 alignmentMove = Vector3.zero;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            alignmentMove += item.transform.up;
        }

        alignmentMove /= filteredContext.Count;
        
        return alignmentMove;
    }
}
