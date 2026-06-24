// LOVEEVIXEN
using InputSystem;
using System.Collections.Generic;
using UnityEngine;

namespace EntitySystem
{
    public class Player : Entity
    {
        private const float moveSpeed = 0.15f;
        private Player opponent;
        private Health health;
        private Animator anim;
        private string currentAnimationName = "Idle";

        // Player state
        public enum CurrentState { idle, running, dashForward, dashBackward, dashLeft, dashRight, attacking, hit, lay, knockout };
        private CurrentState currentState;

        // Run and dash settings
        private const float runSpeed = 0.6f;
        private const float dashSpeed = 0.5f;

        [Header("Attack/Combo system")]
        [SerializeField] ComboReader comboReader = new ComboReader();
        [SerializeField] ComboGraph comboGraph;
        private int resetComboReaderSet = 20;
        private int framesUntilResetComboReader;
        private int attackCooldownFramesSet = 4;
        private int attackCooldownFrames;
        [SerializeField] List<ComboGraph.Branch> performedCombos = new List<ComboGraph.Branch>();
        private bool readCombos = true;
        private int inputtedCombosCount = 0;

        // Attack stroll (Player movement while attacking)
        private bool attackStroll = false;
        private const float attackStrollSpeed = 0.075f;

        // On hit/taking damage.
        private Vector3 stumbleDirection = Vector3.zero;
        private float stumbleSpeed = 1f;
        private int stumbleFrames;
        private int immunityFrames = 0;
        private int immunityFramesSet = 10;

        public override void OnAwake()
        {
            base.OnAwake();
            health = GetComponent<Health>();
            anim = GetComponent<Animator>();
        }

        private void Start()
        {
            if (this == SessionManager.instance.GetPlayer1())
                opponent = SessionManager.instance.GetPlayer2();
            else
                opponent = SessionManager.instance.GetPlayer1();
        }

