using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Entity
{
    public float dungeonEnterDelay = 1f;

    public Text movementText;

    public Text turnText;

    public Text levelText;

    public Text healthText;

    public Text experienceText;

    public Text attackText;

    public Text defenceText;

    public Text speedText;

    public Text agilityText;

    public Text accuracyText;

    public Text interactObjectText;

    public Text iteractionInfoText;

    public Text interactText;

    public Text choiceOneText;

    public Text choiceTwoText;

    public Text finalText;

    public Text storyWindowText;

    public GameObject interactionWindow;

    public GameObject resultWindow;

    public GameObject storyWindow;

    public GameObject gameStatusWindow;

    public GameObject levelUpWindow;

    public Sprite gameOverSprite;

    public Sprite newGameSprite;

    public Sprite victorySprite;

    public Text gameStatusWindowText;

    public Image helmetImage;

    public Image weaponImage;

    public Image shieldImage;

    public Image chestplateImage;

    [HideInInspector]
    public PlayerStats stats;

    private int choice = 0;

    private int turn = 1;

    private bool isSlowerMovement = false;

    private bool isDuringInteraction = false;

    private bool finalFight = false;

    private string interactionObjectName;

    private string lastInteractObjectName;

    private string lastInteractObjectInfo;

    private Item tempItem;

    private Collider2D otherObject;

    private int textNumber;

    protected override void Start()
    {
        interactionWindow.SetActive(false);
        resultWindow.SetActive(false);
        levelUpWindow.SetActive(false);
        storyWindow.SetActive(true);
        if (GameManager.instance.level == 0)
        {
            gameStatusWindow.SetActive(true);
            GameManager.instance.basicStats.copyTo (stats);
            textNumber = 0;
            interactObjectText.text = "MagicPortal";
            iteractionInfoText.text =
                "Hero enter the magic portal created by king's wizard and teleported to undiscovered lands. His adventure has began!";
            transform.position =
                GameManager.instance.mapScript.getPlayerStartPostition();
            stats.healthPoints = stats.maxHealth;
        }
        else
        {
            gameStatusWindow.SetActive(false);
            stats = GameManager.instance.playerStats;
            textNumber = 10;
            interactObjectText.text = "DungeonEnter";
            iteractionInfoText.text =
                "Hero enters the dungeon and new turn has been started. The exit closed behind the Hero, leaving him no way back.";
            transform.position =
                GameManager.instance.dungeonScript.getPlayerStartPostition();
        }
        finalFight = false;
        stats.movement = stats.baseMovement;
        movementText.text = stats.movement + "/" + stats.baseMovement;
        turnText.text = turn.ToString();
        updateEntireUI();
        base.Start();
    }

    private bool CheckIfGameOver()
    {
        if (stats.healthPoints <= 0)
        {
            GameManager.instance.GameOver();
            gameStatusWindow.SetActive(true);
            return true;
        }
        return false;
    }

    void Update()
    {
        if (storyWindow.activeSelf || gameStatusWindow.activeSelf || finalFight)
        {
            GetPressedKey();
            if (GameManager.instance.checkIfIsGameOver())
            {
                gameStatusWindow.GetComponent<Image>().sprite = gameOverSprite;
                gameStatusWindowText.text =
                    "You died after a " +
                    turn +
                    " turns.\n Press space or enter to continue...";
                if (choice == 10)
                {
                    enabled = false;
                    SceneManager
                        .LoadScene(SceneManager.GetActiveScene().buildIndex,
                        LoadSceneMode.Single);
                }
            }
            else if (textNumber == 0)
            {
                gameStatusWindow.GetComponent<Image>().sprite = newGameSprite;
                gameStatusWindowText.text =
                    "Press space or enter to start a game.";
                if (choice == 10)
                {
                    textNumber = 1;
                    gameStatusWindow.SetActive(false);
                }
            }
            else if (textNumber == 1)
            {
                storyWindowText.text =
                    "Kingdom of Liberion.\n Formerly the most wonderful place to live in the present day. The reigning king always cared for the fate of each of his subjects, from peasants to townspeople and knights to clerics. But once the king was cursed by a witch whom he refused to give up his daughter. No one in the kingdom can lift this curse. There has been a plague in the kingdom of Literion for 2 years. Every month, more than 100,000 people die of hunger or disease. The kingdom is on the brink of collapse. The court wizard, however, found a solution to the problem. He discovered a land separate from the rest of the world. According to the books by which the wizard discovered this land, there is an ancient dungeon on this land in which there is a book containing a spell that will remove the curse from the kingdom. However, it is not possible to get there by land or sea.";
                if (choice == 10 && textNumber == 1)
                {
                    textNumber = 2;
                }
            }
            else if (textNumber == 2)
            {
                storyWindowText.text =
                    "Thanks to the work of all the people of the kingdom, the wizard managed to create a spell that would transfer one daredevil to inaccessible lands. However, the spell is not perfect. It is possible to teleport only one person to unknown lands: the spell does not work if a human from the kingdom is already in this unknown land. The second drawback is the equipment. The teleported person cannot weigh too much with his equipment, and additionally he cannot have any magical items. Additionally, any equipment worn by the transferred Kingdom Citizen disappears upon its unequipment. These factors meant that nobody wanted to risk their lives. Therefore, the king announced that a person who succeeds in this mission will be granted any 3 wishes: from wealth to even becoming the new king. Thus you volunteered for the 16th. Only you know the reason why you do this. When the wizard detected that volunteer number 15 had died, you were called. You received very basic equipment and then the wizard opened the portal and you jumped into it with a slight hesitation. Your mission is just beginning...";
                if (choice == 10)
                {
                    storyWindow.SetActive(false);
                }
            }
            else if (textNumber == 10)
            {
                storyWindowText.text =
                    "You have reached the dungeon you are looking for. You realize there is a book somewhere in it that could restore the kingdom of Liberion to its former glory. Obtaining this book also means you win a prize. You approach the descent with hesitant steps. Unfortunately, you are stumbling over the unfolded rope. You fall right into the middle of the dungeon. The line turns out to be a trap and the entrance collapses behind you, leaving no way to return. There is a terrible darkness in the dungeon, so your movements will be twice slower while traversing the dungeon than it was on the surface.";
                if (choice == 10)
                {
                    storyWindow.SetActive(false);
                }
            }
            else if (textNumber == 20)
            {
                storyWindowText.text =
                    "You see a pedestal with a mysterious book in front of you. You realize that this is the purpose of your journey. You put your hand towards the book. Suddenly you hear a terrifying scream. A huge monster resembling a flying eye emerges from the book. It appears to be the keeper of this book. Therefore, the greatest test of your skills awaits you.";
                if (choice == 10)
                {
                    textNumber = 21;
                }
            }
            else if (textNumber == 21)
            {
                FightManager
                    .instance
                    .StartFight(this, GameManager.instance.finalBoss);
                storyWindow.SetActive(false);
                textNumber = 22;
            }
            else if (textNumber == 22)
            {
                if (!FightManager.instance.isDuringFight())
                {
                    finalFight = false;
                    if (!CheckIfGameOver())
                    {
                        storyWindow.SetActive(true);
                        textNumber = 23;
                    }
                    else
                    {
                        textNumber = 0;
                    }
                }
            }
            else if (textNumber == 23)
            {
                storyWindowText.text =
                    "After all, your destination is in front of you. Without hesitating, you reach for the book from the pedestal for a long time. It turns out that this book is not only the only hope to save the kingdom of Liberion. It allows you to cast any spell that its user can imagine. However, you pay for casting such a spell with your own life. You find that the power of this book is too powerful to carry it outside this dungeon. Therefore, you cast the last spell of your life using this book. Your spell is to get rid of all magic from all over the world. When you cast a spell, you feel the power overwhelming you. After a while you feel that the spell has been cast successfully, then you feel calm and die without any pain.";
                if (choice == 10)
                {
                    textNumber = 24;
                }
            }
            else if (textNumber == 24)
            {
                storyWindowText.text =
                    "All the magic has gone from all over the world. The king of the kingdom of Liberion is no longer cursed. The kingdom slowly began to return to normal. All wizards lost their power, but found a new calling in the field of alchemy. Only the court wizard was furious about it. As a reward from the king for his achievements so far, he decided to leave the royal court to embark on a journey that will allow him to regain his former magical power. The king agreed as a request. The wizard on that day disappeared from the palace and all information about him was lost. 5 years later, the kingdom reached its best period. And you have been declared a hero of the kingdom, you have been written in the books that your deed will never be forgotten. Many monuments have been erected on your part, and you have also been declared holy by the priests.";
                if (choice == 10)
                {
                    textNumber = 25;
                    storyWindow.SetActive(false);
                    gameStatusWindow.SetActive(true);
                }
            }
            else if (textNumber == 25)
            {
                gameStatusWindow.GetComponent<Image>().sprite = victorySprite;
                gameStatusWindowText.text =
                    "You saved kingdom after " +
                    turn +
                    " turns. \nPress Space or Enter to go to title-page.";
                if (choice == 10)
                {
                    enabled = false;
                    GameManager.instance.GameOver();
                    SceneManager
                        .LoadScene(SceneManager.GetActiveScene().buildIndex,
                        LoadSceneMode.Single);
                }
            }
            return;
        }
        if (CR_running)
        {
            return;
        }
        if (FightManager.instance.isDuringFight())
        {
            return;
        }
        if (CheckIfGameOver())
        {
            return;
        }
        if (!GameManager.instance.isPlayerTurn)
        {
            if (!GameManager.instance.enemyTurn)
            {
                NewTurn();
            }
            return;
        }
        if (isDuringInteraction)
        {
            GetPressedKey();
            InteractWithObject();
            return;
        }
        if (CheckIfGameOver())
        {
            return;
        }
        if (CheckIfLevelUp())
        {
            if (!levelUpWindow.activeSelf)
            {
                levelUpWindow.SetActive(true);
            }
            GetPressedKey();
            stats.LevelUp (choice);
            updateEntireUI();
            return;
        }
        if (levelUpWindow.activeSelf)
        {
            levelUpWindow.SetActive(false);
        }
        if (textNumber == 20 || CheckIfGameOver())
        {
            return;
        }
        if (CheckIfEndTurn())
        {
            GameManager.instance.enemyTurn = true;
            GameManager.instance.isPlayerTurn = false;
            return;
        }
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }
        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Entity> (horizontal, vertical);
        }
    }

    public void updateUIAfterFight(string enemyName, string resultText)
    {
        lastInteractObjectName = enemyName;
        lastInteractObjectInfo = resultText;
        updateLastInteractionObjectUI();
        updateEntireUI();
    }

    private void GetPressedKey()
    {
        if (Input.GetKeyDown("1") || Input.GetKeyDown("[1]"))
        {
            choice = 1;
        }
        else if (Input.GetKeyDown("2") || Input.GetKeyDown("[2]"))
        {
            choice = 2;
        }
        else if (Input.GetKeyDown("3") || Input.GetKeyDown("[3]"))
        {
            choice = 3;
        }
        else if (Input.GetKeyDown("4") || Input.GetKeyDown("[4]"))
        {
            choice = 4;
        }
        else if (Input.GetKeyDown("5") || Input.GetKeyDown("[5]"))
        {
            choice = 5;
        }
        else if (Input.GetKeyDown("space") || Input.GetKeyDown(KeyCode.Return))
        {
            choice = 10;
        }
        else
        {
            choice = 0;
        }
    }

    public void NewTurn()
    {
        turn++;
        stats.movement = stats.baseMovement;
        turnText.text = turn.ToString();
        movementText.text = stats.movement + "/" + stats.baseMovement;
        updateEntireUI();
        if (turn % 5 == 0)
        {
            GameManager.instance.upgradeEnemies();
        }
        GameManager.instance.isPlayerTurn = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "DungeonEnter")
        {
            stats.baseMovement /= 2;
            Invoke("DungeonEnter", dungeonEnterDelay);
            enabled = false;
        }
        if (other.tag == "Tree")
        {
            isSlowerMovement = true;
        }
        if (other.tag == "floor")
        {
            isSlowerMovement = false;
        }
        if (
            other.tag == "Chest" ||
            other.tag == "CampFire" ||
            other.tag == "Obelisk" ||
            other.tag == "Potion" ||
            other.tag == "Home" ||
            other.tag == "Grave" ||
            other.tag == "Meat" ||
            other.tag == "Sign" ||
            other.tag == "Torch" ||
            other.tag == "Spikes" ||
            other.tag == "DungeonChest" ||
            other.tag == "Bones" ||
            other.tag == "DungeonPotion" ||
            other.tag == "Vase" ||
            other.tag == "BookCase" ||
            other.tag == "GameDestination"
        )
        {
            otherObject = other;
            interactionObjectName = other.tag;
            isDuringInteraction = true;
        }
    }

    private void DungeonEnter()
    {
        SceneManager
            .LoadScene(SceneManager.GetActiveScene().buildIndex,
            LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        GameManager.instance.playerStats = this.stats;
    }

    protected override void AttemptMove<T>(int x, int y)
    {
        base.AttemptMove<T>(x, y);

        RaycastHit2D hit;

        if (Move(x, y, out hit))
        {
            if (isSlowerMovement)
            {
                stats.movement--;
            }
            stats.movement--;
        }
        movementText.text = stats.movement + "/" + stats.baseMovement;
        CheckIfGameOver();
    }

    protected bool CheckIfEndTurn()
    {
        if (stats.movement <= 0 || (stats.movement <= 1 && isSlowerMovement))
        {
            return true;
        }
        return false;
    }

    private bool CheckIfLevelUp()
    {
        if (stats.experience >= stats.neededExperience)
        {
            return true;
        }
        return false;
    }

    private void updateEntireUI()
    {
        movementText.text = stats.movement + "/" + stats.baseMovement;
        healthText.text = stats.healthPoints + "/" + stats.maxHealth;
        experienceText.text = stats.experience + "/" + stats.neededExperience;
        levelText.text = stats.level.ToString();
        updatePlayerStatsUI();
        updatePlayerEquipmentUI();
    }

    private void updateLastInteractionObjectUI()
    {
        interactObjectText.text = lastInteractObjectName;
        iteractionInfoText.text = lastInteractObjectInfo;
    }

    private void updatePlayerStatsUI()
    {
        stats.calculateActualStats();
        attackText.text = stats.attack.ToString();
        defenceText.text = stats.defence.ToString();
        speedText.text = stats.speed.ToString();
        agilityText.text = stats.agility.ToString();
        accuracyText.text = stats.accuracy.ToString();
    }

    private void updatePlayerEquipmentUI()
    {
        helmetImage.sprite = stats.helmet.sprite;
        weaponImage.sprite = stats.weapon.sprite;
        shieldImage.sprite = stats.shield.sprite;
        chestplateImage.sprite = stats.chestplate.sprite;
    }

    public void updatePlayerHPUI()
    {
        healthText.text = stats.healthPoints + "/" + stats.maxHealth;
    }

    private void InteractWithObject()
    {
        switch (interactionObjectName)
        {
            case "Chest":
                IteractWithChest();
                break;
            case "ItemEquip":
                IsPlayerWantEquipItem();
                break;
            case "Result":
                InfoPlayerAboutResult();
                break;
            case "CampFire":
                InteractWithCampfire();
                break;
            case "Obelisk":
                InteractWithObelisk();
                break;
            case "Potion":
                InteractWithPotion();
                break;
            case "Home":
                InteractWithHome();
                break;
            case "Grave":
                InteractWithGrave();
                break;
            case "Meat":
                InteractWithMeat();
                break;
            case "Sign":
                InteractWithSign();
                break;
            case "GameDestination":
                Victory();
                break;
            case "Torch":
                InteractWithTorch();
                break;
            case "Spikes":
                InteractWithSpikes();
                break;
            case "DungeonChest":
                InteractWithDungeonChest();
                break;
            case "Bones":
                InteractWithBones();
                break;
            case "DungeonPotion":
                InteractWithDungeonPotion();
                break;
            case "Vase":
                InteractWithVase();
                break;
            case "BookCase":
                InteractWithBookCase();
                break;
            default:
                isDuringInteraction = false;
                break;
        }
    }

    private void IteractWithChest()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Chest";
            interactText.text =
                "You see the chest. It's not locked. Do you open it?";
            choiceOneText.text = "1. Yes";
            choiceTwoText.text = "2. No";
        }
        if (choice == 1)
        {
            tempItem =
                GameManager
                    .instance
                    .itemList[Random
                        .Range(0, GameManager.instance.itemList.Length)];
            interactionObjectName = "ItemEquip";
        }
        else if (choice == 2)
        {
            finalText.text =
                "You choose not to open the chest. You looked around and when you looked at the place where was chest again, you couldn't see it anymore.";
            lastInteractObjectInfo =
                "You choose not to open the chest and it disapear.";
            interactionObjectName = "Result";
        }
    }

    private void IsPlayerWantEquipItem()
    {
        interactText.text =
            "In " +
            lastInteractObjectName +
            " you found: " +
            tempItem.ToString() +
            ". Do you want to equip it? (Equip this item will remove irreversibly previous item of the same type.)";
        choiceOneText.text = "1. Yes";
        choiceTwoText.text = "2. No";
        if (choice == 1)
        {
            stats.equipItem (tempItem);
            finalText.text =
                "You decide to equip: " +
                tempItem.ToString() +
                ". Previous " +
                tempItem.type +
                " has disappered when you unequip it.";
            lastInteractObjectInfo =
                "In " +
                lastInteractObjectName +
                " you found " +
                tempItem.ToString() +
                ". You equip it.";
            interactionObjectName = "Result";
        }
        else if (choice == 2)
        {
            finalText.text =
                "You do not decide to equip: " +
                tempItem.ToString() +
                ". This " +
                tempItem.type +
                " has disappered after a while.";
            lastInteractObjectInfo =
                "In " +
                lastInteractObjectName +
                " you found " +
                tempItem.ToString() +
                ". You do not equip it.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithCampfire()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Campfire";
            interactText.text = "You see the campfire. Do you want to rest?";
            choiceOneText.text = "1. Yes";
            choiceTwoText.text = "2. No";
        }
        if (choice == 1)
        {
            stats.Heal(2);
            stats.movement += 5;
            finalText.text =
                "You choose to rest with a campfire until it goes out. You feel rested and relaxed: +5 Movement Points this turn, 2 HP Healed";
            lastInteractObjectInfo =
                "You choose to rest with a campfire: +5 Movement Points, 2 HP Healed";
            interactionObjectName = "Result";
        }
        else if (choice == 2)
        {
            finalText.text =
                "You choose not to rest with a campfire. After a while, a strong cold wind blew, which extinguished this campfire.";
            lastInteractObjectInfo = "You choose not to rest with a campfire.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithObelisk()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Obelisk";
            interactText.text =
                "You see the obelisk. You feel great power emanating from within him. You feel that praying in front of this obelisk can give you some of that power.";
            choiceOneText.text = "1. Pray";
            choiceTwoText.text = "2. Go away";
        }
        if (choice == 1)
        {
            int statBuffChoice = Random.Range(0, 5);
            string increasedStat = "";
            switch (statBuffChoice)
            {
                case 0:
                    stats.baseAttack += 1;
                    increasedStat = "Attack";
                    break;
                case 1:
                    stats.baseDefence += 1;
                    increasedStat = "Defence";
                    break;
                case 2:
                    stats.baseSpeed += 1;
                    increasedStat = "Speed";
                    break;
                case 3:
                    stats.baseAgility += 1;
                    increasedStat = "Agility";
                    break;
                case 4:
                    stats.baseAccuracy += 1;
                    increasedStat = "Accuracy";
                    break;
            }
            finalText.text =
                "You chose to pray at the obelisk. Your prayers have been heard. Your " +
                increasedStat +
                " has been increased by 1. You feel the power has departed from the obelisk and then it has collapsed. ";
            lastInteractObjectInfo =
                "You chose to pray at the obelisk: +1 " + increasedStat;
            interactionObjectName = "Result";
        }
        else if (choice == 2)
        {
            finalText.text =
                "You choose not to pray at the obelisk. You feel the power has departed from the obelisk and then it has collapsed.";
            lastInteractObjectInfo = "You choose not to pray at the obelisk.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithPotion()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Potion";
            interactText.text =
                "You see the potion on the ground with red fluid in it.";
            choiceOneText.text = "1. Drink it.";
            choiceTwoText.text = "2. Throw it away.";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) == 0)
            {
                stats.healthPoints -= 2;
                finalText.text =
                    "You choose to drink the potion. You drank all the contents without feeling any changes. After a while, however, you felt unwell and vomited red fluid. You lost 2 HP.";
                lastInteractObjectInfo =
                    "You choose to drink the potion: 2 HP lost.";
            }
            else
            {
                stats.Heal(5);
                finalText.text =
                    "You choose to drink the potion. It tasted like strawberries and you feel helthier. You are healed by 5 points.";
                lastInteractObjectInfo =
                    "You choose to drink the potion: 5 HP healed.";
            }

            interactionObjectName = "Result";
        }
        else if (choice == 2)
        {
            finalText.text =
                "You choose to throw potion away. The bottle broke when it hit the ground, causing its contents to spill out and then sink into the ground. ";
            lastInteractObjectInfo = "You choose to throw potion away.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithHome()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Home";
            interactText.text =
                "You see the strange home. Do you want to check it?";
            choiceOneText.text = "1. Yes";
            choiceTwoText.text = "2. No";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > 4)
            {
                stats.movement = 0;
                stats.healthPoints -= 5;
                finalText.text =
                    "You choose to check the home. You knocked without hearing any answer, so decided to go inside. Suddenly you felt a burning smell. It turned out that the house you are in started to burn. You ran away from home, but you loss 5 HP and all remaining movement.";
                lastInteractObjectInfo =
                    "You choose to check the home: 5 HP lost, all remaining movementem points lost";
                interactionObjectName = "Result";
            }
            else
            {
                tempItem =
                    GameManager
                        .instance
                        .itemList[Random
                            .Range(0, GameManager.instance.itemList.Length)];
                interactionObjectName = "ItemEquip";
            }
        }
        else if (choice == 2)
        {
            finalText.text =
                "You don't choose to check this home. People don't live there, but when you were looking around you heard strange sound and the home has disapear.";
            lastInteractObjectInfo = "You don't choose to check this home.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithGrave()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Grave";
            interactText.text =
                "You see a strange stone sticking out of the ground with a rounded upper part. It looks like a tombstone, but only says \"!\". Do you want to dig up the ground next to him?";
            choiceOneText.text = "1. Yes";
            choiceTwoText.text = "2. No";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > 4)
            {
                stats.baseAccuracy -= 1;
                finalText.text =
                    "You started digging and after a while a strange substance spurted from the inside straight into your eye. In panic, you dropped a stone on the spot you were digging, feeling that your eye was permanently damaged. You lost 1 Accuracy.";
                lastInteractObjectInfo =
                    "You choose to dig next to this strange stone: -1 Accuracy.";
                interactionObjectName = "Result";
            }
            else
            {
                stats.experience += 2;
                finalText.text =
                    "You dug a strange scroll that contained some information about the past of the land you are in. You received 2 EXP.";
                lastInteractObjectInfo =
                    "You choose to dig next to this strange stone: +2 EXP.";
                interactionObjectName = "Result";
            }
        }
        else if (choice == 2)
        {
            finalText.text =
                "You don't choose to check this strange stone. As you walked away you accidentally kicked a small rock right into that strange protruding stone that had broken into thousands of pieces.";
            lastInteractObjectInfo =
                "You don't choose to dig next to this strange stone.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithMeat()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Meat";
            interactText.text =
                "You see a meat on the ground. You don't smell any smell coming from that meat. Do you want to eat them?";
            choiceOneText.text = "1. Yes";
            choiceTwoText.text = "2. No";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > 3)
            {
                stats.maxHealth -= 1;
                stats.Heal(0);
                finalText.text =
                    "You eat this meat. It tasted like chicken, but after you finished eat it, you feel sick. Your max health has been deacresed by 1.";
                lastInteractObjectInfo = "You eat this meat: -1 max HP.";
                interactionObjectName = "Result";
            }
            else
            {
                stats.baseAttack += 1;
                finalText.text =
                    "You eat this meat. It tastes disgusting, but you feel stronger after that. Your attack has been increased by 1.";
                lastInteractObjectInfo =
                    "You choose to eat this meat: +1 Attack";
                interactionObjectName = "Result";
            }
        }
        else if (choice == 2)
        {
            finalText.text =
                "You don't choose to eat this meat. Suddenly you notice a strange animal rushing towards you. The animal, however, runs past you, taking the meat in the lob and fleeing out of your sight.";
            lastInteractObjectInfo = "You don't choose to eat this meat.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithSign()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Sign";
            interactText.text =
                "You see a strange sign. Do you want to read what is written on it? ";
            choiceOneText.text = "1. Yes";
            choiceTwoText.text = "2. No";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > 6)
            {
                finalText.text =
                    "You tried to read the text on the sign but couldn't understand anything. You decided to go on, but as you passed the sign you stumbled and fell on it, completely destroying the sign.";
                lastInteractObjectInfo =
                    "You try to read what is written on sign. Nothing happend.";
                interactionObjectName = "Result";
            }
            else
            {
                stats.baseMovement += 2;
                finalText.text =
                    "You've read the sign, making you feel like you know better how to navigate this land. Max movement points increase by 2.";
                lastInteractObjectInfo =
                    "You choose to read the sign: +2 max Movement.";
                interactionObjectName = "Result";
            }
        }
        else if (choice == 2)
        {
            finalText.text =
                "You decided to go on, but as you passed the sign you stumbled and fell on it, completely destroying the sign.";
            lastInteractObjectInfo =
                "You decided to not read the text written on sign.";
            interactionObjectName = "Result";
        }
    }

    private void InfoPlayerAboutResult()
    {
        if (interactionWindow.activeSelf)
        {
            updateLastInteractionObjectUI();
            updateEntireUI();
            interactionWindow.SetActive(false);
            resultWindow.SetActive(true);
        }
        if (choice == 10)
        {
            otherObject.gameObject.SetActive(false);
            choice = 0;
            resultWindow.SetActive(false);
            isDuringInteraction = false;
        }
    }

    private void InteractWithTorch()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Torch";
            interactText.text =
                "You see the torch. Do you want to take it with you?";
            choiceOneText.text = "1. Yes";
            choiceTwoText.text = "2. No";
        }
        if (choice == 1)
        {
            stats.movement = stats.baseMovement;
            finalText.text =
                "You choose to take a torch with you. Thanks to this you have a good look around the area then the torch went out. Your movement points in this turn have been received to base value.";
            lastInteractObjectInfo =
                "You choose to rest a torch: Movement points restored.";
            interactionObjectName = "Result";
        }
        else if (choice == 2)
        {
            finalText.text =
                "You choose not to take a torch. After a while torch went out.";
            lastInteractObjectInfo = "You choose not to take a torch";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithSpikes()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Spikes";
            interactText.text = "You see a spikes blocking way.";
            choiceOneText.text =
                "1. Try to trigger them and after that dodge them.";
            choiceTwoText.text = "2. Try to run fast through them.";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > stats.agility)
            {
                stats.healthPoints -= 5;
                stats.baseMovement =
                    stats.baseMovement <= 1 ? 1 : stats.baseMovement - 1;
                finalText.text =
                    "You tried to trigger spikes and dodge them. You failed and lost 5 HP and 1 max movement point(You can't has less than one max movement point). The spikes turned out to be a one-time trap.";
                lastInteractObjectInfo =
                    "You tried to dodge them: -5HP, -1 max Movement Point";
                interactionObjectName = "Result";
            }
            else
            {
                finalText.text =
                    "You tried to trigger spikes and dodge them. You did it, and the spikes turned out to be a one-time trap.";
                lastInteractObjectInfo = "You tried to dodge them: success.";
                interactionObjectName = "Result";
            }
        }
        else if (choice == 2)
        {
            if (Random.Range(0, 10) > stats.speed)
            {
                stats.healthPoints -= 5;
                stats.baseMovement =
                    stats.baseMovement <= 1 ? 1 : stats.baseMovement - 1;
                finalText.text =
                    "You tried to run fast through them. You failed and lost 5 HP and 1 max movement point(You can't has less than one max movement point). The spikes turned out to be a one-time trap.";
                lastInteractObjectInfo =
                    "You tried to run fast through them.: -5HP, -1 max Movement Point";
                interactionObjectName = "Result";
            }
            else
            {
                finalText.text =
                    "You tried to run fast through them. You did it, and the spikes turned out to be a one-time trap.";
                lastInteractObjectInfo = "You tried to run fast through them.";
                interactionObjectName = "Result";
            }
        }
    }

    private void InteractWithDungeonChest()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "DungeonChest";
            interactText.text =
                "You see the chest. It's not locked. Do you open it?";
            choiceOneText.text = "1. Yes";
            choiceTwoText.text = "2. No";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > 3)
            {
                stats.healthPoints -= 7;
                finalText.text =
                    "You looked inside the chest. At the moment of opening it, you heard a strange noise of the switching mechanism. You immediately walked away to the chest, which exploded after a while. The explosion hurt you seriously. You lost 7HP.";
                lastInteractObjectInfo = "You open the chest: lost 7 HP.";
                interactionObjectName = "Result";
            }
            else
            {
                tempItem =
                    GameManager
                        .instance
                        .dungeonItemList[Random
                            .Range(0,
                            GameManager.instance.dungeonItemList.Length)];
                interactionObjectName = "ItemEquip";
            }
        }
        else if (choice == 2)
        {
            finalText.text =
                "You choose not to open the chest. You looked around and when you looked at the place where was chest again, you couldn't see it anymore.";
            lastInteractObjectInfo =
                "You choose not to open the chest and it disapear.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithBones()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Bones";
            interactText.text = "You see a bones on the floor.";
            choiceOneText.text = "1. Check it.";
            choiceTwoText.text = "2. Go away.";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > stats.defence)
            {
                stats.baseDefence -= 1;
                finalText.text =
                    "You dediced to check the bones. Suddenly you noticed rats running towards you. They chewed through your armor and escaped taking bones with them, but didn't affect your health. Your defence deacresed by 1.";
                lastInteractObjectInfo =
                    "You decided to check the bones: -1 Defence.";
                interactionObjectName = "Result";
            }
            else
            {
                stats.experience += 5;
                finalText.text =
                    "You dediced to check the bones. Suddenly you noticed rats running towards you. They can't chewed through your armor and escaped. You found strange scroll among the bones. You gain 5 EXP.";
                lastInteractObjectInfo =
                    "You decided to check the bones: +5 EXP.";
                interactionObjectName = "Result";
            }
        }
        else if (choice == 2)
        {
            finalText.text =
                "You decided to go away. Suddenly you noticed rats running towards you. Fortunately, they took care of the bones and then escaped, taking them with them.";
            lastInteractObjectInfo = "You decided to go away;";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithDungeonPotion()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "DungeonPotion";
            interactText.text = "You see strange gree potion on the floor.";
            choiceOneText.text = "1. Drink it.";
            choiceTwoText.text = "2. Throw it away.";
        }
        if (choice == 1)
        {
            int modifier;
            if (Random.Range(0, 10) > 4)
            {
                modifier = -1;
                finalText.text =
                    "You dediced to drink a potion. He didn't have the taste, but you feel weakier. All your 5 base stats decreased by 1.";
                lastInteractObjectInfo =
                    "You decided to drink a potion: All your 5 base stats decreased by 1.";
                interactionObjectName = "Result";
            }
            else
            {
                modifier = 1;
                finalText.text =
                    "You dediced to drink a potion. He didn't have the taste, but you feel more powerfull. All your 5 base stats increased by 1.";
                lastInteractObjectInfo =
                    "You decided to drink a potion: All your 5 base stats increased by 1.";
                interactionObjectName = "Result";
            }
            stats.baseAttack += modifier;
            stats.baseAgility += modifier;
            stats.baseSpeed += modifier;
            stats.baseDefence += modifier;
            stats.baseAccuracy += modifier;
        }
        else if (choice == 2)
        {
            finalText.text =
                "You choose to throw potion away. The bottle broke when it hit the floor, causing its contents to spill out and then sink into the floor. ";
            lastInteractObjectInfo = "You choose to throw potion away.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithVase()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Vase";
            interactText.text = "You see a golden vase.";
            choiceOneText.text = "1. Check what's inside.";
            choiceTwoText.text = "2. Ignore it.";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > stats.attack)
            {
                finalText.text =
                    "You dediced to check the vase. It's nothing inside this vase. Pulling your hand out of the vase, you caught its corner and it knocked.";
                lastInteractObjectInfo =
                    "You dediced to check the vase. It's nothing inside it.";
                interactionObjectName = "Result";
            }
            else
            {
                stats.baseMovement += 1;
                finalText.text =
                    "You dediced to check the vase. you found a fragment of this dungeon in it. Your max movement points increased by 1;";
                lastInteractObjectInfo =
                    "You decided to check the vase. +1 max movement point.";
                interactionObjectName = "Result";
            }
        }
        else if (choice == 2)
        {
            finalText.text =
                "You decided ignore the vase. After a while you noticed a bat that flew into this vase breaking it into pieces.";
            lastInteractObjectInfo = "You decided to ignore it.";
            interactionObjectName = "Result";
        }
    }

    private void InteractWithBookCase()
    {
        if (!interactionWindow.activeSelf)
        {
            interactionWindow.SetActive(true);
            lastInteractObjectName = "Bookcase";
            interactText.text = "You see a Bookcase.";
            choiceOneText.text = "1. Read a books.";
            choiceTwoText.text = "2. Ignore it.";
        }
        if (choice == 1)
        {
            if (Random.Range(0, 10) > stats.accuracy)
            {
                stats.movement = 0;
                finalText.text =
                    "You dediced to read a books. You didn't find anything interesting in them and it was only a waste of time. Turn ends.";
                lastInteractObjectInfo =
                    "You dediced to read a books: turn ends.";
                interactionObjectName = "Result";
            }
            else
            {
                stats.movement = 0;
                stats.baseAccuracy += 1;
                stats.baseSpeed += 1;
                finalText.text =
                    "You dediced to read a books. You found interesting information on how to draw a weapon faster and how to use it more accurately. Your accuracy and speed increased by 1, but turn ends.";
                lastInteractObjectInfo =
                    "You dediced to read a books: +1 Accuracy, +1 Speed, Turn ends";
                interactionObjectName = "Result";
            }
        }
        else if (choice == 2)
        {
            finalText.text =
                "You decided ignore bookcase. After a while you noticed that the books started to burn and there was nothing left of them before you started reacting.";
            lastInteractObjectInfo = "You decided to ignore it.";
            interactionObjectName = "Result";
        }
    }

    private void Victory()
    {
        textNumber = 20;
        finalFight = true;
        storyWindow.SetActive(true);
    }

    protected override void SpecialAction<T>(T component)
    {
        Enemy hitEnemy = component as Enemy;
        FightManager.instance.StartFight(this, hitEnemy);
    }
}
