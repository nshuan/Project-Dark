using System;
using Dark.Scripts.Utils.Camera;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public interface INodePreviewPositionLogic
    {
        void UpdatePosition(ref Vector2 mousePos, ref Vector2 hoverNodePosition, ref Vector2 hoverNodePadding, ref Vector2 rectInfoFramePadding,
            ref RectTransform rectInfoFrame);
    }
    
    [Serializable]
    public class NodePreviewOnMousePosition : INodePreviewPositionLogic
    {
        public void UpdatePosition(ref Vector2 mousePos, ref Vector2 hoverNodePosition, ref Vector2 hoverNodePadding, ref Vector2 rectInfoFramePadding,
            ref RectTransform rectInfoFrame)
        {
            var framePos = mousePos;
            var framePivot = new Vector2(0f, 0.5f);
            
            // Check if the panel is outside the screen
            if (mousePos.x + rectInfoFrame.sizeDelta.x - rectInfoFramePadding.x > SafeScaler.ScreenWidth)
            {
                framePivot.x = 1f;
            }
            else
            {
                framePivot.x = 0f;
            }

            if (mousePos.y + rectInfoFrame.sizeDelta.y / 2 - rectInfoFramePadding.y > SafeScaler.ScreenHeight)
                framePivot.y = 1f;
            else if (mousePos.y - rectInfoFrame.sizeDelta.y / 2 + rectInfoFramePadding.y < 0)
                framePivot.y = 0f;
            else
                framePivot.y = 1f;

            rectInfoFrame.position = framePos;
            rectInfoFrame.pivot = framePivot;
        }
    }
    
    [Serializable]
    public class NodePreviewNextToNode : INodePreviewPositionLogic
    {
        public void UpdatePosition(ref Vector2 mousePos, ref Vector2 hoverNodePosition, ref Vector2 hoverNodePadding,
            ref Vector2 rectInfoFramePadding, ref RectTransform rectInfoFrame)
        {
            var framePos = hoverNodePosition;
            var framePivot = new Vector2(0f, 0.5f);
            
            // Check if the panel is outside the screen
            if (hoverNodePosition.x + hoverNodePadding.x * ZoomInOut.CurrentScale + rectInfoFrame.sizeDelta.x - rectInfoFramePadding.x >
                SafeScaler.ScreenWidth)
            {
                framePivot.x = 1f;
                framePos.x = hoverNodePosition.x - hoverNodePadding.x * ZoomInOut.CurrentScale;
            }
            else
            {
                framePivot.x = 0f;
                framePos.x = hoverNodePosition.x + hoverNodePadding.x * ZoomInOut.CurrentScale;
            }

            if (hoverNodePosition.y + rectInfoFrame.sizeDelta.y / 2 - rectInfoFramePadding.y >
                SafeScaler.ScreenHeight)
            {
                framePivot.y = 1f;
            }
            else if (hoverNodePosition.y - rectInfoFrame.sizeDelta.y / 2 - rectInfoFramePadding.y < 0)
            {
                framePivot.y = 0f;
            }
            else
            {
                framePivot.y = 0.5f;
            }
            
            rectInfoFrame.position = framePos;
            rectInfoFrame.pivot = framePivot;
        }
    }
}