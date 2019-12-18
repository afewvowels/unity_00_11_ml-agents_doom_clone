using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    public Material matFloor;
    public Material matCeiling;
    public Material matWall;
    public Material matTop;

    public Color enemyEasy;
    public Color enemyMedium;
    public Color enemyHard;
    public Color enemyBoss;

    public GameObject enemyPrefab;
    public GameObject enemyProjectilePrefab;

    public GameObject pickupPistol;
    public GameObject pickupShotgun;
    public GameObject pickupRocketLauncher;
    public GameObject pickupRailgun;

    public GameObject pickupArmor;
    public GameObject pickupHealth;
    public GameObject pickupAmmo;

    public GameObject sceneRoot;
    public GameObject mazeObject;
    public GameObject instantiatedContainer;
    public GameObject uiCanvas;
    public GameObject doomAgent;

    public int enemyCount;
    public int[] levelDimensions;
    public float cellWidth;

    public Text displayText;

    private MazeDataGenerator mazeDataGenerator;
    private MazeMeshGenerator mazeMeshGenerator;
    public bool flag = true;
    public static bool isOrthographic = true;

    public int[] doomGuyStart;

    public List<int[]> occupiedCells;

    public int[,] data
    {
        get; private set;
    }

    private void Awake()
    {
        mazeDataGenerator = new MazeDataGenerator();
        mazeMeshGenerator = new MazeMeshGenerator();
        doomGuyStart = new int[2];
        levelDimensions = new int[2];
    }

    public void CreateMaze(int rows, int cols, int enemyCount, bool makeInterior, bool obstacles)
    {
        ClearLevel();
        this.enemyCount = enemyCount;
        occupiedCells = new List<int[]>();
        levelDimensions[0] = rows;
        levelDimensions[1] = cols;
        cellWidth = mazeMeshGenerator.width;

        GenerateNewMaze(rows, cols, makeInterior);
        DisplayMaze();
        PlaceDoomAgent();
        PlacePistol();

        if (enemyCount > 5)
        {
            PlaceShotgun();
        }

        if (enemyCount > 10)
        {
            PlaceRocketLauncher();
        }

        if (enemyCount > 15)
        {
            PlaceRailgun();
        }

        PlaceAmmo(enemyCount);
        PlaceRestoreItems(enemyCount);
        PlaceEnemies(enemyCount);
    }

    public void DestroyMaze()
    {
        Destroy(mazeObject);
    }

    public void GenerateNewMaze(int rows, int cols, bool makeInterior)
    {
        if (rows % 2 == 0 || cols % 2 == 0)
        {
            Debug.Log("Works better with odd row & col counts");
            return;
        }

        data = mazeDataGenerator.FromDimensions(rows, cols, makeInterior);
    }

    public void PrintMaze()
    {
        flag = !flag;

        int[,] maze = data;

        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += ". ";
                }
                else if (maze[i, j] == 2)
                {
                    msg += "# ";
                }
                else if (maze[i, j] == 3)
                {
                    msg += "X ";
                }
                else if (maze[i, j] == 4)
                {
                    msg += "S ";
                }
                else if (maze[i, j] == 5)
                {
                    msg += "E ";
                }
                else
                {
                    msg += "= ";
                }
            }
            if (i != 0)
            {
                msg += "\n";
            }
        }

        displayText.text = msg;
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        //go.transform.position = Vector3.zero;
        go.name = "DoomLevel";
        go.tag = "levelmesh";
        go.transform.SetParent(sceneRoot.transform, false);
        go.transform.localPosition = sceneRoot.transform.position;

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = mazeMeshGenerator.FromData(data);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[4] { matFloor, matCeiling, matWall, matTop };

        mazeObject = go;
    }

    public static void ChangeCameraProjection(bool isOrthographic)
    {
        MazeGenerator.isOrthographic = isOrthographic;
    }

    public void PlaceUICanvas()
    {
        float uioffsetX = 0.0f;
        float uioffsetZ = 0.0f;

        float uirootX = 0.0f;
        float uirootZ = 0.0f;

        Quaternion rotationOffset = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        if (!isOrthographic)
        {
            for (int i = 0; i < sceneRoot.transform.childCount; i++)
            {
                if (sceneRoot.transform.GetChild(i).gameObject.CompareTag("mazemesh"))
                {
                    // uioffsetX = sceneRoot.transform.GetChild(i).gameObject.GetComponent<MeshCollider>().bounds.center.x;
                    uioffsetX = sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.max.x -
                                sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.extents.x;
                    uioffsetZ = sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.max.z;

                    uirootX = sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.min.x;
                    uirootZ = sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.min.z;

                    rotationOffset = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < sceneRoot.transform.childCount; i++)
            {
                if (sceneRoot.transform.GetChild(i).gameObject.CompareTag("mazemesh"))
                {
                    uioffsetX = sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.min.x;
                    uioffsetZ = sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.min.z;

                    uirootX = sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.min.x;
                    uirootZ = sceneRoot.transform.GetChild(i).gameObject.GetComponent<Renderer>().bounds.min.z;

                    uioffsetX += 5.0f;
                    uioffsetZ += 2.5f;

                    uirootX += 5.0f;
                    uirootZ += 2.5f;

                    rotationOffset = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                    break;
                }
            }
        }
            
        Vector3 uiPos = new Vector3(uioffsetX, 8.0f, uioffsetZ);
        uiCanvas.transform.parent.transform.position = new Vector3(uirootX, 0.0f, uirootZ);
        uiCanvas.transform.position = uiPos;
        uiCanvas.transform.localRotation = rotationOffset;
    }

    public void PlaceDoomAgent()
    {
        int row = UnityEngine.Random.Range(1, levelDimensions[0] - 2);
        int col = UnityEngine.Random.Range(1, levelDimensions[1] - 2);

        while (data[row, col] != 0)
        {
            row = UnityEngine.Random.Range(1, levelDimensions[0] - 2);
            col = UnityEngine.Random.Range(1, levelDimensions[1] - 2);
        }

        Vector3 doomGuyPosition = new Vector3(row * cellWidth, 1.0f, col * cellWidth);

        doomAgent.transform.localPosition = doomGuyPosition;
        doomAgent.transform.localRotation = Quaternion.Euler(0.0f, 360.0f * UnityEngine.Random.value, 0.0f);

        doomGuyStart[0] = row;
        doomGuyStart[1] = col;

        occupiedCells.Add(doomGuyStart);
    }

    private void PlacePistol()
    {
        bool placed = false;

        int[] location = new int[2];

        while(!placed)
        {
            int randomDirection = UnityEngine.Random.Range(0, 4);

            Vector3 pickupPosition = Vector3.zero;
            GameObject pistolPickup = (GameObject)Instantiate(pickupPistol);
            pistolPickup.transform.SetParent(instantiatedContainer.transform, false);

            switch (randomDirection)
            {
                case 0:
                    location[0] = doomGuyStart[0] - 1;
                    location[1] = doomGuyStart[1];

                    if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                    {
                        pickupPosition = new Vector3((doomGuyStart[0] - 1) * cellWidth, 0.0f, doomGuyStart[1] * cellWidth);
                        pistolPickup.transform.position = pickupPosition;
                        placed = true;
                    }
                    else
                    {
                        continue;
                    }
                    break;
                case 1:
                    location[0] = doomGuyStart[0] + 1;
                    location[1] = doomGuyStart[1];

                    if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                    {
                        pickupPosition = new Vector3((doomGuyStart[0] + 1) * cellWidth, 0.0f, doomGuyStart[1] * cellWidth);
                        pistolPickup.transform.position = pickupPosition;
                        placed = true;
                    }
                    else
                    {
                        continue;
                    }
                    break;
                case 2:
                    location[0] = doomGuyStart[0];
                    location[1] = doomGuyStart[1] - 1;

                    if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                    {
                        pickupPosition = new Vector3(doomGuyStart[0] * cellWidth, 0.0f, (doomGuyStart[1] - 1) * cellWidth);
                        pistolPickup.transform.position = pickupPosition;
                        placed = true;
                    }
                    else
                    {
                        continue;
                    }
                    break;
                case 3:
                    location[0] = doomGuyStart[0];
                    location[1] = doomGuyStart[1] + 1;

                    if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                    {
                        pickupPosition = new Vector3(doomGuyStart[0] * cellWidth, 0.0f, (doomGuyStart[1] + 1) * cellWidth);
                        pistolPickup.transform.position = pickupPosition;
                        placed = true;
                    }
                    else
                    {
                        continue;
                    }
                    break;
            }
        }
    }

    private void PlaceShotgun()
    {
        bool placed = false;

        int[] location = new int[2];

        while (!placed)
        {
            location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
            location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);

            if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
            {
                GameObject shotgunPickup = (GameObject)Instantiate(pickupShotgun);
                shotgunPickup.transform.SetParent(instantiatedContainer.transform, false);
                Vector3 newPosition = new Vector3(location[0] * mazeMeshGenerator.width, 0.0f, location[1] * mazeMeshGenerator.width);
                shotgunPickup.transform.position = newPosition;
                placed = true;
            }
        }
    }

    private void PlaceRocketLauncher()
    {
        bool placed = false;

        int[] location = new int[2];

        while (!placed)
        {
            location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
            location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);

            if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
            {
                GameObject rocketLauncherPickup = (GameObject)Instantiate(pickupRocketLauncher);
                rocketLauncherPickup.transform.SetParent(instantiatedContainer.transform, false);
                Vector3 newPosition = new Vector3(location[0] * mazeMeshGenerator.width, 0.0f, location[1] * mazeMeshGenerator.width);
                rocketLauncherPickup.transform.position = newPosition;
                placed = true;
            }
        }
    }

    private void PlaceRailgun()
    {
        bool placed = false;

        int[] location = new int[2];

        while (!placed)
        {
            location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
            location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);

            if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
            {
                GameObject railgunPickup = (GameObject)Instantiate(pickupRailgun);
                railgunPickup.transform.SetParent(instantiatedContainer.transform, false);
                Vector3 newPosition = new Vector3(location[0] * mazeMeshGenerator.width, 0.0f, location[1] * mazeMeshGenerator.width);
                railgunPickup.transform.position = newPosition;
                placed = true;
            }
        }
    }

    public void PlaceAmmo(int enemyCount)
    {
        int shotgunAmmoCount = 0;
        int rocketAmmoCount = 0;
        int railgunAmmoCount = 0;

        if (enemyCount > 15)
        {
            shotgunAmmoCount = 7;
            rocketAmmoCount = 5;
            railgunAmmoCount = 3;
        }
        else if (enemyCount > 10)
        {
            shotgunAmmoCount = 5;
            rocketAmmoCount = 3;
        }
        else if (enemyCount > 5)
        {
            shotgunAmmoCount = 3;
        }

        bool isPlaced;

        float cellWidth = mazeMeshGenerator.width;

        for (int i = 0; i < shotgunAmmoCount; i++)
        {
            isPlaced = false;

            while (!isPlaced)
            {
                int[] location = new int[2];

                location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
                location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);

                if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                {
                    Vector3 newPosition = new Vector3(location[0] * cellWidth, 0.0f, location[1] * cellWidth);
                    PickupAmmo ammo = Instantiate(pickupAmmo).GetComponent<PickupAmmo>();
                    ammo.transform.position = newPosition;
                    ammo.SetToShotgun();
                    ammo.transform.SetParent(instantiatedContainer.transform, false);
                    isPlaced = true;
                }
            }
        }

        for (int i = 0; i < rocketAmmoCount; i++)
        {
            isPlaced = false;

            while (!isPlaced)
            {
                int[] location = new int[2];

                location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
                location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);

                if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                {
                    Vector3 newPosition = new Vector3(location[0] * cellWidth, 0.0f, location[1] * cellWidth);
                    PickupAmmo ammo = Instantiate(pickupAmmo).GetComponent<PickupAmmo>();
                    ammo.transform.position = newPosition;
                    ammo.SetToRocket();
                    ammo.transform.SetParent(instantiatedContainer.transform, false);
                    isPlaced = true;
                }
            }
        }

        for (int i = 0; i < railgunAmmoCount; i++)
        {
            isPlaced = false;

            while (!isPlaced)
            {
                int[] location = new int[2];

                location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
                location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);

                if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                {
                    Vector3 newPosition = new Vector3(location[0] * cellWidth, 0.0f, location[1] * cellWidth);
                    PickupAmmo ammo = Instantiate(pickupAmmo).GetComponent<PickupAmmo>();
                    ammo.transform.position = newPosition;
                    ammo.SetToRailgun();
                    ammo.transform.SetParent(instantiatedContainer.transform, false);
                    isPlaced = true;
                }
            }
        }
    }

    public void PlaceRestoreItems(int enemyCount)
    {
        int healthCount;
        int armorCount;

        if (enemyCount > 15)
        {
            healthCount = 20;
            armorCount = 15;
        }
        else if (enemyCount > 10)
        {
            healthCount = 15;
            armorCount = 10;
        }
        else if (enemyCount > 5)
        {
            healthCount = 10;
            armorCount = 8;
        }
        else
        {
            healthCount = 7;
            armorCount = 5;
        }

        bool isPlaced;

        float cellWidth = mazeMeshGenerator.width;

        for (int i = 0; i < healthCount; i++)
        {
            isPlaced = false;

            while(!isPlaced)
            {
                int[] location = new int[2];

                location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
                location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);

                if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                {
                    Vector3 newPosition = new Vector3(location[0] * cellWidth, 0.0f, location[1] * cellWidth);
                    PickupRestore item = Instantiate(pickupHealth).GetComponent<PickupRestore>();
                    item.transform.position = newPosition;
                    item.transform.SetParent(instantiatedContainer.transform, false);
                    item.SetToHealth();
                    isPlaced = true;
                }
            }
        }

        for (int i = 0; i < armorCount; i++)
        {
            isPlaced = false;

            while(!isPlaced)
            {
                int[] location = new int[2];

                location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
                location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);

                if (data[location[0], location[1]] == 0 && !IsAlreadyInList(location))
                {
                    Vector3 newPosition = new Vector3(location[0] * cellWidth, 0.0f, location[1] * cellWidth);
                    PickupRestore item = Instantiate(pickupArmor).GetComponent<PickupRestore>();
                    item.transform.position = newPosition;
                    item.transform.SetParent(instantiatedContainer.transform, false);
                    item.SetToArmor();
                    isPlaced = true;
                }
            }
        }
    }

    public void PlaceEnemies(int enemyCount)
    {
        float bossChance = 0.0f;
        float hardChance = 0.0f;
        float mediumChance = 0.0f;

        if (enemyCount > 15)
        {
            bossChance = 0.95f;
            hardChance = 0.8f;
            mediumChance = 0.4f;
        }
        else if (enemyCount > 10)
        {
            bossChance = 0.97f;
            hardChance = 0.85f;
            mediumChance = 0.5f;
        }
        else if (enemyCount > 5)
        {
            bossChance = 1.0f;
            hardChance = 0.9f;
            mediumChance = 0.7f;
        }
        else
        {
            bossChance = 1.0f;
            hardChance = 1.0f;
            mediumChance = 0.8f;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            bool isPlaced = false;

            while (!isPlaced)
            {
                int[] location = new int[2];
                location[0] = UnityEngine.Random.Range(1, data.GetUpperBound(0));
                location[1] = UnityEngine.Random.Range(1, data.GetUpperBound(1));

                if (!IsAlreadyInList(location))
                {
                    GameObject enemy = (GameObject)Instantiate(enemyPrefab);
                    enemy.transform.SetParent(instantiatedContainer.transform, false);

                    float chance = UnityEngine.Random.value;

                    if (chance > bossChance)
                    {
                        enemy.AddComponent<EnemyBoss>();
                        enemy.GetComponentInChildren<MeshRenderer>().material.color = enemyBoss;
                    }
                    else if (chance > hardChance)
                    {
                        enemy.AddComponent<EnemyHard>();
                        enemy.GetComponentInChildren<MeshRenderer>().material.color = enemyHard;
                    }
                    else if (chance > mediumChance)
                    {
                        enemy.AddComponent<EnemyMedium>();
                        enemy.GetComponentInChildren<MeshRenderer>().material.color = enemyMedium;
                    }
                    else
                    {
                        enemy.AddComponent<EnemyEasy>();
                        enemy.GetComponentInChildren<MeshRenderer>().material.color = enemyEasy;
                    }

                    enemy.GetComponent<Enemy>().mazeGenerator = this;
                    enemy.GetComponent<Enemy>().enemyProjectile = enemyProjectilePrefab;
                    enemy.GetComponent<Enemy>().Wander();

                    Vector3 newPosition = new Vector3(location[0] * mazeMeshGenerator.width, 0.0f, location[1] * mazeMeshGenerator.width);
                    enemy.transform.localPosition = newPosition;
                    enemy.transform.localRotation = Quaternion.Euler(0.0f, 360.0f * chance, 0.0f);
                    isPlaced = true;
                }
            }
        }
    }

    public bool IsAlreadyInList(int[] input)
    {
        foreach (int[] occupiedCell in occupiedCells)
        {
            if (occupiedCell[0] == input[0] && occupiedCell[1] == input[1])
            {
                return true;
            }
        }

        occupiedCells.Add(input);
        return false;
    }

    public void EnemyDied()
    {
        enemyCount--;
        
        if(enemyCount <= 0)
        {
            doomAgent.GetComponent<DoomAgent>().AddReward(1.0f);
            doomAgent.GetComponent<DoomAgent>().Done();
        }
    }
    private void ClearLevel()
    {
        for (int i = 0; i < instantiatedContainer.transform.childCount; i++)
        {
            Destroy(instantiatedContainer.transform.GetChild(i).gameObject);
        }
    }
}
