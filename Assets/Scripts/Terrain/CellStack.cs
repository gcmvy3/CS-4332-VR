using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStack : ScriptableObject {

    List<CellType> cells = new List<CellType>();
    public HexCoordinates coordinates;

    public bool initialized = false;

    public void init(TerrainChunk chunk, HexCoordinates coords) {
        coordinates = coords;
        initialized = true;
    }

    public int Count() {
        return cells.Count;
    }

    public CellType Peek() {
        return cells[Count() - 1];
    }

    public CellType Pop() {
        CellType top = Peek();
        cells.RemoveAt(Count() - 1);
        return top;
    }

    public void Push(CellType cell) {
        cells.Add(cell);
    }

    public CellType PeekAt(int index) {
        return cells[index];
    }

    public bool CanPop() {
        return Count() > 0 && Peek() != CellType.Bedrock;
    }
}
