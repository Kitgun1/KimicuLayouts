using KimicuUtility;
using UnityEngine;
using UnityEngine.UI;

namespace KimicuLayouts.Runtime
{
    public class KiHorizontalLayout : LayoutGroup
    {
        public RectOffset Padding => m_Padding;
        public float Spacing;

        public VectorBoolean2 ByPercentage;
        public bool ControlSizeHeight = true;
        public float Height = 50;
        public float Width = 50;
        public bool VerticalFit;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            int countChild = rectChildren.Count;

            float containerWidth = rectTransform.rect.width - m_Padding.horizontal;
            float elementWidth;
            if (ByPercentage.X)
            {
                elementWidth = (containerWidth - Spacing * (countChild - 1)) / countChild;
                Width = elementWidth;
            }
            else elementWidth = Width;

            float containerHeight = rectTransform.rect.height - m_Padding.vertical;

            float xPos = m_Padding.left;
            float yPos = m_Padding.top;

            float childMaxHeight = float.MinValue;

            foreach (RectTransform child in rectChildren)
            {
                if (child.sizeDelta.y > childMaxHeight) childMaxHeight = child.sizeDelta.y;

                SetChildAlongAxis(child, 0, xPos, elementWidth);
                if (ByPercentage.Y)
                {
                    SetChildAlongAxis(child, 1, yPos, containerHeight);
                    Height = containerHeight;
                }
                else
                {
                    if (ControlSizeHeight)
                    {
                        SetChildAlongAxis(child, 1, yPos, Height);
                    }
                    else
                    {
                        child.sizeDelta = new Vector2(elementWidth, child.rect.height);
                        float x = (xPos + child.sizeDelta.x / 2);
                        float y = (yPos - child.sizeDelta.y / 2) - m_Padding.top * 2;
                        child.anchoredPosition = new Vector2(x, y);
                    }
                }

                xPos += Spacing + elementWidth;
            }

            if (!VerticalFit) return;
            float sizeX = ByPercentage.X
                ? containerWidth + m_Padding.horizontal
                : (Width + Spacing) * countChild + m_Padding.horizontal - Spacing;

            float sizeY = m_Padding.vertical + (ByPercentage.Y || ControlSizeHeight ? Height : childMaxHeight);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeX);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeY);
        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}