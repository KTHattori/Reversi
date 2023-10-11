//-----------------------------------------------
// Easing.cs
// - A script to use easing functions in Unity
//--[Author]-------------------------------------
// Keita Hattori
//--[History]------------------------------------
// YYYY_MM_DD--EDITOR_NAME--EDIT_DESCRIPTION
// 2023_03_24--Keita_Hattori--Created_This_File
//--[About]--------------------------------------
// Customized Easing for using in UnityEngine.
// Easing is commonly used for : 
// Animations,Transitions
//--[Reference]----------------------------------
// https://easings.net/ - for Easing Functions
//-----------------------------------------------

// --- NAMESPACES ---
using UnityEngine;

namespace Interpolation
{
	// --- CLASS DEFINITIONS <Easing> ---
	static public class Easing
	{
		// --- CONSTANT DEFINITIONS ---
		// Using at "Back" functions.
		// Backで使う定数
		const float BACK_VAL1 = 1.70158f;
		const float BACK_VAL2 = BACK_VAL1 * 1.525f;
		const float BACK_VAL3 = BACK_VAL1 + 1.0f;

		// Using at "Elastic" functions.
		// Elasticで使う定数
		const float ELASTIC_VAL1 = (2.0f * Mathf.PI) / 3.0f;
		const float ELASTIC_VAL2 = (2.0f * Mathf.PI) / 4.5f;

		// Using at "Bounce" functions.
		// Bounceで使う定数
		const float BOUNCE_MUL = 7.5625f;
		const float BOUNCE_DIV = 2.75f;
		const float BOUNCE_SEQ1_IF = 1.0f / BOUNCE_DIV;
		const float BOUNCE_SEQ2_IF = 1.0f / BOUNCE_DIV;
		const float BOUNCE_SEQ3_IF = 2.5f / BOUNCE_DIV;
		const float BOUNCE_SEQ2_SUBEQ = 1.5f / BOUNCE_DIV;
		const float BOUNCE_SEQ3_SUBEQ = 2.25f / BOUNCE_DIV;
		const float BOUNCE_SEQ4_SUBEQ = 2.625f / BOUNCE_DIV;
		const float BOUNCE_SEQ2_ADD = 0.75f;
		const float BOUNCE_SEQ3_ADD = 0.9375f;
		const float BOUNCE_SEQ4_ADD = 0.984375f;

		// Types of Ease
		// イージングの種類
		public enum Type
		{
			Linear, // Linear(Lerp) - 線形補完（一次関数）
			In,		// EaseIn - イーズイン
			Out,	// EaseOut - イーズアウト
			InOut,	// EaseInOut - イーズインアウト
		};

		// Styles of Ease
		// 補間スタイル
		public enum Style
		{
			Sine,		// Sine - 三角関数（sin,cos）
			Quad,		// Quad - 二次関数
			Cubic,		// Cubic - 三次関数
			Quart,		// Quart - 四次関数
			Quint,		// Quint - 五次関数
			Exponential,// Exponential - 指数関数
			Circular,	// Circular - 円関数
			Back,		// Back - 引き戻される動き
			Elastic,	// Elastic - バネのような動き
			Bounce,		// Bounce - バウンドする動き
		};

		// Easing Curve構造体
		// TypeとStyleをまとめたもの
		[System.Serializable]
		public struct Curve
		{
			public Type type;
			public Style style;

			// --- constructor ---
			public Curve(Type easingType,Style easingStyle)
			{
				type = easingType;
				style = easingStyle;
			}

			// --- static methods ---
			// Get Linear
			static public Curve Linear
			{
				get { return new Curve(Type.Linear,Style.Sine);}
			}

			// Get Ease-In Curve
			static public Curve EaseInSine
			{
				get { return new Curve(Type.In,Style.Sine);}
			}
			static public Curve EaseInQuad
			{
				get { return new Curve(Type.In,Style.Quad);}
			}
			static public Curve EaseInCubic
			{
				get { return new Curve(Type.In,Style.Cubic);}
			}
			static public Curve EaseInQuart
			{
				get { return new Curve(Type.In,Style.Quart);}
			}
			static public Curve EaseInQuint
			{
				get { return new Curve(Type.In,Style.Quint);}
			}
			static public Curve EaseInExpo
			{
				get { return new Curve(Type.In,Style.Exponential);}
			}
			static public Curve EaseInCirc
			{
				get { return new Curve(Type.In,Style.Circular);}
			}
			static public Curve EaseInBack
			{
				get { return new Curve(Type.In,Style.Back);}
			}
			static public Curve EaseInElastic
			{
				get { return new Curve(Type.In,Style.Elastic);}
			}
			static public Curve EaseInBounce
			{
				get { return new Curve(Type.In,Style.Bounce);}
			}

