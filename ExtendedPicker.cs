using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace YourNameSpace
{
    public class ExtendedPicker:Picker
    {

        public ExtendedPicker()
        {
            SelectedIndexChanged += (s, o) =>
            {
                SetValue(SelectedItemValueProperty, SelectedItemValue);
                SetValue(SelectedValueProperty, SelectedValue);
            };
        }

        #region setup bindable Properties

        public static readonly BindableProperty DataSourceProperty = BindableProperty.Create(
            propertyName: "DataSource",
            returnType: typeof(System.Collections.IList),
            declaringType: typeof(ExtendedPicker),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (ExtendedPicker)bindable;
                control.DataSource = (System.Collections.IList)newValue;
            });

        public static readonly BindableProperty SelectedItemValueProperty = BindableProperty.Create(
            propertyName: "SelectedItemValue",
            returnType: typeof(object),
            declaringType: typeof(ExtendedPicker),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (ExtendedPicker)bindable;
                control.SelectedItemValue = newValue;
            });

        public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create(
            propertyName: "SelectedValue",
            returnType: typeof(object),
            declaringType: typeof(ExtendedPicker),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (ExtendedPicker)bindable;
                control.SelectedValue = newValue;
            });


        public static readonly BindableProperty DisplayMemberProperty = BindableProperty.Create(
            propertyName: "DisplayMember",
            returnType: typeof(string),
            declaringType: typeof(ExtendedPicker),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (ExtendedPicker)bindable;
                control.DisplayMember = (string)newValue;
            });

        public static readonly BindableProperty ValueMemberProperty = BindableProperty.Create(
            propertyName: "ValueMember",
            returnType: typeof(string),
            declaringType: typeof(ExtendedPicker),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (ExtendedPicker)bindable;
                control.ValueMember = (string)newValue;
            });

        #endregion


        public Dictionary<int, object> DataDictionary;
        public System.Collections.IList RealSource;
        
        private string _displayMember = null;
        public System.Collections.IList DataSource
        {
            get => RealSource;
            set
            {
                RealSource = value;
                var dd = new Dictionary<int, object>();
                for (var i = 0; i < value.Count; i++)
                {
                    dd.Add(i, value[i]);
                }
                DataDictionary = dd;
                ItemsSource = value;
            }
        }
        
        public object SelectedItemValue
        {
            get => SelectedItem;
            set
            {
                try
                {
                    var val = DataDictionary.FirstOrDefault(c => c.Value == value);
                    SelectedItem = val.Value;
                    SelectedIndex = val.Key;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public object SelectedValue
        {
            get => GetPropValue(SelectedItemValue, ValueMember);
            set
            {
                //if display member is null then binding property is member itself
                if (string.IsNullOrEmpty(ValueMember))
                {
                    SelectedItemValue = value;
                }
                else
                {
                    //find out if DataSource object has property
                    if (!DataDictionary.Any()) return;
                    var kvpValue = FindIndexByVal(RealSource, ValueMember, value);
                    if (kvpValue.Key < 0) return;
                    //set selected index and value
                    SelectedItem = kvpValue.Value;
                    SelectedIndex = kvpValue.Key;
                }

            }
        }


        public string DisplayMember
        {
            get => _displayMember;
            set
            {
                _displayMember = value;
                ItemDisplayBinding = new Binding(value);
            }
        }

        public string ValueMember { get; set; }

        #region  Helper Method

        private KeyValuePair<int,object> FindIndexByVal(System.Collections.IList collection, string propName,object value)
        {
            var ret=new KeyValuePair<int,object>(-1,null);
            //return if 0 members
            if (collection.Count == 0) return ret;

            //return if no property found
            var firstOb = collection[0];
            if (firstOb.GetType().GetProperties().All(c => c.Name != propName))
            {
                return ret;
            }

            for (var i = 0; i < collection.Count; i++)
            {
                var currentOb = collection[i];
                var propValue = GetPropValue(currentOb, propName);
                if(propValue==null)continue;
                //check if types are not equal 
                //Todo:here may be some problem with nullable types
                if (propValue.GetType() != value.GetType())
                {
                    //then we need convert
                    var convertedValue = Convert.ChangeType(value, propValue.GetType());
                    if (!propValue.Equals(convertedValue)) continue;
                }
                else
                {
                    if (!propValue.Equals(value)) continue;
                }

                //return value here
                return new KeyValuePair<int, object>(i, currentOb);
            }

            return ret;
        }
        //get property value
        private object GetPropValue(object ob, string propName)
        {
            if (ob == null) return null;
            var currentProp = ob.GetType().GetProperty(propName);
            return currentProp == null ? null : currentProp.GetValue(ob);
        }
        #endregion
    }
}
