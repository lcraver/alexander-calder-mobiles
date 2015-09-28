using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MobileGenerator : MonoBehaviour {

    public Material lineMaterial;

    [Header("Game Object Prefabs")]
    public GameObject[] mobileNodes = new GameObject[1];

    [Header("Game Object References")]
    public GameObject canvas;
    public GameObject seedInput;
    public GameObject seedButton;
    public GameObject mobile;
    
    [System.Serializable]
    public class Colors{
        public List<Color> color = new List<Color>();
        public Color bgColor;
        public Color lineColor;
        public bool used = false;
    }
    [Header("Colors")]
    public List<Colors> colors = new List<Colors>();
    public int colorsUsed = 0;
    public bool allColorsUsed(){ return (colorsUsed >= colors.Count); }

    [Header("Seeds")]
    public string masterSeed;
    public string currentSeed;
    public int generatedSeed;
    public int levels;
    public int curLevel;
    public int type;
    public int color;

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
        currentSeed = _seed;
        char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        string seed = "";
        foreach (char c in _seed.ToLower())
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
            currentSeed = "abcdef";
            Random.seed = 012345;
            Debug.LogWarning("Defaulting Seed!");
        }
        else
        {
            Random.seed = endSeed;
            Debug.Log("Generated Seed - " + endSeed);
        }

        generatedSeed = Random.seed;

        type = Random.Range(0, mobileNodes.Length);
        color = Random.Range(0, colors.Count);
        curLevel = 0;

        this.GetComponent<Camera>().backgroundColor = colors[color].bgColor;

        DeleteChilden(mobile.transform);

        GameObject parent = Instantiate(mobileNodes[type], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        parent.GetComponent<Rigidbody>().isKinematic = true;
        parent.name = "parent";
        parent.transform.SetParent(mobile.transform);
        parent.transform.position = new Vector3(0, 14, 0);
        parent.GetComponent<Renderer>().material.color = Color.black;
        
        GenerateMobile(parent, ref mainNode);

        for (int i = 0; i < mainNode.children.Count; i++)
        {
            Node childNode = mainNode.children[i];
            GenerateLevel(ref childNode);
        }

        GenerateBottomLevel(ref mainNode);
    }

    void GenerateBottomLevel(ref Node main)
    {
        foreach (Node child in main.children)
        {
            if (child.children != null && child.children.Count > 0)
            {
                Node childNode = child;
                GenerateBottomLevel(ref childNode);
            }
            else
            {
                float scale = Random.Range(1, 3);

                GameObject bottomPart;

                if(Random.Range(0,1) < 0.2f)
                    bottomPart = Instantiate(mobileNodes[Random.Range(0, mobileNodes.Length)], new Vector3(child.current.transform.position.x, child.current.transform.position.y - Random.Range(0, 4) - scale - 2, child.current.transform.position.z), Quaternion.identity) as GameObject;
                else
                    bottomPart = Instantiate(mobileNodes[type], new Vector3(child.current.transform.position.x, child.current.transform.position.y - Random.Range(0, 4) - scale - 2, child.current.transform.position.z), Quaternion.identity) as GameObject;
                bottomPart.name = "child-end";
                bottomPart.transform.SetParent(child.current.transform);
                bottomPart.GetComponent<Rigidbody>().isKinematic = true;

                bottomPart.transform.localScale = new Vector3(bottomPart.transform.localScale.x + scale, bottomPart.transform.localScale.y + scale, bottomPart.transform.localScale.z + scale);

                LineRenderer line = bottomPart.AddComponent<LineRenderer>();
                line.materials[0] = lineMaterial;
                line.SetColors(colors[color].lineColor, colors[color].lineColor);
                line.SetWidth(Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.2f));
                line.SetPosition(0, bottomPart.transform.position);
                line.SetPosition(1, child.current.transform.position);

                int randColIndex = Random.Range(0, colors[color].color.Count);
                bottomPart.GetComponent<Renderer>().material.color = colors[color].color[randColIndex];

                Node childNode = new Node();
                childNode.children = new List<Node>();
                childNode.current = bottomPart;

                child.children.Add(childNode);
            }
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
        masterSeed = seedInput.GetComponent<UnityEngine.UI.InputField>().text;

        canvas.transform.FindChild("current-seed").GetComponent<Text>().text = currentSeed;

        if (mainNode != null)
        {
            DisplayLines(ref mainNode);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if(seedInput.activeSelf)
            {
                seedInput.SetActive(false);
                seedButton.SetActive(false);
            }
            else
            {
                seedInput.SetActive(true);
                seedButton.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LSFunctions.Screenshot.TakeScreenshot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            Random.seed = (int)System.DateTime.Now.Ticks;
            string newSeed = alphabet[Random.Range(0, alphabet.Length)].ToString() +
                             alphabet[Random.Range(0, alphabet.Length)].ToString() +
                             alphabet[Random.Range(0, alphabet.Length)].ToString() +
                             alphabet[Random.Range(0, alphabet.Length)].ToString() +
                             alphabet[Random.Range(0, alphabet.Length)].ToString() +
                             alphabet[Random.Range(0, alphabet.Length)].ToString();
            Debug.Log(newSeed);
            SetSeed(newSeed);
        }
	}

    void DisplayLines(ref Node parentNode)
    {
        foreach (Node child in parentNode.children)
        {
            child.current.GetComponent<LineRenderer>().SetPosition(0, child.current.transform.position);
            if(parentNode.spinner)
                child.current.GetComponent<LineRenderer>().SetPosition(1, parentNode.spinner.transform.position);
            else
                child.current.GetComponent<LineRenderer>().SetPosition(1, parentNode.current.transform.position);

            Node childNode = child;
            DisplayLines(ref childNode);
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

        int childrenCount = Random.Range(2, Mathf.Clamp(5 - curLevel, 3, 5));
        Vector3[] childrenPositions = new Vector3[childrenCount];

        float degrees = 360f / childrenCount;

        for (int i = 0; i < childrenCount; i++)
        {
            Vector2 coords = PolarToCartesian(new Vector2(Random.Range(Mathf.Clamp(12 - curLevel, 4, 12), Mathf.Clamp(20 - curLevel * 3, 6, 20)), degrees * i));
            Vector3 position = new Vector3(coords.x, Random.Range(-2, -6), coords.y);
            childrenPositions[i] = position;
        }

        GameObject parentSpinner = Instantiate(mobileNodes[0], new Vector3(parent.transform.position.x, parent.transform.position.y - Random.RandomRange(2,3), parent.transform.position.z), Quaternion.identity) as GameObject;
        parentSpinner.name = "parent-spinner";
        parentSpinner.transform.SetParent(parent.transform);
        parentSpinner.AddComponent<AddForce>();

        parentNode.spinner = parentSpinner;
        
        HingeJoint hingeJoint = parentSpinner.AddComponent<HingeJoint>() as HingeJoint;
        hingeJoint.anchor = new Vector3(0, -2, 0);
        hingeJoint.axis = new Vector3(0, 1, 0);
        hingeJoint.connectedBody = parent.GetComponent<Rigidbody>();

        parentSpinner.GetComponent<Renderer>().material.color = parent.GetComponent<Renderer>().material.color;


        for (int i = 0; i < childrenCount; i++)
        {
            GameObject child = Instantiate(mobileNodes[type], new Vector3(parentSpinner.transform.position.x + childrenPositions[i].x, parentSpinner.transform.position.y + childrenPositions[i].y, parentSpinner.transform.position.z + childrenPositions[i].z), Quaternion.identity) as GameObject;
            child.name = "child - " + i;
            child.transform.SetParent(parentSpinner.transform);
            child.GetComponent<Rigidbody>().isKinematic = true;

            /*FixedJoint joint = child.AddComponent<FixedJoint>() as FixedJoint;
            joint.anchor = new Vector3(0, -2, 0);
            joint.axis = new Vector3(0, 1, 0);
            joint.connectedBody = parentSpinner.GetComponent<Rigidbody>();*/

            LineRenderer line = child.AddComponent<LineRenderer>();
            line.materials[0] = lineMaterial;
            line.SetColors(colors[color].lineColor, colors[color].lineColor);
            line.SetWidth(Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.2f));
            line.SetPosition(0, child.transform.position);
            line.SetPosition(1, parentSpinner.transform.position);

            int randColIndex = Random.Range(0, colors[color].color.Count);
            child.GetComponent<Renderer>().material.color = colors[color].color[randColIndex];

            Node childNode = new Node();
            childNode.children = new List<Node>();
            childNode.current = child;

            parentNode.children.Add(childNode);
        }

        curLevel++;
    }

    void DeleteChilden(Transform parent) {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
