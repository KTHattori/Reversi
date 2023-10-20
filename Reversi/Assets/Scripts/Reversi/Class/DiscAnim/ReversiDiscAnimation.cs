using UnityEngine;
using Interpolation;

namespace Reversi
{
    public abstract class DiscAnimation
    {
        protected ReversiDisc3D _disc;

        public DiscAnimation(ReversiDisc3D disc)
        {
            _disc = disc;
        }

        public abstract void Start();
        public abstract void Update(float progress);
        public abstract void End();

        /// <summary>
        /// 跳ねる
        /// </summary>
        /// <param name="progress"></param>
        protected void Hop(float progress)
        {
            Vector3 pos = _disc.transform.position;
            pos.y = Mathf.Sin(progress * Mathf.PI) + _disc.InitPos.y;
            _disc.transform.position = pos;
        }

        /// <summary>
        /// ひっくり返る
        /// </summary>
        /// <param name="progress"></param>
        protected void Flip(float progress)
        {
            Vector3 angle = _disc.transform.localEulerAngles;
            angle.x = MathUtility.Remap(progress,0.0f,1.0f,_disc.InvertedFlipAngle,_disc.CurrentFlipAngle);
            _disc.transform.localEulerAngles = angle;
        }

        /// <summary>
        /// 落ちる
        /// </summary>
        /// <param name="progress"></param>
        protected void Fall(float progress)
        {
            Vector3 pos = _disc.transform.position;
            pos.y = (1.0f - progress) * 2.0f + _disc.InitPos.y;
            _disc.transform.position = pos;
        }

        /// <summary>
        /// 大きさ
        /// </summary>
        /// <param name="progress"></param>
        protected void Scale(float progress)
        {
            _disc.transform.localScale = progress * _disc.InitScale;
        }

        /// <summary>
        /// 上に昇っていく
        /// </summary>
        /// <param name="progress"></param>
        protected void Rise(float progress)
        {
            Vector3 pos = _disc.transform.position;
            pos.y = progress * 2.0f + _disc.InitPos.y;
            _disc.transform.position = pos;
        }
    }

    public class PlaceAnimation : DiscAnimation
    {
        public PlaceAnimation(ReversiDisc3D disc) : base(disc)
        {
            
        }
        public override void Start()
        {
            Fall(0.0f);
            Flip(1.0f);
        }
        public override void Update(float progress)
        {
            Fall(Easing.Ease(progress,1.0f,Easing.Curve.EaseInQuint));
        }
        public override void End()
        {
            Fall(1.0f);
        }
    }

    public class RecallAnimation : DiscAnimation
    {
        public RecallAnimation(ReversiDisc3D disc) : base(disc)
        {
            
        }
        public override void Start()
        {
            Rise(0.0f);
        }
        public override void Update(float progress)
        {
            Rise(Easing.Ease(progress,1.0f,Easing.Curve.EaseInQuint));
        }
        public override void End()
        {
            Rise(1.0f);
            _disc.gameObject.SetActive(false);
        }
    }

    public class FlipAnimation : DiscAnimation
    {
        public FlipAnimation(ReversiDisc3D disc) : base(disc)
        {
            
        }
        public override void Start()
        {
            Flip(0.0f);
            Hop(0.0f);
        }
        public override void Update(float progress)
        {
            Flip(Easing.Ease(progress,1.0f,_disc.Settings.FlipEase));
            Hop(Easing.Ease(progress,1.0f,_disc.Settings.HopEase));
        }
        public override void End()
        {
            Flip(1.0f);
            Hop(1.0f);
        }
    }
}
