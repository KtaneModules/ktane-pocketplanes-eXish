using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using System;

public class PocketPlanesScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;
    public KMSelectable[] buttons;

    public TextMesh[] destinationTexts;
    public TextMesh[] cargoTexts;
    public GameObject[] scenes;
    public GameObject[] bitizenSelectors;
    public Renderer[] unloadButtons;
    public Renderer[] planeButtons;
    public Renderer backing;
    public Material[] planeMats;
    public Material[] backgrounds;

    private string[] bitizenNames = new string[] { "Eugene Chavez", "Samantha Wade", "Glen Perry" };
    private string[] destinations = new string[] { "Detroit", "Los Angeles", "Chicago", "New York", "Washington DC", "Nuuk", "Reykjavik", "Goose Bay", "Ontario", "London" };
    private string[] eugeneCorr = new string[] { "Aw, yeah! Party in XXX!", "My friend just got a new crib in XXX, time for a housewarming party!", "Aww, I’m my friend’s best man for his wedding in XXX!", "Anyone know how to get to the football stadium in XXX?", "What time does the rave start in XXX’s town square tomorrow?" };
    private string[] eugeneIncorr = new string[] { "Christmas shopping time in XXX!", "Aw, yeah! I just got a free business trip to XXX!", "All-expenses paid snore-fest in XXX again…", "Oooh, the mall in XXX is half off EVERYTHING right now!" };
    private string[] samCorr = new string[] { "I’m going off to XXX to get new shoes, mine got taken at the TSA :/", "Woohoooo! 2k spending spree in XXX!", "What’s that new clothes shop in XXX called again?", "Getting my hubby a present in XXX :)", "A new mall in XXX just opened and I HAVE to go to it!" };
    private string[] samIncorr = new string[] { "Time to go to my friend’s wedding in XXX!", "Oooh, look, XXX has a concert going on right now!", "Work is sending me to XXX for like five meetings. Yawn.", "Why is it every time I go to XXX weather is bad?" };
    private string[] glenCorr = new string[] { "I’ve been requested to represent my company in XXX.", "The finance convention in XXX should bring wonderful returns.", "Business class for a business trip to XXX. #allexpensespaid", "Hope to meet new partners in XXX.", "Ten meetings, ten days. Sounds like my kinda trip to XXX." };
    private string[] glenIncorr = new string[] { "The wife wants a new ring from XXX.", "Well, I’ve been invited to my friend’s wedding, in XXX.", "How much does it cost to purchase a meal in XXX?", "Asking for a friend: What hotel in XXX is the best?" };
    private string[] planeNames = new string[] { "Bearclaw-C", "Kangaroo-P", "Airvan-M", "Birchcraft-M" };
    private string[] itemNames = new string[] { "Live Bees", "Tentacles", "Pianos", "Bandsaws", "Uranium Rods", "Rare Vase", "Leaf Blowers", "Fried Chicken", "Pumpkins" };
    private string[] passNames = new string[] { "Max D.", "Nick S.", "Victor L.", "Brendan K.", "David P-V.", "Thomas S.", "Caleb C.", "Simon J.", "Alex J." };
    private List<int> planeIndexes = new List<int>() { 0, 1, 2, 3 };
    private List<int> orderSelected = new List<int>();
    private List<int> correctOrder = new List<int>();
    private int selectedBitizen = -1;
    private int correctBitizen = -1;
    private int correctPlane = -1;
    private int stage = 0;
    private string dayOfWeek = "";
    private string destination = "";
    private bool firstInter = true;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        foreach (KMSelectable obj in buttons)
        {
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
        DateTime curTime = DateTime.Now;
        dayOfWeek = curTime.DayOfWeek.ToString();
    }

    void Update()
    {
        if (!moduleSolved)
        {
            DateTime curTime = DateTime.Now;
            if (curTime.Hour >= 19 || curTime.Hour < 7)
            {
                backing.material = backgrounds[2];
            }
            else
            {
                backing.material = backgrounds[0];
            }
        }
        else
        {
            DateTime curTime = DateTime.Now;
            if (curTime.Hour >= 19 || curTime.Hour < 7)
            {
                backing.material = backgrounds[3];
            }
            else
            {
                backing.material = backgrounds[1];
            }
        }
    }

    void Start () {
        Debug.LogFormat("[Pocket Planes #{0}] --Destination City--", moduleId);
        List<string> destNames = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            string name = destinations[UnityEngine.Random.Range(0, destinations.Length)];
            while (destNames.Contains(name))
                name = destinations[UnityEngine.Random.Range(0, destinations.Length)];
            destNames.Add(name);
        }
        correctBitizen = UnityEngine.Random.Range(0, 3);
        destination = destNames[correctBitizen];
        string[] temps = new string[3];
        switch (correctBitizen)
        {
            case 0:
                temps[0] = eugeneCorr[UnityEngine.Random.Range(0, eugeneCorr.Length)].Replace("XXX", destNames[0]);
                temps[1] = samIncorr[UnityEngine.Random.Range(0, samIncorr.Length)].Replace("XXX", destNames[1]);
                temps[2] = glenIncorr[UnityEngine.Random.Range(0, glenIncorr.Length)].Replace("XXX", destNames[2]);
                break;
            case 1:
                temps[0] = eugeneIncorr[UnityEngine.Random.Range(0, eugeneIncorr.Length)].Replace("XXX", destNames[0]);
                temps[1] = samCorr[UnityEngine.Random.Range(0, samCorr.Length)].Replace("XXX", destNames[1]);
                temps[2] = glenIncorr[UnityEngine.Random.Range(0, glenIncorr.Length)].Replace("XXX", destNames[2]);
                break;
            case 2:
                temps[0] = eugeneIncorr[UnityEngine.Random.Range(0, eugeneIncorr.Length)].Replace("XXX", destNames[0]);
                temps[1] = samIncorr[UnityEngine.Random.Range(0, samIncorr.Length)].Replace("XXX", destNames[1]);
                temps[2] = glenCorr[UnityEngine.Random.Range(0, glenCorr.Length)].Replace("XXX", destNames[2]);
                break;
        }
        for (int i = 0; i < 3; i++)
        {
            if (i != 2)
            {
                if (temps[i].Length > 46)
                {
                    int index = 45;
                    while (temps[i][index] != ' ')
                    {
                        index--;
                    }
                    temps[i] = temps[i].Substring(0, index) + "\n" + temps[i].Substring(index + 1, temps[i].Length - index - 1);
                }
            }
            else
            {
                if (temps[i].Length > 40)
                {
                    int index = 39;
                    while (temps[i][index] != ' ')
                    {
                        index--;
                    }
                    temps[i] = temps[i].Substring(0, index) + "\n" + temps[i].Substring(index + 1, temps[i].Length - index - 1);
                }
                else
                {
                    temps[i] += "\n";
                }
            }
            destinationTexts[i].text = temps[i];
        }
        Debug.LogFormat("[Pocket Planes #{0}] The displayed BitBook posts are as follows:\nEugene Chavez - {1}\nSamantha Wade - {2}\nGlen Perry - {3}", moduleId, destinationTexts[0].text.Replace("\n", " "), destinationTexts[1].text.Replace("\n", " "), destinationTexts[2].text.Replace("\n", " "));
        Debug.LogFormat("[Pocket Planes #{0}] The person who made a post with their corresponding action was {1}, therefore the destination city is {2}", moduleId, bitizenNames[correctBitizen], destNames[correctBitizen]);
        Debug.LogFormat("[Pocket Planes #{0}] --Plane--", moduleId);
        planeIndexes = planeIndexes.Shuffle();
        for (int i = 0; i < 4; i++)
        {
            planeButtons[i].material = planeMats[planeIndexes[i]];
        }
        int[] planeVals = new int[4];
        if (bomb.GetSerialNumberLetters().Contains('A') || bomb.GetSerialNumberLetters().Contains('B') || bomb.GetSerialNumberLetters().Contains('C') || bomb.GetSerialNumberLetters().Contains('D') || bomb.GetSerialNumberLetters().Contains('E'))
            planeVals[0]++;
        if (bomb.GetSerialNumberLetters().Contains('F') || bomb.GetSerialNumberLetters().Contains('G') || bomb.GetSerialNumberLetters().Contains('H') || bomb.GetSerialNumberLetters().Contains('I') || bomb.GetSerialNumberLetters().Contains('J'))
            planeVals[1]++;
        if (bomb.GetSerialNumberLetters().Contains('K') || bomb.GetSerialNumberLetters().Contains('L') || bomb.GetSerialNumberLetters().Contains('M') || bomb.GetSerialNumberLetters().Contains('N') || bomb.GetSerialNumberLetters().Contains('O'))
            planeVals[2]++;
        if (bomb.GetSerialNumberLetters().Contains('P') || bomb.GetSerialNumberLetters().Contains('Q') || bomb.GetSerialNumberLetters().Contains('R') || bomb.GetSerialNumberLetters().Contains('S') || bomb.GetSerialNumberLetters().Contains('T'))
            planeVals[3]++;
        if (bomb.GetModuleNames().Contains("European Travel"))
            planeVals[0]++;
        if (bomb.GetModuleNames().Contains("Lightspeed"))
            planeVals[1]++;
        if (bomb.GetModuleNames().Contains("Gridlock"))
            planeVals[2]++;
        if (bomb.GetModuleNames().Contains("Railway Cargo Loading"))
            planeVals[3]++;
        if (bomb.IsPortPresent(Port.DVI) || bomb.IsPortPresent(Port.PS2))
            planeVals[0]++;
        if (bomb.IsPortPresent(Port.Parallel) || bomb.IsPortPresent(Port.Serial))
            planeVals[1]++;
        if (bomb.IsPortPresent(Port.RJ45) || bomb.IsPortPresent(Port.StereoRCA))
            planeVals[2]++;
        if (bomb.GetIndicators().Count() > 2)
            planeVals[3]++;
        if (bomb.GetModuleNames().Count() < 10)
            planeVals[0]++;
        if (bomb.GetModuleNames().Count() >= 10 && bomb.GetModuleNames().Count() < 20)
            planeVals[1]++;
        if (bomb.GetModuleNames().Count() >= 20 && bomb.GetModuleNames().Count() < 30)
            planeVals[2]++;
        if (bomb.GetModuleNames().Count() >= 30)
            planeVals[3]++;
        if (dayOfWeek == "Monday" || dayOfWeek == "Tuesday")
            planeVals[0]++;
        if (dayOfWeek == "Wednesday" || dayOfWeek == "Thursday")
            planeVals[1]++;
        if (dayOfWeek == "Friday" || dayOfWeek == "Saturday")
            planeVals[2]++;
        if (dayOfWeek == "Sunday")
            planeVals[3]++;
        Debug.LogFormat("[Pocket Planes #{0}] The values of the planes are as follows:\nBearclaw-C = {1}\nKangaroo-P = {2}\nAirvan-M = {3}\nBirchcraft-M = {4}", moduleId, planeVals[0], planeVals[1], planeVals[2], planeVals[3]);
        List<int> largests = new List<int>();
        int ind = 5;
        while (largests.Count == 0)
        {
            for (int i = 0; i < 4; i++)
                if (planeVals[i] == ind)
                    largests.Add(i);
            if (largests.Count == 0)
                ind--;
        }
        if (largests.Count == 1)
        {
            correctPlane = largests[0];
            Debug.LogFormat("[Pocket Planes #{0}] Due to having the highest value, {1} is the correct plane", moduleId, planeNames[correctPlane]);
        }
        else
        {
            correctPlane = largests.Max();
            Debug.LogFormat("[Pocket Planes #{0}] Due to having the highest capacity as there was a tie for the largest value, {1} is the correct plane", moduleId, planeNames[correctPlane]);
        }
        Debug.LogFormat("[Pocket Planes #{0}] --Cargo/Passengers--", moduleId);
        List<string> dests = new List<string>();
        List<string> items = new List<string>();
        List<string> names = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            int rand = UnityEngine.Random.Range(0, 2);
            cargoTexts[i].text = rand == 0 ? destination : destinations[UnityEngine.Random.Range(0, destinations.Length)];
            dests.Add(cargoTexts[i].text);
            if (i == 0 || i == 3)
            {
                int temp2 = UnityEngine.Random.Range(0, passNames.Length);
                while (names.Contains(passNames[temp2]))
                    temp2 = UnityEngine.Random.Range(0, passNames.Length);
                names.Add(passNames[temp2]);
                cargoTexts[i + 4].text = passNames[temp2];
            }
            else
            {
                string item = itemNames[UnityEngine.Random.Range(0, itemNames.Length)];
                if (i == 2 && cargoTexts[1].text == cargoTexts[2].text)
                {
                    while (cargoTexts[5].text == item)
                        item = itemNames[UnityEngine.Random.Range(0, itemNames.Length)];
                }
                cargoTexts[i + 4].text = item;
            }
            items.Add(cargoTexts[i + 4].text);
        }
        Debug.LogFormat("[Pocket Planes #{0}] The displayed items are as follows:\nItem 0 - Passenger {1} going to {2}\nItem 1 - Cargo {3} going to {4}\nItem 2 - Cargo {5} going to {6}\nItem 3 - Passenger {7} going to {8}", moduleId, cargoTexts[4].text, cargoTexts[0].text, cargoTexts[5].text, cargoTexts[1].text, cargoTexts[6].text, cargoTexts[2].text, cargoTexts[7].text, cargoTexts[3].text);
        string alpha = bomb.GetSerialNumber()[0].ToString();
        int temp = 0;
        int x = 0;
        if (!int.TryParse(alpha, out temp))
        {
            x = mod(char.ToUpper(alpha.ToCharArray()[0]) - 64 + bomb.GetSerialNumberNumbers().Last() - (bomb.GetBatteryCount() * bomb.GetBatteryHolderCount()), 4);
            Debug.LogFormat("[Pocket Planes #{0}] The item that should be loaded with top priority is:\n((First Serial # Alphanumeric Val + Last Serial # Digit) - (Batteries * Battery Holders)) % 4 = (({1} + {2}) - ({3} * {4})) % 4 = {5}", moduleId, char.ToUpper(alpha.ToCharArray()[0]) - 64, bomb.GetSerialNumberNumbers().Last(), bomb.GetBatteryCount(), bomb.GetBatteryHolderCount(), x);
        }
        else
        {
            x = mod(temp + bomb.GetSerialNumberNumbers().Last() - (bomb.GetBatteryCount() * bomb.GetBatteryHolderCount()), 4);
            Debug.LogFormat("[Pocket Planes #{0}] The item that should be loaded with top priority is:\n((First Serial # Alphanumeric Val + Last Serial # Digit) - (Batteries * Battery Holders)) % 4 = (({1} + {2}) - ({3} * {4})) % 4 = {5}", moduleId, temp, bomb.GetSerialNumberNumbers().Last(), bomb.GetBatteryCount(), bomb.GetBatteryHolderCount(), x);
        }
        correctOrder.Add(x);
        for (int i = 0; i < 3; i++)
        {
            correctOrder.Add(mod(x + i + 1, 4));
        }
        Debug.LogFormat("[Pocket Planes #{0}] The initial determined load order of the items is: [{1}]", moduleId, correctOrder.Join(", "));
        for (int i = 0; i < correctOrder.Count; i++)
        {
            if (dests[correctOrder[i]] != destination)
            {
                correctOrder.Remove(correctOrder[i]);
                i--;
            }
        }
        if (correctOrder.Count == 0)
            Debug.LogFormat("[Pocket Planes #{0}] The new determined load order of the items after removing items that were not traveling to the destination city is: None", moduleId);
        else
            Debug.LogFormat("[Pocket Planes #{0}] The new determined load order of the items after removing items that were not traveling to the destination city is: [{1}]", moduleId, correctOrder.Join(", "));
        for (int i = 0; i < correctOrder.Count; i++)
        {
            if (items[correctOrder[i]][0] == bomb.GetSerialNumberLetters().ElementAt(1))
            {
                correctOrder.Remove(correctOrder[i]);
                i--;
            }
        }
        if (correctOrder.Count == 0)
            Debug.LogFormat("[Pocket Planes #{0}] The new determined load order of the items after removing items with a first letter that is the second letter of the Serial # is: None", moduleId);
        else
            Debug.LogFormat("[Pocket Planes #{0}] The new determined load order of the items after removing items with a first letter that is the second letter of the Serial # is: [{1}]", moduleId, correctOrder.Join(", "));
        int[] passAmts = new int[] { 0, 2, 2, 2 };
        int[] cargoAmts = new int[] { 1, 0, 1, 2 };
        int carg = 0;
        int pass = 0;
        for (int i = 0; i < correctOrder.Count; i++)
        {
            if (correctOrder[i] == 1 || correctOrder[i] == 2)
            {
                carg++;
                if (carg > cargoAmts[correctPlane])
                {
                    correctOrder.Remove(correctOrder[i]);
                    i--;
                }
            }
            else
            {
                pass++;
                if (pass > passAmts[correctPlane])
                {
                    correctOrder.Remove(correctOrder[i]);
                    i--;
                }
            }
        }
        if (correctOrder.Count == 0)
            Debug.LogFormat("[Pocket Planes #{0}] The new final determined load order of the items after removing items that cannot fit on the correct plane is: None", moduleId);
        else
            Debug.LogFormat("[Pocket Planes #{0}] The new final determined load order of the items after removing items that cannot fit on the correct plane is: [{1}]", moduleId, correctOrder.Join(", "));
        scenes[1].SetActive(false);
        scenes[2].SetActive(false);
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved != true)
        {
            if (stage == 0 && Array.IndexOf(buttons, pressed) >= 0 && Array.IndexOf(buttons, pressed) <= 2)
            {
                if (selectedBitizen != -1 && selectedBitizen == Array.IndexOf(buttons, pressed))
                    return;
                pressed.AddInteractionPunch(.5f);
                audio.PlaySoundAtTransform("click", pressed.transform);
                if (selectedBitizen != -1)
                    bitizenSelectors[selectedBitizen].SetActive(false);
                selectedBitizen = Array.IndexOf(buttons, pressed);
                bitizenSelectors[selectedBitizen].SetActive(true);
            }
            else if (stage == 0 && Array.IndexOf(buttons, pressed) == 3)
            {
                pressed.AddInteractionPunch(.5f);
                if (firstInter)
                {
                    firstInter = false;
                    Debug.LogFormat("[Pocket Planes #{0}] --Interactions--", moduleId);
                }
                if (selectedBitizen == correctBitizen)
                {
                    audio.PlaySoundAtTransform("stepcomplete", pressed.transform);
                    Debug.LogFormat("[Pocket Planes #{0}] Selected {1}, who mentioned the correct destination city. Advancing to step 2...", moduleId, bitizenNames[selectedBitizen]);
                    stage++;
                    scenes[0].SetActive(false);
                    scenes[1].SetActive(true);
                }
                else
                {
                    audio.PlaySoundAtTransform("click", pressed.transform);
                    Debug.LogFormat("[Pocket Planes #{0}] Selected {1}, who did not mention the correct destination city. Strike!", moduleId, selectedBitizen == -1 ? "None" : bitizenNames[selectedBitizen]);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else if (stage == 1 && Array.IndexOf(buttons, pressed) > 3 && Array.IndexOf(buttons, pressed) <= 7)
            {
                pressed.AddInteractionPunch(.5f);
                if (planeIndexes[Array.IndexOf(buttons, pressed) - 4] == correctPlane)
                {
                    audio.PlaySoundAtTransform("stepcomplete", pressed.transform);
                    Debug.LogFormat("[Pocket Planes #{0}] Clicked {1}, which is the correct plane. Advancing to step 3...", moduleId, planeNames[planeIndexes[Array.IndexOf(buttons, pressed) - 4]]);
                    stage++;
                    scenes[1].SetActive(false);
                    scenes[2].SetActive(true);
                }
                else
                {
                    audio.PlaySoundAtTransform("click", pressed.transform);
                    Debug.LogFormat("[Pocket Planes #{0}] Clicked {1}, which is not the correct plane. Strike!", moduleId, planeNames[planeIndexes[Array.IndexOf(buttons, pressed) - 4]]);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else if (stage == 2 && Array.IndexOf(buttons, pressed) == 8)
            {
                pressed.AddInteractionPunch(.5f);
                if (correctOrder.Count == orderSelected.Count)
                {
                    for (int i = 0; i < correctOrder.Count; i++)
                    {
                        if (correctOrder[i] != orderSelected[i])
                        {
                            audio.PlaySoundAtTransform("click", pressed.transform);
                            if (orderSelected.Count == 0)
                                Debug.LogFormat("[Pocket Planes #{0}] Loaded items in the order of None, which is incorrect. Strike!", moduleId);
                            else
                                Debug.LogFormat("[Pocket Planes #{0}] Loaded items in the order of [{1}], which is incorrect. Strike!", moduleId, orderSelected.Join(", "));
                            GetComponent<KMBombModule>().HandleStrike();
                            return;
                        }
                    }
                    audio.PlaySoundAtTransform("solve", pressed.transform);
                    if (orderSelected.Count == 0)
                        Debug.LogFormat("[Pocket Planes #{0}] Loaded items in the order of None, which is correct. Have a nice flight!", moduleId);
                    else
                        Debug.LogFormat("[Pocket Planes #{0}] Loaded items in the order of [{1}], which is correct. Have a nice flight!", moduleId, orderSelected.Join(", "));
                    scenes[2].SetActive(false);
                    DateTime curTime = DateTime.Now;
                    if (curTime.Hour >= 19 || curTime.Hour < 7)
                    {
                        backing.material = backgrounds[3];
                    }
                    else
                    {
                        backing.material = backgrounds[1];
                    }
                    moduleSolved = true;
                    GetComponent<KMBombModule>().HandlePass();
                }
                else
                {
                    audio.PlaySoundAtTransform("click", pressed.transform);
                    if (orderSelected.Count == 0)
                        Debug.LogFormat("[Pocket Planes #{0}] Loaded items in the order of None, which is incorrect. Strike!", moduleId);
                    else
                        Debug.LogFormat("[Pocket Planes #{0}] Loaded items in the order of [{1}], which is incorrect. Strike!", moduleId, orderSelected.Join(", "));
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else if (stage == 2 && Array.IndexOf(buttons, pressed) >= 9 && Array.IndexOf(buttons, pressed) <= 12)
            {
                pressed.AddInteractionPunch(.5f);
                audio.PlaySoundAtTransform("click", pressed.transform);
                if (!orderSelected.Contains(Array.IndexOf(buttons, pressed) - 9))
                {
                    orderSelected.Add(Array.IndexOf(buttons, pressed) - 9);
                    unloadButtons[Array.IndexOf(buttons, pressed) - 9].enabled = true;
                }
                else
                {
                    orderSelected.Remove(Array.IndexOf(buttons, pressed) - 9);
                    unloadButtons[Array.IndexOf(buttons, pressed) - 9].enabled = false;
                }
            }
        }
    }

    private int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} select <eugene/sam/glen> [Selects the specified Bitizen] | !{0} press <x/fly> [Presses the specified button] | !{0} plane <tl/tr/bl/br> [Presses the plane in the specified position] | !{0} load/unload <#> [Loads/Unloads the specified item '#' (optionally load/unload multiple items by putting a space between each item)] | Valid items are 0-3";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*select\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (stage != 0)
            {
                yield return "sendtochaterror A Bitizen can only be selected at step 1!";
                yield break;
            }
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (parameters[1].EqualsIgnoreCase("eugene"))
                {
                    if (selectedBitizen != 0)
                    {
                        buttons[0].OnInteract();
                    }
                    else
                    {
                        yield return "sendtochaterror Eugene has already been selected!";
                    }
                }
                else if (parameters[1].EqualsIgnoreCase("sam") || parameters[1].EqualsIgnoreCase("samantha"))
                {
                    if (selectedBitizen != 1)
                    {
                        buttons[1].OnInteract();
                    }
                    else
                    {
                        yield return "sendtochaterror Samantha has already been selected!";
                    }
                }
                else if (parameters[1].EqualsIgnoreCase("glen"))
                {
                    if (selectedBitizen != 2)
                    {
                        buttons[2].OnInteract();
                    }
                    else
                    {
                        yield return "sendtochaterror Glen has already been selected!";
                    }
                }
                else
                {
                    yield return "sendtochaterror The specified Bitizen to select '" + parameters[1] + "' is invalid!";
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify which Bitizen to select!";
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (parameters[1].EqualsIgnoreCase("x"))
                {
                    if (stage != 0)
                    {
                        yield return "sendtochaterror The X button can only be pressed at step 1!";
                        yield break;
                    }
                    buttons[3].OnInteract();
                }
                else if (parameters[1].EqualsIgnoreCase("fly"))
                {
                    if (stage != 2)
                    {
                        yield return "sendtochaterror The fly button can only be pressed at step 3!";
                        yield break;
                    }
                    buttons[8].OnInteract();
                }
                else
                {
                    yield return "sendtochaterror The specified button to press '" + parameters[1] + "' is invalid!";
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify what you would like to press!";
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*plane\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (stage != 1)
            {
                yield return "sendtochaterror A plane can only be pressed at step 2!";
                yield break;
            }
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (parameters[1].EqualsIgnoreCase("tl"))
                {
                    buttons[4].OnInteract();
                }
                else if (parameters[1].EqualsIgnoreCase("tr"))
                {
                    buttons[5].OnInteract();
                }
                else if (parameters[1].EqualsIgnoreCase("bl"))
                {
                    buttons[6].OnInteract();
                }
                else if (parameters[1].EqualsIgnoreCase("br"))
                {
                    buttons[7].OnInteract();
                }
                else
                {
                    yield return "sendtochaterror The specified position of the plane to press '" + parameters[1] + "' is invalid!";
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the position of the plane to press!";
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*load\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (stage != 2)
            {
                yield return "sendtochaterror Items can only be loaded at step 3!";
                yield break;
            }
            if (parameters.Length >= 2)
            {
                List<int> loads = new List<int>();
                for (int i = 1; i < parameters.Length; i++)
                {
                    int temp = 0;
                    if (!int.TryParse(parameters[i], out temp))
                    {
                        yield return "sendtochaterror The specified item to load '" + parameters[i] + "' is invalid!";
                        yield break;
                    }
                    if (temp < 0 || temp > 3)
                    {
                        yield return "sendtochaterror The specified item to load '" + parameters[i] + "' is out of range 0-3!";
                        yield break;
                    }
                    if (orderSelected.Contains(temp))
                    {
                        yield return "sendtochaterror The specified item to load '" + parameters[i] + "' has already been loaded!";
                        yield break;
                    }
                    if (loads.Contains(temp))
                    {
                        yield return "sendtochaterror The specified item to load '" + parameters[i] + "' cannot be loaded twice!";
                        yield break;
                    }
                    loads.Add(temp);
                }
                for (int i = 0; i < loads.Count; i++)
                {
                    buttons[loads[i] + 9].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the item(s) to load!";
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*unload\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (stage != 2)
            {
                yield return "sendtochaterror Items can only be unloaded at step 3!";
                yield break;
            }
            if (parameters.Length >= 2)
            {
                List<int> unloads = new List<int>();
                for (int i = 1; i < parameters.Length; i++)
                {
                    int temp = 0;
                    if (!int.TryParse(parameters[i], out temp))
                    {
                        yield return "sendtochaterror The specified item to unload '" + parameters[i] + "' is invalid!";
                        yield break;
                    }
                    if (temp < 0 || temp > 3)
                    {
                        yield return "sendtochaterror The specified item to unload '" + parameters[i] + "' is out of range 0-3!";
                        yield break;
                    }
                    if (!orderSelected.Contains(temp))
                    {
                        yield return "sendtochaterror The specified item to unload '" + parameters[i] + "' has already been unloaded!";
                        yield break;
                    }
                    if (unloads.Contains(temp))
                    {
                        yield return "sendtochaterror The specified item to unload '" + parameters[i] + "' cannot be unloaded twice!";
                        yield break;
                    }
                    unloads.Add(temp);
                }
                for (int i = 0; i < unloads.Count; i++)
                {
                    buttons[unloads[i] + 9].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the item(s) to unload!";
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (stage == 0)
        {
            if (selectedBitizen != correctBitizen)
            {
                buttons[correctBitizen].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            buttons[3].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        if (stage == 1)
        {
            buttons[Array.IndexOf(planeIndexes.ToArray(), correctPlane) + 4].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        if (stage == 2)
        {
            int end = orderSelected.Count;
            for (int i = 0; i < end; i++)
            {
                if (orderSelected[i] != correctOrder[i])
                {
                    for (int j = i; j < orderSelected.Count; j++)
                    {
                        buttons[orderSelected[j] + 9].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                }
            }
            int start = orderSelected.Count;
            int end2 = correctOrder.Count;
            for (int i = start; i < end2; i++)
            {
                buttons[correctOrder[i] + 9].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            buttons[8].OnInteract();
        }
    }
}
