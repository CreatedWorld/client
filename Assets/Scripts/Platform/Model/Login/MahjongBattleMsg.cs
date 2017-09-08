//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: ProtoFile/MahjongBattle.proto
// Note: requires additional types generated from: ProtoFile/Battle.proto
namespace Platform.Model
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Core_GameStart_S2C")]
  public partial class Core_GameStart_S2C : global::ProtoBuf.IExtensible
  {
    public Core_GameStart_S2C() {}
    
    private int _bankerUserId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"bankerUserId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int bankerUserId
    {
      get { return _bankerUserId; }
      set { _bankerUserId = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _dices = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(2, Name=@"dices", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> dices
    {
      get { return _dices; }
    }
  
    private int _currentTimes;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"currentTimes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int currentTimes
    {
      get { return _currentTimes; }
      set { _currentTimes = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _handCards = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(4, Name=@"handCards", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> handCards
    {
      get { return _handCards; }
    }
  
    private readonly global::System.Collections.Generic.List<int> _flowerCards = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(5, Name=@"flowerCards", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> flowerCards
    {
      get { return _flowerCards; }
    }
  
    private int _touchMahjongCode;
    [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name=@"touchMahjongCode", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int touchMahjongCode
    {
      get { return _touchMahjongCode; }
      set { _touchMahjongCode = value; }
    }
    private int _leftCardCount;
    [global::ProtoBuf.ProtoMember(7, IsRequired = true, Name=@"leftCardCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int leftCardCount
    {
      get { return _leftCardCount; }
      set { _leftCardCount = value; }
    }
    private Platform.Model.PushPlayerActTipS2C _pushPlayerActTipS2C;
    [global::ProtoBuf.ProtoMember(8, IsRequired = true, Name=@"pushPlayerActTipS2C", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public Platform.Model.PushPlayerActTipS2C pushPlayerActTipS2C
    {
      get { return _pushPlayerActTipS2C; }
      set { _pushPlayerActTipS2C = value; }
    }
    private long _startTime = default(long);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"startTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long startTime
    {
      get { return _startTime; }
      set { _startTime = value; }
    }
    private int _treasureMotherCard = default(int);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"treasureMotherCard", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int treasureMotherCard
    {
      get { return _treasureMotherCard; }
      set { _treasureMotherCard = value; }
    }
    private int _treasureCard = default(int);
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"treasureCard", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int treasureCard
    {
      get { return _treasureCard; }
      set { _treasureCard = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SetCardC2S")]
  public partial class SetCardC2S : global::ProtoBuf.IExtensible
  {
    public SetCardC2S() {}
    
    private readonly global::System.Collections.Generic.List<Platform.Model.PlayerCardSet> _cardSets = new global::System.Collections.Generic.List<Platform.Model.PlayerCardSet>();
    [global::ProtoBuf.ProtoMember(1, Name=@"cardSets", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Platform.Model.PlayerCardSet> cardSets
    {
      get { return _cardSets; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PlayerCardSet")]
  public partial class PlayerCardSet : global::ProtoBuf.IExtensible
  {
    public PlayerCardSet() {}
    
    private int _sit;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"sit", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int sit
    {
      get { return _sit; }
      set { _sit = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _handCards = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(2, Name=@"handCards", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> handCards
    {
      get { return _handCards; }
    }
  
    private int _getCard = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"getCard", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int getCard
    {
      get { return _getCard; }
      set { _getCard = value; }
    }
    private int _nextGetCard = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"nextGetCard", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int nextGetCard
    {
      get { return _nextGetCard; }
      set { _nextGetCard = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SetCardS2C")]
  public partial class SetCardS2C : global::ProtoBuf.IExtensible
  {
    public SetCardS2C() {}
    
    private readonly global::System.Collections.Generic.List<Platform.Model.PlayerCardSet> _cardSets = new global::System.Collections.Generic.List<Platform.Model.PlayerCardSet>();
    [global::ProtoBuf.ProtoMember(1, Name=@"cardSets", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Platform.Model.PlayerCardSet> cardSets
    {
      get { return _cardSets; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetAllCardC2S")]
  public partial class GetAllCardC2S : global::ProtoBuf.IExtensible
  {
    public GetAllCardC2S() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetAllCardS2C")]
  public partial class GetAllCardS2C : global::ProtoBuf.IExtensible
  {
    public GetAllCardS2C() {}
    
    private readonly global::System.Collections.Generic.List<int> _remainder = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(1, Name=@"remainder", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> remainder
    {
      get { return _remainder; }
    }
  
    private readonly global::System.Collections.Generic.List<Platform.Model.PlayerCardSet> _cardSets = new global::System.Collections.Generic.List<Platform.Model.PlayerCardSet>();
    [global::ProtoBuf.ProtoMember(2, Name=@"cardSets", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Platform.Model.PlayerCardSet> cardSets
    {
      get { return _cardSets; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}