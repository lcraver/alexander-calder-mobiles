using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobileGenerator : MonoBehaviour {

    public Material lineMaterial;

    [Header("Game Object Prefabs")]
    public GameObject[] mobileNodes = new GameObject[1];

    [Header("Game Object References")]
    public GameObject canvas;
    public GameObject mobile;

    [Header("Seeds")]
    public string masterSeed;
    public int generatedSeed;
    public int levels;

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

        DeleteChilden(mobile.transform);

        GameObject parent = Instantiate(mobileNodes[Random.Range(0, mobileNodes.Length)], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        parent.GetComponent<Rigidbody>().isKinematic = true;
        parent.name = "parent";
        parent.transform.SetParent(mobile.transform);
        parent.transform.position = new Vector3(0, 20, 0);
        
        GenerateMobile(parent, ref mainNode);

        for (int i = 0; i < mainNode.children.Count; i++)
        {
            Node childNode = mainNode.children[i];
            GenerateLevel(ref childNode);
        }
    }

    void GenerateLevel(ref Node childNode)
    {
        GenerateMobile(childNode.current, ref childNode);
    }
	
    [System.Serializable]
    public class Node
    {
        public GameObject current;
        public List<Node> children;
        public GameObject spinner;
        public float weight;
        public Vector3 distance;
    }
    public Node mainNode = new Node();

	void Update () {
        masterSeed = canvas.transform.FindChild("seed-input").GetComponent<UnityEngine.UI.InputField>().text;

        if (mainNode != null)
        {
            DisplayLines(mainNode);
            for (int i = 0; i < mainNode.children.Count; i++)
            {
                DisplayLines(mainNode.children[i]);
            }
        }

        /*for (int i = 0; i < mainNode.children.Count; i++)
        {
            mainNode.children[i].current.GetComponent<LineRenderer>().SetPosition(0, mainNode.children[i].current.transform.position);
            mainNode.children[i].current.GetComponent<LineRenderer>().SetPosition(1, mainNode.current.transform.position);

            /*for (int j = 0; j < mainNode.children[i].children.Count; j++)
            {
                mainNode.children[i].children[j].current.GetComponent<LineRenderer>().SetPosition(0, mainNode.children[i].children[j].current.transform.position);
                mainNode.children[i].children[j].current.GetComponent<LineRenderer>().SetPosition(1, mainNode.children[i].current.transform.position);
            }*/
        //}
	}

    void DisplayLines(Node parentNode)
    {
        for (int i = 0; i < parentNode.children.Count; i++)
        {
            parentNode.children[i].current.GetComponent<LineRenderer>().SetPosition(0, parentNode.children[i].current.transform.position);
            parentNode.children[i].current.GetComponent<LineRenderer>().SetPosition(1, parentNode.spinner.transform.position);
        }
    }

    Vector3 PolarToCartesian(Vector2 polar)
    {
        polar.y = Mathf.Deg2Rad * polar.y;
        float x = polar.x * Mathf.Cos(polar.y);
        float y = polar.x * Mathf.Sin(polar.y);
 
        return new Vector2(x,y);
    }

    void GenerateMobile(GameObject parent, ref Node parentNode)
    {
        //parentNode = new Node();

        parentNode.current = parent;
        parentNode.children = new List<Node>();

        int childrenCount = Random.Range(3,4);
        Vector3[] childrenPositions = new Vector3[childrenCount];

        float degrees = 360f / childrenCount;
        float weight = Random.Range(1,3);

        for (int i = 0; i < childrenCount; i++)
        {
            Debug.Log(degrees * i + "- child - " + i);
            Vector2 coords = PolarToCartesian(new Vector2(Random.Range(4, 16), degrees * i));
            Vector3 position = new Vector3(coords.x, Random.Range(-2, -6), coords.y);
            childrenPositions[i] = position;
        }

        GameObject parentSpinner = Instantiate(mobileNodes[0], new Vector3(parent.transform.position.x, parent.transform.position.y - 2, parent.transform.position.z), Quaternion.identity) as GameObject;
        parentSpinner.name = "parent-spinner";
        parentSpinner.transform.SetParent(parent.transform);
        parentSpinner.AddComponent<AddForce>();

        parentNode.spinner = parentSpinner;
        
        HingeJoint hingeJoint = parentSpinner.AddComponent<HingeJoint>() as HingeJoint;
        hingeJoint.anchor = new Vector3(0, -2, 0);
        hingeJoint.axis = new Vector3(0, 1, 0);
        hingeJoint.connectedBody = parent.GetComponent<Rigidbody>();


        for (int i = 0; i < childrenCount; i++)
        {
            GameObject child = Instantiate(mobileNodes[Random.Range(0, mobileNodes.Length)], new Vector3(parent.transform.position.x + childrenPositions[i].x, parent.transform.position.y + childrenPositions[i].y, childrenPositions[i].z), Quaternion.identity) as GameObject;
            child.name = "child - " + i;
            child.transform.SetParent(parentSpinner.transform);
            child.GetComponent<Rigidbody>().isKinematic = true;

            /*FixedJoint joint = child.AddComponent<FixedJoint>() as FixedJoint;
            joint.anchor = new Vector3(0, -2, 0);
            joint.axis = new Vector3(0, 1, 0);
            joint.connectedBody = parentSpinner.GetComponent<Rigidbody>();*/

            LineRenderer line = child.AddComponent<LineRenderer>();
            line.materials[0] = lineMaterial;
            //line.SetColors(Color.black, Color.black);
            line.SetWidth(0.2f, 0.2f);
            line.SetPosition(0, child.transform.position);
            line.SetPosition(1, parentSpinner.transform.position);

            Node childNode = new Node();
            childNode.children = new List<Node>();
            childNode.current = child;

            parentNode.children.Add(childNode);
        }
    }

    void DeleteChilden(Transform parent) {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
