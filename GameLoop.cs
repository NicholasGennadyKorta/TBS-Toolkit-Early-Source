using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour
{

    public static int currentTurn;
    public static string phase = "none";
    bool doneAction = false;
    bool _return = false;
    public static int currentCommandMenuItem = 0, commandMenuItemCount = 6;
    public static int currentItemMenuItem = 0, itemMenuItemCount, currentItemMenuActionItem = 0;
    public static bool chooseItemAction = false;
    public static Unit unitHovered, unitSelected;
    public static Vector2 unitSelectedOriginalGridPosition;
    AudioClip selectAudio = new AudioClip(),deselectAudio = new AudioClip();
    AudioClip handCursorMoveAudio = new AudioClip();
    AudioClip equip = new AudioClip(), dequip = new AudioClip();

    float timer;

    void Start()
    {
        ObjectPool.gameLoop = this;
        gameObject.AddComponent<AudioSource>();
        selectAudio = ObjectPool.audioDatabase.GetSoundEffect("Select");
        deselectAudio = ObjectPool.audioDatabase.GetSoundEffect("Deselect");
        handCursorMoveAudio = ObjectPool.audioDatabase.GetSoundEffect("HandCursorMove");
        equip = ObjectPool.audioDatabase.GetSoundEffect("Equip");
        dequip = ObjectPool.audioDatabase.GetSoundEffect("Dequip");
    }
    
    void Update()
    {
        _return = false;
        timer += Time.deltaTime;

        GoToNextPlayerTurn(true);
        UpdateUnitHovered();
        UpdateUnitSelected();
        UpdateMovePhase();
        UpdateChoosePhase();
        UpdateAttackPhase();
        UpdateItemPhase();
    }


    void UpdateUnitHovered()
    {
        //Get Our Hovered Unit
        for (int i = 0; i < ObjectPool.units.Count; ++i)
        {
            if (ObjectPool.units[i].GetComponent<Mover>().gridPosition == ObjectPool.noneTouchCursor.gridPosition)
            {
                unitHovered = ObjectPool.units[i];
                break;
            }
            unitHovered = null;
        }

    }

    void UpdateUnitSelected()
    {
        if (phase == "none" && !_return)
        {
            //On Input Select hovering Unit if it is on current players turn team and if it has not moved, else open the command window
            if (Input.GetKeyDown(KeyCode.Z) && unitHovered != null && unitSelected == null)
            {
                if (!unitHovered.moved && unitHovered.playerNumb == currentTurn)
                {
                    phase = "move";
                    doneAction = false;
                    currentCommandMenuItem = 0;
                    unitSelected = unitHovered;
                    unitSelected.GetComponent<Seeker>().GenerateMovementGrid(unitSelected.GetComponent<Mover>().gridPosition, unitSelected.stats.moveRange);
                    unitSelectedOriginalGridPosition = unitSelected.GetComponent<Mover>().gridPosition;
                    audio.PlayOneShot(selectAudio);
                    _return = true;
                }
            }
        }
    }

    void UpdateMovePhase()
    {
        if (phase == "move" && !_return)
        {
            //Move Unit if walkable grid position is selected
            if (Input.GetKeyDown(KeyCode.Z) && GridGraph.instance.CellWalkable(ObjectPool.noneTouchCursor.gridPosition))
            {
                phase = "moving";
                GridGraph.instance.ClearFloodFill();
                unitSelected.GetComponent<Mover>().SetPath(ObjectPool.noneTouchCursor.gridPosition);
                _return = true;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                phase = "none";
                unitSelected = null;
                audio.PlayOneShot(deselectAudio);
                GridGraph.instance.ClearFloodFill();
                _return = true;
            }
        }

        if (phase == "moving")
        {
            ObjectPool.noneTouchCursor.transform.position = unitSelected.transform.position;
            ObjectPool.noneTouchCursor.gridPosition = unitSelected.GetComponent<Mover>().gridPosition;
        }
    }

    public void UpdateChoosePhase()
    {

        if (phase == "none" && Input.GetKeyDown(KeyCode.Z) && unitHovered == null && !_return)
        {
            phase = "choose";
            _return = true;
        }


        if (phase == "choose" && !_return)
        {
            if (unitSelected)
            {
                ObjectPool.noneTouchCursor.transform.position = unitSelected.transform.position;
                ObjectPool.noneTouchCursor.gridPosition = unitSelected.GetComponent<Mover>().gridPosition;
            }

            if (Input.GetAxis("Vertical") <= -0.1f && timer > 0.3f)
            {
                timer = 0;

                currentCommandMenuItem++;
                if (currentCommandMenuItem > commandMenuItemCount - 1)
                    currentCommandMenuItem = 0;
                audio.PlayOneShot(handCursorMoveAudio);
            }
            else if (Input.GetAxis("Vertical") >= 0.1f && timer > 0.3f)
            {
                timer = 0;
                currentCommandMenuItem--;
                if (currentCommandMenuItem < 0)
                    currentCommandMenuItem = commandMenuItemCount - 1;
                audio.PlayOneShot(handCursorMoveAudio);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (ObjectPool.guiCommandMenu.GetCurrentMenuItem() == "Attack")
                {
                    if (unitSelected.inventory.equipedWeapon != null)
                    {
                        currentCommandMenuItem = 0;
                        phase = "chooseAttackTarget";
                        unitSelected.GetComponent<Seeker>().GenerateAttackGrid(unitSelected.GetComponent<Mover>().gridPosition, unitSelected.inventory.equipedWeapon.range, unitSelected.inventory.equipedWeapon.attackDiagonally);
                    }
                }

                else if (ObjectPool.guiCommandMenu.GetCurrentMenuItem() == "Items")
                {
                    currentCommandMenuItem = 0;
                    phase = "items";
                }

                else if (ObjectPool.guiCommandMenu.GetCurrentMenuItem() == "Wait")
                {
                    currentCommandMenuItem = 0;
                    unitSelected.SetMoved(true);
                    unitSelected = null;
                    phase = "none";
                }

                else if (ObjectPool.guiCommandMenu.GetCurrentMenuItem() == "End")
                    GoToNextPlayerTurn(false);

                _return = true;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                if (unitSelected && !doneAction)
                {
                    phase = "move";
                    currentCommandMenuItem = 0;
                    unitSelected.SetGridPosition(unitSelectedOriginalGridPosition);
                    unitSelected.GetComponent<Seeker>().GenerateMovementGrid(unitSelected.GetComponent<Mover>().gridPosition, unitSelected.stats.moveRange);
                    audio.PlayOneShot(deselectAudio);
                }
                else if (!doneAction)
                {
                    phase = "none";
                    currentCommandMenuItem = 0;
                    audio.PlayOneShot(deselectAudio);
                }

                _return = true;
            }
        }
    }

    public void UpdateAttackPhase()
    {
        if (phase == "chooseAttackTarget" && !_return)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (unitHovered != null && GridGraph.instance.pathNodes[(int)unitHovered.GetComponent<Mover>().gridPosition.x, (int)unitHovered.GetComponent<Mover>().gridPosition.y].isAttackable)
                {
                    DoBattle(unitSelected, unitHovered);
                    _return = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                GridGraph.instance.ClearFloodFill();
                phase = "choose";
            }
        }
    }

    public void UpdateItemPhase()
    {
        if (phase == "items" && !_return)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!chooseItemAction)
                {
                    currentCommandMenuItem = 0;
                    phase = "choose";
                }
                else
                {
                    ObjectPool.guiItemMenu.handCursorPositionXOffset = 0;
                    ObjectPool.guiItemMenu.handCusorPositionX -= 192;
                    currentItemMenuActionItem = 0;
                    chooseItemAction = false;
                }

                _return = true;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (chooseItemAction == false && itemMenuItemCount > 0)
                {
                    ObjectPool.guiItemMenu.handCursorPositionXOffset = 192;
                    ObjectPool.guiItemMenu.handCusorPositionX += 192;
                    chooseItemAction = true;
                }
                else if (chooseItemAction)
                {
                    string action = ObjectPool.guiItemMenu.GetCurrentActionItem();
                    if (action == "Consume")
                    {
                        unitSelected.ConsumeItem(currentItemMenuItem);
                        currentCommandMenuItem = 0;
                        unitSelected.SetMoved(true);
                        unitSelected = null;
                        phase = "none";
                    }
                    if (action == "Equip")
                    {
                        unitSelected.EquipWeapon(currentItemMenuItem);
                        audio.PlayOneShot(equip);
                    }
                    else if (action == "Dequip")
                    {
                        unitSelected.DequipWeapon();
                        audio.PlayOneShot(dequip);
                    }
                    else if (action == "Discard")
                        unitSelected.DiscardWeapon(currentItemMenuItem);


                    ObjectPool.guiItemMenu.handCursorPositionXOffset = 0;
                    ObjectPool.guiItemMenu.handCusorPositionX -= 192;
                    currentItemMenuActionItem = 0;
                    currentItemMenuItem = 0;
                    chooseItemAction = false;
                    doneAction = true;
                }

                _return = true;
            }

            if (Input.GetAxis("Vertical") <= -0.1f && timer > 0.3f)
            {
                timer = 0;

                if (!chooseItemAction)
                    currentItemMenuItem++;
                else currentItemMenuActionItem++;

                if (currentItemMenuItem > itemMenuItemCount - 1)
                    currentItemMenuItem = 0;
                if (currentItemMenuActionItem > 1)
                    currentItemMenuActionItem = 0;

                audio.PlayOneShot(handCursorMoveAudio);
            }
            else if (Input.GetAxis("Vertical") >= 0.1f && timer > 0.3f)
            {
                timer = 0;

                if (!chooseItemAction)
                    currentItemMenuItem--;
                else currentItemMenuActionItem--;

                if (currentItemMenuItem < 0)
                    currentItemMenuItem = itemMenuItemCount - 1;
                if (currentItemMenuActionItem < 0)
                    currentItemMenuActionItem = 1;

                audio.PlayOneShot(handCursorMoveAudio);

            }
        }
    }

    public void GoToNextPlayerTurn(bool checkUnits)
    {
        if (checkUnits)
        for (int i = 0; i < ObjectPool.units.Count; ++i)
            if (ObjectPool.units[i].playerNumb == currentTurn && !ObjectPool.units[i].moved )
                return;

        unitHovered = null;
        unitSelected = null;
        phase = "none";
        currentTurn++;
        if (currentTurn > ObjectPool.playersDatabase.players.Count)
            currentTurn = 0;

        for (int i = 0; i < ObjectPool.units.Count; ++i)
            ObjectPool.units[i].SetMoved(false);

        for (int i = 0; i < ObjectPool.units.Count; ++i)
        {
            if (ObjectPool.units[i].playerNumb == currentTurn)
            {
                ObjectPool.noneTouchCursor.gridPosition = ObjectPool.units[i].GetComponent<Mover>().gridPosition;
                break;
            }
        }
    }

    public void DoBattle(Unit unitAttacking, Unit unitDefending)
    {
        phase = "battling";

        int unitAttackingDamage = unitAttacking.stats.strength + unitAttacking.inventory.equipedWeapon.might - unitDefending.stats.defence;
        int unitDefendingDamage = unitDefending.stats.strength + unitDefending.inventory.equipedWeapon.might - unitAttacking.stats.defence;

        if (unitAttacking.inventory.equipedWeapon.weaponType == 1)
        {
            unitAttackingDamage = unitAttacking.stats.magic + unitAttacking.inventory.equipedWeapon.might - unitDefending.stats.resistance;
            //Debug.Log(unitAttackingDamage);
        }
        if (unitDefending.inventory.equipedWeapon.weaponType == 1)
        {
            unitDefendingDamage = unitDefending.stats.magic + unitDefending.inventory.equipedWeapon.might - unitAttacking.stats.resistance;
           // Debug.Log(unitDefendingDamage);
        }

        if (unitAttackingDamage < 0) unitAttackingDamage = 0;
        if (unitDefendingDamage < 0) unitDefendingDamage = 0;

      
        if (unitDefending.inventory.equipedWeapon != null && unitAttacking.stats.currentHp > 0)
            unitDefending.stats.currentHp -= unitAttackingDamage;

        if (unitDefending.stats.currentHp > 0 && unitDefending.inventory.equipedWeapon.range >= unitAttacking.inventory.equipedWeapon.range)
            unitAttacking.stats.currentHp -= unitDefendingDamage;

        GridGraph.instance.ClearFloodFill();
        currentCommandMenuItem = 0;
        unitSelected.SetMoved(true);
        unitSelected = null;
        phase = "none";
    }
}
