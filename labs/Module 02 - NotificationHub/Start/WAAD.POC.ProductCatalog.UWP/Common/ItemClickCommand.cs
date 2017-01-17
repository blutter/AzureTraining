﻿using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WAAD.POC.ProductCatalog.UWP.Common
{
    public static class ItemClickCommand
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
                typeof(ItemClickCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var listView = dependencyObject as ListViewBase;
                if (listView != null)
                {
                    //listView.ItemClick += ListView_ItemClick;
                    listView.ItemClick += delegate(object sender, ItemClickEventArgs itemClickEventArgs)
                    {
                        var viewBase = sender as ListViewBase;
                        var command = GetCommand(viewBase);

                        if (command != null && command.CanExecute(itemClickEventArgs.ClickedItem))
                            command.Execute(itemClickEventArgs.ClickedItem);
                    };
                }
            }
            catch (Exception)
            {

            }
        }

    }
}