        public void OnTick()
        {
            if (currentState == CurrentState.running)
            {
                // Make player run to it's opponent until close enough.
                if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                    MoveDirection(transform.forward * runSpeed);
                else
                    Idle();
            }

            if (currentState == CurrentState.dashForward)
            {
                if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                    MoveDirection(transform.forward * dashSpeed);
            }

            if (currentState == CurrentState.dashBackward)
            {
                if (SessionManager.instance.PlayerDistance() < SessionManager.instance.GetMaxPlayerDistance())
                    MoveDirection(-transform.forward * dashSpeed);
            }

            if (currentState == CurrentState.dashLeft)
                MoveDirection(-transform.right * dashSpeed);

            if (currentState == CurrentState.dashRight)
                MoveDirection(transform.right * dashSpeed);

            if(currentState == CurrentState.attacking && attackStroll && SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                MoveDirection(transform.forward * attackStrollSpeed);

            if(currentState == CurrentState.hit)
                MoveDirection(stumbleDirection * stumbleSpeed);

            // Tick down timer until ready to reset combo reader.
            if (framesUntilResetComboReader > 0)
                framesUntilResetComboReader--;

            // Once timer is at zero, reset the combo reader as soon as the player isn't in attacking state.
            if (framesUntilResetComboReader == 0 && currentState != CurrentState.attacking)
                ResetComboSystem();

            // Tick down timer until player can do initiating attacks again.
            if (attackCooldownFrames > 0 && currentState != CurrentState.attacking)
                attackCooldownFrames--;

            // Tick down timer until player is no longer immune.
            if (immunityFrames > 0)
                immunityFrames--;

            // Tick down timer until player is no longer stumbling.
            if (stumbleFrames > 0)
            {
                stumbleFrames--;
                if (stumbleFrames == 0)
                    Idle();
            }
        }

        void MoveDirection(Vector3 direction)
        {
            transform.position += direction;
            SnapPosition();
        }

        // Call this at the end of each attack animation clip using the Unity animation event feature.
        public void Idle()
        {
            ResetComboSystem();
            PlayAnimation("Idle");
            attackStroll = false;
            currentState = CurrentState.idle;
            DisableAttackForAllHitboxes();
        }

        // Perform an attack.
        public void ExecuteAttack(ComboGraph.Branch branch)
        {
            if (attackCooldownFrames == 0 || performedCombos.Count > 0)
            {
                // Make sure no hitboxes are already in attack mode.
                DisableAttackForAllHitboxes();

                // Prevent player from performing anymore attacks for a given amount of frames.
                readCombos = false;

                // Apply attack cooldown for initiating attacks. Cooldown doesn't apply to follow up combos.
                if(!branch.attack.avoidApplyingAttackCooldown)
                    attackCooldownFrames = attackCooldownFramesSet;

                // Play attack animation.
                bool hasAltLeftFacingAnimation = branch.attack.playLeftFacingAnimation != "";
                if (!IsFacingRight() && hasAltLeftFacingAnimation)
                    PlayAnimation(branch.attack.playLeftFacingAnimation);
                else if(branch.attack.playAnimation != "")
                    PlayAnimation(branch.attack.playAnimation);

                // Make player move while attacking.
                attackStroll = branch.attack.enableAttackStroll;  

                if (!branch.attack.avoidPlayerStateUpdate)
                    currentState = CurrentState.attacking;

                // Pass attack data to hitboxes about to enable attack mode.
                SetPerformingAttackForHitboxes(branch.attack);

                // Record combo data.
                performedCombos.Add(branch);
                inputtedCombosCount++;
            }
        }

        // Player on-hit stumble types.
        #region
        public void HighHit()
        {
            if (immunityFrames == 0)
            {
                currentState = CurrentState.hit;
                ApplyImmunity();

                if(currentAnimationName == "HighHit1")
                    PlayAnimation("HighHit2");
                else
                    PlayAnimation("HighHit1");
            }
        }

        public void SetStumbleDirection(Vector3 setStumbleDirection) { stumbleDirection = setStumbleDirection; }
        public void SetStumbleSpeed(float setStumbleSpeed) {  stumbleSpeed = setStumbleSpeed; }

        #endregion

        public void AttackWithHitbox(string hitboxName)
        {
            Hitbox hitbox = FindHitbox(hitboxName);
            hitbox.AttackOnCollision(true);
        }

        public void SetPerformingAttackForHitboxes(Attack setAttack)
        {
            foreach (Hitbox hitbox in GetHitboxesList())
                hitbox.SetPerformingAttack(setAttack);
        }

        // Make sure no hitboxes are in attack mode.
        void DisableAttackForAllHitboxes()
        {
            foreach (Hitbox hitbox in GetHitboxesList())
                hitbox.AttackOnCollision(false);
        }

        public void StopAttackStroll()
        {
            attackStroll = false;
        }

        void ApplyImmunity()
        {
            immunityFrames = immunityFramesSet;
        }

        public void MoveForward()
        {
            if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                MoveDirection(transform.forward * moveSpeed);
        }

        public void MoveBackward()
        {
            if (SessionManager.instance.PlayerDistance() < SessionManager.instance.GetMaxPlayerDistance())
                MoveDirection(-transform.forward * moveSpeed);
        }

        public void SideStepRight()
        {
            MoveDirection(transform.right * moveSpeed);
        }

        public void SideStepLeft()
        {
            MoveDirection(-transform.right * moveSpeed);
        }

        public void FaceOpponent()
        {
            transform.LookAt(opponent.Pos());
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        public void StartResetComboReaderTimer()
        {
            framesUntilResetComboReader = resetComboReaderSet;
        }

        public void PlayAnimation(string animName)
        {
            anim.Play(animName);
            currentAnimationName = animName;
        }

        public bool IsFacingRight()
        {
            bool facingRight = false;

            if (this == SessionManager.instance.GetPlayer1() && !SessionManager.instance.GetFlipCamera())
                facingRight = true;
            else if (this == SessionManager.instance.GetPlayer1())
                facingRight = false;

            if (this == SessionManager.instance.GetPlayer2() && SessionManager.instance.GetFlipCamera())
                facingRight = true;
            else if (this == SessionManager.instance.GetPlayer2())
                facingRight = false;

            return facingRight;
        }

        public void ResetComboSystem()
        {
            comboReader.Reset();
            inputtedCombosCount = 0;
            ResetPerformedCombosList();
            readCombos = true;
        }

        public Health GetHealth() { return health; }
        public CurrentState GetCurrentState() { return currentState; }

        public ComboReader GetComboReader() { return comboReader; }

        public ComboGraph GetComboGraph(){ return comboGraph; }

        public void ReadCombos() { readCombos = true; }

        public bool IsReadingCombos() { return readCombos; }

        public int GetInputtedCombosCount() { return inputtedCombosCount; }

        void ResetPerformedCombosList() { performedCombos.RemoveRange(0, performedCombos.Count); }

        public List<ComboGraph.Branch> GetPerformedCombosList() { return performedCombos; }

        public ComboGraph.Branch LastPerformedCombo() { return performedCombos[performedCombos.Count - 1]; }
        public void SetStumbleFrames(int setStumbleFrames) { stumbleFrames = setStumbleFrames; }

        // Universal character attacks/abilities
        #region
        public void Run()
        {
            PlayAnimation("Run");
            currentState = CurrentState.running;
            ResetComboSystem();
        }

        public void DashForward()
        {
            currentState = CurrentState.dashForward;
        }

        public void DashBackward()
        {
            currentState = CurrentState.dashBackward;
        }

        public void DashLeft()
        {
            PlayAnimation("DashLeft");
            currentState = CurrentState.dashLeft;
        }

        public void DashRight()
        {
            PlayAnimation("DashRight");
            currentState = CurrentState.dashRight;
        }
        #endregion
    }
}