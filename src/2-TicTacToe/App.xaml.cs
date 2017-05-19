/******************************************************************************
Module:  App.xaml.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/

using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App {
   sealed partial class App : Application {
      public App() {
         this.InitializeComponent();
      }

      protected override void OnLaunched(LaunchActivatedEventArgs args) {
         Frame rootFrame = Window.Current.Content as Frame;

         if (rootFrame == null) {
            rootFrame = new Frame();
            Window.Current.Content = rootFrame;
         }

         if (rootFrame.Content == null) {
            if (!rootFrame.Navigate(typeof(MainPage), args.Arguments)) {
               throw new Exception("Failed to create initial page");
            }
         }
         Window.Current.Activate();
      }
   }
}
