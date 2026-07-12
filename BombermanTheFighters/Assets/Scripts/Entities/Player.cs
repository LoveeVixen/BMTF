// LOVEEVIXEN
using Fusion;
using InputSystem;
using System.Collections.Generic;
using UnityEngine;

namespace EntitySystem
{
    public class Player : Entity
    {
        private float moveSpeed = 0.15f;
        private Player opponent;
        private Health health;
        private Animator anim;
        private NetworkMecanimAnimator networkAnim;
        private enum WalkDirection { idle, forward, backward, left, right};
        private WalkDirection walkDirection = WalkDirection.idle;
        [Networked] private WalkDirection lastWalkDirection { get; set; }
        private string currentAnimationName = "Idle";
        private GameObject loadedCharacterPrefab;

        // Player state
        public enum CurrentState { idle, running, dashForward, dashBackward, dashLeft, dashRight, attacking, hit, lay, rollForward, rollBackward, rollLeft, rollRight, knockout };
        private CurrentState currentState;

        // Run and dash settings
        private float runSpeed = 0.7f;
        private float dashSpeed = 0.5f;

        [Header("Attack/Combo system")]
        [SerializeField] ComboReader comboReader = new ComboReader();
        [SerializeField] ComboGraph comboGraph;
        private float resetComboReaderTime = 0.25f;
        private float resetComboReaderTimer;
        private float attackCooldownTime = 0.2f;
        private float attackCooldownTimer;
        [SerializeField] List<ComboGraph.Branch> performedCombos = new List<ComboGraph.Branch>();
        private bool readCombos = true;
        private int inputtedCombosCount = 0;

        // Attack stroll (Player movement while attacking)
        private bool attackStroll = false;
        private float attackStrollSpeed = 0.075f;

        // On hit/taking damage.
        private Vector3 stumbleDirection = Vector3.zero;
        private float stumbleSpeed = 1f;
        private float stumbleTimer;
        private float immunityTime = 0.02f;
        private float immunityTimer = 0f;

        // Lay/roll settings.
        private float rollSpeed = 0.8f;
        private bool rollMovement = false;

        public override void OnAwake()
        {
            base.OnAwake();
            health = GetComponent<Health>();
            anim = GetComponent<Animator>();
            networkAnim = GetComponent<NetworkMecanimAnimator>();
        }

        private void Start()
        {
            // Confirm to session manager that this player has been instantiated.
            SessionManager.instance.ConfirmLoadedPlayer(this);
        }

        public override void OnTick()
        {
            base.OnTick();

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

            if (currentState == CurrentState.rollForward)
            {
                if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance() && rollMovement)
                    MoveDirection(transform.forward * rollSpeed);
            }

            if (currentState == CurrentState.rollBackward)
            {
                if (SessionManager.instance.PlayerDistance() < SessionManager.instance.GetMaxPlayerDistance() && rollMovement)
                    MoveDirection(-transform.forward * rollSpeed);
            }

            // Tick down timer until ready to reset combo reader.
            if (resetComboReaderTimer > 0f)
            {
                resetComboReaderTimer -= GetRunner().DeltaTime;
                if(resetComboReaderTimer < 0f)
                    resetComboReaderTimer = 0f;
            }

            // Once timer is at zero, reset the combo reader as soon as the player isn't in attacking state.
            if (resetComboReaderTimer == 0f && currentState != CurrentState.attacking)
                ResetComboSystem();

            // Tick down timer until player can do initiating attacks again.
            if (attackCooldownTimer > 0f && currentState != CurrentState.attacking)
            {
                attackCooldownTimer -= GetRunner().DeltaTime;
                if (attackCooldownTimer < 0f)
                    attackCooldownTimer = 0f;
            }

            // Tick down timer until player is no longer immune.
            if (immunityTimer > 0f)
            {
                immunityTimer -= GetRunner().DeltaTime;
                if(immunityTimer < 0f)
                    immunityTimer = 0f;
            }

            // Tick down timer until player is no longer stumbling.
            if (stumbleTimer > 0f)
            {
                stumbleTimer -= GetRunner().DeltaTime;
                if(stumbleTimer < 0f)
                    stumbleTimer = 0f;

                if (stumbleTimer == 0f && !IsAirborne())
                    Idle();
            }
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            // Setup animator.
            if (HasStateAuthority)
                lastWalkDirection = walkDirection;
        }

