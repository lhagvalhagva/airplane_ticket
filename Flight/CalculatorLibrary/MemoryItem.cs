using System;

namespace CalculatorLibrary;

public class MemoryItem:IArithmetic
{
    private byte savedValue;

    public byte SavedValue
    {

        get
        {
        return savedValue;
        }
    }

    public MemoryItem(byte savedValue)
    {
        this.savedValue = savedValue;
    }

    public void Add(byte value) => this.savedValue += value;
    public void Calc() => throw new NotImplementedException();
    public void Sub(byte value) => this.savedValue -= value;
}
