using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightManager : MonoBehaviour
{
    public static FightManager instance = null;

    private Player player;

    private Enemy enemy;

    private Text powerText;

    private Text healthText;

    private Text attackText;

    private Text defenceText;

    private Text speedText;

    private Text agilityText;

    private Text accuracyText;

    private Image portrait;

    private GameObject background;

    private Text enemyName;

    private GameObject enemyInfo;

    private GameObject attackChose;

    private Text fightText;

    public Sprite fightBackgroundSurface;

    public Sprite fightBackgroundDungeon;

    private bool duringFight = false;

    private int action = 0;

    private int pressKey = 0;

    private int textLine = 0;

    private int playerLostHp = 0;

    private int accuracyModifier = 0;

    private int attackModifier = 0;

    private int accAdventage = 0;

    private StringBuilder sb = new StringBuilder();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy (gameObject);
        }
        DontDestroyOnLoad (gameObject);
    }

    public void FindUIFields()
    {
        enemyInfo = GameObject.Find("EnemyFrame");
        powerText = GameObject.Find("EPowerNumber").GetComponent<Text>();
        attackText = GameObject.Find("EAttackPoints").GetComponent<Text>();
        defenceText = GameObject.Find("EDefencePoints").GetComponent<Text>();
        speedText = GameObject.Find("ESpeedPoints").GetComponent<Text>();
        agilityText = GameObject.Find("EAgilityPoints").GetComponent<Text>();
        accuracyText = GameObject.Find("EAccuracyPoints").GetComponent<Text>();
        healthText = GameObject.Find("EHealthPoints").GetComponent<Text>();
        enemyName = GameObject.Find("EName").GetComponent<Text>();
        portrait = GameObject.Find("EImage").GetComponent<Image>();
        fightText = GameObject.Find("FightText").GetComponent<Text>();
        background = GameObject.Find("EBackGround");
        attackChose = GameObject.Find("EAttackChose");
        attackChose.SetActive(false);
        background.SetActive(false);
        enemyInfo.SetActive(false);
    }

    void Start()
    {
        if (!isDuringFight())
        {
            enabled = false;
        }
    }

    void Update()
    {
        GetPressedKey();
        switch (action)
        {
            case 0:
                DetermineWhoStarts();
                break;
            case 1:
                PlayerTurn();
                break;
            case 2:
                EnemyTurn();
                break;
            case 3:
                EndFight();
                break;
        }
        if (action != 3 && this.textLine == 0 && CheckIfFightEnd())
        {
            action = 3;
        }
    }

    private void GetPressedKey()
    {
        if (Input.GetKeyDown("1") || Input.GetKeyDown("[1]"))
        {
            pressKey = 1;
        }
        else if (Input.GetKeyDown("2") || Input.GetKeyDown("[2]"))
        {
            pressKey = 2;
        }
        else if (Input.GetKeyDown("3") || Input.GetKeyDown("[3]"))
        {
            pressKey = 3;
        }
        else if (Input.GetKeyDown("space") || Input.GetKeyDown(KeyCode.Return))
        {
            pressKey = 10;
        }
        else
        {
            pressKey = 0;
        }
    }

    private void DetermineWhoStarts()
    {
        if (textLine == 0)
        {
            sb.Clear();
            sb.Append("Determining who attack first:\n");
            textLine += 1;
        }
        else if (textLine == 1 && pressKey == 10)
        {
            sb.Append("You have " + player.stats.speed + " Speed.\n");
            sb
                .Append(enemy.enemyName +
                " has " +
                enemy.stats.speed +
                " Speed.\n");
            if (player.stats.speed > enemy.stats.speed)
            {
                sb.Append("You attack first.");
                textLine = 2;
            }
            else if (player.stats.speed < enemy.stats.speed)
            {
                sb.Append(enemy.enemyName + " attacks first.");
                textLine = 3;
            }
            else
            {
                sb
                    .Append("You and " +
                    enemy.enemyName +
                    " have the same value of Speed.\n");
                sb.Append("Roll for speed: ");
                textLine = 4;
            }
        }
        else if (textLine == 2 && pressKey == 10)
        {
            action = 1;
            this.textLine = 0;
            sb.Clear();
        }
        else if (textLine == 3 && pressKey == 10)
        {
            action = 2;
            this.textLine = 0;
            sb.Clear();
        }
        else if (textLine == 4 && pressKey == 10)
        {
            int proll = Random.Range(1, 11);
            int eroll = Random.Range(1, 11);
            while (proll == eroll)
            {
                proll = Random.Range(1, 11);
                eroll = Random.Range(1, 11);
            }
            sb.Append(proll + "\n");
            sb.Append(enemy.enemyName + " rolls for Speed: " + eroll + "\n");
            if (proll > eroll)
            {
                sb.Append("You attack first.");
                textLine = 2;
            }
            else
            {
                sb.Append(enemy.enemyName + " attacks first.");
                textLine = 3;
            }
        }
        fightText.text = sb.ToString();
    }

    private void PlayerTurn()
    {
        if (textLine == 0)
        {
            sb.Clear();
            accuracyModifier = 0;
            attackModifier = 0;
            sb.Append("Your turn:\n");
            sb.Append("Choose the type of attack:\n");
            sb.Append("Your choice: ");
            attackChose.SetActive(true);
            textLine += 1;
        }
        else if (
            textLine == 1 && (pressKey == 1 || pressKey == 2 || pressKey == 3)
        )
        {
            attackChose.SetActive(false);
            switch (pressKey)
            {
                case 1:
                    accuracyModifier = 2;
                    attackModifier = -2;
                    sb.Append("Quick Attack\n");
                    break;
                case 2:
                    accuracyModifier = 1;
                    attackModifier = 1;
                    sb.Append("Normal Attack\n");
                    break;
                case 3:
                    accuracyModifier = -2;
                    attackModifier = 2;
                    sb.Append("Power Attack\n");
                    break;
            }
            textLine += 1;
            sb.Append("Roll for accuracy: ");
        }
        else if (textLine == 2 && pressKey == 10)
        {
            int roll = Random.Range(1, 11);
            sb.Append(roll + "\n");
            int realacc;
            if (accuracyModifier > 0)
            {
                realacc = player.stats.accuracy * accuracyModifier;
            }
            else
            {
                realacc = player.stats.accuracy / (-1 * accuracyModifier);
            }
            if (roll == 10)
            {
                sb.Append("Critical miss.");
                textLine = 20;
            }
            else if (roll == 1)
            {
                sb.Append("Critical hit. \n");
                sb.Append("Enemy can't dodge it. His defence is ignored.\n");
                sb.Append("Roll for damage: ");
                textLine = 10;
            }
            else if (roll > realacc)
            {
                sb.Append("You missed your attack.");
                textLine = 20;
            }
            else
            {
                accAdventage = realacc - roll;
                sb.Append(enemy.enemyName + " rolls for Agility: ");
                textLine = 3;
            }
        }
        else if (textLine == 3 && pressKey == 10)
        {
            int roll = Random.Range(1, 11);
            sb.Append(roll + "\n");
            if (
                roll > enemy.stats.agility ||
                (enemy.stats.agility - roll) <= accAdventage
            )
            {
                sb.Append("You hit " + enemy.enemyName + ".\n");
                sb.Append("Roll for damage: ");
                textLine = 4;
            }
            else
            {
                sb.Append(enemy.enemyName + " dodged your attack.");
                textLine = 20;
            }
        }
        else if (textLine == 4 && pressKey == 10)
        {
            int roll = Random.Range(1, 11);
            sb.Append(roll + "\n");
            int realDamage;
            if (attackModifier > 0)
            {
                realDamage = (player.stats.attack + roll) * attackModifier;
            }
            else
            {
                realDamage =
                    (player.stats.attack + roll) / (-1 * attackModifier);
            }
            int damage = realDamage - enemy.stats.defence;
            if (damage <= 0)
            {
                sb.Append("You failed to penetrate enemy armor.\n");
                textLine = 20;
            }
            else
            {
                sb
                    .Append("Dealed " +
                    damage +
                    " Damage to " +
                    enemy.enemyName +
                    ".");
                enemy.stats.healthPoints -= damage;
                UpdateEnemyHPUI();
                textLine = 20;
            }
        }
        else if (textLine == 10 && pressKey == 10)
        {
            int roll = Random.Range(1, 11);
            sb.Append(roll + "\n");
            int damage;
            if (attackModifier > 0)
            {
                damage = (player.stats.attack + roll) * attackModifier;
            }
            else
            {
                damage = (player.stats.attack + roll) / (-1 * attackModifier);
            }
            enemy.stats.healthPoints -= damage;
            sb
                .Append("Dealed " +
                damage +
                " Damage to " +
                enemy.enemyName +
                ".");
            UpdateEnemyHPUI();
            textLine = 20;
        }
        else if (textLine == 20 && pressKey == 10)
        {
            action = 2;
            this.textLine = 0;
            sb.Clear();
        }
        fightText.text = sb.ToString();
    }

    private void EnemyTurn()
    {
        if (textLine == 0)
        {
            sb.Clear();
            accuracyModifier = 0;
            attackModifier = 0;
            sb.Append(enemy.enemyName + " turn:\n");
            sb.Append(enemy.enemyName + " choose the type of attack:\n");
            sb.Append(enemy.enemyName + " choice: ");
            textLine += 1;
        }
        else if (textLine == 1 && (pressKey == 10))
        {
            int roll = Random.Range(1, 4);
            switch (roll)
            {
                case 1:
                    accuracyModifier = 2;
                    attackModifier = -2;
                    sb.Append("Quick Attack\n");
                    break;
                case 2:
                    accuracyModifier = 1;
                    attackModifier = 1;
                    sb.Append("Normal Attack\n");
                    break;
                case 3:
                    accuracyModifier = -2;
                    attackModifier = 2;
                    sb.Append("Power Attack\n");
                    break;
            }
            textLine += 1;
            sb.Append(enemy.enemyName + "rolls for accuracy: ");
        }
        else if (textLine == 2 && pressKey == 10)
        {
            int roll = Random.Range(1, 11);
            sb.Append(roll + "\n");
            int realacc;
            if (accuracyModifier > 0)
            {
                realacc = enemy.stats.accuracy * accuracyModifier;
            }
            else
            {
                realacc = enemy.stats.accuracy / (-1 * accuracyModifier);
            }
            if (roll == 10)
            {
                sb.Append("Critical miss.");
                textLine = 20;
            }
            else if (roll == 1)
            {
                sb.Append("Critical hit. \n");
                sb.Append("You can't dodge it. Your defence is ignored.\n");
                sb.Append(enemy.enemyName + " rolls for damage: ");
                textLine = 10;
            }
            else if (roll > realacc)
            {
                sb.Append(enemy.enemyName + " missed his attack.");
                textLine = 20;
            }
            else
            {
                accAdventage = realacc - roll;
                sb.Append("Your roll for Agility: ");
                textLine = 3;
            }
        }
        else if (textLine == 3 && pressKey == 10)
        {
            int roll = Random.Range(1, 11);
            sb.Append(roll + "\n");
            if (
                roll > player.stats.agility ||
                (player.stats.agility - roll) <= accAdventage
            )
            {
                sb.Append(enemy.enemyName + " hits you.\n");
                sb.Append(enemy.enemyName + " rolls for damage: ");
                textLine = 4;
            }
            else
            {
                sb.Append("You dodged " + enemy.enemyName + "'s attack.");
                textLine = 20;
            }
        }
        else if (textLine == 4 && pressKey == 10)
        {
            int roll = Random.Range(1, 11);
            sb.Append(roll + "\n");
            int realDamage;
            if (attackModifier > 0)
            {
                realDamage = (enemy.stats.attack + roll) * attackModifier;
            }
            else
            {
                realDamage =
                    (enemy.stats.attack + roll) / (-1 * attackModifier);
            }
            int damage = realDamage - player.stats.defence;
            if (damage <= 0)
            {
                sb
                    .Append(enemy.enemyName +
                    " failed to penetrate your armor.\n");
                textLine = 20;
            }
            else
            {
                sb
                    .Append(enemy.enemyName +
                    " dealed " +
                    damage +
                    " Damage to you.");
                player.stats.healthPoints -= damage;
                this.playerLostHp += damage;
                player.updatePlayerHPUI();
                textLine = 20;
            }
        }
        else if (textLine == 10 && pressKey == 10)
        {
            int roll = Random.Range(1, 11);
            sb.Append(roll + "\n");
            int damage;
            if (attackModifier > 0)
            {
                damage = (enemy.stats.attack + roll) * attackModifier;
            }
            else
            {
                damage = (enemy.stats.attack + roll) / (-1 * attackModifier);
            }
            player.stats.healthPoints -= damage;
            sb
                .Append(enemy.enemyName +
                " dealed " +
                damage +
                " Damage to you.");
            player.updatePlayerHPUI();
            this.playerLostHp += damage;
            textLine = 20;
        }
        else if (textLine == 20 && pressKey == 10)
        {
            action = 1;
            this.textLine = 0;
            sb.Clear();
        }
        fightText.text = sb.ToString();
    }

    private void EndFight()
    {
        if (textLine == 0)
        {
            sb.Clear();
            sb.Append("Fight ends:\n");
            if (player.stats.healthPoints <= 0)
            {
                sb.Append("You lost.");
            }
            else
            {
                sb.Append("You win.\n");
                sb
                    .Append("You have taken " +
                    this.playerLostHp +
                    " damage during this fight.\n");
                sb
                    .Append("You gain: " +
                    enemy.stats.CalculateExperience() +
                    " EXP.");
                player.stats.experience += enemy.stats.CalculateExperience();
                string resultText =
                    "You killed this enemy: \n-" +
                    this.playerLostHp +
                    " HP, Gain " +
                    enemy.stats.CalculateExperience() +
                    " EXP.";
                player.updateUIAfterFight(enemy.enemyName, resultText);
                GameManager.instance.RemoveEnemyFromList (enemy);
                enemy.gameObject.SetActive(false);
            }
            textLine += 1;
        }
        else if (textLine == 1 && pressKey == 10)
        {
            enabled = false;
            duringFight = false;
            enemyInfo.SetActive(false);
            background.SetActive(false);
        }
        fightText.text = sb.ToString();
    }

    private bool CheckIfFightEnd()
    {
        if (player.stats.healthPoints <= 0 || enemy.stats.healthPoints <= 0)
        {
            return true;
        }
        return false;
    }

    public void StartFight(Player player, Enemy enemy)
    {
        this.action = 0;
        this.textLine = 0;
        this.playerLostHp = 0;
        this.player = player;
        this.enemy = enemy;
        duringFight = true;
        enemy.stats.calculateStats();
        player.stats.calculateActualStats();
        SetUIForFight();
        enabled = true;
    }

    public bool isDuringFight()
    {
        return this.duringFight;
    }

    private void SetUIForFight()
    {
        enemyInfo.SetActive(true);
        background.SetActive(true);
        portrait.sprite = enemy.GetComponent<SpriteRenderer>().sprite;
        powerText.text = enemy.stats.power.ToString();
        attackText.text = enemy.stats.attack.ToString();
        defenceText.text = enemy.stats.defence.ToString();
        speedText.text = enemy.stats.speed.ToString();
        agilityText.text = enemy.stats.agility.ToString();
        accuracyText.text = enemy.stats.accuracy.ToString();
        enemyName.text = enemy.enemyName;
        if (GameManager.instance.level == 0)
        {
            background.GetComponent<Image>().sprite = fightBackgroundSurface;
        }
        else
        {
            background.GetComponent<Image>().sprite = fightBackgroundDungeon;
        }
        UpdateEnemyHPUI();
    }

    private void UpdateEnemyHPUI()
    {
        healthText.text =
            enemy.stats.healthPoints + "/" + enemy.stats.maxHealth;
    }
}
