using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIHelper {

	public static float HeightDifference(this ScrollRect scrollrect)
	{
		return Mathf.Max (scrollrect.content.rect.height - (scrollrect.transform as RectTransform).rect.height, 0f);
	}

	public static float HeightDifference(this ScrollRect scrollrect, RectTransform scrollrectTransform)
	{
		return Mathf.Max (scrollrect.content.rect.height - scrollrectTransform.rect.height, 0f);
	}

	public static float TopOffset(this ScrollRect scrollrect)
	{
		return scrollrect.HeightDifference() * (1f - scrollrect.verticalNormalizedPosition);
	}

	public static float BottomOffset(this ScrollRect scrollrect)
	{
		return scrollrect.HeightDifference() * scrollrect.verticalNormalizedPosition;
	}

	public static float BottomOffset(this ScrollRect scrollrect, RectTransform scrollrectTransform)
	{
		return scrollrect.HeightDifference(scrollrectTransform) * scrollrect.verticalNormalizedPosition;
	}

	public static void SetBottomOffset(this ScrollRect scrollrect, RectTransform scrollrectTransform, float bottomOffset)
	{
		float heighDiff = scrollrect.HeightDifference(scrollrectTransform);
		if(Mathf.Approximately(heighDiff, 0))
		{
			scrollrect.verticalNormalizedPosition = 0;
		}
		else
		{
			scrollrect.verticalNormalizedPosition = Mathf.Clamp01(bottomOffset/heighDiff);
		}
	}

	public static void SetTopOffset(this ScrollRect scrollrect, RectTransform scrollrectTransform, float topOffset)
	{
		float heighDiff = scrollrect.HeightDifference(scrollrectTransform);
		if(Mathf.Approximately(heighDiff, 0))
		{
			scrollrect.verticalNormalizedPosition = 0;
		}
		else
		{
			scrollrect.verticalNormalizedPosition = Mathf.Clamp01(1 - topOffset/heighDiff);
		}
	}
}
