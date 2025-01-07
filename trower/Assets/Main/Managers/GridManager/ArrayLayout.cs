[System.Serializable]
public class ArrayLayout
{


    [System.Serializable]
    public struct rowData
    {
        public bool[] row;
    }


    public rowData[] rows = new rowData[0];
    public int xBounds = 4;
    public int yBounds = 4;
}
