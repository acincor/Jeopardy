using System;

public static class MachineEvents
{
    public static Action<Machine> OnMachineDestroyed;

    public static void RaiseMachineDestroyed(Machine machine)
    {
        OnMachineDestroyed?.Invoke(machine);
    }
}
