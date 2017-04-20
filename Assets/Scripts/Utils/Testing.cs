using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Testing : MonoBehaviour {

    //public List<NpcBase> Enemies = new List<NpcBase>();
    public InstantiatedLocation Owner;
    bool show = false;
    Rect windowRect;
    public Rect DebugRect;
    public int MissionLevel = 0;
    List<deb> Output = new List<deb>();
    static Testing instance;

    // Use this for initialization
    void Start () {
        //Enemies = Resources.LoadAll<NpcBase>("Enemies").ToList();
        instance = this;
        windowRect = new Rect(Screen.width - 310, 10, 300, 400);
        DebugRect = new Rect(Screen.width - 300, 0, 290, Screen.height);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F2))
            show = !show;

        Output.ForEach((m) => {
            m.TTL -= Time.deltaTime;
        });
        Output.RemoveAll(m => m.TTL < 0f);
    }

    void OnGUI()
    {
        if(show && MyAvatar.Instance.isServer)
        {
            windowRect = GUI.Window(10, windowRect, TestWindow, "test");
        }

        GUILayout.BeginArea(DebugRect);
        GUILayout.FlexibleSpace();
        foreach (var o in Output)
            GUILayout.Label(o.Content);
        if (Output.Count > 0 && GUILayout.Button("clear"))
            Output.Clear();
        GUILayout.EndArea();
    }

    public static void AddDebug(string content)
    {
        instance.Output.Add(new deb() { Content = content });
    }

    class deb
    {
        public string Content;
        public float TTL = 10f;
    }

    void TestWindow(int id)
    {
        foreach (var e in NetworkHelper.Instance.Enemies)
            if (GUILayout.Button("Spawn " + e.name))
            {
                if (Owner == null)
                    Owner = GameObject.FindObjectOfType<InstantiatedLocation>();

                var inst = Instantiate(e, Owner.transform);
                inst.SpawnEnemy(Owner, Owner.FindOpenSpotInLocation());
            }

        MissionLevel = (int)GUILayout.HorizontalSlider(MissionLevel, 0, 20);
        if (GUILayout.Button("Add MIssion"))
        {
            NetworkHelper.Instance.CreateMission("Mission" + Random.Range(0, 100), Random.Range(0, 20000), 0, MissionLevel);
        }
        GUI.DragWindow();
    }
}
