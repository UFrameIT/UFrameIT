using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Tape : Gadget
{

    public override void OnHit(RaycastHit hit)
    {
        if (!this.isActiveAndEnabled) return;
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            Fact tempFact = Facts[hit.transform.GetComponent<FactObject>().Id];

            //we can only reach points that are lower than that with the measuring tape
            if (ActiveToolMode == ToolMode.CreateLineMode && tempFact.Representation.transform.position.y > 2.5f)
                return;

            //no 0 distances
            if (FactManager.lineModeIsFirstPointSelected && FactManager.lineModeFirstPointSelected.Id != tempFact.Id)
            {
                //Event for end of line-drawing in "ShinyThings"
                CommunicationEvents.StopLineDrawingEvent.Invoke(null);
                //Create LineFact
                //Check if exactle the same line/distance already exists
                if (!FactManager.factAlreadyExists(new int[] { FactManager.lineModeFirstPointSelected.Id, tempFact.Id }))
                    if (ActiveToolMode == ToolMode.CreateLineMode)
                        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddLineFact(FactManager.lineModeFirstPointSelected.Id, tempFact.Id, FactManager.GetFirstEmptyID()));
                    else
                    {
                        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddRayFact(FactManager.lineModeFirstPointSelected.Id, tempFact.Id, FactManager.GetFirstEmptyID()));

                    }

                FactManager.lineModeIsFirstPointSelected = false;
                FactManager.lineModeFirstPointSelected = null;
            }
            else
            {
                //Activate LineDrawing for preview
                FactManager.lineModeIsFirstPointSelected = true;
                FactManager.lineModeFirstPointSelected = tempFact;
                //Event for start line-drawing in "ShinyThings"
                CommunicationEvents.StartLineDrawingEvent.Invoke(FactManager.lineModeFirstPointSelected);
            }
        }
    }

}
