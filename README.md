# XamarinPickerExtended
Xamarin Forms Picker extended to bind more like win forms Combo box Does


**How it works**

Put ExtendedPicker.cs in your project and change namespace 

In Xaml Add


```xaml
xmlns:common="clr-namespace:YourNameSpace;assembly=YourAssemblyName"

```


Initialization From xaml
```xaml
            <common:ExtendedPicker 
                x:Name="ExtendedPicker1" 
                DataSource="{Binding YourDataSource}"
                DisplayMember="Name"
                ValueMember="Id"
                SelectedValue="{Binding SomeTableValueId}"
                ></common:ExtendedPicker>
```

Initialization From code
```cs
            ExtendedPicker1.DataSource = db.yourtable.ToList();
            ExtendedPicker1.DisplayMember = "Name";
            ExtendedPicker1.ValueMember = "Id";
            ExtendedPicker1.SelectedValue = 2;
```
