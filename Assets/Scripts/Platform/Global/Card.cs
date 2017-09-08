//储存手牌中的index 和牌面信息
public struct Card
{
    public int index;
    public int card;
    public Card(int index, int card)
    {
        this.index = index;
        this.card = card;
    }

    public static bool operator ==(Card left, Card right)
    {
        return left.card == right.card;
    }

    public static bool operator !=(Card left, Card right)
    {
        return left.card != right.card;
    }

    public static bool operator <(Card left, Card right)
    {
        return left.card < right.card;
    }

    public static bool operator >(Card left, Card right)
    {
        return left.card > right.card;
    }

    public static bool operator <=(Card left, Card right)
    {
        return left.card <= right.card;
    }

    public static bool operator >=(Card left, Card right)
    {
        return left.card >= right.card;
    }

    public override bool Equals(object obj)
    {
        return this.card.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.GetHashCode();
    }
}
