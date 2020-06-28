using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class AngleTool : Gadget
{

    public override void OnHit(RaycastHit hit)
    {
        if (!this.isActiveAndEnabled) return;
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            Fact tempFact = Facts[hit.transform.GetComponent<FactObject>().Id];

            //If two points were already selected and now the third point got selected
            if (FactManager.angleModeIsFirstPointSelected && FactManager.angleModeIsSecondPointSelected)
            {
                //Event for end of curve-drawing in "ShinyThings"
                CommunicationEvents.StopCurveDrawingEvent.Invoke(null);
                //Create AngleFact
                //Check if new Point is equal to one of the previous points -> if true -> cancel
                if (!(FactManager.angleModeFirstPointSelected.Id == tempFact.Id || FactManager.angleModeSecondPointSelected.Id == tempFact.Id))
                {
                    //Check if exactly the same angle already exists
                    if (!FactManager.factAlreadyExists(new int[] { ((PointFact)FactManager.angleModeFirstPointSelected).Id, ((PointFact)FactManager.angleModeSecondPointSelected).Id, ((PointFact)tempFact).Id }))
                        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddAngleFact(((PointFact)FactManager.angleModeFirstPointSelected).Id, ((PointFact)FactManager.angleModeSecondPointSelected).Id, ((PointFact)tempFact).Id, FactManager.GetFirstEmptyID()));
                }

                FactManager.angleModeIsFirstPointSelected = false;
                FactManager.angleModeFirstPointSelected = null;
                FactManager.angleModeIsSecondPointSelected = false;
                FactManager.angleModeSecondPointSelected = null;
            }
            //If only one point was already selected
            else if (FactManager.angleModeIsFirstPointSelected && !FactManager.angleModeIsSecondPointSelected)
            {
                //Check if the 2 selected points are the same: If not
                if (FactManager.angleModeFirstPointSelected.Id != tempFact.Id)
                {
                    FactManager.angleModeIsSecondPointSelected = true;
                    FactManager.angleModeSecondPointSelected = tempFact;

                    //Event for start of curve-drawing in "ShinyThings"
                    //Create new LineFact with the 2 points
                    LineFact tempLineFact = new LineFact();
                    tempLineFact.Pid1 = FactManager.angleModeFirstPointSelected.Id;
                    tempLineFact.Pid2 = FactManager.angleModeSecondPointSelected.Id;
                    CommunicationEvents.StartCurveDrawingEvent.Invoke(tempLineFact);
                }
                else
                {
                    FactManager.angleModeFirstPointSelected = null;
                    FactManager.angleModeIsFirstPointSelected = false;
                }
            }
            //If no point was selected before
            else
            {
                //Save the first point selected
                FactManager.angleModeIsFirstPointSelected = true;
                FactManager.angleModeFirstPointSelected = tempFact;
            }
        }
        //No point was hit
        else
        {
            if (FactManager.angleModeIsFirstPointSelected && FactManager.angleModeIsSecondPointSelected)
            {
                //Event for end of curve-drawing in "ShinyThings"
                CommunicationEvents.StopCurveDrawingEvent.Invoke(null);
            }

            //Reset Angle-Preview-Attributes
            FactManager.angleModeIsFirstPointSelected = false;
            FactManager.angleModeFirstPointSelected = null;
            FactManager.angleModeIsSecondPointSelected = false;
            FactManager.angleModeSecondPointSelected = null;

            //TODO: Hint that only an angle can be created between 3 already existing points
        }
    }

  
}
