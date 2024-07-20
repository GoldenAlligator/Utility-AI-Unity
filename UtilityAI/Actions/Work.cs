using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Work", menuName = "UtilityAI/Actions/Work")]
    public class Work : Action
    {
        public override void Execute(NPCController npc)
        {
            npc.DoWork(3);
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            if (npc.context == null)
            {
                return;
            }

            if (npc.context.Destinations == null)
            {
                return;
            }

            if (!npc.context.Destinations.ContainsKey(DestinationType.resource))
            {
                Debug.LogError("DestinationType.resource key is missing in NPC context Destinations.");
                return;
            }

            List<Transform> resources = npc.context.Destinations[DestinationType.resource];
            if (resources == null || resources.Count == 0)
            {
                return;
            }

            float distance = Mathf.Infinity;
            Transform nearestResource = null;

            foreach (Transform resource in resources)
            {
                if (resource == null) continue; 
                float distanceFromResource = Vector3.Distance(resource.position, npc.transform.position);
                if (distanceFromResource < distance)
                {
                    nearestResource = resource;
                    distance = distanceFromResource;
                }
            }

            if (nearestResource == null)
            {
                return;
            }

            RequiredDestination = nearestResource;
            npc.mover.destination = RequiredDestination;
        }
    }
}
