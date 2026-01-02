public static class GameResult
{
    // ================= 游戏状态 =================

    public static bool gameEnded = false;

    // ================= 胜负 =================

    public static bool workerWin = false;
    public static bool bossWin = false;

    // ================= 数据统计 =================

    public static int destroyedMachines = 0;
    public static int totalCatches = 0;
    public static int resignedWorkers = 0;

    // ================= 重置 =================

    public static void Reset()
    {
        gameEnded = false;

        workerWin = false;
        bossWin = false;

        destroyedMachines = 0;
        totalCatches = 0;
        resignedWorkers = 0;
    }
}