			// Get Ease-Out Curve
			static public Curve EaseOutSine
			{
				get { return new Curve(Type.Out,Style.Sine);}
			}
			static public Curve EaseOutQuad
			{
				get { return new Curve(Type.Out,Style.Quad);}
			}
			static public Curve EaseOutCubic
			{
				get { return new Curve(Type.Out,Style.Cubic);}
			}
			static public Curve EaseOutQuart
			{
				get { return new Curve(Type.Out,Style.Quart);}
			}
			static public Curve EaseOutQuint
			{
				get { return new Curve(Type.Out,Style.Quint);}
			}
			static public Curve EaseOutExpo
			{
				get { return new Curve(Type.Out,Style.Exponential);}
			}
			static public Curve EaseOutCirc
			{
				get { return new Curve(Type.Out,Style.Circular);}
			}
			static public Curve EaseOutBack
			{
				get { return new Curve(Type.Out,Style.Back);}
			}
			static public Curve EaseOutElastic
			{
				get { return new Curve(Type.Out,Style.Elastic);}
			}
			static public Curve EaseOutBounce
			{
				get { return new Curve(Type.Out,Style.Bounce);}
			}

			// Ease-InOut Curve
			static public Curve EaseInOutSine
			{
				get { return new Curve(Type.InOut,Style.Sine);}
			}
			static public Curve EaseInOutQuad
			{
				get { return new Curve(Type.InOut,Style.Quad);}
			}
			static public Curve EaseInOutCubic
			{
				get { return new Curve(Type.InOut,Style.Cubic);}
			}
			static public Curve EaseInOutQuart
			{
				get { return new Curve(Type.InOut,Style.Quart);}
			}
			static public Curve EaseInOutQuint
			{
				get { return new Curve(Type.InOut,Style.Quint);}
			}
			static public Curve EaseInOutExpo
			{
				get { return new Curve(Type.InOut,Style.Exponential);}
			}
			static public Curve EaseInOutCirc
			{
				get { return new Curve(Type.InOut,Style.Circular);}
			}
			static public Curve EaseInOutBack
			{
				get { return new Curve(Type.InOut,Style.Back);}
			}
			static public Curve EaseInOutElastic
			{
				get { return new Curve(Type.InOut,Style.Elastic);}
			}
			static public Curve EaseInOutBounce
			{
				get { return new Curve(Type.InOut,Style.Bounce);}
			}
		}

		// --- returns easing PROGRESS <float:0.0f ~ 1.0f> ---
		/*
		[引数]
		float	currentTime	現在の経過時間
		float	totalTime	イージングの総時間
		Type	type		イージングの種類
		Style	style		補間スタイル

		[戻り値]
		float	係数(0.0f ～ 1.0f)
		*/
		public static float Ease(float currentTime,float totalTime, Type type, Style style)
		{
			if(totalTime <= 0.0f) return 1.0f;

			if(currentTime > totalTime)
			{
				currentTime = totalTime;
			}

			switch (type)
			{
			case Easing.Type.Linear:
				return Linear(currentTime, totalTime);
			case Easing.Type.In:
				return EaseIn(currentTime, totalTime, style);
			case Easing.Type.Out:
				return EaseOut(currentTime, totalTime, style);
			case Easing.Type.InOut:
				return EaseInOut(currentTime, totalTime, style);
			default:
				return 0.0f;
			}
		}

		// --- returns INTERPOLATED VALUE <float> by easing progress ---
		/*
		[引数]
		float	startValue 　開始地点の値
		float	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Type	type		 イージングの種類
		Style	style		 補間スタイル

		[戻り値]
		float	イージング係数に基づいた値
		*/
		public static float Ease(float startValue,float targetValue,float currentTime, float totalTime, Type type, Style style = Style.Sine)
		{
			if(totalTime <= 0.0f) return targetValue;

			if(currentTime > totalTime)
			{
				currentTime = totalTime;
			}

			switch (type)
			{
			case Easing.Type.Linear:
				return Linear(startValue,targetValue,currentTime, totalTime);
			case Easing.Type.In:
				return EaseIn(startValue,targetValue,currentTime, totalTime, style);
			case Easing.Type.Out:
				return EaseOut(startValue, targetValue, currentTime, totalTime, style);
			case Easing.Type.InOut:
				return EaseInOut(startValue, targetValue, currentTime, totalTime, style);
			default:
				return 0.0f;
			}
		}

