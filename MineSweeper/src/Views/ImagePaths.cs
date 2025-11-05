using System.Runtime.ConstrainedExecution;
using Tizen.Multimedia;

public static class ImagePaths
{
    public static string CELL_CLOSE = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_close.png";

    public static string CELL_MINE = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_mine.png";

    public static string CELL_FLAG = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_flag.png";

    public static string CELL_MINEX = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_mineX.png";

    public static string CELL_0 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_0.png";

    public static string CELL_1 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_1.png";

    public static string CELL_2 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_2.png";

    public static string CELL_3 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_3.png";

    public static string CELL_4 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_4.png";

    public static string CELL_5 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_5.png";

    public static string CELL_6 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_6.png";

    public static string CELL_7 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_7.png";

    public static string CELL_8 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_8.png";

    public static string CELL_SMILE = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_smile.png";

    public static string CELL_DEAD = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_dead.png";

    public static string CELL_SUNGLASS = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_sunglass.png";

    public static string InfoBoard = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/info_board.png";

    public static string UnderBoard = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/under_board.png";

    public static string GetCellImage(string type)
    {
        return Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/cell_" + type + ".png";
    }

    public static string GetCellImage(int type)
    {
        return Tizen.Applications.Application.Current.DirectoryInfo.Resource + $"images/cell_{type}.png";
    }

    public static string GetNumberImage(int type)
    {
        return Tizen.Applications.Application.Current.DirectoryInfo.Resource + $"images/number_{type}.png";
    }
    
    public static string GetImage(string name)
    {
        return Tizen.Applications.Application.Current.DirectoryInfo.Resource + $"images/{name}.png";
    }
}