        public override void Render()
        {
            base.Render();
            anim.SetBool("Idle", lastWalkDirection == WalkDirection.idle);
            anim.SetBool("Forward", lastWalkDirection == WalkDirection.forward);
            anim.SetBool("Backward", lastWalkDirection == WalkDirection.backward);
            anim.SetBool("Left", lastWalkDirection == WalkDirection.left);
            anim.SetBool("Right", lastWalkDirection == WalkDirection.right);
        }

        public override void OnLand()
        {
            base.OnLand();
            if(currentState == CurrentState.hit)
            {
                Collapse();
            }
        }

        void MoveDirection(Vector3 direction)
        {
            NetworkTransform().Teleport(transform.position += direction);
            //SnapPosition();
        }

        // Call this at the end of each attack animation clip using the Unity animation event feature.
        public void Idle()
        {
            ResetComboSystem();
            PlayAnimation("Idle");
            attackStroll = false;
            SetCurrentState(CurrentState.idle);
            DisableAttackForAllHitboxes();
        }

        public void Lay()
        {
            ResetComboSystem();
            PlayAnimation("Lay");
            attackStroll = false;
            rollMovement = false;
            SetCurrentState(CurrentState.lay);
            DisableAttackForAllHitboxes();
        }

        // Perform an attack.
        public void ExecuteAttack(ComboGraph.Branch branch)
        {
            if (attackCooldownTimer == 0 || performedCombos.Count > 0)
            {
                // Make sure no hitboxes are already in attack mode.
                DisableAttackForAllHitboxes();

                // Prevent player from performing anymore attacks for a given amount of time.
                readCombos = false;

                // Apply attack cooldown for initiating attacks. Cooldown doesn't apply to follow up combos.
                if(!branch.attack.avoidApplyingAttackCooldown)
                    attackCooldownTimer = attackCooldownTime;

                // Play attack animation.
                bool hasAltLeftFacingAnimation = branch.attack.playLeftFacingAnimation != "";
                if (!IsFacingRight() && hasAltLeftFacingAnimation)
                    PlayAnimation(branch.attack.playLeftFacingAnimation);
                else if(branch.attack.playAnimation != "")
                    PlayAnimation(branch.attack.playAnimation);

                // Make player move while attacking.
                attackStroll = branch.attack.enableAttackStroll;  

                if (!branch.attack.avoidPlayerStateUpdate)
                    SetCurrentState(CurrentState.attacking);

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
            if (immunityTimer == 0)
            {
                SetCurrentState(CurrentState.hit);
                ApplyImmunity();

                if(currentAnimationName == "HighHit1")
                    PlayAnimation("HighHit2");
                else
                    PlayAnimation("HighHit1");
            }
        }

        public void LaunchHit()
        {
            if (immunityTimer == 0)
            {
                SetCurrentState(CurrentState.hit);
                ApplyImmunity();

                PlayAnimation("LaunchHit");
            }
        }

        public void Collapse()
        {
            PlayAnimation("Collapse");
            stumbleSpeed = 0f;
        }

        public void SetStumbleDirection(Vector3 setStumbleDirection) { stumbleDirection = setStumbleDirection; }
        public void SetStumbleSpeed(float setStumbleSpeed) {  stumbleSpeed = setStumbleSpeed; }

        #endregion

        public void AttackWithHitbox(string hitboxName)
        {
            EntityHitbox hitbox = FindHitbox(hitboxName);
            hitbox.AttackOnCollision(true);
        }

        public void SetPerformingAttackForHitboxes(Attack setAttack)
        {
            foreach (EntityHitbox hitbox in GetHitboxesList())
                hitbox.SetPerformingAttack(setAttack);
        }

        // Make sure no hitboxes are in attack mode.
        void DisableAttackForAllHitboxes()
        {
            foreach (EntityHitbox hitbox in GetHitboxesList())
                hitbox.AttackOnCollision(false);
        }

        public void StopAttackStroll()
        {
            attackStroll = false;
        }

        void ApplyImmunity()
        {
            immunityTimer = immunityTime;
        }

        public void StopMovement()
        {
            walkDirection = WalkDirection.idle;
        }

        public void MoveForward()
        {
            walkDirection = WalkDirection.forward;
            if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                MoveDirection(transform.forward * moveSpeed);
        }

        public void MoveBackward()
        {
            walkDirection = WalkDirection.backward;
            if (SessionManager.instance.PlayerDistance() < SessionManager.instance.GetMaxPlayerDistance())
                MoveDirection(-transform.forward * moveSpeed);
        }

        public void SideStepRight()
        {
            walkDirection = WalkDirection.right;
            MoveDirection(transform.right * moveSpeed);
        }

        public void SideStepLeft()
        {
            walkDirection = WalkDirection.left;
            MoveDirection(-transform.right * moveSpeed);
        }

        public void RollForward()
        {
            PlayAnimation("RollForward");
            SetCurrentState(CurrentState.rollForward);
        }

        public void RollBackward()
        {
            PlayAnimation("RollBackward");
            SetCurrentState(CurrentState.rollBackward);
        }

        public void EnableRollMovement() { rollMovement = true; }

        public void SetOpponent()
        {
            Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (Player player in players)
            {
                if (player != this)
                {
                    opponent = player;
                    return;
                }
            }
        }

        public void FaceOpponent()
        {
            transform.LookAt(opponent.Pos());
            NetworkTransform().Teleport(Pos(), Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f));
        }

