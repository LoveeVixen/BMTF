// LOVEEVIXEN
using UnityEngine;

namespace Audio
{
    public class AudioObject : MonoBehaviour
    {
        private AudioSource source;
        private Transform follow;

        void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        void FixedUpdate()
        {
            if(follow != null)
                transform.position = follow.position;

            if (!source.isPlaying)
                Destroy(gameObject);
        }

        public void FollowTransform(Transform set) { follow = set; }
        public AudioSource GetSource() {  return source; }
    }
}