using UnityEngine;
using System.Collections.Generic;

public class AnimationCurveTweener : FloatTweener
{
	internal float timer = 0.0f;
	public bool ignoreTimeScale = false;

	public AnimationCurve AniCurve { get; set; }

	public override void Update()
	{
		if (timer >= duration)
		{
			Tweening = false;
			timer = 0.0f;
			return;
		}

		timer += ignoreTimeScale ? Time.fixedDeltaTime : Time.deltaTime;

		CurrentValue = AniCurve.Evaluate(timer / duration);
		if (onChanged != null)
			onChanged(CurrentValue);
	}
}

public class FloatTweener
{
	internal float targetValue;
	internal float duration;

	internal float delta;
	internal System.Action<float> onChanged = null;

	public bool Tweening { get; set; }

	public float CurrentValue { get; set; }

	public static FloatTweener CreateTweener(float cv, float tv, float duration)
	{
		if (duration == 0)
			return null;

		FloatTweener ret = new FloatTweener();
		ret.CurrentValue = cv;
		ret.targetValue = tv;
		ret.duration = duration;
		ret.delta = (tv - cv) / duration * Time.deltaTime;
		if (ret.delta != 0)
		{
			ret.Tweening = true;
		}

		return ret;
	}

	public virtual void Update()
	{
		if (CurrentValue == targetValue)
		{
			Tweening = false;
			return;
		}

		if (Mathf.Abs(targetValue - CurrentValue) <= Mathf.Abs(delta))
		{
			CurrentValue = targetValue;
			Tweening = false;
			if (onChanged != null)
				onChanged(CurrentValue);
		}
		else
		{
			CurrentValue += delta;
			Tweening = true;
			if (onChanged != null)
				onChanged(CurrentValue);
		}
	}
}

public class Tweener : MonoSingleton<Tweener>
{
	List<FloatTweener> tweeners = new List<FloatTweener>();

	public FloatTweener CreateFloatTweener(float cv, float tv, float duration, System.Action<float> onchanged)
	{
		if (duration == 0)
			return null;

		FloatTweener ret = new FloatTweener();
		ret.CurrentValue = cv;
		ret.targetValue = tv;
		ret.duration = duration;
		ret.delta = (tv - cv) / duration * Time.deltaTime;
		ret.Tweening = ret.delta != 0;
		ret.onChanged = onchanged;
		tweeners.Add(ret);

		return ret;
	}

	public AnimationCurveTweener CreateAnimationCurveTweener(AnimationCurve aniCurve, float duration, System.Action<float> onchanged, bool ignoreTimeScale = false)
	{
		if (aniCurve == null)
			return null;

		AnimationCurveTweener ret = new AnimationCurveTweener();
		ret.AniCurve = aniCurve;
		ret.Tweening = true;
		ret.onChanged = onchanged;
		ret.duration = duration;
		ret.ignoreTimeScale = ignoreTimeScale;
		tweeners.Add(ret);

		return ret;
	}

	public void RemoveTweener(FloatTweener ft)
	{
		if(ft != null && tweeners.Contains(ft))
		{
			tweeners.Remove(ft);
		}
	}

	public void Clear()
	{
		tweeners.Clear();
	}

	public void Update()
	{
		for(int i = 0; i < tweeners.Count; i++)
		{
			tweeners[i].Update();
		}
	}

	void LateUpdate()
	{
		tweeners.RemoveAll((x) => { return !x.Tweening; });
	}
}
