using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AnnouncementData
{
    private string content;
    private int cirCount;

    public string Content
    {
        get
        {
            return content;
        }
    }

    public int CirCount
    {
        get
        {
            return cirCount;
        }
    }

    public AnnouncementData(string content, int cirCount)
    {
        this.content = content;
        this.cirCount = cirCount;
    }

    public int ReduceCirCount()
    {
        return cirCount = 1;//cirCount -= 1;
    }
}
