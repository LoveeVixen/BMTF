// LOVEEVIXEN
using UnityEngine;

namespace EntitySystem
{
    public class DarknessSpotlight : MonoBehaviour
    {
        private Entity followEntity;
        [SerializeField] float yPos = 10f;

        // Update is called once per frame
        void Update()
        {
            if (followEntity != null) transform.position = new Vector3(followEntity.Pos().x, yPos, followEntity.Pos().z);
        }

        public void FollowEntity(Entity set)
        {
            followEntity = set;
        }
    }
}