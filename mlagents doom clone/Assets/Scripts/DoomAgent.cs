using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MLAgents;

public class DoomAgent : Agent
{
    public DoomAcademy academy;
    public MazeGenerator mazeGenerator;
    public Rigidbody agentRB;
    public int actions;
    public int goals;
    public int health;
    public int maxHealth;
    public int armor;
    public int maxArmor;
    public float rewards;
    public Text rewardsText;
    public Text actionsText;
    public Text healthText;
    public Text armorText;
    public Text shotgunAmmoText;
    public Text rocketsAmmoText;
    public Text railgunAmmoText;
    public Text activeWeaponText;
    public int currentMazeWidth;
    public int[] heldWeapons;
    public int[] activeWeaponArr;
    public int[] ammoArray;

    public Image healthBar;
    public Image armorBar;
    public Text ammoText;

    public GameObject[] weaponPrefabs;
    public GameObject activeWeaponGO;
    public GameObject activeWeaponRoot;
    public Weapon activeWeaponWeapon;

    public Color goodTextColor;
    public Color badTextColor;

    private bool switchingWeapons;

    private void Start()
    {
        switchingWeapons = false;
        health = 100;
        maxHealth = 100;
        armor = 0;
        maxArmor = 150;

        heldWeapons = new int[4];
        for (int i = 0; i < 4; i++)
        {
            heldWeapons[i] = 0;
        }

        activeWeaponArr = new int[4];
        for (int i = 0; i < 4; i++)
        {
            activeWeaponArr[i] = 0;
        }

        ammoArray = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                ammoArray[i] = int.MaxValue;
            }
            else
            {
                ammoArray[i] = 0;
            }
        }

        agentRB = GetComponent<Rigidbody>();
        actions = 0;
        goals = 0;
        rewards = 0.0f;
        academy = FindObjectOfType<DoomAcademy>();
        MakeMaze();
    }

    public override void InitializeAgent()
    {

    }

    public override void CollectObservations()
    {
        AddVectorObs(actions);
        AddVectorObs(health);
        AddVectorObs(armor);

        for (int i = 0; i < 4; i++)
        {
            AddVectorObs(heldWeapons[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            AddVectorObs(activeWeaponArr[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            AddVectorObs(ammoArray[i]);
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        var moveAction = Mathf.FloorToInt(vectorAction[0]);
        var rotateAction = Mathf.FloorToInt(vectorAction[1]);
        var fireWeaponAction = Mathf.FloorToInt(vectorAction[2]);
        var switchWeaponAction = Mathf.FloorToInt(vectorAction[3]);

        switch (moveAction)
        {
            case 1:
                agentRB.AddRelativeForce(Vector3.forward * 0.5f, ForceMode.VelocityChange);
                break;
            case 2:
                agentRB.AddRelativeForce(Vector3.forward * 1.5f, ForceMode.VelocityChange);
                break;
            case 3:
                agentRB.AddRelativeForce(Vector3.forward * -0.8f, ForceMode.VelocityChange);
                break;
        }

        switch (rotateAction)
        {
            case 1:
                transform.Rotate(0.0f, -5.0f, 0.0f);
                break;
            case 2:
                transform.Rotate(0.0f, 5.0f, 0.0f);
                break;
            case 3:
                transform.Rotate(0.0f, 1.0f, 0.0f);
                break;
            case 4:
                transform.Rotate(0.0f, -1.0f, 0.0f);
                break;
        }

        switch (fireWeaponAction)
        {
            case 1:
                if (activeWeaponGO)
                {
                    activeWeaponWeapon.Shoot(this);
                }
                break;
        }

        switch (switchWeaponAction)
        {
            case 1:
                if (!switchingWeapons)
                {
                    SwitchWeapons();
                }
                break;
        }

        actions++;
        // actionsText.text = actions.ToString();
    }

    public override float[] Heuristic()
    {
        var action = new float[4];

        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 3.0f;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            action[0] = 2.0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2.0f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            action[2] = 1.0f;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            action[3] = 1.0f;
        }

        return action;
    }

    public override void AgentReset()
    {
        health = 100;
        UpdateHealth();
        armor = 0;
        UpdateArmor();

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                ammoArray[i] = int.MaxValue;
            }
            else
            {
                ammoArray[i] = 0;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            heldWeapons[i] = 0;
        }

        for (int i = 0; i < 4; i++)
        {
            activeWeaponArr[i] = 0;
        }

        if (activeWeaponGO)
        {
            Destroy(activeWeaponGO);
        }

        if (activeWeaponWeapon)
        {
            activeWeaponWeapon = null;
        }

        agentRB.velocity = Vector3.zero;
        agentRB.angularVelocity = Vector3.zero;
        mazeGenerator.DestroyMaze();
        actions = 0;
        MakeMaze();
        mazeGenerator.PlaceDoomAgent();
    }
    public void UpdateRewardsText()
    {
        rewardsText.text = rewards.ToString("0.000");
    }

    private void MakeMaze()
    {
        bool obstacles;
        bool makeInterior;

        if ((int)academy.resetParameters["obstacles"] == 0)
        {
            obstacles = false;
        }
        else
        {
            obstacles = true;
        }

        if ((int)academy.resetParameters["interior"] == 0)
        {
            makeInterior = false;
        }
        else
        {
            makeInterior = true;
        }

        int width = Random.Range((int)academy.resetParameters["min_width"], (int)academy.resetParameters["max_width"]);

        if (width % 2 == 0)
        {
            width++;
        }
        currentMazeWidth = width;
        mazeGenerator.CreateMaze(width, width, makeInterior, obstacles);
    }

    public void DoWeaponPickup(PickupWeapon.PickupType weaponType)
    {
        switch ((int)weaponType)
        {
            case 0:
                heldWeapons[0] = 1;
                break;
            case 1:
                heldWeapons[1] = 1;
                ammoArray[1] += 20;
                break;
            case 2:
                heldWeapons[2] = 1;
                ammoArray[2] += 10;
                break;
            case 3:
                heldWeapons[3] = 1;
                ammoArray[3] += 6;
                break;
        }

        StartCoroutine(SwitchWeapon(weaponType));
    }

    public void DoAmmoPickup(PickupAmmo.PickupType ammoType, int amount)
    {
        switch((int)ammoType)
        {
            case 0:
                ammoArray[1] += amount;
                break;
            case 1:
                ammoArray[2] += amount;
                break;
            case 2:
                ammoArray[3] += amount;
                break;
        }

        UpdateDisplayedAmmo();
    }

    public void DoRestorePickup(PickupRestore.PickupType refreshType, int amount)
    {
        switch((int)refreshType)
        {
            case 0:
                if (health + amount <= 100)
                {
                    health += amount;
                    UpdateHealth();
                }
                else
                {
                    health = 100;
                }
                break;
            case 1:
                if (armor + amount <= 150)
                {
                    armor += amount;
                    UpdateArmor();
                }
                else
                {
                    armor = 150;
                }
                break;
        }
    }

    // This may be the ugliest code I've ever written.
    // Someone take a picture.
    private void SwitchWeapons()
    {
        bool hasWeapons = false;

        for (int i = 0; i < 4; i++)
        {
            if (heldWeapons[i] == 1)
            {
                hasWeapons = true;
                break;
            }
        }

        if (hasWeapons)
        {
            int activeIndex = -1;

            for (int i = 0; i < 4; i++)
            {
                if (activeWeaponArr[i] == 1)
                {
                    activeIndex = i;
                    break;
                }
            }

            switch (activeIndex)
            {
                case -1:
                    if (heldWeapons[0] == 1)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Pistol));
                    }
                    else if (heldWeapons[1] == 1 && ammoArray[1] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Shotgun));
                    }
                    else if (heldWeapons[2] == 1 && ammoArray[2] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.RocketLauncher));
                    }
                    else if (heldWeapons[3] == 1 && ammoArray[3] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Railgun));
                    }
                    break;
                case 0:
                    if (heldWeapons[1] == 1 && ammoArray[1] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Shotgun));
                    }
                    else if (heldWeapons[2] == 1 && ammoArray[2] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.RocketLauncher));
                    }
                    else if (heldWeapons[3] == 1 && ammoArray[3] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Railgun));
                    }
                    break;
                case 1:
                    if (heldWeapons[2] == 1 && ammoArray[2] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.RocketLauncher));
                    }
                    else if (heldWeapons[3] == 1 && ammoArray[3] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Railgun));
                    }
                    else if (heldWeapons[0] == 1)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Pistol));
                    }
                    break;
                case 2:    
                    if (heldWeapons[3] == 1 && ammoArray[3] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Railgun));
                    }
                    else if (heldWeapons[0] == 1)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Pistol));
                    }
                    else if (heldWeapons[1] == 1 && ammoArray[1] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Shotgun));
                    }
                    break;
                case 3:
                    if (heldWeapons[0] == 1)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Pistol));
                    }
                    else if (heldWeapons[1] == 1 && ammoArray[1] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.Shotgun));
                    }
                    else if (heldWeapons[2] == 1 && ammoArray[2] > 0)
                    {
                        StartCoroutine(SwitchWeapon(PickupWeapon.PickupType.RocketLauncher));
                    }
                    break;
            }
        }
    }

    private IEnumerator SwitchWeapon(PickupWeapon.PickupType weapon)
    {
        switchingWeapons = true;
        activeWeaponWeapon = null;

        for (int i = 0; i < 4; i++)
        {
            activeWeaponArr[i] = 0;
        }

        Quaternion fromRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Quaternion toRotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);

        Vector3 offScreen = new Vector3(0.0f, -0.5f, 0.0f);

        if (activeWeaponGO != null)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime * 1.5f)
            {
                activeWeaponGO.transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, t);
                activeWeaponGO.transform.localPosition = Vector3.Lerp(Vector3.zero, offScreen, t);
                yield return null;
            }

            Destroy(activeWeaponGO);
        }

        activeWeaponGO = (GameObject)Instantiate(weaponPrefabs[(int)weapon]);
        activeWeaponGO.transform.SetParent(activeWeaponRoot.transform, false);
        activeWeaponGO.transform.localPosition = Vector3.zero;

        for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime * 1.5f)
        {
            activeWeaponGO.transform.localRotation = Quaternion.Lerp(toRotation, fromRotation, t);
            activeWeaponGO.transform.localPosition = Vector3.Lerp(offScreen, Vector3.zero, t);
            yield return null;
        }

        activeWeaponWeapon = activeWeaponGO.GetComponent<Weapon>();

        activeWeaponArr[(int)weapon] = 1;

        UpdateDisplayedAmmo();

        switchingWeapons = false;
    }

    public void GotHit(int value)
    {
        health -= value;
        UpdateHealth();
        if (health <= 0)
        {
            Done();
        }
    }

    public void UpdateDisplayedAmmo()
    {
        if (ammoArray[(int)activeWeaponWeapon.weaponClass] > 1000)
        {
            ammoText.text = "999";
        }
        else
        {
            ammoText.text = ammoArray[(int)activeWeaponWeapon.weaponClass].ToString();
        }
    }

    public void UpdateHealth()
    {
        float healthAmt = (float)health / (float)maxHealth;
        if (healthAmt > 0.0f)
        {
            healthBar.fillAmount = healthAmt;
        }
        else
        {
            healthBar.fillAmount = 0.0f;
        }
    }

    public void UpdateArmor()
    {
        float armorAmt = (float)armor / (float)maxArmor;
        if (armorAmt > 0.0f)
        {
            armorBar.fillAmount = armorAmt;
        }
        else
        {
            armorBar.fillAmount = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("doompickup"))
        {
            c.gameObject.GetComponent<Pickup>().Triggered(this);
        }
    }

    private void OnTriggerStay(Collider c)
    {
        if (c.gameObject.CompareTag("doomenemy"))
        {
            Enemy enemy = c.gameObject.GetComponent<Enemy>();
            if (enemy.IsAgentVisible(transform) && !enemy.isAttacking)
            {
                enemy.Attack(this);
            }
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag("doomenemy"))
        {
            Enemy enemy = c.gameObject.GetComponent<Enemy>();
            enemy.StopAllCoroutines();
            enemy.isAttacking = false;
            enemy.Wander();
        }
    }
}