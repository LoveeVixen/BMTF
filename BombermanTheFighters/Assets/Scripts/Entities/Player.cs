// LOVEEVIXEN
using Audio;
using InputSystem;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace EntitySystem
{
    public class Player : Entity
    {
        [Header("Movement Speed")]
        [SerializeField] float moveSpeed = 0.15f;
        [SerializeField] float runSpeed = 0.7f;
        [SerializeField] float dashSpeed = 0.5f;
        [SerializeField] float attackStrollSpeed = 1f;
        [SerializeField] float rollSpeed = 0.8f;
        private bool rollMovement = false;
        private bool halfWaveDashMovement = false;

        private Player opponent;
        private Animator anim;
        private PlayerInputData lastInputData;
        private enum WalkDirection { idle, forward, backward, left, right};
        private WalkDirection walkDirection = WalkDirection.idle;
        private string currentAnimationName = "Idle";
        private GameObject loadedCharacterPrefab;
        private Character loadedCharacter;

        // Player state
        public enum CurrentState { idle, running, dashForward, dashBackward, dashLeft, dashRight, waveDash, attacking, hit, lay, rollForward, rollBackward, rollLeft, rollRight };
        private CurrentState currentState;

        [Header("Attack/Combo system")]
        [SerializeField] ComboReader comboReader = new ComboReader();
        [SerializeField] ComboGraph comboGraph;
        [SerializeField] float resetComboReaderTime = 0.3f;
        private float resetComboReaderTimer;
        [SerializeField] float attackCooldownTime = 0.2f;
        private float attackCooldownTimer;
        [SerializeField] List<ComboGraph.Branch> performedCombos = new List<ComboGraph.Branch>();
        private bool readCombos = true;
        private int inputtedCombosCount = 0;
        private Transform weldingLimb;
        private Bomb weldBomb;

        // Attack stroll (Player movement while attacking)
        private bool attackStroll = false;

        // On hit/taking damage.
        private Vector3 stumbleDirection = Vector3.zero;
        private float stumbleSpeed = 1f;
        private float stumbleTimer;
        [SerializeField] float immunityTime = 0.02f;
        private float immunityTimer = 0f;

        // Combining attack inputs.
        private enum CombineDirection { none, up, upRight, right, downRight, down, downLeft, left, upLeft };
        private CombineDirection combineDirection = CombineDirection.none;
        private bool combine0 = false;
        private bool combine1 = false;
        private bool combine2 = false;
        private bool combine3 = false;
        private float combineInputTime = 0.02f;
        private float combineInputTimer = 0f;

        public override void OnAwake()
        {
            base.OnAwake();
            anim = GetComponent<Animator>();
            combineInputTimer = combineInputTime;
        }

        private void Start()
        {
            // Confirm to session manager that this player has been instantiated.
            SessionManager.instance.ConfirmLoadedPlayer(this);
        }

        public override void Output()
        {
            // Record the player's inputs on the combo reader.
            PlayerInputData inputData = lastInputData;
            ComboInputData[] comboInputs = comboReader.inputs.ToArray();
            int comboInputsCount = comboInputs.Length;
            int recentIndex = comboReader.RecentIndex();

            // Movement input while standing.
            if (currentState == CurrentState.idle)
            {
                if (IsFacingRight())
                {
                    if (inputData.holdingLeft)
                        MoveBackward();
                    else if (inputData.holdingRight)
                        MoveForward();
                    else if (inputData.holdingUp)
                        SideStepLeft();
                    else if (inputData.holdingDown)
                        SideStepRight();
                    else
                        StopMovement();
                }
                else
                {
                    if (inputData.holdingLeft)
                        MoveForward();
                    else if (inputData.holdingRight)
                        MoveBackward();
                    else if (inputData.holdingUp)
                        SideStepRight();
                    else if (inputData.holdingDown)
                        SideStepLeft();
                    else
                        StopMovement();
                }
            }

            // Movement input while laying.
            if (currentState == CurrentState.lay)
            {
                if (IsFacingRight())
                {
                    // Prevent player from getting back up if they're knocked out.
                    if (!GetHealth().IsKnockedOut())
                    {
                        if (inputData.holdingLeft)
                            RollBackward();
                        else if (inputData.holdingRight)
                            RollForward();
                    }
                }
                else
                {
                    // Prevent player from getting back up if they're knocked out.
                    if (!GetHealth().IsKnockedOut())
                    {
                        if (inputData.holdingLeft)
                            RollForward();
                        else if (inputData.holdingRight)
                            RollBackward();
                    }
                }
            }

            // Record player inputs on combo reader to later see if player has successfully done any combos.
            if (inputData.PressingInputCount() > 0 && !inputData.pressingStart && !inputData.pressingSelect)
            {
                // Begin timer until combined attack inputs are applied.
                if(combineInputTimer == 0f) combineInputTimer = combineInputTime;

                if (inputData.pressingUp) combineDirection = CombineDirection.up;
                else if (inputData.pressingRight) combineDirection = CombineDirection.right;
                else if (inputData.pressingDown) combineDirection = CombineDirection.down;
                else if (inputData.pressingLeft) combineDirection = CombineDirection.left;
                else if (inputData.pressingUpRight) combineDirection = CombineDirection.upRight;
                else if (inputData.pressingDownRight) combineDirection = CombineDirection.downRight;
                else if (inputData.pressingDownLeft) combineDirection = CombineDirection.downLeft;
                else if (inputData.pressingUpLeft) combineDirection = CombineDirection.upLeft;

                if (inputData.pressing0) combine0 = true;
                if (inputData.pressing1) combine1 = true;
                if (inputData.pressing2) combine2 = true;
                if (inputData.pressing3) combine3 = true;
            }

            if (comboInputs.Length > 0)
            {
                // Cancel running state.
                if (currentState == CurrentState.running)
                {
                    if (comboInputs[recentIndex].inputDirection == ComboInputData.InputDirection.backward || comboInputs[recentIndex].inputDirection == ComboInputData.InputDirection.up || comboInputs[recentIndex].inputDirection == ComboInputData.InputDirection.down)
                        Idle();
                }
            }

            // Check to see if player is in a state that can transition to attack state.
            bool readyToAttack = false;
            switch (currentState)
            {
                case CurrentState.idle: readyToAttack = true; break;
                case CurrentState.running: readyToAttack = true; break;
                case CurrentState.waveDash: readyToAttack = true; break;
                case CurrentState.dashForward: readyToAttack = true; break;
                case CurrentState.dashBackward: readyToAttack = true; break;
                case CurrentState.dashLeft: readyToAttack = true; break;
                case CurrentState.dashRight: readyToAttack = true; break;
            }

            // Execute combo moves if combo is successful.
            if (IsReadingCombos())
            {
                // Combo initiators.
                if (GetInputtedCombosCount() == 0 && readyToAttack)
                {
                    foreach (ComboGraph.Branch branch in comboGraph.branches)
                    {
                        if (branch.attack.MatchesRequiredInputs(comboReader.inputs) && MatchesRequiredStateForAttack(branch.attack))
                            ExecuteAttack(branch);
                    }
                }
                else
                {
                    // Follow up combos.
                    int playerPerformedCombosCount = performedCombos.Count;
                    if (playerPerformedCombosCount > 0)
                    {
                        foreach (ComboGraph.Branch branch in performedCombos[playerPerformedCombosCount - 1].followUpCombos)
                        {
                            if (branch.attack.MatchesRequiredInputs(comboReader.inputs) && MatchesRequiredStateForAttack(branch.attack))
                                ExecuteAttack(branch);
                        }
                    }
                }
            }

            //  Determine if player should face it's opponent depending on it's current state.
            bool faceOpponent = false;
            switch (currentState)
            {
                case CurrentState.idle: faceOpponent = true; break;
                case CurrentState.running: faceOpponent = true; break;
            }

            if (faceOpponent)
                FaceOpponent();
        }

        public void OutputInputData(PlayerInputData inputData)
        {
            if(!GetHealth().IsKnockedOut())
                lastInputData = inputData;
        }

        public override void OnTick()
        {
            base.OnTick();

            if (photonView.IsMine)
            {
                // Player output movement.
                if (currentState == CurrentState.running)
                {
                    // Make player run to it's opponent until close enough.
                    if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                        MoveDirection(transform.forward * runSpeed);
                    else
                        Idle();
                }

                float calcDashSpeed = dashSpeed;
                if (currentState == CurrentState.waveDash)
                {
                    if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                    {
                        if (halfWaveDashMovement) calcDashSpeed /= 2f;
                        MoveDirection(transform.forward * calcDashSpeed);
                    }
                }

                if (currentState == CurrentState.dashForward)
                {
                    if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                        MoveDirection(transform.forward * calcDashSpeed);
                }

                if (currentState == CurrentState.dashBackward)
                {
                    if (SessionManager.instance.PlayerDistance() < SessionManager.instance.GetMaxPlayerDistance())
                        MoveDirection(-transform.forward * calcDashSpeed);
                }

                if (currentState == CurrentState.dashLeft)
                    MoveDirection(-transform.right * calcDashSpeed);

                if (currentState == CurrentState.dashRight)
                    MoveDirection(transform.right * calcDashSpeed);

                if (currentState == CurrentState.attacking && attackStroll && SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                    MoveDirection(transform.forward * attackStrollSpeed);

                if (currentState == CurrentState.hit)
                {
                    if (weldBomb != null) DropBomb();
                    MoveDirection(stumbleDirection * stumbleSpeed);
                }

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

                // Make sure player is not standing when they're KO.
                if(GetHealth().IsKnockedOut())
                {
                    if (weldBomb != null) DropBomb();

                    if (currentState == CurrentState.idle)
                        DizzyFall();
                }

                // Tick down attack input combine timer until it reaches zero.
                if (SessionManager.instance.HasRoundBegun())
                {
                    if (combineInputTimer > 0f)
                    {
                        combineInputTimer -= Time.deltaTime;
                        if (combineInputTimer < 0f)
                            combineInputTimer = 0f;
                    }

                    if (combineInputTimer == 0f)
                    {
                        PlayerInputData combinedInputData = PlayerInputData.CloneData(lastInputData);
                        combinedInputData.pressingUp = combineDirection == CombineDirection.up;
                        combinedInputData.pressingRight = combineDirection == CombineDirection.right;
                        combinedInputData.pressingDown = combineDirection == CombineDirection.down;
                        combinedInputData.pressingLeft = combineDirection == CombineDirection.left;
                        combinedInputData.pressingUpRight = combineDirection == CombineDirection.upRight;
                        combinedInputData.pressingDownRight = combineDirection == CombineDirection.downRight;
                        combinedInputData.pressingDownLeft = combineDirection == CombineDirection.downLeft;
                        combinedInputData.pressingUpLeft = combineDirection == CombineDirection.upLeft;
                        combinedInputData.pressing0 = combine0;
                        combinedInputData.pressing1 = combine1;
                        combinedInputData.pressing2 = combine2;
                        combinedInputData.pressing3 = combine3;

                        if (combinedInputData.PressingInputCount() > 0 && !combinedInputData.pressingStart && !combinedInputData.pressingSelect)
                        {
                            comboReader.inputs.Add(new ComboInputData(combinedInputData, IsFacingRight()));
                            ResetComboReaderTimer();
                        }
                    }
                }
            }

            if(weldBomb != null && weldBomb.photonView.IsMine)
                weldBomb.MoveTo(weldingLimb.position);

            // Tick down timer until ready to reset combo reader.
            if (resetComboReaderTimer > 0f)
            {
                resetComboReaderTimer -= Time.deltaTime;
                if(resetComboReaderTimer < 0f)
                    resetComboReaderTimer = 0f;
            }

            // Once timer is at zero, reset the combo reader as soon as the player isn't in attacking state.
            if (resetComboReaderTimer == 0f && currentState != CurrentState.attacking)
                ResetComboSystem();

            // Tick down timer until player can do initiating attacks again.
            if (attackCooldownTimer > 0f && currentState != CurrentState.attacking)
            {
                attackCooldownTimer -= Time.deltaTime;
                if (attackCooldownTimer < 0f)
                    attackCooldownTimer = 0f;
            }

            // Tick down timer until player is no longer immune.
            if (immunityTimer > 0f)
            {
                immunityTimer -= Time.deltaTime;
                if(immunityTimer < 0f)
                    immunityTimer = 0f;
            }

            // Tick down timer until player is no longer stumbling.
            if (stumbleTimer > 0f)
            {
                stumbleTimer -= Time.deltaTime;
                if(stumbleTimer < 0f)
                    stumbleTimer = 0f;

                if (stumbleTimer == 0f && !IsAirborne())
                    Idle();
            }

            // Setup animator.
            anim.SetBool("Idle", walkDirection == WalkDirection.idle);
            anim.SetBool("Forward", walkDirection == WalkDirection.forward);
            anim.SetBool("Backward", walkDirection == WalkDirection.backward);
            anim.SetBool("Left", walkDirection == WalkDirection.left);
            anim.SetBool("Right", walkDirection == WalkDirection.right);
        }

        public override void OnLand()
        {
            base.OnLand();
            if(currentState == CurrentState.hit)
                Collapse();
        }

        // Entity sound methods.
        #region

        // Play a sound, treated as a voice from the entity.
        public void PlayVoice(string soundName)
        {
            photonView.RPC("RPC_PlayVoice", RpcTarget.All, soundName);
        }

        [PunRPC]
        void RPC_PlayVoice(string soundName)
        {
            SoundProperties properties = new SoundProperties();
            properties.follow = transform;
            AudioManager.instance.PlaySound(soundName, Pos(), properties);
        }
        #endregion

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
                attackStrollSpeed = branch.attack.attackStrollSpeed;

                // Play attack sound.
                if(branch.attack.playSoundOnExecute == Attack.PlaySoundOnExecute.playCharacterAttack)
                    PlayVoice(loadedCharacter.attackSound);
                else if (branch.attack.playSoundOnExecute == Attack.PlaySoundOnExecute.playCharacterSpecial)
                    PlayVoice(loadedCharacter.specialSound);
                else if(branch.attack.playSoundOnExecute == Attack.PlaySoundOnExecute.playOverrideSound)
                    PlayVoice(branch.attack.playOverrideSound);

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

        public void DizzyFall()
        {
            SetCurrentState(CurrentState.hit);
            PlayAnimation("DizzyFall");
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
        public void ReduceWaveDashMovement() {  halfWaveDashMovement = true; }

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
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        public void ResetComboReaderTimer()
        {
            resetComboReaderTimer = resetComboReaderTime;
            combineDirection = CombineDirection.none;
            combine0 = false;
            combine1 = false;
            combine2 = false;
            combine3 = false;
        }

        public void PlayAnimation(string animName)
        {
            if (photonView.IsMine)
                photonView.RPC("RPC_PlayAnimation", RpcTarget.All, animName);
        }

        [PunRPC]
        void RPC_PlayAnimation(string animName)
        {
            anim.Play(animName);
            currentAnimationName = animName;
        }

        public bool IsFacingRight()
        {
            bool facingRight = false;

            if (this == SessionManager.instance.GetParticipate(0).GetPlayer() && !SessionManager.instance.GetFlipCamera())
                facingRight = true;
            else if (this == SessionManager.instance.GetParticipate(0).GetPlayer())
                facingRight = false;

            if (this == SessionManager.instance.GetParticipate(1).GetPlayer() && SessionManager.instance.GetFlipCamera())
                facingRight = true;
            else if (this == SessionManager.instance.GetParticipate(1).GetPlayer())
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
        public void LoadCharacter(string characterName)
        {
            if (photonView.IsMine)
                photonView.RPC("RPC_LoadCharacter", RpcTarget.All, characterName);
        }

        [PunRPC]
        void RPC_LoadCharacter(string characterName)
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
            loadedCharacter = characterData;
            gameObject.name = characterName;

            // Setup animator.
            anim.runtimeAnimatorController = characterData.runtimeAnimator;
            PhotonAnimatorView animView = GetComponent<PhotonAnimatorView>();

            for (int i = 0; i < animView.GetSynchronizedParameters().Count; i++)
                animView.GetSynchronizedParameters()[i].SynchronizeType = PhotonAnimatorView.SynchronizeType.Continuous;

            Idle();

            // Setup character hitbox.
            SetupCharacterHitbox();

            // Setup welding limb for where held weapons will be positioned.
            weldingLimb = FindHitbox("RightHand").transform;
        }

        void SetCurrentState(CurrentState setCurrentState)
        {
            if(photonView.IsMine)
                RPC_SetCurrentState((int)setCurrentState);
        }

        [PunRPC]
        void RPC_SetCurrentState(int setCurrentState)
        {
            currentState = (CurrentState)setCurrentState;
        }

        bool MatchesRequiredStateForAttack(Attack attack)
        {
            if (attack.requiredState == Attack.RequiredState.none)
                return true;
            else
            {
                if (currentState == CurrentState.idle && attack.requiredState == Attack.RequiredState.idle)
                    return true;
                else if (currentState == CurrentState.dashForward && attack.requiredState == Attack.RequiredState.dashForward)
                    return true;
                else if (currentState == CurrentState.dashBackward && attack.requiredState == Attack.RequiredState.dashBackward)
                    return true;
                else if (currentState == CurrentState.dashLeft && attack.requiredState == Attack.RequiredState.dashLeft)
                    return true;
                else if (currentState == CurrentState.dashRight && attack.requiredState == Attack.RequiredState.dashRight)
                    return true;
                else if (currentState == CurrentState.waveDash && attack.requiredState == Attack.RequiredState.waveDash)
                    return true;
                else if (weldBomb != null && attack.requiredState == Attack.RequiredState.weldBomb)
                    return true;
                else if (weldBomb == null && attack.requiredState == Attack.RequiredState.notWeldBomb)
                    return true;
            }

            return false;
        }

        public CurrentState GetCurrentState() { return currentState; }
        public string GetCurrentAnimationName() { return currentAnimationName; }

        public ComboReader GetComboReader() { return comboReader; }

        public ComboGraph GetComboGraph(){ return comboGraph; }
        public Character GetLoadedCharacter() {  return loadedCharacter; }

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
            SetCurrentState(CurrentState.running);
            ResetComboSystem();
        }

        public void WaveDash()
        {
            SetCurrentState(CurrentState.waveDash);
            halfWaveDashMovement = false;
            ResetComboSystem();
        }

        public void DashForward()
        {
            SetCurrentState(CurrentState.dashForward);
            ResetComboSystem();
        }

        public void DashBackward()
        {
            SetCurrentState(CurrentState.dashBackward);
            ResetComboSystem();
        }

        public void DashLeft()
        {
            PlayAnimation("DashLeft");
            SetCurrentState(CurrentState.dashLeft);
            ResetComboSystem();
        }

        public void DashRight()
        {
            PlayAnimation("DashRight");
            SetCurrentState(CurrentState.dashRight);
            ResetComboSystem();
        }

        public void CastBomb()
        {
            if(photonView.IsMine)
            {
                // Make sure to drop any existing bomb the player may already be holding.
                if (weldBomb != null) DropBomb();

                // Cast new bomb.
                GameObject projectile = PhotonNetwork.Instantiate("Projectiles/Bomb", Pos(), Quaternion.identity);
                weldBomb = projectile.GetComponent<Bomb>();
                weldBomb.EffectedByGravity = false;
            }
        }

        public void DropBomb()
        {
            if (photonView.IsMine && weldBomb != null)
            {
                weldBomb.EffectedByGravity = true;
                weldBomb.PauseFuseTime = false;
                weldBomb = null;
            }
        }

        public void ThrowBomb()
        {
            if (photonView.IsMine && weldBomb != null)
            {
                weldBomb.ExplodeOnCollision = true;
                weldBomb.PauseFuseTime = false;
                weldBomb.SetEnableDirectionalMovement(true);
                weldBomb.SetMoveDirection(transform.forward);
                weldBomb.SetMoveSpeed(40f);
                weldBomb = null;
            }
        }
        #endregion
    }
}