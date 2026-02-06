using ByteBurrow;

namespace BBTests;

public class TestSaveable : SaveData
{
    [SaveField(0, "1.0.0", "1.0.0")] public string Username = "";
    [SaveField(1, "1.0.0", "1.0.0")] public int Coins = 0;
    [SaveField(2, "0.9.0", "0.9.0")] public int NeverSaving = 0;

    protected override string Prefix => "TEST\0SV";
    protected override string Version => "1.0.0";
}