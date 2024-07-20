using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UI;

namespace TL.UtilityAI
{
    public class AIBrain : MonoBehaviour
    {
        public bool finishedDeciding { get; set; }
        public bool finishedExecutingBestAction { get; set; }

        public Action bestAction { get; set; }
        private NPCController npc;

        [SerializeField] Billboard billBoard;
        [SerializeField] Action[] actionsAvailable;

        // Start is called before the first frame update
        void Start()
        {
            npc = GetComponent<NPCController>();
            if (npc == null)
            {
                Debug.LogError("NPCController component is missing from the AIBrain GameObject.");
            }

            finishedDeciding = false;
            finishedExecutingBestAction = false;
        }

        // Update is called once per frame
        void Update()
        {
            //if (bestAction is null)
            //{
            //    DecideBestAction(npc.actionsAvailable);
            //}
        }

        // Loop through all the available actions 
        // Give me the highest scoring action
        public void DecideBestAction()
        {
            finishedExecutingBestAction = false;

            float score = 0f;
            int nextBestActionIndex = -1; // Initialize to -1 to detect no valid action

            for (int i = 0; i < actionsAvailable.Length; i++)
            {
                float actionScore = ScoreAction(actionsAvailable[i]);
                if (actionScore > score)
                {
                    nextBestActionIndex = i;
                    score = actionScore;
                }
            }

            if (nextBestActionIndex == -1)
            {
                Debug.LogError("No valid action found.");
                bestAction = null;
            }
            else
            {
                bestAction = actionsAvailable[nextBestActionIndex];
                bestAction.SetRequiredDestination(npc);
                billBoard.UpdateBestActionText(bestAction.Name);
            }

            finishedDeciding = true;
        }

        // Loop through all the considerations of the action
        // Score all the considerations
        // Average the consideration scores ==> overall action score
        public float ScoreAction(Action action)
        {
            float score = 1f;
            for (int i = 0; i < action.considerations.Length; i++)
            {
                float considerationScore = action.considerations[i].ScoreConsideration(npc);
                score *= considerationScore;

                if (score == 0)
                {
                    action.score = 0;
                    return action.score; // No point computing further
                }
            }

            // Averaging scheme of overall score
            float originalScore = score;
            float modFactor = 1 - (1 / action.considerations.Length);
            float makeupValue = (1 - originalScore) * modFactor;
            action.score = originalScore + (makeupValue * originalScore);

            return action.score;
        }
    }
}
