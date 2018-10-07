using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStack : ScriptableObject {

    List<HexCell> cells = new List<HexCell>();
    public HexCoordinates coordinates;

    public bool initialized = false;

    public void init(TerrainChunk chunk, HexCoordinates coords) {
        coordinates = coords;
        initialized = true;
    }

    public int Count() {
        return cells.Count;
    }

    public HexCell Peek() {
        return cells[Count() - 1];
    }

    public HexCell Pop() {
        HexCell top = Peek();
        cells.RemoveAt(Count() - 1);
        return top;
    }

    public void Push(HexCell cell) {
        cell.cellStack = this;
        cell.elevation = Count();
        cells.Add(cell);
    }

    public HexCell PeekAt(int index) {
        return cells[index];
    }

    public bool CanPop() {
        return Count() > 0 && !(Peek().GetType() == typeof(BedrockCell));
    }
}