        public void StartResetComboReaderTimer()
        {
            resetComboReaderTimer = resetComboReaderTime;
        }

        public void PlayAnimation(string animName)
        {
            if(HasInputAuthority)
                RPC_PlayAnimation(animName);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        void RPC_PlayAnimation(string animName)
        {
            anim.Play(animName);
            currentAnimationName = animName;
        }

        public bool IsFacingRight()
        {
            bool facingRight = false;

            if (this == SessionManager.instance.GetPlayer(0) && !SessionManager.instance.GetFlipCamera())
                facingRight = true;
            else if (this == SessionManager.instance.GetPlayer(0))
                facingRight = false;

            if (this == SessionManager.instance.GetPlayer(1) && SessionManager.instance.GetFlipCamera())
                facingRight = true;
            else if (this == SessionManager.instance.GetPlayer(1))
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

        // Load a character prefab into this player gameobject.
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_LoadCharacter(string characterName)
        {
            Character characterData = GameManager.instance.FindCharacter(characterName);

            // Clear last loaded character prefab if there is one.
            if (loadedCharacterPrefab != null)
                Destroy(loadedCharacterPrefab);

            // Instantiate new character prefab into player.
            GameObject characterObj = (GameObject)Instantiate(Resources.Load(characterData.GetCharacterPath(0)), transform.position, Quaternion.identity, transform);
            characterObj.transform.Rotate(transform.rotation.eulerAngles);
            characterObj.name = "Character";
            loadedCharacterPrefab = characterObj;
            gameObject.name = characterName;

            // Setup animator.
            anim.runtimeAnimatorController = characterData.runtimeAnimator;

            Idle();

            // Setup character hitbox.
            SetupCharacterHitbox();
        }

        void SetCurrentState(CurrentState setCurrentState)
        {
            if(HasInputAuthority)
                RPC_SetCurrentState((int)setCurrentState);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        void RPC_SetCurrentState(int setCurrentState)
        {
            currentState = (CurrentState)setCurrentState;
        }

        public Health GetHealth() { return health; }
        public CurrentState GetCurrentState() { return currentState; }
        public string GetCurrentAnimationName() { return currentAnimationName; }

        public ComboReader GetComboReader() { return comboReader; }

        public ComboGraph GetComboGraph(){ return comboGraph; }

        public void ReadCombos() { readCombos = true; }

        public bool IsReadingCombos() { return readCombos; }

        public int GetInputtedCombosCount() { return inputtedCombosCount; }

        void ResetPerformedCombosList() { performedCombos.RemoveRange(0, performedCombos.Count); }

        public List<ComboGraph.Branch> GetPerformedCombosList() { return performedCombos; }

        public ComboGraph.Branch LastPerformedCombo() { return performedCombos[performedCombos.Count - 1]; }
        public void SetStumbleTimer(float setStumbleTimer) { stumbleTimer = setStumbleTimer; }

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