using UnityEngine;
using System.Collections;

public class MobileGenerator : MonoBehaviour {

    [Header("Game Object Prefabs")]
    public GameObject mobileNode;

    [Header("Game Object References")]
    public GameObject canvas;
    public GameObject mobile;

    [Header("Seeds")]
    public string masterSeed;
    public int generatedSeed;

    public void SetSeedFromMaster()
    {
        SetSeed(masterSeed);
    }

    /// <summary>
    /// Sets the Random.seed to a parsed seed from the inputed string.
    /// </summary>
    /// <param name="_seed">The seed to parse and set as the generated seed.</param>
    public void SetSeed(string _seed)
    {
        char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

        string seed = "";
        foreach (char c in _seed)
        {
            for (int i = 0; i < alphabet.Length; i++)
            {
                if (c == alphabet[i])
                {
                    seed += i.ToString();
                }
            }
        }

        int endSeed;
        bool ok = int.TryParse(seed, out endSeed);

        if (!ok)
        {
            Random.seed = 123456;
            Debug.LogWarning("Defaulting Seed!");
        }
        else
        {
            Random.seed = endSeed;
            Debug.Log("Generated Seed - " + endSeed);
        }

        generatedSeed = Random.seed;

        GenerateMobile();
    }
	
	void Update () {
        masterSeed = canvas.transform.FindChild("seed-input").GetComponent<UnityEngine.UI.InputField>().text;
	}

    void GenerateMobile()
    {
        GameObject head = Instantiate(mobileNode, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        head.GetComponent<Rigidbody>().isKinematic = true;
        head.name = "head";
        head.transform.SetParent(mobile.transform);

        GameObject branch1 = Instantiate(mobileNode, new Vector3(Random.Range(-32, -16), -4, 0), Quaternion.identity) as GameObject;
        branch1.name = "branch1";
        branch1.transform.SetParent(mobile.transform);

        GameObject branch2 = Instantiate(mobileNode, new Vector3(Random.Range(-16, 16), -6, 0), Quaternion.identity) as GameObject;
        branch2.name = "branch2";
        branch2.transform.SetParent(mobile.transform);

        GameObject branch3 = Instantiate(mobileNode, new Vector3(Random.Range(16, 32), -8, 0), Quaternion.identity) as GameObject;
        branch3.name = "branch3";
        branch3.transform.SetParent(mobile.transform);

        head.GetComponent<SpringJoint>().connectedBody = branch1.GetComponent<Rigidbody>();
    }
}
