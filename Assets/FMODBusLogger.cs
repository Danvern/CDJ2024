using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODBusLogger : MonoBehaviour
{
    void Start()
    {
        OutputAllBusesToConsole();
    }

    private void OutputAllBusesToConsole()
    {
        Bank[] banks = new Bank[0];
        RuntimeManager.StudioSystem.getBankList(out banks);

        foreach (var bank in banks)
        {
            string bankPath;
            bank.getPath(out bankPath);

            Debug.Log("Loading Bank: " + bankPath);

            // Load the bank if not already loaded
            bank.loadSampleData();

            // Get the buses from this bank
            var buses = new Bus[0];
            bank.getBusList(out buses);

            foreach (var bus in buses)
            {
                string busPath;
                bus.getPath(out busPath);
                Debug.Log("Bus Path: " + busPath);
            }
        }
    }
}
