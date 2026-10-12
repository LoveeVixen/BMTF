// LOVEEVIXEN
using EntitySystem;
using InputSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Frame
{
    public PlayerInputData player1Input = new PlayerInputData();
    public PlayerInputData player2Input = new PlayerInputData();
    public Player player1;
    public Player player2;

    public float frameTime;

    [System.Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;

        public TransformData(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            this.position = position;
            this.rotation = rotation;
            this.localScale = localScale;
        }
    }

    [System.Serializable]
    public class ReplayDummyFrameData
    {
        [SerializeField] TransformData playerTransform = new TransformData(Vector3.zero, Quaternion.identity, Vector3.zero);
        [SerializeField] string animationName = "Idle";
        [SerializeField] float animationFrameTime = 0f;

        public TransformData GetPlayerTransform() {  return playerTransform; }
        public string GetAnimationName() { return animationName; }
        public float GetAnimationFrameTime() { return animationFrameTime; }

        public ReplayDummyFrameData(Player recordPlayerTransform)
        {
            this.playerTransform.position = recordPlayerTransform.transform.position;
            this.playerTransform.rotation = recordPlayerTransform.transform.rotation;
            this.playerTransform.localScale = recordPlayerTransform.transform.localScale;
            this.animationName = recordPlayerTransform.GetCurrentAnimationName();

            AnimatorStateInfo stateInfo = recordPlayerTransform.GetAnim().GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfo = recordPlayerTransform.GetAnim().GetCurrentAnimatorClipInfo(0);
            if(clipInfo.Length > 0)
            {
                AnimationClip currentClip = clipInfo[0].clip;
                float normalizedTime = stateInfo.normalizedTime % 1f;
                float currentClipTime = normalizedTime * currentClip.length;
                this.animationFrameTime = currentClipTime;
            }
        }
    }
    public List<ReplayDummyFrameData> replayDummyFrameData = new List<ReplayDummyFrameData>();
}
