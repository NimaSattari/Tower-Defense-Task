using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Neu.Animations
{
    public class DoTweenActions : MonoBehaviour
    {
        [SerializeField] AnimationType animationType = AnimationType.Move;

        #region Private Fields
        private Vector3 initalLocation;
        private Vector3 initalSize;
        private Vector3 initalRotation;
        #endregion

        #region Public Fields
        public float animationDuration;
        public float loopDelay;
        public bool doOnStart;
        public bool oneLoop;
        public bool infiniteLoop;
        public Ease animationEase = Ease.Linear;
        public Vector3 targetLocation;
        public Vector3 targetSize;
        public Vector3 targetRotation;
        #endregion

        private enum AnimationType
        {
            Move,
            Rotate,
            Scale,
            MoveAndScale,
            MoveAndRotate,
            ScaleAndRotate
        }

        #region Unity Events
        private void OnEnable()
        {
            initalLocation = transform.localPosition;
            initalSize = transform.localScale;
            initalRotation = transform.localRotation.eulerAngles;
        }

        private void OnDisable()
        {
            DOTween.Kill(transform);
            transform.localPosition = initalLocation;
            transform.localScale = initalSize;
            transform.localEulerAngles = initalRotation;
        }

        private void OnDestroy()
        {
            DOTween.Kill(transform);
        }

        private void Start()
        {
            if (doOnStart)
            {
                if (infiniteLoop)
                {
                    StartCoroutine(InfiniteLoop());
                }
                if (oneLoop)
                {
                    StartCoroutine(OneLoop());
                }
                if (!infiniteLoop || !oneLoop)
                {
                    DoAnimation();
                }
            }
        }
        #endregion

        #region Public Methods
        public void DoAnimation()
        {
            if (animationType == AnimationType.Move)
            {
                transform.DOLocalMove(targetLocation, animationDuration).SetEase(animationEase);
            }
            else if (animationType == AnimationType.Rotate)
            {
                transform.DOLocalRotate(targetRotation, animationDuration).SetEase(animationEase);
            }
            else if (animationType == AnimationType.Scale)
            {
                transform.DOScale(targetSize, animationDuration).SetEase(animationEase);
            }
            else if (animationType == AnimationType.MoveAndScale)
            {
                DOTween.Sequence().SetAutoKill(false)
                    .Append(transform.DOLocalMove(targetLocation, animationDuration).SetEase(animationEase))
                    .Join(transform.DOScale(targetSize, animationDuration).SetEase(animationEase));
            }
            else if (animationType == AnimationType.MoveAndRotate)
            {
                DOTween.Sequence().SetAutoKill(false)
                    .Append(transform.DOLocalMove(targetLocation, animationDuration).SetEase(animationEase))
                    .Join(transform.DOLocalRotate(targetRotation, animationDuration).SetEase(animationEase));
            }
            else if (animationType == AnimationType.ScaleAndRotate)
            {
                DOTween.Sequence().SetAutoKill(false)
                    .Append(transform.DOScale(targetSize, animationDuration).SetEase(animationEase))
                    .Join(transform.DOLocalRotate(targetRotation, animationDuration).SetEase(animationEase));
            }
        }

        public void DoAnimationBackward()
        {
            if (animationType == AnimationType.Move)
            {
                transform.DOLocalMove(initalLocation, animationDuration).SetEase(animationEase);
            }
            else if (animationType == AnimationType.Rotate)
            {
                transform.DOLocalRotate(initalRotation, animationDuration).SetEase(animationEase);
            }
            else if (animationType == AnimationType.Scale)
            {
                transform.DOScale(initalSize, animationDuration).SetEase(animationEase);
            }
            else if (animationType == AnimationType.MoveAndScale)
            {
                DOTween.Sequence().SetAutoKill(false)
                    .Append(transform.DOLocalMove(initalLocation, animationDuration).SetEase(animationEase))
                    .Join(transform.DOScale(initalSize, animationDuration).SetEase(animationEase));
            }
            else if (animationType == AnimationType.MoveAndRotate)
            {
                DOTween.Sequence().SetAutoKill(false)
                    .Append(transform.DOLocalMove(initalLocation, animationDuration).SetEase(animationEase))
                    .Join(transform.DOLocalRotate(initalRotation, animationDuration).SetEase(animationEase));
            }
            else if (animationType == AnimationType.ScaleAndRotate)
            {
                DOTween.Sequence().SetAutoKill(false)
                    .Append(transform.DOScale(initalSize, animationDuration).SetEase(animationEase))
                    .Join(transform.DOLocalRotate(initalRotation, animationDuration).SetEase(animationEase));
            }
        }

        public IEnumerator InfiniteLoop()
        {
            DoAnimation();
            yield return new WaitForSeconds(animationDuration);
            yield return new WaitForSeconds(loopDelay);
            DoAnimationBackward();
            yield return new WaitForSeconds(animationDuration);
            yield return new WaitForSeconds(loopDelay);
            StartCoroutine(InfiniteLoop());
        }

        public IEnumerator OneLoop()
        {
            DoAnimation();
            yield return new WaitForSeconds(animationDuration);
            yield return new WaitForSeconds(loopDelay);
            DoAnimationBackward();
            yield return new WaitForSeconds(animationDuration);
            yield return new WaitForSeconds(loopDelay);

        }
        #endregion

    }
}