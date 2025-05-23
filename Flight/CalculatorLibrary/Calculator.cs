using System;

namespace CalculatorLibrary;
public class Calculator : CalculatorBase, IArithmetic
{
    private Memory memory;
    public Memory MemoryBank
    {
        get
        {
            return memory;
        }
    }
    public byte Result
    {
        get
        {
            return result;
        }

    }

    private byte result;
    private byte val2;
    private bool op;

    public Calculator()
    {
        this.result = 0;
        this.val2 = 0;
        this.op = false;
        memory = new Memory();
    }

    public Calculator(byte result)
    {
        this.result = result;
    }

    //public void Add(byte value)
    //{
    //    op = true;
    //}

    public void Sub(byte value)
    {
        op = false;
        val2 = value;
    }

    public void Calc()
    {
        if (op)
            this.result += val2;
        else
            this.result -= val2;
    }


    public void MS()
    {
        memory.Save(this.result);
    }
    /// <summary>
    /// нэмэх үйлдэл
    /// </summary>
    /// <param name="value">Нэмэгдүүлэх тоо</param>
    public void Add(byte value)
    {
        this.op = true;
        this.val2 = value;
        this.result = 0;
    }

}
