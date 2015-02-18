﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.RenderBoxes
{


#if DEBUG
    [System.Diagnostics.DebuggerDisplay("RenderBoxBase {dbugGetCssBoxInfo}")]
#endif
    public abstract class RenderBoxBase : RenderElement
    {
        VisualLayerCollection layers;
        int myviewportX;
        int myviewportY;

        public RenderBoxBase(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.MayHasViewport = true;
            this.MayHasChild = true;
        }
        protected VisualLayerCollection MyLayers
        {
            get { return this.layers; }
            set { this.layers = value; }
        }

        public void SetViewport(int viewportX, int viewportY)
        {
            this.myviewportX = viewportX;
            this.myviewportY = viewportY;

            this.InvalidateGraphics();
        }
        public override int ViewportX
        {
            get
            {
                return this.myviewportX;
            }
        }
        public override int ViewportY
        {
            get
            {
                return this.myviewportY;
            }
        }


        public sealed override void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
            canvas.OffsetCanvasOrigin(-myviewportX, -myviewportY);
            updateArea.Offset(myviewportX, myviewportY);
            var currentClipRect = canvas.CurrentClipRect;

            

            this.DrawContent(canvas, updateArea);

             

            canvas.OffsetCanvasOrigin(myviewportX, myviewportY);
            updateArea.Offset(-myviewportX, -myviewportY);
        }

        protected virtual void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            //sample ***
            //1. draw background
            //canvas.FillRectangle(Color.White, 0, 0, updateArea.Width, updateArea.Height);
            canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);

            //2. draw each layer

            if (this.layers != null)
            {
                int j = this.layers.LayerCount;
                switch (j)
                {
                    case 0:
                        {

                        } break;
                    case 1:
                        {
                            layers.Layer0.DrawChildContent(canvas, updateArea);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer0);
#endif
                        } break;
                    case 2:
                        {
                            layers.Layer0.DrawChildContent(canvas, updateArea);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer0);
#endif
                            layers.Layer1.DrawChildContent(canvas, updateArea);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer1);
