using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobileGenerator : MonoBehaviour {

    [Header("Game Object Prefabs")]
    public GameObject[] mobileNodes = new GameObject[1];

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
        char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

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
	
    [System.Serializable]
    public class Node
    {
        public GameObject current;
        public List<Node> children;
    }
    public Node mainNode = new Node();

	void Update () {
        masterSeed = canvas.transform.FindChild("seed-input").GetComponent<UnityEngine.UI.InputField>().text;

        for (int i = 0; i < mainNode.children.Count; i++)
        {
            mainNode.children[i].current.GetComponent<LineRenderer>().SetPosition(0, mainNode.children[i].current.transform.position);

            for (int j = 0; j < mainNode.children[i].children.Count; j++)
            {
                mainNode.children[i].children[j].current.GetComponent<LineRenderer>().SetPosition(0, mainNode.children[i].children[j].current.transform.position);
                mainNode.children[i].children[j].current.GetComponent<LineRenderer>().SetPosition(1, mainNode.children[i].current.transform.position);
            }
        }
	}

    void GenerateMobile()
    {
        DeleteChilden(mobile.transform);

        mainNode = new Node();

        GameObject head = Instantiate(mobileNodes[Random.Range(0, mobileNodes.Length)], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        head.GetComponent<Rigidbody>().isKinematic = true;
        head.name = "head";
        head.transform.SetParent(mobile.transform);
        head.transform.position = new Vector3(0,20,0);

        mainNode.current = head;
        mainNode.children = new List<Node>();

        for (int i = 0; i < Random.Range(2, 20); i++)
        {
            HingeJoint joint = head.AddComponent<HingeJoint>() as HingeJoint;
            joint.anchor = new Vector3(0, 0.5f - i, 0);
            joint.axis = new Vector3(0, 1, 0);

            GameObject child = Instantiate(mobileNodes[Random.Range(0, mobileNodes.Length)], new Vector3(head.transform.position.x + Random.Range(4, 16), head.transform.position.y - i * 2, 0), Quaternion.identity) as GameObject;
            child.name = "child - " + i;
            child.transform.SetParent(mobile.transform);
            child.AddComponent<AddForce>();
            joint.connectedBody = child.GetComponent<Rigidbody>();
            LineRenderer line = child.AddComponent<LineRenderer>();
            line.SetColors(Color.black, Color.black);
            line.SetWidth(0.2f, 0.2f);
            line.SetPosition(0, child.transform.position);
            line.SetPosition(1, joint.anchor + head.transform.position);

            Node childNode = new Node();
            childNode.children = new List<Node>();
            childNode.current = child;

            mainNode.children.Add(childNode);

            for (int j = 0; j < Random.Range(0, 4); j++)
            {
                HingeJoint joint2 = child.AddComponent<HingeJoint>() as HingeJoint;
                joint2.anchor = new Vector3(0, 0.5f - j, 0);
                joint2.axis = new Vector3(0, 1, 0);

                GameObject child2 = Instantiate(mobileNodes[Random.Range(0, mobileNodes.Length)], new Vector3(child.transform.position.x + Random.Range(4, 10), child.transform.position.y - j * 2, 0), Quaternion.identity) as GameObject;
                child2.name = "[" + i + "]" + "child - " + j;
                child2.transform.SetParent(mobile.transform);
                child2.AddComponent<AddForce>();
                joint2.connectedBody = child2.GetComponent<Rigidbody>();
                LineRenderer line2 = child2.AddComponent<LineRenderer>();
                line2.SetColors(Color.black, Color.black);
                line2.SetWidth(0.2f, 0.2f);
                line2.SetPosition(0, child2.transform.position);
                line2.SetPosition(1, joint2.anchor + child2.transform.position);

                Node childNode2 = new Node();
                childNode2.current = child2;

                childNode.children.Add(childNode2);
            }
        }
    }

    void DeleteChilden(Transform parent) {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