		// --- returns INTERPOLATED VALUE <Vector2> by easing progress ---
		/*
		[引数]
		Vector2	startValue 　開始地点の値
		Vector2	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Type	type		 イージングの種類
		Style	style		 補間スタイル

		[戻り値]
		Vector2	イージング係数に基づいた値
		*/
		public static Vector2 Ease(Vector2 startValue,Vector2 targetValue,float currentTime, float totalTime, Type type, Style style = Style.Sine)
		{
			if(totalTime <= 0.0f) return targetValue;

			if(currentTime > totalTime)
			{
				currentTime = totalTime;
			}

			Vector2 value;
			switch (type)
			{
			case Easing.Type.Linear:
				value.x = Linear(startValue.x,targetValue.x,currentTime, totalTime);
				value.y = Linear(startValue.y,targetValue.y,currentTime, totalTime);
				return value;
			case Easing.Type.In:
				value.x = EaseIn(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseIn(startValue.y,targetValue.y,currentTime, totalTime, style);
				return value;
			case Easing.Type.Out:
				value.x = EaseOut(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseOut(startValue.y,targetValue.y,currentTime, totalTime, style);
				return value;
			case Easing.Type.InOut:
				value.x = EaseInOut(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseInOut(startValue.y,targetValue.y,currentTime, totalTime, style);
				return value;
			default:
				return new Vector2(0.0f,0.0f);
			}
		}

		// --- returns INTERPOLATED VALUE <Vector3> by easing progress ---
		/*
		[引数]
		Vector3	startValue 　開始地点の値
		Vector3	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Type	type		 イージングの種類
		Style	style		 補間スタイル

		[戻り値]
		Vector3	イージング係数に基づいた値
		*/
		public static Vector3 Ease(Vector3 startValue,Vector3 targetValue,float currentTime, float totalTime, Type type, Style style = Style.Sine)
		{
			if(totalTime <= 0.0f) return targetValue;

			if(currentTime > totalTime)
			{
				currentTime = totalTime;
			}
			Vector3 value;
			switch (type)
			{
			case Easing.Type.Linear:
				value.x = Linear(startValue.x,targetValue.x,currentTime, totalTime);
				value.y = Linear(startValue.y,targetValue.y,currentTime, totalTime);
				value.z = Linear(startValue.z,targetValue.z,currentTime, totalTime);
				return value;
			case Easing.Type.In:
				value.x = EaseIn(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseIn(startValue.y,targetValue.y,currentTime, totalTime, style);
				value.z = EaseIn(startValue.z,targetValue.z,currentTime, totalTime, style);
				return value;
			case Easing.Type.Out:
				value.x = EaseOut(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseOut(startValue.y,targetValue.y,currentTime, totalTime, style);
				value.z = EaseOut(startValue.z,targetValue.z,currentTime, totalTime, style);
				return value;
			case Easing.Type.InOut:
				value.x = EaseInOut(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseInOut(startValue.y,targetValue.y,currentTime, totalTime, style);
				value.z = EaseInOut(startValue.z,targetValue.z,currentTime, totalTime, style);
				return value;
			default:
				return new Vector3(0.0f,0.0f,0.0f);
			}
		}

		// --- returns INTERPOLATED VALUE <Vector4> by easing progress ---
		/*
		[引数]
		Vector4	startValue 　開始地点の値
		Vector4	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Type	type		 イージングの種類
		Style	style		 補間スタイル

		[戻り値]
		Vector4	イージング係数に基づいた値
		*/
		public static Vector4 Ease(Vector4 startValue,Vector4 targetValue,float currentTime, float totalTime, Type type, Style style = Style.Sine)
		{
			if(totalTime <= 0.0f) return targetValue;

			if(currentTime > totalTime)
			{
				currentTime = totalTime;
			}

			Vector4 value;
			switch (type)
			{
			case Easing.Type.Linear:
				value.x = Linear(startValue.x,targetValue.x,currentTime, totalTime);
				value.y = Linear(startValue.y,targetValue.y,currentTime, totalTime);
				value.z = Linear(startValue.z,targetValue.z,currentTime, totalTime);
				value.w = Linear(startValue.w,targetValue.w,currentTime, totalTime);
				return value;
			case Easing.Type.In:
				value.x = EaseIn(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseIn(startValue.y,targetValue.y,currentTime, totalTime, style);
				value.z = EaseIn(startValue.z,targetValue.z,currentTime, totalTime, style);
				value.w = EaseIn(startValue.w,targetValue.w,currentTime, totalTime, style);
				return value;
			case Easing.Type.Out:
				value.x = EaseOut(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseOut(startValue.y,targetValue.y,currentTime, totalTime, style);
				value.z = EaseOut(startValue.z,targetValue.z,currentTime, totalTime, style);
				value.w = EaseOut(startValue.w,targetValue.w,currentTime, totalTime, style);
				return value;
			case Easing.Type.InOut:
				value.x = EaseInOut(startValue.x,targetValue.x,currentTime, totalTime, style);
				value.y = EaseInOut(startValue.y,targetValue.y,currentTime, totalTime, style);
				value.z = EaseInOut(startValue.z,targetValue.z,currentTime, totalTime, style);
				value.w = EaseInOut(startValue.w,targetValue.w,currentTime, totalTime, style);
				return value;
			default:
				return new Vector4(0.0f,0.0f,0.0f,0.0f);
			}
		}

		// --- returns INTERPOLATED VALUE <Color> by easing progress ---
		/*
		[引数]
		Color	startValue 　開始地点の値
		Color	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Type	type		 イージングの種類
		Style	style		 補間スタイル

		[戻り値]
		Color	イージング係数に基づいた色（透明度あり）
		*/
		public static Color EaseColor(Color startValue,Color targetValue,float currentTime, float totalTime, Type type, Style style = Style.Sine)
		{
			if(totalTime <= 0.0f) return targetValue;

			if(currentTime > totalTime)
			{
				currentTime = totalTime;
			}

			Color value;
			switch (type)
			{
			case Easing.Type.Linear:
				value.r = Linear(startValue.r,targetValue.r,currentTime, totalTime);
				value.g = Linear(startValue.g,targetValue.g,currentTime, totalTime);
				value.b = Linear(startValue.b,targetValue.b,currentTime, totalTime);
				value.a = Linear(startValue.a,targetValue.a,currentTime, totalTime);
				return value;
			case Easing.Type.In:
				value.r = EaseIn(startValue.r,targetValue.r,currentTime, totalTime, style);
				value.g = EaseIn(startValue.g,targetValue.g,currentTime, totalTime, style);
				value.b = EaseIn(startValue.b,targetValue.b,currentTime, totalTime, style);
				value.a = EaseIn(startValue.a,targetValue.a,currentTime, totalTime, style);
				return value;
			case Easing.Type.Out:
				value.r = EaseOut(startValue.r,targetValue.r,currentTime, totalTime, style);
				value.g = EaseOut(startValue.g,targetValue.g,currentTime, totalTime, style);
				value.b = EaseOut(startValue.b,targetValue.b,currentTime, totalTime, style);
				value.a = EaseOut(startValue.a,targetValue.a,currentTime, totalTime, style);
				return value;
			case Easing.Type.InOut:
				value.r = EaseInOut(startValue.r,targetValue.r,currentTime, totalTime, style);
				value.g = EaseInOut(startValue.g,targetValue.g,currentTime, totalTime, style);
				value.b = EaseInOut(startValue.b,targetValue.b,currentTime, totalTime, style);
				value.a = EaseInOut(startValue.a,targetValue.a,currentTime, totalTime, style);
				return value;
			default:
				return new Color(1.0f,1.0f,1.0f,1.0f);
			}
		}

		// ------ WITH Curve -------
		// --- returns easing PROGRESS <float:0.0f ~ 1.0f> ---
		/*
		[引数]
		float	currentTime	現在の経過時間
		float	totalTime	イージングの総時間
		Curve 	curve		カーブの種類

		[戻り値]
		float	係数(0.0f ～ 1.0f)
		*/
		public static float Ease(float currentTime,float totalTime, Curve curve)
		{
			return Ease(currentTime,totalTime,curve.type,curve.style);
		}

		// --- returns INTERPOLATED VALUE <float> by easing progress ---
		/*
		[引数]
		float	startValue 　開始地点の値
		float	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Curve	curve		 カーブの種類

		[戻り値]
		float	イージング係数に基づいた値
		*/
		public static float Ease(float startValue,float targetValue,float currentTime, float totalTime, Curve curve)
		{
			return Ease(startValue,targetValue,currentTime,totalTime,curve.type,curve.style);
		}

		// --- returns INTERPOLATED VALUE <Vector2> by easing progress ---
		/*
		[引数]
		Vector2	startValue 　開始地点の値
		Vector2	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Curve	curve		 カーブの種類

		[戻り値]
		Vector2	イージング係数に基づいた値
		*/
		public static Vector2 Ease(Vector2 startValue,Vector2 targetValue,float currentTime, float totalTime, Curve curve)
		{
			return Ease(startValue,targetValue,currentTime,totalTime,curve.type,curve.style);
		}

		// --- returns INTERPOLATED VALUE <Vector3> by easing progress ---
		/*
		[引数]
		Vector3	startValue 　開始地点の値
		Vector3	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Curve	curve		 カーブの種類

		[戻り値]
		Vector3	イージング係数に基づいた値
		*/
		public static Vector3 Ease(Vector3 startValue,Vector3 targetValue,float currentTime, float totalTime, Curve curve)
		{
			return Ease(startValue,targetValue,currentTime,totalTime,curve.type,curve.style);
		}

		// --- returns INTERPOLATED VALUE <Vector4> by easing progress ---
		/*
		[引数]
		Vector4	startValue 　開始地点の値
		Vector4	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Curve	curve		 カーブの種類

		[戻り値]
		Vector4	イージング係数に基づいた値
		*/
		public static Vector4 Ease(Vector4 startValue,Vector4 targetValue,float currentTime, float totalTime, Curve curve)
		{
			return Ease(startValue,targetValue,currentTime,totalTime,curve.type,curve.style);
		}

		// --- returns INTERPOLATED VALUE <Color> by easing progress ---
		/*
		[引数]
		Color	startValue 　開始地点の値
		Color	targetValue	 最終地点の値
		float	currentTime	 現在の経過時間
		float	totalTime	 イージングの総時間
		Curve	curve		 カーブの種類

		[戻り値]
		Color	イージング係数に基づいた色（透明度あり）
		*/
		public static Color EaseColor(Color startValue,Color targetValue,float currentTime, float totalTime, Curve curve)
		{
			return Ease(startValue,targetValue,currentTime,totalTime,curve.type,curve.style);
		}

		
		// --- 線形（一次関数）---
		public static float Linear(float currentTime, float totalTime)
		{
			return Mathf.Clamp01(currentTime / totalTime);
		}

		// --- イーズイン ---
		private static float EaseIn(float currentTime, float totalTime,Style style)
		{
			float progress = Mathf.Clamp01(currentTime / totalTime);

			switch (style)
			{
			case Easing.Style.Sine:
				return EaseInSine(progress);			
			case Easing.Style.Quad:
				return EaseInQuad(progress);			
			case Easing.Style.Cubic:
				return EaseInCubic(progress);			
			case Easing.Style.Quart:
				return EaseInQuart(progress);		
			case Easing.Style.Quint:
				return EaseInQuint(progress);		
			case Easing.Style.Exponential:
				return EaseInExponential(progress);
			case Easing.Style.Circular:
				return EaseInCircular(progress);	
			case Easing.Style.Back:
				return EaseInBack(progress);
			case Easing.Style.Elastic:
				return EaseInElastic(progress);
			case Easing.Style.Bounce:
				return EaseInBounce(progress);	
			default:
				return progress;		
			}
		}

		// --- イーズアウト ---
		private static float EaseOut(float currentTime, float totalTime,Style style)
		{
			float progress = Mathf.Clamp01(currentTime / totalTime);

			if (progress > 1.0f)
			{
				return 1.0f;
			}

			switch (style)
			{
			case Easing.Style.Sine:
				return EaseOutSine(progress);			
			case Easing.Style.Quad:
				return EaseOutQuad(progress);			
			case Easing.Style.Cubic:
				return EaseOutCubic(progress);			
			case Easing.Style.Quart:
				return EaseOutQuart(progress);		
			case Easing.Style.Quint:
				return EaseOutQuint(progress);		
			case Easing.Style.Exponential:
				return EaseOutExponential(progress);
			case Easing.Style.Circular:
				return EaseOutCircular(progress);	
			case Easing.Style.Back:
				return EaseOutBack(progress);
			case Easing.Style.Elastic:
				return EaseOutElastic(progress);
			case Easing.Style.Bounce:
				return EaseOutBounce(progress);	
			default:
				return progress;		
			}
		}

		// --- イーズインアウト ---
		private static float EaseInOut(float currentTime, float totalTime,Style style)
		{
			float progress = Mathf.Clamp01(currentTime / totalTime);

			if (progress > 1.0f)
			{
				return 1.0f;
			}

			switch (style)
			{
			case Easing.Style.Sine:
				return EaseInOutSine(progress);			
			case Easing.Style.Quad:
				return EaseInOutQuad(progress);			
			case Easing.Style.Cubic:
				return EaseInOutCubic(progress);			
			case Easing.Style.Quart:
				return EaseInOutQuart(progress);		
			case Easing.Style.Quint:
				return EaseInOutQuint(progress);		
			case Easing.Style.Exponential:
				return EaseInOutExponential(progress);
			case Easing.Style.Circular:
				return EaseInOutCircular(progress);	
			case Easing.Style.Back:
				return EaseInOutBack(progress);
			case Easing.Style.Elastic:
				return EaseInOutElastic(progress);
			case Easing.Style.Bounce:
				return EaseInOutBounce(progress);	
			default:
				return progress;		
			}
		}

		// --- 線形（一次関数）---
		public static float Linear(float startValue, float targetValue, float currentTime, float totalTime)
		{
			float progress = Mathf.Clamp01(currentTime / totalTime);
			float total = targetValue - startValue;

			return startValue + total * progress;
		}

		// --- イーズイン ---
		public static float EaseIn(float startValue, float targetValue, float currentTime, float totalTime, Style style)
		{
			float progress = Mathf.Clamp01(currentTime / totalTime);
			float total = targetValue - startValue;
			if (progress > 1.0f)
			{
				return targetValue;
			}

			switch (style)
			{
			case Easing.Style.Sine:
				return startValue + total * EaseInSine(progress);
			case Easing.Style.Quad:
				return startValue + total * EaseInQuad(progress);
			case Easing.Style.Cubic:
				return startValue + total * EaseInCubic(progress);
			case Easing.Style.Quart:
				return startValue + total * EaseInQuart(progress);
			case Easing.Style.Quint:
				return startValue + total * EaseInQuint(progress);
			case Easing.Style.Exponential:
				return startValue + total * EaseInExponential(progress);
			case Easing.Style.Circular:
				return startValue + total * EaseInCircular(progress);
			case Easing.Style.Back:
				return startValue + total * EaseInBack(progress);
			case Easing.Style.Elastic:
				return startValue + total * EaseInElastic(progress);
			case Easing.Style.Bounce:
				return startValue + total * EaseInBounce(progress);
			default:
				return startValue + total * progress;
			}
		}

		// --- イーズアウト ---
		public static float EaseOut(float startValue, float targetValue, float currentTime, float totalTime, Style style)
		{
			float progress = Mathf.Clamp01(currentTime / totalTime);
			float total = targetValue - startValue;
			if (progress > 1.0f)
			{
				return targetValue;
			}

			switch (style)
			{
			case Easing.Style.Sine:
				return startValue + total * EaseOutSine(progress);
			case Easing.Style.Quad:
				return startValue + total * EaseOutQuad(progress);
			case Easing.Style.Cubic:
				return startValue + total * EaseOutCubic(progress);
			case Easing.Style.Quart:
				return startValue + total * EaseOutQuart(progress);
			case Easing.Style.Quint:
				return startValue + total * EaseOutQuint(progress);
			case Easing.Style.Exponential:
				return startValue + total * EaseOutExponential(progress);
			case Easing.Style.Circular:
				return startValue + total * EaseOutCircular(progress);
			case Easing.Style.Back:
				return startValue + total * EaseOutBack(progress);
			case Easing.Style.Elastic:
				return startValue + total * EaseOutElastic(progress);
			case Easing.Style.Bounce:
				return startValue + total * EaseOutBounce(progress);
			default:
				return startValue + total * progress;
			}
		}

		// --- イーズインアウト ---
		public static float EaseInOut(float startValue,float targetValue,float currentTime, float totalTime, Style style)
		{
			float progress = Mathf.Clamp01(currentTime / totalTime);
			float total = targetValue - startValue;
			if (progress > 1.0f)
			{
				return targetValue;
			}

			switch (style)
			{
			case Easing.Style.Sine:
				return startValue + total * EaseInOutSine(progress);
			case Easing.Style.Quad:
				return startValue + total * EaseInOutQuad(progress);
			case Easing.Style.Cubic:
				return startValue + total * EaseInOutCubic(progress);
			case Easing.Style.Quart:
				return startValue + total * EaseInOutQuart(progress);
			case Easing.Style.Quint:
				return startValue + total * EaseInOutQuint(progress);
			case Easing.Style.Exponential:
				return startValue + total * EaseInOutExponential(progress);
			case Easing.Style.Circular:
				return startValue + total * EaseInOutCircular(progress);
			case Easing.Style.Back:
				return startValue + total * EaseInOutBack(progress);
			case Easing.Style.Elastic:
				return startValue + total * EaseInOutElastic(progress);
			case Easing.Style.Bounce:
				return startValue + total * EaseInOutBounce(progress);
			default:
				return startValue + total * progress;		
			}
		}


		// --- イーズイン ---

		// サイン波
		static float EaseInSine(float progress)
		{
			return 1 - Mathf.Cos((progress * Mathf.PI) / 2);
		}
		// 二次関数
		static float EaseInQuad(float progress)
		{
			return progress * progress;
		}
		// 三次関数
		static float EaseInCubic(float progress)
		{
			return progress * progress * progress;
		}
		// 四次関数
		static float EaseInQuart(float progress)
		{
			return progress * progress * progress * progress;
		}
		// 五次関数
		static float EaseInQuint(float progress)
		{
			return progress * progress * progress * progress * progress;
		}
		// 指数関数的
		static float EaseInExponential(float progress)
		{
			return progress == 0 
				? 0 
				: Mathf.Pow(2, 10 * progress - 10);
		}
		// 円関数的
		static float EaseInCircular(float progress)
		{
			return 1 - Mathf.Sqrt(1 - Mathf.Pow(progress, 2));
		}

		// 少し戻って(Back)から動く
		static float EaseInBack(float progress)
		{
			return BACK_VAL3 * progress * progress * progress - BACK_VAL1 * progress * progress;
		}
		// ばねのような動き
		static float EaseInElastic(float progress)
		{
			return progress == 0 
				? 0 
				: progress == 1 
				? 1 
				: -Mathf.Pow(2, 10 * progress - 10) * Mathf.Sin((progress * 10 - 10.75f) * ELASTIC_VAL1);
		}
		// ボールが跳ねるような動き
		static float EaseInBounce(float progress)
		{
			return 1 - EaseOutBounce(1 - progress);
		}


		// --- イーズアウト ---

		// サイン波
		static float EaseOutSine(float progress)
		{
			return Mathf.Sin((progress * Mathf.PI) / 2);
		}
		// 二次関数
		static float EaseOutQuad(float progress)
		{
			float val = (1 - progress);
			return 1 - val * val;
		}
		// 三次関数
		static float EaseOutCubic(float progress)
		{
			float val = (1 - progress);
			return 1 - val * val * val;
		}
		// 四次関数
		static float EaseOutQuart(float progress)
		{
			float val = (1 - progress);
			return 1 - val * val * val * val;
		}
		// 五次関数
		static float EaseOutQuint(float progress)
		{
			float val = (1 - progress);
			return 1 - val * val * val * val * val;
		}
		// 指数関数的
		static float EaseOutExponential(float progress)
		{
			return progress == 1 ? 1 : 1 - Mathf.Pow(2, -10 * progress);
		}
		// 円関数的
		static float EaseOutCircular(float progress)
		{
			return Mathf.Sqrt(1 - Mathf.Pow(progress - 1, 2));
		}
		// 少し戻って(Back)から動く
		static float EaseOutBack(float progress)
		{
			float val = progress - 1;
			return 1 + BACK_VAL3 * Mathf.Pow(val, 3) + BACK_VAL1 * Mathf.Pow(val, 2);
		}
		// ばねのような動き
		static float EaseOutElastic(float progress)
		{
			float val = progress;
			return val == 0 
				? 0 
				: val == 1 
				? 1
				: -Mathf.Pow(2, -10 * val) * Mathf.Sin((val * 10 - 0.75f) * ELASTIC_VAL1) + 1;
		}
		// ボールが跳ねるような動き
		static float EaseOutBounce(float progress)
		{
			float val = progress;
			if (val < BOUNCE_SEQ1_IF) {			// SEQUENCE 1
				return BOUNCE_MUL * val * val;
			}
			else if (val < BOUNCE_SEQ2_IF) {	// SEQUENCE 2
				return BOUNCE_MUL * (val -= BOUNCE_SEQ2_SUBEQ) * val + BOUNCE_SEQ2_ADD;
			}
			else if (val < BOUNCE_SEQ3_IF) {	// SEQUENCE 3
				return BOUNCE_MUL * (val -= BOUNCE_SEQ3_SUBEQ) * val + BOUNCE_SEQ3_ADD;
			}
			else {								// SEQUENCE 4
				return BOUNCE_MUL * (val -= BOUNCE_SEQ4_SUBEQ) * val + BOUNCE_SEQ4_ADD;
			}
		}


		// イーズイン・アウト

		// サイン波
		static float EaseInOutSine(float progress)
		{
			return -(Mathf.Cos(Mathf.PI * progress) - 1) / 2;
		}
		// 二次関数
		static float EaseInOutQuad(float progress)
		{
			return progress < 0.5f
				? 2 * progress * progress 
				: 1 - Mathf.Pow(-2 * progress + 2, 2) / 2;
		}
		// 三次関数
		static float EaseInOutCubic(float progress)
		{
			return progress < 0.5f
				? 4 * progress * progress * progress
				: 1 - Mathf.Pow(-2 * progress + 2, 3) / 2;
		}
		// 四次関数
		static float EaseInOutQuart(float progress)
		{
			return progress < 0.5f
				? 8 * progress * progress * progress * progress
				: 1 - Mathf.Pow(-2 * progress + 2, 4) / 2;
		}
		// 五次関数
		static float EaseInOutQuint(float progress)
		{
			return progress < 0.5f
				? 16 * progress * progress * progress * progress * progress
				: 1 - Mathf.Pow(-2 * progress + 2, 5) / 2;
		}
		// 指数関数的
		static float EaseInOutExponential(float progress)
		{
			return progress == 0
				? 0
				: progress == 1
				? 1
				: progress < 0.5f 
				? Mathf.Pow(2, 20 * progress - 10) / 2
				: (2 - Mathf.Pow(2, -20 * progress + 10)) / 2;
		}
		// 円関数的
		static float EaseInOutCircular(float progress)
		{
			return progress < 0.5f
				? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * progress, 2))) / 2
				: (Mathf.Sqrt(1 - Mathf.Pow(-2 * progress + 2, 2)) + 1) / 2;
		}

		// 少し戻って(Back)から動く
		static float EaseInOutBack(float progress)
		{
			float val = progress * 2;
			return progress < 0.5f
				? (Mathf.Pow(val, 2) * ((BACK_VAL2 + 1) * val - BACK_VAL2)) / 2
				: (Mathf.Pow(val - 2, 2) * ((BACK_VAL2 + 1) * (val - 2) + BACK_VAL2) + 2) / 2;
		}
		// ばねのような動き
		static float EaseInOutElastic(float progress)
		{
			return progress == 0
				? 0
				: progress == 1
				? 1
				: progress < 0.5f
				? -(Mathf.Pow(2, 20 * progress - 10) * Mathf.Sin((20 * progress - 11.125f) * ELASTIC_VAL2)) / 2
				: (Mathf.Pow(2, -20 * progress + 10) * Mathf.Sin((20 * progress - 11.125f) * ELASTIC_VAL2)) / 2 + 1;
		}
		// ボールが跳ねるような動き
		static float EaseInOutBounce(float progress)
		{
			return progress < 0.5f
				? (1 - EaseOutBounce(1 - 2 * progress)) / 2
				: (1 + EaseOutBounce(2 * progress - 1)) / 2;
		}
	}
}