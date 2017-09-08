using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform.Model.VO.BattleVO
{
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"ActionVO")]
    public partial class ActionVO : global::ProtoBuf.IExtensible
    {
        public ActionVO() { }

        private bool _isActionTip;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"isActionTip", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public bool isActionTip
        {
            get { return _isActionTip; }
            set { _isActionTip = value; }
        }
        private long _actionTime;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"actionTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public long actionTime
        {
            get { return _actionTime; }
            set { _actionTime = value; }
        }
        private Platform.Model.PushPlayerActTipS2C _actTip = null;
        [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"actTip", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(null)]
        public Platform.Model.PushPlayerActTipS2C actTip
        {
            get { return _actTip; }
            set { _actTip = value; }
        }
        private Platform.Model.PushPlayerActS2C _act = null;
        [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"act", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(null)]
        public Platform.Model.PushPlayerActS2C act
        {
            get { return _act; }
            set { _act = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }
}