#endif
                        } break;
                    default:
                        {
                            for (int i = 0; i < j; ++i)
                            {
                                var layer = this.layers.GetLayer(i);
                                layer.DrawChildContent(canvas, updateArea);
#if DEBUG
                                debug_RecordLayerInfo(layer);
#endif
                            }
                        } break;
                }
            }
        }

        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            if (this.layers != null)
            {
                layers.ChildrenHitTestCore(hitChain);
            }
        }

        public void InvalidateContentArrangementFromContainerSizeChanged()
        {
            this.MarkInvalidContentArrangement();
            //foreach (VisualLayer layer in this.GetAllLayerBottomToTopIter())
            //{
            //    layer.InvalidateContentArrangementFromContainerSizeChanged();
            //}
        }
        protected static void InnerDoTopDownReCalculateContentSize(RenderBoxBase containerBase)
        {
            containerBase.TopDownReCalculateContentSize();
        }
        protected static void InnerTopDownReArrangeContentIfNeed(RenderBoxBase containerBase)
        {
            containerBase.TopDownReArrangeContentIfNeed();
        }
        public override sealed void TopDownReCalculateContentSize()
        {

            if (!ForceReArrange && this.HasCalculatedSize)
            {
                return;
            }
#if DEBUG
            dbug_EnterTopDownReCalculateContent(this);
#endif
            int cHeight = this.Height;
            int cWidth = this.Width;
            Size ground_contentSize = Size.Empty;
            if (layers != null)
            {
                ground_contentSize = layers.TopDownReCalculateContentSize();
            }
            int finalWidth = ground_contentSize.Width;
            if (finalWidth == 0)
            {
                finalWidth = this.Width;
            }
            int finalHeight = ground_contentSize.Height;
            if (finalHeight == 0)
            {
                finalHeight = this.Height;
            }
            switch (GetLayoutSpecificDimensionType(this))
            {
                case RenderElementConst.LY_HAS_SPC_HEIGHT:
                    {
                        finalHeight = cHeight;
                    } break;
                case RenderElementConst.LY_HAS_SPC_WIDTH:
                    {
                        finalWidth = cWidth;
                    } break;
                case RenderElementConst.LY_HAS_SPC_SIZE:
                    {
                        finalWidth = cWidth;
                        finalHeight = cHeight;
                    } break;
            }


            SetCalculatedDesiredSize(this, finalWidth, finalHeight);
#if DEBUG
            dbug_ExitTopDownReCalculateContent(this);
#endif

        }


        //-----------------------------------------------------------------
        public void ForceTopDownReArrangeContent()
        {

#if DEBUG
            dbug_EnterReArrangeContent(this);
            dbug_topDownReArrContentPass++;
            this.dbug_BeginArr++;
            debug_PushTopDownElement(this);
#endif

            this.MarkValidContentArrangement();

            IsInTopDownReArrangePhase = true;

            this.layers.ForceTopDownReArrangeContent();
            // BoxEvaluateScrollBar();

#if DEBUG
            this.dbug_FinishArr++;
            debug_PopTopDownElement(this);
            dbug_ExitReArrangeContent();
#endif
        }

        public void TopDownReArrangeContentIfNeed()
        {
#if DEBUG
            bool isIncr = false;
#endif

            if (!ForceReArrange && !this.NeedContentArrangement)
            {
                if (!this.FirstArrangementPass)
                {
                    this.FirstArrangementPass = true;
#if DEBUG
                    dbug_WriteInfo(dbugVisitorMessage.PASS_FIRST_ARR);
#endif

                }
                else
                {
#if DEBUG
                    isIncr = true;
                    this.dbugVRoot.dbugNotNeedArrCount++;
                    this.dbugVRoot.dbugNotNeedArrCountEpisode++;
                    dbug_WriteInfo(dbugVisitorMessage.NOT_NEED_ARR);
                    this.dbugVRoot.dbugNotNeedArrCount--;
#endif
                }
                return;
            }

            ForceTopDownReArrangeContent();


#if DEBUG
            if (isIncr)
            {
                this.dbugVRoot.dbugNotNeedArrCount--;
            }
#endif
        }

        //-------------------------------------------------------------------------------
        public abstract override void ClearAllChildren();

        public override RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
#if DEBUG
            if (afterThisChild.dbugParentVisualElement != this)
            {
                throw new Exception("not a parent-child relation");
            }
#endif
            if (afterThisChild.ParentLink.MayHasOverlapChild)
            {
                return afterThisChild.ParentLink.FindOverlapedChildElementAtPoint(afterThisChild, point);
            }
            return null;
        }
        public Size InnerContentSize
        {
            get
            {
                if (this.layers != null && layers.LayerCount > 0)
                {
                    var layer0 = this.layers.Layer0;
                    Size s1 = layer0.PostCalculateContentSize;

                    if (s1.Width < this.Width)
                    {
                        s1.Width = this.Width;
                    }
                    if (s1.Height < this.Height)
                    {
                        s1.Height = this.Height;
                    }
                    return s1;
                }
                else
                {
                    return this.Size;
                }

            }
        }

        public int ClientTop
        {
            get
            {
                return 0;
            }
        }
        public int ClientLeft
        {
            get
            {
                return 0;
            }
        }

        //--------------------------------------------
#if DEBUG
        public override void dbug_DumpVisualProps(dbugLayoutMsgWriter writer)
        {
            base.dbug_DumpVisualProps(writer);
            writer.EnterNewLevel();

            writer.LeaveCurrentLevel();
        }
        void debug_RecordLayerInfo(RenderElementLayer layer)
        {
            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                visualroot.dbug_AddDrawLayer(layer);
            }
        }
        static int dbug_topDownReArrContentPass = 0;
#endif

    }



}