// LOVEEVIXEN
using UnityEngine;
using Photon.Pun;

namespace EntitySystem
{
    public enum FacialExpression { normal, happy, angry, hurt, dizzy };

    public class Face : MonoBehaviourPunCallbacks
    {
        private FacialExpression expression = FacialExpression.normal;
        private Character character;
        private MeshRenderer render;

        private void Awake()
        {
            render = GetComponent<MeshRenderer>();
        }

        public void SetCharacter(Character set)
        {
            character = set;
        }

        public void SetExpression(FacialExpression set)
        {
            expression = (FacialExpression)set;
            if (expression == FacialExpression.normal)
                render.material = character.normalFace;
            else if (expression == FacialExpression.happy)
                render.material = character.happyFace;
            else if (expression == FacialExpression.angry)
                render.material = character.angryFace;
            else if (expression == FacialExpression.hurt)
                render.material = character.hurtFace;
            else if (expression == FacialExpression.dizzy)
                render.material = character.dizzyFace;
        }

        public FacialExpression GetExpression() {  return expression; }
    }
}