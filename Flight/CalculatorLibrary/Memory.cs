using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CalculatorLibrary;
public class Memory
{
    
    List<MemoryItem> memoryItems = new List<MemoryItem>();

    /// <summary>
    /// Тооны машины хадгалах ....
    /// </summary>
    public List<MemoryItem> MemoryItems
    {
        get
        {
        return memoryItems; 
        }
    }
    
    public bool Clear()
    {
        return true;
    }

    public void Save(byte saveValue)
    {
        MemoryItem memoryItem = new MemoryItem(saveValue);
        memoryItems.Add(memoryItem);
    }

    /// <summary>
    /// тоог харуулна
    /// </summary>
    /// <param name="loadValue">парам бөйбөйыб</param>
    /// <returns>нийт тоо нь буцна</returns>
    public int Load(byte loadValue)
    {
        return memoryItems.Count;
    }

}
