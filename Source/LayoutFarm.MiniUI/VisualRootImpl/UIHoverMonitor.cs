﻿//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;





namespace LayoutFarm.Presentation
{


    public class ArtUIHoverMonitorTask : ArtVisualRootTimerTask
    {

        int mouseMoveCounter = -1;
        EventHandler targetEventHandler;
        public ArtUIHoverMonitorTask(ArtVisualRootWindow winroot, EventHandler targetEventHandler)
            : base(winroot)
        {
            this.targetEventHandler = targetEventHandler;
        }
        public override void Reset()
        {
            mouseMoveCounter = -1;
        }
        public override void Tick()
        {
            mouseMoveCounter++;
            if (mouseMoveCounter > 1)
            {
                targetEventHandler(this, EventArgs.Empty);
            }
        }
    }


}