//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: ProtoFile/Login.proto
namespace Platform.Model
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"LoginC2S")]
  public partial class LoginC2S : global::ProtoBuf.IExtensible
  {
    public LoginC2S() {}
    
    private string _mac;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"mac", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string mac
    {
      get { return _mac; }
      set { _mac = value; }
    }
    private string _name;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private string _psw;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"psw", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string psw
    {
      get { return _psw; }
      set { _psw = value; }
    }
    private string _imageUrl;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"imageUrl", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string imageUrl
    {
      get { return _imageUrl; }
      set { _imageUrl = value; }
    }
    private int _sex;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"sex", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int sex
    {
      get { return _sex; }
      set { _sex = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"LoginS2C")]
  public partial class LoginS2C : global::ProtoBuf.IExtensible
  {
    public LoginS2C() {}
    
    private int _userId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }
    private int _status;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private string _serverIp;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"serverIp", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string serverIp
    {
      get { return _serverIp; }
      set { _serverIp = value; }
    }
    private int _port;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"port", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int port
    {
      get { return _port; }
      set { _port = value; }
    }
    private long _time;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public long time
    {
      get { return _time; }
      set { _time = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}