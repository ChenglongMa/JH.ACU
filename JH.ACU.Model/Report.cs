/* ==============================================================================
 * 功能描述：Report  
 * 创 建 者：Administrator
 * 创建日期：2017/3/23 15:07:16
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using JH.ACU.Model.Annotations;
using JH.ACU.Model.Config.TestConfig;


namespace JH.ACU.Model
{
    /// <summary>
    /// 需要报告给UI层的信息
    /// </summary>
    [Serializable]
    public class Report : INotifyPropertyChanged//,ICloneable
    {
        #region 构造函数

        public Report()
        {
            ValueDict = new Dictionary<string, object>();
            SpecUnitsDict = new Dictionary<TvType, List<SpecItem>>();
        }

        #endregion

        #region 属性字段
        private int _acuIndex;
        /// <summary>
        /// 正在测试ACU索引
        /// </summary>
        public int AcuIndex
        {
            get { return _acuIndex; }
            set
            {
                _acuIndex = value;
                OnPropertyChanged("AcuIndex");
            }
        }
        private string _acuName;
        /// <summary>
        /// 正在测试ACU ID信息
        /// </summary>
        public string AcuName
        {
            get { return _acuName; }
            set
            {
                _acuName = value;
                OnPropertyChanged("AcuName");
            }
        }

        private bool _chamberEnable;
        /// <summary>
        /// 温箱是否可用
        /// </summary>
        public bool ChamberEnable
        {
            get { return _chamberEnable; }
            set
            {
                _chamberEnable = value;
                OnPropertyChanged("ChamberEnable");
            }
        }
        private double _actualTemp;

        /// <summary>
        /// 温度实际值
        /// </summary>
        public double ActualTemp
        {
            get { return _actualTemp; }
            set
            {
                _actualTemp = value;
                OnPropertyChanged("ActualTemp");
            }
        }

        private double _actualVolt;

        /// <summary>
        /// 电压实际值
        /// </summary>
        public double ActualVolt
        {
            get { return _actualVolt; }
            set
            {
                _actualVolt = value;
                OnPropertyChanged("ActualVolt");
            }


        }

        private double _settingTemp;

        /// <summary>
        /// 温度设定值
        /// </summary>
        public double SettingTemp
        {
            get { return _settingTemp; }
            set
            {
                _settingTemp = value;
                OnPropertyChanged("SettingTemp");
            }
        }

        private double _settingVolt;

        /// <summary>
        /// 电压设定值
        /// </summary>
        public double SettingVolt
        {
            get { return _settingVolt; }
            set
            {
                _settingVolt = value;
                OnPropertyChanged("SettingVolt");
            }
        }

        private string _message;

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private double _progress;

        /// <summary>
        /// 总进度
        /// </summary>
        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        private Dictionary<TvType, List<SpecItem>> _specUnitsDict;

        /// <summary>
        /// 所有测试项
        /// </summary>
        public Dictionary<TvType, List<SpecItem>> SpecUnitsDict
        {
            get { return _specUnitsDict; }
            set
            {
                _specUnitsDict = value;
                OnPropertyChanged("SpecUnitsDict");
            }
        }

        private Dictionary<string, object> _vauleDict;

        /// <summary>
        /// 扩展属性
        /// </summary>
        public Dictionary<string, object> ValueDict
        {
            get { return _vauleDict; }
            set
            {
                _vauleDict = value;
                OnPropertyChanged("ValueDict");
            }
        }

        #endregion

        #region UI通知事件

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public static T Copy<T>(T realObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制     
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, realObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        public void DeepCopy(Report obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties.Where(p => p.CanRead && p.CanWrite))
            {
                property.SetValue(this, property.GetValue(obj, null), null);
            }
        }
    }
}